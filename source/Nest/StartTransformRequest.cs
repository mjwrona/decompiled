// Decompiled with JetBrains decompiler
// Type: Nest.StartTransformRequest
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
  public class StartTransformRequest : 
    PlainRequestBase<StartTransformRequestParameters>,
    IStartTransformRequest,
    IRequest<StartTransformRequestParameters>,
    IRequest
  {
    protected IStartTransformRequest Self => (IStartTransformRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.TransformStart;

    public StartTransformRequest(Id transformId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("transform_id", (IUrlParameter) transformId)))
    {
    }

    [SerializationConstructor]
    protected StartTransformRequest()
    {
    }

    [IgnoreDataMember]
    Id IStartTransformRequest.TransformId => this.Self.RouteValues.Get<Id>("transform_id");

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
