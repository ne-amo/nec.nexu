using System;

namespace nec.nexu.ViewModels
{
    public class DesignResultMessage
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string TapeNumber { get; set; }
        public int SortOrder { get; set; }
        public int HeightPixel { get; set; }
        public int WidthPixel { get; set; }
        public string OfficialImage { get; set; }
        public string ThumbnailImage { get; set; }
        public int DefaultColorwayId { get; set; }
        public int OfficialColorwayId { get; set; }
        public bool Active { get; set; }
    }
}
