using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class MaterialSub : DomainEntities.DomainEntities
    {
        public Guid MaterialID { get; set; }
        public string FileUrl { get; set; }
        public string FilePath { get; set; }
    }
}
