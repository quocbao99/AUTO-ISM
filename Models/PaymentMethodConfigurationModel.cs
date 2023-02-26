using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PaymentMethodConfigurationModel : DomainModels.AppDomainModel
    {
        // momo
        public Guid PaymentMethodID { get; set; }
        public string endpoint { get; set; }
        public string partnerCode { get; set; }
        public string accessKey { get; set; }
        public string serectkey { get; set; }
        public string returnUrl { get; set; }
        public string notifyurl { get; set; }
        // vnpay
        public string Command { get; set; }
        public string CurrCode { get; set; }
        public string Locale { get; set; }

        public PaymentMethodTypeModel PaymentMethodTypeModel { get; set; }
    }
}
