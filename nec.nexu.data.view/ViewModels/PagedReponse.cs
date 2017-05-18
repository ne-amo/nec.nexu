using System;
using System.Collections.Generic;

namespace nec.nexu.ViewModels
{
    public class PagedResponse<T>
    {
        public int Total { get; set; }
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
