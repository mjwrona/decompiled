// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi.TokenHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi
{
  [ResourceArea("0AD75E84-88AE-4325-84B5-EBB30910283C")]
  public class TokenHttpClient : VssHttpClientBase
  {
    public TokenHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TokenHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TokenHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TokenHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TokenHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<AccessToken> ExchangeAccessTokenKeyAsync(
      string accessTokenKey,
      bool? isPublic = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("94c2bcfb-bf10-4b41-ac01-738122d6b5e0");
      HttpContent httpContent = (HttpContent) new ObjectContent<string>(accessTokenKey, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (isPublic.HasValue)
        collection.Add(nameof (isPublic), isPublic.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AccessToken>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    [Obsolete("Use ExchangeAccessTokenKey instead.  This endpoint should be removed after all services are updated to M123.")]
    public Task<AccessToken> GetAccessTokenAsync(
      string key = null,
      bool? isPublic = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94c2bcfb-bf10-4b41-ac01-738122d6b5e0");
      object routeValues = (object) new{ key = key };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (isPublic.HasValue)
        keyValuePairList.Add(nameof (isPublic), isPublic.Value.ToString());
      return this.SendAsync<AccessToken>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<AppSessionTokenResult> IssueAppSessionTokenAsync(
      Guid clientId,
      Guid? userId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("b743b207-6dc5-457b-b1df-b9b63d640f0b");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      if (userId.HasValue)
        keyValuePairList.Add(nameof (userId), userId.Value.ToString());
      return this.SendAsync<AppSessionTokenResult>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<AccessTokenResult> ExchangeAppTokenAsync(
      AppTokenSecretPair appInfo,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9ce3c96a-34a2-41af-807d-205da73f227b");
      HttpContent httpContent = (HttpContent) new ObjectContent<AppTokenSecretPair>(appInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AccessTokenResult>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<SessionToken> CreateSessionTokenAsync(
      SessionToken sessionToken,
      SessionTokenType? tokenType = null,
      bool? isPublic = null,
      bool? isRequestedByTfsPatWebUI = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ada996bc-8c18-4193-b20c-cd41b13f5b4d");
      HttpContent httpContent = (HttpContent) new ObjectContent<SessionToken>(sessionToken, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      if (tokenType.HasValue)
        collection1.Add(nameof (tokenType), tokenType.Value.ToString());
      bool flag;
      if (isPublic.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        flag = isPublic.Value;
        string str = flag.ToString();
        collection2.Add(nameof (isPublic), str);
      }
      if (isRequestedByTfsPatWebUI.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        flag = isRequestedByTfsPatWebUI.Value;
        string str = flag.ToString();
        collection3.Add(nameof (isRequestedByTfsPatWebUI), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<SessionToken>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<SessionToken> GetSessionTokenAsync(
      Guid authorizationId,
      bool? isPublic = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ada996bc-8c18-4193-b20c-cd41b13f5b4d");
      object routeValues = (object) new
      {
        authorizationId = authorizationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (isPublic.HasValue)
        keyValuePairList.Add(nameof (isPublic), isPublic.Value.ToString());
      return this.SendAsync<SessionToken>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<SessionToken>> GetSessionTokensAsync(
      bool? isPublic = null,
      bool? includePublicData = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ada996bc-8c18-4193-b20c-cd41b13f5b4d");
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (isPublic.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isPublic.Value;
        string str = flag.ToString();
        collection.Add(nameof (isPublic), str);
      }
      if (includePublicData.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includePublicData.Value;
        string str = flag.ToString();
        collection.Add(nameof (includePublicData), str);
      }
      return this.SendAsync<List<SessionToken>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PagedSessionTokens> GetSessionTokensPageAsync(
      DisplayFilterOptions displayFilterOption,
      CreatedByOptions createdByOption,
      SortByOptions sortByOption,
      bool isSortAscending,
      int startRowNumber,
      int pageSize,
      string pageRequestTimeStamp,
      bool? isPublic = null,
      bool? includePublicData = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ada996bc-8c18-4193-b20c-cd41b13f5b4d");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (displayFilterOption), displayFilterOption.ToString());
      keyValuePairList.Add(nameof (createdByOption), createdByOption.ToString());
      keyValuePairList.Add(nameof (sortByOption), sortByOption.ToString());
      keyValuePairList.Add(nameof (isSortAscending), isSortAscending.ToString());
      keyValuePairList.Add(nameof (startRowNumber), startRowNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (pageSize), pageSize.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (pageRequestTimeStamp), pageRequestTimeStamp);
      bool flag;
      if (isPublic.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isPublic.Value;
        string str = flag.ToString();
        collection.Add(nameof (isPublic), str);
      }
      if (includePublicData.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includePublicData.Value;
        string str = flag.ToString();
        collection.Add(nameof (includePublicData), str);
      }
      return this.SendAsync<PagedSessionTokens>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RemovePublicKeyAsync(
      SshPublicKey publicData,
      bool remove,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TokenHttpClient tokenHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ada996bc-8c18-4193-b20c-cd41b13f5b4d");
      HttpContent httpContent = (HttpContent) new ObjectContent<SshPublicKey>(publicData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (remove), remove.ToString());
      TokenHttpClient tokenHttpClient2 = tokenHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await tokenHttpClient2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RevokeAllSessionTokensOfUserAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("ada996bc-8c18-4193-b20c-cd41b13f5b4d"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task RevokeSessionTokenAsync(
      Guid authorizationId,
      bool? isPublic = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TokenHttpClient tokenHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("ada996bc-8c18-4193-b20c-cd41b13f5b4d");
      object routeValues = (object) new
      {
        authorizationId = authorizationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (isPublic.HasValue)
        keyValuePairList.Add(nameof (isPublic), isPublic.Value.ToString());
      using (await tokenHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }
  }
}
