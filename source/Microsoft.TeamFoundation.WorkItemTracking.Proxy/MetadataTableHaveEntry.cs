// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.MetadataTableHaveEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public struct MetadataTableHaveEntry
  {
    private long m_rowVersion;
    private string m_tableName;

    public MetadataTableHaveEntry(long rowVersion, string name)
    {
      this.m_rowVersion = rowVersion;
      this.m_tableName = name;
    }

    public long RowVersion
    {
      get => this.m_rowVersion;
      set => this.m_rowVersion = value;
    }

    public string TableName
    {
      get => this.m_tableName;
      set => this.m_tableName = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static MetadataTableHaveEntry FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      MetadataTableHaveEntry metadataTableHaveEntry = new MetadataTableHaveEntry();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "RowVersion":
              metadataTableHaveEntry.m_rowVersion = XmlUtility.Int64FromXmlElement(reader);
              continue;
            case "TableName":
              metadataTableHaveEntry.m_tableName = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return metadataTableHaveEntry;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("MetadataTableHaveEntry instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  RowVersion: " + this.m_rowVersion.ToString());
      stringBuilder.AppendLine("  TableName: " + this.m_tableName);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_rowVersion != 0L)
        XmlUtility.ToXmlElement(writer, "RowVersion", this.m_rowVersion);
      if (this.m_tableName != null)
        XmlUtility.ToXmlElement(writer, "TableName", this.m_tableName);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, MetadataTableHaveEntry obj) => obj.ToXml(writer, element);
  }
}
