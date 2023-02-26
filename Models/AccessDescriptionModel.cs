using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AccessDescriptionModel : DomainModels.AppDomainModel
    {
        public string WiringDiagram { get; set; }
        public string Specifications { get; set; }
        public string DTCSearch { get; set; }
        public string TimingChain_Belt { get; set; }
        public string TransmissionManual { get; set; }
        public string TroubleShootingGuide { get; set; }
        public int PackageType { get; set; }
    }
    public class AccessSub
    {
        public string Basic { get; set; }
        public string Pro { get; set; }
    }
}
