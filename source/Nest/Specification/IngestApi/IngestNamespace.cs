// Decompiled with JetBrains decompiler
// Type: Nest.Specification.IngestApi.IngestNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.IngestApi
{
  public class IngestNamespace : Nest.NamespacedClientProxy
  {
    internal IngestNamespace(ElasticClient client)
      : base(client)
    {
    }

    public DeletePipelineResponse DeletePipeline(
      Id id,
      Func<DeletePipelineDescriptor, IDeletePipelineRequest> selector = null)
    {
      return this.DeletePipeline(selector.InvokeOrDefault<DeletePipelineDescriptor, IDeletePipelineRequest>(new DeletePipelineDescriptor(id)));
    }

    public Task<DeletePipelineResponse> DeletePipelineAsync(
      Id id,
      Func<DeletePipelineDescriptor, IDeletePipelineRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeletePipelineAsync(selector.InvokeOrDefault<DeletePipelineDescriptor, IDeletePipelineRequest>(new DeletePipelineDescriptor(id)), ct);
    }

    public DeletePipelineResponse DeletePipeline(IDeletePipelineRequest request) => this.DoRequest<IDeletePipelineRequest, DeletePipelineResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeletePipelineResponse> DeletePipelineAsync(
      IDeletePipelineRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeletePipelineRequest, DeletePipelineResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetPipelineResponse GetPipeline(
      Func<GetPipelineDescriptor, IGetPipelineRequest> selector = null)
    {
      return this.GetPipeline(selector.InvokeOrDefault<GetPipelineDescriptor, IGetPipelineRequest>(new GetPipelineDescriptor()));
    }

    public Task<GetPipelineResponse> GetPipelineAsync(
      Func<GetPipelineDescriptor, IGetPipelineRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetPipelineAsync(selector.InvokeOrDefault<GetPipelineDescriptor, IGetPipelineRequest>(new GetPipelineDescriptor()), ct);
    }

    public GetPipelineResponse GetPipeline(IGetPipelineRequest request) => this.DoRequest<IGetPipelineRequest, GetPipelineResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetPipelineResponse> GetPipelineAsync(
      IGetPipelineRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetPipelineRequest, GetPipelineResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GrokProcessorPatternsResponse GrokProcessorPatterns(
      Func<GrokProcessorPatternsDescriptor, IGrokProcessorPatternsRequest> selector = null)
    {
      return this.GrokProcessorPatterns(selector.InvokeOrDefault<GrokProcessorPatternsDescriptor, IGrokProcessorPatternsRequest>(new GrokProcessorPatternsDescriptor()));
    }

    public Task<GrokProcessorPatternsResponse> GrokProcessorPatternsAsync(
      Func<GrokProcessorPatternsDescriptor, IGrokProcessorPatternsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GrokProcessorPatternsAsync(selector.InvokeOrDefault<GrokProcessorPatternsDescriptor, IGrokProcessorPatternsRequest>(new GrokProcessorPatternsDescriptor()), ct);
    }

    public GrokProcessorPatternsResponse GrokProcessorPatterns(IGrokProcessorPatternsRequest request) => this.DoRequest<IGrokProcessorPatternsRequest, GrokProcessorPatternsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GrokProcessorPatternsResponse> GrokProcessorPatternsAsync(
      IGrokProcessorPatternsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGrokProcessorPatternsRequest, GrokProcessorPatternsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutPipelineResponse PutPipeline(
      Id id,
      Func<PutPipelineDescriptor, IPutPipelineRequest> selector)
    {
      return this.PutPipeline(selector.InvokeOrDefault<PutPipelineDescriptor, IPutPipelineRequest>(new PutPipelineDescriptor(id)));
    }

    public Task<PutPipelineResponse> PutPipelineAsync(
      Id id,
      Func<PutPipelineDescriptor, IPutPipelineRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutPipelineAsync(selector.InvokeOrDefault<PutPipelineDescriptor, IPutPipelineRequest>(new PutPipelineDescriptor(id)), ct);
    }

    public PutPipelineResponse PutPipeline(IPutPipelineRequest request) => this.DoRequest<IPutPipelineRequest, PutPipelineResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutPipelineResponse> PutPipelineAsync(
      IPutPipelineRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutPipelineRequest, PutPipelineResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public SimulatePipelineResponse SimulatePipeline(
      Func<SimulatePipelineDescriptor, ISimulatePipelineRequest> selector = null)
    {
      return this.SimulatePipeline(selector.InvokeOrDefault<SimulatePipelineDescriptor, ISimulatePipelineRequest>(new SimulatePipelineDescriptor()));
    }

    public Task<SimulatePipelineResponse> SimulatePipelineAsync(
      Func<SimulatePipelineDescriptor, ISimulatePipelineRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.SimulatePipelineAsync(selector.InvokeOrDefault<SimulatePipelineDescriptor, ISimulatePipelineRequest>(new SimulatePipelineDescriptor()), ct);
    }

    public SimulatePipelineResponse SimulatePipeline(ISimulatePipelineRequest request) => this.DoRequest<ISimulatePipelineRequest, SimulatePipelineResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SimulatePipelineResponse> SimulatePipelineAsync(
      ISimulatePipelineRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ISimulatePipelineRequest, SimulatePipelineResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
