// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestResults.WebApi.TestResultsHttpClient
// Assembly: Microsoft.VisualStudio.Services.TestResults.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A1B70BC6-DD93-426A-A4F2-75066CF77D48
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestResults.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.TestResults.WebApi
{
  [ResourceArea("C83EAF52-EDF3-4034-AE11-17D38F25404C")]
  public class TestResultsHttpClient : TestResultsHttpClientBase, ITestResultsHttpClient
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();

    public TestResultsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestResultsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestResultsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestResultsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestResultsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    static TestResultsHttpClient()
    {
      TestResultsHttpClient.s_translatedExceptions.Add("AccessDeniedException", typeof (AccessDeniedException));
      TestResultsHttpClient.s_translatedExceptions.Add("TestObjectNotFoundException", typeof (TestObjectNotFoundException));
      TestResultsHttpClient.s_translatedExceptions.Add("TeamProjectNotFoundException", typeof (TeamProjectNotFoundException));
      TestResultsHttpClient.s_translatedExceptions.Add("InvalidPropertyException", typeof (InvalidPropertyException));
      TestResultsHttpClient.s_translatedExceptions.Add("TestObjectInUseException", typeof (TestObjectInUseException));
      TestResultsHttpClient.s_translatedExceptions.Add("TestObjectUpdatedException", typeof (TestObjectUpdatedException));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Task<TestRunStatistic> GetTestRunStatisticsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRunStatistic>(new HttpMethod("GET"), new Guid("82b986e8-ca9e-4a89-b39e-f65c69bc104a"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(6.0, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<TestRunStatistic>>) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Task<TestRunStatistic> GetTestRunStatisticsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRunStatistic>(new HttpMethod("GET"), new Guid("82b986e8-ca9e-4a89-b39e-f65c69bc104a"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(6.0, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<TestRunStatistic>>) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<ShallowTestCaseResult>> GetTestResultsByBuildWithContinuationTokenAsync(
      string project,
      int buildId,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f48cc885-dbc4-4efc-ab19-ae8c19d1e02a");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<ShallowTestCaseResult>>>(this.GetPagedList<ShallowTestCaseResult>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<ShallowTestCaseResult>> GetTestResultsByBuildWithContinuationTokenAsync(
      Guid project,
      int buildId,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("f48cc885-dbc4-4efc-ab19-ae8c19d1e02a");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<ShallowTestCaseResult>>>(this.GetPagedList<ShallowTestCaseResult>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<ShallowTestCaseResult>> GetTestResultsByReleaseWithContinuationTokenAsync(
      string project,
      int releaseId,
      int? releaseEnvid = null,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3994b949-77e5-495d-8034-edf80d95b84e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num;
      if (releaseEnvid.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = releaseEnvid.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (releaseEnvid), str);
      }
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<ShallowTestCaseResult>>>(this.GetPagedList<ShallowTestCaseResult>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<ShallowTestCaseResult>> GetTestResultsByReleaseWithContinuationTokenAsync(
      Guid project,
      int releaseId,
      int? releaseEnvid = null,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3994b949-77e5-495d-8034-edf80d95b84e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num;
      if (releaseEnvid.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = releaseEnvid.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (releaseEnvid), str);
      }
      if (publishContext != null)
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<ShallowTestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion(5.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<ShallowTestCaseResult>>>(this.GetPagedList<ShallowTestCaseResult>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<ShallowTestCaseResult>> GetTestResultsByPipelineWithContinuationTokenAsync(
      string project,
      int pipelineId,
      string stageName = null,
      string phaseName = null,
      string jobName = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("80169dc2-30c3-4c25-84b2-dd67d7ff1f52");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (pipelineId), pipelineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (stageName != null)
        keyValuePairList1.Add(nameof (stageName), stageName);
      if (phaseName != null)
        keyValuePairList1.Add(nameof (phaseName), phaseName);
      if (jobName != null)
        keyValuePairList1.Add(nameof (jobName), jobName);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList1.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
        keyValuePairList1.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<IPagedList<ShallowTestCaseResult>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<ShallowTestCaseResult>>>(this.GetPagedList<ShallowTestCaseResult>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<ShallowTestCaseResult>> GetTestResultsByPipelineWithContinuationTokenAsync(
      Guid project,
      int pipelineId,
      string stageName = null,
      string phaseName = null,
      string jobName = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("80169dc2-30c3-4c25-84b2-dd67d7ff1f52");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList1 = new List<KeyValuePair<string, string>>();
      keyValuePairList1.Add(nameof (pipelineId), pipelineId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (stageName != null)
        keyValuePairList1.Add(nameof (stageName), stageName);
      if (phaseName != null)
        keyValuePairList1.Add(nameof (phaseName), phaseName);
      if (jobName != null)
        keyValuePairList1.Add(nameof (jobName), jobName);
      if (outcomes != null && outcomes.Any<TestOutcome>())
        keyValuePairList1.Add(nameof (outcomes), string.Join<TestOutcome>(",", outcomes));
      if (top.HasValue)
        keyValuePairList1.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<KeyValuePair<string, string>> keyValuePairList2 = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList2.Add("x-ms-continuationtoken", continuationToken);
      return this.SendAsync<IPagedList<ShallowTestCaseResult>>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList2, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList1, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<ShallowTestCaseResult>>>(this.GetPagedList<ShallowTestCaseResult>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<FieldDetailsForTestResults>> GetResultGroupsByBuildWithContinuationTokenAsync(
      string project,
      int buildId,
      string publishContext,
      IEnumerable<string> fields = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("E49244D1-C49F-49AD-A717-3BBAEFE6A201");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<FieldDetailsForTestResults>>>(this.GetPagedList<FieldDetailsForTestResults>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<FieldDetailsForTestResults>> GetResultGroupsByBuildWithContinuationTokenAsync(
      Guid project,
      int buildId,
      string publishContext,
      IEnumerable<string> fields = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("E49244D1-C49F-49AD-A717-3BBAEFE6A201");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<FieldDetailsForTestResults>>>(this.GetPagedList<FieldDetailsForTestResults>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<FieldDetailsForTestResults>> GetResultGroupsByReleaseWithContinuationTokenAsync(
      Guid project,
      int releaseId,
      string publishContext,
      int? releaseEnvId = null,
      IEnumerable<string> fields = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3C2B6BB0-0620-434A-A5C3-26AA0FCFDA15");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<FieldDetailsForTestResults>>>(this.GetPagedList<FieldDetailsForTestResults>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<IPagedList<FieldDetailsForTestResults>> GetResultGroupsByReleaseWithContinuationTokenAsync(
      string project,
      int releaseId,
      string publishContext,
      int? releaseEnvId = null,
      IEnumerable<string> fields = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3C2B6BB0-0620-434A-A5C3-26AA0FCFDA15");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<FieldDetailsForTestResults>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<FieldDetailsForTestResults>>>(this.GetPagedList<FieldDetailsForTestResults>));
    }

    public virtual Task<TestResultsGroupsForBuild> GetResultGroupsByBuildV1Async(
      string project,
      int buildId,
      string publishContext,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("E49244D1-C49F-49AD-A717-3BBAEFE6A201");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForBuild>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<TestResultsGroupsForBuild>>) null);
    }

    public virtual Task<TestResultsGroupsForBuild> GetResultGroupsByBuildV1Async(
      Guid project,
      int buildId,
      string publishContext,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("E49244D1-C49F-49AD-A717-3BBAEFE6A201");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForBuild>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<TestResultsGroupsForBuild>>) null);
    }

    public virtual Task<TestResultsGroupsForRelease> GetResultGroupsByReleaseV1Async(
      string project,
      int releaseId,
      string publishContext,
      int? releaseEnvId = null,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3C2B6BB0-0620-434A-A5C3-26AA0FCFDA15");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForRelease>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<TestResultsGroupsForRelease>>) null);
    }

    public virtual Task<TestResultsGroupsForRelease> GetResultGroupsByReleaseV1Async(
      Guid project,
      int releaseId,
      string publishContext,
      int? releaseEnvId = null,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3C2B6BB0-0620-434A-A5C3-26AA0FCFDA15");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForRelease>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<TestResultsGroupsForRelease>>) null);
    }

    public virtual async Task<Attachment> GetTcmRunAttachmentContentAsync(
      Guid project,
      int runId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClient resultsHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("B5731898-8206-477A-A51D-3FDF116FC6BF");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      Attachment tcmAttachment = new Attachment();
      tcmAttachment.FileName = httpResponseMessage.Content.Headers.ContentDisposition.FileName.Trim('"');
      if (httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        tcmAttachment.CompressionType = "gzip";
        tcmAttachment.Stream = (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        Attachment attachment = tcmAttachment;
        attachment.Stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        attachment = (Attachment) null;
      }
      Attachment attachmentContentAsync = tcmAttachment;
      tcmAttachment = (Attachment) null;
      return attachmentContentAsync;
    }

    public virtual async Task<Attachment> GetTcmResultAttachmentContentAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClient resultsHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2A632E97-E014-4275-978F-8E5C4906D4B3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      Attachment tcmAttachment = new Attachment();
      tcmAttachment.FileName = httpResponseMessage.Content.Headers.ContentDisposition.FileName.Trim('"');
      if (httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        tcmAttachment.CompressionType = "gzip";
        tcmAttachment.Stream = (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        Attachment attachment = tcmAttachment;
        attachment.Stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        attachment = (Attachment) null;
      }
      Attachment attachmentContentAsync = tcmAttachment;
      tcmAttachment = (Attachment) null;
      return attachmentContentAsync;
    }

    public virtual async Task<Attachment> GetTcmSubResultAttachmentContentAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int testSubResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClient resultsHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2A632E97-E014-4275-978F-8E5C4906D4B3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testSubResultId), testSubResultId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      Attachment tcmAttachment = new Attachment();
      tcmAttachment.FileName = httpResponseMessage.Content.Headers.ContentDisposition.FileName.Trim('"');
      if (httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        tcmAttachment.CompressionType = "gzip";
        tcmAttachment.Stream = (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        Attachment attachment = tcmAttachment;
        attachment.Stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        attachment = (Attachment) null;
      }
      Attachment attachmentContentAsync = tcmAttachment;
      tcmAttachment = (Attachment) null;
      return attachmentContentAsync;
    }

    public virtual async Task<Attachment> GetTcmIterationAttachmentContentAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      int iterationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestResultsHttpClient resultsHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2A632E97-E014-4275-978F-8E5C4906D4B3");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        attachmentId = attachmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (iterationId), iterationId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await resultsHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await resultsHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      Attachment tcmAttachment = new Attachment();
      tcmAttachment.FileName = httpResponseMessage.Content.Headers.ContentDisposition.FileName.Trim('"');
      if (httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        tcmAttachment.CompressionType = "gzip";
        tcmAttachment.Stream = (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress);
      }
      else
      {
        Attachment attachment = tcmAttachment;
        attachment.Stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        attachment = (Attachment) null;
      }
      Attachment attachmentContentAsync = tcmAttachment;
      tcmAttachment = (Attachment) null;
      return attachmentContentAsync;
    }

    public virtual Task<IPagedList<TestRun>> QueryTestRunsAsync2(
      string project,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state = null,
      IEnumerable<int> planIds = null,
      bool? isAutomated = null,
      TestRunPublishContext? publishContext = null,
      IEnumerable<int> buildIds = null,
      IEnumerable<int> buildDefIds = null,
      string branchName = null,
      IEnumerable<int> releaseIds = null,
      IEnumerable<int> releaseDefIds = null,
      IEnumerable<int> releaseEnvIds = null,
      IEnumerable<int> releaseEnvDefIds = null,
      string runTitle = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("364538F9-8062-4CE0-B024-75A0FB463F0D");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minLastUpdatedDate), minLastUpdatedDate);
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxLastUpdatedDate), maxLastUpdatedDate);
      if (state.HasValue)
        keyValuePairList.Add(nameof (state), state.Value.ToString());
      if (planIds != null && planIds.Any<int>())
        keyValuePairList.Add(nameof (planIds), string.Join<int>(",", planIds));
      if (isAutomated.HasValue)
        keyValuePairList.Add(nameof (isAutomated), isAutomated.Value.ToString());
      if (publishContext.HasValue)
        keyValuePairList.Add(nameof (publishContext), publishContext.Value.ToString());
      if (buildIds != null && buildIds.Any<int>())
        keyValuePairList.Add(nameof (buildIds), string.Join<int>(",", buildIds));
      if (buildDefIds != null && buildDefIds.Any<int>())
        keyValuePairList.Add(nameof (buildDefIds), string.Join<int>(",", buildDefIds));
      if (!string.IsNullOrEmpty(branchName))
        keyValuePairList.Add(nameof (branchName), branchName);
      if (releaseIds != null && releaseIds.Any<int>())
        keyValuePairList.Add(nameof (releaseIds), string.Join<int>(",", releaseIds));
      if (releaseDefIds != null && releaseDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseDefIds), string.Join<int>(",", releaseDefIds));
      if (releaseEnvIds != null && releaseEnvIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvIds), string.Join<int>(",", releaseEnvIds));
      if (releaseEnvDefIds != null && releaseEnvDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvDefIds), string.Join<int>(",", releaseEnvDefIds));
      if (!string.IsNullOrEmpty(runTitle))
        keyValuePairList.Add(nameof (runTitle), runTitle);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<TestRun>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<TestRun>>>(this.GetPagedList<TestRun>));
    }

    public virtual Task<IPagedList<TestRun>> QueryTestRunsAsync2(
      Guid project,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state = null,
      IEnumerable<int> planIds = null,
      bool? isAutomated = null,
      TestRunPublishContext? publishContext = null,
      IEnumerable<int> buildIds = null,
      IEnumerable<int> buildDefIds = null,
      string branchName = null,
      IEnumerable<int> releaseIds = null,
      IEnumerable<int> releaseDefIds = null,
      IEnumerable<int> releaseEnvIds = null,
      IEnumerable<int> releaseEnvDefIds = null,
      string runTitle = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("364538F9-8062-4CE0-B024-75A0FB463F0D");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minLastUpdatedDate), minLastUpdatedDate);
      this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxLastUpdatedDate), maxLastUpdatedDate);
      if (state.HasValue)
        keyValuePairList.Add(nameof (state), state.Value.ToString());
      if (planIds != null && planIds.Any<int>())
        keyValuePairList.Add(nameof (planIds), string.Join<int>(",", planIds));
      if (isAutomated.HasValue)
        keyValuePairList.Add(nameof (isAutomated), isAutomated.Value.ToString());
      if (publishContext.HasValue)
        keyValuePairList.Add(nameof (publishContext), publishContext.Value.ToString());
      if (buildIds != null && buildIds.Any<int>())
        keyValuePairList.Add(nameof (buildIds), string.Join<int>(",", buildIds));
      if (buildDefIds != null && buildDefIds.Any<int>())
        keyValuePairList.Add(nameof (buildDefIds), string.Join<int>(",", buildDefIds));
      if (!string.IsNullOrEmpty(branchName))
        keyValuePairList.Add(nameof (branchName), branchName);
      if (releaseIds != null && releaseIds.Any<int>())
        keyValuePairList.Add(nameof (releaseIds), string.Join<int>(",", releaseIds));
      if (releaseDefIds != null && releaseDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseDefIds), string.Join<int>(",", releaseDefIds));
      if (releaseEnvIds != null && releaseEnvIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvIds), string.Join<int>(",", releaseEnvIds));
      if (releaseEnvDefIds != null && releaseEnvDefIds.Any<int>())
        keyValuePairList.Add(nameof (releaseEnvDefIds), string.Join<int>(",", releaseEnvDefIds));
      if (!string.IsNullOrEmpty(runTitle))
        keyValuePairList.Add(nameof (runTitle), runTitle);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<IPagedList<TestRun>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<TestRun>>>(this.GetPagedList<TestRun>));
    }

    public HttpClient HttpClient => this.Client;

    protected Task<T> SendAsync<T>(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      return this.SendAsync<T>(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, routeValues, version, content, queryParameters, userState, cancellationToken, processResponse);
    }

    protected async Task<T> SendAsync<T>(
      HttpMethod method,
      IEnumerable<KeyValuePair<string, string>> additionalHeaders,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      TestResultsHttpClient resultsHttpClient = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await resultsHttpClient.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await resultsHttpClient.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      TestResultsHttpClient resultsHttpClient = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) resultsHttpClient).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await resultsHttpClient.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }

    protected async Task<IPagedList<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      TestResultsHttpClient resultsHttpClient = this;
      string continuationToken = resultsHttpClient.GetContinuationToken(responseMessage);
      IPagedList<T> pagedList = (IPagedList<T>) new PagedList<T>((IEnumerable<T>) await resultsHttpClient.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
      continuationToken = (string) null;
      return pagedList;
    }

    protected string GetContinuationToken(HttpResponseMessage responseMessage)
    {
      string continuationToken = (string) null;
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (responseMessage.Headers.TryGetValues("x-ms-continuationtoken", out values))
        continuationToken = values.FirstOrDefault<string>();
      return continuationToken;
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) TestResultsHttpClient.s_translatedExceptions;
  }
}
