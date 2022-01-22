using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HMS2.Models
{
    public class BaseEntity
    {
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Updated By")]
        public string UpdatedBy { get; set; }
        [DisplayName("Updated Date")]
        public DateTime UpdatedDate { get; set; }

    }
}

