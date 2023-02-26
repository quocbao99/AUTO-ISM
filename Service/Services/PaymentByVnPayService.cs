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
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Interface.Services.Specializing;
using Extensions;
using System.Net;
using Interface.Services.Configuration;

namespace Service.Services
{
    public class PaymentByVnPayService : IPaymentByVnPayService
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


        public PaymentByVnPayService(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IPaymentService paymentService,
            IPaymentMethodConfigurationService PaymentMethodConfigurationService,
            IPaymentMethodTypeService PaymentMethodTypeService,
            IOrderService orderService,
            IContractSpecializingService contractSpecializingService,
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
    

        public async Task<string> PaymentByVnPay(Payment payment)
        {
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                try
                {
                    // phương thức than toán
                    var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                    if (paymentMethodType == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.VNPay) throw new Exception("Không tìm thấy phương thức thanh toán VNPay");

                    var httpContext = Extensions.HttpContext.Current;

                    // tạo phiếu thanh toán
                    payment.Status = (int)PaymentStatus.Waiting;
                    payment.paymentVNPayId = Timestamp.Now().ToString() + RandomUtilities.RandomNumber(3);
                    payment.Created = Timestamp.Now();
                    await paymentService.CreateAsync(payment);
                    var timeNow = DateTime.Now;
                    var pay = new VnPayUtilities();
                    var urlCallBack = paymentMethodConfig.returnUrl;
                    pay.AddRequestData("vnp_Version", paymentMethodType.Version);// phiên bản
                    pay.AddRequestData("vnp_Command", paymentMethodConfig.Command);// mã api sử dụng
                    pay.AddRequestData("vnp_TmnCode", paymentMethodConfig.partnerCode);
                    pay.AddRequestData("vnp_Amount", ((int)payment.AmountMoney * 100).ToString());
                    pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
                    pay.AddRequestData("vnp_CurrCode", paymentMethodConfig.CurrCode); // đơn vị tiền tệ
                    pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(httpContext));
                    pay.AddRequestData("vnp_Locale", paymentMethodConfig.Locale); // ngôn ngữ giao diện hiển thị
                    pay.AddRequestData("vnp_OrderInfo", $"{payment.OrderID} {payment.OrderID} {payment.AmountMoney}");
                    //pay.AddRequestData("vnp_OrderType", model.OrderType); loại đơn hàng do VNPay quy định (không bắt buộc)
                    pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
                    pay.AddRequestData("vnp_TxnRef", payment.paymentVNPayId);

                    var paymentUrl =
                        pay.CreateRequestUrl(paymentMethodConfig.endpoint, paymentMethodConfig.serectkey);
                    await transaction.CommitAsync();
                    return paymentUrl;
                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message.ToString());
                }
            }
            throw new NotImplementedException();
        }

        public async Task<string> PaymentBy_VnPay(Payment payment, Package package)
        {
           
                    // phương thức than toán
                    var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                    if (paymentMethodType == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.VNPay) throw new Exception("Không tìm thấy phương thức thanh toán VNPay");

                    var httpContext = Extensions.HttpContext.Current;

                    // tạo phiếu thanh toán
                    
                    if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.VNPay)
                    {
                        payment.Status = (int)PaymentStatus.Waiting;
                    }
                    payment.paymentVNPayId = Timestamp.Now().ToString() + RandomUtilities.RandomNumber(3);
                    payment.Created = Timestamp.Now();
                    await paymentService.CreateAsync(payment);
                    var timeNow = DateTime.Now;
                    var pay = new VnPayUtilities();
                    var urlCallBack = paymentMethodConfig.returnUrl;
                    pay.AddRequestData("vnp_Version", paymentMethodType.Version);// phiên bản
                    pay.AddRequestData("vnp_Command", paymentMethodConfig.Command);// mã api sử dụng
                    pay.AddRequestData("vnp_TmnCode", paymentMethodConfig.partnerCode);
                    pay.AddRequestData("vnp_Amount", ((int)payment.AmountMoney * 100).ToString());
                    pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
                    pay.AddRequestData("vnp_CurrCode", paymentMethodConfig.CurrCode); // đơn vị tiền tệ
                    pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(httpContext));
                    pay.AddRequestData("vnp_Locale", paymentMethodConfig.Locale); // ngôn ngữ giao diện hiển thị
                    pay.AddRequestData("vnp_OrderInfo", $"{payment.OrderID} {payment.OrderID} {payment.AmountMoney}");
                    //pay.AddRequestData("vnp_OrderType", model.OrderType); loại đơn hàng do VNPay quy định (không bắt buộc)
                    pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
                    pay.AddRequestData("vnp_TxnRef", payment.paymentVNPayId);

                    var paymentUrl =
                        pay.CreateRequestUrl(paymentMethodConfig.endpoint, paymentMethodConfig.serectkey);
                    return paymentUrl;

                
            throw new NotImplementedException();
        }

        public async Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections)
        {
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                try
                {
                    var pay = new VnPayUtilities();

                    foreach (var (key, value) in collections)
                    {
                        if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                        {
                            pay.AddResponseData(key, value);
                        }
                    }
                    var paymentVNPayId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef"));

                    // phương thức than toán
                    var payments = await paymentService.GetAsync(d => d.paymentVNPayId == paymentVNPayId.ToString() && d.Deleted == false && d.Active == true); ;
                    if (payments.Count() != 1) throw new Exception("Phiếu thanh toán nhiều hơn 1 phiếu");
                    var payment = payments.FirstOrDefault();
                    var order = await orderService.GetByIdAsync((Guid)payment.OrderID);
                    if (order == null) throw new Exception("Không tìm thấy đơn hàng");

                    var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                    if (paymentMethodType == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.VNPay) throw new Exception("Không tìm thấy phương thức thanh toán VNPay");

                    // Tạo phiên than toán mới
                    PaymentSession paymentSession = new PaymentSession();
                    paymentSession.Amount = payment.AmountMoney;
                    paymentSession.PaymentID = payment.Id;
                    paymentSession.Active = true;
                    paymentSession.Deleted = false;
                    paymentSession.Created = Timestamp.Now();
                    var response = pay.GetFullResponseData(collections, paymentMethodConfig.serectkey);
                    if (response.Success == false)
                    {
                        payment.Status = (int)PaymentStatus.Fail;
                        paymentSession.Status = (int)PaymentSessionStatus.Fail;
                        await paymentService.UpdateFieldAsync(payment, d => d.Status);
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

                    if (response.VnPayResponseCode == "00" && response.VnPayResponseCode == "00")
                    {
                        //Thanh toan thanh cong
                        paymentSession.Status = (int)PaymentSessionStatus.Success;
                        payment.Status = (int)PaymentStatus.Success;
                        await paymentService.UpdateFieldAsync(payment, d => d.Status);

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
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        paymentSession.Status = (int)PaymentSessionStatus.Fail;
                        payment.Status = (int)PaymentStatus.Fail;
                        await paymentService.UpdateFieldAsync(payment, d => d.Status);

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
    }
}
