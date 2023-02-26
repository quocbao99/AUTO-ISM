using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class PaypalUtilities
    {
        //public static double ConvertVndToDollar(double vnd)
        //{
        //    var total = Math.Round(vnd / ExchangeRate, 2); //ExchangeRate là tỉ giá của đồng đô la hiện tại

        //    return total;
        //}
        

        
    }
    public class PaymentPaypalResponseModel
    {
        public string OrderDescription { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public string PayerId { get; set; }
        public string Token { get; set; }
        public bool Success { get; set; }
        public string SubscriptionID { get; set; }
        public string BaToken { get; set; }
        
        // stripe sài ké
        public string StripeSubscriptionID { get; set; }
        public string StripeSessionID { get; set; }

    }
}
