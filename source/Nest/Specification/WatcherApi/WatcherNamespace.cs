// Decompiled with JetBrains decompiler
// Type: Nest.Specification.WatcherApi.WatcherNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.WatcherApi
{
  public class WatcherNamespace : Nest.NamespacedClientProxy
  {
    internal WatcherNamespace(ElasticClient client)
      : base(client)
    {
    }

    public AcknowledgeWatchResponse Acknowledge(
      Id watchId,
      Func<AcknowledgeWatchDescriptor, IAcknowledgeWatchRequest> selector = null)
    {
      return this.Acknowledge(selector.InvokeOrDefault<AcknowledgeWatchDescriptor, IAcknowledgeWatchRequest>(new AcknowledgeWatchDescriptor(watchId)));
    }

    public Task<AcknowledgeWatchResponse> AcknowledgeAsync(
      Id watchId,
      Func<AcknowledgeWatchDescriptor, IAcknowledgeWatchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.AcknowledgeAsync(selector.InvokeOrDefault<AcknowledgeWatchDescriptor, IAcknowledgeWatchRequest>(new AcknowledgeWatchDescriptor(watchId)), ct);
    }

    public AcknowledgeWatchResponse Acknowledge(IAcknowledgeWatchRequest request) => this.DoRequest<IAcknowledgeWatchRequest, AcknowledgeWatchResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<AcknowledgeWatchResponse> AcknowledgeAsync(
      IAcknowledgeWatchRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IAcknowledgeWatchRequest, AcknowledgeWatchResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ActivateWatchResponse Activate(
      Id watchId,
      Func<ActivateWatchDescriptor, IActivateWatchRequest> selector = null)
    {
      return this.Activate(selector.InvokeOrDefault<ActivateWatchDescriptor, IActivateWatchRequest>(new ActivateWatchDescriptor(watchId)));
    }

    public Task<ActivateWatchResponse> ActivateAsync(
      Id watchId,
      Func<ActivateWatchDescriptor, IActivateWatchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ActivateAsync(selector.InvokeOrDefault<ActivateWatchDescriptor, IActivateWatchRequest>(new ActivateWatchDescriptor(watchId)), ct);
    }

    public ActivateWatchResponse Activate(IActivateWatchRequest request) => this.DoRequest<IActivateWatchRequest, ActivateWatchResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ActivateWatchResponse> ActivateAsync(
      IActivateWatchRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IActivateWatchRequest, ActivateWatchResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeactivateWatchResponse Deactivate(
      Id watchId,
      Func<DeactivateWatchDescriptor, IDeactivateWatchRequest> selector = null)
    {
      return this.Deactivate(selector.InvokeOrDefault<DeactivateWatchDescriptor, IDeactivateWatchRequest>(new DeactivateWatchDescriptor(watchId)));
    }

    public Task<DeactivateWatchResponse> DeactivateAsync(
      Id watchId,
      Func<DeactivateWatchDescriptor, IDeactivateWatchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeactivateAsync(selector.InvokeOrDefault<DeactivateWatchDescriptor, IDeactivateWatchRequest>(new DeactivateWatchDescriptor(watchId)), ct);
    }

    public DeactivateWatchResponse Deactivate(IDeactivateWatchRequest request) => this.DoRequest<IDeactivateWatchRequest, DeactivateWatchResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeactivateWatchResponse> DeactivateAsync(
      IDeactivateWatchRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeactivateWatchRequest, DeactivateWatchResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteWatchResponse Delete(
      Id id,
      Func<DeleteWatchDescriptor, IDeleteWatchRequest> selector = null)
    {
      return this.Delete(selector.InvokeOrDefault<DeleteWatchDescriptor, IDeleteWatchRequest>(new DeleteWatchDescriptor(id)));
    }

    public Task<DeleteWatchResponse> DeleteAsync(
      Id id,
      Func<DeleteWatchDescriptor, IDeleteWatchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteAsync(selector.InvokeOrDefault<DeleteWatchDescriptor, IDeleteWatchRequest>(new DeleteWatchDescriptor(id)), ct);
    }

    public DeleteWatchResponse Delete(IDeleteWatchRequest request) => this.DoRequest<IDeleteWatchRequest, DeleteWatchResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteWatchResponse> DeleteAsync(IDeleteWatchRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IDeleteWatchRequest, DeleteWatchResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ExecuteWatchResponse Execute(
      Func<ExecuteWatchDescriptor, IExecuteWatchRequest> selector = null)
    {
      return this.Execute(selector.InvokeOrDefault<ExecuteWatchDescriptor, IExecuteWatchRequest>(new ExecuteWatchDescriptor()));
    }

    public Task<ExecuteWatchResponse> ExecuteAsync(
      Func<ExecuteWatchDescriptor, IExecuteWatchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ExecuteAsync(selector.InvokeOrDefault<ExecuteWatchDescriptor, IExecuteWatchRequest>(new ExecuteWatchDescriptor()), ct);
    }

    public ExecuteWatchResponse Execute(IExecuteWatchRequest request) => this.DoRequest<IExecuteWatchRequest, ExecuteWatchResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExecuteWatchResponse> ExecuteAsync(
      IExecuteWatchRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IExecuteWatchRequest, ExecuteWatchResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetWatchResponse Get(
      Id id,
      Func<GetWatchDescriptor, IGetWatchRequest> selector = null)
    {
      return this.Get(selector.InvokeOrDefault<GetWatchDescriptor, IGetWatchRequest>(new GetWatchDescriptor(id)));
    }

    public Task<GetWatchResponse> GetAsync(
      Id id,
      Func<GetWatchDescriptor, IGetWatchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetAsync(selector.InvokeOrDefault<GetWatchDescriptor, IGetWatchRequest>(new GetWatchDescriptor(id)), ct);
    }

    public GetWatchResponse Get(IGetWatchRequest request) => this.DoRequest<IGetWatchRequest, GetWatchResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetWatchResponse> GetAsync(IGetWatchRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetWatchRequest, GetWatchResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public PutWatchResponse Put(
      Id id,
      Func<PutWatchDescriptor, IPutWatchRequest> selector = null)
    {
      return this.Put(selector.InvokeOrDefault<PutWatchDescriptor, IPutWatchRequest>(new PutWatchDescriptor(id)));
    }

    public Task<PutWatchResponse> PutAsync(
      Id id,
      Func<PutWatchDescriptor, IPutWatchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutAsync(selector.InvokeOrDefault<PutWatchDescriptor, IPutWatchRequest>(new PutWatchDescriptor(id)), ct);
    }

    public PutWatchResponse Put(IPutWatchRequest request) => this.DoRequest<IPutWatchRequest, PutWatchResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutWatchResponse> PutAsync(IPutWatchRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPutWatchRequest, PutWatchResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public QueryWatchesResponse QueryWatches(
      Func<QueryWatchesDescriptor, IQueryWatchesRequest> selector = null)
    {
      return this.QueryWatches(selector.InvokeOrDefault<QueryWatchesDescriptor, IQueryWatchesRequest>(new QueryWatchesDescriptor()));
    }

    public Task<QueryWatchesResponse> QueryWatchesAsync(
      Func<QueryWatchesDescriptor, IQueryWatchesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.QueryWatchesAsync(selector.InvokeOrDefault<QueryWatchesDescriptor, IQueryWatchesRequest>(new QueryWatchesDescriptor()), ct);
    }

    public QueryWatchesResponse QueryWatches(IQueryWatchesRequest request) => this.DoRequest<IQueryWatchesRequest, QueryWatchesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<QueryWatchesResponse> QueryWatchesAsync(
      IQueryWatchesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IQueryWatchesRequest, QueryWatchesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public StartWatcherResponse Start(
      Func<StartWatcherDescriptor, IStartWatcherRequest> selector = null)
    {
      return this.Start(selector.InvokeOrDefault<StartWatcherDescriptor, IStartWatcherRequest>(new StartWatcherDescriptor()));
    }

    public Task<StartWatcherResponse> StartAsync(
      Func<StartWatcherDescriptor, IStartWatcherRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StartAsync(selector.InvokeOrDefault<StartWatcherDescriptor, IStartWatcherRequest>(new StartWatcherDescriptor()), ct);
    }

    public StartWatcherResponse Start(IStartWatcherRequest request) => this.DoRequest<IStartWatcherRequest, StartWatcherResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StartWatcherResponse> StartAsync(IStartWatcherRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IStartWatcherRequest, StartWatcherResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public WatcherStatsResponse Stats(
      Func<WatcherStatsDescriptor, IWatcherStatsRequest> selector = null)
    {
      return this.Stats(selector.InvokeOrDefault<WatcherStatsDescriptor, IWatcherStatsRequest>(new WatcherStatsDescriptor()));
    }

    public Task<WatcherStatsResponse> StatsAsync(
      Func<WatcherStatsDescriptor, IWatcherStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StatsAsync(selector.InvokeOrDefault<WatcherStatsDescriptor, IWatcherStatsRequest>(new WatcherStatsDescriptor()), ct);
    }

    public WatcherStatsResponse Stats(IWatcherStatsRequest request) => this.DoRequest<IWatcherStatsRequest, WatcherStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<WatcherStatsResponse> StatsAsync(IWatcherStatsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IWatcherStatsRequest, WatcherStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public StopWatcherResponse Stop(
      Func<StopWatcherDescriptor, IStopWatcherRequest> selector = null)
    {
      return this.Stop(selector.InvokeOrDefault<StopWatcherDescriptor, IStopWatcherRequest>(new StopWatcherDescriptor()));
    }

    public Task<StopWatcherResponse> StopAsync(
      Func<StopWatcherDescriptor, IStopWatcherRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StopAsync(selector.InvokeOrDefault<StopWatcherDescriptor, IStopWatcherRequest>(new StopWatcherDescriptor()), ct);
    }

    public StopWatcherResponse Stop(IStopWatcherRequest request) => this.DoRequest<IStopWatcherRequest, StopWatcherResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StopWatcherResponse> StopAsync(IStopWatcherRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IStopWatcherRequest, StopWatcherResponse>(request, (IRequestParameters) request.RequestParameters, ct);
  }
}
