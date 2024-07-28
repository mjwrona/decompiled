// Decompiled with JetBrains decompiler
// Type: Nest.GetTransformRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.TransformApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetTransformRequest : 
    PlainRequestBase<GetTransformRequestParameters>,
    IGetTransformRequest,
    IRequest<GetTransformRequestParameters>,
    IRequest
  {
    protected IGetTransformRequest Self => (IGetTransformRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformGet;

    public GetTransformRequest(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("transform_id", (IUrlParameter) transformId)))
    {
    }

    public GetTransformRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public bool? AllowNoMatch
    {
      get => this.Q<bool?>("allow_no_match");
      set => this.Q("allow_no_match", (object) value);
    }

    public bool? ExcludeGenerated
    {
      get => this.Q<bool?>("exclude_generated");
      set => this.Q("exclude_generated", (object) value);
    }

    public int? From
    {
      get => this.Q<int?>("from");
      set => this.Q("from", (object) value);
    }

    public int? Size
    {
      get => this.Q<int?>("size");
      set => this.Q("size", (object) value);
    }
  }
}
