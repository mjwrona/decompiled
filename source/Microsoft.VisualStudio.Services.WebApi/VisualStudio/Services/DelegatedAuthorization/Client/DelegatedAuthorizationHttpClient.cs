// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.Client.DelegatedAuthorizationHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Tokens;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.Client
{
  [ResourceArea("0AD75E84-88AE-4325-84B5-EBB30910283C")]
  [Obsolete("This class has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient or Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.TokenHttpClient instead.")]
  public class DelegatedAuthorizationHttpClient : VssHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();
    private static readonly ApiResourceVersion s_currentApiVersion = new ApiResourceVersion(2.0);

    public DelegatedAuthorizationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DelegatedAuthorizationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DelegatedAuthorizationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DelegatedAuthorizationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DelegatedAuthorizationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.GetAccessTokenAsync instead.")]
    public async Task<AccessToken> Exchange(
      string key,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.Exchange(false, key, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.GetAccessTokenAsync instead.")]
    public virtual async Task<AccessToken> Exchange(
      bool isPublic,
      string key,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      AccessToken accessToken1;
      using (new VssHttpClientBase.OperationScope("Token", nameof (Exchange)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (isPublic), isPublic.ToString());
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid accessToken2 = TokenResourceIds.AccessToken;
        var routeValues = new{ key = key };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        accessToken1 = await authorizationHttpClient2.SendAsync<AccessToken>(get, accessToken2, (object) routeValues, currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return accessToken1;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.CreateSessionTokenAsync instead.")]
    public async Task<SessionToken> CreateSessionToken(
      Guid? clientId = null,
      Guid? userId = null,
      string displayName = null,
      DateTime? validTo = null,
      string scope = null,
      IList<Guid> targetAccounts = null,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.CreateSessionToken((string) null, (string) null, false, clientId, userId, displayName, validTo, scope, targetAccounts, tokenType, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.CreateSessionTokenAsync instead.")]
    public async Task<SessionToken> CreateSessionToken(
      bool isPublic,
      Guid? clientId = null,
      Guid? userId = null,
      string displayName = null,
      DateTime? validTo = null,
      string scope = null,
      IList<Guid> targetAccounts = null,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.CreateSessionToken((string) null, (string) null, isPublic, clientId, userId, displayName, validTo, scope, targetAccounts, tokenType, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.CreateSessionTokenAsync instead.")]
    public async Task<SessionToken> CreateSessionToken(
      string source,
      bool isPublic,
      Guid? clientId = null,
      Guid? userId = null,
      string displayName = null,
      DateTime? validTo = null,
      string scope = null,
      IList<Guid> targetAccounts = null,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.CreateSessionToken((string) null, source, isPublic, clientId, userId, displayName, validTo, scope, targetAccounts, tokenType, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.CreateSessionTokenAsync instead.")]
    public async Task<SessionToken> CreateSessionToken(
      string publicData,
      string source,
      bool isPublic,
      Guid? clientId = null,
      Guid? userId = null,
      string displayName = null,
      DateTime? validTo = null,
      string scope = null,
      IList<Guid> targetAccounts = null,
      SessionTokenType tokenType = SessionTokenType.SelfDescribing,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      SessionToken sessionToken1;
      using (new VssHttpClientBase.OperationScope("Token", nameof (CreateSessionToken)))
      {
        SessionToken sessionToken2 = new SessionToken();
        if (userId.HasValue)
          sessionToken2.UserId = userId.Value;
        if (validTo.HasValue)
          sessionToken2.ValidTo = validTo.Value;
        if (clientId.HasValue)
          sessionToken2.ClientId = clientId.Value;
        sessionToken2.DisplayName = displayName;
        sessionToken2.Scope = scope;
        sessionToken2.TargetAccounts = targetAccounts;
        sessionToken2.Source = source;
        sessionToken2.PublicData = publicData;
        HttpContent content = (HttpContent) new ObjectContent<SessionToken>(sessionToken2, authorizationHttpClient.Formatter);
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        if (tokenType != SessionTokenType.SelfDescribing)
          keyValuePairList.Add(nameof (tokenType), tokenType.ToString());
        keyValuePairList.Add(nameof (isPublic), isPublic.ToString());
        sessionToken1 = await authorizationHttpClient.SendAsync<SessionToken>(HttpMethod.Post, TokenResourceIds.SessionToken, version: DelegatedAuthorizationHttpClient.s_currentApiVersion, content: content, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return sessionToken1;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.CreateSessionTokenAsync instead.")]
    public async Task<SessionToken> UpdateSessionToken(
      Guid authorizationId,
      string displayName = null,
      string scope = null,
      DateTime? validTo = null,
      IList<Guid> targetAccounts = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.UpdateSessionToken(false, authorizationId, displayName, scope, validTo, targetAccounts, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.CreateSessionTokenAsync instead.")]
    public async Task<SessionToken> UpdateSessionToken(
      bool isPublic,
      Guid authorizationId,
      string displayName = null,
      string scope = null,
      DateTime? validTo = null,
      IList<Guid> targetAccounts = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      SessionToken sessionToken1;
      using (new VssHttpClientBase.OperationScope("Token", nameof (UpdateSessionToken)))
      {
        SessionToken sessionToken2 = new SessionToken();
        sessionToken2.AuthorizationId = authorizationId;
        if (validTo.HasValue)
          sessionToken2.ValidTo = validTo.Value;
        sessionToken2.DisplayName = displayName;
        sessionToken2.Scope = scope;
        sessionToken2.TargetAccounts = targetAccounts;
        HttpContent content = (HttpContent) new ObjectContent<SessionToken>(sessionToken2, authorizationHttpClient.Formatter);
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (isPublic), isPublic.ToString());
        sessionToken1 = await authorizationHttpClient.SendAsync<SessionToken>(HttpMethod.Post, TokenResourceIds.SessionToken, version: DelegatedAuthorizationHttpClient.s_currentApiVersion, content: content, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return sessionToken1;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.GetSessionTokensAsync instead.")]
    public async Task<List<SessionToken>> ListSessionTokens(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.ListSessionTokens(false, false, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.GetSessionTokensAsync instead.")]
    public async Task<List<SessionToken>> ListSessionTokens(
      bool isPublic,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.ListSessionTokens(false, isPublic, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.GetSessionTokensAsync instead.")]
    public async Task<List<SessionToken>> ListSessionTokens(
      bool includePublicData,
      bool isPublic,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      List<SessionToken> sessionTokenList;
      using (new VssHttpClientBase.OperationScope("Token", nameof (ListSessionTokens)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (isPublic), isPublic.ToString());
        collection.Add(nameof (includePublicData), includePublicData.ToString());
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid sessionToken = TokenResourceIds.SessionToken;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        sessionTokenList = await authorizationHttpClient2.SendAsync<List<SessionToken>>(get, sessionToken, version: currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return sessionTokenList;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.GetSessionTokenAsync instead.")]
    public async Task<SessionToken> GetSessionToken(
      Guid authorizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.GetSessionToken(false, authorizationId, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.GetSessionTokenAsync instead.")]
    public async Task<SessionToken> GetSessionToken(
      bool isPublic,
      Guid authorizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      SessionToken sessionToken1;
      using (new VssHttpClientBase.OperationScope("Token", nameof (GetSessionToken)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (authorizationId), authorizationId.ToString());
        collection.Add(nameof (isPublic), isPublic.ToString());
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid sessionToken2 = TokenResourceIds.SessionToken;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        sessionToken1 = await authorizationHttpClient2.SendAsync<SessionToken>(get, sessionToken2, version: currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return sessionToken1;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.RemovePublicKeyAsync instead.")]
    public async Task RemovePublicKey(
      string publicKey,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Token", nameof (RemovePublicKey)))
      {
        HttpContent httpContent1 = (HttpContent) new ObjectContent<SshPublicKey>(new SshPublicKey()
        {
          Value = publicKey
        }, authorizationHttpClient1.Formatter);
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add("remove", "true");
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod post = HttpMethod.Post;
        Guid sessionToken = TokenResourceIds.SessionToken;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        HttpContent httpContent2 = httpContent1;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        HttpContent content = httpContent2;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpResponseMessage httpResponseMessage = await authorizationHttpClient2.SendAsync(post, sessionToken, version: currentApiVersion, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.RemovePublicKeyAsync instead.")]
    public async Task RevokeSessionToken(
      Guid authorizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      await this.RevokeSessionToken(false, authorizationId, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.RevokeSessionTokenAsync instead.")]
    public async Task RevokeSessionToken(
      bool isPublic,
      Guid authorizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Token", nameof (RevokeSessionToken)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (authorizationId), authorizationId.ToString());
        collection.Add(nameof (isPublic), isPublic.ToString());
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod delete = HttpMethod.Delete;
        Guid sessionToken = TokenResourceIds.SessionToken;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpResponseMessage httpResponseMessage = await authorizationHttpClient2.SendAsync(delete, sessionToken, version: currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.InitiateAuthorizationAsync instead.")]
    public async Task<AuthorizationDescription> InitiateAuthorization(
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      AuthorizationDescription authorizationDescription;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (InitiateAuthorization)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (responseType), responseType.ToString());
        collection.Add(nameof (clientId), clientId.ToString());
        collection.Add(nameof (redirectUri), redirectUri.ToString());
        collection.Add(nameof (scopes), scopes);
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid authorization = DelegatedAuthResourceIds.Authorization;
        var routeValues = new{ userId = userId };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        authorizationDescription = await authorizationHttpClient2.SendAsync<AuthorizationDescription>(get, authorization, (object) routeValues, currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return authorizationDescription;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.InitiateAuthorizationAsync instead.")]
    public async Task<AuthorizationDecision> Authorize(
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      AuthorizationDecision authorizationDecision;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (Authorize)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (responseType), responseType.ToString());
        collection.Add(nameof (clientId), clientId.ToString());
        collection.Add(nameof (redirectUri), redirectUri.ToString());
        collection.Add(nameof (scopes), scopes);
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod post = HttpMethod.Post;
        Guid authorization = DelegatedAuthResourceIds.Authorization;
        var routeValues = new{ userId = userId };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        authorizationDecision = await authorizationHttpClient2.SendAsync<AuthorizationDecision>(post, authorization, (object) routeValues, currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return authorizationDecision;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.RevokeAuthorizationAsync instead.")]
    public async Task RevokeAuthorization(
      Guid userId,
      Guid authorizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (RevokeAuthorization)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (authorizationId), authorizationId.ToString());
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod post = HttpMethod.Post;
        Guid authorization = DelegatedAuthResourceIds.Authorization;
        var routeValues = new{ userId = userId };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpResponseMessage httpResponseMessage = await authorizationHttpClient2.SendAsync(post, authorization, (object) routeValues, currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.GetAuthorizationsAsync instead.")]
    public async Task<IEnumerable<AuthorizationDetails>> GetAuthorizations(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      IEnumerable<AuthorizationDetails> authorizations;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (GetAuthorizations)))
        authorizations = await authorizationHttpClient.SendAsync<IEnumerable<AuthorizationDetails>>(HttpMethod.Get, DelegatedAuthResourceIds.Authorization, (object) new
        {
          userId = userId
        }, DelegatedAuthorizationHttpClient.s_currentApiVersion, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return authorizations;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.IssueAppSessionTokenAsync instead.")]
    public async Task<AppSessionTokenResult> IssueAppSessionToken(
      Guid clientId,
      Guid? userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      AppSessionTokenResult sessionTokenResult;
      using (new VssHttpClientBase.OperationScope("Token", nameof (IssueAppSessionToken)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (clientId), clientId.ToString());
        if (userId.HasValue)
          keyValuePairList.Add(nameof (userId), userId.ToString());
        sessionTokenResult = await authorizationHttpClient.SendAsync<AppSessionTokenResult>(HttpMethod.Post, TokenResourceIds.AppSessionToken, version: DelegatedAuthorizationHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return sessionTokenResult;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.AuthorizeHostAsync instead.")]
    public async Task<HostAuthorizationDecision> AuthorizeHost(
      Guid clientId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      HostAuthorizationDecision authorizationDecision;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (AuthorizeHost)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (clientId), clientId.ToString());
        authorizationDecision = await authorizationHttpClient.SendAsync<HostAuthorizationDecision>(HttpMethod.Post, DelegatedAuthResourceIds.HostAuthorizeId, version: DelegatedAuthorizationHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return authorizationDecision;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.RevokeHostAuthorizationAsync instead.")]
    public async Task<HttpResponseMessage> RevokeHostAuthorization(
      Guid clientId,
      Guid? hostId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      HttpResponseMessage httpResponseMessage;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (RevokeHostAuthorization)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (clientId), clientId.ToString());
        keyValuePairList.Add(nameof (hostId), hostId.ToString());
        httpResponseMessage = await authorizationHttpClient.SendAsync<HttpResponseMessage>(HttpMethod.Delete, DelegatedAuthResourceIds.HostAuthorizeId, version: DelegatedAuthorizationHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.GetHostAuthorizationsAsync instead.")]
    public async Task<IList<HostAuthorization>> GetHostAuthorizations(
      Guid hostId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      IList<HostAuthorization> hostAuthorizations;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (GetHostAuthorizations)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (hostId), hostId.ToString());
        hostAuthorizations = await authorizationHttpClient.SendAsync<IList<HostAuthorization>>(HttpMethod.Get, DelegatedAuthResourceIds.HostAuthorizeId, version: DelegatedAuthorizationHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return hostAuthorizations;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.SessionTokenHttpClient.ExchangeAppTokenAsync instead.")]
    public async Task<AccessTokenResult> ExchangeAppToken(
      string appToken,
      string clientSecret,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      AccessTokenResult accessTokenResult;
      using (new VssHttpClientBase.OperationScope("Token", nameof (ExchangeAppToken)))
      {
        HttpContent content = (HttpContent) new ObjectContent<AppTokenSecretPair>(new AppTokenSecretPair()
        {
          AppToken = appToken,
          ClientSecret = clientSecret
        }, authorizationHttpClient.Formatter);
        accessTokenResult = await authorizationHttpClient.SendAsync<AccessTokenResult>(HttpMethod.Post, TokenResourceIds.AppTokenPair, version: DelegatedAuthorizationHttpClient.s_currentApiVersion, content: content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return accessTokenResult;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.CreateRegistrationAsync instead.")]
    public async Task<Registration> CreateRegistration(
      Registration registration,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.CreateRegistration(registration, false, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.CreateRegistrationAsync instead.")]
    public async Task<Registration> CreateRegistration(
      Registration registration,
      bool includeSecret,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      Registration registration1;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (CreateRegistration)))
      {
        HttpContent httpContent = (HttpContent) new ObjectContent<Registration>(registration, authorizationHttpClient1.Formatter);
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (includeSecret), includeSecret.ToString());
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod put = HttpMethod.Put;
        Guid registration2 = DelegatedAuthResourceIds.Registration;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        HttpContent content = httpContent;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = collection;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        registration1 = await authorizationHttpClient2.SendAsync<Registration>(put, registration2, version: currentApiVersion, content: content, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return registration1;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.UpdateRegistrationAsync instead.")]
    public async Task<Registration> UpdateRegistration(
      Registration registration,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.UpdateRegistration(registration, false, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.UpdateRegistrationAsync instead.")]
    public async Task<Registration> UpdateRegistration(
      Registration registration,
      bool includeSecret,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      Registration registration1;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (UpdateRegistration)))
      {
        HttpContent httpContent = (HttpContent) new ObjectContent<Registration>(registration, authorizationHttpClient1.Formatter);
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (includeSecret), includeSecret.ToString());
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod post = HttpMethod.Post;
        Guid registration2 = DelegatedAuthResourceIds.Registration;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        HttpContent content = httpContent;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = collection;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        registration1 = await authorizationHttpClient2.SendAsync<Registration>(post, registration2, version: currentApiVersion, content: content, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return registration1;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.DeleteRegistrationAsync instead.")]
    public async Task Delete(
      Guid registrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (Delete)))
      {
        HttpResponseMessage httpResponseMessage = await authorizationHttpClient.SendAsync(HttpMethod.Delete, DelegatedAuthResourceIds.Registration, (object) new
        {
          registrationId = registrationId
        }, DelegatedAuthorizationHttpClient.s_currentApiVersion, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.GetRegistrationAsync instead.")]
    public async Task<Registration> GetRegistration(
      Guid registrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.GetRegistration(registrationId, false, userState, cancellationToken).ConfigureAwait(false);
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.GetRegistrationAsync instead.")]
    public async Task<Registration> GetRegistration(
      Guid registrationId,
      bool includeSecret,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient1 = this;
      Registration registration1;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (GetRegistration)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (registrationId), registrationId.ToString());
        collection.Add(nameof (includeSecret), includeSecret.ToString());
        DelegatedAuthorizationHttpClient authorizationHttpClient2 = authorizationHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid registration2 = DelegatedAuthResourceIds.Registration;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        ApiResourceVersion currentApiVersion = DelegatedAuthorizationHttpClient.s_currentApiVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        registration1 = await authorizationHttpClient2.SendAsync<Registration>(get, registration2, version: currentApiVersion, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return registration1;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.GetSecretAsync instead.")]
    public async Task<JsonWebToken> GetSecret(
      Guid registrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      JsonWebToken secret;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (GetSecret)))
        secret = await authorizationHttpClient.SendAsync<JsonWebToken>(HttpMethod.Get, DelegatedAuthResourceIds.RegistrationSecret, (object) new
        {
          registrationId = registrationId
        }, DelegatedAuthorizationHttpClient.s_currentApiVersion, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return secret;
    }

    [Obsolete("This methos has been deprecated. Please use Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.DelegatedAuthorizationHttpClient.GetRegistrationsAsync instead.")]
    public async Task<IList<Registration>> ListRegistrations(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DelegatedAuthorizationHttpClient authorizationHttpClient = this;
      IList<Registration> registrationList;
      using (new VssHttpClientBase.OperationScope("DelegatedAuth", nameof (ListRegistrations)))
        registrationList = await authorizationHttpClient.SendAsync<IList<Registration>>(HttpMethod.Get, DelegatedAuthResourceIds.Registration, version: DelegatedAuthorizationHttpClient.s_currentApiVersion, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return registrationList;
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) DelegatedAuthorizationHttpClient.s_translatedExceptions;
  }
}
