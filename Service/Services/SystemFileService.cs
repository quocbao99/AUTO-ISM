using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Service.Services.DomainServices;
using Entities.Search;
using Newtonsoft.Json;
using System.Reflection;
using System.Net;
using static Utilities.CoreContants;

namespace Service.Services
{
    public class SystemFileService : DomainService<SystemFile, SystemFileSearch>, ISystemFileService
    {
        protected IAppDbContext coreDbContext;
        protected ILineOffService lineOffService;
        public SystemFileService(IAppUnitOfWork unitOfWork
            , IMapper mapper
            , IAppDbContext coreDbContext
            , ILineOffService lineOffService
            ) : base(unitOfWork, mapper)
        {
            this.coreDbContext = coreDbContext;
            this.lineOffService = lineOffService;
        }
        protected override string GetStoreProcName()
        {
            return "SystemFile_GetPagingSystemFile";
        }
        public async override Task<bool> CreateAsync(IList<SystemFile> items)
        {
            foreach (var item in items)
            {
                var lineoff = await lineOffService.GetByIdAsync(item.LineOffID);
                if (lineoff == null) throw new MyException("Không tìm thấy đời xe", HttpStatusCode.BadRequest);
                if (item.ParentID is null) // thằng cha gốc
                {
                    if (item.SystemFileCategory is null) throw new MyException("Nhập loại file!", HttpStatusCode.BadRequest);

                    if (item.SystemFileCategory.Contains(SystemFileCategory.WrittingDiagram.ToString())
                        && item.SystemFileCategory.Contains(SystemFileCategory.Specifications.ToString())
                        && item.SystemFileCategory.Contains(SystemFileCategory.TimingBeltChain.ToString())
                        && item.SystemFileCategory.Contains(SystemFileCategory.TroubleShootingGuide.ToString())
                        && item.SystemFileCategory.Contains(SystemFileCategory.TransmissionManual.ToString())
                        )
                        throw new MyException("Sai loại file!", HttpStatusCode.BadRequest);
                }
                else {
                    //var systemFile = await this.GetByIdAsync((Guid)item.ParentID);
                    //if (systemFile is null) throw new MyException("Không tìm thấy loại file!", HttpStatusCode.BadRequest);

                    //item.SystemFileCategory = systemFile.SystemFileCategory;
                }
                if (item.SystemFileType != (int)SystemFileType.SystemFile && item.SystemFileType != (int)SystemFileType.MaterialFile) throw new MyException("Sai định dạng file", HttpStatusCode.BadRequest);

            }
            return await base.CreateAsync(items);
        }
        public override async Task<bool> UpdateAsync(SystemFile item)
        {
            if (item.ParentID is null) {
                var systemFiles = await this.GetAsync(d=>d.ParentID == item.Id && d. Deleted == false && d.Active == true);
                if (systemFiles is not null) {
                    foreach (var systemfile in systemFiles) {
                        systemfile.SystemFileCategory = item.SystemFileCategory;
                        await this.UpdateFieldAsync(systemfile, d => d.SystemFileCategory);
                    }
                }
            }
            return await base.UpdateAsync(item);
        }

    }
}
