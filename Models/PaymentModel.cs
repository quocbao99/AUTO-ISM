using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PaymentModel: DomainModels.AppDomainModel
    {
        public Guid? OrderID { get; set; }
        public Guid? UserID { get; set; }
        public Guid? PaymentMethodConfigurationID { get; set; }
        public string PaymentCode { get; set; }
        public string paymentVNPayId { get; set; }
        public int? Status { get; set; }
        public decimal? AmountMoney { get; set; }
        public string CallbackToken { get; set; }
        public string AccessToken { get; set; }
        public int? StatusForSubScription { get; set; }

        public UserModel UserModel { get; set; }
        public OrderModel OrderModel { get; set; }
        public string PaymentMethodName { get; set; }
        public int PaymentMethodCode { get; set; }
        public string OrderCode { get; set; }
    }
}
