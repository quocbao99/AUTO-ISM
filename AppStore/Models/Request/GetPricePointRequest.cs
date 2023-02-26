using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Models.Request
{
    public class GetPricePointRequest
    {
        public string subcription_id { get; set; }
        public string territories { get; set; }
        public string territory { get; set; }
        public string subscriptionPricePoints { get; set; }
        public string include { get; set; }
    }
}
