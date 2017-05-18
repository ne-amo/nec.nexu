using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class DownloadFormat : LocalizeDisplayName
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string HtmlMessage { get; set; }
        public int SortOrder { get; set; }

    }
}
