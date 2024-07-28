// Decompiled with JetBrains decompiler
// Type: Nest.Specification.IndexLifecycleManagementApi.IndexLifecycleManagementNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.IndexLifecycleManagementApi
{
  public class IndexLifecycleManagementNamespace : Nest.NamespacedClientProxy
  {
    internal IndexLifecycleManagementNamespace(ElasticClient client)
      : base(client)
    {
    }

    public DeleteLifecycleResponse DeleteLifecycle(
      Id policyId,
      Func<DeleteLifecycleDescriptor, IDeleteLifecycleRequest> selector = null)
    {
      return this.DeleteLifecycle(selector.InvokeOrDefault<DeleteLifecycleDescriptor, IDeleteLifecycleRequest>(new DeleteLifecycleDescriptor(policyId)));
    }

    public Task<DeleteLifecycleResponse> DeleteLifecycleAsync(
      Id policyId,
      Func<DeleteLifecycleDescriptor, IDeleteLifecycleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteLifecycleAsync(selector.InvokeOrDefault<DeleteLifecycleDescriptor, IDeleteLifecycleRequest>(new DeleteLifecycleDescriptor(policyId)), ct);
    }

    public DeleteLifecycleResponse DeleteLifecycle(IDeleteLifecycleRequest request) => this.DoRequest<IDeleteLifecycleRequest, DeleteLifecycleResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteLifecycleResponse> DeleteLifecycleAsync(
      IDeleteLifecycleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteLifecycleRequest, DeleteLifecycleResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExplainLifecycleResponse ExplainLifecycle(
      IndexName index,
      Func<ExplainLifecycleDescriptor, IExplainLifecycleRequest> selector = null)
    {
      return this.ExplainLifecycle(selector.InvokeOrDefault<ExplainLifecycleDescriptor, IExplainLifecycleRequest>(new ExplainLifecycleDescriptor(index)));
    }

    public Task<ExplainLifecycleResponse> ExplainLifecycleAsync(
      IndexName index,
      Func<ExplainLifecycleDescriptor, IExplainLifecycleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ExplainLifecycleAsync(selector.InvokeOrDefault<ExplainLifecycleDescriptor, IExplainLifecycleRequest>(new ExplainLifecycleDescriptor(index)), ct);
    }

    public ExplainLifecycleResponse ExplainLifecycle(IExplainLifecycleRequest request) => this.DoRequest<IExplainLifecycleRequest, ExplainLifecycleResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExplainLifecycleResponse> ExplainLifecycleAsync(
      IExplainLifecycleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IExplainLifecycleRequest, ExplainLifecycleResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetLifecycleResponse GetLifecycle(
      Func<GetLifecycleDescriptor, IGetLifecycleRequest> selector = null)
    {
      return this.GetLifecycle(selector.InvokeOrDefault<GetLifecycleDescriptor, IGetLifecycleRequest>(new GetLifecycleDescriptor()));
    }

    public Task<GetLifecycleResponse> GetLifecycleAsync(
      Func<GetLifecycleDescriptor, IGetLifecycleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetLifecycleAsync(selector.InvokeOrDefault<GetLifecycleDescriptor, IGetLifecycleRequest>(new GetLifecycleDescriptor()), ct);
    }

    public GetLifecycleResponse GetLifecycle(IGetLifecycleRequest request) => this.DoRequest<IGetLifecycleRequest, GetLifecycleResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetLifecycleResponse> GetLifecycleAsync(
      IGetLifecycleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetLifecycleRequest, GetLifecycleResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetIlmStatusResponse GetStatus(
      Func<GetIlmStatusDescriptor, IGetIlmStatusRequest> selector = null)
    {
      return this.GetStatus(selector.InvokeOrDefault<GetIlmStatusDescriptor, IGetIlmStatusRequest>(new GetIlmStatusDescriptor()));
    }

    public Task<GetIlmStatusResponse> GetStatusAsync(
      Func<GetIlmStatusDescriptor, IGetIlmStatusRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetStatusAsync(selector.InvokeOrDefault<GetIlmStatusDescriptor, IGetIlmStatusRequest>(new GetIlmStatusDescriptor()), ct);
    }

    public GetIlmStatusResponse GetStatus(IGetIlmStatusRequest request) => this.DoRequest<IGetIlmStatusRequest, GetIlmStatusResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetIlmStatusResponse> GetStatusAsync(
      IGetIlmStatusRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetIlmStatusRequest, GetIlmStatusResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public MigrateToDataTiersResponse MigrateToDataTiers(
      Func<MigrateToDataTiersDescriptor, IMigrateToDataTiersRequest> selector = null)
    {
      return this.MigrateToDataTiers(selector.InvokeOrDefault<MigrateToDataTiersDescriptor, IMigrateToDataTiersRequest>(new MigrateToDataTiersDescriptor()));
    }

    public Task<MigrateToDataTiersResponse> MigrateToDataTiersAsync(
      Func<MigrateToDataTiersDescriptor, IMigrateToDataTiersRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.MigrateToDataTiersAsync(selector.InvokeOrDefault<MigrateToDataTiersDescriptor, IMigrateToDataTiersRequest>(new MigrateToDataTiersDescriptor()), ct);
    }

    public MigrateToDataTiersResponse MigrateToDataTiers(IMigrateToDataTiersRequest request) => this.DoRequest<IMigrateToDataTiersRequest, MigrateToDataTiersResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<MigrateToDataTiersResponse> MigrateToDataTiersAsync(
      IMigrateToDataTiersRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IMigrateToDataTiersRequest, MigrateToDataTiersResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public MoveToStepResponse MoveToStep(
      IndexName index,
      Func<MoveToStepDescriptor, IMoveToStepRequest> selector = null)
    {
      return this.MoveToStep(selector.InvokeOrDefault<MoveToStepDescriptor, IMoveToStepRequest>(new MoveToStepDescriptor(index)));
    }

    public Task<MoveToStepResponse> MoveToStepAsync(
      IndexName index,
      Func<MoveToStepDescriptor, IMoveToStepRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.MoveToStepAsync(selector.InvokeOrDefault<MoveToStepDescriptor, IMoveToStepRequest>(new MoveToStepDescriptor(index)), ct);
    }

    public MoveToStepResponse MoveToStep(IMoveToStepRequest request) => this.DoRequest<IMoveToStepRequest, MoveToStepResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<MoveToStepResponse> MoveToStepAsync(
      IMoveToStepRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IMoveToStepRequest, MoveToStepResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutLifecycleResponse PutLifecycle(
      Id policyId,
      Func<PutLifecycleDescriptor, IPutLifecycleRequest> selector = null)
    {
      return this.PutLifecycle(selector.InvokeOrDefault<PutLifecycleDescriptor, IPutLifecycleRequest>(new PutLifecycleDescriptor(policyId)));
    }

    public Task<PutLifecycleResponse> PutLifecycleAsync(
      Id policyId,
      Func<PutLifecycleDescriptor, IPutLifecycleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutLifecycleAsync(selector.InvokeOrDefault<PutLifecycleDescriptor, IPutLifecycleRequest>(new PutLifecycleDescriptor(policyId)), ct);
    }

    public PutLifecycleResponse PutLifecycle(IPutLifecycleRequest request) => this.DoRequest<IPutLifecycleRequest, PutLifecycleResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutLifecycleResponse> PutLifecycleAsync(
      IPutLifecycleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutLifecycleRequest, PutLifecycleResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public RemovePolicyResponse RemovePolicy(
      IndexName index,
      Func<RemovePolicyDescriptor, IRemovePolicyRequest> selector = null)
    {
      return this.RemovePolicy(selector.InvokeOrDefault<RemovePolicyDescriptor, IRemovePolicyRequest>(new RemovePolicyDescriptor(index)));
    }

    public Task<RemovePolicyResponse> RemovePolicyAsync(
      IndexName index,
      Func<RemovePolicyDescriptor, IRemovePolicyRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RemovePolicyAsync(selector.InvokeOrDefault<RemovePolicyDescriptor, IRemovePolicyRequest>(new RemovePolicyDescriptor(index)), ct);
    }

    public RemovePolicyResponse RemovePolicy(IRemovePolicyRequest request) => this.DoRequest<IRemovePolicyRequest, RemovePolicyResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<RemovePolicyResponse> RemovePolicyAsync(
      IRemovePolicyRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IRemovePolicyRequest, RemovePolicyResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public RetryIlmResponse Retry(
      IndexName index,
      Func<RetryIlmDescriptor, IRetryIlmRequest> selector = null)
    {
      return this.Retry(selector.InvokeOrDefault<RetryIlmDescriptor, IRetryIlmRequest>(new RetryIlmDescriptor(index)));
    }

    public Task<RetryIlmResponse> RetryAsync(
      IndexName index,
      Func<RetryIlmDescriptor, IRetryIlmRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RetryAsync(selector.InvokeOrDefault<RetryIlmDescriptor, IRetryIlmRequest>(new RetryIlmDescriptor(index)), ct);
    }

    public RetryIlmResponse Retry(IRetryIlmRequest request) => this.DoRequest<IRetryIlmRequest, RetryIlmResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<RetryIlmResponse> RetryAsync(IRetryIlmRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IRetryIlmRequest, RetryIlmResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public StartIlmResponse Start(
      Func<StartIlmDescriptor, IStartIlmRequest> selector = null)
    {
      return this.Start(selector.InvokeOrDefault<StartIlmDescriptor, IStartIlmRequest>(new StartIlmDescriptor()));
    }

    public Task<StartIlmResponse> StartAsync(
      Func<StartIlmDescriptor, IStartIlmRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StartAsync(selector.InvokeOrDefault<StartIlmDescriptor, IStartIlmRequest>(new StartIlmDescriptor()), ct);
    }

    public StartIlmResponse Start(IStartIlmRequest request) => this.DoRequest<IStartIlmRequest, StartIlmResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StartIlmResponse> StartAsync(IStartIlmRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IStartIlmRequest, StartIlmResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public StopIlmResponse Stop(Func<StopIlmDescriptor, IStopIlmRequest> selector = null) => this.Stop(selector.InvokeOrDefault<StopIlmDescriptor, IStopIlmRequest>(new StopIlmDescriptor()));

    public Task<StopIlmResponse> StopAsync(
      Func<StopIlmDescriptor, IStopIlmRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StopAsync(selector.InvokeOrDefault<StopIlmDescriptor, IStopIlmRequest>(new StopIlmDescriptor()), ct);
    }

    public StopIlmResponse Stop(IStopIlmRequest request) => this.DoRequest<IStopIlmRequest, StopIlmResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StopIlmResponse> StopAsync(IStopIlmRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IStopIlmRequest, StopIlmResponse>(request, (IRequestParameters) request.RequestParameters, ct);
  }
}
