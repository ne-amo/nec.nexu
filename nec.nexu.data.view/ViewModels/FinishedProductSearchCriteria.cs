using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nec.nexu.ViewModels
{
    public class FinishedProductSearchCriteria
    {
        public string ExactKeyword;
        public string[] Keywords { get; set; }
        public int[] HiearchyIds { get; set; }
        public int[] TemplateIds { get; set; }
        public int[] ColorContainers { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string OrderBy = "RECENT";
    }
}
