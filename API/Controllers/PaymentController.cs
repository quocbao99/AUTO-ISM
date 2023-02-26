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
    /// Quản lý Thanh toán
    /// </summary>
    [Route("api/payment")]
    [ApiController]
    [Description("Quản lý thanh toán")]
    [Authorize]
    public class PaymentController : BaseController<Payment, PaymentModel, PaymentCreate, PaymentUpdate, PaymentSearch>
    {
        private IPaymentByMomoService paymentByMomoService;
        private IPaymentByVnPayService paymentByVnPayService;
        private IPaymentByPaypalService paymentByPaypalService;
        private IPaymentByStripeService paymentByStripeService;
        private IUserService userService;
        private IOrderService orderService;
        private IPackageService packageService;
        public PaymentController(IServiceProvider serviceProvider, ILogger<BaseController<Payment, PaymentModel, PaymentCreate, PaymentUpdate, PaymentSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPaymentService>();
            this.paymentByMomoService = serviceProvider.GetRequiredService<IPaymentByMomoService>();
            this.paymentByVnPayService = serviceProvider.GetRequiredService<IPaymentByVnPayService>();
            this.paymentByPaypalService = serviceProvider.GetRequiredService<IPaymentByPaypalService>();
            this.paymentByStripeService = serviceProvider.GetRequiredService<IPaymentByStripeService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.orderService = serviceProvider.GetRequiredService<IOrderService>();
            this.packageService = serviceProvider.GetRequiredService<IPackageService>();
        }

        public async override Task<AppDomainResult> GetById(Guid id)
        {
            Payment item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                var itemModel = mapper.Map<PaymentModel>(item);
                var user = await userService.GetByIdAsync((Guid)item.UserID);
                if (user == null) throw new Exception("Không tìm thấy thông tin khách hàng !");
                itemModel.UserModel = mapper.Map<UserModel>(user);

                var order = await orderService.GetSingleAsync(d => d.Id == item.OrderID && d.Active == true && d.Deleted == false);
                if (order == null) throw new Exception("Không tìm thấy thông tin thanh toán !");
                itemModel.OrderModel = mapper.Map<OrderModel>(order);
                return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
            }
            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
        }

        public async override Task<AppDomainResult> Get([FromQuery] PaymentSearch baseSearch)
        {
            if (ModelState.IsValid)
            {
                PagedList<Payment> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<PaymentModel> pagedDataModel = mapper.Map<PagedList<PaymentModel>>(pagedData);
                if (pagedDataModel.Items == null)
                {
                    return new AppDomainResult
                    {
                        Data = pagedDataModel,
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                for (int i = 0; i < pagedDataModel.Items.Count(); i++)
                {

                    var user = await userService.GetByIdAsync((Guid)pagedDataModel.Items[i].UserID);
                    if (user == null) throw new Exception("Không tìm thấy thông tin khách hàng !");
                    pagedDataModel.Items[i].UserModel = mapper.Map<UserModel>(user);

                    var order = await orderService.GetSingleAsync(d => d.Id == pagedDataModel.Items[i].OrderID && d.Active == true && d.Deleted == false);
                    if (order == null)
                        throw new Exception("Không tìm thấy thông tin thanh toán !");
                    pagedDataModel.Items[i].OrderModel = mapper.Map<OrderModel>(order);
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

        [HttpPost]
        [AppAuthorize]
        [Route("payment-by-momo")]
        [Description("Thanh toán momo")]
        public async Task<AppDomainResult> PaymentByMomo([FromBody] PaymentCreate itemModel)
        {
            if (ModelState.IsValid)
            {
                var item = mapper.Map<Payment>(itemModel);
                if (item != null)
                {
                    var rs= await paymentByMomoService.PaymentByMomo(item);

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

        [HttpGet]
        [Route("ConfirmPaymentMomo")]
        [AllowAnonymous]
        public async Task<AppDomainResult> ConfirmPaymentMomo([FromQuery]MomoUtilities.Result result)
        {
            
            await paymentByMomoService.ProcessPaymentResult(result);
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                ResultMessage = "Lấy Thông tin sau khi than toán của momo thành công!",
                Data = result,
                Success = true
            }; 
        }

        [HttpPost]
        [AppAuthorize]
        [Route("payment-by-vnpay")]
        [Description("Thanh toán VnPay")]
        public async Task<AppDomainResult> PaymentByVnPay([FromBody] PaymentCreate itemModel)
        {
            if (ModelState.IsValid)
            {
                var item = mapper.Map<Payment>(itemModel);
                if (item != null)
                {
                    var rs = await paymentByVnPayService.PaymentByVnPay(item);

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

        [HttpGet]
        [Route("ConfirmPaymentVNPay")]
        [AllowAnonymous]
        public async Task<AppDomainResult> ConfirmPaymentVNpay()
        {
            var response = await paymentByVnPayService.PaymentExecute(Request.Query);
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                ResultMessage = "Lấy Thông tin sau khi than toán của momo thành công!",
                Data = response,
                Success = true
            };
        }

        [HttpPost]
        [AppAuthorize]
        [Route("payment-by-paypal")]
        [Description("Thanh toán paypal")]
        public async Task<AppDomainResult> PaymentByPayPal([FromBody] PaymentCreate itemModel)
        {
            if (ModelState.IsValid)
            {
                var item = mapper.Map<Payment>(itemModel);
                if (item != null)
                {
                    var rs = await paymentByPaypalService.PaymentSubscriptionByPaypal(item);

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

        [HttpGet]
        [Route("ConfirmPaymentPaypal")]
        [AllowAnonymous]
        public async Task<AppDomainResult> ConfirmPaymentPaypal()
        {
            var response = await paymentByPaypalService.PaymentByPaypalResult(Request.Query);
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                ResultMessage = "Lấy Thông tin sau khi than toán của paypal thành công!",
                Data = response,
                Success = true
            };
        }

        [HttpPost]
        [AppAuthorize]
        [Route("payment-by-stripe")]
        [Description("Thanh toán stripe")]
        public async Task<AppDomainResult> PaymentByStripe([FromBody] PaymentCreate itemModel)
        {
            if (ModelState.IsValid)
            {
                var item = mapper.Map<Payment>(itemModel);
                if (item != null)
                {
                    var rs = await paymentByStripeService.PaymentByStripe(item);

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

        [HttpGet]
        [Route("ConfirmPaymentStripe")]
        [AllowAnonymous]
        public async Task<AppDomainResult> ConfirmPaymentStripe()
        {
            var response = await paymentByStripeService.PaymentByStripeResult(Request.Query);
            return new AppDomainResult()
            {
                ResultCode = (int)HttpStatusCode.OK,
                ResultMessage = "Lấy Thông tin sau khi than toán của paypal thành công!",
                Data = response,
                Success = true
            };
        }
    }
}