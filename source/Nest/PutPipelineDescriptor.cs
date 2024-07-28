// Decompiled with JetBrains decompiler
// Type: Nest.PutPipelineDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IngestApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class PutPipelineDescriptor : 
    RequestDescriptorBase<PutPipelineDescriptor, PutPipelineRequestParameters, IPutPipelineRequest>,
    IPutPipelineRequest,
    IPipeline,
    IRequest<PutPipelineRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IngestPutPipeline;

    public PutPipelineDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected PutPipelineDescriptor()
    {
    }

    Id IPutPipelineRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public PutPipelineDescriptor IfVersion(int? ifversion) => this.Qs("if_version", (object) ifversion);

    public PutPipelineDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public PutPipelineDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    string IPipeline.Description { get; set; }

    IEnumerable<IProcessor> IPipeline.OnFailure { get; set; }

    IEnumerable<IProcessor> IPipeline.Processors { get; set; }

    long? IPipeline.Version { get; set; }

    public PutPipelineDescriptor Description(string description) => this.Assign<string>(description, (Action<IPutPipelineRequest, string>) ((a, v) => a.Description = v));

    public PutPipelineDescriptor Processors(IEnumerable<IProcessor> processors) => this.Assign<List<IProcessor>>(processors.ToListOrNullIfEmpty<IProcessor>(), (Action<IPutPipelineRequest, List<IProcessor>>) ((a, v) => a.Processors = (IEnumerable<IProcessor>) v));

    public PutPipelineDescriptor Processors(
      Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>> selector)
    {
      return this.Assign<Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>(selector, (Action<IPutPipelineRequest, Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>) ((a, v) => a.Processors = v != null ? (IEnumerable<IProcessor>) v(new ProcessorsDescriptor())?.Value : (IEnumerable<IProcessor>) null));
    }

    public PutPipelineDescriptor OnFailure(IEnumerable<IProcessor> processors) => this.Assign<List<IProcessor>>(processors.ToListOrNullIfEmpty<IProcessor>(), (Action<IPutPipelineRequest, List<IProcessor>>) ((a, v) => a.OnFailure = (IEnumerable<IProcessor>) v));

    public PutPipelineDescriptor OnFailure(
      Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>> selector)
    {
      return this.Assign<Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>(selector, (Action<IPutPipelineRequest, Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>) ((a, v) => a.OnFailure = v != null ? (IEnumerable<IProcessor>) v(new ProcessorsDescriptor())?.Value : (IEnumerable<IProcessor>) null));
    }

    public PutPipelineDescriptor Version(long? version = null) => this.Assign<long?>(version, (Action<IPutPipelineRequest, long?>) ((a, v) => a.Version = v));
  }
}
