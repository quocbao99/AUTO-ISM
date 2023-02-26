using Entities;
using Entities.Search;
using Extensions;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
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
    /// Quản lý kiểu Thanh toán
    /// </summary>
    [Route("api/payment-method-type")]
    [ApiController]
    [Description("Quản lý kiểu thanh toán")]
    [Authorize]
    public class PaymentMethodTypeController : BaseController<PaymentMethodType, PaymentMethodTypeModel, PaymentMethodTypeCreate, PaymentMethodTypeUpdate, PaymentMethodTypeSearch>
    {
        private IPaymentByMomoService paymentByMomoService;
        public PaymentMethodTypeController(IServiceProvider serviceProvider, ILogger<BaseController<PaymentMethodType, PaymentMethodTypeModel, PaymentMethodTypeCreate, PaymentMethodTypeUpdate, PaymentMethodTypeSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPaymentMethodTypeService>();
        }
    }
}