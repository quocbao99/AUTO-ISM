using Entities;
using Entities.Configuration;
using Extensions;
using Facebook;
using Google.Apis.Auth;
using Interface.DbContext;
using Interface.Services;
using Interface.Services.Configuration;
using Interface.Services.Specializing;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Models;
using Request.Auth;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CoreContants;

namespace Service.Services.Specializing
{
    public class OrderSpecializingService : IOrderSpecializingService
    {
        private IAppDbContext coreDbContext;
        private IConfiguration configuration;
        private IOrderService orderService;
        private IContractSpecializingService contractSpecializingService;

        private IUserService userService;
        private IPackageService packageService;
        private IOTPHistoryService oTPHistoryService;
        private IOTPHistoriesSpecializingService oTPHistoriesSpecializingService;
        private IHangFireManageSpecializingService hangFireManageSpecializingService;
        private IEmailConfigurationService emailConfigurationService;
        private IPaymentMethodConfigurationService paymentMethodConfigurationService;
        private IPaymentMethodTypeService paymentMethodTypeService;
        private INotificationService notificationService;

        private IPaymentByMomoService paymentByMomoService;
        private IPaymentByVnPayService paymentByVnPayService;
        private IPaymentByPaypalService paymentByPaypalService;
        private IPaymentByStripeService paymentByStripeService;
        public OrderSpecializingService(
            IConfiguration configuration
            , IAppDbContext coreDbContext
            , IPackageService packageService
            , IUserService userService
            , IOrderService orderService
            , IOTPHistoriesSpecializingService oTPHistoriesSpecializingService
            , IOTPHistoryService oTPHistoryService
            , IHangFireManageSpecializingService hangFireManageSpecializingService
            , IEmailConfigurationService emailConfigurationService
            , IPaymentMethodConfigurationService paymentMethodConfigurationService
            , IPaymentMethodTypeService paymentMethodTypeService
            , INotificationService notificationService
            , IPaymentByMomoService paymentByMomoService
            , IPaymentByVnPayService paymentByVnPayService
            , IPaymentByPaypalService paymentByPaypalService
            , IPaymentByStripeService paymentByStripeService
            , IContractSpecializingService contractSpecializingService
            ) {

            this.configuration = configuration;
            this.coreDbContext = coreDbContext;
            this.userService = userService;
            this.oTPHistoryService = oTPHistoryService;
            this.oTPHistoriesSpecializingService = oTPHistoriesSpecializingService;
            this.hangFireManageSpecializingService = hangFireManageSpecializingService;
            this.emailConfigurationService = emailConfigurationService;

            this.orderService = orderService;
            this.paymentMethodConfigurationService = paymentMethodConfigurationService;
            this.paymentMethodTypeService = paymentMethodTypeService;
            this.packageService = packageService;
            this.contractSpecializingService = contractSpecializingService;
            // phương thức thanh toán
            this.paymentByMomoService = paymentByMomoService;
            this.paymentByVnPayService = paymentByVnPayService;
            this.paymentByPaypalService = paymentByPaypalService;
            this.paymentByStripeService = paymentByStripeService;
            this.notificationService = notificationService;
        }

        public async Task<string> OrderPackageWithPaymentMethod(OrderPackageWithPaymentMethodCreate item)
        {
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction()) {
                try
                {
                    var userId = LoginContext.Instance.CurrentUser.userId;
                    var user = await userService.GetByIdAsync(userId);
                    if (user == null) throw new Exception("Không tìm thấy thông tin User!");
                    var package = await packageService.GetByIdAsync(item.PackageID);
                    if (package == null) throw new Exception("Không tìm thấy thông tin gói!");

                    // tạo đơn hàng mới
                    var order = new Order();
                    order.PackageID = package.Id;
                    order.PackageInfo = System.Text.Json.JsonSerializer.Serialize(package).ToString();
                    order.UserID = userId;
                    order.Created = Timestamp.Now();
                    order.OrderCode = GenerateCodeUtilities.GenerateCode("OD","");
                    await orderService.CreateAsync(order);

                    // phương thức than toán
                    var paymentMethodConfig = await paymentMethodConfigurationService.GetByIdAsync((Guid)item.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new MyException("Không tìm thấy phương thức thanh toán", HttpStatusCode.BadRequest);
                    var paymentMethodType = await paymentMethodTypeService.GetByIdAsync((Guid)paymentMethodConfig.PaymentMethodID);
                    if (paymentMethodType == null) throw new MyException("Không tìm thấy phương thức thanh toán", HttpStatusCode.BadRequest);

                    // thông tin thanh toán
                    var payment = new Payment() {
                        UserID = userId
                        , PaymentMethodConfigurationID = paymentMethodConfig.Id
                            ,
                        OrderID = order.Id
                            ,
                        AmountMoney = package.Price
                        , Created = Timestamp.Now()
                    };
                    
                    await notificationService.CreateAsync(new Notification() { 
                        Title="Giao dịch mới phát sinh"
                        , Content = $"{user.Email} giao dịch mua gói {package.Name} : {package.Price} / {package.MonthExp}, mã đơn hàng: {order.OrderCode}"
                        ,IsRead = false
                        ,Type = NotificationType.USERs.ToString()
                        ,
                        UserID = new Guid(ADMIN_ID)
                    });
                    
                    var url = "";
                    // thanh toán bằng momo
                    if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Momo
                        || paymentMethodType.Code == (int)CoreContants.PaymentMethodType.MomoSubscription)
                    {
                         url = await paymentByMomoService.PaymentBy_Momo(payment, package);
                    }

                    // thanh toán bằng VNPay
                    else if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.VNPay)
                    {
                        url= await paymentByVnPayService.PaymentBy_VnPay(payment, package);
                    }

                    // thanh toán bằng paypal
                    else if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Paypal
                        || paymentMethodType.Code == (int)CoreContants.PaymentMethodType.PaypalSubscription)
                    {
                        url = await paymentByPaypalService.PaymentSubscriptionByPaypal(payment, package);
                    }


                    // thanh toán bằng stripe
                    else if (paymentMethodType.Code == (int)CoreContants.PaymentMethodType.Stripe
                        || paymentMethodType.Code == (int)CoreContants.PaymentMethodType.StripeSubscription)
                    {
                        url = await paymentByStripeService.PaymentByStripeSubscription(payment, package);
                    }
                    else {
                        throw new Exception("Lỗi thanh toán!");
                    }

                    await transaction.CommitAsync();

                    return url;
                }
                catch (MyException e)
                {
                    await transaction.RollbackAsync();
                    throw new MyException(e.Message, e.HttpStatusCode);
                }
            }
            
        }
    }
}
