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

namespace Service.Services
{
    public class LineOffService : DomainService<LineOff, LineOffSearch>, ILineOffService
    {
        protected IAppDbContext coreDbContext;
        private IUserHistoryService userHistoryService;
        private IEModelService eModelService;
        public LineOffService(IAppUnitOfWork unitOfWork
            , IMapper mapper
            , IAppDbContext coreDbContext
            , IUserHistoryService userHistoryService
            , IEModelService eModelService
            ) : base(unitOfWork, mapper)
        {
            this.coreDbContext = coreDbContext;
            this.userHistoryService = userHistoryService;
            this.eModelService = eModelService;
        }
        protected override string GetStoreProcName()
        {
            return "LineOff_GetPagingLineOff";
        }
        public async override Task<LineOff> GetByIdAsync(Guid id)
        {
            try {
                UserHistory userHistory = new UserHistory();
                userHistory.UserId = LoginContext.Instance.CurrentUser.userId;
                userHistory.LineOffModel = id;
                await userHistoryService.CreateAsync(userHistory);
            } catch (MyException ex) {
                throw new MyException("Lỗi hệ thống", HttpStatusCode.InternalServerError);
            }
            return await base.GetByIdAsync(id);
        }
        public async override Task<bool> CreateAsync(IList<LineOff> items)
        {
            foreach (var item in items)
            {
                var Emodel = await eModelService.GetByIdAsync(item.EModelID);
                if (Emodel is null) throw new MyException("Không tìm thấy thông tin Model! ", HttpStatusCode.BadRequest);
                var lineoff = await this.GetSingleAsync(d=> d.Year == item.Year && d.EModelID == Emodel.Id && d.Deleted == false);
                if(lineoff is not null) throw new MyException("LineOff đã tồn tại! ", HttpStatusCode.BadRequest);
                if (Emodel == null) throw new MyException("Không tìm thấy hãng xe", HttpStatusCode.BadRequest);
            }
            return await base.CreateAsync(items);
        }

    }
}
