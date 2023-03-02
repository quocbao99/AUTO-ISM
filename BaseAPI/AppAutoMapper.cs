using AutoMapper;
using Entities;
using Entities.Catalogue;
using Entities.Paypal;
using Entities.Stripe;
using Models.Catalogue;
using Models.Paypal;
using Models.Stripe;
using Request.Catalogue.CatalogueCreate;
using Request.Catalogue.CatalogueUpdate;
using Request.RequestCreate;
using Request.RequestUpdate;
using Utilities;

namespace Models.AutoMapper
{
    public class AppAutoMapper : Profile
    {
        public AppAutoMapper()
        {
            //người dùng
            CreateMap<UserModel, Users>().ReverseMap();
            CreateMap<UserCreate, Users>().ReverseMap();
            CreateMap<UserUpdate, Users>().ReverseMap();
            CreateMap<PagedList<UserModel>, PagedList<Users>>().ReverseMap();

            //danh sách người dùng yêu thích
            CreateMap<UserFavoriteModel, UserFavorite>().ReverseMap();
            CreateMap<UserFavoriteCreate, UserFavorite>().ReverseMap();
            CreateMap<UserFavoriteUpdate, UserFavorite>().ReverseMap();
            CreateMap<PagedList<UserFavoriteModel>, PagedList<UserFavorite>>().ReverseMap();

            // lịch sử người dùng
            CreateMap<UserHistoryModel, UserHistory>().ReverseMap();
            CreateMap<UserHistoryCreate, UserHistory>().ReverseMap();
            CreateMap<UserHistoryUpdate, UserHistory>().ReverseMap();
            CreateMap<PagedList<UserHistoryModel>, PagedList<UserHistory>>().ReverseMap();

            // Quy đổi tiền tệ
            CreateMap<CurrencyExchangeRateModel, CurrencyExchangeRate>().ReverseMap();
            CreateMap<CurrencyExchangeRateCreate, CurrencyExchangeRate>().ReverseMap();
            CreateMap<CurrencyExchangeRateUpdate, CurrencyExchangeRate>().ReverseMap();
            CreateMap<PagedList<CurrencyExchangeRateModel>, PagedList<CurrencyExchangeRate>>().ReverseMap();

            //quyền
            CreateMap<RoleModel, Role>().ReverseMap();
            CreateMap<RoleCreate, Role>().ReverseMap();
            CreateMap<RoleUpdate, Role>().ReverseMap();
            CreateMap<PagedList<RoleModel>, PagedList<Role>>().ReverseMap();

            //menu
            CreateMap<MenuModel, Menu>().ReverseMap();
            CreateMap<RequestMenuCreateModel, Menu>().ReverseMap();
            CreateMap<RequestMenuUpdateModel, Menu>().ReverseMap();
            CreateMap<PagedList<MenuModel>, PagedList<Menu>>().ReverseMap();

            //market
            CreateMap<MarketModel, Market>().ReverseMap();
            CreateMap<MarketCreate, Market>().ReverseMap();
            CreateMap<MarketUpdate, Market>().ReverseMap();
            CreateMap<PagedList<MarketModel>, PagedList<Market>>().ReverseMap();

            //Brand
            CreateMap<BrandModel, Brand>().ReverseMap();
            CreateMap<BrandCreate, Brand>().ReverseMap();
            CreateMap<BrandUpdate, Brand>().ReverseMap();
            CreateMap<PagedList<BrandModel>, PagedList<Brand>>().ReverseMap();

            //Car
            CreateMap<CarModel, Car>().ReverseMap();
            CreateMap<CarCreate, Car>().ReverseMap();
            CreateMap<EModelUpdate, Car>().ReverseMap();
            CreateMap<PagedList<EModelModel>, PagedList<Car>>().ReverseMap();

            //EModel
            CreateMap<EModelModel, EModel>().ReverseMap();
            CreateMap<EModelCreate, EModel>().ReverseMap();
            CreateMap<EModelUpdate, EModel>().ReverseMap();
            CreateMap<PagedList<EModelModel>, PagedList<EModel>>().ReverseMap();

            //LineOff
            CreateMap<LineOffModel, LineOff>().ReverseMap();
            CreateMap<LineOffCreate, LineOff>().ReverseMap();
            CreateMap<LineOffUpdate, LineOff>().ReverseMap();
            CreateMap<PagedList<LineOffModel>, PagedList<LineOff>>().ReverseMap();

            //SystemFile
            CreateMap<SystemFileModel, SystemFile>().ReverseMap();
            CreateMap<SystemFileCreate, SystemFile>().ReverseMap();
            CreateMap<SystemFileUpdate, SystemFile>().ReverseMap();
            CreateMap<PagedList<SystemFileModel>, PagedList<SystemFile>>().ReverseMap();

            //DTC
            CreateMap<DTCModel, DTC>().ReverseMap();
            CreateMap<DTCCreate, DTC>().ReverseMap();
            CreateMap<DTCUpdate, DTC>().ReverseMap();
            CreateMap<PagedList<DTCModel>, PagedList<DTC>>().ReverseMap();

            //material
            CreateMap<MaterialModel, Material>().ReverseMap();
            CreateMap<MaterialCreate, Material>().ReverseMap();
            CreateMap<MaterialUpdate, Material>().ReverseMap();
            CreateMap<PagedList<MaterialModel>, PagedList<Material>>().ReverseMap();

            //Package
            CreateMap<PackageModel, Package>().ReverseMap();
            CreateMap<PackageCreate, Package>().ReverseMap();
            CreateMap<PackageUpdate, Package>().ReverseMap();
            CreateMap<PagedList<PackageModel>, PagedList<Package>>().ReverseMap();

            //Policy
            CreateMap<PolicyModel, Policy>().ReverseMap();
            CreateMap<PolicyCreate, Policy>().ReverseMap();
            CreateMap<PolicyUpdate, Policy>().ReverseMap();
            CreateMap<PagedList<PolicyModel>, PagedList<Policy>>().ReverseMap();

            //Order
            CreateMap<OrderModel, Order>().ReverseMap();
            CreateMap<OrderCreate, Order>().ReverseMap();
            CreateMap<OrderUpdate, Order>().ReverseMap();
            CreateMap<PagedList<OrderModel>, PagedList<Order>>().ReverseMap();


            //payment
            CreateMap<PaymentModel, Payment>().ReverseMap();
            CreateMap<PaymentCreate, Payment>().ReverseMap();
            CreateMap<PaymentUpdate, Payment>().ReverseMap();
            CreateMap<PagedList<PaymentModel>, PagedList<Payment>>().ReverseMap();

            //contract
            CreateMap<ContractModel, Contract>().ReverseMap();
            CreateMap<ContractCreate, Contract>().ReverseMap();
            CreateMap<ContractUpdate, Contract>().ReverseMap();
            CreateMap<PagedList<ContractModel>, PagedList<Contract>>().ReverseMap();

            // payment session
            CreateMap<MomoUtilities.Result, PaymentSession>().ReverseMap();
            CreateMap<PaymentSessionModel, PaymentSession>().ReverseMap();
            CreateMap<PaymentSessionCreate, PaymentSession>().ReverseMap();
            CreateMap<PaymentSessionUpdate, PaymentSession>().ReverseMap();
            CreateMap<PagedList<PaymentSessionModel>, PagedList<PaymentSession>>().ReverseMap();

            //paymentConfig
            CreateMap<PaymentMethodConfigurationModel, PaymentMethodConfiguration>().ReverseMap();
            CreateMap<PaymentMethodConfigurationCreate, PaymentMethodConfiguration>().ReverseMap();
            CreateMap<PaymentMethodConfigurationUpdate, PaymentMethodConfiguration>().ReverseMap();
            CreateMap<PagedList<PaymentMethodConfigurationModel>, PagedList<PaymentMethodConfiguration>>().ReverseMap();
            
            //payment method type
            CreateMap<PaymentMethodTypeModel, PaymentMethodType>().ReverseMap();
            CreateMap<PaymentMethodTypeCreate, PaymentMethodType>().ReverseMap();
            CreateMap<PaymentMethodTypeUpdate, PaymentMethodType>().ReverseMap();
            CreateMap<PagedList<PaymentMethodTypeModel>, PagedList<PaymentMethodType>>().ReverseMap();

            //materialSub
            CreateMap<MaterialSubModel, MaterialSub>().ReverseMap();
            CreateMap<MaterialSubCreate, MaterialSub>().ReverseMap();
            CreateMap<MaterialSubUpdate, MaterialSub>().ReverseMap();
            CreateMap<PagedList<MaterialSubModel>, PagedList<MaterialSub>>().ReverseMap();

            //AccessDescriptionService
            CreateMap<AccessDescriptionModel, AccessDescription>().ReverseMap();
            CreateMap<AccessDescriptionCreate, AccessDescription>().ReverseMap();
            CreateMap<AccessDescriptionUpdate, AccessDescription>().ReverseMap();
            CreateMap<PagedList<AccessDescriptionModel>, PagedList<AccessDescription>>().ReverseMap();


            //Notification
            CreateMap<NotificationModel, Notification>().ReverseMap();
            CreateMap<NotificationCreate, Notification>().ReverseMap();
            CreateMap<NotificationUpdate, Notification>().ReverseMap();
            CreateMap<PagedList<NotificationModel>, PagedList<Notification>>().ReverseMap();

            //UserNotification
            CreateMap<UserNotificationModel, UserNotification>().ReverseMap();
            CreateMap<UserNotificationCreate, UserNotification>().ReverseMap();
            CreateMap<UserNotificationUpdate, UserNotification>().ReverseMap();
            CreateMap<PagedList<UserNotificationModel>, PagedList<UserNotification>>().ReverseMap();

            #region Paypal
            CreateMap<PlanPaypalModel, PlanPaypal>().ReverseMap();
            CreateMap<PlanPaypalCreate, PlanPaypal>().ReverseMap();
            CreateMap<PlanPaypalUpdate, PlanPaypal>().ReverseMap();
            CreateMap<PagedList<PlanPaypalModel>, PagedList<PlanPaypal>>().ReverseMap();

            #endregion

            #region Stripe
            CreateMap<PriceStripeModel, PriceStripe>().ReverseMap();
            CreateMap<PriceStripeCreate, PriceStripe>().ReverseMap();
            CreateMap<PriceStripeUpdate, PriceStripe>().ReverseMap();
            CreateMap<PagedList<PriceStripeModel>, PagedList<PriceStripe>>().ReverseMap();

            #endregion
        }
    }
}