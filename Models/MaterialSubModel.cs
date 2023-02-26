using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MaterialSubModel: DomainModels.AppDomainModel
    {
        public string MaterialID { get; set; }
        public string FileUrl { get; set; }
        public string FilePath { get; set; }
    }
}
