// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Metadata.XmlReaderExtensions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Text;
using System.Xml;

namespace Microsoft.OData.Metadata
{
  internal static class XmlReaderExtensions
  {
    internal static string ReadElementValue(this XmlReader reader)
    {
      string str = reader.ReadElementContentValue();
      reader.Read();
      return str;
    }

    internal static string ReadElementContentValue(this XmlReader reader)
    {
      reader.MoveToElement();
      string str = (string) null;
      if (reader.IsEmptyElement)
      {
        str = string.Empty;
      }
      else
      {
        StringBuilder stringBuilder = (StringBuilder) null;
        bool flag = false;
        while (!flag && reader.Read())
        {
          switch (reader.NodeType)
          {
            case XmlNodeType.Text:
            case XmlNodeType.CDATA:
            case XmlNodeType.SignificantWhitespace:
              if (str == null)
              {
                str = reader.Value;
                continue;
              }
              if (stringBuilder == null)
              {
                stringBuilder = new StringBuilder();
                stringBuilder.Append(str);
                stringBuilder.Append(reader.Value);
                continue;
              }
              stringBuilder.Append(reader.Value);
              continue;
            case XmlNodeType.ProcessingInstruction:
            case XmlNodeType.Comment:
            case XmlNodeType.Whitespace:
              continue;
            case XmlNodeType.EndElement:
              flag = true;
              continue;
            default:
              throw new ODataException(Strings.XmlReaderExtension_InvalidNodeInStringValue((object) reader.NodeType));
          }
        }
        if (stringBuilder != null)
          str = stringBuilder.ToString();
        else if (str == null)
          str = string.Empty;
      }
      return str;
    }

    internal static bool NamespaceEquals(this XmlReader reader, string namespaceUri) => (object) reader.NamespaceURI == (object) namespaceUri;

    internal static bool LocalNameEquals(this XmlReader reader, string localName) => (object) reader.LocalName == (object) localName;

    internal static bool TryReadToNextElement(this XmlReader reader)
    {
      while (reader.Read())
      {
        if (reader.NodeType == XmlNodeType.Element)
          return true;
      }
      return false;
    }
  }
}
