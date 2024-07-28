// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.QuerySortOrderEntry
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
  public struct QuerySortOrderEntry
  {
    private bool m_ascending;
    private bool? m_nullsFirst;
    private string m_columnName;

    public bool Ascending
    {
      get => this.m_ascending;
      set => this.m_ascending = value;
    }

    public bool? NullsFirst
    {
      get => this.m_nullsFirst;
      set => this.m_nullsFirst = value;
    }

    public string ColumnName
    {
      get => this.m_columnName;
      set => this.m_columnName = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static QuerySortOrderEntry FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      QuerySortOrderEntry querySortOrderEntry = new QuerySortOrderEntry();
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
            case "Ascending":
              querySortOrderEntry.m_ascending = XmlUtility.BooleanFromXmlElement(reader);
              continue;
            case "ColumnName":
              querySortOrderEntry.m_columnName = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "NullsFirst":
              querySortOrderEntry.NullsFirst = new bool?(XmlUtility.BooleanFromXmlElement(reader));
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return querySortOrderEntry;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("QuerySortOrderEntry instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Ascending: " + this.m_ascending.ToString());
      stringBuilder.AppendLine("  NullsFirst: " + this.m_nullsFirst.ToString());
      stringBuilder.AppendLine("  ColumnName: " + this.m_columnName);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_ascending)
        XmlUtility.ToXmlElement(writer, "Ascending", this.m_ascending);
      if (this.m_nullsFirst.HasValue)
        XmlUtility.ToXmlElement(writer, "NullsFirst", this.m_nullsFirst.Value);
      if (this.m_columnName != null)
        XmlUtility.ToXmlElement(writer, "ColumnName", this.m_columnName);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, QuerySortOrderEntry obj) => obj.ToXml(writer, element);
  }
}
