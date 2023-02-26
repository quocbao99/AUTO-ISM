using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MaterialModel: DomainModels.AppDomainModel
    {
        public Guid SystemFileID { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string SysTemFileName { get; set; }
        public string FileUrl { get; set; }
        public long Size { get; set; }

        //public IFormFile file { get; set; }
    }
}
