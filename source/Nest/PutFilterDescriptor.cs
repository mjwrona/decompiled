// Decompiled with JetBrains decompiler
// Type: Nest.PutFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class PutFilterDescriptor : 
    RequestDescriptorBase<PutFilterDescriptor, PutFilterRequestParameters, IPutFilterRequest>,
    IPutFilterRequest,
    IRequest<PutFilterRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPutFilter;

    public PutFilterDescriptor(Id filterId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("filter_id", (IUrlParameter) filterId)))
    {
    }

    [SerializationConstructor]
    protected PutFilterDescriptor()
    {
    }

    Id IPutFilterRequest.FilterId => this.Self.RouteValues.Get<Id>("filter_id");

    string IPutFilterRequest.Description { get; set; }

    IEnumerable<string> IPutFilterRequest.Items { get; set; }

    public PutFilterDescriptor Description(string description) => this.Assign<string>(description, (Action<IPutFilterRequest, string>) ((a, v) => a.Description = v));

    public PutFilterDescriptor Items(params string[] items) => this.Assign<string[]>(items, (Action<IPutFilterRequest, string[]>) ((a, v) => a.Items = (IEnumerable<string>) v));

    public PutFilterDescriptor Items(IEnumerable<string> items) => this.Assign<IEnumerable<string>>(items, (Action<IPutFilterRequest, IEnumerable<string>>) ((a, v) => a.Items = v));
  }
}
