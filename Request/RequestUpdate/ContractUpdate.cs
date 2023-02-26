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
    public class ContractUpdate : DomainUpdate
    {
        public Guid PaymenntID { get; set; }
        public int ContractType { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public int Status { get; set; }

    }
}
