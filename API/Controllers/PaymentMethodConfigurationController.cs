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

namespace BaseAPI.Controllers
{
    /// <summary>
    /// Quản lý cấu hình Thanh toán
    /// </summary>
    [Route("api/paymentConfiguration")]
    [ApiController]
    [Description("Quản lý cấu hình thanh toán")]
    [Authorize]
    public class PaymentMethodConfigurationController : BaseController<PaymentMethodConfiguration, PaymentMethodConfigurationModel, PaymentMethodConfigurationCreate, PaymentMethodConfigurationUpdate, PaymentMethodConfigurationSearch>
    {
        private IPaymentMethodConfigurationService PaymentMethodConfiguration;
        private IPaymentMethodTypeService paymentMethodType;
        public PaymentMethodConfigurationController(IServiceProvider serviceProvider, ILogger<BaseController<PaymentMethodConfiguration, PaymentMethodConfigurationModel, PaymentMethodConfigurationCreate, PaymentMethodConfigurationUpdate, PaymentMethodConfigurationSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IPaymentMethodConfigurationService>();
            this.paymentMethodType = serviceProvider.GetRequiredService<IPaymentMethodTypeService>();
        }
        public async override Task<AppDomainResult> GetById(Guid id)
        {
            PaymentMethodConfiguration item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                var itemModel = mapper.Map<PaymentMethodConfigurationModel>(item);
                var paymentMdType = await paymentMethodType.GetByIdAsync((Guid)item.PaymentMethodID);
                if (paymentMdType == null) throw new Exception("Không tìm thấy thông tin !");
                itemModel.PaymentMethodTypeModel = mapper.Map<PaymentMethodTypeModel>(paymentMdType);

            }
            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
        }
        public async override Task<AppDomainResult> Get([FromQuery] PaymentMethodConfigurationSearch baseSearch)
        {
            if (ModelState.IsValid)
            {
                PagedList<PaymentMethodConfiguration> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<PaymentMethodConfigurationModel> pagedDataModel = mapper.Map<PagedList<PaymentMethodConfigurationModel>>(pagedData);
                if (pagedDataModel.Items == null) {
                    return new AppDomainResult
                    {
                        Data = pagedDataModel,
                        Success = true,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                for (int i = 0; i < pagedDataModel.Items.Count(); i++)
                {

                    var paymentMdType = await paymentMethodType.GetByIdAsync((Guid)pagedDataModel.Items[i].PaymentMethodID);
                    if (paymentMdType == null) throw new Exception("Không tìm thấy thông tin khách hàng !");
                    pagedDataModel.Items[i].PaymentMethodTypeModel = mapper.Map<PaymentMethodTypeModel>(paymentMdType);
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