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
using Entities.Search;
using System.Reflection;
using System.Data;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Service.Services
{
    public class GetStatisticalService : IGetStatisticalService
    {
        private IAppDbContext coreDbContext;
        private ISystemFileService systemFileService;
        private IMaterialService marterialService;
        private IMaterialSubService materialSubService;
        private IAddMarterialSubService addMarterialSubService;

        private IUserService userService; 
        public GetStatisticalService(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            ISystemFileService systemFileService,
            IMaterialService marterialService,
            IMaterialSubService materialSubService,
            IAddMarterialSubService addMarterialSubService,
            IUserService userService,
            IAppDbContext coreDbContext
            ) 
        {
            this.userService = userService;
            this.coreDbContext = coreDbContext;
            this.systemFileService = systemFileService;
            this.marterialService = marterialService;
            this.materialSubService = materialSubService;
            this.addMarterialSubService = addMarterialSubService;
        }

        public async Task<GetUserRegisterStatisticalModel> getStatisticalUserRegisters(StatisticalUserRegisterSearch statisticalUserRegisterSearch)
        {
            var rs = await ExcuteQueryAsync("GetStatisticalUserRegister", GetSqlParameters(statisticalUserRegisterSearch));
            List<GetUserRegisterStatisticalModel> userRegisterStatisticalModels = new List<GetUserRegisterStatisticalModel>();
            userRegisterStatisticalModels = MappingDataTable.ConvertToList<GetUserRegisterStatisticalModel>(rs);
            if (userRegisterStatisticalModels.Any())
                return userRegisterStatisticalModels[0];
            throw new Exception("Lỗi thống kê");
        }

        public async Task<GetRevenueStatisticalModel> getStatisticalRevenue(StatisticalRevenueSearch statisticalRevenueSearch)
        {
            var rs = await ExcuteQueryAsync("GetStatisticalRevenue", GetSqlParameters(statisticalRevenueSearch));
            List<GetRevenueStatisticalModel> statisticalRevenue = new List<GetRevenueStatisticalModel>();
            statisticalRevenue = MappingDataTable.ConvertToList<GetRevenueStatisticalModel>(rs);
            if (statisticalRevenue.Any())
                return statisticalRevenue[0];
            throw new Exception("Lỗi thống kê");
        }

        public async Task<List<GetYearRevenueStatisticalModel>> GetStatisticalYearRevenue(StatisticalYearRevenueSearch statisticalYearRevenueSearch )
        {
            var rs = await ExcuteQueryAsync("GetStatisticalYearRevenue", GetSqlParameters(statisticalYearRevenueSearch));
            List<GetYearRevenueStatisticalModel> YearRevenueStatistical = new List<GetYearRevenueStatisticalModel>();
            YearRevenueStatistical = MappingDataTable.ConvertToList<GetYearRevenueStatisticalModel>(rs);
            if (YearRevenueStatistical.Any())
                return YearRevenueStatistical;
            throw new Exception("Lỗi thống kê");
        }
        
        public async Task<List<GetYearUserRegisterStatisticalModel>> GetStatisticalYearUserRegister(StatisticalYearUserRegisterSearch statisticalYearUserRegisterSearch)
        {
            var rs = await ExcuteQueryAsync("GetStatisticalYearUserRegister", GetSqlParameters(statisticalYearUserRegisterSearch));
            List<GetYearUserRegisterStatisticalModel> YearUserRegisterStatistical = new List<GetYearUserRegisterStatisticalModel>();
            YearUserRegisterStatistical = MappingDataTable.ConvertToList<GetYearUserRegisterStatisticalModel>(rs);
            if (YearUserRegisterStatistical.Any())
                return YearUserRegisterStatistical;
            throw new Exception("Lỗi thống kê");
        }


        public async Task<GetUserRegisterMonthStatisticalModel> getStatisticalMonthUserRegisters(StatisticalMonthSearch statisticalUserRegisterSearch)
        {
            var rs = await ExcuteQueryAsync("GetStatisticalMonthUserRegisters", GetSqlParameters(statisticalUserRegisterSearch));
            List<GetUserRegisterMonthStatisticalModel> userRegisterMonthStatistical = new List<GetUserRegisterMonthStatisticalModel>();
            userRegisterMonthStatistical = MappingDataTable.ConvertToList<GetUserRegisterMonthStatisticalModel>(rs);
            if (userRegisterMonthStatistical.Any())
                return userRegisterMonthStatistical[0];
            throw new Exception("Lỗi thống kê");
        }

        public async Task<GetRevenueMonthStatisticalModel> getStatisticalMonthRevenue(StatisticalMonthSearch statisticalMonthSearch)
        {
            var rs = await ExcuteQueryAsync("GetStatisticalMonthRevenue", GetSqlParameters(statisticalMonthSearch));
            List<GetRevenueMonthStatisticalModel> revenueMonthStatistical = new List<GetRevenueMonthStatisticalModel>();
            revenueMonthStatistical = MappingDataTable.ConvertToList<GetRevenueMonthStatisticalModel>(rs);
            if (revenueMonthStatistical.Any())
                return revenueMonthStatistical[0];
            throw new Exception("Lỗi thống kê");
        }
        protected virtual SqlParameter[] GetSqlParameters(object baseSearch)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            foreach (PropertyInfo prop in baseSearch.GetType().GetProperties())
            {
                Type type = prop.PropertyType;
                var name = prop.Name;
                var value = prop.GetValue(baseSearch, null);
                //nếu param dạng list thì convert to string. lưu ý value khác null mới convert được.
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && value != null)
                {
                    List<object> result = ((IEnumerable)value).Cast<object>().ToList();
                    string arrayString = string.Join(",", result.ToArray());
                    sqlParameters.Add(new SqlParameter(name, arrayString));
                }
                else
                {
                    sqlParameters.Add(new SqlParameter(name, value));
                }
            }
            SqlParameter[] parameters = sqlParameters.ToArray();
            return parameters;
        }

        public Task<DataTable> ExcuteQueryAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return Task.Run(() =>
            {
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)coreDbContext.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = commandText;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(sqlParameters);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    return dataTable;
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }
    }
}
