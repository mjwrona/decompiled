// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.WebApi.TokenIssueHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Tokens.WebApi
{
  [ResourceArea("6b10046c-829d-44d2-8a1d-02f88f4ff032")]
  public class TokenIssueHttpClient : VssHttpClientBase
  {
    public TokenIssueHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TokenIssueHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TokenIssueHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TokenIssueHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TokenIssueHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
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
      Guid guid = new Guid("24691e90-c8bd-42c0-8aae-71b7511a797a");
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

    public Task<AppSessionTokenResult> IssueAppSessionTokenAsync(
      SubjectDescriptor subjectDescriptor,
      Guid clientId,
      Guid? authorizationId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("325f73ea-e978-4ad1-8f3a-c30b39000a17");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (clientId), clientId.ToString());
      if (authorizationId.HasValue)
        keyValuePairList.Add(nameof (authorizationId), authorizationId.Value.ToString());
      return this.SendAsync<AppSessionTokenResult>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<AccessTokenResult> ExchangeAppTokenAsync(
      AppTokenSecretPair appInfo,
      Guid? accessId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9030cb81-c1fd-4f3b-9910-c90eb559b830");
      HttpContent httpContent = (HttpContent) new ObjectContent<AppTokenSecretPair>(appInfo, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (accessId.HasValue)
        collection.Add(nameof (accessId), accessId.Value.ToString());
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
      return this.SendAsync<AccessTokenResult>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<SessionToken> CreateSessionTokenAsync(
      SessionToken sessionToken,
      Guid hostId,
      Guid orgHostId,
      Guid deploymentHostId,
      SessionTokenType? tokenType = null,
      bool? isPublic = null,
      bool? isRequestedByTfsPatWebUI = null,
      bool? isImpersonating = null,
      string secretToken = null,
      Guid? requestedById = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("98e25729-952a-4b1f-ac89-7ca8b9803261");
      HttpContent httpContent = (HttpContent) new ObjectContent<SessionToken>(sessionToken, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      collection1.Add(nameof (hostId), hostId.ToString());
      collection1.Add(nameof (orgHostId), orgHostId.ToString());
      collection1.Add(nameof (deploymentHostId), deploymentHostId.ToString());
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
      if (isImpersonating.HasValue)
      {
        List<KeyValuePair<string, string>> collection4 = collection1;
        flag = isImpersonating.Value;
        string str = flag.ToString();
        collection4.Add(nameof (isImpersonating), str);
      }
      if (secretToken != null)
        collection1.Add(nameof (secretToken), secretToken);
      if (requestedById.HasValue)
        collection1.Add(nameof (requestedById), requestedById.Value.ToString());
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
      Guid locationId = new Guid("98e25729-952a-4b1f-ac89-7ca8b9803261");
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
      Guid locationId = new Guid("98e25729-952a-4b1f-ac89-7ca8b9803261");
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
      Guid locationId = new Guid("98e25729-952a-4b1f-ac89-7ca8b9803261");
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
      TokenIssueHttpClient tokenIssueHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("98e25729-952a-4b1f-ac89-7ca8b9803261");
      HttpContent httpContent = (HttpContent) new ObjectContent<SshPublicKey>(publicData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (remove), remove.ToString());
      TokenIssueHttpClient tokenIssueHttpClient2 = tokenIssueHttpClient1;
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
      using (await tokenIssueHttpClient2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RevokeAllSessionTokensOfUserAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("98e25729-952a-4b1f-ac89-7ca8b9803261"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task RevokeSessionTokenAsync(
      Guid authorizationId,
      bool? isPublic = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TokenIssueHttpClient tokenIssueHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("98e25729-952a-4b1f-ac89-7ca8b9803261");
      object routeValues = (object) new
      {
        authorizationId = authorizationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (isPublic.HasValue)
        keyValuePairList.Add(nameof (isPublic), isPublic.Value.ToString());
      using (await tokenIssueHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<SessionToken> UpdateSessionTokenAsync(
      Guid authorizationId,
      SessionToken sessionToken,
      SessionTokenType? tokenType = null,
      bool? isPublic = null,
      bool? isRequestedByTfsPatWebUI = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("98e25729-952a-4b1f-ac89-7ca8b9803261");
      object obj1 = (object) new
      {
        authorizationId = authorizationId
      };
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
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<SessionToken>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }
  }
}
