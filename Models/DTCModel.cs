using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DTCModel: DomainModels.AppDomainModel
    {
        public Guid EModelID { get; set; }
        public string DTCCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PossibleCause { get; set; }
        public string ReferenceCircuitDiagram { get; set; }
        public string ReferenceLocation { get; set; }

        public string EModelName { get; set; }

        public Guid? BrandID { get; set; }
        public string BrandName { get; set; }
    }
}
