// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.KeyVaultCredential
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.KeyVault
{
  public class KeyVaultCredential : ServiceClientCredentials
  {
    private KeyVaultClient _client;

    public event KeyVaultClient.AuthenticationCallback OnAuthenticate;

    public string Token { get; set; }

    public KeyVaultCredential(
      KeyVaultClient.AuthenticationCallback authenticationCallback)
    {
      this.OnAuthenticate = authenticationCallback;
    }

    internal KeyVaultCredential Clone() => new KeyVaultCredential(this.OnAuthenticate);

    private async Task<string> PreAuthenticate(Uri url)
    {
      if (this.OnAuthenticate != null)
      {
        HttpBearerChallenge challengeForUrl = HttpBearerChallengeCache.GetInstance().GetChallengeForURL(url);
        if (challengeForUrl != null)
          return await this.OnAuthenticate(challengeForUrl.AuthorizationServer, challengeForUrl.Resource, challengeForUrl.Scope).ConfigureAwait(false);
      }
      return (string) null;
    }

    protected async Task<string> PostAuthenticate(HttpResponseMessage response)
    {
      if (this.OnAuthenticate != null)
      {
        string challenge = response.Headers.WwwAuthenticate.ElementAt<AuthenticationHeaderValue>(0).ToString();
        if (HttpBearerChallenge.IsBearerChallenge(challenge))
        {
          HttpBearerChallenge httpBearerChallenge = new HttpBearerChallenge(response.RequestMessage.RequestUri, challenge);
          if (httpBearerChallenge != null)
          {
            HttpBearerChallengeCache.GetInstance().SetChallengeForURL(response.RequestMessage.RequestUri, httpBearerChallenge);
            return await this.OnAuthenticate(httpBearerChallenge.AuthorizationServer, httpBearerChallenge.Resource, httpBearerChallenge.Scope).ConfigureAwait(false);
          }
        }
      }
      return (string) null;
    }

    public override void InitializeServiceClient<T>(ServiceClient<T> client)
    {
      base.InitializeServiceClient<T>(client);
      this._client = client is KeyVaultClient keyVaultClient ? keyVaultClient : throw new ArgumentException("KeyVaultCredential credentials are only for use with the KeyVaultClient service client.");
    }

    public override async Task ProcessHttpRequestAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      string parameter1 = await this.PreAuthenticate(request.RequestUri).ConfigureAwait(false);
      if (!string.IsNullOrEmpty(parameter1))
      {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", parameter1);
      }
      else
      {
        HttpResponseMessage response;
        using (HttpRequestMessage r = new HttpRequestMessage(request.Method, request.RequestUri))
          response = await (this._client?.HttpClient ?? new HttpClient()).SendAsync(r).ConfigureAwait(false);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
          return;
        string parameter2 = await this.PostAuthenticate(response).ConfigureAwait(false);
        if (string.IsNullOrEmpty(parameter2))
          return;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", parameter2);
      }
    }
  }
}
