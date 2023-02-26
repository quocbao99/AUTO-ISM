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
using System.Security.Cryptography;
using Extensions;
using System.Net;
using System.Text.Json;
using Interface.Services.Specializing;
using Interface.Services.Configuration;

namespace Service.Services
{
    public class PaymentByMomoService : IPaymentByMomoService
    {
        private IAppDbContext coreDbContext;
        private IMapper mapper;

        private IPaymentService paymentService;
        private IPaymentSessionService PaymentSessionService;
        private IPaymentMethodConfigurationService PaymentMethodConfigurationService;
        private IPaymentMethodTypeService PaymentMethodTypeService;
        private IOrderService orderService;
        private IContractSpecializingService contractSpecializingService;
        private IHangFireManageSpecializingService hangFireManageSpecializingService;
        private IHangFireManageService hangFireManageService;
        private IUserService userService;
        private IEmailConfigurationService emailConfigurationService;

        public PaymentByMomoService(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IPaymentService paymentService,
            IOrderService orderService,
            IPaymentSessionService PaymentSessionService,
            IPaymentMethodConfigurationService PaymentMethodConfigurationService,
            IPaymentMethodTypeService PaymentMethodTypeService,
            IContractSpecializingService contractSpecializingService,
            IHangFireManageSpecializingService hangFireManageSpecializingService,
            IHangFireManageService hangFireManageService,
            IUserService userService,
            IEmailConfigurationService emailConfigurationService,
            IAppDbContext coreDbContext
            ) 
        {
            this.coreDbContext = coreDbContext;
            this.paymentService = paymentService;
            this.mapper = mapper;
            this.PaymentSessionService = PaymentSessionService;
            this.PaymentMethodConfigurationService = PaymentMethodConfigurationService;
            this.PaymentMethodTypeService = PaymentMethodTypeService;
            this.orderService = orderService;
            this.contractSpecializingService = contractSpecializingService;
            this.hangFireManageSpecializingService = hangFireManageSpecializingService;
            this.hangFireManageService = hangFireManageService;
            this.userService = userService;
            this.emailConfigurationService = emailConfigurationService;
        }
        #region thanh toán thường
        public async Task<string> PaymentByMomo(Payment payment)
        {
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                try
                {
                    // phương thức than toán
                    var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new MyException("Không tìm thấy phương thức thanh toán",HttpStatusCode.BadRequest);
                    var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                    if (paymentMethodType == null) throw new MyException("Không tìm thấy phương thức thanh toán", HttpStatusCode.BadRequest);
                    if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Momo && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.MomoSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Momo");

                    // tạo phiếu thanh toán
                    Guid orderId = Guid.NewGuid();
                    payment.OrderID = orderId;
                    if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.MomoSubscription) { 
                        payment.StatusForSubScription = (int)PaymentStatusForSubScription.Waiting;
                    }
                    if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Momo) { 
                        payment.Status = (int)PaymentStatus.Waiting;
                    }

                    payment.Created = Timestamp.Now();
                    await paymentService.CreateAsync(payment);
                    //request params need to request to MoMo system
                    string endpoint = paymentMethodConfig.endpoint;
                    string partnerCode = paymentMethodConfig.partnerCode;
                    string accessKey = paymentMethodConfig.accessKey;
                    string serectkey = paymentMethodConfig.serectkey;
                    string orderInfo = "test";
                    string returnUrl = paymentMethodConfig.returnUrl;
                    string notifyurl = paymentMethodConfig.notifyurl;
                    string lang = paymentMethodConfig.Locale;
                    string requestType = paymentMethodConfig.Command;
                    string requestId = Guid.NewGuid().ToString();
                    string extraData = "";
                    long amount = 0;
                    string rawHash = "";
                    JObject message = null;
                    MomoUtilities crypto = new MomoUtilities();
                    if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Momo)
                    {
                        amount = (long)payment.AmountMoney;

                        //Before sign HMAC SHA256 signature
                        rawHash =
                           "accessKey=" + accessKey +
                           "&amount=" + amount +
                           "&extraData=" + extraData +
                           "&ipnUrl=" + notifyurl +
                           "&orderId=" + orderId.ToString() +
                           "&orderInfo=" + orderInfo +
                           "&partnerCode=" + partnerCode +
                           "&redirectUrl=" + returnUrl +
                           "&requestId=" + requestId +
                           "&requestType=" + requestType
                           ;

                        //sign signature SHA256
                        string signature = crypto.signSHA256(rawHash, serectkey);
                        //build body json request
                        message = new JObject
                        {
                            { "partnerCode", partnerCode },
                            { "requestId", requestId },
                            { "amount", amount },
                            { "orderId", orderId.ToString() },
                            { "orderInfo", orderInfo },
                            { "redirectUrl", returnUrl },
                            { "ipnUrl", notifyurl },
                            { "lang", lang },
                            { "extraData", extraData },
                            { "requestType", requestType },
                            { "signature", signature }
                        };
                    }
                    if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.MomoSubscription)
                    {
                        amount = (long)payment.AmountMoney;
                        string partnerClientId = Guid.NewGuid().ToString();
                        string partnerSubsId = Guid.NewGuid().ToString();
                        string name = "Goi ABC Premium 1645170503079";
                        string subsOwner = "Owner A";
                        string type = "VARIABLE";
                        long recurringAmount = 60000;
                        string nextPaymentDate = "2023-01-25";
                        string expiryDate = "2023-02-22";
                        string frequency = "MONTHLY";
                        lang = "en";
                        JObject subscriptionInfo = new JObject
                        {
                            { "partnerSubsId", partnerSubsId },
                            { "name", name },
                            { "subsOwner", subsOwner },
                            { "type", type },
                            { "recurringAmount", recurringAmount },
                            { "nextPaymentDate", nextPaymentDate },
                            { "expiryDate", expiryDate },
                            { "frequency", frequency }
                        };

                        //Before sign HMAC SHA256 signature
                        rawHash =
                           "accessKey=" + accessKey +
                           "&amount=" + amount.ToString() +
                           "&extraData=" + extraData +
                           "&ipnUrl=" + notifyurl +
                           "&orderId=" + orderId.ToString() +
                           "&orderInfo=" + orderInfo +
                           "&partnerClientId=" + partnerClientId +
                           "&partnerCode=" + partnerCode +
                           "&redirectUrl=" + returnUrl +
                           "&requestId=" + requestId +
                           "&requestType=" + requestType
                           ;
                        //sign signature SHA256
                        string signature = crypto.signSHA256(rawHash, serectkey);

                        //build body json request
                        message = new JObject
                        {
                            { "partnerCode", partnerCode },
                            { "requestType", requestType },
                            { "ipnUrl", notifyurl },
                            { "redirectUrl", returnUrl },
                            { "orderId", orderId.ToString() },
                            { "amount", amount.ToString() },
                            { "lang", lang },
                            { "orderInfo", orderInfo },
                            { "requestId", requestId },
                            { "partnerClientId", partnerClientId },
                            { "extraData", extraData },
                            { "signature", signature },
                            { "subscriptionInfo", subscriptionInfo }
                        };
                    }
                    try {
                        string responseFromMomo = MomoUtilities.sendPaymentRequest(endpoint, message.ToString());

                        JObject jmessage = JObject.Parse(responseFromMomo);
                        if (jmessage.GetValue("resultCode").ToString() != "0")
                            throw new Exception(jmessage.GetValue("message").ToString());
                        await transaction.CommitAsync();
                        return jmessage.GetValue("payUrl").ToString();

                    } catch (MyException ex) {
                        throw new MyException(ex.Message, HttpStatusCode.BadRequest);
                    }
                }

                catch (MyException ex)
                {
                    await transaction.RollbackAsync();
                    throw new MyException(ex.Message.ToString(), ex.HttpStatusCode);
                }
            }
            throw new NotImplementedException();
        }
        #endregion

        #region thanh toán subscription
        public async Task<bool> ProcessPaymentResult(MomoUtilities.Result result)
        {
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
                var payments = await paymentService.GetAsync(d => d.OrderID.ToString() == result.orderId && d.Deleted == false && d.Active == true);
                    
                if (payments.Count() < 1)
                    throw new Exception("ít hoặc nhiều hơn 1 đơn thanh toán");
                var payment = payments.FirstOrDefault();
                var order = await orderService.GetByIdAsync((Guid)payment.OrderID);
                try {
                    if(order == null) throw new Exception("Không tìm thấy đơn hàng");

                    // phương thức than toán
                    var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                    if (paymentMethodType == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                    if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Momo && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.MomoSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Momo");


                    if (result.resultCode == "0" || result.resultCode == "9000") // Momo thanh toán thành công
                    {
                        // thanh toán ủy quyền
                        if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.MomoSubscription)
                        {
                            // tạo mới phiên thanh toán
                            PaymentSession paymentSession = mapper.Map<PaymentSession>(result);
                            paymentSession.PaymentID = payment.Id;
                            paymentSession.Created = Timestamp.Now();
                            await PaymentSessionService.CreateAsync(paymentSession);
                            payment.CallbackToken = result.callbackToken;

                            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/subscription/create";

                            //Before sign HMAC SHA256 signature
                            var rawHash =
                               "accessKey=" + paymentMethodConfig.accessKey +
                               "&callbackToken=" + payment.CallbackToken +
                               "&orderId=" + payment.OrderID.ToString() +
                               "&partnerClientId=" + paymentSession.PartnerClientId +
                               "&partnerCode=" + paymentMethodConfig.partnerCode +
                               "&requestId=" + paymentSession.RequestId
                               ;
                            MomoUtilities crypto = new MomoUtilities();
                            //sign signature SHA256
                            string signature = crypto.signSHA256(rawHash, paymentMethodConfig.serectkey);

                            //build body json request
                            var message = new JObject
                        {
                            { "partnerCode", paymentMethodConfig.partnerCode },
                            { "callbackToken", payment.CallbackToken },
                            { "requestId", paymentSession.RequestId },
                            { "orderId", payment.OrderID.ToString() },
                            { "partnerClientId", paymentSession.PartnerClientId },
                            { "lang", paymentMethodConfig.Locale },
                            { "signature", signature }
                        };

                            string responseFromMomo = MomoUtilities.sendPaymentRequest(endpoint, message.ToString());

                            JObject jmessage = JObject.Parse(responseFromMomo);
                            if (jmessage.GetValue("resultCode").ToString() != "0")
                            {
                                throw new Exception(jmessage.GetValue("message").ToString());
                            }
                            payment.AccessToken = jmessage.GetValue("aesToken").ToString();
                            payment.StatusForSubScription = (int)PaymentStatusForSubScription.Success;

                            await paymentService.UpdateAsync(payment);

                            // kết thúc hợp đồng cũ nếu có
                            Package package = JsonSerializer.Deserialize<Package>(order.PackageInfo);
                            var contract = await contractSpecializingService.ContractsIsUsing((Guid)payment.UserID, package.PackageType);
                            if (contract != null) { 
                                await contractSpecializingService.EndedContract(contract.Id);
                            }

                            // thanh toán thành công => tạo hợp đồng mới
                            var newContract = await contractSpecializingService.NewContractFromPayment(payment);
                            // tạo hangfire hết hạn hợp đồng
                            var rs = await hangFireManageSpecializingService.GenerateJobDelayForContractExpried(newContract.Id);
                            if (rs == false) throw new MyException("Lỗi hệ thống!",HttpStatusCode.InternalServerError);

                            // thanh toán bằng subcription đã tạo
                            // await PayUsingToken(paymentSession);
                            
                            var jobId = BackgroundJob.Schedule(
                                        () => this.PayUsingToken(paymentSession , package),
                                        Timestamp.ToDateTime(newContract.EndTime));

                            var hangfireManage = new HangfireManage();
                            hangfireManage.JobID = jobId;
                            hangfireManage.PaymentID = payment.Id;
                            hangfireManage.HangfireManageType = (int)HangfireManageType.HangFireMomoPayUsingToken;
                            var res = await hangFireManageService.CreateAsync(hangfireManage);
                            if (res == false) throw new MyException("Lỗi hệ thống!");

                            // gửi mã OTP qua mail
                            var user = await userService.GetByIdAsync(order.UserID);
                            string Tos = user.Email.Equals(String.Empty) ? user.Username : user.Email;
                            if (String.Empty.Contains(Tos)) throw new Exception("Lỗi không thể gửi mail");
                            string[] CCs = { };
                            string[] BCCs = { };

                            // tạo luồng gửi mail
                            //Thread SendMail = new Thread(() => { ThreadSendMail(Tos, CCs, BCCs, oTP); });
                            await emailConfigurationService.SendMail("AUTO-ISM thông báo", Tos, CCs, BCCs, new EmailContent { isHtml = true, content = $"Thanh toán thành công đơn hàng mã: {order.OrderCode}", attachments = null });
                            
                            await transaction.CommitAsync();

                            return true;
                        }
                        // thanh thanh toán thường
                        else if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Momo) {
                            payment.Status = (int)PaymentStatus.Success;
                            await paymentService.UpdateAsync(payment);
                            await transaction.CommitAsync();
                        }
                    }
                    else 
                    {
                        // gửi mã OTP qua mail
                        var user = await userService.GetByIdAsync(order.UserID);
                        string Tos = user.Email.Equals(String.Empty) ? user.Username : user.Email;
                        if (String.Empty.Contains(Tos)) throw new Exception("Lỗi không thể gửi mail");
                        string[] CCs = { };
                        string[] BCCs = { };

                        // tạo luồng gửi mail
                        //Thread SendMail = new Thread(() => { ThreadSendMail(Tos, CCs, BCCs, oTP); });
                        await emailConfigurationService.SendMail("AUTO-ISM thông báo", Tos, CCs, BCCs, new EmailContent { isHtml = true, content = $"Thanh toán thành công đơn hàng mã: {order.OrderCode}", attachments = null });

                        if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.MomoSubscription) {
                            payment.StatusForSubScription = (int)PaymentStatusForSubScription.Fail;
                        }
                        else if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Momo) { 
                            payment.Status = (int)PaymentStatus.Fail;
                        }
                    }

                    await paymentService.UpdateFieldAsync(payment, d => d.Status);

                    await transaction.CommitAsync();
                    return true;
                }
                catch (MyException ex) {
                    // gửi mã OTP qua mail
                    var user = await userService.GetByIdAsync(order.UserID);
                    string Tos = user.Email.Equals(String.Empty) ? user.Username : user.Email;
                    if (String.Empty.Contains(Tos)) throw new Exception("Lỗi không thể gửi mail");
                    string[] CCs = { };
                    string[] BCCs = { };
                    await emailConfigurationService.SendMail("AUTO-ISM thông báo", Tos, CCs, BCCs, new EmailContent { isHtml = true, content = $"Thanh toán thất bại đơn hàng mã: {order.OrderCode}", attachments = null });

                    await transaction.RollbackAsync();
                    throw new MyException(ex.Message.ToString());
                }
            }
                
        }

        public async Task<object> ReceiveSubscriptionTokenMomo(PaymentSession paymentSession) {
            var payment = await paymentService.GetByIdAsync((Guid)paymentSession.PaymentID);
            if (payment == null) { throw new Exception("Không tìm thấy phiếu thanh toán"); }
            // phương thức than toán
            var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
            if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");
            var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
            if (paymentMethodType == null) throw new Exception("Không tìm thấy phương thức thanh toán");
            if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Momo && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.MomoSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Momo");

            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/subscription/create";

            //Before sign HMAC SHA256 signature
            var rawHash =
               "accessKey=" + paymentMethodConfig.accessKey +
               "&callbackToken=" + payment.CallbackToken +
               "&orderId=" + payment.OrderID.ToString() +
               "&partnerClientId=" + paymentSession.PartnerClientId +
               "&partnerCode=" + paymentMethodConfig.partnerCode +
               "&requestId=" + paymentSession.RequestId
               ;
            MomoUtilities crypto = new MomoUtilities();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, paymentMethodConfig.serectkey);

            //build body json request
            var message = new JObject
                        {
                            { "partnerCode", paymentMethodConfig.partnerCode },
                            { "callbackToken", payment.CallbackToken },
                            { "requestId", paymentSession.RequestId },
                            { "orderId", payment.OrderID.ToString() },
                            { "partnerClientId", paymentSession.PartnerClientId },
                            { "lang", paymentMethodConfig.Locale },
                            { "signature", signature }
                        };

            string responseFromMomo = MomoUtilities.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);
            if (jmessage.GetValue("resultCode").ToString() != "0") { 
                throw new Exception(jmessage.GetValue("message").ToString());
            }

            payment.AccessToken = jmessage.GetValue("aesToken").ToString();

            await paymentService.UpdateFieldAsync(payment, d => d.AccessToken);
            //await PayUsingToken(paymentSession);
            return responseFromMomo;
        }

        public async Task<object> PayUsingToken(PaymentSession paymentSession, Package package)
        {
            try
            {
                var payment = await paymentService.GetByIdAsync((Guid)paymentSession.PaymentID);
                if (payment == null) { throw new Exception("Không tìm thấy phiếu thanh toán"); }
                var order = await orderService.GetByIdAsync((Guid)payment.OrderID);
                if (order == null) throw new Exception("Không tìm thấy đơn hàng");
                // phương thức than toán
                var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
                if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                if (paymentMethodType == null) throw new Exception("Không tìm thấy phương thức thanh toán");
                if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Momo && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.MomoSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Momo");



                MomoUtilities crypto = new MomoUtilities();
                string endpoint = "https://test-payment.momo.vn/v2/gateway/api/subscription/pay";

                JObject aseToken = JObject.Parse(crypto.decrypt_AES(paymentMethodConfig.serectkey, payment.AccessToken));
                string valueToken = aseToken.GetValue("value").ToString();

                string publickey = "2XH2JFw5YakSagtabOr6Qy/GBy8tY35usAOnHZ08ahIGomMLrS7MPtxK30Foa2AKaF6z/gFqrsBF+IB8yLC7UtYUatPsCz/zzlWR5jP6+SCsjv8l0bXGzPA8O31UVPUnoFFUBfL3K5ORQ8REKjlpRe6EZpLQndVRu93V8LqjOdpp7xT+zhICB9FOEGKHmOR69v+ewubsuLAC88d5ALowopm1zx5DRA6MgBFt0SId108X2JOItJ6y3NlKJhJGC8oXNduUp5SvnlKigH75mqcgBzvA1jvWbRQwDiiIIcBvPh8UXgU8qDOh24rY6Ly0e2leMdO9nZ6aEWKox4fU8otmY2q8RpswuEA0Aq3jz6A/QXy/EoW9rIA4OjfifqhY1eCSIfDAd1/YkgU7n+gxiP21HnDfj/aw9Dj+/rLva+ohy4oWZvfYxHpiCpB8tTBfiHpGCMxik2ejf9qT0Nnx/xP10zW34JSiBX0u0ByV/Ol2X7g/tIfTGRyGIUDqj+DYmO1Tu+WjJli0KBNX0TQvdFNjnvbsLvDxTPKVNSJImpPZb/V/1f8z5fUEEvrC7TNNhuJL+j0OoI15PeFRlUsM7052EiSr08Tgh8yIt2T7Tjbms25ljfM2+glh+UvrqW9RIZm/eNkYfPRQSG3a2kV7y29xebnKX60R4rq3XWpgT2nxGIE=";

                string publickeyXML = $"<RSAKeyValue><Modulus>{publickey}</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

                string newOrderID = Guid.NewGuid().ToString(); // mã đơn hàng mới
                string token = crypto.RSAEncryption(valueToken, payment.OrderID.ToString(), publickeyXML);


                string orderInfo = "test";
                long amount = Convert.ToInt64(payment.AmountMoney);
                //Before sign HMAC SHA256 signature
                var rawHash =
                   "accessKey=" + paymentMethodConfig.accessKey +
                   "&amount=" + amount +
                   "&extraData=" + "" +
                   "&orderId=" + newOrderID + // gửi đơn hàng mới
                   "&orderInfo=" + orderInfo +
                   "&partnerClientId=" + paymentSession.PartnerClientId +
                   "&partnerCode=" + paymentMethodConfig.partnerCode +
                   "&requestId=" + paymentSession.RequestId +
                   "&token=" + token
                   ;
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, paymentMethodConfig.serectkey);

                //build body json request
                var message = new JObject
                            {
                                { "token", token },
                                { "partnerCode", paymentMethodConfig.partnerCode },
                                { "orderId",  newOrderID }, // gửi đơn hàng mới
                                { "amount", amount},
                                { "lang", paymentMethodConfig.Locale },
                                { "orderInfo", orderInfo },
                                { "requestId", paymentSession.RequestId },
                                { "extraData", "" },
                                { "partnerClientId", paymentSession.PartnerClientId },
                                { "nextPaymentDate", DateTime.Now.AddMonths(package.MonthExp).ToString("yyyy-MM-dd") },
                                { "signature", signature }
                            };

                string responseFromMomo = MomoUtilities.sendPaymentRequest(endpoint, message.ToString());
                JObject jmessage = JObject.Parse(responseFromMomo);
                // vì momo không nhận url reponse nên phải đặt lịch lại
                // tạo mới phiên thanh toán
                PaymentSession newPaymentSession = new PaymentSession();
                newPaymentSession.Amount = (decimal)amount;
                newPaymentSession.RequestId = jmessage.GetValue("requestId").ToString();
                newPaymentSession.PaymentID = payment.Id;
                newPaymentSession.Created = Timestamp.Now();

                if (jmessage.GetValue("resultCode").ToString() != "0")
                {
                    newPaymentSession.Status = (int)PaymentSessionStatus.Fail;
                    payment.Status = (int)PaymentStatus.Fail;
                }
                if (jmessage.GetValue("resultCode").ToString() == "0")
                {
                    newPaymentSession.Status = (int)PaymentSessionStatus.Success;
                    payment.Status = (int)PaymentStatus.Success;

                }
                await PaymentSessionService.CreateAsync(newPaymentSession);

                // kết thúc hợp đồng cũ nếu có
                var contract = await contractSpecializingService.ContractsIsUsing((Guid)payment.UserID, package.PackageType);
                if (contract != null)
                {
                    await contractSpecializingService.EndedContract(contract.Id);
                }
                if (newPaymentSession.Status == (int)PaymentSessionStatus.Success) {
                    // thanh toán thành công => tạo hợp đồng mới
                    var newContract = await contractSpecializingService.NewContractFromPayment(payment);
                    // tạo hangfire hết hạn hợp đồng
                    var rs = await hangFireManageSpecializingService.GenerateJobDelayForContractExpried(newContract.Id);
                    if (rs == false) throw new MyException("Lỗi hệ thống!", HttpStatusCode.InternalServerError);

                    // thanh toán bằng subcription đã tạo
                    // await PayUsingToken(paymentSession);

                    var jobId = BackgroundJob.Schedule(
                                () => this.PayUsingToken(newPaymentSession, package),
                                Timestamp.ToDateTime(newContract.EndTime));

                    var hangfireManage = new HangfireManage();
                    hangfireManage.JobID = jobId;
                    hangfireManage.PaymentID = payment.Id;
                    hangfireManage.HangfireManageType = (int)HangfireManageType.HangFireMomoPayUsingToken;
                    var res = await hangFireManageService.CreateAsync(hangfireManage);
                    if (res == false) throw new MyException("Lỗi hệ thống!");

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
                if (newPaymentSession.Status == (int)PaymentSessionStatus.Fail)
                {
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

                await paymentService.UpdateFieldAsync(payment, d => d.AccessToken);
            
                return responseFromMomo;
            } catch (Exception ex) {
                return null;
            }
        }

        public async Task<string> PaymentBy_Momo(Payment payment, Package package)
        {

            var userId = LoginContext.Instance.CurrentUser.userId;
            // phương thức than toán
            var paymentMethodConfig = await PaymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
                if (paymentMethodConfig == null) throw new MyException("Không tìm thấy phương thức thanh toán", HttpStatusCode.BadRequest);
                var paymentMethodType = await PaymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                if (paymentMethodType == null) throw new MyException("Không tìm thấy phương thức thanh toán", HttpStatusCode.BadRequest);
                if (paymentMethodType.Code != (int)CoreContants.PaymentMethodType.Momo && paymentMethodType.Code != (int)CoreContants.PaymentMethodType.MomoSubscription) throw new Exception("Không tìm thấy phương thức thanh toán Momo");

                // kiểm tra User còn hợp đồng cũ hay không có thì bớt tiền gói mới
                //var SubAmount = await contractSpecializingService.SubPriceForPackageIfUserhavingContractUsing(userId, package.PackageType);
                //if ((package.Price - SubAmount) < 10000) throw new MyException("bạn còn sử dụng hợp đồng cũ, số tiền được bù lại cho hợp đồng mới chênh lệch dưới 10.000 momo không thể thanh toán!", HttpStatusCode.BadRequest);
               
                // tạo phiếu thanh toán
                Guid orderId = (Guid)payment.OrderID;
                payment.OrderID = orderId;
                if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.MomoSubscription)
                {
                    payment.StatusForSubScription = (int)PaymentStatusForSubScription.Waiting;
                }
                if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Momo)
                {
                    payment.Status = (int)PaymentStatus.Waiting;
                }

                payment.Created = Timestamp.Now();
                await paymentService.CreateAsync(payment);
                //request params need to request to MoMo system
                string endpoint = paymentMethodConfig.endpoint;
                string partnerCode = paymentMethodConfig.partnerCode;
                string accessKey = paymentMethodConfig.accessKey;
                string serectkey = paymentMethodConfig.serectkey;
                string orderInfo = "test";
                string returnUrl = paymentMethodConfig.returnUrl;
                string notifyurl = paymentMethodConfig.notifyurl;
                string lang = paymentMethodConfig.Locale;
                string requestType = paymentMethodConfig.Command;
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";
                long amount = 0;
                string rawHash = "";
                JObject message = null;
                MomoUtilities crypto = new MomoUtilities();
                if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Momo)
                {
                    amount = (long)payment.AmountMoney;// - (long)SubAmount;

                    //Before sign HMAC SHA256 signature
                    rawHash =
                        "accessKey=" + accessKey +
                        "&amount=" + amount +
                        "&extraData=" + extraData +
                        "&ipnUrl=" + notifyurl +
                        "&orderId=" + orderId.ToString() +
                        "&orderInfo=" + orderInfo +
                        "&partnerCode=" + partnerCode +
                        "&redirectUrl=" + returnUrl +
                        "&requestId=" + requestId +
                        "&requestType=" + requestType
                        ;

                    //sign signature SHA256
                    string signature = crypto.signSHA256(rawHash, serectkey);
                    //build body json request
                    message = new JObject
                    {
                        { "partnerCode", partnerCode },
                        { "requestId", requestId },
                        { "amount", amount },
                        { "orderId", orderId.ToString() },
                        { "orderInfo", orderInfo },
                        { "redirectUrl", returnUrl },
                        { "ipnUrl", notifyurl },
                        { "lang", lang },
                        { "extraData", extraData },
                        { "requestType", requestType },
                        { "signature", signature }
                    };
                }
                if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.MomoSubscription)
                {

                    amount = (long)payment.AmountMoney;// - (long)SubAmount;
                    string partnerClientId = Guid.NewGuid().ToString();
                    string partnerSubsId = Guid.NewGuid().ToString();
                    string name = package.Name;
                    string subsOwner = "Auto_ism";
                    string type = "VARIABLE";
                    long recurringAmount = (long)package.Price;
                    string nextPaymentDate = DateTime.Now.AddMonths(package.MonthExp).ToString("yyyy-MM-dd"); //"2023-01-25";
                    string expiryDate = DateTime.Now.AddYears(10).ToString("yyyy-MM-dd");//"2023-02-22";
                    string frequency = "MONTHLY";
                    lang = "en";
                    JObject subscriptionInfo = new JObject
                    {
                        { "partnerSubsId", partnerSubsId },
                        { "name", name },
                        { "subsOwner", subsOwner },
                        { "type", type },
                        { "recurringAmount", recurringAmount },
                        { "nextPaymentDate", nextPaymentDate },
                        { "expiryDate", expiryDate },
                        { "frequency", frequency }
                    };

                    //Before sign HMAC SHA256 signature
                    rawHash =
                        "accessKey=" + accessKey +
                        "&amount=" + amount.ToString() +
                        "&extraData=" + extraData +
                        "&ipnUrl=" + notifyurl +
                        "&orderId=" + orderId.ToString() +
                        "&orderInfo=" + orderInfo +
                        "&partnerClientId=" + partnerClientId +
                        "&partnerCode=" + partnerCode +
                        "&redirectUrl=" + returnUrl +
                        "&requestId=" + requestId +
                        "&requestType=" + requestType
                        ;
                    //sign signature SHA256
                    string signature = crypto.signSHA256(rawHash, serectkey);

                    //build body json request
                    message = new JObject
                    {
                        { "partnerCode", partnerCode },
                        { "requestType", requestType },
                        { "ipnUrl", notifyurl },
                        { "redirectUrl", returnUrl },
                        { "orderId", orderId.ToString() },
                        { "amount", amount.ToString() },
                        { "lang", lang },
                        { "orderInfo", orderInfo },
                        { "requestId", requestId },
                        { "partnerClientId", partnerClientId },
                        { "extraData", extraData },
                        { "signature", signature },
                        { "subscriptionInfo", subscriptionInfo }
                    };
                }
                try
                {
                    string responseFromMomo = MomoUtilities.sendPaymentRequest(endpoint, message.ToString());

                    JObject jmessage = JObject.Parse(responseFromMomo);
                    if (jmessage.GetValue("resultCode").ToString() != "0")
                        throw new Exception(jmessage.GetValue("message").ToString());
                    return jmessage.GetValue("payUrl").ToString();

                }
                catch (MyException ex)
                {
                    throw new MyException(ex.Message, HttpStatusCode.BadRequest);
                }

                
            throw new NotImplementedException();
        }

        #endregion
    }
}
