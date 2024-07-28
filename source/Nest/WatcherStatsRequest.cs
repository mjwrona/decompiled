// Decompiled with JetBrains decompiler
// Type: Nest.WatcherStatsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.WatcherApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class WatcherStatsRequest : 
    PlainRequestBase<WatcherStatsRequestParameters>,
    IWatcherStatsRequest,
    IRequest<WatcherStatsRequestParameters>,
    IRequest
  {
    protected IWatcherStatsRequest Self => (IWatcherStatsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherStats;

    public WatcherStatsRequest()
    {
    }

    public WatcherStatsRequest(Metrics metric)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (metric), metric)))
    {
    }

    [IgnoreDataMember]
    Metrics IWatcherStatsRequest.Metric => this.Self.RouteValues.Get<Metrics>("metric");

    public bool? EmitStacktraces
    {
      get => this.Q<bool?>("emit_stacktraces");
      set => this.Q("emit_stacktraces", (object) value);
    }
  }
}
