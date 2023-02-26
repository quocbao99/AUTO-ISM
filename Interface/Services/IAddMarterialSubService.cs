using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IAddMarterialSubService
    {
        Task<bool> AddMarterialSub(Guid materialID, string contentRootPath);
    }
}
