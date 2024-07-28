// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDatabaseXmlWriter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationDatabaseXmlWriter
  {
    private bool m_xmlFlushed;
    private StringWriter m_stringWriter;
    private XmlTextWriter m_xmlTextWriter;

    public TeamFoundationDatabaseXmlWriter(string startDocumentElement)
    {
      this.m_stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      this.m_xmlTextWriter = new XmlTextWriter((TextWriter) this.m_stringWriter);
      this.m_xmlTextWriter.WriteStartDocument();
      this.m_xmlTextWriter.WriteStartElement(startDocumentElement);
    }

    public void WriteStartElement(string localName) => this.m_xmlTextWriter.WriteStartElement(localName);

    public void WriteAttributeString(string localName, string value) => this.m_xmlTextWriter.WriteAttributeString(localName, value);

    public void WriteEndElement() => this.m_xmlTextWriter.WriteEndElement();

    public string GetXmlString()
    {
      if (!this.m_xmlFlushed)
      {
        this.m_xmlTextWriter.WriteEndElement();
        this.m_xmlTextWriter.WriteEndDocument();
        this.m_xmlTextWriter.Flush();
        this.m_xmlFlushed = true;
      }
      return this.m_stringWriter.ToString();
    }
  }
}
