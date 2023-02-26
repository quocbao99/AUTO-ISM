using Entities;
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
    public interface IPaymentByMomoService
    {
        Task<string> PaymentByMomo(Payment payment);
        Task<string> PaymentBy_Momo(Payment payment, Package package);
        Task<bool> ProcessPaymentResult(MomoUtilities.Result result);
        Task<object> PayUsingToken(PaymentSession paymentSession, Package package);
    }
}
