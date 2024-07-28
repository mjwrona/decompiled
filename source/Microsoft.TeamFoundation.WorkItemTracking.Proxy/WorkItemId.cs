// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.WorkItemId
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
  public sealed class WorkItemId
  {
    private int m_id;
    private long m_rowVersion;

    private WorkItemId()
    {
    }

    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    public long RowVersion
    {
      get => this.m_rowVersion;
      set => this.m_rowVersion = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static WorkItemId FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      WorkItemId workItemId = new WorkItemId();
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
            case "Id":
              workItemId.m_id = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "RowVersion":
              workItemId.m_rowVersion = XmlUtility.Int64FromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return workItemId;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("WorkItemId instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Id: " + this.m_id.ToString());
      stringBuilder.AppendLine("  RowVersion: " + this.m_rowVersion.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_id != 0)
        XmlUtility.ToXmlElement(writer, "Id", this.m_id);
      if (this.m_rowVersion != 0L)
        XmlUtility.ToXmlElement(writer, "RowVersion", this.m_rowVersion);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, WorkItemId obj) => obj.ToXml(writer, element);
  }
}
