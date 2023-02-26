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
    public class PaymentMethodConfigurationCreate : DomainCreate
    {
        public Guid PaymentMethodID { get; set; }
        public string endpoint { get; set; }
        public string partnerCode { get; set; }
        public string accessKey { get; set; }
        public string serectkey { get; set; }
        public string returnUrl { get; set; }
        public string notifyurl { get; set; }
        // vnpay
        public string Command { get; set; }
        public string CurrCode { get; set; }
        public string Locale { get; set; }
    }
}
