using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class Contract : DomainEntities.DomainEntities
    {
        public Guid PaymenntID { get; set; }
        public Guid? UserID { get; set; }
        public int ContractType { get; set; }
        public string PackageInfo { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public int Status { get; set; }
        [NotMapped]
        public string OrderCode { get; set; }
    }
}
