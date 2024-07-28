// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry.AadToAcrRefreshTokenProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry
{
  internal class AadToAcrRefreshTokenProvider : IAuthorizationTokenProvider
  {
    private readonly IAuthorizationTokenProvider _aadTokenProvider;
    private readonly ServiceEndpoint _serviceEndpoint;
    private readonly IExternalProviderHttpRequesterFactory _requesterFactory;
    private readonly IRequestEvaluator _acrRequestEvaluator;

    public AadToAcrRefreshTokenProvider(
      IAuthorizationTokenProvider aadTokenProvider,
      ServiceEndpoint serviceEndpoint,
      IExternalProviderHttpRequesterFactory requesterFactory,
      IRequestEvaluator acrRequestEvaluator)
    {
      ArgumentUtility.CheckForNull<IAuthorizationTokenProvider>(aadTokenProvider, nameof (aadTokenProvider));
      ArgumentUtility.CheckForNull<IRequestEvaluator>(acrRequestEvaluator, nameof (acrRequestEvaluator));
      ArgumentUtility.CheckForNull<ServiceEndpoint>(serviceEndpoint, nameof (serviceEndpoint));
      ArgumentUtility.CheckForNull<IExternalProviderHttpRequesterFactory>(requesterFactory, nameof (requesterFactory));
      this._aadTokenProvider = aadTokenProvider;
      this._serviceEndpoint = serviceEndpoint;
      this._requesterFactory = requesterFactory;
      this._acrRequestEvaluator = acrRequestEvaluator;
    }

    public bool CanProcess(HttpWebRequest request) => this._acrRequestEvaluator.ComplyWith(request);

    public string GetToken(HttpWebRequest request, string resourceUrl)
    {
      if (!this.CanProcess(request))
        throw new ArgumentException(nameof (request));
      string str;
      if (!this._serviceEndpoint.Authorization.Parameters.TryGetValue("TenantId", out str))
        return (string) null;
      if (string.IsNullOrEmpty(str))
        return (string) null;
      string token = this._aadTokenProvider.GetToken(request, resourceUrl);
      if (token == null)
        return (string) null;
      Uri requestUri = new Uri(request.RequestUri, "/oauth2/exchange");
      using (IExternalProviderHttpRequester requester = this._requesterFactory.GetRequester())
      {
        using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, requestUri))
        {
          message.Content = (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) new Dictionary<string, string>()
          {
            {
              "grant_type",
              "access_token"
            },
            {
              "service",
              requestUri.Host
            },
            {
              "tenant",
              str
            },
            {
              "access_token",
              token
            }
          });
          HttpResponseMessage httpResponseMessage = requester.SendRequest(message);
          httpResponseMessage.EnsureSuccessStatusCode();
          return (string) JObject.Parse(httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()).SelectToken("refresh_token");
        }
      }
    }
  }
}
