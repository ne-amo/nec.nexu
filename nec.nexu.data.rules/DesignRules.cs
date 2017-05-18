using System;
using System.Collections.Generic;

namespace nec.nexu.Models.rules
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
       
        string DisplayName { get; set; }
        string UserMessage { get; set; }
        //DesignRuleType Type { get; set; }
    }


    public abstract class DesignRuleBase<TPrimaryKey> : IRule
    {
        public virtual TPrimaryKey Id { get; set; }
        public virtual bool Active { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string UserMessage { get; set; }
        public virtual int DesignId { get; set; }

        public virtual string LocalizedUserMessageXml
        {
            get { return LocalizedUserMessage.Value; }
            set { LocalizedUserMessage.Value = value; }
        }
        private MultilingualString _LocalizedUserMessage;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        [TranslationFor("UserMessage")]
        public virtual MultilingualString LocalizedUserMessage
        {
            get { return _LocalizedUserMessage ?? (_LocalizedUserMessage = new MultilingualString()); }
            set { _LocalizedUserMessage = value; }
        }
    }

    public class CountryDesignRule : DesignRuleBase<int>
    {
        public DesignRuleType Type = DesignRuleType.COUNTRY;
        public int CountryId { get; set; }
    }

    /// <summary>
    /// Can be used to enforce strict use of a rear logo when a specific front logo is chosen.
    /// </summary>
    public class RequiredLogoRule : DesignRuleBase<int>
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
    public class DesignColorRule : DesignRuleBase<int>
    {
        public void BuildValues(DesignColorRule rule)
        {
            this.Values = rule.Values;
        }
        public DesignRuleType Type = DesignRuleType.COLORESTRICTION;
        public int ImageDataId { get; set; }
        public string Combination { get; set; }
        public const string Field = "id";
        //public Dictionary<string, string> ValueOperatorFilters { get; set; }
        public virtual List<DesignColorRuleValue> Values { get; set; }
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
    public  class ColorwayRule : DesignRuleBase<int>
    {
        public DesignRuleType Type = DesignRuleType.COLORWAY;
        //public Dictionary<int, int> ImageDataAndColorId { get; set; }
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
    public class ColorDifferentiationRule : DesignRuleBase<int>
    {
        public DesignRuleType Type = DesignRuleType.DIFFERENTIATION;
        public int RequiredColors { get; set; }
    }
}
