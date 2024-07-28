// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Social.WebApi.Clients.SocialHttpClient
// Assembly: Microsoft.VisualStudio.Services.Social.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0D2A928F-A131-41A8-A9E6-C3C26BFE105A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Social.WebApi.Clients
{
  public class SocialHttpClient : VssHttpClientBase
  {
    public SocialHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public SocialHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public SocialHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public SocialHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public SocialHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<SocialEngagementRecord> CreateSocialEngagementAsync(
      SocialEngagementCreateParameter socialEngagementCreateParameter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("99a61482-7000-4af0-9d84-daeacbea71d1");
      HttpContent httpContent = (HttpContent) new ObjectContent<SocialEngagementCreateParameter>(socialEngagementCreateParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<SocialEngagementRecord>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<SocialEngagementRecord> DeleteSocialEngagementAsync(
      string artifactType,
      string artifactId,
      SocialEngagementType engagementType,
      string artifactScopeType,
      string artifactScopeId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("99a61482-7000-4af0-9d84-daeacbea71d1");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (artifactId), artifactId);
      keyValuePairList.Add(nameof (engagementType), engagementType.ToString());
      keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      return this.SendAsync<SocialEngagementRecord>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<SocialEngagementRecord> GetSocialEngagementAsync(
      string artifactType,
      string artifactId,
      SocialEngagementType engagementType,
      string artifactScopeType,
      string artifactScopeId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("99a61482-7000-4af0-9d84-daeacbea71d1");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (artifactId), artifactId);
      keyValuePairList.Add(nameof (engagementType), engagementType.ToString());
      keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      return this.SendAsync<SocialEngagementRecord>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<SocialEngagementAggregateMetric> GetSocialEngagementAggregateMetricAsync(
      string artifactType,
      string artifactId,
      SocialEngagementType engagementType,
      string artifactScopeType,
      string artifactScopeId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b38448b8-44ec-4470-8328-08fe78efe297");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (artifactId), artifactId);
      keyValuePairList.Add(nameof (engagementType), engagementType.ToString());
      keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      return this.SendAsync<SocialEngagementAggregateMetric>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<KeyValuePair<SocialEngagementType, string>>> GetSocialEngagementProvidersAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<KeyValuePair<SocialEngagementType, string>>>(new HttpMethod("GET"), new Guid("7dc56847-4efe-4461-bd12-6c2f31e8144d"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<IdentityRef>> GetEngagedUsersAsync(
      string artifactType,
      string artifactId,
      SocialEngagementType engagementType,
      string artifactScopeType,
      string artifactScopeId = null,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("358536c5-2742-4c3e-9301-b46945becd73");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (artifactType), artifactType);
      keyValuePairList.Add(nameof (artifactId), artifactId);
      keyValuePairList.Add(nameof (engagementType), engagementType.ToString());
      keyValuePairList.Add(nameof (artifactScopeType), artifactScopeType);
      if (artifactScopeId != null)
        keyValuePairList.Add(nameof (artifactScopeId), artifactScopeId);
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (skip), str);
      }
      return this.SendAsync<List<IdentityRef>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
