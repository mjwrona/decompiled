// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IPlannedTestResultsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IPlannedTestResultsHelper
  {
    void UpdatePlannedResultDetails(
      IVssRequestContext tfsRequestContext,
      string teamProjectName,
      int testPlanId,
      bool isAutomated,
      List<Tuple<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResult>> resultsMap);

    Dictionary<int, string> GetTestPlanTitles(
      TestManagementRequestContext context,
      Guid projectId,
      List<int> testPlanIds);

    void PopulateTestPointDetails(
      Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultDataContracts,
      List<Tuple<TestCaseResultIdentifier, int, IdAndRev>> testResultTupleList,
      string projectName);

    void PopulateTestSuiteDetails(List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults, string projectName);

    ShallowReference GetShallowTestPlan(
      TestManagementRequestContext context,
      string projectName,
      int testPlanId);

    Dictionary<int, ShallowReference> GetShallowTestPlans(
      TestManagementRequestContext context,
      string projectName,
      List<int> testPlanIds);

    string GetTestPlanIteration(
      TestManagementRequestContext context,
      string projectName,
      int testPlanId);
  }
}
