using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class SystemFile : DomainEntities.DomainEntities
    {
        public Guid LineOffID { get; set; }
        public string Name { get; set; }
        public Guid? ParentID{ get; set; }
        public int SystemFileType { get; set; }
        public string SystemFileCategory { get; set; }

        [NotMapped]
        public string LineOffName { get; set; }
        [NotMapped]
        public string ParentSystemFileName { get; set; }
        [NotMapped]
        public Guid? BrandID { get; set; }
        [NotMapped]
        public string BrandName { get; set; }
        [NotMapped]
        public Guid? EModelID { get; set; }
        [NotMapped]
        public string EmodelName { get; set; }
    }
}
