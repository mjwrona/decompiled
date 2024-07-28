// Decompiled with JetBrains decompiler
// Type: Nest.UpdateFilterRequest
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
  public class UpdateFilterRequest : 
    PlainRequestBase<UpdateFilterRequestParameters>,
    IUpdateFilterRequest,
    IRequest<UpdateFilterRequestParameters>,
    IRequest
  {
    protected IUpdateFilterRequest Self => (IUpdateFilterRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningUpdateFilter;

    public UpdateFilterRequest(Id filterId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("filter_id", (IUrlParameter) filterId)))
    {
    }

    [SerializationConstructor]
    protected UpdateFilterRequest()
    {
    }

    [IgnoreDataMember]
    Id IUpdateFilterRequest.FilterId => this.Self.RouteValues.Get<Id>("filter_id");

    public IEnumerable<string> AddItems { get; set; }

    public string Description { get; set; }

    public IEnumerable<string> RemoveItems { get; set; }
  }
}
