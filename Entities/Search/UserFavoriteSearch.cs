using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using static Utilities.CatalogueEnums;

namespace Entities.Search
{
    public class UserFavoriteSearch : BaseSearch
    {
        public Guid? UserId { get; set; }
    }
}
