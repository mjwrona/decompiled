// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.FixedIntervalDateRange
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class FixedIntervalDateRange : 
    DateRangeBase,
    IFixedIntervalDateRange,
    IDateRange,
    IHistoryRange
  {
    public override IEnumerable<DateTime> InstantiateFrom(DateTime anchorSample)
    {
      TimeSpan timeSpan1 = TimeSpan.FromTicks(this.Interval.Ticks * (long) (this.SampleCount - 1));
      DateTime dateTime1 = anchorSample.Add(timeSpan1);
      TimeSpan timeSpan2 = this.Interval;
      DateTime sampleDate;
      if (timeSpan2.CompareTo(TimeSpan.Zero) > 0)
      {
        sampleDate = anchorSample;
      }
      else
      {
        sampleDate = dateTime1;
        timeSpan2 = timeSpan2.Negate();
      }
      List<DateTime> dateTimeList = new List<DateTime>();
      DateTime dateTime2 = DateRangeBase.SnapToPreMidnight(sampleDate);
      for (int index = 0; index < this.SampleCount; ++index)
      {
        dateTimeList.Add(dateTime2);
        dateTime2 = dateTime2.Add(timeSpan2);
      }
      return (IEnumerable<DateTime>) dateTimeList;
    }

    public TimeSpan Interval { get; set; }
  }
}
