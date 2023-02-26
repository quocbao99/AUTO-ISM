using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SystemFileModel : DomainModels.AppDomainModel
    {
        public Guid LineOffID { get; set; }
        public string Name { get; set; }
        public Guid? ParentID { get; set; }
        public int SystemFileType { get; set; }
        public string LineOffName { get; set; }
        public string ParentSystemFileName { get; set; }
        public string SystemFileCategory { get; set; }

        public int CountChildFile { get; set; }

        public Guid? BrandID { get; set; }
        public string BrandName { get; set; }
        public Guid? EModelID { get; set; }
        public string EmodelName { get; set; }
    }
}
