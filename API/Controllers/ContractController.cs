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
    /// Quản lý hợp đồng
    /// </summary>
    [Route("api/contract")]
    [ApiController]
    [Description("Quản lý hợp đồng")]
    [Authorize]
    public class ContractController : BaseController<Contract, ContractModel, ContractCreate, ContractUpdate, ContractSearch>
    {
        private IUserService userService;
        private IPaymentService paymentService;
        private IPaymentMethodConfigurationService paymentMethodConfigurationService;
        private IPaymentMethodTypeService paymentMethodTypeService;
        private IContractSpecializingService contractSpecializingService;
        public ContractController(IServiceProvider serviceProvider, ILogger<BaseController<Contract, ContractModel, ContractCreate, ContractUpdate, ContractSearch>> logger
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IContractService>();
            this.userService = serviceProvider.GetRequiredService<IUserService>();
            this.paymentService = serviceProvider.GetRequiredService<IPaymentService>();
            this.paymentMethodConfigurationService = serviceProvider.GetRequiredService<IPaymentMethodConfigurationService>();
            this.paymentMethodTypeService = serviceProvider.GetRequiredService<IPaymentMethodTypeService>();
            this.contractSpecializingService = serviceProvider.GetRequiredService<IContractSpecializingService>();
        }

        public async override Task<AppDomainResult> GetById(Guid id)
        {
            Contract item = await this.domainService.GetByIdAsync(id);
            if (item != null)
            {
                var itemModel = mapper.Map<ContractModel>(item);
                var user = await userService.GetByIdAsync((Guid)item.UserID);
                if (user == null) throw new Exception("Không tìm thấy thông tin khách hàng !");
                itemModel.UserModel = mapper.Map<UserModel>(user);

                var payment = await paymentService.GetSingleAsync(d => d.Id == item.PaymenntID && d.Active == true && d.Deleted == false);
                if (payment == null) throw new Exception("Không tìm thấy thông tin thanh toán !");
                var paymentConfig= await paymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
                if (paymentConfig == null) throw new Exception("Không tìm thấy thông tin thanh toán !");
                var paymentMethod = await paymentMethodTypeService.GetByIdAsync(paymentConfig.PaymentMethodID);
                if (paymentMethod == null) throw new Exception("Không tìm thấy thông tin thanh toán !");
                payment.PaymentMethodName = paymentMethod.Name;
                payment.PaymentMethodCode = paymentMethod.Code;
                itemModel.PaymentModel = mapper.Map<PaymentModel>(payment);
                return new AppDomainResult() { Success = true, Data = itemModel, ResultCode = (int)HttpStatusCode.OK };
            }
            throw new KeyNotFoundException(ApiMessage.ItemNotFound);
        }

        public async override Task<AppDomainResult> Get([FromQuery] ContractSearch baseSearch)
        {
            if (ModelState.IsValid)
            {
                PagedList<Contract> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<ContractModel> pagedDataModel = mapper.Map<PagedList<ContractModel>>(pagedData);
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

                    var payment = await paymentService.GetSingleAsync(d => d.Id == pagedDataModel.Items[i].PaymenntID && d.Active == true && d.Deleted == false);
                    if (payment == null)
                        throw new Exception("Không tìm thấy thông tin thanh toán !");
                    var paymentConfig = await paymentMethodConfigurationService.GetByIdAsync((Guid)payment.PaymentMethodConfigurationID);
                    if (paymentConfig == null) throw new Exception("Không tìm thấy thông tin thanh toán !");
                    var paymentMethod = await paymentMethodTypeService.GetByIdAsync(paymentConfig.PaymentMethodID);
                    if (paymentMethod == null) throw new Exception("Không tìm thấy thông tin thanh toán !");
                    payment.PaymentMethodName = paymentMethod.Name;
                    payment.PaymentMethodCode = paymentMethod.Code;
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
        
        [HttpGet]
        [Route("CancelSubcription")]
        [AppAuthorize]
        [Description("Hủy subcription của hợp đồng và sử dụng hợp đồng đến khi hết hạn")]
        public async Task<AppDomainResult> CancelSubcription(Guid ContractID)
        {
            if (ModelState.IsValid)
            {
                var rs = await contractSpecializingService.EndedSubcription(ContractID);
                return new AppDomainResult
                {
                    Data = rs,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
        }
    }
}