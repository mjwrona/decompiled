// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.WeeklyDateRange
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class WeeklyDateRange : FixedIntervalDateRange
  {
    private const int c_DaysInWeek = 7;

    public override IEnumerable<DateTime> InstantiateFrom(DateTime anchorSample)
    {
      anchorSample = DateRangeBase.SnapToPreMidnight(anchorSample);
      int num1 = (int) (anchorSample.DayOfWeek - 0);
      int num2 = (num1 > 0 ? num1 : 0) + 7 * (this.SampleCount - 1);
      DateTime dateTime = anchorSample - TimeSpan.FromDays((double) num2);
      List<DateTime> dateTimeList = new List<DateTime>();
      for (int index = 0; index < this.SampleCount; ++index)
      {
        dateTimeList.Add(dateTime);
        dateTime = dateTime.AddDays(7.0);
      }
      dateTimeList.Add(anchorSample);
      return (IEnumerable<DateTime>) dateTimeList;
    }
  }
}
