// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle.Client.TokenSigningKeyHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle.Client
{
  [ResourceArea("{f189ca86-04a2-413c-81a0-abdbd7c472da}")]
  public class TokenSigningKeyHttpClient : VssHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();
    private static readonly ApiResourceVersion s_currentApiVersion = new ApiResourceVersion(1.0);

    public TokenSigningKeyHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TokenSigningKeyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TokenSigningKeyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TokenSigningKeyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TokenSigningKeyHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<TokenSigningKey> GetSigningKeys(
      string signingKeyNamespaceName,
      int keyId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TokenSigningKeyHttpClient signingKeyHttpClient1 = this;
      TokenSigningKey signingKeys;
      using (new VssHttpClientBase.OperationScope("TokenSigning", "GetValidationSigningKeys"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add("namespaceName", signingKeyNamespaceName);
        collection.Add(nameof (keyId), keyId.ToString());
        TokenSigningKeyHttpClient signingKeyHttpClient2 = signingKeyHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid signingKeysLocationId = TokenSigningKeyLifecycleResourceIds.SigningKeysLocationId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        ApiResourceVersion currentApiVersion = TokenSigningKeyHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        signingKeys = await signingKeyHttpClient2.SendAsync<TokenSigningKey>(get, signingKeysLocationId, version: currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return signingKeys;
    }

    public async Task<TokenSigningKeyNamespace> GetNamespace(
      string signingKeyNamespaceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TokenSigningKeyHttpClient signingKeyHttpClient1 = this;
      TokenSigningKeyNamespace signingKeyNamespace;
      using (new VssHttpClientBase.OperationScope("TokenSigning", nameof (GetNamespace)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add("namespaceName", signingKeyNamespaceName);
        TokenSigningKeyHttpClient signingKeyHttpClient2 = signingKeyHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid namespaceLocationId = TokenSigningKeyLifecycleResourceIds.NamespaceLocationId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        ApiResourceVersion currentApiVersion = TokenSigningKeyHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        signingKeyNamespace = await signingKeyHttpClient2.SendAsync<TokenSigningKeyNamespace>(get, namespaceLocationId, version: currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return signingKeyNamespace;
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) TokenSigningKeyHttpClient.s_translatedExceptions;
  }
}
