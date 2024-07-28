// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.XmlComparisonUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public static class XmlComparisonUtils
  {
    public static void Compare(
      XElement a,
      XElement b,
      StringComparer comparer,
      bool ensureElementOrder = true)
    {
      if (a.Name != b.Name)
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Elements have different names: {0}, {1}", (object) a.Name, (object) b.Name));
      if (a.Attributes().Count<XAttribute>() != b.Attributes().Count<XAttribute>())
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Element {0} has different numbers of attributes: {1}, {2}", (object) a.Name, (object) a.Attributes().Count<XAttribute>(), (object) b.Attributes().Count<XAttribute>()));
      foreach (XAttribute attribute in a.Attributes())
      {
        if (b.Attribute(attribute.Name) == null)
          throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Attribute {0} does not exist in element {1}", (object) attribute.Name, (object) b.Name));
      }
      XElement[] array1 = a.Elements().ToArray<XElement>();
      XElement[] array2 = b.Elements().ToArray<XElement>();
      if (array1.Length != array2.Length)
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Element {0} has different numbers of elemenets: {1}, {2}", (object) a.Name, (object) array1.Length, (object) array2.Length));
      if (array1.Length == 0)
        return;
      for (int index = 0; index < array1.Length; ++index)
      {
        XElement aElem = array1[index];
        XElement b1 = array2[index];
        if (!ensureElementOrder)
        {
          b1 = ((IEnumerable<XElement>) array2).Where<XElement>((Func<XElement, bool>) (x =>
          {
            if (x.Name != aElem.Name)
              return false;
            foreach (XAttribute attribute in aElem.Attributes())
            {
              if (x.Attribute(attribute.Name) == null)
                return false;
            }
            return true;
          })).FirstOrDefault<XElement>();
          if (b1 == null)
            throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to find match element {0}", (object) aElem.Name));
        }
        XmlComparisonUtils.Compare(aElem, b1, comparer, false);
      }
    }
  }
}
