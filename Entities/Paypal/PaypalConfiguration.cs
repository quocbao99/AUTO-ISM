using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Paypal
{
    public class PaypalConfiguration : DomainEntities.DomainEntities
    {
        public string ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string BillingCycles { get; set; }
        public string PaymentPreferences { get; set; }
        public string Taxes { get; set; }
    }
}
