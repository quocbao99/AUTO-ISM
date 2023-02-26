using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Interface;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using System.IO;
using Models;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Hangfire;
using Newtonsoft.Json.Linq;
using static Utilities.CoreContants;
using System.Net;
using Microsoft.AspNetCore.Http;
using PayPal.Api;
using Stripe;
using Stripe.Checkout;
using System.Text.Json;
using Interface.Services.Specializing;
using Extensions;
using Interface.Services.Configuration;

namespace Service.Services
{
    public class PaymentByStripeService : IPaymentByStripeService
    {
        private IAppDbContext coreDbContext;
        private IPaymentService paymentService;
        private IPaymentMethodConfigurationService PaymentMethodConfigurationService;
        private IPaymentMethodTypeService PaymentMethodTypeService;
        private IOrderService orderService; 
        private IContractSpecializingService contractSpecializingService;
        private IHangFireManageSpecializingService hangFireManageSpecializingService;
        private IPaymentSessionService paymentSessionService;
        private IUserService userService;
        private IEmailConfigurationService emailConfigurationService;

        public PaymentByStripeService(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IPaymentService paymentService,
            IPaymentMethodConfigurationService PaymentMethodConfigurationService,
            IPaymentMethodTypeService PaymentMethodTypeService,
            IContractSpecializingService contractSpecializingService,
            IOrderService orderService,
            IHangFireManageSpecializingService hangFireManageSpecializingService,
            IPaymentSessionService paymentSessionService,
            IUserService userService,
            IEmailConfigurationService emailConfigurationService,
            IAppDbContext coreDbContext
            )
        {
            this.coreDbContext = coreDbContext;
            this.paymentService = paymentService;
            this.PaymentMethodConfigurationService = PaymentMethodConfigurationService;
            this.PaymentMethodTypeService = PaymentMethodTypeService;
            this.contractSpecializingService = contractSpecializingService;
            this.hangFireManageSpecializingService = hangFireManageSpecializingService;
            this.orderService = orderService;
            this.paymentSessionService = paymentSessionService;
            this.userService = userService;
            this.emailConfigurationService = emailConfigurationService;
        }


        #region thanh toán thường
        public async Task<string> PaymentByStripe(Entities.Payment paymentPaypal)
        {
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                try
                {
                    // phương thức than toán
                    var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)paymentPaypal.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                    if (paymentMethodType == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Stripe && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.StripeSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Momo");
                    // tạo phiếu thanh toán
                    paymentPaypal.Status = (int)PaymentStatus.Waiting;
                    paymentPaypal.Created = Timestamp.Now();
                    await paymentService.CreateAsync(paymentPaypal);

                    // phương thức thanh toán trả ra url
                    StripeConfiguration.ApiKey = paymentMethodConfig.serectkey;
                    if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Stripe) { 
                        var options = new SessionCreateOptions
                        {
                            LineItems = new List<SessionLineItemOptions>
                            {
                              new SessionLineItemOptions
                              {
                                PriceData = new SessionLineItemPriceDataOptions
                                {
                                  UnitAmount = 2000,
                                  Currency = "usd",
                                  ProductData = new SessionLineItemPriceDataProductDataOptions
                                  {
                                    Name = "T-shirt",
                                  },
                                },
                                Quantity = 1,
                              },
                            },
                            Mode = "payment",
                            SuccessUrl = $"{paymentMethodConfig.returnUrl}?order_id={paymentPaypal.Id}&success=1",
                            CancelUrl = $"{paymentMethodConfig.returnUrl}?order_id={paymentPaypal.Id}&success=0",
                        };

                        var service = new SessionService();
                        Session session = service.Create(options);
                        await transaction.CommitAsync();

                        return session.Url;
                    }
                    if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.StripeSubscription) {

                        

                        var options = new SessionCreateOptions
                        {
                            LineItems = new List<SessionLineItemOptions>{
                                   new SessionLineItemOptions{
                                    Price = "price_1MR8iVAAXViBjCVsSq9ZGz4r",
                                    Quantity = 1,
                                    },
                            },
                            Mode = "subscription",
                            SuccessUrl = $"{paymentMethodConfig.returnUrl}?order_id={paymentPaypal.Id}&success=1",
                            CancelUrl = $"{paymentMethodConfig.returnUrl}?order_id={paymentPaypal.Id}&success=0",
                        };
                        var service = new SessionService();
                        Session session = service.Create(options);

                       
                        return session.Url;

                    }
                    return null;

                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message.ToString());
                }
            }

        }

        
        public async Task<PaymentPaypalResponseModel> PaymentByStripeResult(IQueryCollection collections)
        {
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                try
                {
                    var response = new PaymentPaypalResponseModel();

                    foreach (var (key, value) in collections)
                    {
                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("order_description"))
                        {
                            response.OrderDescription = value;
                        }

                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("transaction_id"))
                        {
                            response.TransactionId = value;
                        }

                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("order_id"))
                        {
                            response.OrderId = value;
                        }

                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("payment_method"))
                        {
                            response.PaymentMethod = value;
                        }

                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("success"))
                        {
                            response.Success = Convert.ToInt32(value) > 0;
                        }

                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("paymentid"))
                        {
                            response.PaymentId = value;
                        }

                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("session_id")) // subscription_id  chưa sài dc nên sài sessionid
                        {
                            response.StripeSubscriptionID = value;
                        }

                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("session_id"))
                        {
                            response.StripeSessionID = value;
                        }

                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("payerid"))
                        {
                            response.PayerId = value;
                        }
                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("token"))
                        {
                            response.Token = value;
                        }
                    }


                    // phương thức than toán
                    var payments = await paymentService.GetAsync(d => d.Id.ToString() == response.OrderId.ToString() && d.Deleted == false && d.Active == true); ;
                    if (payments.Count() != 1) throw new Exception("Phiếu thanh toán nhiều hơn 1 phiếu");
                    var payment = payments.FirstOrDefault();
                    var order = await orderService.GetByIdAsync((Guid)payment.OrderID);

                    if (order == null) throw new Exception("Không tìm thấy đơn hàng");
                    // phương thức than toán
                    var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                    if (paymentMethodType == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Stripe && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.StripeSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Momo");

                    // Tạo phiên than toán mới
                    PaymentSession paymentSession = new PaymentSession();
                    paymentSession.Amount = payment.AmountMoney;
                    paymentSession.PaymentID = payment.Id;
                    paymentSession.Active = true;
                    paymentSession.Deleted = false;
                    paymentSession.Created = Timestamp.Now();
                    // approve phiếu thanh toán đó

                    if (response.Success == true)
                    {
                        //Thanh toan thanh cong
                        if (!String.Empty.Equals(response.StripeSessionID))
                        { payment.StripeSubscriptionID = response.StripeSubscriptionID; }
                        else {
                            payment.StripeSubscriptionID = payment.StripeSubscriptionID;
                        }
                        paymentSession.Status = (int)PaymentSessionStatus.Success;
                        payment.StatusForSubScription = (int)PaymentStatusForSubScription.Success;
                        await paymentService.UpdateFieldAsync(payment, d => d.StatusForSubScription, d=>d.StripeSubscriptionID);

                        // kết thúc hợp đồng cũ nếu có
                        Package package = JsonSerializer.Deserialize<Package>(order.PackageInfo);
                        var contract = await contractSpecializingService.ContractsIsUsing((Guid)payment.UserID, package.PackageType);
                        if (contract != null)
                        {
                            await contractSpecializingService.EndedContract(contract.Id);
                        }

                        // thanh toán thành công => tạo hợp đồng mới
                        var newContract = await contractSpecializingService.NewContractFromPayment(payment);
                        // tạo hangfire hết hạn hợp đồng
                        var rs = await hangFireManageSpecializingService.GenerateJobDelayForContractExpried(newContract.Id);
                        if (rs == false) throw new MyException("Lỗi hệ thống!", HttpStatusCode.InternalServerError);

                        // gửi mã OTP qua mail
                        var user = await userService.GetByIdAsync(order.UserID);
                        string Tos = user.Email.Equals(String.Empty) ? user.Username : user.Email;
                        if (String.Empty.Contains(Tos)) throw new Exception("Lỗi không thể gửi mail");
                        string[] CCs = { };
                        string[] BCCs = { };

                        // tạo luồng gửi mail
                        //Thread SendMail = new Thread(() => { ThreadSendMail(Tos, CCs, BCCs, oTP); });
                        await emailConfigurationService.SendMail("AUTO-ISM thông báo", Tos, CCs, BCCs, new EmailContent { isHtml = true, content = $"Thanh toán thành công đơn hàng mã: {order.OrderCode}", attachments = null });

                    }
                    else
                    {
                        paymentSession.Status = (int)PaymentSessionStatus.Fail;
                        //Thanh toan khong thanh cong.
                        payment.Status = (int)PaymentStatusForSubScription.Fail;
                        await paymentService.UpdateFieldAsync(payment, d => d.StatusForSubScription);

                        // gửi mã OTP qua mail
                        var user = await userService.GetByIdAsync(order.UserID);
                        string Tos = user.Email.Equals(String.Empty) ? user.Username : user.Email;
                        if (String.Empty.Contains(Tos)) throw new Exception("Lỗi không thể gửi mail");
                        string[] CCs = { };
                        string[] BCCs = { };

                        // tạo luồng gửi mail
                        //Thread SendMail = new Thread(() => { ThreadSendMail(Tos, CCs, BCCs, oTP); });
                        await emailConfigurationService.SendMail("AUTO-ISM thông báo", Tos, CCs, BCCs, new EmailContent { isHtml = true, content = $"Thanh toán thất bại đơn hàng mã: {order.OrderCode}", attachments = null });

                    }
                    await paymentSessionService.CreateAsync(paymentSession);
                    await transaction.CommitAsync();
                    return response;
                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message.ToString());
                }
            }

        }
        #endregion

        #region thanh toán subscriptions
        // tạo sản phẩm trên stripe

        public async Task<string> PaymentByStripeSubscription(Entities.Payment paymentPaypal, Package package)
        {
            
                // phương thức than toán
                var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)paymentPaypal.PaymentMethodConfigurationID);
                if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                if (paymentMethodType == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Stripe && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.StripeSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Stripe");
                // tạo phiếu thanh toán
                //paymentPaypal.Status = (int)PaymentStatus.Waiting;
                paymentPaypal.Status = (int)PaymentStatusForSubScription.Waiting;
                paymentPaypal.Created = Timestamp.Now();
                await paymentService.CreateAsync(paymentPaypal);

                // phương thức thanh toán trả ra url
                StripeConfiguration.ApiKey = paymentMethodConfig.serectkey;
                //if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Stripe)
                //{
                //    var options = new SessionCreateOptions
                //    {
                //        LineItems = new List<SessionLineItemOptions>
                //        {
                //            new SessionLineItemOptions
                //            {
                //            PriceData = new SessionLineItemPriceDataOptions
                //            {
                //                UnitAmount = 2000,
                //                Currency = "usd",
                //                ProductData = new SessionLineItemPriceDataProductDataOptions
                //                {
                //                Name = "T-shirt",
                //                },
                //            },
                //            Quantity = 1,
                //            },
                //        },
                //        Mode = "payment",
                //        SuccessUrl = $"{paymentMethodConfig.returnUrl}?order_id={paymentPaypal.Id}&success=1",
                //        CancelUrl = $"{paymentMethodConfig.returnUrl}?order_id={paymentPaypal.Id}&success=0",
                //    };

                //    var service = new SessionService();
                //    Session session = service.Create(options);

                //    return session.Url;
                //}
                if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.StripeSubscription)
                {

                    var options = new SessionCreateOptions
                    {
                        LineItems = new List<SessionLineItemOptions>{
                                new SessionLineItemOptions{
                                Price = package.PriceStripeID,
                                Quantity = 1,
                                },
                        },
                        Mode = "subscription",
                        SuccessUrl = $"{paymentMethodConfig.returnUrl}?order_id={paymentPaypal.Id}&success=1"+ "&session_id={CHECKOUT_SESSION_ID}&subscription_id={SUBSCRIPTION_ID}",
                        CancelUrl = $"{paymentMethodConfig.returnUrl}?order_id={paymentPaypal.Id}&success=0",
                    };
                    var service = new SessionService();
                    Session session = service.Create(options);
                    
                    return session.Url;
                }
                return null;
                

        }


        #endregion
    }
}
