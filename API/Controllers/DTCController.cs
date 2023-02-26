using ClosedXML.Excel;
using Entities;
using Entities.Search;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.Services.Specializing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
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
using static Utilities.CoreContants;

namespace BaseAPI.Controllers
{
    /// <summary>
    /// Quản lý DTC
    /// </summary>
    [Route("api/dtc")]
    [ApiController]
    [Description("Quản lý DTC")]
    [Authorize]
    public class DTCController : BaseController<DTC, DTCModel, DTCCreate, DTCUpdate, DTCSearch>
    {
        IEModelService eModelService;
        IBrandService brandService;
        IContractSpecializingService contractSpecializingService;
        private IAppDbContext coreDbContext;
        public DTCController(IServiceProvider serviceProvider, ILogger<BaseController<DTC, DTCModel, DTCCreate, DTCUpdate, DTCSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IDTCService>();
            this.contractSpecializingService = serviceProvider.GetRequiredService<IContractSpecializingService>();
            this.brandService = serviceProvider.GetRequiredService<IBrandService>();
            this.eModelService = serviceProvider.GetRequiredService<IEModelService>();
            this.coreDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
        }
        public async override Task<AppDomainResult> Get([FromQuery] DTCSearch baseSearch)
        {
            var user = LoginContext.Instance.CurrentUser;
            if (user.trial == true || user.isAdmin == true) {

                if (ModelState.IsValid)
                {
                    PagedList<DTC> pagedData = await this.domainService.GetPagedListData(baseSearch);
                    PagedList<DTCModel> pagedDataModel = mapper.Map<PagedList<DTCModel>>(pagedData);
                    if (pagedDataModel.Items is not null) {
                        for (int i = 0; i < pagedDataModel.Items.Count(); i++)
                        {
                            var Emodel = await eModelService.GetByIdAsync(pagedData.Items[i].EModelID);
                            if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");

                            var brand = await brandService.GetByIdAsync(Emodel.BrandID);
                            if (brand is null) throw new Exception("Không tìm thấy thông tin brand!");

                            pagedDataModel.Items[i].EModelName = Emodel.Name;
                            pagedDataModel.Items[i].BrandID = brand.Id;
                            pagedDataModel.Items[i].BrandName = brand.Name;
                        }
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
                PagedList<DTC> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<DTCModel> pagedDataModel = mapper.Map<PagedList<DTCModel>>(pagedData);
                if (pagedDataModel.Items is not null)
                {
                    for (int i = 0; i < pagedDataModel.Items.Count(); i++)
                    {
                        var Emodel = await eModelService.GetByIdAsync(pagedData.Items[i].EModelID);
                        if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");

                        var brand = await brandService.GetByIdAsync(Emodel.BrandID);
                        if (brand is null) throw new Exception("Không tìm thấy thông tin brand!");

                        pagedDataModel.Items[i].EModelName = Emodel.Name;
                        pagedDataModel.Items[i].BrandID = brand.Id;
                        pagedDataModel.Items[i].BrandName = brand.Name;

                        if (user.OpenCar == true && user.OpenTruct == true)
                        {
                            /// không xử lý
                        }
                        else if (user.OpenTruct == true)
                        {
                            if (Emodel.EmodelType == (int)EModelType.Car)
                            {
                                pagedDataModel.Items[i].EModelName = Emodel.Name;
                                pagedDataModel.Items[i].BrandID = brand.Id;
                                pagedDataModel.Items[i].BrandName = brand.Name;
                                pagedDataModel.Items[i].PossibleCause = MessageBuyPackageForDetail;
                                pagedDataModel.Items[i].ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                                pagedDataModel.Items[i].ReferenceLocation = MessageBuyPackageForDetail;
                                pagedDataModel.Items[i].Description = MessageBuyPackageForDetail;
                            }
                        }
                        else if (user.OpenCar == true)
                        {
                            if (Emodel.EmodelType == (int)EModelType.Truck)
                            {
                                pagedDataModel.Items[i].EModelName = Emodel.Name;
                                pagedDataModel.Items[i].BrandID = brand.Id;
                                pagedDataModel.Items[i].BrandName = brand.Name;
                                pagedDataModel.Items[i].PossibleCause = MessageBuyPackageForDetail;
                                pagedDataModel.Items[i].ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                                pagedDataModel.Items[i].ReferenceLocation = MessageBuyPackageForDetail;
                                pagedDataModel.Items[i].Description = MessageBuyPackageForDetail;
                            }
                        }

                    }
                }
                return new AppDomainResult() { Success = true, Data = pagedDataModel, ResultCode = (int)HttpStatusCode.OK };
            }
            else {
                var contracts = await contractSpecializingService.ContractsIsUsing(user.userId);
                PagedList<DTC> pagedData = await this.domainService.GetPagedListData(baseSearch);

                if (pagedData.Items == null) {
                    PagedList<DTCModel> pagedDataModel = mapper.Map<PagedList<DTCModel>>(pagedData);
                    return new AppDomainResult
                    {
                        Data = pagedDataModel,
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                if (contracts.Count() == 0) { // Không có hợp đồng
                    for (int i = 0; i < pagedData.Items.Count(); i++) {
                        var Emodel = await eModelService.GetByIdAsync(pagedData.Items[i].EModelID);
                        if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");

                        var brand = await brandService.GetByIdAsync(Emodel.BrandID);
                        if (brand is null) throw new Exception("Không tìm thấy thông tin brand!");
                        if (Emodel.EmodelType == (int)EModelType.Car ) {
                            pagedData.Items[i].EModelName = Emodel.Name;
                            pagedData.Items[i].BrandID = brand.Id;
                            pagedData.Items[i].BrandName = brand.Name;
                            pagedData.Items[i].PossibleCause = MessageBuyPackageForDetail;
                            pagedData.Items[i].ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                            pagedData.Items[i].ReferenceLocation = MessageBuyPackageForDetail;
                            pagedData.Items[i].Description = MessageBuyPackageForDetail;
                        }
                        if (Emodel.EmodelType == (int)EModelType.Truck)
                        {
                            pagedData.Items[i].EModelName = Emodel.Name;
                            pagedData.Items[i].BrandID = brand.Id;
                            pagedData.Items[i].BrandName = brand.Name;
                            pagedData.Items[i].PossibleCause = MessageBuyPackageForDetail;
                            pagedData.Items[i].ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                            pagedData.Items[i].ReferenceLocation = MessageBuyPackageForDetail;
                            pagedData.Items[i].Description = MessageBuyPackageForDetail;
                        }
                    }
                    PagedList<DTCModel> pagedDataModel = mapper.Map<PagedList<DTCModel>>(pagedData);
                    return new AppDomainResult
                    {
                        Data = pagedDataModel,
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }

                if (contracts.Count() > 0)
                { // có hợp đồng
                    for (int i = 0; i < pagedData.Items.Count(); i++)
                    {
                        var Emodel = await eModelService.GetByIdAsync(pagedData.Items[i].EModelID);
                        if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");
                        var brand = await brandService.GetByIdAsync(Emodel.BrandID);
                        if (brand is null) throw new Exception("Không tìm thấy thông tin brand!");
                        if (Emodel.EmodelType == (int)EModelType.Car && !contracts.Select(d=>d.ContractType).Contains((int)PackageContractType.Car))
                        {
                            pagedData.Items[i].EModelName = Emodel.Name;
                            pagedData.Items[i].BrandID = brand.Id;
                            pagedData.Items[i].BrandName = brand.Name;
                            pagedData.Items[i].PossibleCause = MessageBuyPackageForDetail;
                            pagedData.Items[i].ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                            pagedData.Items[i].ReferenceLocation = MessageBuyPackageForDetail;
                            pagedData.Items[i].Description = MessageBuyPackageForDetail;
                        }
                        if (Emodel.EmodelType == (int)EModelType.Truck && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Truck))
                        {
                            pagedData.Items[i].EModelName = Emodel.Name;
                            pagedData.Items[i].BrandID = brand.Id;
                            pagedData.Items[i].BrandName = brand.Name;
                            pagedData.Items[i].PossibleCause = MessageBuyPackageForDetail;
                            pagedData.Items[i].ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                            pagedData.Items[i].ReferenceLocation = MessageBuyPackageForDetail;
                            pagedData.Items[i].Description = MessageBuyPackageForDetail;
                        }
                    }
                    PagedList<DTCModel> pagedDataModel = mapper.Map<PagedList<DTCModel>>(pagedData);
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

        public async override Task<AppDomainResult> GetById(Guid id)
        {

            var user = LoginContext.Instance.CurrentUser;
            if (user.trial == true || user.isAdmin == true)
            {

                DTC item = await this.domainService.GetByIdAsync(id);
                if (item != null)
                {
                    var Emodel = await eModelService.GetByIdAsync(item.EModelID);
                    if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");

                    var brand = await brandService.GetByIdAsync(Emodel.BrandID);
                    if (brand is null) throw new Exception("Không tìm thấy thông tin brand!");

                    item.BrandID = brand.Id;
                    item.BrandName = brand.Name;
                    var itemModel = mapper.Map<DTCModel>(item);

                    return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                }
                throw new KeyNotFoundException(ApiMessage.ItemNotFound);
            }
            else if (user.OpenCar == true || user.OpenTruct == true)
            {
                DTC item = await this.domainService.GetByIdAsync(id);
                if (item != null) {
                    var Emodel = await eModelService.GetByIdAsync(item.EModelID);
                    if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");

                    var brand = await brandService.GetByIdAsync(Emodel.BrandID);
                    if (brand is null) throw new Exception("Không tìm thấy thông tin brand!");

                    item.BrandID = brand.Id;
                    item.BrandName = brand.Name;
                    var itemModel = mapper.Map<DTCModel>(item);
                    if (user.OpenCar == true && user.OpenTruct == true)
                    {
                        /// không xử lý
                    }
                    else if (user.OpenTruct == true)
                    {
                        if (Emodel.EmodelType == (int)EModelType.Car)
                        {
                            itemModel.BrandID = brand.Id;
                            itemModel.BrandName = brand.Name;
                            itemModel.PossibleCause = MessageBuyPackageForDetail;
                            itemModel.ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                            itemModel.ReferenceLocation = MessageBuyPackageForDetail;
                            itemModel.Description = MessageBuyPackageForDetail;
                        }
                    }
                    else if (user.OpenCar == true)
                    {
                        if (Emodel.EmodelType == (int)EModelType.Truck)
                        {
                            itemModel.BrandID = brand.Id;
                            itemModel.BrandName = brand.Name;
                            itemModel.PossibleCause = MessageBuyPackageForDetail;
                            itemModel.ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                            itemModel.ReferenceLocation = MessageBuyPackageForDetail;
                            itemModel.Description = MessageBuyPackageForDetail;
                        }
                    }

                    return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                }
                throw new KeyNotFoundException(ApiMessage.ItemNotFound);
            }
            else
            {
                var contracts = await contractSpecializingService.ContractsIsUsing(user.userId);
                DTC item = await this.domainService.GetByIdAsync(id);
                if (contracts.Count() == 0)
                { // Không có hợp đồng
                    var Emodel = await eModelService.GetByIdAsync(item.EModelID);
                    if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");

                    var brand = await eModelService.GetByIdAsync(Emodel.BrandID);
                    if (brand is null) throw new Exception("Không tìm thấy thông tin brand!");

                    if (item != null)
                    {
                        if (Emodel.EmodelType == (int)EModelType.Car)
                        {
                            item.BrandID = brand.Id;
                            item.BrandName = brand.Name;
                            item.PossibleCause = MessageBuyPackageForDetail;
                            item.ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                            item.ReferenceLocation = MessageBuyPackageForDetail;
                            item.Description = MessageBuyPackageForDetail;
                        }
                        if (Emodel.EmodelType == (int)EModelType.Truck)
                        {
                            item.PossibleCause = MessageBuyPackageForDetail;
                            item.ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                            item.ReferenceLocation = MessageBuyPackageForDetail;
                            item.Description = MessageBuyPackageForDetail;
                        }
                        var itemModel = mapper.Map<DTC>(item);
                        return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                    }
                }

                if (contracts.Count() > 0)
                { // có hợp đồng
                    var Emodel = await eModelService.GetByIdAsync(item.EModelID);
                    if (Emodel is null) throw new Exception("Không tìm thấy thông tin model!");
                    if (Emodel.EmodelType == (int)EModelType.Car && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Car))
                    {
                        item.PossibleCause = MessageBuyPackageForDetail;
                        item.ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                        item.ReferenceLocation = MessageBuyPackageForDetail;
                        item.Description = MessageBuyPackageForDetail;
                    }
                    if (Emodel.EmodelType == (int)EModelType.Truck && !contracts.Select(d => d.ContractType).Contains((int)PackageContractType.Truck))
                    {
                        item.PossibleCause = MessageBuyPackageForDetail;
                        item.ReferenceCircuitDiagram = MessageBuyPackageForDetail;
                        item.ReferenceLocation = MessageBuyPackageForDetail;
                        item.Description = MessageBuyPackageForDetail;
                    }
                    var itemModel = mapper.Map<DTC>(item);
                    return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
                }
            }

            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
        }

        //Import File
        [HttpPost("import-DTC")]
        [Authorize]
        public async Task<AppDomainResult> UploadFile(IFormFile file, Guid? EmodelID)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            await Task.Run(async () =>
            {
                if (file != null && file.Length > 0)
                {
                    string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), file.FileName);
                    //string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME);
                    string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME);
                    string path = Path.Combine(fileUploadPath, fileName);
                    FileUtilities.CreateDirectory(fileUploadPath);
                    var fileByte = FileUtilities.StreamToByte(file.OpenReadStream());
                    FileUtilities.SaveToPath(path, fileByte);

                    using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            int rowno = 1;
                            XLWorkbook workbook = XLWorkbook.OpenFromTemplate(path);
                            var sheets = workbook.Worksheets.First();
                            var rows = sheets.Rows().ToList();
                            foreach (var row in rows)
                            {
                                if (rowno != 1)
                                {
                                    var test = row.Cell(1).Value.ToString();
                                    if (string.IsNullOrWhiteSpace(test) || string.IsNullOrEmpty(test))
                                    {
                                        break;
                                    }
                                    DTC dTC;
                                    var dTCs = await domainService.GetAsync(s => s.DTCCode == row.Cell(1).Value.ToString());
                                    dTC = dTCs.FirstOrDefault();
                                    if (dTC == null)
                                    {
                                        dTC = new DTC();
                                    }
                                    dTC.EModelID = new Guid("daf41fb0-48c4-483d-50c5-08db08ba3a73");
                                    if (EmodelID is not null)
                                    {
                                        dTC.EModelID = (Guid)EmodelID;
                                    }
                                    var emodel = await eModelService.GetByIdAsync(dTC.EModelID);
                                    if (emodel == null) throw new Exception("Không tìm thấy thông tin model");

                                    dTC.DTCCode = row.Cell(1).Value.ToString();
                                    dTC.Name = row.Cell(2).Value.ToString();
                                    dTC.Description = row.Cell(3).Value.ToString();
                                    dTC.PossibleCause = row.Cell(3).Value.ToString();
                                    dTC.Created = Timestamp.Now();

                                    if (dTC.Id == Guid.Empty)
                                        await domainService.CreateAsync(dTC);
                                    else
                                        await domainService.UpdateAsync(dTC);
                                }
                                else
                                {
                                    rowno = 2;
                                }
                            }
                            await transaction.CommitAsync();
                        }
                        catch ( Exception ex ) {
                            await transaction.RollbackAsync();

                            throw new Exception("Lỗi hệ thống vui lòng thử lại sau!");
                        }
                    }
                    

                    var currentLinkSite = $"{Extensions.HttpContext.Current.Request.Scheme}://{Extensions.HttpContext.Current.Request.Host}/{CoreContants.UPLOAD_FOLDER_NAME}/";
                    if (!currentLinkSite.Contains("https"))
                    {
                        currentLinkSite.Replace("http", "https");
                    }
                    string fileUrl = Path.Combine(currentLinkSite, fileName);
                    var fileStr = new FileModel() { fileName = fileName, fileUrl = fileUrl };
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = fileStr,
                        ResultMessage = "Upload File Excel thành công!",
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
            });
            return appDomainResult;
        }

        //Import File
        [HttpPost("import-DTC-image")]
        [Authorize]
        public async Task<AppDomainResult> UploadFileImage(IFormFile file, Guid? EmodelID)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            await Task.Run(async () =>
            {
                if (file != null && file.Length > 0)
                {
                    string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), file.FileName);
                    //string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME);
                    string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME);
                    string path = Path.Combine(fileUploadPath, fileName);
                    FileUtilities.CreateDirectory(fileUploadPath);
                    var fileByte = FileUtilities.StreamToByte(file.OpenReadStream());
                    FileUtilities.SaveToPath(path, fileByte);

                    using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            int rowno = 1;
                            XLWorkbook workbook = XLWorkbook.OpenFromTemplate(path);
                            var sheets = workbook.Worksheets.First();
                            var rows = sheets.Rows().ToList();
                            foreach (var row in rows)
                            {
                                if (rowno != 1)
                                {
                                    var test = row.Cell(1).Value.ToString();
                                    if (string.IsNullOrWhiteSpace(test) || string.IsNullOrEmpty(test))
                                    {
                                        break;
                                    }
                                    
                                }
                                else
                                {
                                    rowno = 2;
                                }
                            }
                            //// Open the Excel workbook and worksheet
                            //Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                            //Microsoft.Office.Interop.Excel.Workbook workbookimage = excelApp.Workbooks.Open(path);
                            //Microsoft.Office.Interop.Excel.Worksheet worksheet = workbookimage.Sheets[1];
                            //Microsoft.Office.Interop.Excel.Shape imageShape = null;
                            //foreach (Microsoft.Office.Interop.Excel.Shape shape in worksheet.Shapes)
                            //{
                            //    if (shape.Type == Microsoft.Office.Core.MsoShapeType.msoPicture)
                            //    {
                            //        imageShape = shape;
                            //        break;
                            //    }
                            //}

                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();

                            throw new Exception("Lỗi hệ thống vui lòng thử lại sau!");
                        }
                    }


                    var currentLinkSite = $"{Extensions.HttpContext.Current.Request.Scheme}://{Extensions.HttpContext.Current.Request.Host}/{CoreContants.UPLOAD_FOLDER_NAME}/";
                    if (!currentLinkSite.Contains("https"))
                    {
                        currentLinkSite.Replace("http", "https");
                    }
                    string fileUrl = Path.Combine(currentLinkSite, fileName);
                    var fileStr = new FileModel() { fileName = fileName, fileUrl = fileUrl };
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = fileStr,
                        ResultMessage = "Upload File Excel thành công!",
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
            });
            return appDomainResult;
        }
    }
}