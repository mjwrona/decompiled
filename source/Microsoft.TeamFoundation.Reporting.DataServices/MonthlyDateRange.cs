// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.MonthlyDateRange
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class MonthlyDateRange : FixedIntervalDateRange
  {
    public override IEnumerable<DateTime> InstantiateFrom(DateTime currentSample)
    {
      this.SampleCount = this.SampleCount;
      List<DateTime> dateTimeList = new List<DateTime>();
      int year = currentSample.Year;
      int month = currentSample.Month;
      DateTime preMidnight1 = DateRangeBase.SnapToPreMidnight(currentSample);
      if (preMidnight1.Day != 1)
        dateTimeList.Add(preMidnight1);
      for (int index = 0; index < this.SampleCount; ++index)
      {
        DateTime preMidnight2 = DateRangeBase.SnapToPreMidnight(new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Unspecified));
        dateTimeList.Add(preMidnight2);
        --month;
        if (month == 0)
        {
          --year;
          month = 12;
        }
      }
      dateTimeList.Reverse();
      return (IEnumerable<DateTime>) dateTimeList;
    }
  }
}
