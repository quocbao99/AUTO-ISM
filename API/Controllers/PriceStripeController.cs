using Entities;
using Entities.Paypal;
using Entities.Search;
using Entities.Stripe;
using Extensions;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Models.Paypal;
using Models.Stripe;
using Newtonsoft.Json;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Request.RequestCreate;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CatalogueEnums;

namespace BaseAPI.Controllers
{
    /// <summary>
    /// Quản lý price stripe
    /// </summary>
    [Route("api/pricestripe")]
    [ApiController]
    [Description("Quản lý giá stripe ")]
    [Authorize]
    public class PriceStripeController : BaseController<PriceStripe, PriceStripeModel, PriceStripeCreate, PriceStripeUpdate, PriceStripeSearch>
    {
        public PriceStripeController(IServiceProvider serviceProvider, ILogger<BaseController<PriceStripe, PriceStripeModel, PriceStripeCreate, PriceStripeUpdate, PriceStripeSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPriceStripeService>();
        }
    }
}