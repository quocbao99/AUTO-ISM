using Entities;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Specializing
{
    public interface IContractSpecializingService
    {
        Task<Contract> NewContractFromPayment(Payment payment);

        Task<bool> ExpiredContract(Guid ContractId);
        Task<bool> EndedContract(Guid ContractId);
        Task<List<Contract>> ContractsIsUsing(Guid UserID);
        Task<Contract> ContractsIsUsing(Guid userID, int packageContractType);
        Task<decimal> SubPriceForPackageIfUserhavingContractUsing(Guid userID, int packageType);
        Task<bool> EndedSubcription(Guid ContractId);
    }
}
