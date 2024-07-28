// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenAdmin.Client.TokenAdminHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.TokenAdmin.Client
{
  [ResourceArea("af68438b-ed04-4407-9eb6-f1dbae3f922e")]
  public class TokenAdminHttpClient : VssHttpClientBase
  {
    public TokenAdminHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TokenAdminHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TokenAdminHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TokenAdminHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TokenAdminHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<TokenAdminPagedSessionTokens> ListPersonalAccessTokensAsync(
      SubjectDescriptor subjectDescriptor,
      int? pageSize = null,
      string continuationToken = null,
      bool? isPublic = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("af68438b-ed04-4407-9eb6-f1dbae3f922e");
      object routeValues = (object) new
      {
        subjectDescriptor = subjectDescriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (pageSize.HasValue)
        keyValuePairList.Add(nameof (pageSize), pageSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (isPublic.HasValue)
        keyValuePairList.Add(nameof (isPublic), isPublic.Value.ToString());
      return this.SendAsync<TokenAdminPagedSessionTokens>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task CreateRevocationRuleAsync(
      TokenAdminRevocationRule revocationRule,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TokenAdminHttpClient tokenAdminHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ee4afb16-e7ab-4ed8-9d4b-4ef3e78f97e4");
      HttpContent httpContent = (HttpContent) new ObjectContent<TokenAdminRevocationRule>(revocationRule, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TokenAdminHttpClient tokenAdminHttpClient2 = tokenAdminHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await tokenAdminHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RevokeAuthorizationsAsync(
      IEnumerable<TokenAdminRevocation> revocations,
      bool? isPublic = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TokenAdminHttpClient tokenAdminHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a9c08b2c-5466-4e22-8626-1ff304ffdf0f");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TokenAdminRevocation>>(revocations, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (isPublic.HasValue)
        collection.Add(nameof (isPublic), isPublic.Value.ToString());
      TokenAdminHttpClient tokenAdminHttpClient2 = tokenAdminHttpClient1;
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
      using (await tokenAdminHttpClient2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<SessionTokenResult> GetPersonalAccessTokenAsync(
      string accessTokenKey,
      bool isPublic,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("11e3d37f-fa7e-4721-ab2d-2d931bd944c4");
      HttpContent httpContent = (HttpContent) new ObjectContent<string>(accessTokenKey, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (isPublic), isPublic.ToString());
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
      return this.SendAsync<SessionTokenResult>(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task RevokePersonalAccessTokenAsync(
      string accessTokenKey,
      bool isPublic,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TokenAdminHttpClient tokenAdminHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("55687c95-c811-41e7-889f-25afb03eda19");
      HttpContent httpContent = (HttpContent) new ObjectContent<string>(accessTokenKey, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (isPublic), isPublic.ToString());
      TokenAdminHttpClient tokenAdminHttpClient2 = tokenAdminHttpClient1;
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
      using (await tokenAdminHttpClient2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
