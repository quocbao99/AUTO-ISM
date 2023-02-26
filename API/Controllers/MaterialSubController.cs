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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CatalogueEnums;

namespace BaseAPI.Controllers
{
    /// <summary>
    /// Quản lý phần tử tài liệu
    /// </summary>
    [Route("api/materialsub")]
    [ApiController]
    [Description("Quản lý phần tử tài liệu")]
    [Authorize]
    public class MaterialSubController : BaseController<MaterialSub, MaterialSubModel, MaterialSubCreate, MaterialSubUpdate, MaterialSubSearch>
    {

        public MaterialSubController(IServiceProvider serviceProvider, ILogger<BaseController<MaterialSub, MaterialSubModel, MaterialSubCreate, MaterialSubUpdate, MaterialSubSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IMaterialSubService>();
        }
        public override async Task<AppDomainResult> Get([FromQuery] MaterialSubSearch baseSearch)
        {
            if (ModelState.IsValid)
            {
                PagedList<MaterialSub> pagedData = await domainService.GetPagedListData(baseSearch);
                PagedList<MaterialSubModel> pagedDataModel = mapper.Map<PagedList<MaterialSubModel>>(pagedData);
                if (pagedDataModel.Items == null)
                {
                    return new AppDomainResult
                    {
                        Data = pagedDataModel,
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                foreach (var item in pagedDataModel.Items) { 
                     var currentLinkSite = $"{item.FileUrl.Split(":")[0]}://{item.FileUrl.Split("/")[2]}/api/materialsub/file/";
                    item.FileUrl = Path.Combine(currentLinkSite, item.Id.ToString());
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
        //GET api/download/12345abc
        [HttpGet("file/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> File(Guid id)
        {
            var materialSub = await domainService.GetByIdAsync(id);
            if (materialSub == null) throw new Exception("Không tìm thấy File");
            string path = materialSub.FilePath;
            try {
                Stream stream = System.IO.File.Open(path, System.IO.FileMode.Open);
                if (stream == null)
                    return NotFound(); // returns a NotFoundResult with Status404NotFound response.

                return File(stream, "application/pdf"); // returns a FileStreamResult
            }
            catch (Exception e) {
                throw new Exception("Lỗi hệ thống vui lòng thử lại sau");
            }
        }
    }
}