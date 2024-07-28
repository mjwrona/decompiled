// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Test.WebApi.TestHttpClient
// Assembly: Microsoft.TeamFoundation.Test.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17829F78-DAC0-47C1-AC4C-95D401C54448
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Test.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Test.WebApi
{
  [ResourceArea("3b95fb80-fdda-4218-b60e-1052d070ae6b")]
  public class TestHttpClient : VssHttpClientBase, ITestHttpClient
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();

    static TestHttpClient()
    {
      TestHttpClient.s_translatedExceptions.Add("TestExecutionServiceInvalidOperation", typeof (TestExecutionServiceInvalidOperationException));
      TestHttpClient.s_translatedExceptions.Add("TestExecutionObjectAlreadyExists", typeof (TestExecutionObjectAlreadyExistsException));
      TestHttpClient.s_translatedExceptions.Add("TestExecutionObjectNotFoundException", typeof (TestExecutionObjectNotFoundException));
      TestHttpClient.s_translatedExceptions.Add("TestExecutionAccessDeniedException", typeof (TestExecutionAccessDeniedException));
    }

    public TestHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<TestAgent> CreateAgentAsync(TestAgent testAgent) => this.SendAsync<TestAgent>(HttpMethod.Post, TestExecutionServiceResourceIds.AgentLocationId, content: (HttpContent) new ObjectContent<TestAgent>(testAgent, this.Formatter));

    public Task<TestAgent> GetAgentAsync(int id) => this.SendAsync<TestAgent>(HttpMethod.Get, TestExecutionServiceResourceIds.AgentLocationId, (object) new
    {
      id = id
    });

    public Task DeleteAgentAsync(int id) => (Task) this.SendAsync(HttpMethod.Delete, TestExecutionServiceResourceIds.AgentLocationId, (object) new
    {
      id = id
    });

    public Task<TestAutomationRunSlice> GetSliceAsync(int testAgentId)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (testAgentId), testAgentId.ToString());
      return this.SendAsync<TestAutomationRunSlice>(HttpMethod.Get, TestExecutionServiceResourceIds.SliceLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList);
    }

    public Task UpdateSliceAsync(TestAutomationRunSlice sliceUpdatePackage) => (Task) this.SendAsync(new HttpMethod("PATCH"), TestExecutionServiceResourceIds.SliceLocationId, content: (HttpContent) new ObjectContent<TestAutomationRunSlice>(sliceUpdatePackage, this.Formatter));

    public Task<TestExecutionServiceCommand> GetCommandAsync(
      int testAgentId,
      long? lastCommandId = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestExecutionServiceCommand>(HttpMethod.Get, TestExecutionServiceResourceIds.CommandLocationId, (object) new
      {
        testagentId = testAgentId,
        commandId = lastCommandId
      }, cancellationToken: cancellationToken);
    }

    public virtual Task<DistributedTestRun> UpdateTestRunAsync(
      DistributedTestRun distributedTestRun,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("b7c4fe2a-9dd1-4dae-8b77-8412002de5a4");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DistributedTestRun>(distributedTestRun, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.1-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DistributedTestRun>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<TestRunExecutionConfiguration> GetRerunConfigurationAsync(
      TestRunExecutionConfiguration testRunExecutionConfiguration,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("30421b98-ac6a-48ad-a2bf-0cad4528183f");
      HttpContent httpContent = (HttpContent) new ObjectContent<TestRunExecutionConfiguration>(testRunExecutionConfiguration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("4.1-preview.1");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestRunExecutionConfiguration>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<TestExecutionControlOptions> GetTestExecutionControlOptionsAsync(
      string envUrl,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestExecutionControlOptions>(new HttpMethod("GET"), new Guid("cbd7e2a6-a3ba-4c32-825f-2f48896ccca7"), (object) new
      {
        envUrl = envUrl
      }, new ApiResourceVersion(5.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public static int LongPollTimeOutForMessageQueueInSeconds => 10;

    public HttpClient HttpClient => this.Client;

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) TestHttpClient.s_translatedExceptions;
  }
}
