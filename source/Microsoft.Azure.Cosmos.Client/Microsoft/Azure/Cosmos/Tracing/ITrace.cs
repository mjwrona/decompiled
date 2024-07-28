// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.ITrace
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Tracing
{
  internal interface ITrace : IDisposable
  {
    string Name { get; }

    Guid Id { get; }

    DateTime StartTime { get; }

    TimeSpan Duration { get; }

    TraceLevel Level { get; }

    TraceComponent Component { get; }

    TraceSummary Summary { get; }

    ITrace Parent { get; }

    IReadOnlyList<ITrace> Children { get; }

    IReadOnlyDictionary<string, object> Data { get; }

    ITrace StartChild(string name);

    ITrace StartChild(string name, TraceComponent component, TraceLevel level);

    void AddDatum(string key, TraceDatum traceDatum);

    void AddDatum(string key, object value);

    void AddOrUpdateDatum(string key, object value);

    void AddChild(ITrace trace);
  }
}
