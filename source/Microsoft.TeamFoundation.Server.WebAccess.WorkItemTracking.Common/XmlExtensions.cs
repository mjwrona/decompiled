// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.XmlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal static class XmlExtensions
  {
    public static XmlElement ToXmlElement(this XElement element)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      using (XmlReader reader1 = element.CreateReader())
      {
        using (XmlReader reader2 = XmlReader.Create(reader1, settings))
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load(reader2);
          return xmlDocument.DocumentElement;
        }
      }
    }

    public static XElement ToXElement(this XmlElement element)
    {
      XDocument xdocument = new XDocument();
      using (XmlWriter writer = xdocument.CreateWriter())
        element.WriteTo(writer);
      return xdocument.Root;
    }
  }
}
