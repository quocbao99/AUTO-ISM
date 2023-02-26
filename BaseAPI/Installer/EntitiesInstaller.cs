using Entities.Configuration;
using Interface.Services;
using Interface.Services.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;
using Service.Services.Configurations;
//using SignalrHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAPI.Installer
{
    public class EntitiesInstaller : IInstaller
    {
        public void Installer(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUserFavoriteService, UserFavoriteService>();
            services.AddTransient<IUserHistoryService, UserHistoryService>();

            services.AddTransient<IMarketService, MarketService>();
            services.AddTransient<IBrandService, BrandService>();
            services.AddTransient<ICarService,CarService>();
            services.AddTransient<IEModelService, EModelService>();
            services.AddTransient<ILineOffService, LineOffService>();
            services.AddTransient<ISystemFileService, SystemFileService>();
            services.AddTransient<IDTCService, DTCService>();
            services.AddTransient<IMaterialService, MaterialService>();
            services.AddTransient<IMaterialSubService, MaterialSubService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IPackageService, PackageService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IContractService, ContractService>();
            services.AddTransient<IPaymentSessionService, PaymentSessionService>();
            services.AddTransient<IPaymentMethodConfigurationService, PaymentMethodConfigurationService>();
            services.AddTransient<IPaymentMethodTypeService, PaymentMethodTypeService>();
            services.AddScoped<IEmailConfigurationService, EmailConfigurationService>();
            services.AddTransient<IOTPHistoryService, OTPHistoryService>();
            services.AddTransient<IHangFireManageService, HangFireManageService>();
            services.AddTransient<IAccessDescriptionService, AccessDescriptionService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IPolicyService, PolicyService>();

            #region paypal
            services.AddTransient<IPlanPaypalService, PlanPaypalService>();
            #endregion

            #region stripe
            services.AddTransient<IPriceStripeService, PriceStripeService>();
            #endregion
            
        }
    }
}
