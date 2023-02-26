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
using PayPal;
using PayPal.Models.Requests;
using Request.Auth;
using Request.RequestCreate;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CoreContants;

namespace Service.Services.Specializing
{
    public class ContractSpecializingService : IContractSpecializingService
    {
        private IAppDbContext coreDbContext;
        private IConfiguration configuration;
        private IOrderService orderService;
        private IPaymentService paymentService;
        private IContractService contractService;

        private IUserService userService;
        private IPackageService packageService;
        private IOTPHistoryService oTPHistoryService;
        private IOTPHistoriesSpecializingService oTPHistoriesSpecializingService;
        //private IHangFireManageSpecializingService hangFireManageSpecializingService;
        private IEmailConfigurationService emailConfigurationService;
        private IPaymentMethodConfigurationService paymentMethodConfigurationService;
        private IPaymentMethodTypeService paymentMethodTypeService;

       
        public ContractSpecializingService(
            IConfiguration configuration
            , IAppDbContext coreDbContext
            , IPackageService packageService
            , IUserService userService
            , IOrderService orderService
            , IOTPHistoriesSpecializingService oTPHistoriesSpecializingService
            , IOTPHistoryService oTPHistoryService
            //, IHangFireManageSpecializingService hangFireManageSpecializingService
            , IEmailConfigurationService emailConfigurationService
            , IPaymentMethodConfigurationService paymentMethodConfigurationService
            , IPaymentMethodTypeService paymentMethodTypeService
            
            , IContractService contractService
            , IPaymentService paymentService
            ) {

            this.configuration = configuration;
            this.coreDbContext = coreDbContext;
            this.userService = userService;
            this.oTPHistoryService = oTPHistoryService;
            this.oTPHistoriesSpecializingService = oTPHistoriesSpecializingService;
            //this.hangFireManageSpecializingService = hangFireManageSpecializingService;
            this.emailConfigurationService = emailConfigurationService;

            this.orderService = orderService;
            this.paymentMethodConfigurationService = paymentMethodConfigurationService;
            this.paymentMethodTypeService = paymentMethodTypeService;
            this.packageService = packageService;
            this.contractService = contractService;
            this.paymentService = paymentService;

            
        }

        public async Task<List<Contract>> ContractsIsUsing(Guid UserID)
        {
            return (List<Contract>)await contractService.GetAsync(d =>d.UserID == UserID && d.Status == (int)ContractStatus.Using && d.Deleted == false && d.Active == true);
        }

        public async Task<Contract> ContractsIsUsing(Guid userID, int packageContractType)
        {
            return await contractService.GetSingleAsync(d => d.UserID == userID && d.ContractType == packageContractType && d.Status == (int)ContractStatus.Using && d.Deleted == false && d.Active == true);
        }

        public async Task<bool> EndedContract(Guid ContractId)
        {
            var contract = await contractService.GetByIdAsync(ContractId);
            if (contract == null) throw new Exception("không tìm thấy thông tin hợp đồng");
            contract.Status = (int)ContractStatus.Ended;

            var payment = await paymentService.GetByIdAsync(contract.PaymenntID);
            if (payment is null) throw new Exception("Không tìm thấy thông tin thanh toán");
            var paymentConfig = await paymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
            if (paymentConfig is null) throw new Exception("Không tìm thấy thông tin thanh toán");
            var paymentMethod = await paymentMethodTypeService.GetByIdAsync((Guid)paymentConfig.PaymentMethodID);
            if (paymentMethod is null) throw new Exception("Không tìm thấy thông tin thanh toán");
            #region paypal
            if (paymentMethod.Code == (int)CoreContants.PaymentMethodType.PaypalSubscription) {
                // hủy subscription
                var client = new PayPalClientApi();
                var authorizationReponse = await client.GetAuthorizationRequest();
                client.SetToken(authorizationReponse.access_token);
                var response = await client.GetAuthorizationRequest();
                client.SetToken(response.access_token);

                if (string.Empty.Equals(payment.PayPalSubscriptionID) || payment.PayPalSubscriptionID is null)
                    return true;


                string subscriptionId = payment.PayPalSubscriptionID;

                // Create the cancel subscription request
                var cancelSubscriptionRequest = new SubscriptionStatusChangeRequest
                {
                    reason = "Customer cancellation"
                };
                var CancelSubscriptionResponse = await client.CancelSubscription(subscriptionId, cancelSubscriptionRequest);
            }
            #endregion
            if (paymentMethod.Code == (int)CoreContants.PaymentMethodType.StripeSubscription)
            {
                // hủy subscription
                // phương thức thanh toán trả ra url
                StripeConfiguration.ApiKey = paymentConfig.serectkey;
                var sessionService = new SessionService();
                var subscriptionService = new SubscriptionService();
                
                if (string.Empty.Equals(payment.StripeSubscriptionID) || payment.StripeSubscriptionID is null)
                    return true;
                // Retrieve the session by its ID
                var session = sessionService.Get(payment.StripeSubscriptionID); // StripeSubscriptionID đang lưu trữ session id


                // Retrieve the subscription from the session object
                var subscriptionId = session.SubscriptionId;

                // Cancel the subscription
                var options = new SubscriptionCancelOptions
                {
                    InvoiceNow = true, // Set any cancellation options
                    Prorate = true
                };
                var canceledSubscription = subscriptionService.Cancel(subscriptionId, options);
            }


            return await contractService.UpdateFieldAsync(contract, d => d.Status);
        }

        public async Task<bool> EndedSubcription(Guid ContractId)
        {
            var contract = await contractService.GetByIdAsync(ContractId);
            if (contract == null) throw new Exception("không tìm thấy thông tin hợp đồng");

            var payment = await paymentService.GetByIdAsync(contract.PaymenntID);
            if (payment is null) throw new Exception("Không tìm thấy thông tin thanh toán");
            var paymentConfig = await paymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
            if (paymentConfig is null) throw new Exception("Không tìm thấy thông tin thanh toán");
            var paymentMethod = await paymentMethodTypeService.GetByIdAsync((Guid)paymentConfig.PaymentMethodID);
            if (paymentMethod is null) throw new Exception("Không tìm thấy thông tin thanh toán");
            #region paypal
            if (paymentMethod.Code == (int)CoreContants.PaymentMethodType.PaypalSubscription)
            {
                // hủy subscription
                var client = new PayPalClientApi();
                var authorizationReponse = await client.GetAuthorizationRequest();
                client.SetToken(authorizationReponse.access_token);
                var response = await client.GetAuthorizationRequest();
                client.SetToken(response.access_token);

                if (string.Empty.Equals(payment.PayPalSubscriptionID) || payment.PayPalSubscriptionID is null)
                    return true;

                string subscriptionId = payment.PayPalSubscriptionID;
                // Create the cancel subscription request
                var cancelSubscriptionRequest = new SubscriptionStatusChangeRequest
                {
                    reason = "Customer cancellation"
                };
                var CancelSubscriptionResponse = await client.CancelSubscription(subscriptionId, cancelSubscriptionRequest);
            }
            #endregion
            if (paymentMethod.Code == (int)CoreContants.PaymentMethodType.StripeSubscription)
            {
                // hủy subscription
                // phương thức thanh toán trả ra url
                StripeConfiguration.ApiKey = paymentConfig.serectkey;
                var sessionService = new SessionService();
                var subscriptionService = new SubscriptionService();
                
                if (string.Empty.Equals(payment.StripeSubscriptionID) || payment.StripeSubscriptionID is null)
                    return true;
                // Retrieve the session by its ID
                var session = sessionService.Get(payment.StripeSubscriptionID); // StripeSubscriptionID đang lưu trữ session id

                // Retrieve the subscription from the session object
                var subscriptionId = session.SubscriptionId;

                // Cancel the subscription
                var options = new SubscriptionCancelOptions
                {
                    InvoiceNow = true, // Set any cancellation options
                    Prorate = true
                };
                var canceledSubscription = subscriptionService.Cancel(subscriptionId, options);
            }


            return true;
        }

        public async Task<bool> ExpiredContract(Guid ContractId)
        {
            var contract = await contractService.GetByIdAsync(ContractId);
            if (contract == null) throw new Exception("không tìm thấy thông tin hợp đồng");
            contract.Status = (int)ContractStatus.Expired;
            return await contractService.UpdateFieldAsync(contract, d => d.Status);
        }

        public async Task<Contract> NewContractFromPayment(Payment payment)
        {
            // thông tin đơn hàng
            var order = await orderService.GetByIdAsync((Guid)payment.OrderID);
            if (order == null) throw new Exception("Không tìm thấy đơn hàng");

            // thông tin gói cước
            Package package = JsonSerializer.Deserialize<Package>(order.PackageInfo);

            Contract contract = new Contract()
            {
                PaymenntID = payment.Id
                , UserID = order.UserID
                , PackageInfo = order.PackageInfo
                , ContractType = package.PackageType
                                ,
                StartTime = Timestamp.Now()
                                ,
                EndTime = Timestamp.Date(DateTime.Now.AddMonths(package.MonthExp))
                                ,
                Status = (int)ContractStatus.Using
            };

            var rs = await contractService.CreateAsync(contract);
            if (rs == false) throw new MyException("");
            return contract; 

            throw new NotImplementedException();
        }

        public async Task<decimal> SubPriceForPackageIfUserhavingContractUsing(Guid userID, int packageType)
        {
            var contract = await contractService.GetSingleAsync(d => d.UserID == userID && d.ContractType == packageType && d.Status == (int)ContractStatus.Using && d.Deleted == false && d.Active == true);
            if (contract == null) return 0;
            else {
                // thông tin gói cước
                Package package = JsonSerializer.Deserialize<Package>(contract.PackageInfo);
                var totalDays = (Timestamp.ToDateTime(contract.EndTime) - Timestamp.ToDateTime(contract.StartTime)).TotalDays;
                var usingDays = (DateTime.Now - Timestamp.ToDateTime(contract.StartTime)).TotalDays;
                var amountPerDay = (double)package.Price / totalDays;
                return (decimal)(amountPerDay * (totalDays - usingDays));
            }

            throw new NotImplementedException();
        }
    }
}
