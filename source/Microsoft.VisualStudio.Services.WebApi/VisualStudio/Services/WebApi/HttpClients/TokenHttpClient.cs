// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.HttpClients.TokenHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi.HttpClients
{
  [ResourceArea("0AD75E84-88AE-4325-84B5-EBB30910283C")]
  public class TokenHttpClient : VssHttpClientBase
  {
    private static ApiResourceVersion DefaultApiResourceVersion = new ApiResourceVersion(1.0);
    private static Dictionary<string, Type> TranslatedExceptionsMap = new Dictionary<string, Type>();

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) TokenHttpClient.TranslatedExceptionsMap;

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

    public virtual Task<string> GetAadUserAccessToken(
      string resource,
      string tenantId,
      IdentityDescriptor identityDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (resource), resource);
      keyValuePairList.Add(nameof (tenantId), tenantId);
      keyValuePairList.Add("identity", identityDescriptor.IdentityType + ";" + identityDescriptor.Identifier);
      using (new VssHttpClientBase.OperationScope("Token", nameof (GetAadUserAccessToken)))
        return this.GetAsync<string>(TokenResourceIds.AadUserToken, version: TokenHttpClient.DefaultApiResourceVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<string> GetAadAppAccessToken(
      string resource,
      string tenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (resource), resource);
      keyValuePairList.Add(nameof (tenantId), tenantId);
      using (new VssHttpClientBase.OperationScope("Token", "GetAadAccessToken"))
        return this.GetAsync<string>(TokenResourceIds.AadAppToken, version: TokenHttpClient.DefaultApiResourceVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<string> UpdateRefreshTokenOnBehalfOf(
      string accessToken,
      string resource,
      string tenantId,
      IdentityDescriptor identityDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(accessToken, nameof (accessToken));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (resource), resource);
      keyValuePairList.Add(nameof (tenantId), tenantId);
      keyValuePairList.Add("identity", identityDescriptor.IdentityType + ";" + identityDescriptor.Identifier);
      using (new VssHttpClientBase.OperationScope("Token", nameof (UpdateRefreshTokenOnBehalfOf)))
        return this.PostAsync<object, string>((object) new
        {
          accessToken = accessToken
        }, TokenResourceIds.AadUserToken, version: TokenHttpClient.DefaultApiResourceVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<string> GetUserAccessTokenFromAuthCode(
      string authCode,
      string resource,
      string tenantId,
      string replyToUri,
      IdentityDescriptor identityDescriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrEmpty(authCode, nameof (authCode));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckStringForNullOrEmpty(replyToUri, nameof (replyToUri));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (resource), resource);
      keyValuePairList.Add(nameof (tenantId), tenantId);
      keyValuePairList.Add("identity", identityDescriptor.IdentityType + ";" + identityDescriptor.Identifier);
      keyValuePairList.Add(nameof (replyToUri), replyToUri);
      using (new VssHttpClientBase.OperationScope("Token", nameof (GetUserAccessTokenFromAuthCode)))
        return this.PostAsync<object, string>((object) authCode, TokenResourceIds.AadUserToken, version: TokenHttpClient.DefaultApiResourceVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
