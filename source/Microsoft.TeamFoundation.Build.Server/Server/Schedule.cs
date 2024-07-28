// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Schedule
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class Schedule : IValidatable
  {
    private bool hasTimeBeenAdjusted;
    private const int oneDayInSeconds = 86400;

    public Schedule() => this.hasTimeBeenAdjusted = false;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string TimeZoneId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int UtcStartTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public ScheduleDays UtcDaysToBuild { get; set; }

    internal string DefinitionUri { get; set; }

    internal Guid ProjectId { get; set; }

    internal DateTime LastModifiedUtc { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.Check("TimeZoneId", (object) this.TimeZoneId, false);
      try
      {
        TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId);
      }
      catch (InvalidTimeZoneException ex)
      {
        throw new ArgumentException(ex.Message, (Exception) ex);
      }
      catch (TimeZoneNotFoundException ex)
      {
        throw new ArgumentException(ex.Message, (Exception) ex);
      }
      ArgumentValidation.CheckBound("UtcStartTime", this.UtcStartTime, 0, 86399);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[Schedule DefinitionUri={0} TimeZoneId={1} UtcStartTime={2} UtcDaysToBuild={3}]", (object) this.DefinitionUri, (object) this.TimeZoneId, (object) this.UtcStartTime, (object) this.UtcDaysToBuild);

    public void AdjustUtcStartTimeForDST(DateTime now)
    {
      if (this.hasTimeBeenAdjusted || string.IsNullOrWhiteSpace(this.TimeZoneId) || this.LastModifiedUtc == DateTime.MinValue)
        return;
      TimeZoneInfo systemTimeZoneById = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId);
      if (!systemTimeZoneById.SupportsDaylightSavingTime)
      {
        this.hasTimeBeenAdjusted = true;
      }
      else
      {
        int totalDays = (int) (now - this.LastModifiedUtc).TotalDays;
        DateTime dateTime1 = this.LastModifiedUtc.AddDays((double) totalDays);
        if (systemTimeZoneById.IsDaylightSavingTime(now) != systemTimeZoneById.IsDaylightSavingTime(dateTime1))
        {
          dateTime1 = dateTime1.AddDays(2.0);
          totalDays += 2;
        }
        DateTime dateTime2 = TimeZoneInfo.ConvertTimeFromUtc(this.LastModifiedUtc, systemTimeZoneById);
        DateTime dateTime3 = TimeZoneInfo.ConvertTimeFromUtc(dateTime1, systemTimeZoneById);
        this.UtcStartTime += (int) (dateTime2.AddDays((double) totalDays) - dateTime3).TotalSeconds;
        if (this.UtcStartTime < 0)
        {
          this.UtcStartTime += 86400;
          this.UtcDaysToBuild = this.ShiftScheduleDays(this.UtcDaysToBuild, -1);
        }
        else if (this.UtcStartTime >= 86400)
        {
          this.UtcStartTime -= 86400;
          this.UtcDaysToBuild = this.ShiftScheduleDays(this.UtcDaysToBuild, 1);
        }
        this.hasTimeBeenAdjusted = true;
      }
    }

    internal ScheduleDays ShiftScheduleDays(ScheduleDays days, int offset)
    {
      offset %= 7;
      if (offset == 0)
        return days;
      int num1 = 1;
      int num2 = 64;
      int maxValue = (int) sbyte.MaxValue;
      int num3 = (int) days;
      if (offset < 0)
      {
        for (; offset < 0; ++offset)
        {
          int num4 = (num3 & num1) == num1 ? num2 : 0;
          num3 = num3 >> 1 | num4;
        }
      }
      else
      {
        for (; offset > 0; --offset)
        {
          int num5 = (num3 & num2) == num2 ? num1 : 0;
          num3 = (num3 << 1 | num5) & maxValue;
        }
      }
      return (ScheduleDays) num3;
    }
  }
}
