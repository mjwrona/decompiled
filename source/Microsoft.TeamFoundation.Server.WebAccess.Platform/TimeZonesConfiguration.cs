// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TimeZonesConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class TimeZonesConfiguration : WebSdkMetadata
  {
    private WebContext m_webContext;
    private static Dictionary<string, List<DaylightSavingsAdjustmentEntry>> s_timezonesMap = new Dictionary<string, List<DaylightSavingsAdjustmentEntry>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    static TimeZonesConfiguration()
    {
      foreach (TimeZoneInfo timeZoneInfo in TimeZoneInfo.GetSystemTimeZones().Where<TimeZoneInfo>((Func<TimeZoneInfo, bool>) (timeZone => timeZone.SupportsDaylightSavingTime)))
      {
        List<DaylightSavingsAdjustmentEntry> adjustmentEntries = TimeZonesConfiguration.GetDaylightSavingsAdjustmentEntries(timeZoneInfo);
        TimeZonesConfiguration.s_timezonesMap[timeZoneInfo.Id] = adjustmentEntries;
      }
    }

    public TimeZonesConfiguration(WebContext webContext) => this.m_webContext = webContext;

    [DataMember(EmitDefaultValue = false)]
    public List<DaylightSavingsAdjustmentEntry> DaylightSavingsAdjustments
    {
      get
      {
        List<DaylightSavingsAdjustmentEntry> savingsAdjustments = (List<DaylightSavingsAdjustmentEntry>) null;
        TimeZoneInfo userTimeZone = this.m_webContext.Globalization.UserTimeZone;
        if (userTimeZone != null && userTimeZone.SupportsDaylightSavingTime)
          TimeZonesConfiguration.s_timezonesMap.TryGetValue(userTimeZone.Id, out savingsAdjustments);
        return savingsAdjustments;
      }
    }

    private static List<DaylightSavingsAdjustmentEntry> GetDaylightSavingsAdjustmentEntries(
      TimeZoneInfo timeZoneInfo)
    {
      DateTime universalTime = DateTime.Today.ToUniversalTime();
      DateTime dateTime1 = universalTime.AddYears(10);
      DateTime dateTime2 = universalTime.AddYears(-10);
      List<DaylightSavingsAdjustmentEntry> adjustmentEntries = new List<DaylightSavingsAdjustmentEntry>(42);
      double totalMilliseconds = timeZoneInfo.GetUtcOffset(dateTime2).TotalMilliseconds;
      adjustmentEntries.Add(new DaylightSavingsAdjustmentEntry()
      {
        Start = dateTime2,
        Offset = (int) totalMilliseconds
      });
      DateTime dateTime3;
      for (; dateTime2 < dateTime1; dateTime2 = dateTime3)
      {
        int num = 1024;
        if (timeZoneInfo.Id.Equals("Samoa Standard Time") && dateTime2.Year <= 2009 && dateTime2.Year >= 2008)
          num = 24;
        dateTime3 = dateTime2.AddHours((double) num);
        TimeSpan timeSpan = timeZoneInfo.GetUtcOffset(dateTime3);
        if (timeSpan.TotalMilliseconds != totalMilliseconds)
        {
          timeSpan = dateTime3 - dateTime2;
          for (int index = (int) (timeSpan.TotalHours + 1.0) / 2; index > 0; index = (index + 1) / 2)
          {
            dateTime3 = dateTime2.AddHours((double) index);
            timeSpan = timeZoneInfo.GetUtcOffset(dateTime3);
            if (timeSpan.TotalMilliseconds == totalMilliseconds)
              dateTime2 = dateTime3;
            else if (index == 1)
              break;
          }
          if (!(dateTime2 > dateTime1))
          {
            DateTime dateTime4 = dateTime2.AddHours(1.0);
            timeSpan = timeZoneInfo.GetUtcOffset(dateTime4);
            totalMilliseconds = timeSpan.TotalMilliseconds;
            adjustmentEntries.Add(new DaylightSavingsAdjustmentEntry()
            {
              Start = dateTime4,
              Offset = (int) totalMilliseconds
            });
          }
          else
            break;
        }
      }
      return adjustmentEntries;
    }
  }
}
