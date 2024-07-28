// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry.AcrAccessTokenProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry
{
  internal class AcrAccessTokenProvider : IAuthorizationTokenProvider
  {
    private readonly IAuthenticateChallengeProvider _authenticateOptionsProvider;
    private readonly IAuthorizationTokenProvider _refreshTokenProvider;
    private readonly IExternalProviderHttpRequesterFactory _requesterFactory;
    private readonly IRequestEvaluator _acrRequestEvaluator;

    public AcrAccessTokenProvider(
      IAuthenticateChallengeProvider authenticateOptionsProvider,
      IAuthorizationTokenProvider refreshTokenProvider,
      IExternalProviderHttpRequesterFactory requesterFactory,
      IRequestEvaluator acrRequestEvaluator)
    {
      ArgumentUtility.CheckForNull<IAuthenticateChallengeProvider>(authenticateOptionsProvider, nameof (authenticateOptionsProvider));
      ArgumentUtility.CheckForNull<IAuthorizationTokenProvider>(refreshTokenProvider, nameof (refreshTokenProvider));
      ArgumentUtility.CheckForNull<IExternalProviderHttpRequesterFactory>(requesterFactory, nameof (requesterFactory));
      this._authenticateOptionsProvider = authenticateOptionsProvider;
      this._refreshTokenProvider = refreshTokenProvider;
      this._requesterFactory = requesterFactory;
      this._acrRequestEvaluator = acrRequestEvaluator;
    }

    public string GetToken(HttpWebRequest request, string resourceUrl)
    {
      if (!this.CanProcess(request))
        throw new ArgumentException(nameof (request));
      AuthenticationChallenge authenticationChallenge = this._authenticateOptionsProvider.GetChallenges(request).FirstOrDefault<AuthenticationChallenge>((Func<AuthenticationChallenge, bool>) (ch => string.Equals(ch.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase)));
      if (authenticationChallenge == null)
        return (string) null;
      IDictionary<string, string> options = authenticationChallenge.Options;
      string requestUri;
      if (!options.TryGetValue("realm", out requestUri))
        return (string) null;
      string str1;
      if (!options.TryGetValue("service", out str1))
        return (string) null;
      string str2;
      if (!options.TryGetValue("scope", out str2))
        return (string) null;
      string token = this._refreshTokenProvider.GetToken(request, resourceUrl);
      if (token == null)
        return (string) null;
      using (IExternalProviderHttpRequester requester = this._requesterFactory.GetRequester())
      {
        using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, requestUri))
        {
          message.Content = (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) new Dictionary<string, string>()
          {
            {
              "grant_type",
              "refresh_token"
            },
            {
              "service",
              str1
            },
            {
              "scope",
              str2
            },
            {
              "refresh_token",
              token
            }
          });
          HttpResponseMessage httpResponseMessage = requester.SendRequest(message);
          httpResponseMessage.EnsureSuccessStatusCode();
          return (string) JObject.Parse(httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()).SelectToken("access_token");
        }
      }
    }

    public bool CanProcess(HttpWebRequest request) => this._acrRequestEvaluator.ComplyWith(request);
  }
}
