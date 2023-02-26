using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Models.Request
{
    public class CreateSubcriptionGroupRequest
    {
        public Data data { get; set; }
    }
    public class Data
    {
        public Relationships relationships { get; set; }
        public Attributes attributes { get; set; }
        public string type { get; set; } = "subscriptionGroups";

    }
    public class RelationshipsData
    {
        public string id { get; set; }
        public string type { get; set; } = "apps";

    }
    public class Attributes {
        public string referenceName { get; set; }
    }
    public class Relationships
    {
        public App app { get; set; }
    }

    public class App
    {
        public RelationshipsData data { get; set; }
    }
}
