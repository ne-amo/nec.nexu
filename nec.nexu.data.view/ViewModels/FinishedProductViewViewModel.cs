using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nec.nexu.Models;

namespace nec.nexu.ViewModels
{
    public class FinishedProductViewViewModel
    {
        public string DisplayName { get; set; }

        //public int? MaterialId { get; set; }

        //public Guid Guid { get; set; }

        //public NexuUser Creator { get; set; }

        //public DateTime DateCreated { get; set; }

        //public DateTime DateModified { get; set; }

        //public ProductTemplate Template { get; set; }

        //public IList<FinishedRegion> Regions { get; set; }

        //public IList<FinishedDesign> Designs { get; set; }

        //public  ImageFile BackgroundImage { get; set; }

        //public IList<FinishedProductImage> Images { get; set; }

        public int UserViewCount { get; set; }
    }
}