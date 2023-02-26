using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class EModel : DomainEntities.DomainEntities
    {
        public Guid BrandID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int EmodelType { get; set; }

        [NotMapped]
        public string BrandName { get; set; }
    }
}
