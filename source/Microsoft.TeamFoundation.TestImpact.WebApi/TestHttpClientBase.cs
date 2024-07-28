// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.WebApi.TestHttpClientBase
// Assembly: Microsoft.TeamFoundation.TestImpact.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7794F4D7-E029-40EA-A47C-F590EB851438
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.WebApi.dll

using Microsoft.TeamFoundation.TestImpact.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestImpact.WebApi
{
  [ResourceArea("B4A54C31-29A1-41E6-B301-D35B1ED663A0")]
  public abstract class TestHttpClientBase : VssHttpClientBase
  {
    public TestHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<TestImpactBuildData> PublishTestImpactBuildDataAsync(
      TestImpactBuildData testImapctBuildData,
      string project,
      DefinitionRunInfo definitionRunInfo,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9ff68920-1c90-47e7-95f3-0d58479a4bd7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestImpactBuildData>(testImapctBuildData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) queryParams, nameof (definitionRunInfo), (object) definitionRunInfo);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParams;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestImpactBuildData>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TestImpactBuildData> PublishTestImpactBuildDataAsync(
      TestImpactBuildData testImapctBuildData,
      Guid project,
      DefinitionRunInfo definitionRunInfo,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9ff68920-1c90-47e7-95f3-0d58479a4bd7");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestImpactBuildData>(testImapctBuildData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) queryParams, nameof (definitionRunInfo), (object) definitionRunInfo);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) queryParams;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestImpactBuildData>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TestImpactBuildData> QueryCodeChangesAsync(
      string project,
      DefinitionRunInfo definitionRunInfo,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9ff68920-1c90-47e7-95f3-0d58479a4bd7");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (definitionRunInfo), (object) definitionRunInfo);
      return this.SendAsync<TestImpactBuildData>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestImpactBuildData> QueryCodeChangesAsync(
      Guid project,
      DefinitionRunInfo definitionRunInfo,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9ff68920-1c90-47e7-95f3-0d58479a4bd7");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (definitionRunInfo), (object) definitionRunInfo);
      return this.SendAsync<TestImpactBuildData>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestResultSignaturesInfo> PublishCodeSignaturesAsync(
      TestResultSignaturesInfo results,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("754d6530-da2e-4aea-a677-75eaa653b5cc");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultSignaturesInfo>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultSignaturesInfo>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestResultSignaturesInfo> PublishCodeSignaturesAsync(
      TestResultSignaturesInfo results,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("754d6530-da2e-4aea-a677-75eaa653b5cc");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestResultSignaturesInfo>(results, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestResultSignaturesInfo>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<BuildType> QueryBuildTypeAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("754d6530-da2e-4aea-a677-75eaa653b5cc");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<BuildType>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BuildType> QueryBuildTypeAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("754d6530-da2e-4aea-a677-75eaa653b5cc");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<BuildType>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ImpactedTests> QueryImpactedTestsAsync(
      string project,
      DefinitionRunInfo definitionRunInfo,
      int currentTestRunId,
      TestInclusionOptions? typesToInclude = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("754d6530-da2e-4aea-a677-75eaa653b5cc");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (definitionRunInfo), (object) definitionRunInfo);
      keyValuePairList.Add(nameof (currentTestRunId), currentTestRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (typesToInclude.HasValue)
        keyValuePairList.Add(nameof (typesToInclude), typesToInclude.Value.ToString());
      return this.SendAsync<ImpactedTests>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<ImpactedTests> QueryImpactedTestsAsync(
      Guid project,
      DefinitionRunInfo definitionRunInfo,
      int currentTestRunId,
      TestInclusionOptions? typesToInclude = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("754d6530-da2e-4aea-a677-75eaa653b5cc");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (definitionRunInfo), (object) definitionRunInfo);
      keyValuePairList.Add(nameof (currentTestRunId), currentTestRunId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (typesToInclude.HasValue)
        keyValuePairList.Add(nameof (typesToInclude), typesToInclude.Value.ToString());
      return this.SendAsync<ImpactedTests>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
