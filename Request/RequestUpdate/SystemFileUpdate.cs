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
    public class SystemFileUpdate : DomainUpdate
    {
        public Guid LineOffID { get; set; }
        public string Name { get; set; }
        public Guid? ParentID { get; set; }
        public int SystemFileType { get; set; }
        public string SystemFileCategory { get; set; }
    }
}
