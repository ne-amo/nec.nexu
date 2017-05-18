using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nec.nexu.Models;
using nec.nexu.ViewModels;

namespace nec.nexu.site.admin.Models
{
    public class FaceoffAdminViewModel
    {
        public FaceOff Faceoff { get; set; }
        public List<FinishedProductGalleryDetailView> AllFinishedProducts { get; set; }
        public int SelectedFinishedProductA { get; set; }
        public int SelectedFinishedProductB { get; set; }

        public IEnumerable<SelectListItem> FinishedProductIds
        {
            get
            {
                IEnumerable<int> ids = AllFinishedProducts.Select(prod => prod.Id);
                return ids.Select(id => new SelectListItem() {Text = id.ToString(), Value = id.ToString()});
            }
        } 
    }
}