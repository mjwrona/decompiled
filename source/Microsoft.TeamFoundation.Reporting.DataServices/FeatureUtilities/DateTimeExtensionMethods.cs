// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities.DateTimeExtensionMethods
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System;

namespace Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities
{
  public static class DateTimeExtensionMethods
  {
    public static DateTime FromUtc(this DateTime originalDate, TimeZoneInfo timeZone)
    {
      timeZone = timeZone ?? TimeZoneInfo.Local;
      return TimeZoneInfo.ConvertTime(originalDate, TimeZoneInfo.Utc, timeZone);
    }

    public static DateTime ToUtc(this DateTime originalDate, TimeZoneInfo timeZone)
    {
      timeZone = timeZone ?? TimeZoneInfo.Local;
      return TimeZoneInfo.ConvertTime(originalDate, timeZone, TimeZoneInfo.Utc);
    }
  }
}
