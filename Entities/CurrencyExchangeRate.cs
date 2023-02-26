using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CurrencyExchangeRate : DomainEntities.DomainEntities
    {
        public string Name { get; set; }
        public decimal AmountVN { get; set; }
        public decimal AmountType { get; set; }
        public int ExchangeType { get; set; }
    }
}
