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
    public class PaymentUpdate : DomainUpdate
    {
        public Guid? OrderID { get; set; }
        public Guid? PaymentMethodConfigurationID { get; set; }
        public string PaymentCode { get; set; }
        public string paymentVNPayId { get; set; }
        public int? Status { get; set; }
        public decimal? AmountMoney { get; set; }
        public string CallbackToken { get; set; }
        public string AccessToken { get; set; }
    }
}
