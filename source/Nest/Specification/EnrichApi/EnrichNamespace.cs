// Decompiled with JetBrains decompiler
// Type: Nest.Specification.EnrichApi.EnrichNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.EnrichApi
{
  public class EnrichNamespace : Nest.NamespacedClientProxy
  {
    internal EnrichNamespace(ElasticClient client)
      : base(client)
    {
    }

    public DeleteEnrichPolicyResponse DeletePolicy(
      Name name,
      Func<DeleteEnrichPolicyDescriptor, IDeleteEnrichPolicyRequest> selector = null)
    {
      return this.DeletePolicy(selector.InvokeOrDefault<DeleteEnrichPolicyDescriptor, IDeleteEnrichPolicyRequest>(new DeleteEnrichPolicyDescriptor(name)));
    }

    public Task<DeleteEnrichPolicyResponse> DeletePolicyAsync(
      Name name,
      Func<DeleteEnrichPolicyDescriptor, IDeleteEnrichPolicyRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeletePolicyAsync(selector.InvokeOrDefault<DeleteEnrichPolicyDescriptor, IDeleteEnrichPolicyRequest>(new DeleteEnrichPolicyDescriptor(name)), ct);
    }

    public DeleteEnrichPolicyResponse DeletePolicy(IDeleteEnrichPolicyRequest request) => this.DoRequest<IDeleteEnrichPolicyRequest, DeleteEnrichPolicyResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteEnrichPolicyResponse> DeletePolicyAsync(
      IDeleteEnrichPolicyRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteEnrichPolicyRequest, DeleteEnrichPolicyResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExecuteEnrichPolicyResponse ExecutePolicy(
      Name name,
      Func<ExecuteEnrichPolicyDescriptor, IExecuteEnrichPolicyRequest> selector = null)
    {
      return this.ExecutePolicy(selector.InvokeOrDefault<ExecuteEnrichPolicyDescriptor, IExecuteEnrichPolicyRequest>(new ExecuteEnrichPolicyDescriptor(name)));
    }

    public Task<ExecuteEnrichPolicyResponse> ExecutePolicyAsync(
      Name name,
      Func<ExecuteEnrichPolicyDescriptor, IExecuteEnrichPolicyRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ExecutePolicyAsync(selector.InvokeOrDefault<ExecuteEnrichPolicyDescriptor, IExecuteEnrichPolicyRequest>(new ExecuteEnrichPolicyDescriptor(name)), ct);
    }

    public ExecuteEnrichPolicyResponse ExecutePolicy(IExecuteEnrichPolicyRequest request) => this.DoRequest<IExecuteEnrichPolicyRequest, ExecuteEnrichPolicyResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExecuteEnrichPolicyResponse> ExecutePolicyAsync(
      IExecuteEnrichPolicyRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IExecuteEnrichPolicyRequest, ExecuteEnrichPolicyResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetEnrichPolicyResponse GetPolicy(
      Names name = null,
      Func<GetEnrichPolicyDescriptor, IGetEnrichPolicyRequest> selector = null)
    {
      return this.GetPolicy(selector.InvokeOrDefault<GetEnrichPolicyDescriptor, IGetEnrichPolicyRequest>(new GetEnrichPolicyDescriptor().Name(name)));
    }

    public Task<GetEnrichPolicyResponse> GetPolicyAsync(
      Names name = null,
      Func<GetEnrichPolicyDescriptor, IGetEnrichPolicyRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetPolicyAsync(selector.InvokeOrDefault<GetEnrichPolicyDescriptor, IGetEnrichPolicyRequest>(new GetEnrichPolicyDescriptor().Name(name)), ct);
    }

    public GetEnrichPolicyResponse GetPolicy(IGetEnrichPolicyRequest request) => this.DoRequest<IGetEnrichPolicyRequest, GetEnrichPolicyResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetEnrichPolicyResponse> GetPolicyAsync(
      IGetEnrichPolicyRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetEnrichPolicyRequest, GetEnrichPolicyResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutEnrichPolicyResponse PutPolicy<TDocument>(
      Name name,
      Func<PutEnrichPolicyDescriptor<TDocument>, IPutEnrichPolicyRequest> selector)
      where TDocument : class
    {
      return this.PutPolicy(selector.InvokeOrDefault<PutEnrichPolicyDescriptor<TDocument>, IPutEnrichPolicyRequest>(new PutEnrichPolicyDescriptor<TDocument>(name)));
    }

    public Task<PutEnrichPolicyResponse> PutPolicyAsync<TDocument>(
      Name name,
      Func<PutEnrichPolicyDescriptor<TDocument>, IPutEnrichPolicyRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.PutPolicyAsync(selector.InvokeOrDefault<PutEnrichPolicyDescriptor<TDocument>, IPutEnrichPolicyRequest>(new PutEnrichPolicyDescriptor<TDocument>(name)), ct);
    }

    public PutEnrichPolicyResponse PutPolicy(IPutEnrichPolicyRequest request) => this.DoRequest<IPutEnrichPolicyRequest, PutEnrichPolicyResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutEnrichPolicyResponse> PutPolicyAsync(
      IPutEnrichPolicyRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutEnrichPolicyRequest, PutEnrichPolicyResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public EnrichStatsResponse Stats(
      Func<EnrichStatsDescriptor, IEnrichStatsRequest> selector = null)
    {
      return this.Stats(selector.InvokeOrDefault<EnrichStatsDescriptor, IEnrichStatsRequest>(new EnrichStatsDescriptor()));
    }

    public Task<EnrichStatsResponse> StatsAsync(
      Func<EnrichStatsDescriptor, IEnrichStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StatsAsync(selector.InvokeOrDefault<EnrichStatsDescriptor, IEnrichStatsRequest>(new EnrichStatsDescriptor()), ct);
    }

    public EnrichStatsResponse Stats(IEnrichStatsRequest request) => this.DoRequest<IEnrichStatsRequest, EnrichStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<EnrichStatsResponse> StatsAsync(IEnrichStatsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IEnrichStatsRequest, EnrichStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
  }
}
