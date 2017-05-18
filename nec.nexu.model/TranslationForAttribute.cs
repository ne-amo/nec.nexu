using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nec.nexu.Models
{

    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
    public class TranslationFor : System.Attribute
    {
        string propertyname;
        public TranslationFor(string name)
        {
            this.propertyname = name;
        }
        public string GetTarget()
        {
            return propertyname;
        }
    }
}
