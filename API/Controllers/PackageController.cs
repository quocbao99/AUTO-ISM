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
    /// Quản lý Gói cước
    /// </summary>
    [Route("api/package")]
    [ApiController]
    [Description("Quản lý gói cước")]
    [Authorize]
    public class PackageController : BaseController<Package, PackageModel, PackageCreate, PackageUpdate, PackageSearch>
    {
        private ICurrencyExchangeRateService currencyExchangeRate;
        public PackageController(IServiceProvider serviceProvider, ILogger<BaseController<Package, PackageModel, PackageCreate, PackageUpdate, PackageSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPackageService>();
            this.currencyExchangeRate = serviceProvider.GetRequiredService<ICurrencyExchangeRateService>();
        }
        public override async Task<AppDomainResult> Get([FromQuery] PackageSearch baseSearch)
        {

            if (ModelState.IsValid)
            {
                //đổi giá gói sang USD
                var exchangeRates = await currencyExchangeRate.GetAsync(d => d.ExchangeType == (int)ExchangeType.USD && d.Deleted == false);
                if (exchangeRates is null || exchangeRates.Count() == 0)
                {
                    throw new Exception("Không tìm được đơn vị quy đổi");
                }
                var exchangeRate = exchangeRates.FirstOrDefault();
                //
                PagedList<Package> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<PackageModel> pagedDataModel = mapper.Map<PagedList<PackageModel>>(pagedData);
                if (pagedDataModel is null) return await base.Get(baseSearch);
                for (int i = 0; i < pagedDataModel.Items.Count(); i++) {
                    pagedDataModel.Items[i].PriceUSD = (pagedDataModel.Items[i].Price / exchangeRate.AmountVN) * exchangeRate.AmountType;
                }
                return new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
        }
        public override Task<AppDomainResult> GetById(Guid id)
        {
            return base.GetById(id);
        }
    }
}