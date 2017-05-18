using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    /*
    Rules

    Logo Rules

    Type: Required Logo
        Enforced Upon: Design datasource is populated. Server Side
	
    Type: Color Exclusion/Inclusion
        Enforced Upon: Layer select / Colorwheel load. Configurator side.

    Type: Colorway Exclusion/Inclusion
        Enforced Upon: "Completing" a logo re-coloring. Configurator calls service.

    Type: Color Differentiation
        Enforced Upon: "Completing" a logo re-coloring. Configurator side.
    
    Type: Country Exlusion 
        Enforced during search. Server side
     * 
     * 
    */

    public enum DesignRuleType
    {
        NONE,
        REQUIREDLOGO,
        COLORESTRICTION,
        COLORWAY,
        DIFFERENTIATION,
        COUNTRY
    }

    public interface IRule
    {
        int Id { get; set; }
        bool Active { get; set; }
        string DisplayName { get; set; }
        string UserMessage { get; set; }
        //DesignRuleType Type { get; set; }
    }


    public abstract class DesignRuleBase : IRule
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string DisplayName { get; set; }
        public string UserMessage { get; set; }
        public int DesignId { get; set; }
    }

    public class CountryDesignRule : DesignRuleBase
    {
        public DesignRuleType Type = DesignRuleType.COUNTRY;
        public int CountryId { get; set; }
    }

    /// <summary>
    /// Can be used to enforce strict use of a rear logo when a specific front logo is chosen.
    /// </summary>
    public class RequiredLogoRule : DesignRuleBase
    {
        public DesignRuleType Type = DesignRuleType.REQUIREDLOGO;
        public int SourceLocationId { get; set; }
        public int TargetDesignId { get; set; }
        public int TargetLocationId { get; set; }
    }

    /// <summary>
    /// Can be used to limit colors available when recoloring portions of a logo. 
    /// Combination must be "and" or  "or" . This should be the same
    /// Operator should be: "!=" or "=="
    /// Value should be ID value of DesignColor.
    /// </summary>
    public class DesignColorRule : DesignRuleBase
    {
        public DesignRuleType Type = DesignRuleType.COLORESTRICTION;
        public int ImageDataId { get; set; }
        public string Combination { get; set; }
        public const string Field = "id";
        public virtual ICollection<DesignColorRuleValue> Values { get; set; }
    }
    public class DesignColorRuleValue
    {
        public int Id { get; set; }
        public int DesignColorId { get; set; }
        public string Operator { get; set; }
    }

    /// <summary>
    /// Can be used to prevent the user from submitting with a particular color combination.
    /// </summary>
    public class ColorwayRule : DesignRuleBase
    {
        public DesignRuleType Type = DesignRuleType.COLORWAY;
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<ColorwayRuleValue> Values { get; set; }
    }
    public class ColorwayRuleValue
    {
        public int Id { get; set; }
        public int ImageDataId { get; set; }
        public int DesignColorId { get; set; }
    }

    /// <summary>
    /// Can be used to prevent the user from creating tonal color combinations.
    /// </summary>
    public class ColorDifferentiationRule : DesignRuleBase
    {
        public DesignRuleType Type = DesignRuleType.DIFFERENTIATION;
        public int RequiredColors { get; set; }
    }
}
