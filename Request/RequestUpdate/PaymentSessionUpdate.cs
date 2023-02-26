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
    public class PaymentSessionUpdate : DomainUpdate
    {
        public Guid? PaymentID { get; set; }

        // data return when pay by subscription
        public string RequestId { get; set; }
        public decimal? Amount { get; set; }
        public string TransId { get; set; }
        
        public string PartnerClientId { get; set; }
        public string PayType { get; set; }
        public int? Status { get; set; }
        public string Message { get; set; }
    }
}
