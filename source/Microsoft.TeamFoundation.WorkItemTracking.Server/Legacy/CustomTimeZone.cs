// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Legacy.CustomTimeZone
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Legacy
{
  public class CustomTimeZone : TimeZone
  {
    private TimeZoneInfo m_timeZoneInfo;

    public CustomTimeZone(TimeZoneInfo timeZoneInfo)
    {
      ArgumentUtility.CheckForNull<TimeZoneInfo>(timeZoneInfo, nameof (timeZoneInfo));
      this.m_timeZoneInfo = timeZoneInfo;
    }

    public TimeZoneInfo Info => this.m_timeZoneInfo;

    public override string DaylightName => this.m_timeZoneInfo.DaylightName;

    public override string StandardName => this.m_timeZoneInfo.StandardName;

    public override bool IsDaylightSavingTime(DateTime time) => this.m_timeZoneInfo.IsDaylightSavingTime(time);

    public override DateTime ToLocalTime(DateTime time) => time.Kind == DateTimeKind.Local ? time : TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(time, DateTimeKind.Unspecified), TimeZoneInfo.Utc, this.m_timeZoneInfo);

    public override DateTime ToUniversalTime(DateTime time) => time.Kind == DateTimeKind.Utc ? time : TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(time, DateTimeKind.Unspecified), this.m_timeZoneInfo, TimeZoneInfo.Utc);

    public override DaylightTime GetDaylightChanges(int year) => throw new NotSupportedException();

    public override TimeSpan GetUtcOffset(DateTime time) => this.m_timeZoneInfo.GetUtcOffset(time);
  }
}
