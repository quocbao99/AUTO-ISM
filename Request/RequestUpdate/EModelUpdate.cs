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
    public class EModelUpdate : DomainUpdate
    {
        public Guid BrandID { get; set; }
        public string Name { get; set; }
        public int EmodelType { get; set; }
        public string Icon { get; set; }

    }
}
