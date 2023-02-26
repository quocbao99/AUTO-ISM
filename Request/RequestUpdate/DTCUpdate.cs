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
    public class DTCUpdate : DomainUpdate
    {
        public Guid EModelID { get; set; }
        public string DTCCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PossibleCause { get; set; }
        public string ReferenceCircuitDiagram { get; set; }
        public string ReferenceLocation { get; set; }

    }
}
