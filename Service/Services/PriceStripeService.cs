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
using Entities.Stripe;
using Stripe;

namespace Service.Services
{
    public class PriceStripeService : DomainService<PriceStripe, PriceStripeSearch>, IPriceStripeService
    {
        protected IAppDbContext coreDbContext;
        protected IPaymentMethodConfigurationService paymentMethodConfigurationService;
        public PriceStripeService(IAppUnitOfWork unitOfWork
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
        public async override Task<bool> CreateAsync(PriceStripe item)
        {
            // tạo plan và active plan
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                try
                {
                    // phương thức than toán
                    var paymentMethodConfig = await paymentMethodConfigurationService.GetByIdAsync((Guid)item.PaymentMethodConfigurationID);
                    if (paymentMethodConfig == null) throw new Exception("Không tìm thấy phương thức thanh toán");

                    StripeConfiguration.ApiKey = paymentMethodConfig.serectkey;

                    var options = new PriceCreateOptions
                    {
                        Product = "prod_NBVj0DParmqXDr",
                        UnitAmount = 10,
                        Currency = "usd",
                        Recurring = new PriceRecurringOptions { Interval = "month" },
                    };
                    var service = new PriceService();
                    var price = service.Create(options);
                    item.PriceStripeID = price.Id;
                    item.ProductID = price.ProductId;
                    await base.CreateAsync(item);
                    await transaction.CommitAsync();


                    return true;
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
