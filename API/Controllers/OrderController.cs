using Entities;
using Entities.Search;
using Extensions;
using Interface.Services;
using Interface.Services.Specializing;
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

namespace BaseAPI.Controllers
{
    /// <summary>
    /// Quản lý Đơn hàng
    /// </summary>
    [Route("api/order")]
    [ApiController]
    [Description("Quản lý đơn hàng")]
    [Authorize]
    public class OrderController : BaseController<Order, OrderModel, OrderCreate, OrderUpdate, OrderSearch>
    {
        private IOrderSpecializingService orderSpecializingService;
        private IUserService userService;
        private IPaymentService paymentService;

        public OrderController(IServiceProvider serviceProvider, ILogger<BaseController<Order, OrderModel, OrderCreate, OrderUpdate, OrderSearch>> logger
            , IWebHostEnvironment env
            ) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IOrderService>();
            this.orderSpecializingService = serviceProvider.GetRequiredService<IOrderSpecializingService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.paymentService = serviceProvider.GetRequiredService<IPaymentService>();
        }
        [HttpPost]
        [AppAuthorize]
        [Route("payment-order")]
        [Description("Thanh toán đơn hàng")]
        public async Task<AppDomainResult> PaymentOrder([FromBody] OrderPackageWithPaymentMethodCreate itemModel)
        {
            if (ModelState.IsValid)
            {
                if (itemModel != null)
                {
                    var rs = await orderSpecializingService.OrderPackageWithPaymentMethod(itemModel);

                    return new AppDomainResult()
                    {
                        ResultCode = (int)HttpStatusCode.OK,
                        ResultMessage = "Tạo phiếu thanh toán thành công!",
                        Data = rs,
                        Success = true
                    };
                }
                else
                    throw new AppException("Item không tồn tại");
            }
            throw new AppException(ModelState.GetErrorMessage());
        }

        public async override Task<AppDomainResult> GetById(Guid id)
        {
            Order item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                var itemModel = mapper.Map<OrderModel>(item);
                var user = await userService.GetByIdAsync(item.UserID);
                if (user == null) throw new Exception("Không tìm thấy thông tin khách hàng !");
                itemModel.UserModel = mapper.Map<UserModel>(user);

                var payment = await paymentService.GetSingleAsync(d=>d.OrderID == item .Id && d.Active == true && d.Deleted == false);
                if (payment == null) throw new Exception("Không tìm thấy thông tin thanh toán !");
                itemModel.PaymentModel = mapper.Map<PaymentModel>(payment);
                return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
            }
            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
        }
        public async override Task<AppDomainResult> Get([FromQuery] OrderSearch baseSearch)
        {
            if (ModelState.IsValid)
            {
                PagedList<Order> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<OrderModel> pagedDataModel = mapper.Map<PagedList<OrderModel>>(pagedData);

                for (int i = 0; i < pagedDataModel.Items.Count(); i++) {

                    var user = await userService.GetByIdAsync(pagedDataModel.Items[i].UserID);
                    if (user == null) throw new Exception("Không tìm thấy thông tin khách hàng !");
                    pagedDataModel.Items[i].UserModel = mapper.Map<UserModel>(user);

                    var payment = await paymentService.GetSingleAsync(d => d.OrderID == pagedDataModel.Items[i].Id && d.Active == true && d.Deleted == false);
                    if (payment == null) 
                        throw new Exception("Không tìm thấy thông tin thanh toán !");
                    pagedDataModel.Items[i].PaymentModel = mapper.Map<PaymentModel>(payment);
                }

                return new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
        }
    }
}