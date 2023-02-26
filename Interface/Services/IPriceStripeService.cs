using Entities;
using Entities.Paypal;
using Entities.Search;
using Entities.Stripe;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IPriceStripeService : IDomainService<PriceStripe, PriceStripeSearch>
    {

    }
}
