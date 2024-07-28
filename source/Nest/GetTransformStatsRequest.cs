// Decompiled with JetBrains decompiler
// Type: Nest.GetTransformStatsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetTransformStatsRequest : 
    PlainRequestBase<GetTransformStatsRequestParameters>,
    IGetTransformStatsRequest,
    IRequest<GetTransformStatsRequestParameters>,
    IRequest
  {
    protected IGetTransformStatsRequest Self => (IGetTransformStatsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformGetStats;

    public GetTransformStatsRequest(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("transform_id", (IUrlParameter) transformId)))
    {
    }

    [SerializationConstructor]
    protected GetTransformStatsRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetTransformStatsRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public bool? AllowNoMatch
    {
      get => this.Q<bool?>("allow_no_match");
      set => this.Q("allow_no_match", (object) value);
    }

    public long? From
    {
      get => this.Q<long?>("from");
      set => this.Q("from", (object) value);
    }

    public long? Size
    {
      get => this.Q<long?>("size");
      set => this.Q("size", (object) value);
    }
  }
}
