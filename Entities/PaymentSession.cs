using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class PaymentSession : DomainEntities.DomainEntities
    {
        public Guid? PaymentID { get; set; }
        
        // data return when pay by subscription
        public string RequestId { get; set; }
        public decimal? Amount { get; set; }
        public string TransId { get; set; }
        
        public string PartnerClientId { get; set; }
        public string PayType { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }
}
