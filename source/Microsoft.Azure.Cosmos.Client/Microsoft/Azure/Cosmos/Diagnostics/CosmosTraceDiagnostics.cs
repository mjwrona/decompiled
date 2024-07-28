// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Diagnostics.CosmosTraceDiagnostics
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Cosmos.Diagnostics
{
  internal sealed class CosmosTraceDiagnostics : CosmosDiagnostics
  {
    public CosmosTraceDiagnostics(ITrace trace)
    {
      ITrace trace1 = trace != null ? trace : throw new ArgumentNullException(nameof (trace));
      while (trace1.Parent != null)
        trace1 = trace1.Parent;
      this.Value = trace1;
    }

    public ITrace Value { get; }

    public override string ToString() => this.ToJsonString();

    public override TimeSpan GetClientElapsedTime() => this.Value.Duration;

    public override IReadOnlyList<(string regionName, Uri uri)> GetContactedRegions() => this.Value?.Summary?.RegionsContacted;

    internal bool IsGoneExceptionHit() => this.WalkTraceTreeForGoneException(this.Value);

    private bool WalkTraceTreeForGoneException(ITrace currentTrace)
    {
      if (currentTrace == null)
        return false;
      foreach (object obj in currentTrace.Data.Values)
      {
        if (obj is ClientSideRequestStatisticsTraceDatum statisticsTraceDatum)
        {
          foreach (ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics responseStatistics in (IEnumerable<ClientSideRequestStatisticsTraceDatum.StoreResponseStatistics>) statisticsTraceDatum.StoreResponseStatisticsList)
          {
            if (responseStatistics.StoreResult != null && responseStatistics.StoreResult.StatusCode == StatusCodes.Gone)
              return true;
          }
        }
      }
      foreach (ITrace child in (IEnumerable<ITrace>) currentTrace.Children)
      {
        if (this.WalkTraceTreeForGoneException(child))
          return true;
      }
      return false;
    }

    private string ToJsonString() => Encoding.UTF8.GetString(this.WriteTraceToJsonWriter(JsonSerializationFormat.Text).Span);

    private ReadOnlyMemory<byte> WriteTraceToJsonWriter(
      JsonSerializationFormat jsonSerializationFormat)
    {
      IJsonWriter writer = JsonWriter.Create(jsonSerializationFormat);
      TraceWriter.WriteTrace(writer, this.Value);
      return writer.GetResult();
    }

    public override DateTime? GetStartTimeUtc()
    {
      if (this.Value == null)
        return new DateTime?();
      DateTime startTime = this.Value.StartTime;
      return new DateTime?(this.Value.StartTime);
    }

    public override int GetFailedRequestCount() => this.Value == null || this.Value.Summary == null ? 0 : this.Value.Summary.GetFailedCount();
  }
}
