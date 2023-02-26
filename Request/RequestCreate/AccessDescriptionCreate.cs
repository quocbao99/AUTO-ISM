using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using Models;
using Newtonsoft.Json;
using Request.DomainRequests;
using Utilities;
using static Utilities.CatalogueEnums;

namespace Request.RequestCreate
{
    public class AccessDescriptionCreate : DomainCreate
    {
        public AccessSub WiringDiagram { get; set; }
        public AccessSub Specifications { get; set; }
        public AccessSub DTCSearch { get; set; }
        public AccessSub TimingChain_Belt { get; set; }
        public AccessSub TransmissionManual { get; set; }
        public AccessSub TroubleShootingGuide { get; set; }
        public int PackageType { get; set; }
    }
}
