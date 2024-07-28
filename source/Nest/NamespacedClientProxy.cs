// Decompiled with JetBrains decompiler
// Type: Nest.NamespacedClientProxy
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  public class NamespacedClientProxy
  {
    private readonly ElasticClient _client;

    protected NamespacedClientProxy(ElasticClient client) => this._client = client;

    internal TResponse DoRequest<TRequest, TResponse>(
      TRequest p,
      IRequestParameters parameters,
      Action<IConnectionConfigurationValues, IRequestConfiguration> forceConfiguration = null)
      where TRequest : class, IRequest
      where TResponse : class, IElasticsearchResponse, new()
    {
      return this._client.DoRequest<TRequest, TResponse>(p, parameters, forceConfiguration);
    }

    internal Task<TResponse> DoRequestAsync<TRequest, TResponse>(
      TRequest p,
      IRequestParameters parameters,
      CancellationToken ct,
      Action<IConnectionConfigurationValues, IRequestConfiguration> forceConfiguration = null)
      where TRequest : class, IRequest
      where TResponse : class, IElasticsearchResponse, new()
    {
      return this._client.DoRequestAsync<TRequest, TResponse>(p, parameters, ct, forceConfiguration);
    }

    protected CatResponse<TCatRecord> DoCat<TRequest, TParams, TCatRecord>(TRequest request)
      where TRequest : class, IRequest<TParams>
      where TParams : RequestParameters<TParams>, new()
      where TCatRecord : ICatRecord
    {
      if (typeof (TCatRecord) == typeof (CatHelpRecord))
      {
        request.RequestParameters.CustomResponseBuilder = (CustomResponseBuilderBase) CatHelpResponseBuilder.Instance;
        return this.DoRequest<TRequest, CatResponse<TCatRecord>>(request, (IRequestParameters) request.RequestParameters, (Action<IConnectionConfigurationValues, IRequestConfiguration>) ((s, r) => ElasticClient.ForceTextPlain(s, r)));
      }
      request.RequestParameters.CustomResponseBuilder = (CustomResponseBuilderBase) CatResponseBuilder<TCatRecord>.Instance;
      return this.DoRequest<TRequest, CatResponse<TCatRecord>>(request, (IRequestParameters) request.RequestParameters, (Action<IConnectionConfigurationValues, IRequestConfiguration>) ((s, r) => ElasticClient.ForceJson(s, r)));
    }

    protected Task<CatResponse<TCatRecord>> DoCatAsync<TRequest, TParams, TCatRecord>(
      TRequest request,
      CancellationToken ct)
      where TRequest : class, IRequest<TParams>
      where TParams : RequestParameters<TParams>, new()
      where TCatRecord : ICatRecord
    {
      if (typeof (TCatRecord) == typeof (CatHelpRecord))
      {
        request.RequestParameters.CustomResponseBuilder = (CustomResponseBuilderBase) CatHelpResponseBuilder.Instance;
        return this.DoRequestAsync<TRequest, CatResponse<TCatRecord>>(request, (IRequestParameters) request.RequestParameters, ct, (Action<IConnectionConfigurationValues, IRequestConfiguration>) ((s, r) => ElasticClient.ForceTextPlain(s, r)));
      }
      request.RequestParameters.CustomResponseBuilder = (CustomResponseBuilderBase) CatResponseBuilder<TCatRecord>.Instance;
      return this.DoRequestAsync<TRequest, CatResponse<TCatRecord>>(request, (IRequestParameters) request.RequestParameters, ct, (Action<IConnectionConfigurationValues, IRequestConfiguration>) ((s, r) => ElasticClient.ForceJson(s, r)));
    }

    internal IRequestParameters ResponseBuilder(
      PreviewDatafeedRequestParameters parameters,
      CustomResponseBuilderBase builder)
    {
      parameters.CustomResponseBuilder = builder;
      return (IRequestParameters) parameters;
    }
  }
}
