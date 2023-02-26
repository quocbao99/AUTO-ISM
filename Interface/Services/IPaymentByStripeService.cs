using Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IPaymentByStripeService
    {
        Task<string> PaymentByStripe(Payment payment);
        Task<PaymentPaypalResponseModel> PaymentByStripeResult(IQueryCollection collections);
        Task<string> PaymentByStripeSubscription(Entities.Payment paymentPaypal, Package package);
    }
}
