// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TestExecution.WebApi.TestExecutionHttpClientBase
// Assembly: Microsoft.Azure.Pipelines.TestExecution.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D8DB80F5-61CE-408E-B0A6-21067A361E64
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.TestExecution.WebApi.dll

using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.TestExecution.WebApi
{
  [ResourceArea("75CAD6D7-EE47-4E86-9A06-DB41AE372B00")]
  public abstract class TestExecutionHttpClientBase : VssHttpClientBase
  {
    public TestExecutionHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TestExecutionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TestExecutionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TestExecutionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TestExecutionHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<TestAgent> CreateAgentAsync(
      TestAgent testAgent,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9782642a-22b0-42bc-8ec1-1f4725aa20c2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAgent>(testAgent, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestAgent>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestAgent> CreateAgentAsync(
      TestAgent testAgent,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9782642a-22b0-42bc-8ec1-1f4725aa20c2");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAgent>(testAgent, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestAgent>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteAgentAsync(
      string project,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9782642a-22b0-42bc-8ec1-1f4725aa20c2"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteAgentAsync(
      Guid project,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9782642a-22b0-42bc-8ec1-1f4725aa20c2"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestAgent> GetAgentAsync(
      string project,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestAgent>(new HttpMethod("GET"), new Guid("9782642a-22b0-42bc-8ec1-1f4725aa20c2"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestAgent> GetAgentAsync(
      Guid project,
      int id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestAgent>(new HttpMethod("GET"), new Guid("9782642a-22b0-42bc-8ec1-1f4725aa20c2"), (object) new
      {
        project = project,
        id = id
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestExecutionServiceCommand> GetCommandAsync(
      string project,
      int? testAgentId = null,
      long? commandId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestExecutionServiceCommand>(new HttpMethod("GET"), new Guid("daa073ea-7f66-4025-949a-9a052695ab17"), (object) new
      {
        project = project,
        testAgentId = testAgentId,
        commandId = commandId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestExecutionServiceCommand> GetCommandAsync(
      Guid project,
      int? testAgentId = null,
      long? commandId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestExecutionServiceCommand>(new HttpMethod("GET"), new Guid("daa073ea-7f66-4025-949a-9a052695ab17"), (object) new
      {
        project = project,
        testAgentId = testAgentId,
        commandId = commandId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DistributedTestRun> UpdateTestRunAsync(
      DistributedTestRun distributedTestRun,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("01627d68-de6c-4e6f-b6b5-afebdcb81f19");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DistributedTestRun>(distributedTestRun, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DistributedTestRun>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DistributedTestRun> UpdateTestRunAsync(
      DistributedTestRun distributedTestRun,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("01627d68-de6c-4e6f-b6b5-afebdcb81f19");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DistributedTestRun>(distributedTestRun, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DistributedTestRun>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestAutomationRunSlice> GetSliceAsync(
      string project,
      int testAgentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestAutomationRunSlice>(new HttpMethod("GET"), new Guid("487be43d-93c4-4b24-ba9a-d78b67370334"), (object) new
      {
        project = project,
        testAgentId = testAgentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TestAutomationRunSlice> GetSliceAsync(
      Guid project,
      int testAgentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TestAutomationRunSlice>(new HttpMethod("GET"), new Guid("487be43d-93c4-4b24-ba9a-d78b67370334"), (object) new
      {
        project = project,
        testAgentId = testAgentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateSliceAsync(
      TestAutomationRunSlice sliceDetails,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestExecutionHttpClientBase executionHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("487be43d-93c4-4b24-ba9a-d78b67370334");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAutomationRunSlice>(sliceDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TestExecutionHttpClientBase executionHttpClientBase2 = executionHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await executionHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateSliceAsync(
      TestAutomationRunSlice sliceDetails,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TestExecutionHttpClientBase executionHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("487be43d-93c4-4b24-ba9a-d78b67370334");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestAutomationRunSlice>(sliceDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      TestExecutionHttpClientBase executionHttpClientBase2 = executionHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await executionHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<TestRunExecutionConfiguration> GetRerunConfigurationAsync(
      TestRunExecutionConfiguration testRunExecutionConfiguration,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a7ba9848-dc97-4049-949d-0c1d03b4b412");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestRunExecutionConfiguration>(testRunExecutionConfiguration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestRunExecutionConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TestRunExecutionConfiguration> GetRerunConfigurationAsync(
      TestRunExecutionConfiguration testRunExecutionConfiguration,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a7ba9848-dc97-4049-949d-0c1d03b4b412");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<TestRunExecutionConfiguration>(testRunExecutionConfiguration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TestRunExecutionConfiguration>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
