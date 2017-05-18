using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nec.nexu.JsonModels
{
    public class UserColorList : nec.nexu.JsonModels.EntityBase
    {

        //public virtual nec.nexu.Models.ApplicationUser Creator { get; set; }

        public virtual ColorList ColorList { get; set; }
    }
}