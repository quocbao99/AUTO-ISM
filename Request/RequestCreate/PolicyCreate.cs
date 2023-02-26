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
    public class PolicyCreate : DomainCreate
    {
        public string HTMLContent { get; set; }
        public bool IsUsed { get; set; }
    }
}
