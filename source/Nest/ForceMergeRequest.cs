// Decompiled with JetBrains decompiler
// Type: Nest.ForceMergeRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ForceMergeRequest : 
    PlainRequestBase<ForceMergeRequestParameters>,
    IForceMergeRequest,
    IRequest<ForceMergeRequestParameters>,
    IRequest
  {
    protected IForceMergeRequest Self => (IForceMergeRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesForceMerge;

    public ForceMergeRequest()
    {
    }

    public ForceMergeRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices IForceMergeRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public bool? Flush
    {
      get => this.Q<bool?>("flush");
      set => this.Q("flush", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public long? MaxNumSegments
    {
      get => this.Q<long?>("max_num_segments");
      set => this.Q("max_num_segments", (object) value);
    }

    public bool? OnlyExpungeDeletes
    {
      get => this.Q<bool?>("only_expunge_deletes");
      set => this.Q("only_expunge_deletes", (object) value);
    }
  }
}
