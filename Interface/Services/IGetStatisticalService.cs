using Entities.Search;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IGetStatisticalService
    {
        Task<GetUserRegisterStatisticalModel> getStatisticalUserRegisters(StatisticalUserRegisterSearch statisticalUserRegisterSearch);
        Task<GetUserRegisterMonthStatisticalModel> getStatisticalMonthUserRegisters(StatisticalMonthSearch statisticalUserRegisterSearch);
        Task<GetRevenueStatisticalModel> getStatisticalRevenue(StatisticalRevenueSearch statisticalRevenueSearch);
        Task<GetRevenueMonthStatisticalModel> getStatisticalMonthRevenue(StatisticalMonthSearch statisticalRevenueSearch);
        Task<List<GetYearRevenueStatisticalModel>> GetStatisticalYearRevenue(StatisticalYearRevenueSearch statisticalRevenueSearch);
        Task<List<GetYearUserRegisterStatisticalModel>> GetStatisticalYearUserRegister(StatisticalYearUserRegisterSearch statisticalRevenueSearch);

    }
}
