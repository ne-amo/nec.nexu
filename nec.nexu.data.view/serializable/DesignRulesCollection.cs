using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class DesignRulesCollection
    {
        public int DesignId { get; set; }
        public List<DesignColorRule> DesignColorRules = new List<DesignColorRule>();
        public List<ColorwayRule> ColorwayRules = new List<ColorwayRule>();
        public List<ColorDifferentiationRule> ColorDifferentiationRules = new List<ColorDifferentiationRule>();
    }
}
