// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestCompatHttpClientBase
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

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

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TestCompatHttpClientBase : VssHttpClientBase
  {
    public TestCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestCompatHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestActionResultModel>> GetActionResultsAsync(
      string project,
      int runId,
      int testCaseResultId,
      int iterationId,
      string actionPath,
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<List<TestActionResultModel>>(new HttpMethod("GET"), new Guid("eaf40c31-ff84-4062-aafd-d5664be11a37"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        iterationId = iterationId,
        actionPath = actionPath
      }, new ApiResourceVersion(6.0, 3), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestActionResultModel>> GetActionResultsAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int iterationId,
      string actionPath,
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<List<TestActionResultModel>>(new HttpMethod("GET"), new Guid("eaf40c31-ff84-4062-aafd-d5664be11a37"), (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        iterationId = iterationId,
        actionPath = actionPath
      }, new ApiResourceVersion(6.0, 3), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestResultParameterModel>> GetResultParametersAsync(
      string project,
      int runId,
      int testCaseResultId,
      int iterationId,
      string paramName,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7c69810d-3354-4af3-844a-180bd25db08a");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (paramName != null)
        keyValuePairList.Add(nameof (paramName), paramName);
      return this.SendAsync<List<TestResultParameterModel>>(method, locationId, routeValues, new ApiResourceVersion(6.0, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestResultParameterModel>> GetResultParametersAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int iterationId,
      string paramName,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7c69810d-3354-4af3-844a-180bd25db08a");
      object routeValues = (object) new
      {
        project = project,
        runId = runId,
        testCaseResultId = testCaseResultId,
        iterationId = iterationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (paramName != null)
        keyValuePairList.Add(nameof (paramName), paramName);
      return this.SendAsync<List<TestResultParameterModel>>(method, locationId, routeValues, new ApiResourceVersion(6.0, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestCaseResult>> GetTestResultsAsync(
      string project,
      int runId,
      ResultDetails? detailsToInclude,
      int? skip,
      int? top,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4637d869-3a76-4468-8057-0bb02aa385cf");
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
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestCaseResult>> GetTestResultsAsync(
      Guid project,
      int runId,
      ResultDetails? detailsToInclude,
      int? skip,
      int? top,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4637d869-3a76-4468-8057-0bb02aa385cf");
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
      return this.SendAsync<List<TestCaseResult>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestSuite> GetTestSuiteByIdAsync(
      string project,
      int planId,
      int suiteId,
      bool? includeChildSuites,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeChildSuites.HasValue)
        keyValuePairList.Add(nameof (includeChildSuites), includeChildSuites.Value.ToString());
      return this.SendAsync<TestSuite>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestSuite> GetTestSuiteByIdAsync(
      Guid project,
      int planId,
      int suiteId,
      bool? includeChildSuites,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeChildSuites.HasValue)
        keyValuePairList.Add(nameof (includeChildSuites), includeChildSuites.Value.ToString());
      return this.SendAsync<TestSuite>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestSuite>> GetTestSuitesForPlanAsync(
      string project,
      int planId,
      bool? includeSuites,
      int? skip,
      int? top,
      bool? asTreeView,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object routeValues = (object) new
      {
        project = project,
        planId = planId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeSuites.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeSuites.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeSuites), str);
      }
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (asTreeView.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = asTreeView.Value;
        string str = flag.ToString();
        collection.Add("$asTreeView", str);
      }
      return this.SendAsync<List<TestSuite>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestSuite>> GetTestSuitesForPlanAsync(
      Guid project,
      int planId,
      bool? includeSuites,
      int? skip,
      int? top,
      bool? asTreeView,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object routeValues = (object) new
      {
        project = project,
        planId = planId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeSuites.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeSuites.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeSuites), str);
      }
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (asTreeView.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = asTreeView.Value;
        string str = flag.ToString();
        collection.Add("$asTreeView", str);
      }
      return this.SendAsync<List<TestSuite>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestResultsDetails> GetTestResultDetailsForBuildAsync(
      string project,
      int buildId,
      string publishContext = null,
      string groupBy = null,
      string filter = null,
      string orderby = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("aae1bb55-a1b2-4951-86f5-0e72f1396ef9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (!string.IsNullOrEmpty(groupBy))
        keyValuePairList.Add(nameof (groupBy), groupBy);
      if (!string.IsNullOrEmpty(filter))
        keyValuePairList.Add("$filter", filter);
      if (!string.IsNullOrEmpty(orderby))
        keyValuePairList.Add("$orderby", orderby);
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestResultsDetails> GetTestResultDetailsForBuildAsync(
      Guid project,
      int buildId,
      string publishContext = null,
      string groupBy = null,
      string filter = null,
      string orderby = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("aae1bb55-a1b2-4951-86f5-0e72f1396ef9");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (!string.IsNullOrEmpty(groupBy))
        keyValuePairList.Add(nameof (groupBy), groupBy);
      if (!string.IsNullOrEmpty(filter))
        keyValuePairList.Add("$filter", filter);
      if (!string.IsNullOrEmpty(orderby))
        keyValuePairList.Add("$orderby", orderby);
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestResultsDetails> GetTestResultDetailsForReleaseAsync(
      string project,
      int releaseId,
      int releaseEnvId,
      string publishContext = null,
      string groupBy = null,
      string filter = null,
      string orderby = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5ee72329-c86b-4e7e-be8e-0744c2301a84");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (!string.IsNullOrEmpty(groupBy))
        keyValuePairList.Add(nameof (groupBy), groupBy);
      if (!string.IsNullOrEmpty(filter))
        keyValuePairList.Add("$filter", filter);
      if (!string.IsNullOrEmpty(orderby))
        keyValuePairList.Add("$orderby", orderby);
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestResultsDetails> GetTestResultDetailsForReleaseAsync(
      Guid project,
      int releaseId,
      int releaseEnvId,
      string publishContext = null,
      string groupBy = null,
      string filter = null,
      string orderby = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5ee72329-c86b-4e7e-be8e-0744c2301a84");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (!string.IsNullOrEmpty(groupBy))
        keyValuePairList.Add(nameof (groupBy), groupBy);
      if (!string.IsNullOrEmpty(filter))
        keyValuePairList.Add("$filter", filter);
      if (!string.IsNullOrEmpty(orderby))
        keyValuePairList.Add("$orderby", orderby);
      return this.SendAsync<TestResultsDetails>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestResultsGroupsForBuild> GetResultGroupsByBuildAsync(
      string project,
      int buildId,
      string publishContext,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d279d052-c55a-4204-b913-42f733b52958");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForBuild>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestResultsGroupsForBuild> GetResultGroupsByBuildAsync(
      Guid project,
      int buildId,
      string publishContext,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d279d052-c55a-4204-b913-42f733b52958");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (buildId), buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForBuild>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestResultsGroupsForRelease> GetResultGroupsByReleaseAsync(
      string project,
      int releaseId,
      string publishContext,
      int? releaseEnvId = null,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ef5ce5d4-a4e5-47ee-804c-354518f8d03f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForRelease>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestResultsGroupsForRelease> GetResultGroupsByReleaseAsync(
      Guid project,
      int releaseId,
      string publishContext,
      int? releaseEnvId = null,
      IEnumerable<string> fields = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ef5ce5d4-a4e5-47ee-804c-354518f8d03f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (releaseId), releaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(publishContext))
        keyValuePairList.Add(nameof (publishContext), publishContext);
      if (releaseEnvId.HasValue)
        keyValuePairList.Add(nameof (releaseEnvId), releaseEnvId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fields != null && fields.Any<string>())
        keyValuePairList.Add(nameof (fields), string.Join(",", fields));
      return this.SendAsync<TestResultsGroupsForRelease>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestRun> GetTestRunByIdAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRun>(new HttpMethod("GET"), new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion("5.0-preview.2"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestRun> GetTestRunByIdAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestRun>(new HttpMethod("GET"), new Guid("cadb3810-d47d-4a3c-a234-fe5f3be50138"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion("5.0-preview.2"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<CloneOperationInformation> GetCloneInformationAsync(
      string project,
      int cloneOperationId,
      bool? includeDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5b9d6320-abed-47a5-a151-cd6dc3798be6");
      object routeValues = (object) new
      {
        project = project,
        cloneOperationId = cloneOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDetails.HasValue)
        keyValuePairList.Add("$includeDetails", includeDetails.Value.ToString());
      return this.SendAsync<CloneOperationInformation>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<CloneOperationInformation> GetCloneInformationAsync(
      Guid project,
      int cloneOperationId,
      bool? includeDetails = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5b9d6320-abed-47a5-a151-cd6dc3798be6");
      object routeValues = (object) new
      {
        project = project,
        cloneOperationId = cloneOperationId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDetails.HasValue)
        keyValuePairList.Add("$includeDetails", includeDetails.Value.ToString());
      return this.SendAsync<CloneOperationInformation>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<CloneOperationInformation> CloneTestPlanAsync(
      TestPlanCloneRequest cloneRequestBody,
      string project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("edc3ef4b-8460-4e86-86fa-8e4f5e9be831");
      object obj1 = (object) new
      {
        project = project,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestPlanCloneRequest>(cloneRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CloneOperationInformation>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<CloneOperationInformation> CloneTestPlanAsync(
      TestPlanCloneRequest cloneRequestBody,
      Guid project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("edc3ef4b-8460-4e86-86fa-8e4f5e9be831");
      object obj1 = (object) new
      {
        project = project,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestPlanCloneRequest>(cloneRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CloneOperationInformation>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<CloneOperationInformation> CloneTestSuiteAsync(
      TestSuiteCloneRequest cloneRequestBody,
      string project,
      int planId,
      int sourceSuiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("751e4ab5-5bf6-4fb5-9d5d-19ef347662dd");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        sourceSuiteId = sourceSuiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSuiteCloneRequest>(cloneRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CloneOperationInformation>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<CloneOperationInformation> CloneTestSuiteAsync(
      TestSuiteCloneRequest cloneRequestBody,
      Guid project,
      int planId,
      int sourceSuiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("751e4ab5-5bf6-4fb5-9d5d-19ef347662dd");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        sourceSuiteId = sourceSuiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSuiteCloneRequest>(cloneRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CloneOperationInformation>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<SuiteEntry>> GetSuiteEntriesAsync(
      string project,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SuiteEntry>>(new HttpMethod("GET"), new Guid("bf8b7f78-0c1f-49cb-89e9-d1a17bcaaad3"), (object) new
      {
        project = project,
        suiteId = suiteId
      }, new ApiResourceVersion(5.0, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<SuiteEntry>> GetSuiteEntriesAsync(
      Guid project,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<SuiteEntry>>(new HttpMethod("GET"), new Guid("bf8b7f78-0c1f-49cb-89e9-d1a17bcaaad3"), (object) new
      {
        project = project,
        suiteId = suiteId
      }, new ApiResourceVersion(5.0, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<SuiteEntry>> ReorderSuiteEntriesAsync(
      IEnumerable<SuiteEntryUpdateModel> suiteEntries,
      string project,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("bf8b7f78-0c1f-49cb-89e9-d1a17bcaaad3");
      object obj1 = (object) new
      {
        project = project,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SuiteEntryUpdateModel>>(suiteEntries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<SuiteEntry>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<SuiteEntry>> ReorderSuiteEntriesAsync(
      IEnumerable<SuiteEntryUpdateModel> suiteEntries,
      Guid project,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("bf8b7f78-0c1f-49cb-89e9-d1a17bcaaad3");
      object obj1 = (object) new
      {
        project = project,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SuiteEntryUpdateModel>>(suiteEntries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<SuiteEntry>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestConfiguration> CreateTestConfigurationAsync(
      TestConfiguration testConfiguration,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d667591b-b9fd-4263-997a-9a084cca848f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestConfiguration>(testConfiguration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestConfiguration> CreateTestConfigurationAsync(
      TestConfiguration testConfiguration,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d667591b-b9fd-4263-997a-9a084cca848f");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestConfiguration>(testConfiguration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteTestConfigurationAsync(
      string project,
      int testConfigurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("d667591b-b9fd-4263-997a-9a084cca848f"), (object) new
      {
        project = project,
        testConfigurationId = testConfigurationId
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteTestConfigurationAsync(
      Guid project,
      int testConfigurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("d667591b-b9fd-4263-997a-9a084cca848f"), (object) new
      {
        project = project,
        testConfigurationId = testConfigurationId
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestConfiguration> GetTestConfigurationByIdAsync(
      string project,
      int testConfigurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestConfiguration>(new HttpMethod("GET"), new Guid("d667591b-b9fd-4263-997a-9a084cca848f"), (object) new
      {
        project = project,
        testConfigurationId = testConfigurationId
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestConfiguration> GetTestConfigurationByIdAsync(
      Guid project,
      int testConfigurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestConfiguration>(new HttpMethod("GET"), new Guid("d667591b-b9fd-4263-997a-9a084cca848f"), (object) new
      {
        project = project,
        testConfigurationId = testConfigurationId
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestConfiguration>> GetTestConfigurationsAsync(
      string project,
      int? skip = null,
      int? top = null,
      string continuationToken = null,
      bool? includeAllProperties = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d667591b-b9fd-4263-997a-9a084cca848f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
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
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (includeAllProperties.HasValue)
        keyValuePairList.Add(nameof (includeAllProperties), includeAllProperties.Value.ToString());
      return this.SendAsync<List<TestConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestConfiguration>> GetTestConfigurationsAsync(
      Guid project,
      int? skip = null,
      int? top = null,
      string continuationToken = null,
      bool? includeAllProperties = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d667591b-b9fd-4263-997a-9a084cca848f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
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
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (includeAllProperties.HasValue)
        keyValuePairList.Add(nameof (includeAllProperties), includeAllProperties.Value.ToString());
      return this.SendAsync<List<TestConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestConfiguration> UpdateTestConfigurationAsync(
      TestConfiguration testConfiguration,
      string project,
      int testConfigurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d667591b-b9fd-4263-997a-9a084cca848f");
      object obj1 = (object) new
      {
        project = project,
        testConfigurationId = testConfigurationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestConfiguration>(testConfiguration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestConfiguration> UpdateTestConfigurationAsync(
      TestConfiguration testConfiguration,
      Guid project,
      int testConfigurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d667591b-b9fd-4263-997a-9a084cca848f");
      object obj1 = (object) new
      {
        project = project,
        testConfigurationId = testConfigurationId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestConfiguration>(testConfiguration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestPlan> CreateTestPlanAsync(
      PlanUpdateModel testPlan,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("51712106-7278-4208-8563-1c96f40cf5e4");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<PlanUpdateModel>(testPlan, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestPlan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestPlan> CreateTestPlanAsync(
      PlanUpdateModel testPlan,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("51712106-7278-4208-8563-1c96f40cf5e4");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<PlanUpdateModel>(testPlan, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestPlan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteTestPlanAsync(
      string project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("51712106-7278-4208-8563-1c96f40cf5e4"), (object) new
      {
        project = project,
        planId = planId
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteTestPlanAsync(
      Guid project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("51712106-7278-4208-8563-1c96f40cf5e4"), (object) new
      {
        project = project,
        planId = planId
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestPlan> GetPlanByIdAsync(
      string project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestPlan>(new HttpMethod("GET"), new Guid("51712106-7278-4208-8563-1c96f40cf5e4"), (object) new
      {
        project = project,
        planId = planId
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestPlan> GetPlanByIdAsync(
      Guid project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestPlan>(new HttpMethod("GET"), new Guid("51712106-7278-4208-8563-1c96f40cf5e4"), (object) new
      {
        project = project,
        planId = planId
      }, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestPlan>> GetPlansAsync(
      string project,
      string owner = null,
      int? skip = null,
      int? top = null,
      bool? includePlanDetails = null,
      bool? filterActivePlans = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("51712106-7278-4208-8563-1c96f40cf5e4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (owner != null)
        keyValuePairList.Add(nameof (owner), owner);
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
      return this.SendAsync<List<TestPlan>>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestPlan>> GetPlansAsync(
      Guid project,
      string owner = null,
      int? skip = null,
      int? top = null,
      bool? includePlanDetails = null,
      bool? filterActivePlans = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("51712106-7278-4208-8563-1c96f40cf5e4");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (owner != null)
        keyValuePairList.Add(nameof (owner), owner);
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
      return this.SendAsync<List<TestPlan>>(method, locationId, routeValues, new ApiResourceVersion(5.0, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestPlan> UpdateTestPlanAsync(
      PlanUpdateModel planUpdateModel,
      string project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("51712106-7278-4208-8563-1c96f40cf5e4");
      object obj1 = (object) new
      {
        project = project,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PlanUpdateModel>(planUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestPlan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestPlan> UpdateTestPlanAsync(
      PlanUpdateModel planUpdateModel,
      Guid project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("51712106-7278-4208-8563-1c96f40cf5e4");
      object obj1 = (object) new
      {
        project = project,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PlanUpdateModel>(planUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestPlan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestSuite>> CreateTestSuiteAsync(
      SuiteCreateModel testSuite,
      string project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<SuiteCreateModel>(testSuite, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestSuite>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestSuite>> CreateTestSuiteAsync(
      SuiteCreateModel testSuite,
      Guid project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<SuiteCreateModel>(testSuite, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestSuite>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteTestSuiteAsync(
      string project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1"), (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      }, new ApiResourceVersion(5.0, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteTestSuiteAsync(
      Guid project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1"), (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      }, new ApiResourceVersion(5.0, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestSuite> GetTestSuiteByIdAsync(
      string project,
      int planId,
      int suiteId,
      int? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestSuite>(method, locationId, routeValues, new ApiResourceVersion(5.0, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestSuite> GetTestSuiteByIdAsync(
      Guid project,
      int planId,
      int suiteId,
      int? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<TestSuite>(method, locationId, routeValues, new ApiResourceVersion(5.0, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestSuite>> GetTestSuitesForPlanAsync(
      string project,
      int planId,
      int? expand = null,
      int? skip = null,
      int? top = null,
      bool? asTreeView = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object routeValues = (object) new
      {
        project = project,
        planId = planId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (expand.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = expand.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$expand", str);
      }
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
      if (asTreeView.HasValue)
        keyValuePairList.Add("$asTreeView", asTreeView.Value.ToString());
      return this.SendAsync<List<TestSuite>>(method, locationId, routeValues, new ApiResourceVersion(5.0, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestSuite>> GetTestSuitesForPlanAsync(
      Guid project,
      int planId,
      int? expand = null,
      int? skip = null,
      int? top = null,
      bool? asTreeView = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object routeValues = (object) new
      {
        project = project,
        planId = planId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (expand.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = expand.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$expand", str);
      }
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
      if (asTreeView.HasValue)
        keyValuePairList.Add("$asTreeView", asTreeView.Value.ToString());
      return this.SendAsync<List<TestSuite>>(method, locationId, routeValues, new ApiResourceVersion(5.0, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestSuite> UpdateTestSuiteAsync(
      SuiteUpdateModel suiteUpdateModel,
      string project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<SuiteUpdateModel>(suiteUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestSuite>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestSuite> UpdateTestSuiteAsync(
      SuiteUpdateModel suiteUpdateModel,
      Guid project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("7b7619a0-cb54-4ab3-bf22-194056f45dd1");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<SuiteUpdateModel>(suiteUpdateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestSuite>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestSuite>> GetSuitesByTestCaseIdAsync(
      int testCaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("09a6167b-e969-4775-9247-b94cf3819caf");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testCaseId), testCaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestSuite>>(method, locationId, version: new ApiResourceVersion(5.0, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestVariable> CreateTestVariableAsync(
      TestVariable testVariable,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("be3fcb2b-995b-47bf-90e5-ca3cf9980912");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestVariable>(testVariable, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestVariable>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestVariable> CreateTestVariableAsync(
      TestVariable testVariable,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("be3fcb2b-995b-47bf-90e5-ca3cf9980912");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestVariable>(testVariable, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestVariable>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteTestVariableAsync(
      string project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("be3fcb2b-995b-47bf-90e5-ca3cf9980912"), (object) new
      {
        project = project,
        testVariableId = testVariableId
      }, new ApiResourceVersion(5.0, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteTestVariableAsync(
      Guid project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("be3fcb2b-995b-47bf-90e5-ca3cf9980912"), (object) new
      {
        project = project,
        testVariableId = testVariableId
      }, new ApiResourceVersion(5.0, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestVariable> GetTestVariableByIdAsync(
      string project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestVariable>(new HttpMethod("GET"), new Guid("be3fcb2b-995b-47bf-90e5-ca3cf9980912"), (object) new
      {
        project = project,
        testVariableId = testVariableId
      }, new ApiResourceVersion(5.0, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestVariable> GetTestVariableByIdAsync(
      Guid project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestVariable>(new HttpMethod("GET"), new Guid("be3fcb2b-995b-47bf-90e5-ca3cf9980912"), (object) new
      {
        project = project,
        testVariableId = testVariableId
      }, new ApiResourceVersion(5.0, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestVariable>> GetTestVariablesAsync(
      string project,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("be3fcb2b-995b-47bf-90e5-ca3cf9980912");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<TestVariable>>(method, locationId, routeValues, new ApiResourceVersion(5.0, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<List<TestVariable>> GetTestVariablesAsync(
      Guid project,
      int? skip = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("be3fcb2b-995b-47bf-90e5-ca3cf9980912");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      return this.SendAsync<List<TestVariable>>(method, locationId, routeValues, new ApiResourceVersion(5.0, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestVariable> UpdateTestVariableAsync(
      TestVariable testVariable,
      string project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("be3fcb2b-995b-47bf-90e5-ca3cf9980912");
      object obj1 = (object) new
      {
        project = project,
        testVariableId = testVariableId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestVariable>(testVariable, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestVariable>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TestVariable> UpdateTestVariableAsync(
      TestVariable testVariable,
      Guid project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("be3fcb2b-995b-47bf-90e5-ca3cf9980912");
      object obj1 = (object) new
      {
        project = project,
        testVariableId = testVariableId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestVariable>(testVariable, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestVariable>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
