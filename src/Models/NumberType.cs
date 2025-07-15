using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devbeat.DTE.JsonToCSharp.Models
{
    internal enum NumberType
    {
        Int,
        Long,
        Float,
        Double,
        Decimal
    }

    internal static class NumberTypeExtensions
    {
        public static string ToLowerString(this NumberType numberType)
        {
            return numberType.ToString().ToLower();
        }
    }
}
