// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestManagementResultHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestManagementResultHelper : ITestManagementResultHelper
  {
    public List<TestCaseResult> QueryTestResultsByRun(
      TestExecutionRequestContext requestContext,
      TeamProjectReference projectReference,
      int testRunId)
    {
      return this.GetTestCaseResults(requestContext, testRunId, projectReference);
    }

    public void UpdateTestResults(
      TestExecutionRequestContext requestContext,
      TeamProjectReference projectReference,
      TestRun testRun,
      TestCaseResult[] testResults)
    {
      List<TestCaseResult> result = requestContext.TestResultsHttpClient.UpdateTestResultsAsync(testResults, projectReference.Name, testRun.Id).Result;
    }

    private List<TestCaseResult> GetTestCaseResults(
      TestExecutionRequestContext requestContext,
      int runId,
      TeamProjectReference projectReference,
      int skip)
    {
      ITestResultsHttpClient resultsHttpClient = requestContext.TestResultsHttpClient;
      string name = projectReference.Name;
      int runId1 = runId;
      int? nullable = new int?(skip);
      ResultDetails? detailsToInclude = new ResultDetails?();
      int? skip1 = nullable;
      int? top = new int?();
      CancellationToken cancellationToken = new CancellationToken();
      return resultsHttpClient.GetTestResultsAsync(name, runId1, detailsToInclude, skip1, top, cancellationToken: cancellationToken).Result;
    }

    private List<TestCaseResult> GetTestCaseResults(
      TestExecutionRequestContext requestContext,
      int runId,
      TeamProjectReference projectReference)
    {
      List<TestCaseResult> testCaseResults1 = new List<TestCaseResult>();
      int skip = 0;
      int num = 1;
      while (num != 0)
      {
        List<TestCaseResult> testCaseResults2 = this.GetTestCaseResults(requestContext, runId, projectReference, skip);
        if (testCaseResults2 != null)
        {
          testCaseResults1.AddRange((IEnumerable<TestCaseResult>) testCaseResults2);
          num = testCaseResults2.Count;
          skip += num;
        }
        else
          num = 0;
      }
      return testCaseResults1;
    }
  }
}
