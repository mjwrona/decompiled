// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.OperationProfiler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public class OperationProfiler : IOperationProfiler, IDisposable, ICounterLookup
  {
    private Task task;
    private bool m_stopped;
    private int m_unit;
    private ConcurrentDictionary<string, OperationProfiler.IBox> m_extBoxes;
    private ConcurrentDictionary<ProfilingGroup, ConcurrentDictionary<string, OperationProfiler.IBox>> m_boxesByGroup;
    private Stopwatch m_watch;
    private Action<TraceLevel, string> tracer;
    private const int c_count = 100;

    public event ProfilingResultsHandler OnCompletion;

    public long? GetBy(IProfilingCategory category, ProfilingResultType type)
    {
      ConcurrentDictionary<string, OperationProfiler.IBox> concurrentDictionary;
      OperationProfiler.IBox box;
      if (!this.m_boxesByGroup.TryGetValue(category.Group, out concurrentDictionary) || !concurrentDictionary.TryGetValue(category.Name, out box))
        return new long?();
      if (!(box is IProfilingResult profilingResult))
        return new long?(box.lval);
      long by = profilingResult.GetBy(type);
      return by < 0L ? new long?() : new long?(by);
    }

    public OperationProfiler(
      Action<TraceLevel, string> tracer,
      TimeSpan reportFrequency,
      bool autoReset = false,
      CancellationToken ctok = default (CancellationToken))
    {
      OperationProfiler lookup = this;
      this.tracer = tracer;
      this.m_unit = (int) (reportFrequency.TotalMilliseconds / 100.0);
      this.m_boxesByGroup = new ConcurrentDictionary<ProfilingGroup, ConcurrentDictionary<string, OperationProfiler.IBox>>();
      this.m_extBoxes = new ConcurrentDictionary<string, OperationProfiler.IBox>();
      this.m_watch = new Stopwatch();
      this.m_watch.Start();
      this.task = Task.Run((Func<Task>) (async () =>
      {
        while (!lookup.m_stopped && !ctok.IsCancellationRequested)
        {
          for (int cnt = 100; cnt > 0 && !lookup.m_stopped && !ctok.IsCancellationRequested; --cnt)
            await Task.Delay(lookup.m_unit);
          if (!lookup.m_stopped && !ctok.IsCancellationRequested)
          {
            List<string> values1 = new List<string>();
            foreach (KeyValuePair<string, OperationProfiler.IBox> extBox in lookup.m_extBoxes)
            {
              string key = extBox.Key;
              OperationProfiler.IBox box = extBox.Value;
              string str;
              try
              {
                str = key + " = " + box.lval.ToString();
              }
              catch (Exception ex)
              {
                str = key + " = (error)";
              }
              values1.Add(str);
            }
            values1.Sort();
            Dictionary<ProfilingGroup, List<string>> dictionary = new Dictionary<ProfilingGroup, List<string>>();
            foreach (KeyValuePair<ProfilingGroup, ConcurrentDictionary<string, OperationProfiler.IBox>> keyValuePair1 in lookup.m_boxesByGroup)
            {
              ProfilingGroup key1 = keyValuePair1.Key;
              List<string> stringList = new List<string>();
              List<string> collection = (List<string>) null;
              foreach (KeyValuePair<string, OperationProfiler.IBox> keyValuePair2 in keyValuePair1.Value)
              {
                string key2 = keyValuePair2.Key;
                OperationProfiler.IBox box = keyValuePair2.Value;
                try
                {
                  if (box is OperationProfiler.DerivedBox derivedBox2)
                  {
                    if (collection == null)
                      collection = new List<string>();
                    string str = key2 + " = " + derivedBox2.counter.GetValue((ICounterLookup) lookup);
                    collection.Add(str);
                  }
                  else
                  {
                    string str = key2 + " = " + box.lval.ToString();
                    if (box is OperationProfiler.AggBox aggBox2)
                    {
                      long laggval = aggBox2.laggval;
                      double totalSeconds = lookup.m_watch.Elapsed.TotalSeconds;
                      double num = totalSeconds != 0.0 ? Math.Floor((double) laggval / totalSeconds) : 0.0;
                      if (aggBox2.incremental)
                      {
                        str += string.Format("({0}/s)", (object) num);
                      }
                      else
                      {
                        long ltotalval = aggBox2.ltotalval;
                        str += string.Format("({0}, {1}/s)", (object) (ltotalval + laggval), (object) num);
                      }
                    }
                    stringList.Add(str);
                  }
                }
                catch (Exception ex)
                {
                  string str = key2 + " = (error)";
                }
              }
              stringList.Sort();
              if (collection != null)
              {
                collection.Sort();
                stringList.AddRange((IEnumerable<string>) collection);
              }
              dictionary.Add(key1, stringList);
            }
            if (autoReset)
            {
              // ISSUE: explicit non-virtual call
              __nonvirtual (lookup.Reset());
            }
            foreach (KeyValuePair<ProfilingGroup, List<string>> keyValuePair in dictionary)
            {
              ProfilingGroup key = keyValuePair.Key;
              List<string> values2 = keyValuePair.Value;
              tracer(TraceLevel.Info, "(profiling/" + key.ToString().ToLower() + ") " + string.Join("; ", (IEnumerable<string>) values2));
            }
            tracer(TraceLevel.Info, "(profiling/external) " + string.Join("; ", (IEnumerable<string>) values1));
          }
          else
            break;
        }
        if (!lookup.m_stopped)
          return;
        ProfilingResultsHandler onCompletion = lookup.OnCompletion;
        if (onCompletion == null)
          return;
        onCompletion(lookup.GetProfilingResult(lookup.m_boxesByGroup));
      })).ContinueWith((Action<Task>) (ante => lookup.m_watch.Stop()));
    }

    private ProfilingReport GetProfilingResult(
      ConcurrentDictionary<ProfilingGroup, ConcurrentDictionary<string, OperationProfiler.IBox>> boxes)
    {
      Dictionary<ProfilingGroup, List<Tuple<string, long>>> dict = new Dictionary<ProfilingGroup, List<Tuple<string, long>>>();
      foreach (KeyValuePair<ProfilingGroup, ConcurrentDictionary<string, OperationProfiler.IBox>> box in boxes)
      {
        ProfilingGroup key1 = box.Key;
        List<Tuple<string, long>> tupleList = new List<Tuple<string, long>>();
        foreach (KeyValuePair<string, OperationProfiler.IBox> keyValuePair in box.Value)
        {
          string key2 = keyValuePair.Key;
          if (!(keyValuePair.Value is OperationProfiler.DerivedBox))
          {
            long lval = keyValuePair.Value.lval;
            tupleList.Add(Tuple.Create<string, long>(key2, lval));
          }
        }
        dict.Add(key1, tupleList);
      }
      return new ProfilingReport(dict);
    }

    public virtual int TimeElapsedInSecond => (int) this.m_watch.Elapsed.TotalSeconds;

    public void Reset()
    {
      foreach (KeyValuePair<string, OperationProfiler.IBox> extBox in this.m_extBoxes)
      {
        if (extBox.Value is IResettable resettable)
          resettable.Reset();
      }
      foreach (KeyValuePair<ProfilingGroup, ConcurrentDictionary<string, OperationProfiler.IBox>> keyValuePair1 in this.m_boxesByGroup)
      {
        ConcurrentDictionary<string, OperationProfiler.IBox> concurrentDictionary = keyValuePair1.Value;
        foreach (KeyValuePair<string, OperationProfiler.IBox> keyValuePair2 in keyValuePair1.Value)
        {
          if (keyValuePair2.Value is IResettable resettable)
            resettable.Reset();
        }
      }
      lock (this)
        this.m_watch.Restart();
    }

    public IProfilingResult GetResult(string category)
    {
      IProfilingResult profilingResult = (IProfilingResult) null;
      OperationProfiler.IBox box;
      if (this.m_extBoxes.TryGetValue(category, out box))
        profilingResult = box as IProfilingResult;
      return profilingResult ?? (IProfilingResult) new NoOpProfilingResult();
    }

    public IProfilingResult GetResult(ProfilingCategory category)
    {
      IProfilingResult profilingResult = (IProfilingResult) null;
      if (category.Group == ProfilingGroup.External)
      {
        OperationProfiler.IBox box;
        if (this.m_extBoxes.TryGetValue(category.Name, out box))
          profilingResult = box as IProfilingResult;
      }
      else
      {
        ConcurrentDictionary<string, OperationProfiler.IBox> concurrentDictionary;
        OperationProfiler.IBox box;
        if (this.m_boxesByGroup.TryGetValue(category.Group, out concurrentDictionary) && concurrentDictionary.TryGetValue(category.Name, out box))
          profilingResult = box as IProfilingResult;
      }
      return profilingResult ?? (IProfilingResult) new NoOpProfilingResult();
    }

    private void AddCategory(
      ProfilingType type,
      string category,
      ConcurrentDictionary<string, OperationProfiler.IBox> box)
    {
      switch (type)
      {
        case ProfilingType.ManualCounter:
          box.GetOrAdd(category, (OperationProfiler.IBox) new OperationProfiler.ManualBox());
          break;
        case ProfilingType.AggregateCounter:
          box.GetOrAdd(category, (OperationProfiler.IBox) new OperationProfiler.AggBox(false));
          break;
        case ProfilingType.AccumulativeCounter:
          box.GetOrAdd(category, (OperationProfiler.IBox) new OperationProfiler.AggBox(true));
          break;
        case ProfilingType.AverageCounter:
          box.GetOrAdd(category, (OperationProfiler.IBox) new OperationProfiler.AvgBox());
          break;
        default:
          throw new ArgumentException(string.Format("Cannot add category with type = {0}.", (object) type));
      }
    }

    public void AddExternalCounter(ExternalCounter extCounter)
    {
      if (extCounter == null)
        return;
      this.m_extBoxes.GetOrAdd(extCounter.Name, (OperationProfiler.IBox) new OperationProfiler.ExtBox()
      {
        ExtCounter = extCounter
      });
    }

    public void AddDerivedCounter(DerivedCounter drvCounter)
    {
      if (drvCounter == null)
        return;
      this.m_boxesByGroup.GetOrAdd(drvCounter.Group, (Func<ProfilingGroup, ConcurrentDictionary<string, OperationProfiler.IBox>>) (grp => new ConcurrentDictionary<string, OperationProfiler.IBox>())).GetOrAdd(drvCounter.Name, (OperationProfiler.IBox) new OperationProfiler.DerivedBox(drvCounter));
    }

    public void AddCategory(ProfilingCategory category)
    {
      if (category.Group == ProfilingGroup.External)
      {
        this.AddCategory(category.Type, category.Name, this.m_extBoxes);
      }
      else
      {
        ConcurrentDictionary<string, OperationProfiler.IBox> orAdd = this.m_boxesByGroup.GetOrAdd(category.Group, (Func<ProfilingGroup, ConcurrentDictionary<string, OperationProfiler.IBox>>) (grp => new ConcurrentDictionary<string, OperationProfiler.IBox>()));
        this.AddCategory(category.Type, category.Name, orAdd);
      }
    }

    public void Increment(ProfilingCategory category)
    {
      ConcurrentDictionary<string, OperationProfiler.IBox> concurrentDictionary = (ConcurrentDictionary<string, OperationProfiler.IBox>) null;
      if (category.Group == ProfilingGroup.External)
        concurrentDictionary = this.m_extBoxes;
      else
        this.m_boxesByGroup.TryGetValue(category.Group, out concurrentDictionary);
      if (!(concurrentDictionary[category.Name] is OperationProfiler.ManualBox manualBox))
        return;
      Interlocked.Increment(ref manualBox.m_mval);
      if (!(manualBox is OperationProfiler.AggBox aggBox))
        return;
      Interlocked.Increment(ref aggBox.laggval);
    }

    public void Decrement(ProfilingCategory category)
    {
      ConcurrentDictionary<string, OperationProfiler.IBox> extBoxes;
      if (category.Group == ProfilingGroup.External)
        extBoxes = this.m_extBoxes;
      else
        this.m_boxesByGroup.TryGetValue(category.Group, out extBoxes);
      if (!(extBoxes[category.Name] is OperationProfiler.ManualBox manualBox))
        return;
      Interlocked.Decrement(ref manualBox.m_mval);
    }

    public void Update(ProfilingCategory category, long val)
    {
      ConcurrentDictionary<string, OperationProfiler.IBox> extBoxes;
      if (category.Group == ProfilingGroup.External)
        extBoxes = this.m_extBoxes;
      else
        this.m_boxesByGroup.TryGetValue(category.Group, out extBoxes);
      if (!(extBoxes[category.Name] is OperationProfiler.AvgBox avgBox))
        return;
      Interlocked.Increment(ref avgBox.lcount);
      Interlocked.Add(ref avgBox.ltotal, val);
    }

    public void Dispose()
    {
      if (!this.m_stopped)
        this.m_stopped = true;
      if (this.OnCompletion != null)
      {
        try
        {
          this.task.GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
          if (this.task.Exception != null)
            throw this.task.Exception;
          throw ex;
        }
      }
      this.Reset();
      foreach (KeyValuePair<string, OperationProfiler.IBox> extBox in this.m_extBoxes)
      {
        if (extBox.Value is IDisposable disposable)
          disposable.Dispose();
      }
      foreach (KeyValuePair<ProfilingGroup, ConcurrentDictionary<string, OperationProfiler.IBox>> keyValuePair1 in this.m_boxesByGroup)
      {
        foreach (KeyValuePair<string, OperationProfiler.IBox> keyValuePair2 in keyValuePair1.Value)
        {
          if (keyValuePair2.Value is IDisposable disposable)
            disposable.Dispose();
        }
      }
    }

    private interface IBox
    {
      long lval { get; }
    }

    private class ManualBox : OperationProfiler.IBox, IProfilingResult
    {
      internal long m_mval;

      public long lval
      {
        get => this.m_mval;
        set => this.m_mval = value;
      }

      public virtual long GetBy(ProfilingResultType type) => type == ProfilingResultType.Current ? this.lval : -1L;
    }

    private class AggBox : OperationProfiler.ManualBox, IResettable
    {
      public long laggval;
      public long ltotalval;
      public readonly bool incremental;

      internal AggBox(bool incremental) => this.incremental = incremental;

      public override long GetBy(ProfilingResultType type)
      {
        if (type == ProfilingResultType.Total)
          return this.ltotalval;
        return type == ProfilingResultType.AggregatedInTimeWindow ? this.laggval : base.GetBy(type);
      }

      public void Reset()
      {
        long laggval;
        long num;
        do
        {
          laggval = this.laggval;
          num = Interlocked.CompareExchange(ref this.laggval, 0L, laggval);
        }
        while (num != laggval);
        Interlocked.Add(ref this.ltotalval, num);
      }
    }

    private class AvgBox : OperationProfiler.IBox, IProfilingResult
    {
      internal long ltotal;
      internal long lcount;

      public long lval => this.lcount == 0L ? 0L : this.ltotal / this.lcount;

      public long GetBy(ProfilingResultType type)
      {
        if (type == ProfilingResultType.Current)
          return this.lcount;
        return type == ProfilingResultType.AverageInTimeWindow ? this.lval : -1L;
      }
    }

    private class ExtBox : OperationProfiler.IBox, IProfilingResult, IResettable, IDisposable
    {
      internal ExternalCounter ExtCounter { private get; set; }

      public long lval => (long) this.ExtCounter.Value;

      public virtual long GetBy(ProfilingResultType type) => type == ProfilingResultType.Current ? this.lval : -1L;

      public void Reset()
      {
        if (!(this.ExtCounter is IResettable extCounter))
          return;
        extCounter.Reset();
      }

      public void Dispose()
      {
        if (!(this.ExtCounter is IDisposable extCounter))
          return;
        extCounter.Dispose();
      }
    }

    private class DerivedBox : OperationProfiler.IBox
    {
      internal DerivedCounter counter;

      public long lval => throw new NotImplementedException();

      internal DerivedBox(DerivedCounter counter) => this.counter = counter;
    }
  }
}
