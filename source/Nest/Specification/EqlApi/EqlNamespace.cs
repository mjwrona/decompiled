// Decompiled with JetBrains decompiler
// Type: Nest.Specification.EqlApi.EqlNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.EqlApi
{
  public class EqlNamespace : Nest.NamespacedClientProxy
  {
    internal EqlNamespace(ElasticClient client)
      : base(client)
    {
    }

    public EqlDeleteResponse Delete(
      Id id,
      Func<EqlDeleteDescriptor, IEqlDeleteRequest> selector = null)
    {
      return this.Delete(selector.InvokeOrDefault<EqlDeleteDescriptor, IEqlDeleteRequest>(new EqlDeleteDescriptor(id)));
    }

    public Task<EqlDeleteResponse> DeleteAsync(
      Id id,
      Func<EqlDeleteDescriptor, IEqlDeleteRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteAsync(selector.InvokeOrDefault<EqlDeleteDescriptor, IEqlDeleteRequest>(new EqlDeleteDescriptor(id)), ct);
    }

    public EqlDeleteResponse Delete(IEqlDeleteRequest request) => this.DoRequest<IEqlDeleteRequest, EqlDeleteResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<EqlDeleteResponse> DeleteAsync(IEqlDeleteRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IEqlDeleteRequest, EqlDeleteResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public EqlGetResponse<TDocument> Get<TDocument>(
      Id id,
      Func<EqlGetDescriptor, IEqlGetRequest> selector = null)
      where TDocument : class
    {
      return this.Get<TDocument>(selector.InvokeOrDefault<EqlGetDescriptor, IEqlGetRequest>(new EqlGetDescriptor(id)));
    }

    public Task<EqlGetResponse<TDocument>> GetAsync<TDocument>(
      Id id,
      Func<EqlGetDescriptor, IEqlGetRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.GetAsync<TDocument>(selector.InvokeOrDefault<EqlGetDescriptor, IEqlGetRequest>(new EqlGetDescriptor(id)), ct);
    }

    public EqlGetResponse<TDocument> Get<TDocument>(IEqlGetRequest request) where TDocument : class => this.DoRequest<IEqlGetRequest, EqlGetResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);

    public Task<EqlGetResponse<TDocument>> GetAsync<TDocument>(
      IEqlGetRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<IEqlGetRequest, EqlGetResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public EqlSearchStatusResponse SearchStatus(
      Id id,
      Func<EqlSearchStatusDescriptor, IEqlSearchStatusRequest> selector = null)
    {
      return this.SearchStatus(selector.InvokeOrDefault<EqlSearchStatusDescriptor, IEqlSearchStatusRequest>(new EqlSearchStatusDescriptor(id)));
    }

    public Task<EqlSearchStatusResponse> SearchStatusAsync(
      Id id,
      Func<EqlSearchStatusDescriptor, IEqlSearchStatusRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.SearchStatusAsync(selector.InvokeOrDefault<EqlSearchStatusDescriptor, IEqlSearchStatusRequest>(new EqlSearchStatusDescriptor(id)), ct);
    }

    public EqlSearchStatusResponse SearchStatus(IEqlSearchStatusRequest request) => this.DoRequest<IEqlSearchStatusRequest, EqlSearchStatusResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<EqlSearchStatusResponse> SearchStatusAsync(
      IEqlSearchStatusRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IEqlSearchStatusRequest, EqlSearchStatusResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public EqlSearchResponse<TDocument> Search<TInferDocument, TDocument>(
      Func<EqlSearchDescriptor<TInferDocument>, IEqlSearchRequest> selector = null)
      where TInferDocument : class
      where TDocument : class
    {
      return this.Search<TDocument>(selector.InvokeOrDefault<EqlSearchDescriptor<TInferDocument>, IEqlSearchRequest>(new EqlSearchDescriptor<TInferDocument>()));
    }

    public Task<EqlSearchResponse<TDocument>> SearchAsync<TInferDocument, TDocument>(
      Func<EqlSearchDescriptor<TInferDocument>, IEqlSearchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TInferDocument : class
      where TDocument : class
    {
      return this.SearchAsync<TDocument>(selector.InvokeOrDefault<EqlSearchDescriptor<TInferDocument>, IEqlSearchRequest>(new EqlSearchDescriptor<TInferDocument>()), ct);
    }

    public EqlSearchResponse<TDocument> Search<TDocument>(
      Func<EqlSearchDescriptor<TDocument>, IEqlSearchRequest> selector = null)
      where TDocument : class
    {
      return this.Search<TDocument>(selector.InvokeOrDefault<EqlSearchDescriptor<TDocument>, IEqlSearchRequest>(new EqlSearchDescriptor<TDocument>()));
    }

    public Task<EqlSearchResponse<TDocument>> SearchAsync<TDocument>(
      Func<EqlSearchDescriptor<TDocument>, IEqlSearchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.SearchAsync<TDocument>(selector.InvokeOrDefault<EqlSearchDescriptor<TDocument>, IEqlSearchRequest>(new EqlSearchDescriptor<TDocument>()), ct);
    }

    public EqlSearchResponse<TDocument> Search<TDocument>(IEqlSearchRequest request) where TDocument : class => this.DoRequest<IEqlSearchRequest, EqlSearchResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);

    public Task<EqlSearchResponse<TDocument>> SearchAsync<TDocument>(
      IEqlSearchRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<IEqlSearchRequest, EqlSearchResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
