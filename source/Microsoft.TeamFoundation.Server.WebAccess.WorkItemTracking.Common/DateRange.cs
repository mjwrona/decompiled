// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DateRange
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class DateRange
  {
    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DateRange_StringFormat, (object) this.Start, (object) this.End);

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("Start", (object) this.Start);
      json.Add("End", (object) this.End);
      return json;
    }

    public void Validate() => ArgumentUtility.CheckForDateTimeRange(this.End, "End", this.Start, DateTime.MaxValue);

    public void ValidateForSql()
    {
      this.Validate();
      ArgumentUtility.CheckForDateTimeRange(this.Start, "Start", (DateTime) SqlDateTime.MinValue, (DateTime) SqlDateTime.MaxValue);
      ArgumentUtility.CheckForDateTimeRange(this.End, "End", (DateTime) SqlDateTime.MinValue, (DateTime) SqlDateTime.MaxValue);
    }

    public bool Overlaps(DateRange otherRange)
    {
      ArgumentUtility.CheckForNull<DateRange>(otherRange, nameof (otherRange));
      return (this.Start > otherRange.End ? 1 : (otherRange.Start > this.End ? 1 : 0)) == 0;
    }

    public static void CheckForOverlaps(ICollection<DateRange> dateRanges, string paramName)
    {
      ArgumentUtility.CheckForNull<ICollection<DateRange>>(dateRanges, nameof (dateRanges));
      ArgumentUtility.CheckForNull<string>(paramName, nameof (paramName));
      int count = 1;
      foreach (DateRange dateRange1 in (IEnumerable<DateRange>) dateRanges)
      {
        foreach (DateRange dateRange2 in dateRanges.Skip<DateRange>(count))
        {
          if (dateRange1.Overlaps(dateRange2))
            throw new OverlappingDateRangesException(dateRange1, dateRange2, paramName);
        }
        ++count;
      }
    }
  }
}
