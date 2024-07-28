// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobSchedule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  public class TeamFoundationJobSchedule : IComparable<TeamFoundationJobSchedule>
  {
    internal static readonly int MaxScheduleInterval = 34560000;
    internal static readonly DateTime MinScheduleTime = new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private const int c_24hours = 86400;
    private const int c_maxScheduleIntervalDays = 400;

    public TeamFoundationJobSchedule() => this.PriorityLevel = TeamFoundationJobSchedule.DefaultPriorityLevel;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int Interval { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public DateTime ScheduledTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string TimeZoneId { get; set; }

    internal Guid JobSource { get; set; }

    internal Guid JobId { get; set; }

    [XmlIgnore]
    public JobPriorityLevel PriorityLevel
    {
      get => (JobPriorityLevel) this.PriorityLevelValue;
      set => this.PriorityLevelValue = (int) value;
    }

    [XmlAttribute("priorityLevel")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "PriorityLevel")]
    public int PriorityLevelValue { get; set; }

    public TeamFoundationJobSchedule Clone() => (TeamFoundationJobSchedule) this.MemberwiseClone();

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Interval {0} ScheduledTime {1} TimeZoneId {2} JobId {3}", (object) this.Interval, (object) this.ScheduledTime, (object) this.TimeZoneId, (object) this.JobId);

    internal void Validate(
      IVssRequestContext requestContext,
      string topLevelParamName,
      int minimumScheduleInterval)
    {
      PropertyValidation.CheckRange<DateTime>(TeamFoundationJobSchedule.GetUtcDateTime(this.ScheduledTime), TeamFoundationJobSchedule.MinScheduleTime, this.GetMaxScheduleTime(requestContext), "ScheduledTime", this.GetType(), topLevelParamName);
      PropertyValidation.CheckPropertyLength(this.TimeZoneId, true, 0, 32, "TimeZoneId", this.GetType(), topLevelParamName);
      if (this.Interval != 0 && (this.Interval < minimumScheduleInterval || this.Interval > TeamFoundationJobSchedule.MaxScheduleInterval))
        throw new ArgumentOutOfRangeException(topLevelParamName, FrameworkResources.JobIntervalOutOfRange((object) this.Interval, (object) minimumScheduleInterval, (object) TeamFoundationJobSchedule.MaxScheduleInterval));
      if (string.IsNullOrEmpty(this.TimeZoneId))
      {
        this.TimeZoneId = TimeZoneInfo.Utc.Id;
      }
      else
      {
        if (!(this.TimeZoneId != TimeZoneInfo.Utc.Id))
          return;
        try
        {
          TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId);
          if (this.Interval % 86400 != 0)
            throw new JobIntervalNotSupportedException(FrameworkResources.TimezoneIntervalNotSupportedError((object) this.TimeZoneId, (object) this.Interval, (object) TimeZoneInfo.Utc.Id));
        }
        catch (InvalidTimeZoneException ex)
        {
          throw new ArgumentException(FrameworkResources.ArgumentPropertyIsInvalid((object) "TimeZoneId", (object) ex.Message), topLevelParamName, (Exception) ex);
        }
        catch (TimeZoneNotFoundException ex)
        {
          throw new ArgumentException(FrameworkResources.ArgumentPropertyIsInvalid((object) "TimeZoneId", (object) ex.Message), topLevelParamName, (Exception) ex);
        }
      }
    }

    internal DateTime GetMaxScheduleTime(IVssRequestContext requestContext)
    {
      int defaultValue = !requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? 400 : 0;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.MaxJobScheduleOffsetDays, true, defaultValue);
      return num <= 0 ? DateTime.MaxValue : DateTime.UtcNow.AddDays((double) num);
    }

    internal static DateTime GetUtcDateTime(DateTime dateTime) => !(dateTime == DateTime.MinValue) && !(dateTime == DateTime.MaxValue) ? dateTime.ToUniversalTime() : dateTime;

    internal static JobPriorityLevel DefaultPriorityLevel => JobPriorityLevel.BelowNormal;

    public int CompareTo(TeamFoundationJobSchedule other)
    {
      int num = this.JobId.CompareTo(other.JobId) * 4 + this.ScheduledTime.CompareTo(other.ScheduledTime) * 2 + this.Interval.CompareTo(other.Interval);
      if (num == 0)
        return 0;
      return num <= 0 ? -1 : 1;
    }
  }
}
