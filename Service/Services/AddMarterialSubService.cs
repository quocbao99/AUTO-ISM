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
using static Utilities.CoreContants;

namespace Service.Services
{
    public class AddMarterialSubService : IAddMarterialSubService
    {
        private IAppDbContext coreDbContext;
        private IMaterialService marterialService;
        private IMaterialSubService materialSubService;

        public AddMarterialSubService(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IMaterialService marterialService,
            IMaterialSubService materialSubService,
            IAppDbContext coreDbContext
            ) 
        {
            this.coreDbContext = coreDbContext;
            this.marterialService = marterialService;
            this.materialSubService = materialSubService;
        }

        public async Task<bool> AddMarterialSub(Guid materialID,string contentRootPath)
        {
            var material = await marterialService.GetByIdAsync(materialID);
            material.Status = (int)MaterialStatus.Fail;
            try {
                await marterialService.UpdateFieldAsync(material, d => d.Status);
            } catch (Exception e) {
            
            }
            if (material == null) throw new Exception("Không có dữ liệu");
            using (IDbContextTransaction transaction = coreDbContext.Database.BeginTransaction())
            {
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    PdfDocument fulldoc = PdfReader.Open(material.FilePath, PdfDocumentOpenMode.Import);
                    var unique = Guid.NewGuid().ToString();
                    string fileUploadPath = Path.Combine(contentRootPath, CoreContants.UPLOAD_FOLDER_NAME, CoreContants.UPLOAD_MATERIALSUB_FOLDER_NAME,material.Name+"-"+ unique);
                    for (int i = 0; i < fulldoc.PageCount; i++)
                    {
                        
                        PdfDocument newdoc = new PdfDocument();
                        newdoc.AddPage(fulldoc.Pages[i]);

                        string fileName = string.Format(material.Name + "page-" + i.ToString() + ".pdf");
                        string path = Path.Combine(fileUploadPath, fileName);
                        FileUtilities.CreateDirectory(fileUploadPath);
                        var currentLinkSite = $"{material.FileUrl.Split(":")[0]}://{material.FileUrl.Split("/")[2]}/{CoreContants.UPLOAD_FOLDER_NAME}/{CoreContants.UPLOAD_MATERIALSUB_FOLDER_NAME}/{material.Name + "-" + unique}/";
                        if (!currentLinkSite.Contains("https"))
                        {
                            currentLinkSite.Replace("http", "https");
                        }
                        string fileUrl = Path.Combine(currentLinkSite, fileName);
                        newdoc.Save(path);

                        MaterialSub materialSub = new MaterialSub();
                        materialSub.MaterialID = materialID;
                        materialSub.FileUrl = fileUrl;
                        materialSub.FilePath = path;
                        materialSub.Created = Timestamp.Now();
                        await materialSubService.CreateAsync(materialSub);
                    }
                    material.Status = (int)MaterialStatus.Success;
                    await marterialService.UpdateFieldAsync(material, d => d.Status);

                    await transaction.CommitAsync();

                    return true;
                }
                catch(Exception e)
                {
                    await transaction.RollbackAsync();
                    
                    throw new Exception("Lỗi hệ thống vui lòng thử lại sau!");
                }
            }
            throw new NotImplementedException();
        }

        
    }
}
