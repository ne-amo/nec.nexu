using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nec.nexu.Models;

namespace nec.nexu.ViewModels
{
    public class UserDesignerRankViewModel
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Nickname { get; set; }

        public List<int> PurchasedProducts { get; set; }

        public List<int> LikedProducts { get; set; }

        public Country UserCountry { get; set; }

        public decimal DesignScore { get; set; }
    }
}
