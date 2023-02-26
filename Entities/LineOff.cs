using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class LineOff : DomainEntities.DomainEntities
    {
        public Guid EModelID { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }

        public int LineOffType { get; set; }

        [NotMapped]
        public string EModelName { get; set; }
    }
}
