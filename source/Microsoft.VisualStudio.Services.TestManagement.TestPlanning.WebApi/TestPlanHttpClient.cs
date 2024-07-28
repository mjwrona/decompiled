// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlanHttpClient
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [ResourceArea("E4C27205-9D23-4C98-B958-D798BC3F9CD4")]
  public class TestPlanHttpClient : TestPlanHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();

    public TestPlanHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestPlanHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestPlanHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestPlanHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestPlanHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    static TestPlanHttpClient()
    {
      TestPlanHttpClient.s_translatedExceptions.Add("AccessDeniedException", typeof (AccessDeniedException));
      TestPlanHttpClient.s_translatedExceptions.Add("TestObjectNotFoundException", typeof (TestObjectNotFoundException));
      TestPlanHttpClient.s_translatedExceptions.Add("TeamProjectNotFoundException", typeof (TeamProjectNotFoundException));
      TestPlanHttpClient.s_translatedExceptions.Add("InvalidPropertyException", typeof (InvalidPropertyException));
      TestPlanHttpClient.s_translatedExceptions.Add("TestObjectInUseException", typeof (TestObjectInUseException));
      TestPlanHttpClient.s_translatedExceptions.Add("TestObjectUpdatedException", typeof (TestObjectUpdatedException));
    }

    public virtual Task<PagedList<TestConfiguration>> GetTestConfigurationsWithContinuationTokenAsync(
      string project,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8369318E-38FA-4E84-9043-4B2A75D2C256");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestConfiguration>>>(this.GetPagedList<TestConfiguration>));
    }

    public virtual Task<PagedList<TestConfiguration>> GetTestConfigurationsWithContinuationTokenAsync(
      Guid project,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8369318E-38FA-4E84-9043-4B2A75D2C256");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestConfiguration>>>(this.GetPagedList<TestConfiguration>));
    }

    public virtual Task<PagedList<TestPlan>> GetTestPlansWithContinuationTokenAsync(
      string project,
      string owner = null,
      string continuationToken = null,
      bool? includePlanDetails = null,
      bool? filterActivePlans = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0E292477-A0C2-47F3-A9B6-34F153D627F4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (owner != null)
        keyValuePairList.Add(nameof (owner), owner);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      bool flag;
      if (includePlanDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includePlanDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includePlanDetails), str);
      }
      if (filterActivePlans.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = filterActivePlans.Value;
        string str = flag.ToString();
        collection.Add(nameof (filterActivePlans), str);
      }
      return this.SendAsync<PagedList<TestPlan>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestPlan>>>(this.GetPagedList<TestPlan>));
    }

    public virtual Task<PagedList<TestPlan>> GetTestPlansWithContinuationTokenAsync(
      Guid project,
      string owner = null,
      string continuationToken = null,
      bool? includePlanDetails = null,
      bool? filterActivePlans = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0E292477-A0C2-47F3-A9B6-34F153D627F4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (owner != null)
        keyValuePairList.Add(nameof (owner), owner);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      bool flag;
      if (includePlanDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includePlanDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includePlanDetails), str);
      }
      if (filterActivePlans.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = filterActivePlans.Value;
        string str = flag.ToString();
        collection.Add(nameof (filterActivePlans), str);
      }
      return this.SendAsync<PagedList<TestPlan>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestPlan>>>(this.GetPagedList<TestPlan>));
    }

    public virtual Task<PagedList<TestSuite>> GetTestSuitesForPlanWithContinuationTokenAsync(
      string project,
      int planId,
      SuiteExpand? expand = null,
      string continuationToken = null,
      bool? asTreeView = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1046D5D3-AB61-4CA7-A65A-36118A978256");
      object routeValues = (object) new
      {
        project = project,
        planId = planId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add(nameof (expand), expand.Value.ToString());
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (asTreeView.HasValue)
        keyValuePairList.Add(nameof (asTreeView), asTreeView.Value.ToString());
      return this.SendAsync<PagedList<TestSuite>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestSuite>>>(this.GetPagedList<TestSuite>));
    }

    public virtual Task<PagedList<TestSuite>> GetTestSuitesForPlanWithContinuationTokenAsync(
      Guid project,
      int planId,
      SuiteExpand? expand = null,
      string continuationToken = null,
      bool? asTreeView = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1046D5D3-AB61-4CA7-A65A-36118A978256");
      object routeValues = (object) new
      {
        project = project,
        planId = planId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add(nameof (expand), expand.Value.ToString());
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (asTreeView.HasValue)
        keyValuePairList.Add(nameof (asTreeView), asTreeView.Value.ToString());
      return this.SendAsync<PagedList<TestSuite>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestSuite>>>(this.GetPagedList<TestSuite>));
    }

    public virtual Task<PagedList<TestCase>> GetTestCaseListWithContinuationTokenAsync(
      string project,
      int planId,
      int suiteId,
      string testIds = null,
      string configurationIds = null,
      string witFields = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (testIds != null)
        keyValuePairList.Add(nameof (testIds), testIds);
      if (configurationIds != null)
        keyValuePairList.Add(nameof (configurationIds), configurationIds);
      if (witFields != null)
        keyValuePairList.Add(nameof (witFields), witFields);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestCase>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestCase>>>(this.GetPagedList<TestCase>));
    }

    public virtual Task<PagedList<TestCase>> GetTestCaseListWithContinuationTokenAsync(
      Guid project,
      int planId,
      int suiteId,
      string testIds = null,
      string configurationIds = null,
      string witFields = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (testIds != null)
        keyValuePairList.Add(nameof (testIds), testIds);
      if (configurationIds != null)
        keyValuePairList.Add(nameof (configurationIds), configurationIds);
      if (witFields != null)
        keyValuePairList.Add(nameof (witFields), witFields);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestCase>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestCase>>>(this.GetPagedList<TestCase>));
    }

    public virtual Task<PagedList<TestPoint>> GetPointsListWithContinuationTokenAsync(
      string project,
      int planId,
      int suiteId,
      string testPointIds = null,
      string testCaseId = null,
      string continuationToken = null,
      object userState = null,
      bool? includePointDetails = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52df686e-bae4-4334-b0ee-b6cf4e6f6b73");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (testPointIds != null)
        keyValuePairList.Add(nameof (testPointIds), testPointIds);
      if (testCaseId != null)
        keyValuePairList.Add(nameof (testCaseId), testCaseId);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (includePointDetails.HasValue)
        keyValuePairList.Add(nameof (includePointDetails), includePointDetails.Value.ToString());
      return this.SendAsync<PagedList<TestPoint>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestPoint>>>(this.GetPagedList<TestPoint>));
    }

    public virtual Task<PagedList<TestPoint>> GetPointsListWithContinuationTokenAsync(
      Guid project,
      int planId,
      int suiteId,
      string testPointIds = null,
      string testCaseId = null,
      string continuationToken = null,
      object userState = null,
      bool? includePointDetails = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52df686e-bae4-4334-b0ee-b6cf4e6f6b73");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (testPointIds != null)
        keyValuePairList.Add(nameof (testPointIds), testPointIds);
      if (testCaseId != null)
        keyValuePairList.Add(nameof (testCaseId), testCaseId);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (includePointDetails.HasValue)
        keyValuePairList.Add(nameof (includePointDetails), includePointDetails.Value.ToString());
      return this.SendAsync<PagedList<TestPoint>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestPoint>>>(this.GetPagedList<TestPoint>));
    }

    public virtual Task<PagedList<TestVariable>> GetTestVariablesWithContinuationTokenAsync(
      string project,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2C61FAC6-AC4E-45A5-8C38-1C2B8FD8EA6C");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestVariable>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestVariable>>>(this.GetPagedList<TestVariable>));
    }

    public virtual Task<PagedList<TestVariable>> GetTestVariablesWithContinuationTokenAsync(
      Guid project,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2C61FAC6-AC4E-45A5-8C38-1C2B8FD8EA6C");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestVariable>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<PagedList<TestVariable>>>(this.GetPagedList<TestVariable>));
    }

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
      TestPlanHttpClient testPlanHttpClient = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await testPlanHttpClient.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await testPlanHttpClient.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      TestPlanHttpClient testPlanHttpClient = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) testPlanHttpClient).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await testPlanHttpClient.SendAsync(message, cancellationToken: cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }

    protected async Task<PagedList<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      TestPlanHttpClient testPlanHttpClient = this;
      string continuationToken = testPlanHttpClient.GetContinuationToken(responseMessage);
      PagedList<T> pagedList = new PagedList<T>((IEnumerable<T>) await testPlanHttpClient.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
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

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) TestPlanHttpClient.s_translatedExceptions;
  }
}
