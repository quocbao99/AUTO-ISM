using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class AccessDescription : DomainEntities.DomainEntities
    {
        public string WiringDiagram { get; set; }
        public string Specifications { get; set; }
        public string DTCSearch { get; set; }
        public string TimingChain_Belt { get; set; }
        public string TransmissionManual { get; set; }
        public string TroubleShootingGuide { get; set; }
        public int PackageType { get; set; }
    }
}
