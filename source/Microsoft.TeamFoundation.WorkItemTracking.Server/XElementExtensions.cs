// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.XElementExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class XElementExtensions
  {
    public static T Attribute<T>(this XElement element, XName name) => element.Attribute<T>(name, default (T));

    public static T Attribute<T>(this XElement element, XName name, T defaultValue)
    {
      XAttribute xattribute = element.Attribute(name);
      return xattribute != null ? CommonWITUtils.FromString<T>(xattribute.Value, defaultValue) : defaultValue;
    }

    public static T Attribute<T>(this XElement element, XName name, Func<string, T> converter = null)
    {
      XAttribute xattribute = element.Attribute(name);
      if (xattribute == null)
        return default (T);
      if (converter == null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        converter = XElementExtensions.\u003CAttribute\u003EO__2_0<T>.\u003C0\u003E__FromString ?? (XElementExtensions.\u003CAttribute\u003EO__2_0<T>.\u003C0\u003E__FromString = new Func<string, T>(CommonWITUtils.FromString<T>));
      }
      return converter(xattribute.Value);
    }

    public static T Element<T>(this XElement element, XName name, Func<string, T> converter = null)
    {
      XElement xelement = element.Element(name);
      if (xelement == null)
        return default (T);
      if (converter == null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        converter = XElementExtensions.\u003CElement\u003EO__3_0<T>.\u003C0\u003E__FromString ?? (XElementExtensions.\u003CElement\u003EO__3_0<T>.\u003C0\u003E__FromString = new Func<string, T>(CommonWITUtils.FromString<T>));
      }
      return converter(xelement.Value);
    }

    public static XmlElement ToXmlElement(this XElement element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      XmlReaderSettings settings = new XmlReaderSettings();
      settings.DtdProcessing = DtdProcessing.Prohibit;
      settings.XmlResolver = (XmlResolver) null;
      using (XmlReader reader1 = element.CreateReader())
      {
        using (XmlReader reader2 = XmlReader.Create(reader1, settings))
        {
          int content = (int) reader2.MoveToContent();
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load(reader2);
          return xmlDocument.DocumentElement;
        }
      }
    }
  }
}
