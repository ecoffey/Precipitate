using System;
using System.Xml.Linq;

namespace Precipitate
{
    public static class SafeXElementValue
    {
        public static string SafeValue(this XElement xElement)
        {
            return xElement != null ? xElement.Value : String.Empty;
        }
    }
}