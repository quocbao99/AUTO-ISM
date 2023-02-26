using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Request.DomainRequests;
using Utilities;
using static Utilities.CatalogueEnums;

namespace Request.RequestCreate
{
    public class PriceStripeCreate : DomainCreate
    {
        public Guid? PaymentMethodConfigurationID { get; set; }
        public string ProductID { get; set; }
        public string PriceStripeID { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
    }
}
