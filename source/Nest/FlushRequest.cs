// Decompiled with JetBrains decompiler
// Type: Nest.FlushRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class FlushRequest : 
    PlainRequestBase<FlushRequestParameters>,
    IFlushRequest,
    IRequest<FlushRequestParameters>,
    IRequest
  {
    protected IFlushRequest Self => (IFlushRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesFlush;

    public FlushRequest()
    {
    }

    public FlushRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices IFlushRequest.Index => this.Self.RouteValues.Get<Indices>("index");

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

    public bool? Force
    {
      get => this.Q<bool?>("force");
      set => this.Q("force", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public bool? WaitIfOngoing
    {
      get => this.Q<bool?>("wait_if_ongoing");
      set => this.Q("wait_if_ongoing", (object) value);
    }
  }
}
