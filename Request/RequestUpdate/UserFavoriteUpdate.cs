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
    public class UserFavoriteUpdate : DomainUpdate
    {
        public Guid UserId { get; set; }
        public Guid LineOffModel { get; set; }

    }
}
