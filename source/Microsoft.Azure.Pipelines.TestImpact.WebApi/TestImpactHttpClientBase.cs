// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestImpact.WebApi.TestImpactHttpClientBase
// Assembly: Microsoft.Azure.Pipelines.TestImpact.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BB589E1E-9019-4050-9752-2657E26FDC18
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.TestImpact.WebApi.dll

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

namespace Microsoft.Azure.Pipelines.TestImpact.WebApi
{
  [ResourceArea("2E9F9F41-088B-4B4E-8438-CB3FAA3BF7E4")]
  public abstract class TestImpactHttpClientBase : VssHttpClientBase
  {
    public TestImpactHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestImpactHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestImpactHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestImpactHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestImpactHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
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
      Guid guid = new Guid("10fc4ac8-0861-4f96-824a-720fb5b7a412");
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
      Guid guid = new Guid("10fc4ac8-0861-4f96-824a-720fb5b7a412");
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
      Guid locationId = new Guid("10fc4ac8-0861-4f96-824a-720fb5b7a412");
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
      Guid locationId = new Guid("10fc4ac8-0861-4f96-824a-720fb5b7a412");
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
      Guid guid = new Guid("4d49f667-6bd6-4a1a-9d54-b417a7b95824");
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
      Guid guid = new Guid("4d49f667-6bd6-4a1a-9d54-b417a7b95824");
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
      Guid locationId = new Guid("4d49f667-6bd6-4a1a-9d54-b417a7b95824");
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
      Guid locationId = new Guid("4d49f667-6bd6-4a1a-9d54-b417a7b95824");
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
      Guid locationId = new Guid("4d49f667-6bd6-4a1a-9d54-b417a7b95824");
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
      Guid locationId = new Guid("4d49f667-6bd6-4a1a-9d54-b417a7b95824");
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
