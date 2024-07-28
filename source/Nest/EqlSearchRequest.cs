// Decompiled with JetBrains decompiler
// Type: Nest.EqlSearchRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.EqlApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class EqlSearchRequest : 
    PlainRequestBase<EqlSearchRequestParameters>,
    IEqlSearchRequest,
    IRequest<EqlSearchRequestParameters>,
    IRequest,
    ITypedSearchRequest
  {
    protected IEqlSearchRequest Self => (IEqlSearchRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.EqlSearch;

    public EqlSearchRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected EqlSearchRequest()
    {
    }

    [IgnoreDataMember]
    Indices IEqlSearchRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public Time KeepAlive
    {
      get => this.Q<Time>("keep_alive");
      set => this.Q("keep_alive", (object) value);
    }

    public bool? KeepOnCompletion
    {
      get => this.Q<bool?>("keep_on_completion");
      set => this.Q("keep_on_completion", (object) value);
    }

    public Time WaitForCompletionTimeout
    {
      get => this.Q<Time>("wait_for_completion_timeout");
      set => this.Q("wait_for_completion_timeout", (object) value);
    }

    public Field EventCategoryField { get; set; }

    public int? FetchSize { get; set; }

    public Fields Fields { get; set; }

    public QueryContainer Filter { get; set; }

    public string Query { get; set; }

    public EqlResultPosition? ResultPosition { get; set; }

    public IRuntimeFields RuntimeFields { get; set; }

    public float? Size { get; set; }

    public Field TiebreakerField { get; set; }

    public Field TimestampField { get; set; }

    Type ITypedSearchRequest.ClrType => (Type) null;
  }
}
