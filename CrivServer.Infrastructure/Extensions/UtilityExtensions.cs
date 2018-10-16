using System;
using System.Collections.Generic;
using System.Text;

namespace CrivServer.Infrastructure.Extensions
{
    class UtilityExtensions
    {
    }

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
