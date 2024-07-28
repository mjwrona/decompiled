// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DateRangeBase
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public abstract class DateRangeBase : HistoryRange, IDateRange, IHistoryRange
  {
    public abstract IEnumerable<DateTime> InstantiateFrom(DateTime anchorSample);

    public static DateTime SnapToPreMidnight(DateTime sampleDate)
    {
      TimeSpan timeSpan = new TimeSpan(0, 23, 59, 0, 0);
      return sampleDate.Date.Add(timeSpan);
    }

    public int SampleCount { get; set; }
  }
}
