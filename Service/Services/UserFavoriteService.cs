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
    public class UserFavoriteService : DomainService<UserFavorite, UserFavoriteSearch>, IUserFavoriteService
    {
        protected IAppDbContext coreDbContext;
        public UserFavoriteService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext coreDbContext) : base(unitOfWork, mapper)
        {
            this.coreDbContext = coreDbContext;
        }
        protected override string GetStoreProcName()
        {
            return "UserFavorite_GetPagingUserFavorite";
        }

        public async override Task<bool> CreateAsync(UserFavorite item)
        {
            try { 
                item.UserId = LoginContext.Instance.CurrentUser.userId;
            }
            catch (MyException ex) {
                throw new MyException("Lỗi thông tin đăng nhập", HttpStatusCode.BadRequest);
            }
            return await base.CreateAsync(item);
        }

    }
}
