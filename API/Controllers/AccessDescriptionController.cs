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
    /// Mô tả quyền truy cập của gói
    /// </summary>
    [Route("api/access-description")]
    [ApiController]
    [Description("mô tả quyền truy cập")]
    [Authorize]
    public class AccessDescriptionController : BaseController<AccessDescription, AccessDescriptionModel, AccessDescriptionCreate, AccessDescriptionUpdate, AccessDescriptionSearch>
    {
        public AccessDescriptionController(IServiceProvider serviceProvider, ILogger<BaseController<AccessDescription, AccessDescriptionModel, AccessDescriptionCreate, AccessDescriptionUpdate, AccessDescriptionSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IAccessDescriptionService>();
        }
        public async override Task<AppDomainResult> UpdateItem([FromBody] AccessDescriptionUpdate itemModel)
        {
            if (ModelState.IsValid)
            {
                var item = mapper.Map<AccessDescription>(itemModel);
                item.WiringDiagram = System.Text.Json.JsonSerializer.Serialize(itemModel.WiringDiagram).ToString();
                item.Specifications = System.Text.Json.JsonSerializer.Serialize(itemModel.Specifications).ToString();
                item.DTCSearch = System.Text.Json.JsonSerializer.Serialize(itemModel.DTCSearch).ToString();
                item.TimingChain_Belt = System.Text.Json.JsonSerializer.Serialize(itemModel.TimingChain_Belt).ToString();
                item.TransmissionManual = System.Text.Json.JsonSerializer.Serialize(itemModel.TransmissionManual).ToString();
                item.TroubleShootingGuide = System.Text.Json.JsonSerializer.Serialize(itemModel.TroubleShootingGuide).ToString();
                if (item != null)
                {
                    var jtem = await this.domainService.GetByIdAsync(item.Id);

                    bool success = await this.domainService.UpdateAsync(item);
                    if (!success)
                        throw new Exception(ApiMessage.HandlingError);
                    return new AppDomainResult() { Success = true, ResultMessage = ApiMessage.SuccessfulPut, ResultCode = (int)HttpStatusCode.OK };
                }
                else
                    throw new KeyNotFoundException(ApiMessage.ItemNotFound);
            }
            throw new AppException(ModelState.GetErrorMessage());
        }
        public async override Task<AppDomainResult> AddItem([FromBody] AccessDescriptionCreate itemModel)
        {
            if (ModelState.IsValid)
            {
                AccessDescription item = mapper.Map<AccessDescription>(itemModel);
                item.WiringDiagram = System.Text.Json.JsonSerializer.Serialize(itemModel.WiringDiagram).ToString();
                item.Specifications = System.Text.Json.JsonSerializer.Serialize(itemModel.Specifications).ToString();
                item.DTCSearch = System.Text.Json.JsonSerializer.Serialize(itemModel.DTCSearch).ToString();
                item.TimingChain_Belt = System.Text.Json.JsonSerializer.Serialize(itemModel.TimingChain_Belt).ToString();
                item.TransmissionManual = System.Text.Json.JsonSerializer.Serialize(itemModel.TransmissionManual).ToString();
                item.TroubleShootingGuide = System.Text.Json.JsonSerializer.Serialize(itemModel.TroubleShootingGuide).ToString();
                if (item != null)
                {
                    bool success = await this.domainService.CreateAsync(item);
                    if (!success)
                        throw new Exception(ApiMessage.HandlingError);
                    return new AppDomainResult() { Success = true, ResultMessage = ApiMessage.SuccessfulPost, ResultCode = (int)HttpStatusCode.OK };
                }
                else
                    throw new AppException(ApiMessage.ItemNotFound);
            }
            throw new AppException(ModelState.GetErrorMessage());
        }
    }
}