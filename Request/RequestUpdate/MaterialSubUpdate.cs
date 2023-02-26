﻿using System;
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
    public class MaterialSubUpdate : DomainUpdate
    {
        public string MaterialID { get; set; }
        public string Link { get; set; }
    }
}
