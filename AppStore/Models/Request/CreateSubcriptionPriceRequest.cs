using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Models.Request
{
    public class CreateSubcriptionPriceRequest
    {
        public DataSubscriptionPriceCreate data { get; set; }
    }
    public class DataSubscriptionPriceCreate
    {
        public SubscriptionPriceCreateRelationships relationships { get; set; }
        public SubscriptionPriceCreateAttributes attributes { get; set; }
        public string type { get; set; } = "subscriptionPrices";

    }
    
    public class SubscriptionPriceCreateAttributes
    {
        public bool preserveCurrentPrice { get; set; }
        public double startDate { get; set; }
    }
    public class SubscriptionPriceCreateRelationships
    {
        public SubscriptionSubscriptionPriceRelationships subscription { get; set; }
        public SubscriptionPricePointSubscriptionPriceRelationships subscriptionPricePoint { get; set; }
        public TerritorySubscriptionPriceRelationships territory { get; set; }
    }

    public class SubscriptionSubscriptionPriceRelationships
    {
        public SubscriptionDataSubscriptionPrice data { get; set; }
    }
    public class SubscriptionPricePointSubscriptionPriceRelationships
    {
        public SubscriptionPriceDataPointSubscriptionPriceRelationships data { get; set; }
    }
    public class TerritorySubscriptionPriceRelationships
    {
        public TerritorySubscriptionPriceSubscriptionPriceRelationships data { get; set; }
    }
    public class SubscriptionDataSubscriptionPrice
    {
        public string id { get; set; }
        public string type { get; set; } = "subscriptions";
    }
    public class SubscriptionPriceDataPointSubscriptionPriceRelationships
    {
        public string id { get; set; }
        public string type { get; set; } = "subscriptionPricePoints";
    }
    public class TerritorySubscriptionPriceSubscriptionPriceRelationships
    {
        public string id { get; set; }
        public string type { get; set; } = "territories";
    }
}
