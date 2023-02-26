using Entities;
using Entities.Search;
using Extensions;
using Interface.Services;
using Interface.Services.Specializing;
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
    /// Quản lý File đường dẫn
    /// </summary>
    [Route("api/systemfile")]
    [ApiController]
    [Description("Quản lý File đường dẫn")]
    [Authorize]
    public class SystemFileController : BaseController<SystemFile, SystemFileModel, SystemFileCreate, SystemFileUpdate, SystemFileSearch>
    {
        private ILineOffService lineOffService;
        private IEModelService eModelService;
        private IContractSpecializingService contractSpecializingService;
        public SystemFileController(IServiceProvider serviceProvider, ILogger<BaseController<SystemFile, SystemFileModel, SystemFileCreate, SystemFileUpdate, SystemFileSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<ISystemFileService>();
            this.lineOffService = serviceProvider.GetRequiredService<ILineOffService>();
            this.eModelService = serviceProvider.GetRequiredService<IEModelService>();
            this.contractSpecializingService = serviceProvider.GetRequiredService<IContractSpecializingService>();
        }

        public async override Task<AppDomainResult> GetById(Guid id)
        {
            SystemFile item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                var lineoff = await lineOffService.GetByIdAsync(item.LineOffID);
                var user = LoginContext.Instance.CurrentUser;
                var itemModel = mapper.Map<SystemFileModel>(item);
                var childFiles = await this.domainService.GetAsync(d => d.ParentID == itemModel.Id && d.Active == true && d.Deleted == false);
                itemModel.CountChildFile = childFiles.Count();

                if (lineoff == null) throw new Exception("Không tìm thấy Model của line-off");
                itemModel.LineOffName = lineoff.Name;
                if (itemModel.ParentID is null)
                {
                    itemModel.ParentSystemFileName = "";
                }
                else {
                    var parentSystemFile = await domainService.GetByIdAsync((Guid)itemModel.ParentID);
                    if (parentSystemFile == null) {
                        itemModel.ParentSystemFileName = "";
                    }
                    else {
                        itemModel.ParentSystemFileName = parentSystemFile.Name;
                    }
                }
                if (user.trial == true || user.isAdmin == true)
                {
                    return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                }
                else if (user.OpenCar == true || user.OpenTruct == true) {
                    var lineOff = await lineOffService.GetByIdAsync(item.LineOffID);
                    var Emodel = await eModelService.GetByIdAsync(lineOff.EModelID);
                    if (user.OpenCar == true && user.OpenTruct == true)
                    {
                        /// không xử lý
                    }
                    else if (user.OpenTruct == true)
                    {
                        if (Emodel.EmodelType == (int)EModelType.Car)
                        {
                            itemModel = null;
                        }
                    }
                    else if (user.OpenCar == true)
                    {
                        if (Emodel.EmodelType == (int)EModelType.Truck)
                        {
                            itemModel = null;
                        }
                    }
                    return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    var contracts = await contractSpecializingService.ContractsIsUsing(user.userId);
                    var lineOff = await lineOffService.GetByIdAsync(item.LineOffID);
                    var Emodel = await eModelService.GetByIdAsync(lineOff.EModelID);
                    if (contracts.Count() == 0)
                    { // Không có hợp đồng
                        if (item != null)
                        {
                            if (Emodel.EmodelType == (int)EModelType.Car && (lineOff.Year - DateTime.Now.Year) < 2) // 2 năm gần nhất
                            {
                                throw new KeyNotFoundException(ApiMessage.ItemNotFound);
                            }
                            if (Emodel.EmodelType == (int)EModelType.Truck)
                            {
                                throw new KeyNotFoundException(ApiMessage.ItemNotFound);
                            }
                            return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                        }
                    }

                    if (contracts.Count() > 0)
                    { // có hợp đồng

                        if (Emodel.EmodelType == (int)EModelType.Car && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Car))
                        {
                            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
                        }
                        if (Emodel.EmodelType == (int)EModelType.Truck && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Truck))
                        {
                            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
                        }
                        return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                    }
                }
            }
            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
        }

        public async override Task<AppDomainResult> Get([FromQuery] SystemFileSearch baseSearch)
        {
            var user = LoginContext.Instance.CurrentUser;
            if (user.trial == true || user.isAdmin == true)
            {
                if (ModelState.IsValid)
                {
                    PagedList<SystemFile> pagedData = await this.domainService.GetPagedListData(baseSearch);
                    PagedList<SystemFileModel> pagedDataModel = mapper.Map<PagedList<SystemFileModel>>(pagedData);
                    for (int i = 0; i < pagedDataModel.Items.Count(); i++)
                    {
                        var childFiles = await this.domainService.GetAsync(d => d.ParentID == pagedDataModel.Items[i].Id && d.Active == true && d.Deleted == false);
                        pagedDataModel.Items[i].CountChildFile = childFiles.Count();
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
            else if (user.OpenCar == true || user.OpenTruct == true)
            {
                PagedList<SystemFile> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<SystemFileModel> pagedDataModel = mapper.Map<PagedList<SystemFileModel>>(pagedData);
                if (pagedDataModel.Items is not null) {
                    for (int i = 0; i < pagedDataModel.Items.Count(); i++)
                    {
                        var childFiles = await this.domainService.GetAsync(d => d.ParentID == pagedDataModel.Items[i].Id && d.Active == true && d.Deleted == false);
                        pagedDataModel.Items[i].CountChildFile = childFiles.Count();

                        var lineOff = await lineOffService.GetByIdAsync(pagedDataModel.Items[i].LineOffID);
                        var Emodel = await eModelService.GetByIdAsync(lineOff.EModelID);
                        if (user.OpenCar == true && user.OpenTruct == true)
                        {
                            /// không xử lý
                        }
                        else if (user.OpenTruct == true)
                        {
                            if (Emodel.EmodelType == (int)EModelType.Car)
                            {
                                pagedDataModel.Items.Remove(pagedDataModel.Items[i]);
                                pagedDataModel.TotalItem = pagedDataModel.TotalItem - 1;
                            }
                        }
                        else if (user.OpenCar == true)
                        {
                            if (Emodel.EmodelType == (int)EModelType.Truck)
                            {
                                pagedDataModel.Items.Remove(pagedDataModel.Items[i]);
                                pagedDataModel.TotalItem = pagedDataModel.TotalItem - 1;
                            }
                        }

                    }
                }
                
                return new AppDomainResult() { Success = true, Data = pagedDataModel, ResultCode = (int)HttpStatusCode.OK };
            }
            else
            {
                var contracts = await contractSpecializingService.ContractsIsUsing(user.userId);
                PagedList<SystemFile> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<SystemFileModel> pagedDataModel = mapper.Map<PagedList<SystemFileModel>>(pagedData);
                if (pagedData.Items == null)
                {
                    return new AppDomainResult
                    {
                        Data = pagedDataModel,
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                if (contracts.Count() == 0)
                { // Không có hợp đồng
                    for (int i = 0; i < pagedDataModel.Items.Count(); i++)
                    {
                        var childFiles = await this.domainService.GetAsync(d => d.ParentID == pagedDataModel.Items[i].Id && d.Active == true && d.Deleted == false);
                        pagedDataModel.Items[i].CountChildFile = childFiles.Count();
                        var lineOff = await lineOffService.GetByIdAsync(pagedDataModel.Items[i].LineOffID);
                        var Emodel = await eModelService.GetByIdAsync(lineOff.EModelID);
                        if (Emodel.EmodelType == (int)EModelType.Car && ( DateTime.Now.Year - lineOff.Year) < 2)
                        {
                            pagedDataModel.Items.Remove(pagedDataModel.Items[i]);
                            pagedDataModel.TotalItem = pagedDataModel.TotalItem - 1;
                        }
                        if (Emodel.EmodelType == (int)EModelType.Truck)
                        {
                            pagedDataModel.Items.Remove(pagedDataModel.Items[i]);
                            pagedDataModel.TotalItem = pagedDataModel.TotalItem - 1;

                        }
                    }
                    return new AppDomainResult
                    {
                        Data = pagedDataModel,
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }

                if (contracts.Count() > 0)
                { // có hợp đồng
                    for (int i = 0; i < pagedDataModel.Items.Count(); i++)
                    {
                        var childFiles = await this.domainService.GetAsync(d => d.ParentID == pagedDataModel.Items[i].Id && d.Active == true && d.Deleted == false);
                        pagedDataModel.Items[i].CountChildFile = childFiles.Count();
                        var lineOff = await lineOffService.GetByIdAsync(pagedDataModel.Items[i].LineOffID);
                        var Emodel = await eModelService.GetByIdAsync(lineOff.EModelID);
                        if (Emodel.EmodelType == (int)EModelType.Car && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Car) && ( DateTime.Now.Year - lineOff.Year ) < 2)
                        {
                            pagedDataModel.Items.Remove(pagedDataModel.Items[i]);
                            pagedDataModel.TotalItem = pagedDataModel.TotalItem - 1;

                        }
                        if (Emodel.EmodelType == (int)EModelType.Truck && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Truck))
                        {
                            pagedDataModel.Items.Remove(pagedDataModel.Items[i]);
                            pagedDataModel.TotalItem = pagedDataModel.TotalItem - 1;

                        }
                    }
                    return new AppDomainResult
                    { 
                        Data = pagedDataModel,
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
            }
            throw new Exception("Lỗi hệ thống");
        }

        public override Task<AppDomainResult> AddItem([FromBody] SystemFileCreate itemModel)
        {
            return base.AddItem(itemModel);
        }
    }
}