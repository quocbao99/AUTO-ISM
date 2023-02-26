using Entities;
using Microsoft.AspNetCore.Http;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IPaymentByVnPayService
    {
        Task<string> PaymentByVnPay(Payment payment);
        Task<string> PaymentBy_VnPay(Payment payment, Package package);

        Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections);
    }
}
