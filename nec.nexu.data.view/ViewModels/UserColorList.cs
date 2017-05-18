using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nec.nexu.model.Models.ComplexTypes;

namespace nec.nexu.Models
{
    public class UserColorList : EntityBase<int>
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(128)]
        public string CreatorId { get; set; }
        //public virtual ApplicationUser Creator { get; set; }

        public string DisplayName { get; set; }

        public virtual PersistableIntCollection MaterialIds { get; set; }
        //public virtual UserCustomColorList ColorList { get; set; }
    }
}