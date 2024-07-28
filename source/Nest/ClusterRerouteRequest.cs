// Decompiled with JetBrains decompiler
// Type: Nest.ClusterRerouteRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;
using System.Collections.Generic;

namespace Nest
{
  public class ClusterRerouteRequest : 
    PlainRequestBase<ClusterRerouteRequestParameters>,
    IClusterRerouteRequest,
    IRequest<ClusterRerouteRequestParameters>,
    IRequest
  {
    public IList<IClusterRerouteCommand> Commands { get; set; }

    protected IClusterRerouteRequest Self => (IClusterRerouteRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterReroute;

    public bool? DryRun
    {
      get => this.Q<bool?>("dry_run");
      set => this.Q("dry_run", (object) value);
    }

    public bool? Explain
    {
      get => this.Q<bool?>("explain");
      set => this.Q("explain", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public string[] Metric
    {
      get => this.Q<string[]>("metric");
      set => this.Q("metric", (object) value);
    }

    public bool? RetryFailed
    {
      get => this.Q<bool?>("retry_failed");
      set => this.Q("retry_failed", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
