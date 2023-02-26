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
using Entities.Paypal;
using Microsoft.EntityFrameworkCore.Storage;
using PayPal;

namespace Service.Services
{
    public class PlanPaypalService : DomainService<PlanPaypal, PlanPaypalSearch>, IPlanPaypalService
    {
        protected IAppDbContext coreDbContext;
        protected IPaymentMethodConfigurationService paymentMethodConfigurationService;
        public PlanPaypalService(IAppUnitOfWork unitOfWork
            , IMapper mapper
            , IAppDbContext coreDbContext
            , IPaymentMethodConfigurationService paymentMethodConfigurationService
            ) : base(unitOfWork, mapper)
        {
            this.coreDbContext = coreDbContext;
            this.paymentMethodConfigurationService = paymentMethodConfigurationService;
        }
        protected override string GetStoreProcName()
        {
            return "PlanPaypal_GetPagingPlanPaypal";
        }
        public async override Task<bool> CreateAsync(PlanPaypal item)
        {
            // tạo plan và active plan
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                try
                {
                    // phương thức than toán
                    var paymentMethodConfig = await paymentMethodConfigurationService.GetByIdAsync((Guid)item.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");


                    var client = new PayPalClientApi();
                    var authorizationReponse = await client.GetAuthorizationRequest();
                    client.SetToken(authorizationReponse.access_token);

                    var trialBillingCycle = new BillingCycle()
                    {
                        frequency = new Frequency()
                        {
                            interval_unit = "MONTH",
                            interval_count = 1,
                        },
                        tenure_type = "TRIAL",
                        sequence = 1,
                        total_cycles = 1,
                        pricing_scheme = new PricingScheme()
                        {
                            fixed_price = new FixedPrice()
                            {
                                currency_code = "USD",
                                value = "10.00"
                            }
                        }

                    };
                    var regularBillingCycle = new BillingCycle()
                    {

                        frequency = new Frequency()
                        {
                            interval_unit = "MONTH",
                            interval_count = 1,
                        },
                        tenure_type = "REGULAR",
                        sequence = 2,
                        total_cycles = 0,
                        pricing_scheme = new PricingScheme()
                        {
                            fixed_price = new FixedPrice()
                            {
                                currency_code = "USD",
                                value = "100.00"
                            }
                        }
                    };

                    var createPlanRequest = new CreatePlanRequest()
                    {
                        product_id = "1670568338", //Product Id
                        name = "Technical Voice Plan",
                        description = "Technical Voice Plan",
                        status = "ACTIVE",
                        billing_cycles = new List<BillingCycle>()
                            {
                                trialBillingCycle,
                                regularBillingCycle
                            },
                        payment_preferences = new PaymentPreferences()
                        {
                            auto_bill_outstanding = true,
                            setup_fee = new SetupFee()
                            {
                                currency_code = "USD",
                                value = "0"
                            },
                            setup_fee_failure_action = "CONTINUE",
                            payment_failure_threshold = 3
                        }

                    };

                    var createPlanResponse = await client.CreatePlan(createPlanRequest);
                    item.PlanPaypalID = createPlanResponse.id;

                    await transaction.CommitAsync();

                   
                    return await base.CreateAsync(item);
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
