// Decompiled with JetBrains decompiler
// Type: WebGrease.TimeMeasureExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebGrease.Extensions;

namespace WebGrease
{
  public static class TimeMeasureExtensions
  {
    private static readonly object[] HeaderValues = new object[5]
    {
      (object) "Type",
      (object) "Duration (ms)",
      (object) "%",
      (object) "#",
      (object) "ms/#"
    };

    public static string GetCsv(this IEnumerable<TimeMeasureResult> results)
    {
      TimeMeasureResult[] array = results.OrderByDescending<TimeMeasureResult, double>((Func<TimeMeasureResult, double>) (r => r.Duration)).ToArray<TimeMeasureResult>();
      double totalTime = ((IEnumerable<TimeMeasureResult>) array).Sum<TimeMeasureResult>((Func<TimeMeasureResult, double>) (r => r.Duration));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(TimeMeasureExtensions.GetCsvRow(TimeMeasureExtensions.HeaderValues));
      foreach (TimeMeasureResult measureResult in array)
        stringBuilder.AppendLine(TimeMeasureExtensions.GetCsvRow(TimeMeasureExtensions.GetValues(measureResult, totalTime)));
      return stringBuilder.ToString();
    }

    public static string GetTextTable(this IEnumerable<TimeMeasureResult> results, string title)
    {
      TimeMeasureResult[] array = results.OrderByDescending<TimeMeasureResult, double>((Func<TimeMeasureResult, double>) (r => r.Duration)).ToArray<TimeMeasureResult>();
      StringBuilder stringBuilder = new StringBuilder();
      double totalTime = ((IEnumerable<TimeMeasureResult>) array).Sum<TimeMeasureResult>((Func<TimeMeasureResult, double>) (r => r.Duration));
      stringBuilder.AppendLine("/=======================================================================================");
      stringBuilder.AppendLine("| " + title);
      stringBuilder.AppendLine("|--------------------------------------------------------------------------------------");
      stringBuilder.AppendLine("| {1,14} | {2,7} | {3,6} | {4,7} | {0}".InvariantFormat(TimeMeasureExtensions.HeaderValues));
      stringBuilder.AppendLine("|--------------------------------------------------------------------------------------");
      foreach (TimeMeasureResult measureResult in array)
        stringBuilder.AppendLine("| {1,14:N0} | {2,7:P1} | {3,6} | {4,7:N0} | {0}".InvariantFormat(TimeMeasureExtensions.GetValues(measureResult, totalTime)));
      stringBuilder.AppendLine("|--------------------------------------------------------------------------------------");
      stringBuilder.AppendLine("| {1,14:N0} | {2,7:P1} | {3,6} | {4,7} | {0}".InvariantFormat((object) "Total", (object) totalTime, (object) 1, (object) string.Empty, (object) string.Empty));
      stringBuilder.AppendLine("\\______________________________________________________________________________________");
      return stringBuilder.ToString();
    }

    public static IEnumerable<TimeMeasureResult> Group(
      this IEnumerable<TimeMeasureResult> resultsToAdd,
      Func<TimeMeasureResult, string> groupSelector)
    {
      return (IEnumerable<TimeMeasureResult>) resultsToAdd.GroupBy<TimeMeasureResult, string>(groupSelector).Select<IGrouping<string, TimeMeasureResult>, TimeMeasureResult>((Func<IGrouping<string, TimeMeasureResult>, TimeMeasureResult>) (s => new TimeMeasureResult()
      {
        IdParts = WebGreaseContext.ToIdParts(s.Key),
        Count = s.Min<TimeMeasureResult>((Func<TimeMeasureResult, int>) (m => m.Count)),
        Duration = s.Sum<TimeMeasureResult>((Func<TimeMeasureResult, double>) (m => m.Duration))
      })).OrderByDescending<TimeMeasureResult, double>((Func<TimeMeasureResult, double>) (r => r.Duration)).ToArray<TimeMeasureResult>();
    }

    private static string GetCsvRow(object[] values) => "\"" + string.Join("\",\"", values) + "\"";

    private static object[] GetValues(TimeMeasureResult measureResult, double totalTime) => new object[5]
    {
      (object) measureResult.Name,
      (object) Math.Round(measureResult.Duration),
      (object) (measureResult.Duration / totalTime),
      (object) measureResult.Count,
      (object) (measureResult.Duration / (double) measureResult.Count)
    };
  }
}
