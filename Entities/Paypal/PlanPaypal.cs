using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Paypal
{
    public class PlanPaypal : DomainEntities.DomainEntities
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
