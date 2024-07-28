// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.NoOpTrace
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Tracing
{
  internal sealed class NoOpTrace : ITrace, IDisposable
  {
    public static readonly NoOpTrace Singleton = new NoOpTrace();
    public static readonly TraceSummary NoOpTraceSummary = new TraceSummary();
    private static readonly IReadOnlyList<ITrace> NoOpChildren = (IReadOnlyList<ITrace>) new List<ITrace>();
    private static readonly IReadOnlyDictionary<string, object> NoOpData = (IReadOnlyDictionary<string, object>) new Dictionary<string, object>();

    private NoOpTrace()
    {
    }

    public string Name => "NoOp";

    public Guid Id => new Guid();

    public DateTime StartTime => new DateTime();

    public TimeSpan Duration => new TimeSpan();

    public TraceLevel Level => TraceLevel.Off;

    public TraceSummary Summary => NoOpTrace.NoOpTraceSummary;

    public TraceComponent Component => TraceComponent.Unknown;

    public ITrace Parent => (ITrace) null;

    public IReadOnlyList<ITrace> Children => NoOpTrace.NoOpChildren;

    public IReadOnlyDictionary<string, object> Data => NoOpTrace.NoOpData;

    public void Dispose()
    {
    }

    public ITrace StartChild(string name) => this.StartChild(name, this.Component, TraceLevel.Info);

    public ITrace StartChild(string name, TraceComponent component, TraceLevel level) => (ITrace) this;

    public void AddDatum(string key, TraceDatum traceDatum)
    {
    }

    public void AddDatum(string key, object value)
    {
    }

    public void AddChild(ITrace trace)
    {
    }

    public void AddOrUpdateDatum(string key, object value)
    {
    }

    public void UpdateRegionContacted(TraceDatum traceDatum)
    {
    }
  }
}
