using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Models.Reponse
{
    public class GetPricePointsReponse
    {
        public List<DataPricePoints> data { get; set; }
    }
    public class PricePointAttributes {
        public double customerPrice { get; set; }
        public double proceeds { get; set; }
        public double proceedsYear2 { get; set; }
    }

    public class DataPricePoints
    {
        public string type { get; set; }
        public string id { get; set; }
        public PricePointAttributes attributes { get; set; }
    }
}
