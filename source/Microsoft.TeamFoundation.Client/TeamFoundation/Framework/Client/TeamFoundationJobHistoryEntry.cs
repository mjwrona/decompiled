// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationJobHistoryEntry
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class TeamFoundationJobHistoryEntry
  {
    private Guid m_agentId = Guid.Empty;
    private DateTime m_endTime = DateTime.MinValue;
    private DateTime m_executionStartTime = DateTime.MinValue;
    private long m_historyId;
    private Guid m_jobId = Guid.Empty;
    private Guid m_jobSource = Guid.Empty;
    private int m_priority = -1;
    private DateTime m_queueTime = DateTime.MinValue;
    private int m_queuedReasons;
    private int m_result;
    private string m_resultMessage;

    public TeamFoundationJobResult Result
    {
      get => (TeamFoundationJobResult) this.m_result;
      set => this.m_result = (int) value;
    }

    private TeamFoundationJobHistoryEntry()
    {
    }

    public Guid AgentId => this.m_agentId;

    public DateTime EndTime => this.m_endTime;

    public DateTime ExecutionStartTime => this.m_executionStartTime;

    public long HistoryId => this.m_historyId;

    public Guid JobId => this.m_jobId;

    public Guid JobSource => this.m_jobSource;

    public DateTime QueueTime => this.m_queueTime;

    public string ResultMessage => this.m_resultMessage;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TeamFoundationJobHistoryEntry FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      TeamFoundationJobHistoryEntry foundationJobHistoryEntry = new TeamFoundationJobHistoryEntry();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
          if (name != null)
          {
            switch (name.Length)
            {
              case 2:
                if (name == "qr")
                {
                  foundationJobHistoryEntry.m_queuedReasons = XmlUtility.Int32FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 3:
                if (name == "hid")
                {
                  foundationJobHistoryEntry.m_historyId = XmlUtility.Int64FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 5:
                if (name == "JobId")
                {
                  foundationJobHistoryEntry.m_jobId = XmlUtility.GuidFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 6:
                if (name == "result")
                {
                  foundationJobHistoryEntry.m_result = XmlUtility.Int32FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 7:
                switch (name[0])
                {
                  case 'A':
                    if (name == "AgentId")
                    {
                      foundationJobHistoryEntry.m_agentId = XmlUtility.GuidFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'E':
                    if (name == "EndTime")
                    {
                      foundationJobHistoryEntry.m_endTime = XmlUtility.DateTimeFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 8:
                if (name == "priority")
                {
                  foundationJobHistoryEntry.m_priority = XmlUtility.Int32FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 9:
                switch (name[0])
                {
                  case 'J':
                    if (name == "JobSource")
                    {
                      foundationJobHistoryEntry.m_jobSource = XmlUtility.GuidFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'Q':
                    if (name == "QueueTime")
                    {
                      foundationJobHistoryEntry.m_queueTime = XmlUtility.DateTimeFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 18:
                if (name == "ExecutionStartTime")
                {
                  foundationJobHistoryEntry.m_executionStartTime = XmlUtility.DateTimeFromXmlAttribute(reader);
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "ResultMessage")
            foundationJobHistoryEntry.m_resultMessage = XmlUtility.StringFromXmlElement(reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return foundationJobHistoryEntry;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("TeamFoundationJobHistoryEntry instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  AgentId: " + this.m_agentId.ToString());
      stringBuilder.AppendLine("  EndTime: " + this.m_endTime.ToString());
      stringBuilder.AppendLine("  ExecutionStartTime: " + this.m_executionStartTime.ToString());
      stringBuilder.AppendLine("  HistoryId: " + this.m_historyId.ToString());
      stringBuilder.AppendLine("  JobId: " + this.m_jobId.ToString());
      stringBuilder.AppendLine("  JobSource: " + this.m_jobSource.ToString());
      stringBuilder.AppendLine("  Priority: " + this.m_priority.ToString());
      stringBuilder.AppendLine("  QueueTime: " + this.m_queueTime.ToString());
      stringBuilder.AppendLine("  QueuedReasons: " + this.m_queuedReasons.ToString());
      stringBuilder.AppendLine("  Result: " + this.m_result.ToString());
      stringBuilder.AppendLine("  ResultMessage: " + this.m_resultMessage);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_agentId != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "AgentId", this.m_agentId);
      if (this.m_endTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "EndTime", this.m_endTime);
      if (this.m_executionStartTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "ExecutionStartTime", this.m_executionStartTime);
      if (this.m_historyId != 0L)
        XmlUtility.ToXmlAttribute(writer, "hid", this.m_historyId);
      if (this.m_jobId != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "JobId", this.m_jobId);
      if (this.m_jobSource != Guid.Empty)
        XmlUtility.ToXmlAttribute(writer, "JobSource", this.m_jobSource);
      if (this.m_priority != -1)
        XmlUtility.ToXmlAttribute(writer, "priority", this.m_priority);
      if (this.m_queueTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "QueueTime", this.m_queueTime);
      if (this.m_queuedReasons != 0)
        XmlUtility.ToXmlAttribute(writer, "qr", this.m_queuedReasons);
      if (this.m_result != 0)
        XmlUtility.ToXmlAttribute(writer, "result", this.m_result);
      if (this.m_resultMessage != null)
        XmlUtility.ToXmlElement(writer, "ResultMessage", this.m_resultMessage);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, TeamFoundationJobHistoryEntry obj) => obj.ToXml(writer, element);
  }
}
