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
using PayPal;
using Extensions;
using System.Text.Json;
using Interface.Services.Specializing;
using Interface.Services.Configuration;

namespace Service.Services
{
    public class PaymentByPaypalService : IPaymentByPaypalService
    {
        private IAppDbContext coreDbContext;
        private IOrderService orderService;
        private IPaymentService paymentService;
        private IPaymentSessionService paymentSessionService;
        private IPaymentMethodConfigurationService PaymentMethodConfigurationService;
        private IPaymentMethodTypeService PaymentMethodTypeService;
        private IContractSpecializingService contractSpecializingService;
        private IHangFireManageSpecializingService hangFireManageSpecializingService;
        private IUserService userService;
        private IEmailConfigurationService emailConfigurationService;
        public PaymentByPaypalService(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IPaymentService paymentService,
            IPaymentSessionService paymentSessionService,
            IPaymentMethodConfigurationService PaymentMethodConfigurationService,
            IPaymentMethodTypeService PaymentMethodTypeService,
            IContractSpecializingService contractSpecializingService,
            IOrderService orderService,
            IHangFireManageSpecializingService hangFireManageSpecializingService,
            IUserService userService,
            IEmailConfigurationService emailConfigurationService,
            IAppDbContext coreDbContext
            ) 
        {
            this.orderService = orderService;
            this.coreDbContext = coreDbContext;
            this.paymentService = paymentService;
            this.paymentSessionService = paymentSessionService;
            this.PaymentMethodConfigurationService = PaymentMethodConfigurationService;
            this.PaymentMethodTypeService = PaymentMethodTypeService;
            this.contractSpecializingService = contractSpecializingService;
            this.hangFireManageSpecializingService = hangFireManageSpecializingService;
            this.userService = userService;
            this.emailConfigurationService = emailConfigurationService;
        }


        #region than toán thường
        public async Task<string> PaymentByPaypal(Entities.Payment paymentPaypal)
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
                    if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Paypal && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.PaypalSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Momo");

                    // tạo phiếu thanh toán
                    paymentPaypal.OrderID = Guid.NewGuid();
                    paymentPaypal.Status = (int)PaymentStatus.Waiting;
                    paymentPaypal.Created = Timestamp.Now();
                    await paymentService.CreateAsync(paymentPaypal);

                    APIContext apiContext = new APIContext(new OAuthTokenCredential(paymentMethodConfig.partnerCode, paymentMethodConfig.serectkey, PayPal.Api.ConfigManager.Instance.GetProperties()).GetAccessToken()) { 
                        Config = PayPal.Api.ConfigManager.Instance.GetProperties()
                    };

                    //create itemlist and add item objects to it
                    var itemList = new ItemList() { items = new List<Item>() };

                    //Adding Item Details like name, currency, price etc
                    //foreach (var item in paypalOrder.PaypalItemList)
                    //{
                        itemList.items.Add(new Item()
                        {
                            name = "ahhihihi",
                            currency = "USD",
                            price = paymentPaypal.AmountMoney.ToString(),
                            quantity = "1",
                            sku = "sku"
                        });
                    //}

                    var payer = new Payer() { payment_method = "paypal" };

                    // Configure Redirect Urls here with RedirectUrls object
                    var redirUrls = new RedirectUrls()
                    {
                        cancel_url = $"{paymentMethodConfig.returnUrl}?payment_method=PayPal&success=1&order_id={paymentPaypal.Id}",
                        return_url = $"{paymentMethodConfig.returnUrl}?payment_method=PayPal&success=1&order_id={paymentPaypal.Id}"
                    };

                    double subtotal = 10;
                    // Adding Tax, shipping and Subtotal details
                    var details = new Details()
                    {
                        tax = "0",
                        shipping = "0",
                        subtotal = subtotal.ToString()
                    };

                    double total = 0 + 0 + subtotal;
                    //Final amount with details
                    var amount = new Amount()
                    {
                        currency = "USD",
                        total = total.ToString(), // Total must be equal to sum of tax, shipping and subtotal.
                        details = details
                    };

                    var transactionList = new List<Transaction>();
                    // Adding description about the transaction
                    transactionList.Add(new Transaction()
                    {
                        description = "AHihihiihiihihi",
                        invoice_number = paymentPaypal.OrderID.ToString(), //Generate an Invoice No
                        amount = amount,
                        item_list = itemList
                    });


                    PayPal.Api.Payment payment = new PayPal.Api.Payment()
                    {
                        intent = "sale",
                        payer = payer,
                        transactions = transactionList,
                        redirect_urls = redirUrls
                    };

                    // Create a payment using a APIContext
                    var createdPayment = payment.Create(apiContext);

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    await transaction.CommitAsync();
                    
                    return paypalRedirectUrl;
                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message.ToString());
                }
            }
            
        }

        public async Task<PaymentPaypalResponseModel> PaymentByPaypalResult(IQueryCollection collections)
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

                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("payerid"))
                        {
                            response.PayerId = value;
                        }
                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("ba_token"))
                        {
                            response.BaToken = value;
                        }
                        if (!string.IsNullOrEmpty(key) && key.ToLower().Equals("subscription_id"))
                        {
                            response.SubscriptionID = value;
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
                    if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Paypal && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.PaypalSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Momo");

                    if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Paypal) {
                    
                        APIContext apiContext = new APIContext(new OAuthTokenCredential(paymentMethodConfig.partnerCode, paymentMethodConfig.serectkey, PayPal.Api.ConfigManager.Instance.GetProperties()).GetAccessToken())
                        {
                            Config = PayPal.Api.ConfigManager.Instance.GetProperties()
                        };
                        //comfirm
                        var paymentExecution = new PaymentExecution() { payer_id = response.PayerId };
                        PayPal.Api.Payment CLPayment = new PayPal.Api.Payment() { id = response.PaymentId };

                        // approve phiếu thanh toán đó
                        var rs=  CLPayment.Execute(apiContext, paymentExecution);
                    
                        if (response.Success == true)
                        {
                            //Thanh toan thanh cong
                            payment.Status = (int)PaymentStatus.Success;
                            await paymentService.UpdateFieldAsync(payment, d => d.Status);
                        }
                        else
                        {
                            //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                            payment.Status = (int)PaymentStatus.Fail;
                            await paymentService.UpdateFieldAsync(payment, d => d.Status);
                        }
                    }
                    if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.PaypalSubscription){
                        // Tạo phiên than toán mới
                        PaymentSession paymentSession = new PaymentSession();
                        paymentSession.Amount = payment.AmountMoney;
                        paymentSession.PaymentID = payment.Id;
                        paymentSession.Active = true;
                        paymentSession.Deleted = false;
                        paymentSession.Created = Timestamp.Now();
                        if (response.Success == true)
                        {
                            paymentSession.Status = (int)PaymentSessionStatus.Success;

                            //Thanh toan thanh cong
                            if (!String.Empty.Equals(response.SubscriptionID))
                            {
                                payment.PayPalSubscriptionID = response.SubscriptionID;
                            }
                            else { 
                                payment.PayPalSubscriptionID = payment.PayPalSubscriptionID;
                            }
                            payment.StatusForSubScription = (int)PaymentStatus.Success;
                            await paymentService.UpdateFieldAsync(payment, d => d.StatusForSubScription, d=> d.PayPalSubscriptionID);

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
                            payment.StatusForSubScription = (int)PaymentStatus.Fail;
                            
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
                    }

                    
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

        #region thanh toán sub
        public async Task<string> PaymentSubscriptionByPaypal(Entities.Payment paymentPaypal, Package package)
        {
            
               
                    var user = LoginContext.Instance.CurrentUser;
                    // phương thức than toán
                    var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)paymentPaypal.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                    if (paymentMethodType == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Paypal && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.PaypalSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Paypal");

                    // tạo phiếu thanh toán
                    //paymentPaypal.Status = (int)PaymentStatus.Waiting;
                    paymentPaypal.StatusForSubScription = (int)PaymentStatus.Waiting;
                    paymentPaypal.Created = Timestamp.Now();
                    await paymentService.CreateAsync(paymentPaypal);

                    var client = new PayPalClientApi();
                    var authorizationReponse = await client.GetAuthorizationRequest();
                    client.SetToken(authorizationReponse.access_token);
                    var response = await client.GetAuthorizationRequest();
                    client.SetToken(response.access_token);

                    var createSubscriptionRequest = new CreateSubscriptionRequest()
                    {
                        plan_id = package.PayPalPlanID,
                        subscriber = new Subscriber()
                        {
                            email_address = user.email,
                            name = new Name()
                            {
                                full_name = user.fullName,
                                given_name = user.fullName,
                                surname = user.fullName
                            }
                            ,
                            shipping_address = new ShippingAddress()
                            {
                                name = new Name()
                                {
                                    full_name = $"Technical Voice",
                                    given_name = "Technical",
                                    surname = "Voice"
                                },
                                address = new Address()
                                {
                                    address_line_1 = "118-N Block",
                                    country_code = "US",
                                    postal_code = "21045"
                                }
                            }
                        },
                        application_context = new ApplicationContext()
                        {
                            brand_name = "LocationsHub",
                            locale = "en-US",
                            payment_method = new PaymentMethod()
                            {
                                payee_preferred = "IMMEDIATE_PAYMENT_REQUIRED",
                                payer_selected = "PAYPAL"
                            },
                            shipping_preference = "SET_PROVIDED_ADDRESS",
                            user_action = "SUBSCRIBE_NOW",
                            return_url = $"{paymentMethodConfig.returnUrl}?&subscription_id=$subscription_id&payment_method=PayPal&success=1&order_id={paymentPaypal.Id}", // Your app url success case
                            cancel_url = $"{paymentMethodConfig.returnUrl}?payment_method=PayPal&success=0&order_id={paymentPaypal.Id}", // Your app url if user cancels
                        }
                    };

                    var createSubscriptionResponse = await client.CreateSubscription(createSubscriptionRequest);
                    var approvalUrl = createSubscriptionResponse.links.FirstOrDefault(x => x.rel == "approve");

                    return approvalUrl.href.ToString();
                
           
        }

        public Task<string> PaymentSubscriptionByPaypal(Entities.Payment paymentPaypal)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
