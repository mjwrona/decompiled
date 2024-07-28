// Decompiled with JetBrains decompiler
// Type: WebGrease.TimeMeasure
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebGrease.Extensions;

namespace WebGrease
{
  public class TimeMeasure : ITimeMeasure
  {
    private readonly List<IDictionary<string, int>> measurementCounts = new List<IDictionary<string, int>>()
    {
      (IDictionary<string, int>) new Dictionary<string, int>()
    };
    private readonly List<IDictionary<string, double>> measurements = new List<IDictionary<string, double>>()
    {
      (IDictionary<string, double>) new Dictionary<string, double>()
    };
    private readonly IList<TimeMeasureItem> timers = (IList<TimeMeasureItem>) new List<TimeMeasureItem>();

    public TimeMeasureResult[] GetResults() => this.measurements.Last<IDictionary<string, double>>().OrderByDescending<KeyValuePair<string, double>, double>((Func<KeyValuePair<string, double>, double>) (m => m.Value)).Select<KeyValuePair<string, double>, TimeMeasureResult>((Func<KeyValuePair<string, double>, TimeMeasureResult>) (m => new TimeMeasureResult()
    {
      IdParts = WebGreaseContext.ToIdParts(m.Key),
      Duration = m.Value,
      Count = this.measurementCounts.Last<IDictionary<string, int>>()[m.Key]
    })).ToArray<TimeMeasureResult>();

    public void Start(bool isGroup, params string[] idParts)
    {
      string id = WebGreaseContext.ToStringId((IEnumerable<string>) idParts);
      if (this.timers.Any<TimeMeasureItem>((Func<TimeMeasureItem, bool>) (t => t.Id.Equals(id))))
        throw new BuildWorkflowException("An error occurred while starting timer for {0}, probably a wrong start/end for key: ".InvariantFormat((object) id));
      this.PauseLastTimer();
      this.timers.Add(new TimeMeasureItem(id, DateTime.Now));
      if (!isGroup)
        return;
      this.BeginGroup();
    }

    public void End(bool isGroup, params string[] idParts)
    {
      if (isGroup)
        this.EndGroup();
      string stringId = WebGreaseContext.ToStringId((IEnumerable<string>) idParts);
      TimeMeasureItem timer = this.timers.Last<TimeMeasureItem>();
      if (timer.Id != stringId)
        throw new BuildWorkflowException("Trying to end a timer that was not started.");
      this.StopTimer(timer);
      this.ResumeLastTimer();
    }

    public void BeginGroup()
    {
      this.measurementCounts.Add((IDictionary<string, int>) new Dictionary<string, int>());
      this.measurements.Add((IDictionary<string, double>) new Dictionary<string, double>());
    }

    public void EndGroup()
    {
      IDictionary<string, int> dictionary2_1 = this.measurementCounts.Count<IDictionary<string, int>>() != 1 ? this.measurementCounts.Last<IDictionary<string, int>>() : throw new BuildWorkflowException("No measure sections available to end.");
      IDictionary<string, double> dictionary2_2 = this.measurements.Last<IDictionary<string, double>>();
      this.measurementCounts.RemoveAt(this.measurementCounts.Count<IDictionary<string, int>>() - 1);
      this.measurements.RemoveAt(this.measurements.Count<IDictionary<string, double>>() - 1);
      this.measurementCounts.Last<IDictionary<string, int>>().Add<string>((IEnumerable<KeyValuePair<string, int>>) dictionary2_1);
      this.measurements.Last<IDictionary<string, double>>().Add<string>((IEnumerable<KeyValuePair<string, double>>) dictionary2_2);
    }

    public void WriteResults(
      string filePathWithoutExtension,
      string title,
      DateTimeOffset utcStart)
    {
      TimeMeasureResult[] results = this.GetResults();
      File.WriteAllText(filePathWithoutExtension + ".measure.txt", TimeMeasure.GetMeasureTable(title, (IEnumerable<TimeMeasureResult>) results) + "\r\nTotal seconds: {0}".InvariantFormat((object) (DateTimeOffset.Now - utcStart).TotalSeconds));
      File.WriteAllText(filePathWithoutExtension + ".measure.csv", ((IEnumerable<TimeMeasureResult>) results).GetCsv());
    }

    internal static void WriteResults(
      string filePathWithoutExtension,
      IEnumerable<Tuple<string, bool, IEnumerable<TimeMeasureResult>>> results,
      string title,
      DateTimeOffset startTime,
      string activityName)
    {
      DateTimeOffset now = DateTimeOffset.Now;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("Configuration file: {0}", (object) title);
      stringBuilder.AppendLine();
      stringBuilder.AppendFormat("Activity: {0}", (object) activityName);
      stringBuilder.AppendLine();
      stringBuilder.AppendFormat("Started at: {0:yy-MM-dd HH:mm:ss.fff}", (object) startTime);
      stringBuilder.AppendLine();
      stringBuilder.AppendFormat("Ended at: {0:yy-MM-dd HH:mm:ss.fff}", (object) now);
      stringBuilder.AppendLine();
      stringBuilder.AppendFormat("Total Seconds: {0}", (object) (now - startTime).TotalSeconds);
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      foreach (Tuple<string, bool, IEnumerable<TimeMeasureResult>> tuple in (IEnumerable<Tuple<string, bool, IEnumerable<TimeMeasureResult>>>) results.OrderBy<Tuple<string, bool, IEnumerable<TimeMeasureResult>>, bool>((Func<Tuple<string, bool, IEnumerable<TimeMeasureResult>>, bool>) (r => r.Item2)).ThenByDescending<Tuple<string, bool, IEnumerable<TimeMeasureResult>>, double>((Func<Tuple<string, bool, IEnumerable<TimeMeasureResult>>, double>) (r => r.Item3.Sum<TimeMeasureResult>((Func<TimeMeasureResult, double>) (v => v.Duration)))))
      {
        string str = tuple.Item1;
        IEnumerable<TimeMeasureResult> results1 = tuple.Item3;
        stringBuilder.AppendLine(results1.GetTextTable(str + " - Details"));
      }
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      foreach (Tuple<string, bool, IEnumerable<TimeMeasureResult>> tuple in (IEnumerable<Tuple<string, bool, IEnumerable<TimeMeasureResult>>>) results.Where<Tuple<string, bool, IEnumerable<TimeMeasureResult>>>((Func<Tuple<string, bool, IEnumerable<TimeMeasureResult>>, bool>) (r => !r.Item2)).OrderByDescending<Tuple<string, bool, IEnumerable<TimeMeasureResult>>, double>((Func<Tuple<string, bool, IEnumerable<TimeMeasureResult>>, double>) (r => r.Item3.Sum<TimeMeasureResult>((Func<TimeMeasureResult, double>) (v => v.Duration)))))
      {
        string str = tuple.Item1;
        IEnumerable<TimeMeasureResult> resultsToAdd = tuple.Item3;
        stringBuilder.AppendLine(resultsToAdd.Group((Func<TimeMeasureResult, string>) (tm => tm.IdParts.FirstOrDefault<string>())).GetTextTable(str + " - Summary"));
      }
      File.WriteAllText("{0}.{1}.measure.txt".InvariantFormat((object) filePathWithoutExtension, (object) activityName), stringBuilder.ToString());
      foreach (Tuple<string, bool, IEnumerable<TimeMeasureResult>> result in results)
        File.WriteAllText("{0}.{1}.{2}.measure.csv".InvariantFormat((object) filePathWithoutExtension, (object) activityName, (object) result.Item1), result.Item3.GetCsv());
    }

    private static string GetMeasureTable(string title, IEnumerable<TimeMeasureResult> measureTotal) => "{0}\r\n\r\n{1}\r\n\r\nStarted at: {2:yy-MM-dd HH:mm:ss.fff}".InvariantFormat((object) measureTotal.GetTextTable(title), (object) measureTotal.Group((Func<TimeMeasureResult, string>) (tm => tm.IdParts.FirstOrDefault<string>())).GetTextTable(title), (object) DateTime.Now);

    private void AddToResult(TimeMeasureItem timer)
    {
      string id = timer.Id;
      if (!this.measurementCounts.Last<IDictionary<string, int>>().ContainsKey(id))
        this.measurementCounts.Last<IDictionary<string, int>>().Add(id, 0);
      IDictionary<string, int> dictionary1;
      string key1;
      (dictionary1 = this.measurementCounts.Last<IDictionary<string, int>>())[key1 = id] = dictionary1[key1] + 1;
      if (!this.measurements.Last<IDictionary<string, double>>().ContainsKey(id))
        this.measurements.Last<IDictionary<string, double>>().Add(id, 0.0);
      IDictionary<string, double> dictionary2;
      string key2;
      double num = (dictionary2 = this.measurements.Last<IDictionary<string, double>>())[key2 = id] + (DateTime.Now - timer.Value).TotalMilliseconds;
      dictionary2[key2] = num;
    }

    private void PauseLastTimer()
    {
      if (!this.timers.Any<TimeMeasureItem>())
        return;
      this.AddToResult(this.timers.Last<TimeMeasureItem>());
    }

    private void ResumeLastTimer()
    {
      if (!this.timers.Any<TimeMeasureItem>())
        return;
      this.timers.Last<TimeMeasureItem>().Value = DateTime.Now;
    }

    private void StopTimer(TimeMeasureItem timer)
    {
      this.timers.Remove(timer);
      this.AddToResult(timer);
    }
  }
}
