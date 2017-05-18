using System;
using nec.nexu.Models;

namespace nec.nexu.Models.rules
{
    public class CountryProductExclusion
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public int FinishedProductId { get; set; }
    }

    public class CountryHierachyExclusion
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public int? HierarchyId { get; set; }
        public int? TemplateId { get; set; }
    }
}
