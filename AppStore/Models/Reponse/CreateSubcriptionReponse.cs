using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Models.Reponse
{
    public class CreateSubcriptionReponse
    {
        public CreateSubcriptionData data { get; set; }
    }
    public class CreateSubcriptionData {
        public string type { get; set; }
        public string id { get; set; }
        public CreateSubcriptionAttributes attributes { get; set; }
    }
    public class CreateSubcriptionAttributes
    {
        public string name { get; set; }
        public string productId { get; set; }
        public bool familySharable { get; set; }
        public string state { get; set; }
        public string subscriptionPeriod { get; set; }
        public string reviewNote { get; set; }
        public int groupLevel { get; set; }
        public bool availableInAllTerritories { get; set; }
    }
}
