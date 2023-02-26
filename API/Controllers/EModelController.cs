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
    /// Quản lý phiên bản
    /// </summary>
    [Route("api/model")]
    [ApiController]
    [Description("Quản lý Phiên bản")]
    [Authorize]
    public class EModelController : BaseController<EModel, EModelModel, EModelCreate, EModelUpdate, EModelSearch>
    {
        private IBrandService brandService;
        public EModelController(IServiceProvider serviceProvider, ILogger<BaseController<EModel, EModelModel, EModelCreate, EModelUpdate, EModelSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IEModelService>();
            this.brandService = serviceProvider.GetRequiredService<IBrandService>();
        }

        public async override Task<AppDomainResult> GetById(Guid id)
        {
            EModel item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                var brand = await brandService.GetByIdAsync(item.BrandID);
                if (brand == null) throw new Exception("Không tìm thấy Model của line-off");
                item.BrandName = brand.Name;
                var itemModel = mapper.Map<EModel>(item);
                return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
            }
            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
        }
    }
}