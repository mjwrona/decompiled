// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ConnectedServiceHttpClient
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ResourceArea("79134C72-4A58-4B42-976C-04E7115F32BF")]
  public class ConnectedServiceHttpClient : VssHttpClientBase
  {
    private readonly ApiResourceVersion mCurrentApiVersion = new ApiResourceVersion(1.0);

    public ConnectedServiceHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ConnectedServiceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ConnectedServiceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ConnectedServiceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ConnectedServiceHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<IEnumerable<WebApiConnectedService>> GetConnectedServices(
      string projectId,
      ConnectedServiceKind connectionServiceKind = ConnectedServiceKind.Custom,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
      QueryParamHelper.AddNonNullParam(queryParams, "kind", (object) connectionServiceKind);
      var data = new{ projectId = projectId };
      Guid connectedServicesId = CoreConstants.ConnectedServicesId;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParams;
      var routeValues = data;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      object userState1 = userState;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.GetAsync<IEnumerable<WebApiConnectedService>>(connectedServicesId, (object) routeValues, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WebApiConnectedServiceDetails> GetConnectedServiceDetails(
      string projectId,
      string connectionName,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
      QueryParamHelper.AddNonNullParam(queryParams, "name", (object) connectionName);
      var data = new{ projectId = projectId };
      Guid connectedServicesId = CoreConstants.ConnectedServicesId;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParams;
      var routeValues = data;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      object userState1 = userState;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.GetAsync<WebApiConnectedServiceDetails>(connectedServicesId, (object) routeValues, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WebApiConnectedService> CreateConnectedService(
      string projectId,
      WebApiConnectedServiceDetails serviceDetails,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      var data = new{ projectId = projectId };
      WebApiConnectedServiceDetails connectedServiceDetails = serviceDetails;
      object obj = (object) data;
      Guid connectedServicesId = CoreConstants.ConnectedServicesId;
      object routeValues = obj;
      ApiResourceVersion currentApiVersion = this.mCurrentApiVersion;
      CancellationToken cancellationToken1 = cancellationToken;
      object userState1 = userState;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.PostAsync<WebApiConnectedServiceDetails, WebApiConnectedService>(connectedServiceDetails, connectedServicesId, routeValues, currentApiVersion, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
