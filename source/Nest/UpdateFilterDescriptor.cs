// Decompiled with JetBrains decompiler
// Type: Nest.UpdateFilterDescriptor
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
  public class UpdateFilterDescriptor : 
    RequestDescriptorBase<UpdateFilterDescriptor, UpdateFilterRequestParameters, IUpdateFilterRequest>,
    IUpdateFilterRequest,
    IRequest<UpdateFilterRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningUpdateFilter;

    public UpdateFilterDescriptor(Id filterId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("filter_id", (IUrlParameter) filterId)))
    {
    }

    [SerializationConstructor]
    protected UpdateFilterDescriptor()
    {
    }

    Id IUpdateFilterRequest.FilterId => this.Self.RouteValues.Get<Id>("filter_id");

    IEnumerable<string> IUpdateFilterRequest.AddItems { get; set; }

    string IUpdateFilterRequest.Description { get; set; }

    IEnumerable<string> IUpdateFilterRequest.RemoveItems { get; set; }

    public UpdateFilterDescriptor Description(string description) => this.Assign<string>(description, (Action<IUpdateFilterRequest, string>) ((a, v) => a.Description = v));

    public UpdateFilterDescriptor AddItems(params string[] items) => this.Assign<string[]>(items, (Action<IUpdateFilterRequest, string[]>) ((a, v) => a.AddItems = (IEnumerable<string>) v));

    public UpdateFilterDescriptor AddItems(IEnumerable<string> items) => this.Assign<IEnumerable<string>>(items, (Action<IUpdateFilterRequest, IEnumerable<string>>) ((a, v) => a.AddItems = v));

    public UpdateFilterDescriptor RemoveItems(params string[] items) => this.Assign<string[]>(items, (Action<IUpdateFilterRequest, string[]>) ((a, v) => a.RemoveItems = (IEnumerable<string>) v));

    public UpdateFilterDescriptor RemoveItems(IEnumerable<string> items) => this.Assign<IEnumerable<string>>(items, (Action<IUpdateFilterRequest, IEnumerable<string>>) ((a, v) => a.RemoveItems = v));
  }
}
