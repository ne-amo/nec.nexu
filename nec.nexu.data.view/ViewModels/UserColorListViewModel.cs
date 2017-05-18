using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nec.nexu.Models;

namespace nec.nexu.ViewModels
{
    public class UserColorListViewModel
    {
        //public ApplicationUser User { get; set; }
        public string CreatorId { get; set; }
        public string DisplayName { get; set; }
        public int[] MaterialIds { get; set; } 

        //public ColorListViewModel ColorList { get; set; }
    }
}