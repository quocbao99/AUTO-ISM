using Entities;
using Entities.Paypal;
using Entities.Search;
using Extensions;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Models.Paypal;
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
    /// Quản lý plan Paypal
    /// </summary>
    [Route("api/planpaypal")]
    [ApiController]
    [Description("Quản lý kế hoạch thanh toán paypal")]
    [Authorize]
    public class PlanPaypalController : BaseController<PlanPaypal, PlanPaypalModel, PlanPaypalCreate, PlanPaypalUpdate, PlanPaypalSearch>
    {
        public PlanPaypalController(IServiceProvider serviceProvider, ILogger<BaseController<PlanPaypal, PlanPaypalModel, PlanPaypalCreate, PlanPaypalUpdate, PlanPaypalSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPlanPaypalService>();
        }
    }
}