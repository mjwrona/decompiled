// Decompiled with JetBrains decompiler
// Type: Nest.Specification.TransformApi.TransformNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.TransformApi
{
  public class TransformNamespace : Nest.NamespacedClientProxy
  {
    internal TransformNamespace(ElasticClient client)
      : base(client)
    {
    }

    public DeleteTransformResponse Delete(
      Id transformId,
      Func<DeleteTransformDescriptor, IDeleteTransformRequest> selector = null)
    {
      return this.Delete(selector.InvokeOrDefault<DeleteTransformDescriptor, IDeleteTransformRequest>(new DeleteTransformDescriptor(transformId)));
    }

    public Task<DeleteTransformResponse> DeleteAsync(
      Id transformId,
      Func<DeleteTransformDescriptor, IDeleteTransformRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteAsync(selector.InvokeOrDefault<DeleteTransformDescriptor, IDeleteTransformRequest>(new DeleteTransformDescriptor(transformId)), ct);
    }

    public DeleteTransformResponse Delete(IDeleteTransformRequest request) => this.DoRequest<IDeleteTransformRequest, DeleteTransformResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteTransformResponse> DeleteAsync(
      IDeleteTransformRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteTransformRequest, DeleteTransformResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetTransformResponse Get(
      Func<GetTransformDescriptor, IGetTransformRequest> selector = null)
    {
      return this.Get(selector.InvokeOrDefault<GetTransformDescriptor, IGetTransformRequest>(new GetTransformDescriptor()));
    }

    public Task<GetTransformResponse> GetAsync(
      Func<GetTransformDescriptor, IGetTransformRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetAsync(selector.InvokeOrDefault<GetTransformDescriptor, IGetTransformRequest>(new GetTransformDescriptor()), ct);
    }

    public GetTransformResponse Get(IGetTransformRequest request) => this.DoRequest<IGetTransformRequest, GetTransformResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetTransformResponse> GetAsync(IGetTransformRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetTransformRequest, GetTransformResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetTransformStatsResponse GetStats(
      Id transformId,
      Func<GetTransformStatsDescriptor, IGetTransformStatsRequest> selector = null)
    {
      return this.GetStats(selector.InvokeOrDefault<GetTransformStatsDescriptor, IGetTransformStatsRequest>(new GetTransformStatsDescriptor(transformId)));
    }

    public Task<GetTransformStatsResponse> GetStatsAsync(
      Id transformId,
      Func<GetTransformStatsDescriptor, IGetTransformStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetStatsAsync(selector.InvokeOrDefault<GetTransformStatsDescriptor, IGetTransformStatsRequest>(new GetTransformStatsDescriptor(transformId)), ct);
    }

    public GetTransformStatsResponse GetStats(IGetTransformStatsRequest request) => this.DoRequest<IGetTransformStatsRequest, GetTransformStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetTransformStatsResponse> GetStatsAsync(
      IGetTransformStatsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetTransformStatsRequest, GetTransformStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PreviewTransformResponse<TTransform> Preview<TDocument, TTransform>(
      Func<PreviewTransformDescriptor<TDocument>, IPreviewTransformRequest> selector = null)
      where TDocument : class
    {
      return this.Preview<TTransform>(selector.InvokeOrDefault<PreviewTransformDescriptor<TDocument>, IPreviewTransformRequest>(new PreviewTransformDescriptor<TDocument>()));
    }

    public Task<PreviewTransformResponse<TTransform>> PreviewAsync<TDocument, TTransform>(
      Func<PreviewTransformDescriptor<TDocument>, IPreviewTransformRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.PreviewAsync<TTransform>(selector.InvokeOrDefault<PreviewTransformDescriptor<TDocument>, IPreviewTransformRequest>(new PreviewTransformDescriptor<TDocument>()), ct);
    }

    public PreviewTransformResponse<TTransform> Preview<TTransform>(IPreviewTransformRequest request) => this.DoRequest<IPreviewTransformRequest, PreviewTransformResponse<TTransform>>(request, (IRequestParameters) request.RequestParameters);

    public Task<PreviewTransformResponse<TTransform>> PreviewAsync<TTransform>(
      IPreviewTransformRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPreviewTransformRequest, PreviewTransformResponse<TTransform>>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutTransformResponse Put<TDocument>(
      Id transformId,
      Func<PutTransformDescriptor<TDocument>, IPutTransformRequest> selector)
      where TDocument : class
    {
      return this.Put(selector.InvokeOrDefault<PutTransformDescriptor<TDocument>, IPutTransformRequest>(new PutTransformDescriptor<TDocument>(transformId)));
    }

    public Task<PutTransformResponse> PutAsync<TDocument>(
      Id transformId,
      Func<PutTransformDescriptor<TDocument>, IPutTransformRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.PutAsync(selector.InvokeOrDefault<PutTransformDescriptor<TDocument>, IPutTransformRequest>(new PutTransformDescriptor<TDocument>(transformId)), ct);
    }

    public PutTransformResponse Put(IPutTransformRequest request) => this.DoRequest<IPutTransformRequest, PutTransformResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutTransformResponse> PutAsync(IPutTransformRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPutTransformRequest, PutTransformResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public StartTransformResponse Start(
      Id transformId,
      Func<StartTransformDescriptor, IStartTransformRequest> selector = null)
    {
      return this.Start(selector.InvokeOrDefault<StartTransformDescriptor, IStartTransformRequest>(new StartTransformDescriptor(transformId)));
    }

    public Task<StartTransformResponse> StartAsync(
      Id transformId,
      Func<StartTransformDescriptor, IStartTransformRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StartAsync(selector.InvokeOrDefault<StartTransformDescriptor, IStartTransformRequest>(new StartTransformDescriptor(transformId)), ct);
    }

    public StartTransformResponse Start(IStartTransformRequest request) => this.DoRequest<IStartTransformRequest, StartTransformResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StartTransformResponse> StartAsync(
      IStartTransformRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IStartTransformRequest, StartTransformResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public StopTransformResponse Stop(
      Id transformId,
      Func<StopTransformDescriptor, IStopTransformRequest> selector = null)
    {
      return this.Stop(selector.InvokeOrDefault<StopTransformDescriptor, IStopTransformRequest>(new StopTransformDescriptor(transformId)));
    }

    public Task<StopTransformResponse> StopAsync(
      Id transformId,
      Func<StopTransformDescriptor, IStopTransformRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StopAsync(selector.InvokeOrDefault<StopTransformDescriptor, IStopTransformRequest>(new StopTransformDescriptor(transformId)), ct);
    }

    public StopTransformResponse Stop(IStopTransformRequest request) => this.DoRequest<IStopTransformRequest, StopTransformResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StopTransformResponse> StopAsync(
      IStopTransformRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IStopTransformRequest, StopTransformResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public UpdateTransformResponse Update<TDocument>(
      Id transformId,
      Func<UpdateTransformDescriptor<TDocument>, IUpdateTransformRequest> selector)
      where TDocument : class
    {
      return this.Update(selector.InvokeOrDefault<UpdateTransformDescriptor<TDocument>, IUpdateTransformRequest>(new UpdateTransformDescriptor<TDocument>(transformId)));
    }

    public Task<UpdateTransformResponse> UpdateAsync<TDocument>(
      Id transformId,
      Func<UpdateTransformDescriptor<TDocument>, IUpdateTransformRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.UpdateAsync(selector.InvokeOrDefault<UpdateTransformDescriptor<TDocument>, IUpdateTransformRequest>(new UpdateTransformDescriptor<TDocument>(transformId)), ct);
    }

    public UpdateTransformResponse Update(IUpdateTransformRequest request) => this.DoRequest<IUpdateTransformRequest, UpdateTransformResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<UpdateTransformResponse> UpdateAsync(
      IUpdateTransformRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IUpdateTransformRequest, UpdateTransformResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
