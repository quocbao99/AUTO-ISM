using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class LineOffModel : DomainModels.AppDomainModel
    {
        public Guid EModelID { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public int Year{ get; set; }
        public int LineOffType { get; set; }
        public string EModelName { get; set; }

        /// <summary>
        /// chỉ dùng để trả cho Fe
        /// </summary>
        public bool isLock{ get; set; }

    }
}
