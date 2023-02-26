using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Service.Services.DomainServices;
using Entities.Search;
using Newtonsoft.Json;
using System.Reflection;
using static Utilities.CoreContants;
using PayPal;
using MyPayPal.Models.Requests;
using Stripe;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Jose;
using System.Security.Cryptography;
using AppStore;
using AppStore.Configuration;

namespace Service.Services
{
    public class PackageService : DomainService<Package, PackageSearch>, IPackageService
    {
        protected IAppDbContext coreDbContext;
        protected ICurrencyExchangeRateService currencyExchangeRate;
        protected IPaymentMethodConfigurationService paymentMethodConfigurationService;
        protected IPaymentMethodTypeService paymentMethodTypeService;
        public PackageService(IAppUnitOfWork unitOfWork
            , IMapper mapper
            , IAppDbContext coreDbContext
            , ICurrencyExchangeRateService currencyExchangeRate
            , IPaymentMethodConfigurationService paymentMethodConfigurationService
            , IPaymentMethodTypeService paymentMethodTypeService
            ) : base(unitOfWork, mapper)
        {
            this.coreDbContext = coreDbContext;
            this.currencyExchangeRate = currencyExchangeRate;
            this.paymentMethodConfigurationService = paymentMethodConfigurationService;
            this.paymentMethodTypeService = paymentMethodTypeService;
        }
        protected override string GetStoreProcName()
        {
            return "Package_GetPagingPackage";
        }

        public async override Task<bool> CreateAsync(IList<Package> items)
        {
            foreach (var item in items) {
                if (item.PackageType != (int)PackageContractType.Car && item.PackageType != (int)PackageContractType.Truck) throw new Exception("Không cung cấp loại gói này");
                //đổi giá gói sang USD
                var exchangeRates = await currencyExchangeRate.GetAsync(d => d.ExchangeType == (int)ExchangeType.USD && d.Deleted == false);
                if (exchangeRates is null || exchangeRates.Count() == 0) {
                    throw new Exception("Không tìm được đơn vị quy đổi");
                }
                var exchangeRate = exchangeRates.FirstOrDefault();
                decimal amount_USD = (item.Price / exchangeRate.AmountVN) * exchangeRate.AmountType;

                #region App store
                AppStoreClient appStoreClient = new AppStoreClient();
                appStoreClient.SetToken(appStoreClient.GenerateToken());

                //await appStoreClient.GetApps(new AppStore.Models.Request.GetAppsRequest() { data = "" });
                // lấy giá từ apple store

                var subcription = await appStoreClient.CreateSubcriptions(new AppStore.Models.Request.CreateSubcriptionRequest()
                {
                    data = new AppStore.Models.Request.DataSubscriptionCreate()
                    {
                        attributes = new AppStore.Models.Request.SubscriptionCreateAttributes()
                        {
                            availableInAllTerritories = true,
                            familySharable = true,
                            name = item.Name,
                            productId= DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                            reviewNote ="",
                            subscriptionPeriod = appStoreClient.GetSubscriptionPeriodByNumber(item.MonthExp),
                            groupLevel = 1
                            
                        },
                        type = "subscriptions",
                        relationships = new AppStore.Models.Request.SubscriptionCreateRelationships()
                        {
                            group = new AppStore.Models.Request.Group()
                            {
                                data = new AppStore.Models.Request.GroupData()
                                {
                                    id = ConfigHelper.SubscriptionGroupID // id nhóm subcription mỗi app tốt nhất sẽ chỉ có 1 nhóm này nên tui sẽ để cứng
                                    ,
                                    type = "subscriptionGroups"
                                }
                            }
                        }
                    }

                });
                var pricePoints = await appStoreClient.GetPricePoint(new AppStore.Models.Request.GetPricePointRequest() { subcription_id = subcription.data.id, subscriptionPricePoints="30" });
                var pricePoint = pricePoints.data.Where(d => d.attributes.customerPrice == (double)amount_USD).FirstOrDefault();
                if (pricePoint is null) throw new Exception("Lối liên kết với apple Store");
                var subcriptionPrice = await appStoreClient.CreateSubcriptionPrice(new AppStore.Models.Request.CreateSubcriptionPriceRequest()
                {
                    data = new AppStore.Models.Request.DataSubscriptionPriceCreate()
                    {
                        attributes = new AppStore.Models.Request.SubscriptionPriceCreateAttributes()
                        {
                            preserveCurrentPrice = true,
                            startDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        },
                        type = "subscriptionPrices",
                        relationships = new AppStore.Models.Request.SubscriptionPriceCreateRelationships()
                        {
                            subscription = new AppStore.Models.Request.SubscriptionSubscriptionPriceRelationships()
                            {
                                data = new AppStore.Models.Request.SubscriptionDataSubscriptionPrice()
                                {
                                    id = subcription.data.id,
                                    type = "subscriptions"
                                }
                            },
                            subscriptionPricePoint = new AppStore.Models.Request.SubscriptionPricePointSubscriptionPriceRelationships() { 
                                data = new AppStore.Models.Request.SubscriptionPriceDataPointSubscriptionPriceRelationships() { 
                                    id = pricePoint.id,
                                    type = "subscriptionPricePoints"
                                }
                            }
                            //territory= new AppStore.Models.Request.TerritorySubscriptionPriceRelationships() { 
                            //    data = new AppStore.Models.Request.TerritorySubscriptionPriceSubscriptionPriceRelationships() { 
                            //        id = "",
                            //        type = "territories"
                            //    }
                            //}
                        }
                    }

                });

                #endregion

                // tạo product id và plan id cho paypal
                // tạo product
                var client = new PayPalClientApi();
                var authorizationReponse = await client.GetAuthorizationRequest();
                client.SetToken(authorizationReponse.access_token);
                var response = await client.GetAuthorizationRequest();
                client.SetToken(response.access_token);

                var createProductRequest = new CreateProductRequest()
                {
                    id = Timestamp.Now().ToString(),
                    name = item.Name,
                    type = "SERVICE",
                    description = item.Description,
                    category = "ACADEMIC_SOFTWARE",
                    image_url = null,
                    home_url = null
                };
                var createProductResponse = await client.CreateProduct(createProductRequest);
                if (createProductResponse.id is null || String.Empty.Equals(createProductResponse.id))
                {
                    throw new Exception("Liên kết ví paypal thất bại");
                }

                // tạo plan
                var createPlanRequest = new CreatePlanRequest()
                {
                    product_id = createProductResponse.id,
                    name = item.Name,
                    description = item.Description,
                    billing_cycles = new List<BillingCycle>() {
                        new BillingCycle(){
                            frequency = new Frequency(){
                                interval_count = item.MonthExp ,
                                interval_unit ="MONTH"
                            },
                            tenure_type ="REGULAR",
                            sequence = 1,
                            total_cycles = 999,
                            pricing_scheme = new PricingScheme(){
                                fixed_price = new FixedPrice(){
                                    currency_code ="USD",
                                    value = amount_USD.ToString()
                                }
                            }
                        }
                    },
                    payment_preferences = new PaymentPreferences()
                    {
                        auto_bill_outstanding = true,
                        payment_failure_threshold = 1,
                        setup_fee = new SetupFee()
                        {
                            currency_code = "USD",
                            value = "0"
                        },
                        setup_fee_failure_action = "CONTINUE",
                    },
                    status = "ACTIVE",
                    taxes = new Taxes()
                    {
                        inclusive = false,
                        percentage = "0"
                    }

                };


                var createPlanResponse = await client.CreatePlan(createPlanRequest);

                if (createPlanResponse.id is null || String.Empty.Equals(createPlanResponse.id))
                {
                    throw new Exception("Liên kết ví paypal thất bại");
                }

                item.PayPalPlanID = createPlanResponse.id;

                // ví stripe
                var paymentMethodType = await paymentMethodTypeService.GetSingleAsync(d=>d.Code == (int)CoreContants.PaymentMethodType.StripeSubscription);
                if (paymentMethodType == null) throw new Exception("Liên kết ví stripe thất bại");

                var paymentMethodConfig = await paymentMethodConfigurationService.GetSingleAsync(d=> d.PaymentMethodID == paymentMethodType.Id);
                if (paymentMethodConfig == null) throw new Exception("Liên kết ví stripe thất bại");

                StripeConfiguration.ApiKey = paymentMethodConfig.serectkey;
                var options = new ProductCreateOptions
                {
                   Name = item.Name,
                   Description = item.Description,
                   Type = "service",
                   
                };
                var service = new ProductService();
                var product = service.Create(options);
                
                if(product.Id is null || String.Empty.Equals(product.Id) )
                    throw new Exception("Liên kết ví stripe thất bại");
                PriceService priceService = new PriceService();
                PriceCreateOptions priceCreateOptions = new PriceCreateOptions
                {
                    Product = product.Id,
                    UnitAmount = (long?)(amount_USD * 100),
                    Currency = "usd",
                    Recurring = new PriceRecurringOptions
                    {
                        Interval = "month",
                        IntervalCount = item.MonthExp
                    }
                };
                Price price = priceService.Create(priceCreateOptions);
                if (price.Id is null || String.Empty.Equals(price.Id))
                    throw new Exception("Liên kết ví stripe thất bại");
                item.PriceStripeID = price.Id;

                /////////////////////////////

                item.AccessPackageTypes = item.PackageType.ToString();
                item.AccessBrand = 0; // mặc định là truy cập tất cả 
                item.AccessModel = 0;
                
                item.AccessLineOff = (int)AccessLineOff.All;

                //if(item.PackageType == (int)PackageContractType.Car)
                //    item.AccessLineOffTime = 2; // giới hạn 2 năm gần nhất
                //if (item.PackageType == (int)PackageContractType.Car)
                //    item.AccessLineOffTime = 0; // giới hạn 2 năm gần nhất

                item.AccessMaterial = 0;
                item.AccessMaterialSub = 0;
                item.AccessDTC = 0;
            }
            return await base.CreateAsync(items);
        }
        public async override Task<bool> UpdateAsync(Package item)
        {
                if (item.PackageType != (int)PackageContractType.Car && item.PackageType != (int)PackageContractType.Truck) throw new Exception("Không cung cấp loại gói này");
                //đổi giá gói sang USD
                var exchangeRates = await currencyExchangeRate.GetAsync(d => d.ExchangeType == (int)ExchangeType.USD && d.Deleted == false);
                if (exchangeRates is null || exchangeRates.Count() == 0)
                {
                    throw new Exception("Không tìm được đơn vị quy đổi");
                }
                var exchangeRate = exchangeRates.FirstOrDefault();
                decimal amount_USD = (item.Price / exchangeRate.AmountVN) * exchangeRate.AmountType;

            #region paypal
            // tạo product id và plan id cho paypal
            // tạo product
            var client = new PayPalClientApi();
            var authorizationReponse = await client.GetAuthorizationRequest();
            client.SetToken(authorizationReponse.access_token);
            var response = await client.GetAuthorizationRequest();
            client.SetToken(response.access_token);

            var createProductRequest = new CreateProductRequest()
            {
                id = Timestamp.Now().ToString(),
                name = item.Name,
                type = "SERVICE",
                description = item.Description,
                category = "ACADEMIC_SOFTWARE",
                image_url = null,
                home_url = null
            };
            var createProductResponse = await client.CreateProduct(createProductRequest);
            if (createProductResponse.id is null || String.Empty.Equals(createProductResponse.id))
            {
                throw new Exception("Liên kết ví paypal thất bại");
            }

            // tạo plan
            var createPlanRequest = new CreatePlanRequest()
            {
                product_id = createProductResponse.id,
                name = item.Name,
                description = item.Description,
                billing_cycles = new List<BillingCycle>() {
                    new BillingCycle(){
                        frequency = new Frequency(){
                            interval_count = item.MonthExp ,
                            interval_unit ="MONTH"
                        },
                        tenure_type ="REGULAR",
                        sequence = 1,
                        total_cycles = 999,
                        pricing_scheme = new PricingScheme(){
                            fixed_price = new FixedPrice(){
                                currency_code ="USD",
                                value = amount_USD.ToString()
                            }
                        }
                    }
                },
                payment_preferences = new PaymentPreferences()
                {
                    auto_bill_outstanding = true,
                    payment_failure_threshold = 1,
                    setup_fee = new SetupFee()
                    {
                        currency_code = "USD",
                        value = "0"
                    },
                    setup_fee_failure_action = "CONTINUE",
                },
                status = "ACTIVE",
                taxes = new Taxes()
                {
                    inclusive = false,
                    percentage = "0"
                }

            };


            var createPlanResponse = await client.CreatePlan(createPlanRequest);

            if (createPlanResponse.id is null || String.Empty.Equals(createPlanResponse.id))
            {
                throw new Exception("Liên kết ví paypal thất bại");
            }

            item.PayPalPlanID = createPlanResponse.id;
            #endregion

            #region stripe
            // ví stripe
            var paymentMethodType = await paymentMethodTypeService.GetSingleAsync(d => d.Code == (int)CoreContants.PaymentMethodType.StripeSubscription);
            if (paymentMethodType == null) throw new Exception("Liên kết ví stripe thất bại");

            var paymentMethodConfig = await paymentMethodConfigurationService.GetSingleAsync(d => d.PaymentMethodID == paymentMethodType.Id);
            if (paymentMethodConfig == null) throw new Exception("Liên kết ví stripe thất bại");

            StripeConfiguration.ApiKey = paymentMethodConfig.serectkey;
            var options = new ProductCreateOptions
            {
                Name = item.Name,
                Description = item.Description,
                Type = "service",

            };
            var service = new ProductService();
            var product = service.Create(options);

            if (product.Id is null || String.Empty.Equals(product.Id))
                throw new Exception("Liên kết ví stripe thất bại");
            PriceService priceService = new PriceService();
            PriceCreateOptions priceCreateOptions = new PriceCreateOptions
            {
                Product = product.Id,
                UnitAmount = (long?)(amount_USD * 100),
                Currency = "usd",
                Recurring = new PriceRecurringOptions
                {
                    Interval = "month",
                    IntervalCount = item.MonthExp
                }
            };
            Price price = priceService.Create(priceCreateOptions);
            if (price.Id is null || String.Empty.Equals(price.Id))
                throw new Exception("Liên kết ví stripe thất bại");
            item.PriceStripeID = price.Id;
            #endregion
            /////////////////////////////
            item.AccessPackageTypes = item.PackageType.ToString();
            item.AccessBrand = 0; // mặc định là truy cập tất cả 
            item.AccessModel = 0;

            item.AccessLineOff = (int)AccessLineOff.All;

            //if(item.PackageType == (int)PackageContractType.Car)
            //    item.AccessLineOffTime = 2; // giới hạn 2 năm gần nhất
            //if (item.PackageType == (int)PackageContractType.Car)
            //    item.AccessLineOffTime = 0; // giới hạn 2 năm gần nhất

            item.AccessMaterial = 0;
            item.AccessMaterialSub = 0;
            item.AccessDTC = 0;
            return await base.UpdateAsync(item);
        }
    }
}
