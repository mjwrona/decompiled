// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointOutcomeHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestPointOutcomeHelper : ITestPointOutcomeHelper
  {
    public void UpdateTestPointOutcomeFromWebApi(
      IVssRequestContext context,
      string projectName,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results)
    {
      if (!this.IsDualWriteEnabled(context))
        return;
      IList<TestPointOutcomeUpdateFromTestResultRequest> requests = TestPointOutcomeUpdateRequestConverter.ConvertFromWebApiResult(context, projectName, results);
      this.UpdateTestPointOutcome(context, projectName, requests);
    }

    public void UpdateTestPointOutcomeWithoutResult(
      IVssRequestContext context,
      string projectName,
      int testPlanId,
      IList<int> pointIds)
    {
      if (!this.IsDualWriteEnabled(context))
        return;
      IList<TestPointOutcomeUpdateFromTestResultRequest> requests = TestPointOutcomeUpdateRequestConverter.ConvertFromPlanAndPointIds(context, projectName, testPlanId, pointIds);
      this.UpdateTestPointOutcome(context, projectName, requests);
    }

    public void UpdateTestPointOutcomeFromSOAPRequest(
      IVssRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requests,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] responses)
    {
      if (!this.IsDualWriteEnabled(context))
        return;
      IList<TestPointOutcomeUpdateFromTestResultRequest> requests1 = TestPointOutcomeUpdateRequestConverter.ConvertFromSOAPResult(context, projectName, requests, responses);
      this.UpdateTestPointOutcome(context, projectName, requests1);
    }

    public bool IsDualWriteEnabled(IVssRequestContext context) => context.IsFeatureEnabled("TestManagement.Server.EnableDualWriteForTestPoint");

    private void UpdateTestPointOutcome(
      IVssRequestContext context,
      string projectName,
      IList<TestPointOutcomeUpdateFromTestResultRequest> requests)
    {
      if (!this.IsPlannedTestRun(requests))
        return;
      context.GetService<ITeamFoundationTestManagementOutcomeService>().UpdateTestPointOutcome(context, projectName, requests);
    }

    private bool IsPlannedTestRun(
      IList<TestPointOutcomeUpdateFromTestResultRequest> requests)
    {
      return !requests.Where<TestPointOutcomeUpdateFromTestResultRequest>((Func<TestPointOutcomeUpdateFromTestResultRequest, bool>) (res => res.TestPlanId == 0)).Any<TestPointOutcomeUpdateFromTestResultRequest>();
    }
  }
}
