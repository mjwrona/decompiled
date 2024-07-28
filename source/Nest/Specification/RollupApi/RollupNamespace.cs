// Decompiled with JetBrains decompiler
// Type: Nest.Specification.RollupApi.RollupNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.RollupApi
{
  public class RollupNamespace : Nest.NamespacedClientProxy
  {
    internal RollupNamespace(ElasticClient client)
      : base(client)
    {
    }

    public DeleteRollupJobResponse DeleteJob(
      Id id,
      Func<DeleteRollupJobDescriptor, IDeleteRollupJobRequest> selector = null)
    {
      return this.DeleteJob(selector.InvokeOrDefault<DeleteRollupJobDescriptor, IDeleteRollupJobRequest>(new DeleteRollupJobDescriptor(id)));
    }

    public Task<DeleteRollupJobResponse> DeleteJobAsync(
      Id id,
      Func<DeleteRollupJobDescriptor, IDeleteRollupJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteJobAsync(selector.InvokeOrDefault<DeleteRollupJobDescriptor, IDeleteRollupJobRequest>(new DeleteRollupJobDescriptor(id)), ct);
    }

    public DeleteRollupJobResponse DeleteJob(IDeleteRollupJobRequest request) => this.DoRequest<IDeleteRollupJobRequest, DeleteRollupJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteRollupJobResponse> DeleteJobAsync(
      IDeleteRollupJobRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteRollupJobRequest, DeleteRollupJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetRollupJobResponse GetJob(
      Func<GetRollupJobDescriptor, IGetRollupJobRequest> selector = null)
    {
      return this.GetJob(selector.InvokeOrDefault<GetRollupJobDescriptor, IGetRollupJobRequest>(new GetRollupJobDescriptor()));
    }

    public Task<GetRollupJobResponse> GetJobAsync(
      Func<GetRollupJobDescriptor, IGetRollupJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetJobAsync(selector.InvokeOrDefault<GetRollupJobDescriptor, IGetRollupJobRequest>(new GetRollupJobDescriptor()), ct);
    }

    public GetRollupJobResponse GetJob(IGetRollupJobRequest request) => this.DoRequest<IGetRollupJobRequest, GetRollupJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetRollupJobResponse> GetJobAsync(
      IGetRollupJobRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetRollupJobRequest, GetRollupJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetRollupCapabilitiesResponse GetCapabilities(
      Func<GetRollupCapabilitiesDescriptor, IGetRollupCapabilitiesRequest> selector = null)
    {
      return this.GetCapabilities(selector.InvokeOrDefault<GetRollupCapabilitiesDescriptor, IGetRollupCapabilitiesRequest>(new GetRollupCapabilitiesDescriptor()));
    }

    public Task<GetRollupCapabilitiesResponse> GetCapabilitiesAsync(
      Func<GetRollupCapabilitiesDescriptor, IGetRollupCapabilitiesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetCapabilitiesAsync(selector.InvokeOrDefault<GetRollupCapabilitiesDescriptor, IGetRollupCapabilitiesRequest>(new GetRollupCapabilitiesDescriptor()), ct);
    }

    public GetRollupCapabilitiesResponse GetCapabilities(IGetRollupCapabilitiesRequest request) => this.DoRequest<IGetRollupCapabilitiesRequest, GetRollupCapabilitiesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetRollupCapabilitiesResponse> GetCapabilitiesAsync(
      IGetRollupCapabilitiesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetRollupCapabilitiesRequest, GetRollupCapabilitiesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetRollupIndexCapabilitiesResponse GetIndexCapabilities(
      IndexName index,
      Func<GetRollupIndexCapabilitiesDescriptor, IGetRollupIndexCapabilitiesRequest> selector = null)
    {
      return this.GetIndexCapabilities(selector.InvokeOrDefault<GetRollupIndexCapabilitiesDescriptor, IGetRollupIndexCapabilitiesRequest>(new GetRollupIndexCapabilitiesDescriptor(index)));
    }

    public Task<GetRollupIndexCapabilitiesResponse> GetIndexCapabilitiesAsync(
      IndexName index,
      Func<GetRollupIndexCapabilitiesDescriptor, IGetRollupIndexCapabilitiesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetIndexCapabilitiesAsync(selector.InvokeOrDefault<GetRollupIndexCapabilitiesDescriptor, IGetRollupIndexCapabilitiesRequest>(new GetRollupIndexCapabilitiesDescriptor(index)), ct);
    }

    public GetRollupIndexCapabilitiesResponse GetIndexCapabilities(
      IGetRollupIndexCapabilitiesRequest request)
    {
      return this.DoRequest<IGetRollupIndexCapabilitiesRequest, GetRollupIndexCapabilitiesResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<GetRollupIndexCapabilitiesResponse> GetIndexCapabilitiesAsync(
      IGetRollupIndexCapabilitiesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetRollupIndexCapabilitiesRequest, GetRollupIndexCapabilitiesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public CreateRollupJobResponse CreateJob<TDocument>(
      Id id,
      Func<CreateRollupJobDescriptor<TDocument>, ICreateRollupJobRequest> selector)
      where TDocument : class
    {
      return this.CreateJob(selector.InvokeOrDefault<CreateRollupJobDescriptor<TDocument>, ICreateRollupJobRequest>(new CreateRollupJobDescriptor<TDocument>(id)));
    }

    public Task<CreateRollupJobResponse> CreateJobAsync<TDocument>(
      Id id,
      Func<CreateRollupJobDescriptor<TDocument>, ICreateRollupJobRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.CreateJobAsync(selector.InvokeOrDefault<CreateRollupJobDescriptor<TDocument>, ICreateRollupJobRequest>(new CreateRollupJobDescriptor<TDocument>(id)), ct);
    }

    public CreateRollupJobResponse CreateJob(ICreateRollupJobRequest request) => this.DoRequest<ICreateRollupJobRequest, CreateRollupJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CreateRollupJobResponse> CreateJobAsync(
      ICreateRollupJobRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ICreateRollupJobRequest, CreateRollupJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public RollupSearchResponse<TDocument> Search<TDocument>(
      Func<RollupSearchDescriptor<TDocument>, IRollupSearchRequest> selector = null)
      where TDocument : class
    {
      return this.Search<TDocument>(selector.InvokeOrDefault<RollupSearchDescriptor<TDocument>, IRollupSearchRequest>(new RollupSearchDescriptor<TDocument>()));
    }

    public Task<RollupSearchResponse<TDocument>> SearchAsync<TDocument>(
      Func<RollupSearchDescriptor<TDocument>, IRollupSearchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.SearchAsync<TDocument>(selector.InvokeOrDefault<RollupSearchDescriptor<TDocument>, IRollupSearchRequest>(new RollupSearchDescriptor<TDocument>()), ct);
    }

    public RollupSearchResponse<TDocument> Search<TDocument>(IRollupSearchRequest request) where TDocument : class => this.DoRequest<IRollupSearchRequest, RollupSearchResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);

    public Task<RollupSearchResponse<TDocument>> SearchAsync<TDocument>(
      IRollupSearchRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<IRollupSearchRequest, RollupSearchResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public StartRollupJobResponse StartJob(
      Id id,
      Func<StartRollupJobDescriptor, IStartRollupJobRequest> selector = null)
    {
      return this.StartJob(selector.InvokeOrDefault<StartRollupJobDescriptor, IStartRollupJobRequest>(new StartRollupJobDescriptor(id)));
    }

    public Task<StartRollupJobResponse> StartJobAsync(
      Id id,
      Func<StartRollupJobDescriptor, IStartRollupJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StartJobAsync(selector.InvokeOrDefault<StartRollupJobDescriptor, IStartRollupJobRequest>(new StartRollupJobDescriptor(id)), ct);
    }

    public StartRollupJobResponse StartJob(IStartRollupJobRequest request) => this.DoRequest<IStartRollupJobRequest, StartRollupJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StartRollupJobResponse> StartJobAsync(
      IStartRollupJobRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IStartRollupJobRequest, StartRollupJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public StopRollupJobResponse StopJob(
      Id id,
      Func<StopRollupJobDescriptor, IStopRollupJobRequest> selector = null)
    {
      return this.StopJob(selector.InvokeOrDefault<StopRollupJobDescriptor, IStopRollupJobRequest>(new StopRollupJobDescriptor(id)));
    }

    public Task<StopRollupJobResponse> StopJobAsync(
      Id id,
      Func<StopRollupJobDescriptor, IStopRollupJobRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StopJobAsync(selector.InvokeOrDefault<StopRollupJobDescriptor, IStopRollupJobRequest>(new StopRollupJobDescriptor(id)), ct);
    }

    public StopRollupJobResponse StopJob(IStopRollupJobRequest request) => this.DoRequest<IStopRollupJobRequest, StopRollupJobResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StopRollupJobResponse> StopJobAsync(
      IStopRollupJobRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IStopRollupJobRequest, StopRollupJobResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
