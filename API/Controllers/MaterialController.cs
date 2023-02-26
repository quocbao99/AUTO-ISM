using Entities;
using Entities.Search;
using Extensions;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using System.IO.Compression;
using static Utilities.CatalogueEnums;
using static Utilities.CoreContants;
using Microsoft.EntityFrameworkCore.Storage;
using Interface.DbContext;
using Hangfire;
using Interface.Services.Configuration;

namespace BaseAPI.Controllers
{
    /// <summary>
    /// Quản lý Tài liệu
    /// </summary>
    [Route("api/material")]
    [ApiController]
    [Description("Quản lý tài liệu")]
    [Authorize]
    public class MaterialController : BaseController<Material, MaterialModel, MaterialCreate, MaterialUpdate, MaterialSearch>
    {
        private IAddMarterialService addMarterialService;
        private ISystemFileService systemFileService;
        private IUserService userService;
        private IMaterialService materialService;
        private IAppDbContext coreDbContext;
        private ILineOffService lineOffService;
        private IAddMarterialSubService addMarterialSubService;
        private IEmailConfigurationService emailConfigurationService;
        public MaterialController(IServiceProvider serviceProvider, ILogger<BaseController<Material, MaterialModel, MaterialCreate, MaterialUpdate, MaterialSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IMaterialService>();
            this.addMarterialService = serviceProvider.GetRequiredService<IAddMarterialService>();
            this.systemFileService = serviceProvider.GetRequiredService<ISystemFileService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.materialService = serviceProvider.GetRequiredService<IMaterialService>();
            this.coreDbContext = serviceProvider.GetRequiredService<IAppDbContext>();
            this.lineOffService = serviceProvider.GetRequiredService<ILineOffService>();
            this.addMarterialSubService = serviceProvider.GetRequiredService<IAddMarterialSubService>();
            this.emailConfigurationService = serviceProvider.GetRequiredService<IEmailConfigurationService>();
        }

        public async override Task<AppDomainResult> AddItem([FromForm] MaterialCreate itemModel)
        {
            var res = await addMarterialService.AddMarterial(itemModel, env.ContentRootPath);

            if (res == false) throw new Exception("Tạo tài liệu thất bại");
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                ResultMessage = "Tạo tài liệu thành công!",
                Success = true
            };
        }
        public async override Task<AppDomainResult> UpdateItem([FromForm] MaterialUpdate itemModel)
        {
            var res = await addMarterialService.UpdateMarterial(itemModel, env.ContentRootPath);

            if (res == false) throw new Exception("cập nhật tài liệu thất bại");
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                ResultMessage = "Cập nhật tài liệu thành công!",
                Success = true
            };
        }

        public async override Task<AppDomainResult> GetById(Guid id)
        {
            Material item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                var systemFile = await systemFileService.GetByIdAsync(item.SystemFileID);
                if (systemFile == null) throw new Exception("Không tìm thấy Model của line-off");
                item.SysTemFileName = systemFile.Name;
                var itemModel = mapper.Map<Material>(item);
                return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
            }
            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
        }

        public async override Task<AppDomainResult> Get([FromQuery] MaterialSearch baseSearch)
        {
            if (ModelState.IsValid)
            {
                PagedList<Material> pagedData = await domainService.GetPagedListData(baseSearch);
                PagedList<MaterialModel> pagedDataModel = mapper.Map<PagedList<MaterialModel>>(pagedData);
                if (pagedDataModel.Items == null)
                {
                    return new AppDomainResult
                    {
                        Data = pagedDataModel,
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                foreach (var item in pagedDataModel.Items)
                {
                    var currentLinkSite = $"{item.FileUrl.Split(":")[0]}://{item.FileUrl.Split("/")[2]}/api/material/file/";
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
            var material = await domainService.GetByIdAsync(id);
            if (material == null) throw new Exception("Không tìm thấy File");
            string path = material.FilePath;
            try
            {
                Stream stream = System.IO.File.Open(path, System.IO.FileMode.Open);
                if (stream == null)
                    return NotFound(); // returns a NotFoundResult with Status404NotFound response.

                return File(stream, "application/pdf"); // returns a FileStreamResult
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi hệ thống vui lòng thử lại sau");
            }
        }

        //GET api/download/12345abc
        [HttpGet("download/{id}")]
        [Authorize]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var userid = LoginContext.Instance.CurrentUser.userId;
            var user = await userService.GetByIdAsync(userid);

            if (Timestamp.ToDateTime(user.DateDownLoad).Month != DateTime.Now.Month
                || Timestamp.ToDateTime(user.DateDownLoad).Year != DateTime.Now.Year
                )
            {
                user.CoutDownLoadMonth = 1;
            }
            else {
                user.CoutDownLoadMonth = user.CoutDownLoadMonth + 1;
            }

            if(user.CoutDownLoadMonth > (int)LimitedDownload.LimitedDownloadMonth)
                throw new Exception("Quá giới hạn Download trong một tháng");

            if (Timestamp.ToDateTime(user.DateDownLoad).Date != DateTime.Now.Date
                || Timestamp.ToDateTime(user.DateDownLoad).Month != DateTime.Now.Month
                || Timestamp.ToDateTime(user.DateDownLoad).Year != DateTime.Now.Year
                ) // khác ngày hiện tại
            {
                user.DateDownLoad = Timestamp.Now();
                user.CoutDownLoad = 1;
            }
            else
            {
                if (user.CoutDownLoad >= (int)LimitedDownload.LimitedDownload)
                {
                    throw new Exception("Quá giới hạn Download trong một ngày");
                }
                user.CoutDownLoad = user.CoutDownLoad + 1;
            }

            await userService.UpdateFieldAsync(user, d => d.CoutDownLoad, d => d.DateDownLoad);

            var material = await domainService.GetByIdAsync(id);
            if (material == null) throw new Exception("Không tìm thấy File");
            string path = material.FilePath;
            try
            {
                Stream stream = System.IO.File.Open(path, System.IO.FileMode.Open);
                if (stream == null)
                    return NotFound(); // returns a NotFoundResult with Status404NotFound response.

                return File(stream, "application/pdf"); // returns a FileStreamResult
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi hệ thống vui lòng thử lại sau");
            }
        }

        [HttpPost]
        [Route("import-file-zip")]
        [AllowAnonymous]
        [Description("import tài liệu")]
        [RequestSizeLimit(long.MaxValue)] // 500 MB
        public async Task<AppDomainResult> importSystemFile([FromForm] ImportFileCreate importFileCreate)
        {
            var lineoff = await lineOffService.GetByIdAsync(importFileCreate.LineOffID);
            if (lineoff == null) throw new MyException("Không tìm thấy đời xe", HttpStatusCode.BadRequest);
            
            if (importFileCreate.SystemFileCategory is null) throw new MyException("Nhập loại file!", HttpStatusCode.BadRequest);

            if (importFileCreate.SystemFileCategory.Contains(SystemFileCategory.WrittingDiagram.ToString())
                && importFileCreate.SystemFileCategory.Contains(SystemFileCategory.Specifications.ToString())
                && importFileCreate.SystemFileCategory.Contains(SystemFileCategory.TimingBeltChain.ToString())
                && importFileCreate.SystemFileCategory.Contains(SystemFileCategory.TroubleShootingGuide.ToString())
                && importFileCreate.SystemFileCategory.Contains(SystemFileCategory.TransmissionManual.ToString())
                )
                throw new MyException("Sai loại file!", HttpStatusCode.BadRequest);

            string extractPath = "";
            await Task.Run(async () =>
            {
                if (importFileCreate.file != null && importFileCreate.file.Length > 0)
                {
                    string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), importFileCreate.file.FileName);
                    //string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME);
                    string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.UPLOAD_IMPORTJAR_FOLDER_NAME);
                    string path = Path.Combine(fileUploadPath, fileName);
                    FileUtilities.CreateDirectory(fileUploadPath);
                    var fileByte = FileUtilities.StreamToByte(importFileCreate.file.OpenReadStream());
                    FileUtilities.SaveToPath(path, fileByte);


                    var currentLinkSite = $"{Extensions.HttpContext.Current.Request.Scheme}://{Extensions.HttpContext.Current.Request.Host}/{CoreContants.UPLOAD_FOLDER_NAME}/";

                    if (!currentLinkSite.Contains("https"))
                    {
                        currentLinkSite = currentLinkSite.Replace("http", "https");
                    }
                    string fileUrl = Path.Combine(currentLinkSite, fileName);
                    var fileStr = new FileModel() { fileName = fileName, fileUrl = fileUrl };
                    /// giản nén file
                    string zipPath = path;
                    extractPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.UPLOAD_DATAEXTRACT_FOLDER_NAME, fileName);
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                }
            });
            await using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction()) {
                try {

                    await ProcessDirectory(extractPath, null, importFileCreate.LineOffID, importFileCreate.SystemFileCategory);
                    await transaction.CommitAsync();
                } catch (Exception ex) {
                    await transaction.RollbackAsync();
                    return new AppDomainResult
                    {
                        Success = false,
                        ResultCode = (int)HttpStatusCode.OK,
                        ResultMessage = ex.Message
                    };
                }
            }
            return new AppDomainResult
            {
                Data = "",
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        async Task ProcessDirectory(string directory, Guid? SystemFileParentID, Guid LineoffID, string SystemFileCategory)
        {
            // xử lý thư mục
            string[] files = Directory.GetFiles(directory);
            
            string[] subdirectories = Directory.GetDirectories(directory);
            
            if (files.Count() > 0 && subdirectories.Count() > 0) {
                throw new Exception($"Lỗi định dạng file, có file cùng cấp với thư mục! đường dẫn: {directory}");
            }
            if (files.Count() > 0) {
            
            }
            foreach (string file in files)
            {
                var currentLinkSite = $"{Extensions.HttpContext.Current.Request.Scheme}://{Extensions.HttpContext.Current.Request.Host}/{CoreContants.Upload}";
                if (!currentLinkSite.Contains("https"))
                {
                    currentLinkSite.Replace("http", "https");
                }
                var arr = file.Split(CoreContants.UPLOAD_FOLDER_NAME)[1].Replace('\\','/');
                long fileSize = new FileInfo(file).Length;
                Material material = new Material()
                {
                    SystemFileID = (Guid)SystemFileParentID,
                    Name = file.Split("\\").LastOrDefault().ToString(),
                    Status = (int)MaterialStatus.Waiting,
                    FileUrl = currentLinkSite + arr,
                    FilePath = file,
                    Size = fileSize,
                    Created = Timestamp.Now()
                };
                var res = await materialService.CreateAsync(material);

                var jobId = BackgroundJob.Schedule(
                            () =>  addMarterialSubService.AddMarterialSub(material.Id, env.ContentRootPath),
                            DateTime.Now);
            }

            foreach (string subdirectory in subdirectories)
            {
                string[] fileses = Directory.GetFiles(subdirectory);
                // Tạo systemfile
                SystemFile systemFile = new SystemFile();
                if (fileses.Count() > 0) {
                    systemFile.LineOffID = LineoffID;
                    systemFile.Name = subdirectory.Split("\\").LastOrDefault().ToString();
                    systemFile.ParentID = SystemFileParentID;
                    systemFile.SystemFileType = (int)SystemFileType.MaterialFile;
                    systemFile.SystemFileCategory = SystemFileCategory;
                    await systemFileService.CreateAsync(systemFile);
                }
                if (fileses.Count() == 0) {
                    // Tạo systemfile
                    systemFile.LineOffID = LineoffID;
                    systemFile.Name = subdirectory.Split("\\").LastOrDefault().ToString();
                    systemFile.ParentID = SystemFileParentID;
                    systemFile.SystemFileType = (int)SystemFileType.SystemFile;
                    systemFile.SystemFileCategory = SystemFileCategory;
                    await systemFileService.CreateAsync(systemFile);
                }
                await ProcessDirectory(subdirectory, systemFile.Id, LineoffID, SystemFileCategory); // đệ quy đọc các thư mục con
            }
        
        }
    }
}