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
    public class EModelService : DomainService<EModel, EModelSearch>, IEModelService
    {
        protected IAppDbContext coreDbContext;
        protected IBrandService brandService;
        public EModelService(IAppUnitOfWork unitOfWork
            , IMapper mapper
            , IAppDbContext coreDbContext
            , IBrandService brandService
            ) : base(unitOfWork, mapper)
        {
            this.coreDbContext = coreDbContext;
            this.brandService = brandService;
        }
        protected override string GetStoreProcName()
        {
            return "EModel_GetPagingEModel";
        }
        public async override Task<bool> CreateAsync(IList<EModel> items)
        {
            foreach (var item in items) {
                var brand = await brandService.GetByIdAsync(item.BrandID);
                if (brand == null) throw new MyException("Không tìm thấy hãng xe", HttpStatusCode.BadRequest);
                if (item.EmodelType != (int) EModelType.Car && item.EmodelType != (int) EModelType.Truck) throw new MyException("Sai định dạng loại xe!", HttpStatusCode.BadRequest);
            }
            return await base.CreateAsync(items);
        }
    }
}
