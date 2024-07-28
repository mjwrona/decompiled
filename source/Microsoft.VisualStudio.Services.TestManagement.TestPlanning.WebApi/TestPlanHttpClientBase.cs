// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlanHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [ResourceArea("E4C27205-9D23-4C98-B958-D798BC3F9CD4")]
  public abstract class TestPlanHttpClientBase : VssHttpClientBase
  {
    public TestPlanHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestPlanHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestPlanHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestPlanHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestPlanHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<TestConfiguration> CreateTestConfigurationAsync(
      TestConfigurationCreateUpdateParameters testConfigurationCreateUpdateParameters,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8369318e-38fa-4e84-9043-4b2a75d2c256");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestConfigurationCreateUpdateParameters>(testConfigurationCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestConfiguration> CreateTestConfigurationAsync(
      TestConfigurationCreateUpdateParameters testConfigurationCreateUpdateParameters,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8369318e-38fa-4e84-9043-4b2a75d2c256");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestConfigurationCreateUpdateParameters>(testConfigurationCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTestConfgurationAsync(
      string project,
      int testConfiguartionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestPlanHttpClientBase planHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("8369318e-38fa-4e84-9043-4b2a75d2c256");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testConfiguartionId), testConfiguartionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await planHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestConfgurationAsync(
      Guid project,
      int testConfiguartionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestPlanHttpClientBase planHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("8369318e-38fa-4e84-9043-4b2a75d2c256");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testConfiguartionId), testConfiguartionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await planHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestConfiguration> GetTestConfigurationByIdAsync(
      string project,
      int testConfigurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestConfiguration>(new HttpMethod("GET"), new Guid("8369318e-38fa-4e84-9043-4b2a75d2c256"), (object) new
      {
        project = project,
        testConfigurationId = testConfigurationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestConfiguration> GetTestConfigurationByIdAsync(
      Guid project,
      int testConfigurationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestConfiguration>(new HttpMethod("GET"), new Guid("8369318e-38fa-4e84-9043-4b2a75d2c256"), (object) new
      {
        project = project,
        testConfigurationId = testConfigurationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestConfiguration>> GetTestConfigurationsAsync(
      string project,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8369318e-38fa-4e84-9043-4b2a75d2c256");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestConfiguration>> GetTestConfigurationsAsync(
      Guid project,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8369318e-38fa-4e84-9043-4b2a75d2c256");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestConfiguration>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestConfiguration> UpdateTestConfigurationAsync(
      TestConfigurationCreateUpdateParameters testConfigurationCreateUpdateParameters,
      string project,
      int testConfiguartionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("8369318e-38fa-4e84-9043-4b2a75d2c256");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestConfigurationCreateUpdateParameters>(testConfigurationCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (testConfiguartionId), testConfiguartionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestConfiguration>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<TestConfiguration> UpdateTestConfigurationAsync(
      TestConfigurationCreateUpdateParameters testConfigurationCreateUpdateParameters,
      Guid project,
      int testConfiguartionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("8369318e-38fa-4e84-9043-4b2a75d2c256");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestConfigurationCreateUpdateParameters>(testConfigurationCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (testConfiguartionId), testConfiguartionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestConfiguration>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestEntityCount>> GetTestEntityCountByPlanIdAsync(
      string project,
      int planId,
      string states = null,
      UserFriendlyTestOutcome? outcome = null,
      string configurations = null,
      string testers = null,
      string assignedTo = null,
      TestEntityTypes? entity = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("300578da-7b40-4c1e-9542-7aed6029e504");
      object routeValues = (object) new
      {
        project = project,
        planId = planId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (states != null)
        keyValuePairList.Add(nameof (states), states);
      if (outcome.HasValue)
        keyValuePairList.Add(nameof (outcome), outcome.Value.ToString());
      if (configurations != null)
        keyValuePairList.Add(nameof (configurations), configurations);
      if (testers != null)
        keyValuePairList.Add(nameof (testers), testers);
      if (assignedTo != null)
        keyValuePairList.Add(nameof (assignedTo), assignedTo);
      if (entity.HasValue)
        keyValuePairList.Add(nameof (entity), entity.Value.ToString());
      return this.SendAsync<List<TestEntityCount>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestEntityCount>> GetTestEntityCountByPlanIdAsync(
      Guid project,
      int planId,
      string states = null,
      UserFriendlyTestOutcome? outcome = null,
      string configurations = null,
      string testers = null,
      string assignedTo = null,
      TestEntityTypes? entity = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("300578da-7b40-4c1e-9542-7aed6029e504");
      object routeValues = (object) new
      {
        project = project,
        planId = planId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (states != null)
        keyValuePairList.Add(nameof (states), states);
      if (outcome.HasValue)
        keyValuePairList.Add(nameof (outcome), outcome.Value.ToString());
      if (configurations != null)
        keyValuePairList.Add(nameof (configurations), configurations);
      if (testers != null)
        keyValuePairList.Add(nameof (testers), testers);
      if (assignedTo != null)
        keyValuePairList.Add(nameof (assignedTo), assignedTo);
      if (entity.HasValue)
        keyValuePairList.Add(nameof (entity), entity.Value.ToString());
      return this.SendAsync<List<TestEntityCount>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestPlan> CreateTestPlanAsync(
      TestPlanCreateParams testPlanCreateParams,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0e292477-a0c2-47f3-a9b6-34f153d627f4");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestPlanCreateParams>(testPlanCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestPlan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestPlan> CreateTestPlanAsync(
      TestPlanCreateParams testPlanCreateParams,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0e292477-a0c2-47f3-a9b6-34f153d627f4");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestPlanCreateParams>(testPlanCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestPlan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTestPlanAsync(
      string project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("0e292477-a0c2-47f3-a9b6-34f153d627f4"), (object) new
      {
        project = project,
        planId = planId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestPlanAsync(
      Guid project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("0e292477-a0c2-47f3-a9b6-34f153d627f4"), (object) new
      {
        project = project,
        planId = planId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestPlan> GetTestPlanByIdAsync(
      string project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestPlan>(new HttpMethod("GET"), new Guid("0e292477-a0c2-47f3-a9b6-34f153d627f4"), (object) new
      {
        project = project,
        planId = planId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestPlan> GetTestPlanByIdAsync(
      Guid project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestPlan>(new HttpMethod("GET"), new Guid("0e292477-a0c2-47f3-a9b6-34f153d627f4"), (object) new
      {
        project = project,
        planId = planId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestPlan>> GetTestPlansAsync(
      string project,
      string owner = null,
      string continuationToken = null,
      bool? includePlanDetails = null,
      bool? filterActivePlans = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0e292477-a0c2-47f3-a9b6-34f153d627f4");
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
      return this.SendAsync<PagedList<TestPlan>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestPlan>> GetTestPlansAsync(
      Guid project,
      string owner = null,
      string continuationToken = null,
      bool? includePlanDetails = null,
      bool? filterActivePlans = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0e292477-a0c2-47f3-a9b6-34f153d627f4");
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
      return this.SendAsync<PagedList<TestPlan>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestPlan> UpdateTestPlanAsync(
      TestPlanUpdateParams testPlanUpdateParams,
      string project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0e292477-a0c2-47f3-a9b6-34f153d627f4");
      object obj1 = (object) new
      {
        project = project,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestPlanUpdateParams>(testPlanUpdateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestPlan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestPlan> UpdateTestPlanAsync(
      TestPlanUpdateParams testPlanUpdateParams,
      Guid project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0e292477-a0c2-47f3-a9b6-34f153d627f4");
      object obj1 = (object) new
      {
        project = project,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestPlanUpdateParams>(testPlanUpdateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestPlan>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<SuiteEntry>> GetSuiteEntriesAsync(
      string project,
      int suiteId,
      SuiteEntryTypes? suiteEntryType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d6733edf-72f1-4252-925b-c560dfe9b75a");
      object routeValues = (object) new
      {
        project = project,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (suiteEntryType.HasValue)
        keyValuePairList.Add(nameof (suiteEntryType), suiteEntryType.Value.ToString());
      return this.SendAsync<List<SuiteEntry>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SuiteEntry>> GetSuiteEntriesAsync(
      Guid project,
      int suiteId,
      SuiteEntryTypes? suiteEntryType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d6733edf-72f1-4252-925b-c560dfe9b75a");
      object routeValues = (object) new
      {
        project = project,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (suiteEntryType.HasValue)
        keyValuePairList.Add(nameof (suiteEntryType), suiteEntryType.Value.ToString());
      return this.SendAsync<List<SuiteEntry>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SuiteEntry>> ReorderSuiteEntriesAsync(
      IEnumerable<SuiteEntryUpdateParams> suiteEntries,
      string project,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d6733edf-72f1-4252-925b-c560dfe9b75a");
      object obj1 = (object) new
      {
        project = project,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SuiteEntryUpdateParams>>(suiteEntries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<SuiteEntry>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<SuiteEntry>> ReorderSuiteEntriesAsync(
      IEnumerable<SuiteEntryUpdateParams> suiteEntries,
      Guid project,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d6733edf-72f1-4252-925b-c560dfe9b75a");
      object obj1 = (object) new
      {
        project = project,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SuiteEntryUpdateParams>>(suiteEntries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<SuiteEntry>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestSuite>> CreateBulkTestSuitesAsync(
      TestSuiteCreateParams[] testSuiteCreateParams,
      string project,
      int planId,
      int parentSuiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1e58fbe6-1761-43ce-97f6-5492ec9d438e");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        parentSuiteId = parentSuiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSuiteCreateParams[]>(testSuiteCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestSuite>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<TestSuite>> CreateBulkTestSuitesAsync(
      TestSuiteCreateParams[] testSuiteCreateParams,
      Guid project,
      int planId,
      int parentSuiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1e58fbe6-1761-43ce-97f6-5492ec9d438e");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        parentSuiteId = parentSuiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSuiteCreateParams[]>(testSuiteCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestSuite>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestSuite> CreateTestSuiteAsync(
      TestSuiteCreateParams testSuiteCreateParams,
      string project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1046d5d3-ab61-4ca7-a65a-36118a978256");
      object obj1 = (object) new
      {
        project = project,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSuiteCreateParams>(testSuiteCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestSuite>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestSuite> CreateTestSuiteAsync(
      TestSuiteCreateParams testSuiteCreateParams,
      Guid project,
      int planId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1046d5d3-ab61-4ca7-a65a-36118a978256");
      object obj1 = (object) new
      {
        project = project,
        planId = planId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSuiteCreateParams>(testSuiteCreateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestSuite>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTestSuiteAsync(
      string project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("1046d5d3-ab61-4ca7-a65a-36118a978256"), (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestSuiteAsync(
      Guid project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("1046d5d3-ab61-4ca7-a65a-36118a978256"), (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestSuite> GetTestSuiteByIdAsync(
      string project,
      int planId,
      int suiteId,
      SuiteExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1046d5d3-ab61-4ca7-a65a-36118a978256");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add(nameof (expand), expand.Value.ToString());
      return this.SendAsync<TestSuite>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestSuite> GetTestSuiteByIdAsync(
      Guid project,
      int planId,
      int suiteId,
      SuiteExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1046d5d3-ab61-4ca7-a65a-36118a978256");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add(nameof (expand), expand.Value.ToString());
      return this.SendAsync<TestSuite>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestSuite>> GetTestSuitesForPlanAsync(
      string project,
      int planId,
      SuiteExpand? expand = null,
      string continuationToken = null,
      bool? asTreeView = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1046d5d3-ab61-4ca7-a65a-36118a978256");
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
      return this.SendAsync<PagedList<TestSuite>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestSuite>> GetTestSuitesForPlanAsync(
      Guid project,
      int planId,
      SuiteExpand? expand = null,
      string continuationToken = null,
      bool? asTreeView = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1046d5d3-ab61-4ca7-a65a-36118a978256");
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
      return this.SendAsync<PagedList<TestSuite>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestSuite> UpdateTestSuiteAsync(
      TestSuiteUpdateParams testSuiteUpdateParams,
      string project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1046d5d3-ab61-4ca7-a65a-36118a978256");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSuiteUpdateParams>(testSuiteUpdateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestSuite>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestSuite> UpdateTestSuiteAsync(
      TestSuiteUpdateParams testSuiteUpdateParams,
      Guid project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1046d5d3-ab61-4ca7-a65a-36118a978256");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestSuiteUpdateParams>(testSuiteUpdateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestSuite>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestSuite>> GetSuitesByTestCaseIdAsync(
      int testCaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a4080e84-f17b-4fad-84f1-7960b6525bf2");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testCaseId), testCaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<TestSuite>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestCase>> AddTestCasesToSuiteAsync(
      IEnumerable<SuiteTestCaseCreateUpdateParameters> suiteTestCaseCreateUpdateParameters,
      string project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SuiteTestCaseCreateUpdateParameters>>(suiteTestCaseCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCase>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestCase>> AddTestCasesToSuiteAsync(
      IEnumerable<SuiteTestCaseCreateUpdateParameters> suiteTestCaseCreateUpdateParameters,
      Guid project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SuiteTestCaseCreateUpdateParameters>>(suiteTestCaseCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCase>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestCase>> GetTestCaseAsync(
      string project,
      int planId,
      int suiteId,
      string testCaseId,
      string witFields = null,
      bool? returnIdentityRef = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId,
        testCaseId = testCaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (witFields != null)
        keyValuePairList.Add(nameof (witFields), witFields);
      if (returnIdentityRef.HasValue)
        keyValuePairList.Add(nameof (returnIdentityRef), returnIdentityRef.Value.ToString());
      return this.SendAsync<List<TestCase>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestCase>> GetTestCaseAsync(
      Guid project,
      int planId,
      int suiteId,
      string testCaseId,
      string witFields = null,
      bool? returnIdentityRef = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId,
        testCaseId = testCaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (witFields != null)
        keyValuePairList.Add(nameof (witFields), witFields);
      if (returnIdentityRef.HasValue)
        keyValuePairList.Add(nameof (returnIdentityRef), returnIdentityRef.Value.ToString());
      return this.SendAsync<List<TestCase>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestCase>> GetTestCaseListAsync(
      string project,
      int planId,
      int suiteId,
      string testIds = null,
      string configurationIds = null,
      string witFields = null,
      string continuationToken = null,
      bool? returnIdentityRef = null,
      bool? expand = null,
      ExcludeFlags? excludeFlags = null,
      bool? isRecursive = null,
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
      bool flag;
      if (returnIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = returnIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (returnIdentityRef), str);
      }
      if (expand.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = expand.Value;
        string str = flag.ToString();
        collection.Add(nameof (expand), str);
      }
      if (excludeFlags.HasValue)
        keyValuePairList.Add(nameof (excludeFlags), excludeFlags.Value.ToString());
      if (isRecursive.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isRecursive.Value;
        string str = flag.ToString();
        collection.Add(nameof (isRecursive), str);
      }
      return this.SendAsync<PagedList<TestCase>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestCase>> GetTestCaseListAsync(
      Guid project,
      int planId,
      int suiteId,
      string testIds = null,
      string configurationIds = null,
      string witFields = null,
      string continuationToken = null,
      bool? returnIdentityRef = null,
      bool? expand = null,
      ExcludeFlags? excludeFlags = null,
      bool? isRecursive = null,
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
      bool flag;
      if (returnIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = returnIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (returnIdentityRef), str);
      }
      if (expand.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = expand.Value;
        string str = flag.ToString();
        collection.Add(nameof (expand), str);
      }
      if (excludeFlags.HasValue)
        keyValuePairList.Add(nameof (excludeFlags), excludeFlags.Value.ToString());
      if (isRecursive.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isRecursive.Value;
        string str = flag.ToString();
        collection.Add(nameof (isRecursive), str);
      }
      return this.SendAsync<PagedList<TestCase>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task RemoveTestCasesFromSuiteAsync(
      string project,
      int planId,
      int suiteId,
      string testCaseIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestPlanHttpClientBase planHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testCaseIds), testCaseIds);
      using (await planHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task RemoveTestCasesFromSuiteAsync(
      Guid project,
      int planId,
      int suiteId,
      string testCaseIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestPlanHttpClientBase planHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testCaseIds), testCaseIds);
      using (await planHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task RemoveTestCasesListFromSuiteAsync(
      string project,
      int planId,
      int suiteId,
      string testIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestPlanHttpClientBase planHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testIds), testIds);
      using (await planHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task RemoveTestCasesListFromSuiteAsync(
      Guid project,
      int planId,
      int suiteId,
      string testIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestPlanHttpClientBase planHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object routeValues = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testIds), testIds);
      using (await planHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<TestCase>> UpdateSuiteTestCasesAsync(
      IEnumerable<SuiteTestCaseCreateUpdateParameters> suiteTestCaseCreateUpdateParameters,
      string project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SuiteTestCaseCreateUpdateParameters>>(suiteTestCaseCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCase>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<TestCase>> UpdateSuiteTestCasesAsync(
      IEnumerable<SuiteTestCaseCreateUpdateParameters> suiteTestCaseCreateUpdateParameters,
      Guid project,
      int planId,
      int suiteId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a9bd61ac-45cf-4d13-9441-43dcd01edf8d");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SuiteTestCaseCreateUpdateParameters>>(suiteTestCaseCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestCase>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CloneTestCaseOperationInformation> CloneTestCaseAsync(
      CloneTestCaseParams cloneRequestBody,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("529b2b8d-82f4-4893-b1e4-1e74ea534673");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CloneTestCaseParams>(cloneRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CloneTestCaseOperationInformation>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CloneTestCaseOperationInformation> CloneTestCaseAsync(
      CloneTestCaseParams cloneRequestBody,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("529b2b8d-82f4-4893-b1e4-1e74ea534673");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CloneTestCaseParams>(cloneRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CloneTestCaseOperationInformation>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<CloneTestCaseOperationInformation> GetTestCaseCloneInformationAsync(
      string project,
      int cloneOperationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CloneTestCaseOperationInformation>(new HttpMethod("GET"), new Guid("529b2b8d-82f4-4893-b1e4-1e74ea534673"), (object) new
      {
        project = project,
        cloneOperationId = cloneOperationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CloneTestCaseOperationInformation> GetTestCaseCloneInformationAsync(
      Guid project,
      int cloneOperationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CloneTestCaseOperationInformation>(new HttpMethod("GET"), new Guid("529b2b8d-82f4-4893-b1e4-1e74ea534673"), (object) new
      {
        project = project,
        cloneOperationId = cloneOperationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> ExportTestCasesAsync(
      ExportTestCaseParams exportTestCaseRequestBody,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestPlanHttpClientBase planHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("3b9d1c87-6b1a-4e7d-9e7d-1a8e543112bb");
      object routeValues = (object) new{ project = project };
      HttpContent content = (HttpContent) new ObjectContent<ExportTestCaseParams>(exportTestCaseRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await planHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), content, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await planHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> ExportTestCasesAsync(
      ExportTestCaseParams exportTestCaseRequestBody,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestPlanHttpClientBase planHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("3b9d1c87-6b1a-4e7d-9e7d-1a8e543112bb");
      object routeValues = (object) new{ project = project };
      HttpContent content = (HttpContent) new ObjectContent<ExportTestCaseParams>(exportTestCaseRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await planHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), content, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await planHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task DeleteTestCaseAsync(
      string project,
      int testCaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("29006fb5-816b-4ff7-a329-599943569229"), (object) new
      {
        project = project,
        testCaseId = testCaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestCaseAsync(
      Guid project,
      int testCaseId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("29006fb5-816b-4ff7-a329-599943569229"), (object) new
      {
        project = project,
        testCaseId = testCaseId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<CloneTestPlanOperationInformation> CloneTestPlanAsync(
      CloneTestPlanParams cloneRequestBody,
      string project,
      bool? deepClone = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e65df662-d8a3-46c7-ae1c-14e2d4df57e1");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CloneTestPlanParams>(cloneRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (deepClone.HasValue)
        collection.Add(nameof (deepClone), deepClone.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CloneTestPlanOperationInformation>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<CloneTestPlanOperationInformation> CloneTestPlanAsync(
      CloneTestPlanParams cloneRequestBody,
      Guid project,
      bool? deepClone = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e65df662-d8a3-46c7-ae1c-14e2d4df57e1");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CloneTestPlanParams>(cloneRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (deepClone.HasValue)
        collection.Add(nameof (deepClone), deepClone.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CloneTestPlanOperationInformation>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<CloneTestPlanOperationInformation> GetCloneInformationAsync(
      string project,
      int cloneOperationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CloneTestPlanOperationInformation>(new HttpMethod("GET"), new Guid("e65df662-d8a3-46c7-ae1c-14e2d4df57e1"), (object) new
      {
        project = project,
        cloneOperationId = cloneOperationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CloneTestPlanOperationInformation> GetCloneInformationAsync(
      Guid project,
      int cloneOperationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CloneTestPlanOperationInformation>(new HttpMethod("GET"), new Guid("e65df662-d8a3-46c7-ae1c-14e2d4df57e1"), (object) new
      {
        project = project,
        cloneOperationId = cloneOperationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestPoint>> GetPointsAsync(
      string project,
      int planId,
      int suiteId,
      string pointId,
      bool? returnIdentityRef = null,
      bool? includePointDetails = null,
      object userState = null,
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
      keyValuePairList.Add(nameof (pointId), pointId);
      bool flag;
      if (returnIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = returnIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (returnIdentityRef), str);
      }
      if (includePointDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includePointDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includePointDetails), str);
      }
      return this.SendAsync<List<TestPoint>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestPoint>> GetPointsAsync(
      Guid project,
      int planId,
      int suiteId,
      string pointId,
      bool? returnIdentityRef = null,
      bool? includePointDetails = null,
      object userState = null,
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
      keyValuePairList.Add(nameof (pointId), pointId);
      bool flag;
      if (returnIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = returnIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (returnIdentityRef), str);
      }
      if (includePointDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includePointDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includePointDetails), str);
      }
      return this.SendAsync<List<TestPoint>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestPoint>> GetPointsListAsync(
      string project,
      int planId,
      int suiteId,
      string testPointIds = null,
      string testCaseId = null,
      string continuationToken = null,
      bool? returnIdentityRef = null,
      bool? includePointDetails = null,
      bool? isRecursive = null,
      object userState = null,
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
      bool flag;
      if (returnIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = returnIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (returnIdentityRef), str);
      }
      if (includePointDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includePointDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includePointDetails), str);
      }
      if (isRecursive.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isRecursive.Value;
        string str = flag.ToString();
        collection.Add(nameof (isRecursive), str);
      }
      return this.SendAsync<PagedList<TestPoint>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestPoint>> GetPointsListAsync(
      Guid project,
      int planId,
      int suiteId,
      string testPointIds = null,
      string testCaseId = null,
      string continuationToken = null,
      bool? returnIdentityRef = null,
      bool? includePointDetails = null,
      bool? isRecursive = null,
      object userState = null,
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
      bool flag;
      if (returnIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = returnIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (returnIdentityRef), str);
      }
      if (includePointDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includePointDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includePointDetails), str);
      }
      if (isRecursive.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isRecursive.Value;
        string str = flag.ToString();
        collection.Add(nameof (isRecursive), str);
      }
      return this.SendAsync<PagedList<TestPoint>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TestPoint>> UpdateTestPointsAsync(
      IEnumerable<TestPointUpdateParams> testPointUpdateParams,
      string project,
      int planId,
      int suiteId,
      bool? includePointDetails = null,
      bool? returnIdentityRef = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("52df686e-bae4-4334-b0ee-b6cf4e6f6b73");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestPointUpdateParams>>(testPointUpdateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includePointDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includePointDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includePointDetails), str);
      }
      if (returnIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = returnIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (returnIdentityRef), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestPoint>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<List<TestPoint>> UpdateTestPointsAsync(
      IEnumerable<TestPointUpdateParams> testPointUpdateParams,
      Guid project,
      int planId,
      int suiteId,
      bool? includePointDetails = null,
      bool? returnIdentityRef = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("52df686e-bae4-4334-b0ee-b6cf4e6f6b73");
      object obj1 = (object) new
      {
        project = project,
        planId = planId,
        suiteId = suiteId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<TestPointUpdateParams>>(testPointUpdateParams, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includePointDetails.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includePointDetails.Value;
        string str = flag.ToString();
        collection.Add(nameof (includePointDetails), str);
      }
      if (returnIdentityRef.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = returnIdentityRef.Value;
        string str = flag.ToString();
        collection.Add(nameof (returnIdentityRef), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<TestPoint>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<CloneTestSuiteOperationInformation> CloneTestSuiteAsync(
      CloneTestSuiteParams cloneRequestBody,
      string project,
      bool? deepClone = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("181d4c97-0e98-4ee2-ad6a-4cada675e555");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CloneTestSuiteParams>(cloneRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (deepClone.HasValue)
        collection.Add(nameof (deepClone), deepClone.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CloneTestSuiteOperationInformation>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<CloneTestSuiteOperationInformation> CloneTestSuiteAsync(
      CloneTestSuiteParams cloneRequestBody,
      Guid project,
      bool? deepClone = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("181d4c97-0e98-4ee2-ad6a-4cada675e555");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<CloneTestSuiteParams>(cloneRequestBody, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (deepClone.HasValue)
        collection.Add(nameof (deepClone), deepClone.Value.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CloneTestSuiteOperationInformation>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<CloneTestSuiteOperationInformation> GetSuiteCloneInformationAsync(
      string project,
      int cloneOperationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CloneTestSuiteOperationInformation>(new HttpMethod("GET"), new Guid("181d4c97-0e98-4ee2-ad6a-4cada675e555"), (object) new
      {
        project = project,
        cloneOperationId = cloneOperationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<CloneTestSuiteOperationInformation> GetSuiteCloneInformationAsync(
      Guid project,
      int cloneOperationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CloneTestSuiteOperationInformation>(new HttpMethod("GET"), new Guid("181d4c97-0e98-4ee2-ad6a-4cada675e555"), (object) new
      {
        project = project,
        cloneOperationId = cloneOperationId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestVariable> CreateTestVariableAsync(
      TestVariableCreateUpdateParameters testVariableCreateUpdateParameters,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2c61fac6-ac4e-45a5-8c38-1c2b8fd8ea6c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestVariableCreateUpdateParameters>(testVariableCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestVariable>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestVariable> CreateTestVariableAsync(
      TestVariableCreateUpdateParameters testVariableCreateUpdateParameters,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2c61fac6-ac4e-45a5-8c38-1c2b8fd8ea6c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestVariableCreateUpdateParameters>(testVariableCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestVariable>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTestVariableAsync(
      string project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("2c61fac6-ac4e-45a5-8c38-1c2b8fd8ea6c"), (object) new
      {
        project = project,
        testVariableId = testVariableId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteTestVariableAsync(
      Guid project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("2c61fac6-ac4e-45a5-8c38-1c2b8fd8ea6c"), (object) new
      {
        project = project,
        testVariableId = testVariableId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestVariable> GetTestVariableByIdAsync(
      string project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestVariable>(new HttpMethod("GET"), new Guid("2c61fac6-ac4e-45a5-8c38-1c2b8fd8ea6c"), (object) new
      {
        project = project,
        testVariableId = testVariableId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestVariable> GetTestVariableByIdAsync(
      Guid project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestVariable>(new HttpMethod("GET"), new Guid("2c61fac6-ac4e-45a5-8c38-1c2b8fd8ea6c"), (object) new
      {
        project = project,
        testVariableId = testVariableId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestVariable>> GetTestVariablesAsync(
      string project,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2c61fac6-ac4e-45a5-8c38-1c2b8fd8ea6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestVariable>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PagedList<TestVariable>> GetTestVariablesAsync(
      Guid project,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2c61fac6-ac4e-45a5-8c38-1c2b8fd8ea6c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      return this.SendAsync<PagedList<TestVariable>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestVariable> UpdateTestVariableAsync(
      TestVariableCreateUpdateParameters testVariableCreateUpdateParameters,
      string project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2c61fac6-ac4e-45a5-8c38-1c2b8fd8ea6c");
      object obj1 = (object) new
      {
        project = project,
        testVariableId = testVariableId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestVariableCreateUpdateParameters>(testVariableCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestVariable>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestVariable> UpdateTestVariableAsync(
      TestVariableCreateUpdateParameters testVariableCreateUpdateParameters,
      Guid project,
      int testVariableId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("2c61fac6-ac4e-45a5-8c38-1c2b8fd8ea6c");
      object obj1 = (object) new
      {
        project = project,
        testVariableId = testVariableId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestVariableCreateUpdateParameters>(testVariableCreateUpdateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestVariable>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
