using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PolicyModel: DomainModels.AppDomainModel
    {
        public string HTMLContent { get; set; }
        public bool IsUsed { get; set; }

    }
}
