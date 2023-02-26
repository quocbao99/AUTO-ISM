using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GetUserRegisterStatisticalModel
    {
        public int AmountUsers { get; set; }
    }
    public class GetUserRegisterMonthStatisticalModel
    {
        public int AmountUsersThisMonth { get; set; }
        public int AmountUsersLastMonth { get; set; }
    }


    public class GetRevenueStatisticalModel
    {
        public decimal Amount { get; set; }
    }

    public class GetRevenueMonthStatisticalModel
    {
        public decimal AmountThisMonth { get; set; }
        public decimal AmountLastMonth { get; set; }
    }

    public class GetYearRevenueStatisticalModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
    }

    public class GetYearUserRegisterStatisticalModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int AmountUserRegister { get; set; }
    }
}
