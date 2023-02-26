using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EModelModel : DomainModels.AppDomainModel
    {
        public Guid BrandID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string BrandName { get; set; }
        public int EmodelType { get; set; }
    }
}
