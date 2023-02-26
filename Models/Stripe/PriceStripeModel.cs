using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Stripe
{
    public class PriceStripeModel: DomainModels.AppDomainModel
    {
        public Guid? PaymentMethodConfigurationID { get; set; }
        public string ProductID { get; set; }
        public string PriceStripeID { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
    }
}
