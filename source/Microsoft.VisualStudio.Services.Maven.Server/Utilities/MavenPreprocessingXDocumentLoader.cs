// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenPreprocessingXDocumentLoader
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public class MavenPreprocessingXDocumentLoader
  {
    private readonly IReadOnlyDictionary<string, string> entities;

    public MavenPreprocessingXDocumentLoader(IReadOnlyDictionary<string, string> entities) => this.entities = entities;

    public XDocument Load(Stream stream)
    {
      XDocument xdocument = new XDocument();
      using (XmlTextReader reader = new XmlTextReader((Stream) new NoCloseStreamWrapper(stream)))
      {
        using (XmlWriter writer = xdocument.CreateWriter())
        {
          reader.Namespaces = false;
          reader.DtdProcessing = DtdProcessing.Prohibit;
          reader.XmlResolver = (XmlResolver) null;
          reader.EntityHandling = EntityHandling.ExpandCharEntities;
          while (reader.Read())
          {
            switch (reader.NodeType)
            {
              case XmlNodeType.Element:
                this.HandleStartElement(reader, writer);
                continue;
              case XmlNodeType.Text:
                writer.WriteString(reader.Value);
                continue;
              case XmlNodeType.CDATA:
                writer.WriteCData(reader.Value);
                continue;
              case XmlNodeType.EntityReference:
                writer.WriteString(this.ResolveEntity(reader.LocalName));
                continue;
              case XmlNodeType.ProcessingInstruction:
              case XmlNodeType.XmlDeclaration:
                continue;
              case XmlNodeType.Comment:
                writer.WriteComment(reader.Value);
                continue;
              case XmlNodeType.Whitespace:
              case XmlNodeType.SignificantWhitespace:
                writer.WriteWhitespace(reader.Value);
                continue;
              case XmlNodeType.EndElement:
                writer.WriteEndElement();
                continue;
              default:
                throw new XmlException(Resources.Error_XmlUnexpectedNode((object) reader.NodeType, (object) reader.LineNumber, (object) reader.LinePosition));
            }
          }
        }
      }
      return xdocument;
    }

    private (string Prefix, string LocalName) GetName(XmlTextReader reader)
    {
      string[] strArray = reader.Name.Split(new char[1]
      {
        ':'
      }, 2, StringSplitOptions.None);
      string str1;
      string str2;
      if (strArray.Length == 2)
      {
        str1 = XmlConvert.EncodeLocalName(strArray[0]);
        str2 = XmlConvert.EncodeLocalName(strArray[1]);
      }
      else
      {
        str1 = (string) null;
        str2 = XmlConvert.EncodeLocalName(strArray[0]);
      }
      return (str1, str2);
    }

    private bool IsNamespaceDeclaration(string prefix, string localName) => string.IsNullOrWhiteSpace(prefix) && localName == "xmlns" || prefix == "xmlns";

    private void HandleStartElement(XmlTextReader reader, XmlWriter writer)
    {
      bool isEmptyElement = reader.IsEmptyElement;
      string localName = this.GetName(reader).LocalName;
      writer.WriteStartElement(string.Empty, localName, string.Empty);
      while (reader.MoveToNextAttribute())
        this.HandleOneAttribute(reader, writer);
      if (!isEmptyElement)
        return;
      writer.WriteEndElement();
    }

    private void HandleOneAttribute(XmlTextReader reader, XmlWriter writer)
    {
      (string str1, string str2) = this.GetName(reader);
      if (this.IsNamespaceDeclaration(str1, str2))
        return;
      writer.WriteStartAttribute(string.Empty, str2, string.Empty);
      while (reader.ReadAttributeValue())
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.Text:
            writer.WriteString(reader.Value);
            continue;
          case XmlNodeType.EntityReference:
            writer.WriteString(this.ResolveEntity(reader.LocalName));
            continue;
          case XmlNodeType.Whitespace:
          case XmlNodeType.SignificantWhitespace:
            writer.WriteWhitespace(reader.Value);
            continue;
          default:
            throw new XmlException(Resources.Error_XmlUnexpectedNode((object) reader.NodeType, (object) reader.LineNumber, (object) reader.LinePosition));
        }
      }
      writer.WriteEndAttribute();
    }

    private string ResolveEntity(string entityName)
    {
      string str;
      if (this.entities.TryGetValue(entityName, out str))
        return str;
      throw new XmlException("Encountered unknown entity &" + entityName + ";");
    }
  }
}
