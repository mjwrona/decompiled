// Decompiled with JetBrains decompiler
// Type: Nest.Specification.SnapshotLifecycleManagementApi.SnapshotLifecycleManagementNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.SnapshotLifecycleManagementApi
{
  public class SnapshotLifecycleManagementNamespace : Nest.NamespacedClientProxy
  {
    internal SnapshotLifecycleManagementNamespace(ElasticClient client)
      : base(client)
    {
    }

    public DeleteSnapshotLifecycleResponse DeleteSnapshotLifecycle(
      Id policyId,
      Func<DeleteSnapshotLifecycleDescriptor, IDeleteSnapshotLifecycleRequest> selector = null)
    {
      return this.DeleteSnapshotLifecycle(selector.InvokeOrDefault<DeleteSnapshotLifecycleDescriptor, IDeleteSnapshotLifecycleRequest>(new DeleteSnapshotLifecycleDescriptor(policyId)));
    }

    public Task<DeleteSnapshotLifecycleResponse> DeleteSnapshotLifecycleAsync(
      Id policyId,
      Func<DeleteSnapshotLifecycleDescriptor, IDeleteSnapshotLifecycleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteSnapshotLifecycleAsync(selector.InvokeOrDefault<DeleteSnapshotLifecycleDescriptor, IDeleteSnapshotLifecycleRequest>(new DeleteSnapshotLifecycleDescriptor(policyId)), ct);
    }

    public DeleteSnapshotLifecycleResponse DeleteSnapshotLifecycle(
      IDeleteSnapshotLifecycleRequest request)
    {
      return this.DoRequest<IDeleteSnapshotLifecycleRequest, DeleteSnapshotLifecycleResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<DeleteSnapshotLifecycleResponse> DeleteSnapshotLifecycleAsync(
      IDeleteSnapshotLifecycleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteSnapshotLifecycleRequest, DeleteSnapshotLifecycleResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExecuteSnapshotLifecycleResponse ExecuteSnapshotLifecycle(
      Id policyId,
      Func<ExecuteSnapshotLifecycleDescriptor, IExecuteSnapshotLifecycleRequest> selector = null)
    {
      return this.ExecuteSnapshotLifecycle(selector.InvokeOrDefault<ExecuteSnapshotLifecycleDescriptor, IExecuteSnapshotLifecycleRequest>(new ExecuteSnapshotLifecycleDescriptor(policyId)));
    }

    public Task<ExecuteSnapshotLifecycleResponse> ExecuteSnapshotLifecycleAsync(
      Id policyId,
      Func<ExecuteSnapshotLifecycleDescriptor, IExecuteSnapshotLifecycleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ExecuteSnapshotLifecycleAsync(selector.InvokeOrDefault<ExecuteSnapshotLifecycleDescriptor, IExecuteSnapshotLifecycleRequest>(new ExecuteSnapshotLifecycleDescriptor(policyId)), ct);
    }

    public ExecuteSnapshotLifecycleResponse ExecuteSnapshotLifecycle(
      IExecuteSnapshotLifecycleRequest request)
    {
      return this.DoRequest<IExecuteSnapshotLifecycleRequest, ExecuteSnapshotLifecycleResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<ExecuteSnapshotLifecycleResponse> ExecuteSnapshotLifecycleAsync(
      IExecuteSnapshotLifecycleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IExecuteSnapshotLifecycleRequest, ExecuteSnapshotLifecycleResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExecuteRetentionResponse ExecuteRetention(
      Func<ExecuteRetentionDescriptor, IExecuteRetentionRequest> selector = null)
    {
      return this.ExecuteRetention(selector.InvokeOrDefault<ExecuteRetentionDescriptor, IExecuteRetentionRequest>(new ExecuteRetentionDescriptor()));
    }

    public Task<ExecuteRetentionResponse> ExecuteRetentionAsync(
      Func<ExecuteRetentionDescriptor, IExecuteRetentionRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ExecuteRetentionAsync(selector.InvokeOrDefault<ExecuteRetentionDescriptor, IExecuteRetentionRequest>(new ExecuteRetentionDescriptor()), ct);
    }

    public ExecuteRetentionResponse ExecuteRetention(IExecuteRetentionRequest request) => this.DoRequest<IExecuteRetentionRequest, ExecuteRetentionResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExecuteRetentionResponse> ExecuteRetentionAsync(
      IExecuteRetentionRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IExecuteRetentionRequest, ExecuteRetentionResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetSnapshotLifecycleResponse GetSnapshotLifecycle(
      Func<GetSnapshotLifecycleDescriptor, IGetSnapshotLifecycleRequest> selector = null)
    {
      return this.GetSnapshotLifecycle(selector.InvokeOrDefault<GetSnapshotLifecycleDescriptor, IGetSnapshotLifecycleRequest>(new GetSnapshotLifecycleDescriptor()));
    }

    public Task<GetSnapshotLifecycleResponse> GetSnapshotLifecycleAsync(
      Func<GetSnapshotLifecycleDescriptor, IGetSnapshotLifecycleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetSnapshotLifecycleAsync(selector.InvokeOrDefault<GetSnapshotLifecycleDescriptor, IGetSnapshotLifecycleRequest>(new GetSnapshotLifecycleDescriptor()), ct);
    }

    public GetSnapshotLifecycleResponse GetSnapshotLifecycle(IGetSnapshotLifecycleRequest request) => this.DoRequest<IGetSnapshotLifecycleRequest, GetSnapshotLifecycleResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetSnapshotLifecycleResponse> GetSnapshotLifecycleAsync(
      IGetSnapshotLifecycleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetSnapshotLifecycleRequest, GetSnapshotLifecycleResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetSnapshotLifecycleStatsResponse GetSnapshotLifecycleStats(
      Func<GetSnapshotLifecycleStatsDescriptor, IGetSnapshotLifecycleStatsRequest> selector = null)
    {
      return this.GetSnapshotLifecycleStats(selector.InvokeOrDefault<GetSnapshotLifecycleStatsDescriptor, IGetSnapshotLifecycleStatsRequest>(new GetSnapshotLifecycleStatsDescriptor()));
    }

    public Task<GetSnapshotLifecycleStatsResponse> GetSnapshotLifecycleStatsAsync(
      Func<GetSnapshotLifecycleStatsDescriptor, IGetSnapshotLifecycleStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetSnapshotLifecycleStatsAsync(selector.InvokeOrDefault<GetSnapshotLifecycleStatsDescriptor, IGetSnapshotLifecycleStatsRequest>(new GetSnapshotLifecycleStatsDescriptor()), ct);
    }

    public GetSnapshotLifecycleStatsResponse GetSnapshotLifecycleStats(
      IGetSnapshotLifecycleStatsRequest request)
    {
      return this.DoRequest<IGetSnapshotLifecycleStatsRequest, GetSnapshotLifecycleStatsResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<GetSnapshotLifecycleStatsResponse> GetSnapshotLifecycleStatsAsync(
      IGetSnapshotLifecycleStatsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetSnapshotLifecycleStatsRequest, GetSnapshotLifecycleStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetSnapshotLifecycleManagementStatusResponse GetStatus(
      Func<GetSnapshotLifecycleManagementStatusDescriptor, IGetSnapshotLifecycleManagementStatusRequest> selector = null)
    {
      return this.GetStatus(selector.InvokeOrDefault<GetSnapshotLifecycleManagementStatusDescriptor, IGetSnapshotLifecycleManagementStatusRequest>(new GetSnapshotLifecycleManagementStatusDescriptor()));
    }

    public Task<GetSnapshotLifecycleManagementStatusResponse> GetStatusAsync(
      Func<GetSnapshotLifecycleManagementStatusDescriptor, IGetSnapshotLifecycleManagementStatusRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetStatusAsync(selector.InvokeOrDefault<GetSnapshotLifecycleManagementStatusDescriptor, IGetSnapshotLifecycleManagementStatusRequest>(new GetSnapshotLifecycleManagementStatusDescriptor()), ct);
    }

    public GetSnapshotLifecycleManagementStatusResponse GetStatus(
      IGetSnapshotLifecycleManagementStatusRequest request)
    {
      return this.DoRequest<IGetSnapshotLifecycleManagementStatusRequest, GetSnapshotLifecycleManagementStatusResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<GetSnapshotLifecycleManagementStatusResponse> GetStatusAsync(
      IGetSnapshotLifecycleManagementStatusRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetSnapshotLifecycleManagementStatusRequest, GetSnapshotLifecycleManagementStatusResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutSnapshotLifecycleResponse PutSnapshotLifecycle(
      Id policyId,
      Func<PutSnapshotLifecycleDescriptor, IPutSnapshotLifecycleRequest> selector = null)
    {
      return this.PutSnapshotLifecycle(selector.InvokeOrDefault<PutSnapshotLifecycleDescriptor, IPutSnapshotLifecycleRequest>(new PutSnapshotLifecycleDescriptor(policyId)));
    }

    public Task<PutSnapshotLifecycleResponse> PutSnapshotLifecycleAsync(
      Id policyId,
      Func<PutSnapshotLifecycleDescriptor, IPutSnapshotLifecycleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutSnapshotLifecycleAsync(selector.InvokeOrDefault<PutSnapshotLifecycleDescriptor, IPutSnapshotLifecycleRequest>(new PutSnapshotLifecycleDescriptor(policyId)), ct);
    }

    public PutSnapshotLifecycleResponse PutSnapshotLifecycle(IPutSnapshotLifecycleRequest request) => this.DoRequest<IPutSnapshotLifecycleRequest, PutSnapshotLifecycleResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutSnapshotLifecycleResponse> PutSnapshotLifecycleAsync(
      IPutSnapshotLifecycleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutSnapshotLifecycleRequest, PutSnapshotLifecycleResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public StartSnapshotLifecycleManagementResponse Start(
      Func<StartSnapshotLifecycleManagementDescriptor, IStartSnapshotLifecycleManagementRequest> selector = null)
    {
      return this.Start(selector.InvokeOrDefault<StartSnapshotLifecycleManagementDescriptor, IStartSnapshotLifecycleManagementRequest>(new StartSnapshotLifecycleManagementDescriptor()));
    }

    public Task<StartSnapshotLifecycleManagementResponse> StartAsync(
      Func<StartSnapshotLifecycleManagementDescriptor, IStartSnapshotLifecycleManagementRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StartAsync(selector.InvokeOrDefault<StartSnapshotLifecycleManagementDescriptor, IStartSnapshotLifecycleManagementRequest>(new StartSnapshotLifecycleManagementDescriptor()), ct);
    }

    public StartSnapshotLifecycleManagementResponse Start(
      IStartSnapshotLifecycleManagementRequest request)
    {
      return this.DoRequest<IStartSnapshotLifecycleManagementRequest, StartSnapshotLifecycleManagementResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<StartSnapshotLifecycleManagementResponse> StartAsync(
      IStartSnapshotLifecycleManagementRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IStartSnapshotLifecycleManagementRequest, StartSnapshotLifecycleManagementResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public StopSnapshotLifecycleManagementResponse Stop(
      Func<StopSnapshotLifecycleManagementDescriptor, IStopSnapshotLifecycleManagementRequest> selector = null)
    {
      return this.Stop(selector.InvokeOrDefault<StopSnapshotLifecycleManagementDescriptor, IStopSnapshotLifecycleManagementRequest>(new StopSnapshotLifecycleManagementDescriptor()));
    }

    public Task<StopSnapshotLifecycleManagementResponse> StopAsync(
      Func<StopSnapshotLifecycleManagementDescriptor, IStopSnapshotLifecycleManagementRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StopAsync(selector.InvokeOrDefault<StopSnapshotLifecycleManagementDescriptor, IStopSnapshotLifecycleManagementRequest>(new StopSnapshotLifecycleManagementDescriptor()), ct);
    }

    public StopSnapshotLifecycleManagementResponse Stop(
      IStopSnapshotLifecycleManagementRequest request)
    {
      return this.DoRequest<IStopSnapshotLifecycleManagementRequest, StopSnapshotLifecycleManagementResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<StopSnapshotLifecycleManagementResponse> StopAsync(
      IStopSnapshotLifecycleManagementRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IStopSnapshotLifecycleManagementRequest, StopSnapshotLifecycleManagementResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
