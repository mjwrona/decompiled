// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.ITestManagementRunHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public interface ITestManagementRunHelper
  {
    Microsoft.TeamFoundation.TestManagement.WebApi.TestRun GetTestRun(
      TestExecutionRequestContext requestContext,
      int testRunId,
      TeamProjectReference teamProject = null,
      bool includeTestRunDetals = true);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestRun ValidateTestRunIsInProgress(
      TestExecutionRequestContext requestContext,
      int testRunId,
      TeamProjectReference teamProject = null,
      bool includeTestRunDetals = true);

    bool IsTestRunCompleted(TestExecutionRequestContext requestContext, int testRunId);

    void PopulateTestRunInformation(
      TestExecutionRequestContext context,
      TestRunInformation testRunInfo);

    void AbortTestRun(
      TestExecutionRequestContext context,
      int testRunId,
      string reason,
      Guid? abortedBy = null,
      bool timedOut = false);

    void UpdateTestRunStateToCompleted(
      TestExecutionRequestContext requestContext,
      WorkFlowJobDetails workFlowJobDetails);

    void UpdateTestRunStateToInProgress(
      TestExecutionRequestContext requestContext,
      WorkflowJobData workFlowJobData);

    void UpdateTestRunSubstate(TestExecutionRequestContext context, int testRunId, Phase phase);

    void UpdateDtlEnvironmentUrl(
      TestExecutionRequestContext context,
      int testRunId,
      string dtlEnvironmentUrl);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestRun UpdateTestRun(
      TestExecutionRequestContext context,
      int testRunId,
      RunUpdateModel runUpdateModel,
      TeamProjectReference teamProject);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings GetTestSettings(
      TestExecutionRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun);
  }
}
