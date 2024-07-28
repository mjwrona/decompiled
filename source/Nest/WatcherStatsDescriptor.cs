// Decompiled with JetBrains decompiler
// Type: Nest.WatcherStatsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.WatcherApi;
using System;

namespace Nest
{
  public class WatcherStatsDescriptor : 
    RequestDescriptorBase<WatcherStatsDescriptor, WatcherStatsRequestParameters, IWatcherStatsRequest>,
    IWatcherStatsRequest,
    IRequest<WatcherStatsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherStats;

    public WatcherStatsDescriptor()
    {
    }

    public WatcherStatsDescriptor(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    Metrics IWatcherStatsRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    public WatcherStatsDescriptor Metric(Metrics metric) => this.Assign<Metrics>(metric, (Action<IWatcherStatsRequest, Metrics>) ((a, v) => a.RouteValues.Optional(nameof (metric), v)));

    public WatcherStatsDescriptor EmitStacktraces(bool? emitstacktraces = true) => this.Qs("emit_stacktraces", (object) emitstacktraces);
  }
}
