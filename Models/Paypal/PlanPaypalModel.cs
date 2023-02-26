using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Paypal
{
    public class PlanPaypalModel: DomainModels.AppDomainModel
    {
        public Guid? PaymentMethodConfigurationID { get; set; }
        public string PlanPaypalID { get; set; }
        public string ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string BillingCycles { get; set; }
        public string PaymentPreferences { get; set; }
        public string Taxes { get; set; }
    }
}
