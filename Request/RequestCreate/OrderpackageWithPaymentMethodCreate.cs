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
    public class OrderPackageWithPaymentMethodCreate : DomainCreate
    {
        public Guid PackageID { get; set; }
        public Guid? PaymentMethodConfigurationID { get; set; }
    }
}
