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
    public class PackageUpdate : DomainUpdate
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int MonthExp { get; set; }
        public int PackageType { get; set; }
        public string Description { get; set; }

        //// quyền truy cập tài liệu
        //public string AccessPackageTypes { get; set; }

        //// quyền truy cập brand
        //public int AccessBrand { get; set; }
        //public string AccessBrandTypes { get; set; }
        //public string AccessBrandIDs { get; set; }

        //// quyền truy cập Model
        //public int AccessModel { get; set; }
        //public string AccessModelTypes { get; set; }
        //public string AccessModelIDs { get; set; }

        //// quyền truy cập line off
        //public int AccessLineOff { get; set; }
        //public string AccessLineOffTypes { get; set; }
        //public string AccessLineOffIDs { get; set; }
        //public int AccessLineOffTime { get; set; }

        //// quyền truy cập Material
        //public int AccessMaterial { get; set; }
        //public string AccessMaterialTypes { get; set; }
        //public string AccessMaterialIDs { get; set; }

        //// quyền truy cập MaterialSub
        //public int AccessMaterialSub { get; set; }
        //public string AccessMaterialSubTypes { get; set; }
        //public string AccessMaterialSubIDs { get; set; }
    }
}
