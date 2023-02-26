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
    public class PaymentCreate : DomainCreate
    {
        public Guid? OrderID { get; set; }
        public Guid? PaymentMethodConfigurationID { get; set; }
        //public string PaymentCode { get; set; }
        public decimal? AmountMoney { get; set; }
    }
}
