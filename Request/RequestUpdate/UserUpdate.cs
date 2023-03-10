using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using Request.DomainRequests;
using Utilities;
using static Utilities.CatalogueEnums;

namespace Request.RequestUpdate
{
    public class UserUpdate : DomainUpdate
    {
        /// <summary>
        /// tên đăng nhập
        /// </summary>
        public string Username { get; set; }
        //[StringLength(12, ErrorMessage = "Số kí tự của số điện thoại phải lớn hơn 8 và nhỏ hơn 12!", MinimumLength = 9)]
        //[DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        //[StringLength(50, ErrorMessage = "Số kí tự của email phải nhỏ hơn 50!")]
        //[EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        //[StringLength(1000, ErrorMessage = "Số kí tự của email phải nhỏ hơn 1000!")]
        public string Address { get; set; }

        /// <summary>
        /// Mật khẩu người dùng
        /// </summary>
        //[StringLength(255, ErrorMessage = "Mật khẩu phải lớn hơn 8 kí tự", MinimumLength = 8)]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }

        ///// <summary>
        ///// Tuổi
        ///// </summary>
        //public int? Age { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; }
       
        /// <summary>
        /// Ngày sinh
        /// </summary>
        public double? Birthday { get; set; }

        /// <summary>
        /// Chứng minh nhân dân
        /// </summary>
        //[StringLength(50, ErrorMessage = "Số ký tự phải nhỏ hơn 50")]
        public string IdentityCard { get; set; }

        /// <summary>
        /// Giới tính
        /// 0 => Khác
        /// 1 => Nam
        /// 2 => Nữ
        /// </summary>
        public int? Gender { get; set; }
        public string Thumbnail { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Danh sách quyền của nhân viên
        /// </summary>
        public Guid? RoleId { get; set; }
        /// <summary>
        /// cờ active
        /// </summary>
        public bool? Active { get; set; }

        public bool? IsTrial { get; set; }

        public bool? IsSendOTP { get; set; }
        public bool? IsLock { get; set; }

        public bool? OpenCar { get; set; }
        public bool? OpenTruct { get; set; }
        public string OneSignalID { get; set; }
    }
}
