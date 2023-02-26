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
    public class DTCService : DomainService<DTC, DTCSearch>, IDTCService
    {
        protected IAppDbContext coreDbContext;
        public DTCService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext coreDbContext) : base(unitOfWork, mapper)
        {
            this.coreDbContext = coreDbContext;
        }
        protected override string GetStoreProcName()
        {
            return "DTC_GetPagingDTC";
        }
        public async override Task<bool> CreateAsync(DTC item)
        {
            var DTCs = await this.GetAsync(d=> d.DTCCode == item.DTCCode && d.Active == true && d.Deleted == false);
            if (DTCs != null && DTCs.Count() > 0) throw new MyException($"Đã tồn tại {item.DTCCode} DTC code", HttpStatusCode.BadRequest);
            return await base.CreateAsync(item);
        }

        public async override Task<bool> CreateAsync(IList<DTC> items)
        {
            foreach (var item in items) {
                var DTCs = await this.GetAsync(d => d.DTCCode == item.DTCCode && d.Active == true && d.Deleted == false);
                if (DTCs != null && DTCs.Count() > 0) throw new MyException($"Đã tồn tại {item.DTCCode} DTC code", HttpStatusCode.BadRequest);
            }
            return await base.CreateAsync(items);
        }
        public async override Task<bool> UpdateAsync(DTC item)
        {
                var DTCs = await this.GetAsync(d =>d.Id != item.Id && d.DTCCode == item.DTCCode && d.Active == true && d.Deleted == false);
                if (DTCs != null && DTCs.Count() > 0) throw new MyException($"Đã tồn tại {item.DTCCode} DTC code", HttpStatusCode.BadRequest);
            return await base.UpdateAsync(item);
        }
    }
}
