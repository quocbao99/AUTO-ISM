using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
    public class Policy : DomainEntities.DomainEntities
    {
        public string HTMLContent { get; set; }
        public bool IsUsed { get; set; }
    }
}
