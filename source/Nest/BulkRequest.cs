// Decompiled with JetBrains decompiler
// Type: Nest.BulkRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class BulkRequest : 
    PlainRequestBase<BulkRequestParameters>,
    IBulkRequest,
    IRequest<BulkRequestParameters>,
    IRequest
  {
    public BulkOperationsCollection<IBulkOperation> Operations { get; set; }

    protected IBulkRequest Self => (IBulkRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceBulk;

    public BulkRequest()
    {
    }

    public BulkRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    IndexName IBulkRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public string Pipeline
    {
      get => this.Q<string>("pipeline");
      set => this.Q("pipeline", (object) value);
    }

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public bool? RequireAlias
    {
      get => this.Q<bool?>("require_alias");
      set => this.Q("require_alias", (object) value);
    }

    public Routing Routing
    {
      get => this.Q<Routing>("routing");
      set => this.Q("routing", (object) value);
    }

    public bool? SourceEnabled
    {
      get => this.Q<bool?>("_source");
      set => this.Q("_source", (object) value);
    }

    public Fields SourceExcludes
    {
      get => this.Q<Fields>("_source_excludes");
      set => this.Q("_source_excludes", (object) value);
    }

    public Fields SourceIncludes
    {
      get => this.Q<Fields>("_source_includes");
      set => this.Q("_source_includes", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public string TypeQueryString
    {
      get => this.Q<string>("type");
      set => this.Q("type", (object) value);
    }

    public string WaitForActiveShards
    {
      get => this.Q<string>("wait_for_active_shards");
      set => this.Q("wait_for_active_shards", (object) value);
    }
  }
}
