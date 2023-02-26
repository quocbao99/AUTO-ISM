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
    /// Quản lý Car
    /// </summary>
    //[Route("api/car")]
    //[ApiController]
    //[Description("Quản lý xe")]
    //[Authorize]
    //public class CarController : BaseController<Car, CarModel, CarCreate, CarUpdate, CarSearch>
    //{
    //    public CarController(IServiceProvider serviceProvider, ILogger<BaseController<Car, CarModel, CarCreate, CarUpdate, CarSearch>> logger
    //        , IWebHostEnvironment env) : base(serviceProvider, logger, env)
    //    {
    //        this.domainService = serviceProvider.GetRequiredService<ICarService>();
    //    }
    //}
}