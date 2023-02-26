using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Models.Request
{
    public class CreateSubcriptionRequest
    {
        public DataSubscriptionCreate data { get; set; }
    }
    public class DataSubscriptionCreate
    {
        public SubscriptionCreateRelationships relationships { get; set; }
        public SubscriptionCreateAttributes attributes { get; set; }
        public string type { get; set; } = "subscriptions";

    }
    public class GroupData
    {
        public string id { get; set; }
        public string type { get; set; } = "subscriptionGroups";

    }
    public class SubscriptionCreateAttributes
    {
        public bool availableInAllTerritories { get; set; }
        public bool familySharable { get; set; }
        public string name { get; set; }
        public string productId { get; set; }
        public string reviewNote { get; set; }
        /// <summary>
        ///  Possible values: ONE_WEEK, ONE_MONTH, TWO_MONTHS, THREE_MONTHS, SIX_MONTHS, ONE_YEAR
        /// </summary>
        public string subscriptionPeriod { get; set; }
        //public Price price { get; set; }
        public int groupLevel { get; set; }
    }
    public class SubscriptionCreateRelationships
    {
        public Group group { get; set; }
    }

    public class Group
    {
        public GroupData data { get; set; }
    }
    public class Price
    {
        public decimal value { get; set; }
        public string currency { get; set; }
    }
}
