// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.ClientSideMetrics
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class ClientSideMetrics
  {
    public static readonly ClientSideMetrics Empty = new ClientSideMetrics(0L, 0.0, (IEnumerable<FetchExecutionRange>) new List<FetchExecutionRange>());

    public ClientSideMetrics(
      long retries,
      double requestCharge,
      IEnumerable<FetchExecutionRange> fetchExecutionRanges)
    {
      this.Retries = retries;
      this.RequestCharge = requestCharge;
      this.FetchExecutionRanges = fetchExecutionRanges ?? throw new ArgumentNullException(nameof (fetchExecutionRanges));
    }

    public long Retries { get; }

    public double RequestCharge { get; }

    public IEnumerable<FetchExecutionRange> FetchExecutionRanges { get; }

    public ref struct Accumulator
    {
      public Accumulator(
        long retries,
        double requestCharge,
        IEnumerable<FetchExecutionRange> fetchExecutionRanges)
      {
        this.Retries = retries;
        this.RequestCharge = requestCharge;
        this.FetchExecutionRanges = fetchExecutionRanges;
      }

      public long Retries { get; }

      public double RequestCharge { get; }

      public IEnumerable<FetchExecutionRange> FetchExecutionRanges { get; }

      public ClientSideMetrics.Accumulator Accumulate(ClientSideMetrics clientSideMetrics)
      {
        if (clientSideMetrics == null)
          throw new ArgumentNullException(nameof (clientSideMetrics));
        return new ClientSideMetrics.Accumulator(this.Retries + clientSideMetrics.Retries, this.RequestCharge + clientSideMetrics.RequestCharge, (this.FetchExecutionRanges ?? Enumerable.Empty<FetchExecutionRange>()).Concat<FetchExecutionRange>(clientSideMetrics.FetchExecutionRanges));
      }

      public static ClientSideMetrics ToClientSideMetrics(ClientSideMetrics.Accumulator accumulator) => new ClientSideMetrics(accumulator.Retries, accumulator.RequestCharge, accumulator.FetchExecutionRanges);
    }
  }
}
