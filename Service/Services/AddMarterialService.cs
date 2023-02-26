using AutoMapper;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Interface;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using System.IO;
using Models;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Hangfire;
using Request.RequestUpdate;
using Extensions;
using System.Net;
using static Utilities.CoreContants;

namespace Service.Services
{
    public class AddMarterialService : IAddMarterialService
    {
        private IAppDbContext coreDbContext;
        private ISystemFileService systemFileService;
        private IMaterialService marterialService;
        private IMaterialSubService materialSubService;
        private IAddMarterialSubService addMarterialSubService;
        public AddMarterialService(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            ISystemFileService systemFileService,
            IMaterialService marterialService,
            IMaterialSubService materialSubService,
            IAddMarterialSubService addMarterialSubService,
            IAppDbContext coreDbContext
            ) 
        {
            this.coreDbContext = coreDbContext;
            this.systemFileService = systemFileService;
            this.marterialService = marterialService;
            this.materialSubService = materialSubService;
            this.addMarterialSubService = addMarterialSubService;
        }

        public async Task<bool> AddMarterial(MaterialCreate itemModel, string contentRootPath)
        {
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                try
                {
                    if (itemModel.file == null && itemModel.file.Length <= 0) throw new Exception("Dung lượng File Phải lớn hơn 1KB");
                    if (itemModel.file.ContentType != "application/pdf") throw new Exception("File phải là file PDF");

                    var systemFile = await systemFileService.GetByIdAsync(itemModel.SystemFileID);
                    if (systemFile == null) throw new MyException("Không tìm thấy hệ thống file", HttpStatusCode.BadRequest);
                    if (systemFile.SystemFileType != (int)SystemFileType.MaterialFile) throw new MyException("Sai định dạng file system, không thể upload!", HttpStatusCode.BadRequest);
                    var fileStr = new FileModel();
                    await Task.Run(() =>
                    {
                        try {
                            string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), itemModel.file.FileName);
                            //string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(contentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.UPLOAD_MATERIAL_FOLDER_NAME);
                            string path = Path.Combine(fileUploadPath, fileName);
                            FileUtilities.CreateDirectory(fileUploadPath);
                            var fileByte = FileUtilities.StreamToByte(itemModel.file.OpenReadStream());
                            FileUtilities.SaveToPath(path, fileByte);

                            var currentLinkSite = $"{Extensions.HttpContext.Current.Request.Scheme}://{Extensions.HttpContext.Current.Request.Host}/{CoreContants.UPLOAD_FOLDER_NAME}/{CoreContants.UPLOAD_MATERIAL_FOLDER_NAME}/";
                            if (!currentLinkSite.Contains("https"))
                            {
                                currentLinkSite.Replace("http", "https");
                            }
                            string fileUrl = Path.Combine(currentLinkSite, fileName);
                            fileStr.fileName = fileName;
                            fileStr.fileUrl = fileUrl ;
                            fileStr.filePath = path;
                        } catch (Exception e) {
                            throw new Exception("Lỗi hệ thống");
                        }
                    });
                    Material material = new Material()
                    {
                        SystemFileID = systemFile.Id,
                        Name = itemModel.Name,
                        Status = (int)MaterialStatus.Waiting,
                        FileUrl = fileStr.fileUrl,
                        FilePath = fileStr.filePath,
                        Size = itemModel.file.Length,
                        Created = Timestamp.Now()
                    };
                    var res= await marterialService.CreateAsync(material);

                    if (!res)
                        throw new Exception("Lỗi hệ thống");
                    var a = DateTime.Now.AddSeconds(3.0);
                    var jobId = BackgroundJob.Schedule(
                            () => addMarterialSubService.AddMarterialSub(material.Id, contentRootPath),
                            DateTime.Now);

                    await transaction.CommitAsync();

                    return true;
                }
                catch(Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Lỗi hệ thống vui long thử lại sau!");
                }
            }
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateMarterial(MaterialUpdate itemModel, string contentRootPath)
        {
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                try
                {
                    if (itemModel.file == null && itemModel.file.Length <= 0) throw new Exception("Lỗi hệ thống");
                    var fileStr = new FileModel();
                    await Task.Run(() =>
                    {
                        try
                        {
                            string fileName = string.Format("{0}-{1}", Guid.NewGuid().ToString(), itemModel.file.FileName);
                            //string fileUploadPath = Path.Combine(env.ContentRootPath, CoreContants.UPLOAD_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(contentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.UPLOAD_MATERIAL_FOLDER_NAME);
                            string path = Path.Combine(fileUploadPath, fileName);
                            FileUtilities.CreateDirectory(fileUploadPath);
                            var fileByte = FileUtilities.StreamToByte(itemModel.file.OpenReadStream());
                            FileUtilities.SaveToPath(path, fileByte);

                            var currentLinkSite = $"{Extensions.HttpContext.Current.Request.Scheme}://{Extensions.HttpContext.Current.Request.Host}/{CoreContants.UPLOAD_FOLDER_NAME}/{CoreContants.UPLOAD_MATERIAL_FOLDER_NAME}/";
                            string fileUrl = Path.Combine(currentLinkSite, fileName);
                            fileStr.fileName = fileName;
                            fileStr.fileUrl = fileUrl;
                            fileStr.filePath = path;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Lỗi hệ thống");
                        }
                    });

                    // xóa dữ liệu file cũ để cập nhật data mới
                    var materialSubs = await materialSubService.GetAsync(d => d.MaterialID == itemModel.Id && d.Active == true && d.Deleted == false);
                    if (materialSubs == null || materialSubs.Count() == 0) throw new Exception("Lỗi hệ thống");
                    foreach (var item in materialSubs) {
                        var resDelete = await materialSubService.DeleteAsync(item.Id);
                        if (!resDelete)
                            throw new Exception("Lỗi hệ thống");
                    }
                    
                    // không xóa file để update lỗi có thể tim file khôi phục
                    //

                    var systemFile = await systemFileService.GetByIdAsync(itemModel.SystemFileID);
                    if (systemFile == null) throw new MyException("Không tìm thấy hệ thống file", HttpStatusCode.BadRequest);
                    var material = await marterialService.GetByIdAsync(itemModel.Id);

                    material.SystemFileID = itemModel.SystemFileID;
                    material.Name = itemModel.Name;
                    material.FileUrl = fileStr.fileUrl;
                    material.FilePath = fileStr.filePath;
                    material.Created = Timestamp.Now();
                    material.Size = itemModel.file.Length;
                    var res = await marterialService.UpdateAsync(material);

                    if (!res)
                        throw new Exception("Lỗi hệ thống");
                    var a = DateTime.Now.AddSeconds(3.0);

                    var jobId = BackgroundJob.Schedule(
                            () => addMarterialSubService.AddMarterialSub(material.Id, contentRootPath),
                            DateTime.Now.AddSeconds(3.0));

                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Lỗi hệ thống vui long thử lại sau!");
                }
            }
            throw new NotImplementedException();
        }
    }
}
