// Decompiled with JetBrains decompiler
// Type: Nest.RolloverIndexRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class RolloverIndexRequest : 
    PlainRequestBase<RolloverIndexRequestParameters>,
    IRolloverIndexRequest,
    IIndexState,
    IRequest<RolloverIndexRequestParameters>,
    IRequest
  {
    public IAliases Aliases { get; set; }

    public IRolloverConditions Conditions { get; set; }

    public ITypeMapping Mappings { get; set; }

    public IIndexSettings Settings { get; set; }

    protected IRolloverIndexRequest Self => (IRolloverIndexRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesRollover;

    public RolloverIndexRequest(Name alias)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (alias), (IUrlParameter) alias)))
    {
    }

    public RolloverIndexRequest(Name alias, IndexName newIndex)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (alias), (IUrlParameter) alias).Optional("new_index", (IUrlParameter) newIndex)))
    {
    }

    [SerializationConstructor]
    protected RolloverIndexRequest()
    {
    }

    [IgnoreDataMember]
    Name IRolloverIndexRequest.Alias => this.Self.RouteValues.Get<Name>("alias");

    [IgnoreDataMember]
    IndexName IRolloverIndexRequest.NewIndex => this.Self.RouteValues.Get<IndexName>("new_index");

    public bool? DryRun
    {
      get => this.Q<bool?>("dry_run");
      set => this.Q("dry_run", (object) value);
    }

    public bool? IncludeTypeName
    {
      get => this.Q<bool?>("include_type_name");
      set => this.Q("include_type_name", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public string WaitForActiveShards
    {
      get => this.Q<string>("wait_for_active_shards");
      set => this.Q("wait_for_active_shards", (object) value);
    }
  }
}
