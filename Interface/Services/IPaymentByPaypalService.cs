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
    public interface IPaymentByPaypalService
    {
        Task<string> PaymentByPaypal(Payment payment);
        Task<PaymentPaypalResponseModel> PaymentByPaypalResult(IQueryCollection collections);

        Task<string> PaymentSubscriptionByPaypal(Entities.Payment paymentPaypal, Package package);
        Task<string> PaymentSubscriptionByPaypal(Entities.Payment paymentPaypal);
    }
}
