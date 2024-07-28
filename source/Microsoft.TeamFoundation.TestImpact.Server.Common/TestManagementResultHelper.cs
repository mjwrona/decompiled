// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestManagementResultHelper
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  internal class TestManagementResultHelper : ITestManagementResultHelper
  {
    public IList<TestCaseResult> QueryTestResultsByRun(
      TestImpactRequestContext requestContext,
      Guid project,
      int testRunId)
    {
      try
      {
        return (IList<TestCaseResult>) this.GetTestCaseResults(requestContext, testRunId, project);
      }
      catch (Exception ex)
      {
        requestContext.RequestContext.Trace(6200450, TraceLevel.Error, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.CIServiceLayer, string.Format("Failed to get test results for {0}: error {1}", (object) testRunId, (object) ex));
        throw;
      }
    }

    public IList<ShallowTestCaseResult> GetTestResultDetailsForBuild(
      TestImpactRequestContext requestContext,
      Guid project,
      int buildId)
    {
      try
      {
        ITestResultsHttpClient resultsHttpClient = requestContext.TestResultsHttpClient;
        Guid project1 = project;
        int buildId1 = buildId;
        List<TestOutcome> outcomes = new List<TestOutcome>();
        outcomes.Add(TestOutcome.Failed);
        int? top = new int?();
        CancellationToken cancellationToken = new CancellationToken();
        return (IList<ShallowTestCaseResult>) resultsHttpClient.GetTestResultsByBuildAsync(project1, buildId1, outcomes: (IEnumerable<TestOutcome>) outcomes, top: top, cancellationToken: cancellationToken).Result;
      }
      catch (Exception ex)
      {
        requestContext.RequestContext.Trace(6200450, TraceLevel.Error, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.CIServiceLayer, string.Format("Failed to get test results for build: {0}: error {1}", (object) buildId, (object) ex));
        throw;
      }
    }

    public IList<ShallowTestCaseResult> GetTestResultDetailsForRelease(
      TestImpactRequestContext requestContext,
      Guid project,
      int releaseId,
      int releaseEnvId)
    {
      try
      {
        ITestResultsHttpClient resultsHttpClient = requestContext.TestResultsHttpClient;
        Guid project1 = project;
        int releaseId1 = releaseId;
        int? releaseEnvid = new int?(releaseEnvId);
        List<TestOutcome> outcomes = new List<TestOutcome>();
        outcomes.Add(TestOutcome.Failed);
        int? top = new int?();
        CancellationToken cancellationToken = new CancellationToken();
        return (IList<ShallowTestCaseResult>) resultsHttpClient.GetTestResultsByReleaseAsync(project1, releaseId1, releaseEnvid, outcomes: (IEnumerable<TestOutcome>) outcomes, top: top, cancellationToken: cancellationToken).Result;
      }
      catch (Exception ex)
      {
        requestContext.RequestContext.Trace(6200450, TraceLevel.Error, TestImpactServiceConstants.TestImpactArea, TestImpactServiceConstants.CIServiceLayer, string.Format("Failed to get test results for release: {0}: error {1}", (object) releaseId, (object) ex));
        throw;
      }
    }

    private List<TestCaseResult> GetTestCaseResults(
      TestImpactRequestContext requestContext,
      int runId,
      Guid project,
      int skip)
    {
      ITestResultsHttpClient resultsHttpClient = requestContext.TestResultsHttpClient;
      Guid project1 = project;
      int runId1 = runId;
      int? nullable = new int?(skip);
      ResultDetails? detailsToInclude = new ResultDetails?();
      int? skip1 = nullable;
      int? top = new int?();
      CancellationToken cancellationToken = new CancellationToken();
      return resultsHttpClient.GetTestResultsAsync(project1, runId1, detailsToInclude, skip1, top, cancellationToken: cancellationToken).Result;
    }

    private List<TestCaseResult> GetTestCaseResults(
      TestImpactRequestContext requestContext,
      int runId,
      Guid project)
    {
      List<TestCaseResult> testCaseResults1 = new List<TestCaseResult>();
      int skip = 0;
      int num = 1;
      while (num != 0)
      {
        List<TestCaseResult> testCaseResults2 = this.GetTestCaseResults(requestContext, runId, project, skip);
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
