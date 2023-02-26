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
    /// Quản lý dòng đời
    /// </summary>
    [Route("api/lineoff")]
    [ApiController]
    [Description("Quản lý dòng đời")]
    [Authorize]
    public class LineOffController : BaseController<LineOff, LineOffModel, LineOffCreate, LineOffUpdate, LineOffSearch>
    {
        public IEModelService eModelService;
        IContractSpecializingService contractSpecializingService;
        public LineOffController(IServiceProvider serviceProvider, ILogger<BaseController<LineOff, LineOffModel, LineOffCreate, LineOffUpdate, LineOffSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<ILineOffService>();
            this.eModelService = serviceProvider.GetRequiredService<IEModelService>();
            this.contractSpecializingService = serviceProvider.GetRequiredService<IContractSpecializingService>();
        }
        public async override Task<AppDomainResult> GetById(Guid id)
        {
            LineOff item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                var eModel = await eModelService.GetByIdAsync(item.EModelID);
                if (eModel == null) throw new Exception("Không tìm thấy Model của line-off");
                item.EModelName = eModel.Name;
                var user = LoginContext.Instance.CurrentUser;
                if (user.trial == true || user.isAdmin == true)
                {
                    var itemModel = mapper.Map<LineOff>(item);
                    return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                }
                else if (user.OpenCar == true || user.OpenTruct == true) {
                    var Emodel = await eModelService.GetByIdAsync(item.EModelID);
                    var itemModel = mapper.Map<LineOffModel>(item);
                    if (user.OpenCar == true) {
                        if (Emodel.EmodelType == (int)EModelType.Truck) {
                            itemModel.isLock = true;
                        }
                    }
                    if (user.OpenTruct == true) {
                        if (Emodel.EmodelType == (int)EModelType.Car)
                        {
                            itemModel.isLock = true;
                        }
                    }
                    if (user.OpenTruct == true && user.OpenTruct == true)
                    {
                        if (Emodel.EmodelType == (int)EModelType.Car)
                        {
                            itemModel.isLock = true;
                        }
                    }
                    return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                }
                else{
                    var contracts = await contractSpecializingService.ContractsIsUsing(user.userId);
                    var Emodel = await eModelService.GetByIdAsync(item.EModelID);
                    var itemModel = mapper.Map<LineOffModel>(item);
                    if (contracts.Count() == 0)
                    { // Không có hợp đồng
                        if (Emodel.EmodelType == (int)EModelType.Car && (item.Year - DateTime.Now.Year) < 2 ) // 2 năm gần nhất
                        {
                            itemModel.isLock = true;
                        }
                        if (Emodel.EmodelType == (int)EModelType.Truck)
                        {
                            itemModel.isLock = true;
                        }
                        return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                    }

                    if (contracts.Count() > 0)
                    { // có hợp đồng
                        
                        if (Emodel.EmodelType == (int)EModelType.Car && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Car))
                        {
                            itemModel.isLock = true;
                        }
                        if (Emodel.EmodelType == (int)EModelType.Truck && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Truck))
                        {
                            itemModel.isLock = true;
                        }
                        return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                    }
                }
            }
            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
        }

        public async override Task<AppDomainResult> Get([FromQuery] LineOffSearch baseSearch)
        {
            var user = LoginContext.Instance.CurrentUser;
            if (user.trial == true || user.isAdmin == true)
            {

                return await base.Get(baseSearch);
            }
            else if (user.OpenCar == true || user.OpenTruct == true)
            {
                PagedList<LineOff> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<LineOffModel> pagedDataModel = mapper.Map<PagedList<LineOffModel>>(pagedData);
                if (pagedDataModel.Items is not null) {
                    for (int i = 0; i < pagedDataModel.Items.Count(); i++)
                    {
                        pagedDataModel.Items[i].isLock = false;
                        var Emodel = await eModelService.GetByIdAsync(pagedDataModel.Items[i].EModelID);
                        if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");

                        if (user.OpenCar == true)
                        {
                            if (Emodel.EmodelType == (int)EModelType.Truck)
                            {
                                pagedDataModel.Items[i].isLock = true;
                            }
                        }
                        if (user.OpenTruct == true)
                        {
                            if (Emodel.EmodelType == (int)EModelType.Car)
                            {
                                pagedDataModel.Items[i].isLock = true;
                            }
                        }
                        if (user.OpenTruct == true && user.OpenTruct == true)
                        {
                            if (Emodel.EmodelType == (int)EModelType.Car)
                            {
                                pagedDataModel.Items[i].isLock = true;
                            }
                        }
                    }
                }
                
                return new AppDomainResult() { Success = true, Data = pagedDataModel, ResultCode = (int)HttpStatusCode.OK };
            }
            else
            {
                var contracts = await contractSpecializingService.ContractsIsUsing(user.userId);
                PagedList<LineOff> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<LineOffModel> pagedDataModel = mapper.Map<PagedList<LineOffModel>>(pagedData);
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
                        pagedDataModel.Items[i].isLock = false;
                        var Emodel = await eModelService.GetByIdAsync(pagedDataModel.Items[i].EModelID);
                        if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");
                        if (Emodel.EmodelType == (int)EModelType.Car && (DateTime.Now.Year - pagedDataModel.Items[i].Year) < 2)
                        {
                            pagedDataModel.Items[i].isLock = true;
                        }
                        if (Emodel.EmodelType == (int)EModelType.Truck)
                        {
                            pagedDataModel.Items[i].isLock = true;
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
                        pagedDataModel.Items[i].isLock = false;
                        var Emodel = await eModelService.GetByIdAsync(pagedDataModel.Items[i].EModelID);
                        if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");
                        if (Emodel.EmodelType == (int)EModelType.Car && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Car) && (DateTime.Now.Year - pagedDataModel.Items[i].Year) < 2)
                        {
                            pagedDataModel.Items[i].isLock = true;
                        }
                        if (Emodel.EmodelType == (int)EModelType.Truck && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Truck))
                        {
                            pagedDataModel.Items[i].isLock = true;
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
    }
}