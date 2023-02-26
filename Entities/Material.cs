using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class Material : DomainEntities.DomainEntities
    {
        public Guid SystemFileID { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public string FilePath { get; set; }
        public long Size { get; set; }
        [NotMapped]
        public string SysTemFileName { get; set; }
    }
}
