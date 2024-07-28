// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestResults.WebApi.TestResultsCompatHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.TestResults.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A1B70BC6-DD93-426A-A4F2-75066CF77D48
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestResults.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.TestResults.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TestResultsCompatHttpClientBase : VssHttpClientBase
  {
    public TestResultsCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestResultsCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestResultsCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestResultsCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestResultsCompatHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForRunAsync(
      string project,
      int runId,
      TestLogStoreOperationType testLogStoreOperationType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("67eb3f92-6c97-4fd9-8b63-6cbdc7e526ea");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testLogStoreOperationType), testLogStoreOperationType.ToString());
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestLogStoreEndpointDetails> TestLogStoreEndpointDetailsForRunAsync(
      Guid project,
      int runId,
      TestLogStoreOperationType testLogStoreOperationType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("67eb3f92-6c97-4fd9-8b63-6cbdc7e526ea");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testLogStoreOperationType), testLogStoreOperationType.ToString());
      return this.SendAsync<TestLogStoreEndpointDetails>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestResultMetaData>> QueryTestResultsMetaDataAsync(
      IEnumerable<string> testCaseReferenceIds,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b72ff4c0-4341-4213-ba27-f517cf341c95");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(testCaseReferenceIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.1, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultMetaData>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestResultMetaData>> QueryTestResultsMetaDataAsync(
      IEnumerable<string> testCaseReferenceIds,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b72ff4c0-4341-4213-ba27-f517cf341c95");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(testCaseReferenceIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.1, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestResultMetaData>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestCaseResult>> GetTestResultsAsync(
      string project,
      int runId,
      ResultDetails? detailsToInclude,
      int? skip,
      int? top,
      IEnumerable<TestOutcome> outcomes,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("02afa165-e79a-4d70-8f0c-2af0f35b4e07");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(6.0, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestCaseResult>> GetTestResultsAsync(
      Guid project,
      int runId,
      ResultDetails? detailsToInclude,
      int? skip,
      int? top,
      IEnumerable<TestOutcome> outcomes,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("02afa165-e79a-4d70-8f0c-2af0f35b4e07");
      object routeValues = (object) new
      {
        project = project,
        runId = runId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (detailsToInclude.HasValue)
        keyValuePairList.Add(nameof (detailsToInclude), detailsToInclude.Value.ToString());
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(6.0, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
