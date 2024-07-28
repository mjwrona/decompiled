// Decompiled with JetBrains decompiler
// Type: Nest.Specification.AsyncSearchApi.AsyncSearchNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.AsyncSearchApi
{
  public class AsyncSearchNamespace : Nest.NamespacedClientProxy
  {
    internal AsyncSearchNamespace(ElasticClient client)
      : base(client)
    {
    }

    public AsyncSearchDeleteResponse Delete(
      Id id,
      Func<AsyncSearchDeleteDescriptor, IAsyncSearchDeleteRequest> selector = null)
    {
      return this.Delete(selector.InvokeOrDefault<AsyncSearchDeleteDescriptor, IAsyncSearchDeleteRequest>(new AsyncSearchDeleteDescriptor(id)));
    }

    public Task<AsyncSearchDeleteResponse> DeleteAsync(
      Id id,
      Func<AsyncSearchDeleteDescriptor, IAsyncSearchDeleteRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteAsync(selector.InvokeOrDefault<AsyncSearchDeleteDescriptor, IAsyncSearchDeleteRequest>(new AsyncSearchDeleteDescriptor(id)), ct);
    }

    public AsyncSearchDeleteResponse Delete(IAsyncSearchDeleteRequest request) => this.DoRequest<IAsyncSearchDeleteRequest, AsyncSearchDeleteResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<AsyncSearchDeleteResponse> DeleteAsync(
      IAsyncSearchDeleteRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IAsyncSearchDeleteRequest, AsyncSearchDeleteResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public AsyncSearchGetResponse<TDocument> Get<TDocument>(
      Id id,
      Func<AsyncSearchGetDescriptor, IAsyncSearchGetRequest> selector = null)
      where TDocument : class
    {
      return this.Get<TDocument>(selector.InvokeOrDefault<AsyncSearchGetDescriptor, IAsyncSearchGetRequest>(new AsyncSearchGetDescriptor(id)));
    }

    public Task<AsyncSearchGetResponse<TDocument>> GetAsync<TDocument>(
      Id id,
      Func<AsyncSearchGetDescriptor, IAsyncSearchGetRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.GetAsync<TDocument>(selector.InvokeOrDefault<AsyncSearchGetDescriptor, IAsyncSearchGetRequest>(new AsyncSearchGetDescriptor(id)), ct);
    }

    public AsyncSearchGetResponse<TDocument> Get<TDocument>(IAsyncSearchGetRequest request) where TDocument : class => this.DoRequest<IAsyncSearchGetRequest, AsyncSearchGetResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);

    public Task<AsyncSearchGetResponse<TDocument>> GetAsync<TDocument>(
      IAsyncSearchGetRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<IAsyncSearchGetRequest, AsyncSearchGetResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public AsyncSearchStatusResponse Status(
      Id id,
      Func<AsyncSearchStatusDescriptor, IAsyncSearchStatusRequest> selector = null)
    {
      return this.Status(selector.InvokeOrDefault<AsyncSearchStatusDescriptor, IAsyncSearchStatusRequest>(new AsyncSearchStatusDescriptor(id)));
    }

    public Task<AsyncSearchStatusResponse> StatusAsync(
      Id id,
      Func<AsyncSearchStatusDescriptor, IAsyncSearchStatusRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StatusAsync(selector.InvokeOrDefault<AsyncSearchStatusDescriptor, IAsyncSearchStatusRequest>(new AsyncSearchStatusDescriptor(id)), ct);
    }

    public AsyncSearchStatusResponse Status(IAsyncSearchStatusRequest request) => this.DoRequest<IAsyncSearchStatusRequest, AsyncSearchStatusResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<AsyncSearchStatusResponse> StatusAsync(
      IAsyncSearchStatusRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IAsyncSearchStatusRequest, AsyncSearchStatusResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public AsyncSearchSubmitResponse<TDocument> Submit<TInferDocument, TDocument>(
      Func<AsyncSearchSubmitDescriptor<TInferDocument>, IAsyncSearchSubmitRequest> selector = null)
      where TInferDocument : class
      where TDocument : class
    {
      return this.Submit<TDocument>(selector.InvokeOrDefault<AsyncSearchSubmitDescriptor<TInferDocument>, IAsyncSearchSubmitRequest>(new AsyncSearchSubmitDescriptor<TInferDocument>()));
    }

    public Task<AsyncSearchSubmitResponse<TDocument>> SubmitAsync<TInferDocument, TDocument>(
      Func<AsyncSearchSubmitDescriptor<TInferDocument>, IAsyncSearchSubmitRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TInferDocument : class
      where TDocument : class
    {
      return this.SubmitAsync<TDocument>(selector.InvokeOrDefault<AsyncSearchSubmitDescriptor<TInferDocument>, IAsyncSearchSubmitRequest>(new AsyncSearchSubmitDescriptor<TInferDocument>()), ct);
    }

    public AsyncSearchSubmitResponse<TDocument> Submit<TDocument>(
      Func<AsyncSearchSubmitDescriptor<TDocument>, IAsyncSearchSubmitRequest> selector = null)
      where TDocument : class
    {
      return this.Submit<TDocument>(selector.InvokeOrDefault<AsyncSearchSubmitDescriptor<TDocument>, IAsyncSearchSubmitRequest>(new AsyncSearchSubmitDescriptor<TDocument>()));
    }

    public Task<AsyncSearchSubmitResponse<TDocument>> SubmitAsync<TDocument>(
      Func<AsyncSearchSubmitDescriptor<TDocument>, IAsyncSearchSubmitRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.SubmitAsync<TDocument>(selector.InvokeOrDefault<AsyncSearchSubmitDescriptor<TDocument>, IAsyncSearchSubmitRequest>(new AsyncSearchSubmitDescriptor<TDocument>()), ct);
    }

    public AsyncSearchSubmitResponse<TDocument> Submit<TDocument>(IAsyncSearchSubmitRequest request) where TDocument : class => this.DoRequest<IAsyncSearchSubmitRequest, AsyncSearchSubmitResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);

    public Task<AsyncSearchSubmitResponse<TDocument>> SubmitAsync<TDocument>(
      IAsyncSearchSubmitRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<IAsyncSearchSubmitRequest, AsyncSearchSubmitResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
