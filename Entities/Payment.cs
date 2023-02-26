using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class Payment : DomainEntities.DomainEntities
    {
        public Guid? OrderID { get; set; }
        public Guid? UserID { get; set; }
        public Guid? PaymentMethodConfigurationID { get; set; }
        public string PaymentCode { get; set; }
        public string paymentVNPayId { get; set; }
        public int? Status { get; set; }
        public decimal? AmountMoney{ get; set; }
        public string CallbackToken { get; set; }
        public string AccessToken { get; set; }
        public int? StatusForSubScription { get; set; }
        public string PayPalSubscriptionID { get; set; }
        public string StripeSubscriptionID { get; set; }
        [NotMapped]
        public string PaymentMethodName { get; set; }
        [NotMapped]
        public int PaymentMethodCode { get; set; }
        [NotMapped]
        public string OrderCode { get; set; }

    }
}
