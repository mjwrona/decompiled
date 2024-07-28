// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Common.XmlElementValueExtensions
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
  internal static class XmlElementValueExtensions
  {
    internal static IEnumerable<XmlElementValue<T>> OfResultType<T>(
      this IEnumerable<XmlElementValue> elements)
      where T : class
    {
      foreach (XmlElementValue element in elements)
      {
        if (element is XmlElementValue<T> xmlElementValue)
          yield return xmlElementValue;
        else if (element.UntypedValue is T)
          yield return new XmlElementValue<T>(element.Name, element.Location, element.ValueAs<T>());
      }
    }

    internal static IEnumerable<T> ValuesOfType<T>(this IEnumerable<XmlElementValue> elements) where T : class => elements.OfResultType<T>().Select<XmlElementValue<T>, T>((Func<XmlElementValue<T>, T>) (ev => ev.Value));

    internal static IEnumerable<XmlTextValue> OfText(this IEnumerable<XmlElementValue> elements)
    {
      foreach (XmlElementValue element in elements)
      {
        if (element.IsText)
          yield return (XmlTextValue) element;
      }
    }
  }
}
