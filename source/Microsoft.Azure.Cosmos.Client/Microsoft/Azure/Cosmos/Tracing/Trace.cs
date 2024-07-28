// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.Trace
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Tracing
{
  internal sealed class Trace : ITrace, IDisposable
  {
    private static readonly IReadOnlyDictionary<string, object> EmptyDictionary = (IReadOnlyDictionary<string, object>) new Dictionary<string, object>();
    private readonly List<ITrace> children;
    private readonly Lazy<Dictionary<string, object>> data;
    private ValueStopwatch stopwatch;

    private Trace(
      string name,
      TraceLevel level,
      TraceComponent component,
      Trace parent,
      TraceSummary summary)
    {
      this.Name = name ?? throw new ArgumentNullException(nameof (name));
      this.Id = Guid.NewGuid();
      this.StartTime = DateTime.UtcNow;
      this.stopwatch = ValueStopwatch.StartNew();
      this.Level = level;
      this.Component = component;
      this.Parent = (ITrace) parent;
      this.children = new List<ITrace>();
      this.data = new Lazy<Dictionary<string, object>>();
      this.Summary = summary ?? throw new ArgumentNullException(nameof (summary));
    }

    public string Name { get; }

    public Guid Id { get; }

    public DateTime StartTime { get; }

    public TimeSpan Duration => this.stopwatch.Elapsed;

    public TraceLevel Level { get; }

    public TraceComponent Component { get; }

    public TraceSummary Summary { get; }

    public ITrace Parent { get; }

    public IReadOnlyList<ITrace> Children => (IReadOnlyList<ITrace>) this.children;

    public IReadOnlyDictionary<string, object> Data => !this.data.IsValueCreated ? Trace.EmptyDictionary : (IReadOnlyDictionary<string, object>) this.data.Value;

    public void Dispose() => this.stopwatch.Stop();

    public ITrace StartChild(string name) => this.StartChild(name, this.Component, TraceLevel.Verbose);

    public ITrace StartChild(string name, TraceComponent component, TraceLevel level)
    {
      if (this.Parent != null && !this.stopwatch.IsRunning)
        return this.Parent.StartChild(name, component, level);
      Trace child = new Trace(name, level, component, this, this.Summary);
      this.AddChild((ITrace) child);
      return (ITrace) child;
    }

    public void AddChild(ITrace child)
    {
      lock (this.children)
        this.children.Add(child);
    }

    public static Trace GetRootTrace(string name) => Trace.GetRootTrace(name, TraceComponent.Unknown, TraceLevel.Verbose);

    public static Trace GetRootTrace(string name, TraceComponent component, TraceLevel level) => new Trace(name, level, component, (Trace) null, new TraceSummary());

    public void AddDatum(string key, TraceDatum traceDatum)
    {
      this.data.Value.Add(key, (object) traceDatum);
      this.Summary.UpdateRegionContacted(traceDatum);
    }

    public void AddDatum(string key, object value) => this.data.Value.Add(key, value);

    public void AddOrUpdateDatum(string key, object value) => this.data.Value[key] = value;
  }
}
