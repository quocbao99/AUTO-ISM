using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using Request.DomainRequests;
using Utilities;
using static Utilities.CatalogueEnums;

namespace Request.RequestUpdate
{
    public class PlanPaypalUpdate : DomainUpdate
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
