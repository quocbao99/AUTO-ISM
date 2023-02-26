using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class DTC : DomainEntities.DomainEntities
    {
        public Guid EModelID{ get; set; }
        public string DTCCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PossibleCause { get; set; }
        public string ReferenceCircuitDiagram { get; set; }
        public string ReferenceLocation { get; set; }
        [NotMapped]
        public string EModelName { get; set; }
        [NotMapped]
        public Guid? BrandID { get; set; }
        [NotMapped]
        public string BrandName { get; set; }
    }
}
