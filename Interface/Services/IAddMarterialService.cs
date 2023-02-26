using Request.RequestCreate;
using Request.RequestUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IAddMarterialService
    {
        Task<bool> AddMarterial(MaterialCreate itemModel, string contentRootPath);
        Task<bool> UpdateMarterial(MaterialUpdate itemModel, string contentRootPath);
    }
}
