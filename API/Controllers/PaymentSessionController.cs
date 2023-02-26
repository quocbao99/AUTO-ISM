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
using static Utilities.CoreContants;

namespace BaseAPI.Controllers
{
    /// <summary>
    /// Quản lý Thanh toán ủy quyền
    /// </summary>
    [Route("api/payment-session")]
    [ApiController]
    [Description("Quản lý thanh toán ủy quyền")]
    [Authorize]
    public class PaymentSessionController : BaseController<PaymentSession, PaymentSessionModel, PaymentSessionCreate, PaymentSessionUpdate, PaymentSessionSearch>
    {
        private IPaymentByMomoService paymentByMomoService;
        private IPaymentByVnPayService paymentByVnPayService;
        private IPaymentByPaypalService paymentByPaypalService;
        private IPaymentByStripeService paymentByStripeService;
        public PaymentSessionController(IServiceProvider serviceProvider, ILogger<BaseController<PaymentSession, PaymentSessionModel, PaymentSessionCreate, PaymentSessionUpdate, PaymentSessionSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPaymentSessionService>();
            this.paymentByMomoService = serviceProvider.GetRequiredService<IPaymentByMomoService>();
            this.paymentByVnPayService = serviceProvider.GetRequiredService<IPaymentByVnPayService>();
            this.paymentByPaypalService = serviceProvider.GetRequiredService<IPaymentByPaypalService>();
            this.paymentByStripeService = serviceProvider.GetRequiredService<IPaymentByStripeService>();
        }
    }
}