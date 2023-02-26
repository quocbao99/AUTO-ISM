using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PaymentSessionModel: DomainModels.AppDomainModel
    {
        public Guid? PaymentID { get; set; }

        // data return when pay by subscription
        public string RequestId { get; set; }
        public decimal? Amount { get; set; }
        public string TransId { get; set; }
        
        public string PartnerClientId { get; set; }
        public string PayType { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }
}
