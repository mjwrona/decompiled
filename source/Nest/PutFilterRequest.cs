// Decompiled with JetBrains decompiler
// Type: Nest.PutFilterRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class PutFilterRequest : 
    PlainRequestBase<PutFilterRequestParameters>,
    IPutFilterRequest,
    IRequest<PutFilterRequestParameters>,
    IRequest
  {
    protected IPutFilterRequest Self => (IPutFilterRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPutFilter;

    public PutFilterRequest(Id filterId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("filter_id", (IUrlParameter) filterId)))
    {
    }

    [SerializationConstructor]
    protected PutFilterRequest()
    {
    }

    [IgnoreDataMember]
    Id IPutFilterRequest.FilterId => this.Self.RouteValues.Get<Id>("filter_id");

    public string Description { get; set; }

    public IEnumerable<string> Items { get; set; }
  }
}
