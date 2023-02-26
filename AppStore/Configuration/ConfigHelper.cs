using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Configuration
{
    public class ConfigHelper
    {
        
        public static string AppleID = "1672941329";
        public static string BaseUrl = "https://api.appstoreconnect.apple.com";
        public static string PrivateKey = System.IO.File.ReadAllText("H:\\Secretkey\\secretkey.txt");
        public static string Kid = "LT2C3J9JBY";
        public static string Iss = "d75360c8-fa27-4a1c-9a03-c9b275fca739";
        public static string Sku = "baonguyen.com.autoism";
        public static string BundleId = "baonguyen.autoism";
        public static string SubscriptionGroupID = "21242671";
    }
}
