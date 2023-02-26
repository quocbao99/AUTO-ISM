using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserFavoriteModel: DomainModels.AppDomainModel
    {
        public Guid UserId { get; set; }
        public Guid LineOffModel { get; set; }

        public Guid? BrandID { get; set; }
        public string BrandName { get; set; }

        public Guid? ModelID { get; set; }
        public string ModelName { get; set; }

        public Guid? LineOffID { get; set; }
        public string LineOffName { get; set; }
    }
}
