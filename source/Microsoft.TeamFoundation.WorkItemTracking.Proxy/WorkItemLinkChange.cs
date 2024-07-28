// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.WorkItemLinkChange
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
  public sealed class WorkItemLinkChange
  {
    private DateTime m_changedDate = DateTime.MinValue;
    private bool m_isActive;
    private string m_linkType;
    private long m_rowVersion;
    private int m_sourceID;
    private int m_targetID;

    private WorkItemLinkChange()
    {
    }

    public DateTime ChangedDate
    {
      get => this.m_changedDate;
      set => this.m_changedDate = value;
    }

    public bool IsActive
    {
      get => this.m_isActive;
      set => this.m_isActive = value;
    }

    public string LinkType
    {
      get => this.m_linkType;
      set => this.m_linkType = value;
    }

    public long RowVersion
    {
      get => this.m_rowVersion;
      set => this.m_rowVersion = value;
    }

    public int SourceID
    {
      get => this.m_sourceID;
      set => this.m_sourceID = value;
    }

    public int TargetID
    {
      get => this.m_targetID;
      set => this.m_targetID = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static WorkItemLinkChange FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      WorkItemLinkChange workItemLinkChange = new WorkItemLinkChange();
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
            case "ChangedDate":
              workItemLinkChange.m_changedDate = XmlUtility.DateTimeFromXmlElement(reader);
              continue;
            case "IsActive":
              workItemLinkChange.m_isActive = XmlUtility.BooleanFromXmlElement(reader);
              continue;
            case "LinkType":
              workItemLinkChange.m_linkType = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "RowVersion":
              workItemLinkChange.m_rowVersion = XmlUtility.Int64FromXmlElement(reader);
              continue;
            case "SourceID":
              workItemLinkChange.m_sourceID = XmlUtility.Int32FromXmlElement(reader);
              continue;
            case "TargetID":
              workItemLinkChange.m_targetID = XmlUtility.Int32FromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return workItemLinkChange;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("WorkItemLinkChange instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ChangedDate: " + this.m_changedDate.ToString());
      stringBuilder.AppendLine("  IsActive: " + this.m_isActive.ToString());
      stringBuilder.AppendLine("  LinkType: " + this.m_linkType);
      stringBuilder.AppendLine("  RowVersion: " + this.m_rowVersion.ToString());
      stringBuilder.AppendLine("  SourceID: " + this.m_sourceID.ToString());
      stringBuilder.AppendLine("  TargetID: " + this.m_targetID.ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_changedDate != DateTime.MinValue)
        XmlUtility.ToXmlElement(writer, "ChangedDate", this.m_changedDate);
      if (this.m_isActive)
        XmlUtility.ToXmlElement(writer, "IsActive", this.m_isActive);
      if (this.m_linkType != null)
        XmlUtility.ToXmlElement(writer, "LinkType", this.m_linkType);
      if (this.m_rowVersion != 0L)
        XmlUtility.ToXmlElement(writer, "RowVersion", this.m_rowVersion);
      if (this.m_sourceID != 0)
        XmlUtility.ToXmlElement(writer, "SourceID", this.m_sourceID);
      if (this.m_targetID != 0)
        XmlUtility.ToXmlElement(writer, "TargetID", this.m_targetID);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, WorkItemLinkChange obj) => obj.ToXml(writer, element);
  }
}
