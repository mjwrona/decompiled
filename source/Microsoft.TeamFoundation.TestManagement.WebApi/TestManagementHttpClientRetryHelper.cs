// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestManagementHttpClientRetryHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public class TestManagementHttpClientRetryHelper
  {
    private readonly TestManagementHttpClient m_httpClient;
    private readonly TestManagementRetryHelper m_retryHelper;
    private const int c_maxRetries = 5;

    public TestManagementHttpClientRetryHelper(
      TestManagementHttpClient httpClient,
      TestManagementRetryHelper retryHelper = null)
    {
      this.m_retryHelper = retryHelper == null ? new TestManagementRetryHelper(5) : retryHelper;
      this.m_httpClient = httpClient;
    }

    public Task<TestRun> CreateTestRunAsync(
      RunCreateModel runCreateModel,
      string projectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.m_retryHelper.Invoke<TestRun>((Func<Task<TestRun>>) (() => this.m_httpClient.CreateTestRunAsync(runCreateModel, projectId, userState, cancellationToken)));
    }

    public Task<TestRun> CreateTestRunAsync(
      RunCreateModel runCreateModel,
      Guid projectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.m_retryHelper.Invoke<TestRun>((Func<Task<TestRun>>) (() => this.m_httpClient.CreateTestRunAsync(runCreateModel, projectId, userState, cancellationToken)));
    }

    public Task<TestRun> UpdateTestRunAsync(
      RunUpdateModel runUpdateModel,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.m_retryHelper.Invoke<TestRun>((Func<Task<TestRun>>) (() => this.m_httpClient.UpdateTestRunAsync(runUpdateModel, project, runId, userState, cancellationToken)));
    }

    public Task<TestRun> UpdateTestRunAsync(
      RunUpdateModel runUpdateModel,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.m_retryHelper.Invoke<TestRun>((Func<Task<TestRun>>) (() => this.m_httpClient.UpdateTestRunAsync(runUpdateModel, project, runId, userState, cancellationToken)));
    }

    public Task<List<TestCaseResult>> AddTestResultsToTestRunAsync(
      TestCaseResult[] results,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.m_retryHelper.Invoke<List<TestCaseResult>>((Func<Task<List<TestCaseResult>>>) (() => this.m_httpClient.AddTestResultsToTestRunAsync(results, project, runId, userState, cancellationToken)));
    }

    public Task<List<TestCaseResult>> AddTestResultsToTestRunAsync(
      TestCaseResult[] results,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.m_retryHelper.Invoke<List<TestCaseResult>>((Func<Task<List<TestCaseResult>>>) (() => this.m_httpClient.AddTestResultsToTestRunAsync(results, project, runId, userState, cancellationToken)));
    }
  }
}
