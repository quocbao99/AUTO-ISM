using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Models;
using Newtonsoft.Json;
using Request.DomainRequests;
using Utilities;
using static Utilities.CatalogueEnums;

namespace Request.RequestUpdate
{
    public class AccessDescriptionUpdate : DomainUpdate
    {
        public AccessSub WiringDiagram { get; set; }
        public AccessSub Specifications { get; set; }
        public AccessSub DTCSearch { get; set; }
        public AccessSub TimingChain_Belt { get; set; }
        public AccessSub TransmissionManual { get; set; }
        public AccessSub TroubleShootingGuide { get; set; }
    }
}
