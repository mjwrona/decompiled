// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultPlannedTestResultsHelper
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
  public class DefaultPlannedTestResultsHelper : IPlannedTestResultsHelper
  {
    public void UpdatePlannedResultDetails(
      IVssRequestContext tfsRequestContext,
      string teamProjectName,
      int testPlanId,
      bool isAutomated,
      List<Tuple<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResult>> resultsMap)
    {
    }

    public Dictionary<int, string> GetTestPlanTitles(
      TestManagementRequestContext context,
      Guid projectId,
      List<int> testPlanIds)
    {
      return (Dictionary<int, string>) null;
    }

    public void PopulateTestPointDetails(
      Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultDataContracts,
      List<Tuple<TestCaseResultIdentifier, int, IdAndRev>> testResultTupleList,
      string projectName)
    {
      if (testResultTupleList == null)
        return;
      foreach (Tuple<TestCaseResultIdentifier, int, IdAndRev> testResultTuple in testResultTupleList)
      {
        if (testResultTuple.Item2 > 0)
          testCaseResultDataContracts[testResultTuple.Item1].TestPlan = new ShallowReference()
          {
            Id = testResultTuple.Item2.ToString()
          };
        if (testResultTuple.Item3 != null && testResultTuple.Item3.Id > 0)
          testCaseResultDataContracts[testResultTuple.Item1].TestPoint = new ShallowReference()
          {
            Id = testResultTuple.Item3.Id.ToString()
          };
        this.SecureTestResultWebApiObject(testCaseResultDataContracts[testResultTuple.Item1]);
      }
    }

    public void PopulateTestSuiteDetails(List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults, string projectName)
    {
    }

    public ShallowReference GetShallowTestPlan(
      TestManagementRequestContext context,
      string projectName,
      int testPlanId)
    {
      return new ShallowReference()
      {
        Id = testPlanId.ToString()
      };
    }

    public Dictionary<int, ShallowReference> GetShallowTestPlans(
      TestManagementRequestContext context,
      string projectName,
      List<int> testPlanIds)
    {
      return (Dictionary<int, ShallowReference>) null;
    }

    public string GetTestPlanIteration(
      TestManagementRequestContext context,
      string projectName,
      int testPlanId)
    {
      return (string) null;
    }

    private void SecureTestResultWebApiObject(Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testResult) => testResult.EnsureSecureObject();
  }
}
