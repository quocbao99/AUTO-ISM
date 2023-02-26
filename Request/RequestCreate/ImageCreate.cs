using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request.RequestCreate
{
    public class ImageCreate : DomainCreate
    {
        /// <summary>
        /// link hình
        /// </summary>
        public string Link { get; set; }
    }
}
