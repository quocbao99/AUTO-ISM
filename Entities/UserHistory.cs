using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class UserHistory : DomainEntities.DomainEntities
    {
        public Guid UserId { get; set; }
        public Guid LineOffModel { get; set; }

        [NotMapped]
        public Guid? BrandID { get; set; }
        [NotMapped]
        public string BrandName { get; set; }

        [NotMapped]
        public Guid? ModelID { get; set; }
        [NotMapped]
        public string ModelName { get; set; }

        [NotMapped]
        public Guid? LineOffID { get; set; }
        [NotMapped]
        public string LineOffName { get; set; }
    }
}
