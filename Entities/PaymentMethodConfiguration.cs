using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class PaymentMethodConfiguration : DomainEntities.DomainEntities
    {
        // momo
        public Guid PaymentMethodID { get; set; }
        public string endpoint { get; set; }
        public string partnerCode { get; set; }
        public string accessKey { get; set; }
        public string serectkey { get; set; }
        public string returnUrl { get; set; }
        public string notifyurl { get; set; }

        // vnpay
        /// <summary>
        /// loại thanh toán
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// loại tiền tệ
        /// </summary>
        public string CurrCode { get; set; }
        /// <summary>
        /// ngôn ngữ
        /// </summary>
        public string Locale { get; set; }
    }
}
