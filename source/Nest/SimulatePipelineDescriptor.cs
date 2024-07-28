// Decompiled with JetBrains decompiler
// Type: Nest.SimulatePipelineDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IngestApi;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class SimulatePipelineDescriptor : 
    RequestDescriptorBase<SimulatePipelineDescriptor, SimulatePipelineRequestParameters, ISimulatePipelineRequest>,
    ISimulatePipelineRequest,
    IRequest<SimulatePipelineRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IngestSimulatePipeline;

    public SimulatePipelineDescriptor()
    {
    }

    public SimulatePipelineDescriptor(Nest.Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    Nest.Id ISimulatePipelineRequest.Id => this.Self.RouteValues.Get<Nest.Id>("id");

    public SimulatePipelineDescriptor Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<ISimulatePipelineRequest, Nest.Id>) ((a, v) => a.RouteValues.Optional(nameof (id), (IUrlParameter) v)));

    public SimulatePipelineDescriptor Verbose(bool? verbose = true) => this.Qs(nameof (verbose), (object) verbose);

    IEnumerable<ISimulatePipelineDocument> ISimulatePipelineRequest.Documents { get; set; }

    IPipeline ISimulatePipelineRequest.Pipeline { get; set; }

    public SimulatePipelineDescriptor Pipeline(Func<PipelineDescriptor, IPipeline> pipeline) => this.Assign<Func<PipelineDescriptor, IPipeline>>(pipeline, (Action<ISimulatePipelineRequest, Func<PipelineDescriptor, IPipeline>>) ((a, v) => a.Pipeline = v != null ? v(new PipelineDescriptor()) : (IPipeline) null));

    public SimulatePipelineDescriptor Documents(IEnumerable<ISimulatePipelineDocument> documents) => this.Assign<IEnumerable<ISimulatePipelineDocument>>(documents, (Action<ISimulatePipelineRequest, IEnumerable<ISimulatePipelineDocument>>) ((a, v) => a.Documents = v));

    public SimulatePipelineDescriptor Documents(
      Func<SimulatePipelineDocumentsDescriptor, IPromise<IList<ISimulatePipelineDocument>>> selector)
    {
      return this.Assign<Func<SimulatePipelineDocumentsDescriptor, IPromise<IList<ISimulatePipelineDocument>>>>(selector, (Action<ISimulatePipelineRequest, Func<SimulatePipelineDocumentsDescriptor, IPromise<IList<ISimulatePipelineDocument>>>>) ((a, v) => a.Documents = v != null ? (IEnumerable<ISimulatePipelineDocument>) v(new SimulatePipelineDocumentsDescriptor())?.Value : (IEnumerable<ISimulatePipelineDocument>) null));
    }
  }
}
