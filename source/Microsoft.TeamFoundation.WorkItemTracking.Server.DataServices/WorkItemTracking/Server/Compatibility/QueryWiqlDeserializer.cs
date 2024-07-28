// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility.QueryWiqlDeserializer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility
{
  public class QueryWiqlDeserializer
  {
    public static bool TryDeserialize(
      XmlElement psQuery,
      out string wiql,
      out IDictionary context,
      out bool dayPrecision)
    {
      wiql = (string) null;
      context = (IDictionary) null;
      dayPrecision = true;
      XElement element1 = XElement.Parse(psQuery.OuterXml, LoadOptions.PreserveWhitespace);
      if (!element1.Elements().Any<XElement>((Func<XElement, bool>) (x => x.Name == (XName) "Wiql")))
        return false;
      wiql = element1.Element<string>((XName) "Wiql");
      bool result;
      if (bool.TryParse(element1.Element<string>((XName) QueryXmlConstants.DayPrecision), out result))
        dayPrecision = result;
      if (element1.Elements((XName) "Context").Any<XElement>())
      {
        context = (IDictionary) new Hashtable((IEqualityComparer) TFStringComparer.WorkItemQueryText);
        foreach (XElement element2 in element1.Elements((XName) "Context"))
        {
          string key = element2.Attribute<string>((XName) "Key");
          string val = element2.Attribute<string>((XName) "Value");
          string type = element2.Attribute<string>((XName) QueryXmlConstants.ValueType);
          if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrEmpty(val) && !string.IsNullOrWhiteSpace(type))
            context[(object) key] = QueryWiqlDeserializer.GetContextValue(val, type);
        }
      }
      return true;
    }

    private static object GetContextValue(string val, string type)
    {
      switch (type)
      {
        case "DateTime":
          DateTime result1;
          if (DateTime.TryParse(val, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result1))
            return (object) result1;
          break;
        case "Number":
          int result2;
          if (int.TryParse(val, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result2))
            return (object) result2;
          break;
        case "Double":
          double result3;
          if (double.TryParse(val, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result3))
            return (object) result3;
          break;
      }
      return (object) val;
    }
  }
}
