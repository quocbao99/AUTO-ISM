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
    public class CurrencyExchangeRateCreate : DomainCreate
    {
        public string Name { get; set; }
        public decimal AmountVN { get; set; }
        public decimal AmountType { get; set; }
        public int ExchangeType { get; set; }
    }
}
