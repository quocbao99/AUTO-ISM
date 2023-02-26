using Entities;
using Entities.Search;
using Extensions;
using Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Request.RequestCreate;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CatalogueEnums;
using static Utilities.CoreContants;

namespace BaseAPI.Controllers
{
    /// <summary>
    /// Quản lý Thông báo
    /// </summary>
    [Route("api/notification")]
    [ApiController]
    [Description("Quản lý thông báo")]
    [Authorize]
    public class NotificattionController : BaseController<Notification, NotificationModel, NotificationCreate, NotificationUpdate, NotificationSearch>
    {
        IUserService userService; 
        IOneSignalService oneSignalService;
        public NotificattionController(IServiceProvider serviceProvider, ILogger<BaseController<Notification, NotificationModel, NotificationCreate, NotificationUpdate, NotificationSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<INotificationService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.oneSignalService = serviceProvider.GetRequiredService<IOneSignalService>();
        }
        public async override Task<AppDomainResult> AddItem([FromBody] NotificationCreate itemModel)
        {
            if (NotificationType.USER.GetHashCode().ToString().Contains(itemModel.Type))
            {
                if (itemModel.UserID is null) throw new Exception("thông tin User không được để trống");
                var user = await userService.GetByIdAsync((Guid)itemModel.UserID);
                if (user is null) throw new Exception("không tìm thấy thông tin User");

                await oneSignalService.CreateOneSignal(itemModel.Title, itemModel.Content, new string[] { user.OneSignalID } );
                return await base.AddItem(itemModel);

            }
            else if (NotificationType.USERs.GetHashCode().ToString().Contains(itemModel.Type))
            {
                var userids = itemModel.UserIDs.Split(",");
                if (userids is null) throw new Exception("không tìm thấy thông tin User");
                string[] oneSignalIDs = null;
                foreach (var userid in userids) {
                    var user = await userService.GetByIdAsync(new Guid(userid));
                    oneSignalIDs.Append(user.OneSignalID);
                }
                await oneSignalService.CreateOneSignal(itemModel.Title, itemModel.Content, oneSignalIDs);
                return await base.AddItem(itemModel);
                //return new AppDomainResult() { ResultCode =(int)HttpStatusCode.OK, Success= true, ResultMessage= "thành công" };
            }
            else if (NotificationType.ROLES.GetHashCode().ToString().Contains(itemModel.Type)) {
                var roleids = itemModel.Roles.Split(",");
                if (roleids is null) throw new Exception("không tìm thấy thông tin phân quyền");

                var users = await userService.GetAsync(d=>d.Roles == itemModel.Roles && d.Active== true && d.Deleted == false && d.IsVerification== true);
                if (users is null) throw new Exception("không tìm thấy thông tin User");
                string[] oneSignalIDs = null;
                foreach (var user in users)
                {
                    oneSignalIDs.Append(user.OneSignalID);
                }
                await oneSignalService.CreateOneSignal(itemModel.Title, itemModel.Content, oneSignalIDs);
                return await base.AddItem(itemModel);
                //return new AppDomainResult() { ResultCode = (int)HttpStatusCode.OK, Success = true, ResultMessage = "thành công" };
            }
            throw new Exception("Lỗi hệ thống");
        }
    }
    
}