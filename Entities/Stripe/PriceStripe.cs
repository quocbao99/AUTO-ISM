using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Stripe
{
    public class PriceStripe : DomainEntities.DomainEntities
    {
        public Guid? PaymentMethodConfigurationID { get; set; }
        public string ProductID { get; set; }
        public string PriceStripeID { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
    }
}
