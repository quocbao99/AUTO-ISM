using Models.DomainModels;
using System;

namespace Models
{
    public class ImageModel : AppDomainModel
    {
        public string Link { get; set; }
        public Guid TourId { get; set; }
    }
    
}
