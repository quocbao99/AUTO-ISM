using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Request.DomainRequests;
using Utilities;
using static Utilities.CatalogueEnums;

namespace Request.RequestCreate
{
    public class OrderCreate : DomainCreate
    {
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public Guid UserID { get; set; }
        public Guid PackageID { get; set; }

        /// <summary>
        /// thông tin gói khi mua đơn hàng
        /// </summary>
        public string PackageInfo { get; set; }
    }
}
