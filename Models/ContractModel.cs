using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ContractModel: DomainModels.AppDomainModel
    {
        public Guid PaymenntID { get; set; }
        public Guid? UserID { get; set; }
        public int ContractType { get; set; }
        public string PackageInfo { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public int Status { get; set; }

        public UserModel UserModel { get; set; }
        public PaymentModel PaymentModel { get; set; }
        public string OrderCode { get; set; }
    }
}
