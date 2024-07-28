// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionsHttpClient
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [ResourceArea("8477AEC9-A4C7-4BD4-A456-BA4C53C989CB")]
  public class ContributionsHttpClient : VssHttpClientBase
  {
    public ContributionsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ContributionsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ContributionsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ContributionsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ContributionsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<ContributionNodeQueryResult> QueryContributionNodesAsync(
      ContributionNodeQuery query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("db7f2146-2309-4cee-b39c-c767777a1c55");
      HttpContent httpContent = (HttpContent) new ObjectContent<ContributionNodeQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ContributionNodeQueryResult>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DataProviderResult> QueryDataProvidersAsync(
      DataProviderQuery query,
      string scopeName = null,
      string scopeValue = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("738368db-35ee-4b85-9f94-77ed34af2b0d");
      object obj1 = (object) new
      {
        scopeName = scopeName,
        scopeValue = scopeValue
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DataProviderQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DataProviderResult>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<InstalledExtension>> GetInstalledExtensionsAsync(
      IEnumerable<string> contributionIds = null,
      bool? includeDisabledApps = null,
      IEnumerable<string> assetTypes = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2648442b-fd63-4b9a-902f-0c913510f139");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (contributionIds != null && contributionIds.Any<string>())
        keyValuePairList.Add(nameof (contributionIds), string.Join(";", contributionIds));
      if (includeDisabledApps.HasValue)
        keyValuePairList.Add(nameof (includeDisabledApps), includeDisabledApps.Value.ToString());
      if (assetTypes != null && assetTypes.Any<string>())
        keyValuePairList.Add(nameof (assetTypes), string.Join(":", assetTypes));
      return this.SendAsync<List<InstalledExtension>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<InstalledExtension> GetInstalledExtensionByNameAsync(
      string publisherName,
      string extensionName,
      IEnumerable<string> assetTypes = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3e2f6668-0798-4dcb-b592-bfe2fa57fde2");
      object routeValues = (object) new
      {
        publisherName = publisherName,
        extensionName = extensionName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (assetTypes != null && assetTypes.Any<string>())
        keyValuePairList.Add(nameof (assetTypes), string.Join(":", assetTypes));
      return this.SendAsync<InstalledExtension>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
