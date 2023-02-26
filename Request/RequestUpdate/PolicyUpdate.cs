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
    public class PolicyUpdate : DomainUpdate
    {
        public string HTMLContent { get; set; }
        public bool IsUsed { get; set; }
    }
}
