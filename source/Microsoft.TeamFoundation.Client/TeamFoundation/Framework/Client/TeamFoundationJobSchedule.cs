// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationJobSchedule
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class TeamFoundationJobSchedule
  {
    private TimeZoneInfo m_timeZone;
    private int m_interval;
    private int m_priorityLevel;
    private DateTime m_scheduledTime = DateTime.MinValue;
    private string m_timeZoneId;

    public TeamFoundationJobSchedule(DateTime scheduledTime)
      : this(scheduledTime, 0)
    {
    }

    public TeamFoundationJobSchedule(DateTime scheduledTime, int interval)
      : this(scheduledTime, interval, TimeZoneInfo.Utc)
    {
    }

    public TeamFoundationJobSchedule(DateTime scheduledTime, int interval, TimeZoneInfo timeZone)
    {
      this.ScheduledTime = scheduledTime;
      this.Interval = interval;
      this.TimeZone = timeZone;
    }

    public DateTime ScheduledTime
    {
      get => this.m_scheduledTime;
      set
      {
        if (value.Kind == DateTimeKind.Unspecified)
          value = DateTime.SpecifyKind(value, DateTimeKind.Local);
        this.m_scheduledTime = value;
      }
    }

    public int Interval
    {
      get => this.m_interval;
      set => this.m_interval = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof (value));
    }

    public TimeZoneInfo TimeZone
    {
      get => this.m_timeZone;
      set
      {
        this.m_timeZone = value != null ? value : throw new ArgumentNullException(nameof (value));
        this.m_timeZoneId = value.Id;
      }
    }

    private void AfterDeserialize()
    {
      try
      {
        this.m_timeZone = TimeZoneInfo.FindSystemTimeZoneById(this.m_timeZoneId);
      }
      catch (Exception ex)
      {
        this.m_timeZone = TimeZoneInfo.Utc;
      }
    }

    private TeamFoundationJobSchedule()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TeamFoundationJobSchedule FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      TeamFoundationJobSchedule foundationJobSchedule = new TeamFoundationJobSchedule();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "Interval":
              foundationJobSchedule.m_interval = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "priorityLevel":
              foundationJobSchedule.m_priorityLevel = XmlUtility.Int32FromXmlAttribute(reader);
              continue;
            case "ScheduledTime":
              foundationJobSchedule.m_scheduledTime = XmlUtility.DateTimeFromXmlAttribute(reader);
              continue;
            case "TimeZoneId":
              foundationJobSchedule.m_timeZoneId = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      foundationJobSchedule.AfterDeserialize();
      return foundationJobSchedule;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("TeamFoundationJobSchedule instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  Interval: " + this.m_interval.ToString());
      stringBuilder.AppendLine("  PriorityLevel: " + this.m_priorityLevel.ToString());
      stringBuilder.AppendLine("  ScheduledTime: " + this.m_scheduledTime.ToString());
      stringBuilder.AppendLine("  TimeZoneId: " + this.m_timeZoneId);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_interval != 0)
        XmlUtility.ToXmlAttribute(writer, "Interval", this.m_interval);
      if (this.m_priorityLevel != 0)
        XmlUtility.ToXmlAttribute(writer, "priorityLevel", this.m_priorityLevel);
      if (this.m_scheduledTime != DateTime.MinValue)
        XmlUtility.ToXmlAttribute(writer, "ScheduledTime", this.m_scheduledTime);
      if (this.m_timeZoneId != null)
        XmlUtility.ToXmlAttribute(writer, "TimeZoneId", this.m_timeZoneId);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, TeamFoundationJobSchedule obj) => obj.ToXml(writer, element);
  }
}
