using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nec.nexu.model.Models;

namespace nec.nexu.data.security.ComplexTypes
{
    [ComplexType]
    public class PersistableStringCollection : PersistableScalarCollection<string>
    {
        protected override string ConvertSingleValueToRuntime(string rawValue)
        {
            return rawValue;
        }

        protected override string ConvertSingleValueToPersistable(string value)
        {
            return value;
        }
    }
}
