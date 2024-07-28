// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointOutcomeUpdateRequestConverter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TestPointOutcomeUpdateRequestConverter
  {
    private static TestManagementRequestContext tcmContext;

    public static IList<TestPointOutcomeUpdateFromTestResultRequest> ConvertFromWebApiResult(
      IVssRequestContext context,
      string projectName,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results)
    {
      TfsTestManagementRequestContext context1 = new TfsTestManagementRequestContext(context);
      List<TestPointOutcomeUpdateFromTestResultRequest> testResultRequestList = new List<TestPointOutcomeUpdateFromTestResultRequest>();
      if (results != null)
      {
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result in (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) results)
        {
          try
          {
            TestPointOutcomeUpdateFromTestResultRequest testResultRequest1 = new TestPointOutcomeUpdateFromTestResultRequest();
            testResultRequest1.TestResultId = result.Id;
            TestPointOutcomeUpdateFromTestResultRequest testResultRequest2 = testResultRequest1;
            DateTime lastUpdatedDate = result.LastUpdatedDate;
            DateTime dateTime = result.LastUpdatedDate != DateTime.MinValue ? result.LastUpdatedDate : DateTime.UtcNow;
            testResultRequest2.LastUpdated = dateTime;
            testResultRequest1.LastUpdatedBy = result.LastUpdatedBy?.Id != null ? new Guid(result.LastUpdatedBy.Id) : context1.UserTeamFoundationId;
            testResultRequest1.ResolutionStateId = result.ResolutionStateId;
            if (result.TestPlan != null)
              testResultRequest1.TestPlanId = int.Parse(result.TestPlan.Id);
            if (result.TestPoint != null)
              testResultRequest1.TestPointId = int.Parse(result.TestPoint.Id);
            if (result.TestRun != null)
              testResultRequest1.TestRunId = int.Parse(result.TestRun.Id);
            if (!string.IsNullOrEmpty(result.FailureType))
              testResultRequest1.FailureType = new byte?(Convert.ToByte(TestManagementServiceUtility.GetFailureTypeIdFromFailureTypeName((TestManagementRequestContext) context1, result.FailureType, TestFailureType.Query(TestPointOutcomeUpdateRequestConverter.getTestManagementContext(context), -1, projectName))));
            if (!string.IsNullOrEmpty(result.Outcome))
              testResultRequest1.Outcome = new byte?(TestManagementServiceUtility.ValidateAndGetEnumValue<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(result.Outcome, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.None));
            if (!string.IsNullOrEmpty(result.State))
              testResultRequest1.State = new byte?(TestManagementServiceUtility.ValidateAndGetEnumValue<TestResultState>(result.State, TestResultState.Pending));
            testResultRequestList.Add(testResultRequest1);
          }
          catch (Exception ex)
          {
            context.TraceException("DualWriteTestPointOutcome", ex);
            return (IList<TestPointOutcomeUpdateFromTestResultRequest>) testResultRequestList;
          }
        }
      }
      return (IList<TestPointOutcomeUpdateFromTestResultRequest>) testResultRequestList;
    }

    public static IList<TestPointOutcomeUpdateFromTestResultRequest> ConvertFromPlanAndPointIds(
      IVssRequestContext context,
      string projectName,
      int testPlanId,
      IList<int> pointIds)
    {
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(context);
      List<TestPointOutcomeUpdateFromTestResultRequest> testResultRequestList = new List<TestPointOutcomeUpdateFromTestResultRequest>();
      if (testPlanId > 0 && pointIds != null)
      {
        foreach (int pointId in (IEnumerable<int>) pointIds)
        {
          try
          {
            testResultRequestList.Add(new TestPointOutcomeUpdateFromTestResultRequest()
            {
              TestResultId = 0,
              LastUpdated = DateTime.UtcNow,
              LastUpdatedBy = managementRequestContext.UserTeamFoundationId,
              ResolutionStateId = 0,
              TestPlanId = testPlanId,
              TestPointId = pointId,
              TestRunId = 0,
              FailureType = new byte?(),
              Outcome = new byte?(),
              State = new byte?()
            });
          }
          catch (Exception ex)
          {
            context.TraceException("DualWriteTestPointOutcome", ex);
            return (IList<TestPointOutcomeUpdateFromTestResultRequest>) testResultRequestList;
          }
        }
      }
      return (IList<TestPointOutcomeUpdateFromTestResultRequest>) testResultRequestList;
    }

    public static IList<TestPointOutcomeUpdateFromTestResultRequest> ConvertFromSOAPResult(
      IVssRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requests,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] responses)
    {
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(context);
      List<TestPointOutcomeUpdateFromTestResultRequest> testResultRequestList = new List<TestPointOutcomeUpdateFromTestResultRequest>();
      if (requests != null && responses != null)
      {
        Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse> dictionary = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse>) responses).ToDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse, int>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse, int>) (res => res.TestResultId));
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest request in requests)
        {
          try
          {
            TestPointOutcomeUpdateFromTestResultRequest testResultRequest1 = new TestPointOutcomeUpdateFromTestResultRequest();
            testResultRequest1.TestResultId = request.TestResultId;
            testResultRequest1.TestRunId = request.TestRunId;
            TestPointOutcomeUpdateFromTestResultRequest testResultRequest2 = testResultRequest1;
            Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse resultUpdateResponse1 = dictionary[request.TestResultId];
            DateTime dateTime = resultUpdateResponse1 != null ? resultUpdateResponse1.LastUpdated : DateTime.UtcNow;
            testResultRequest2.LastUpdated = dateTime;
            TestPointOutcomeUpdateFromTestResultRequest testResultRequest3 = testResultRequest1;
            Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse resultUpdateResponse2 = dictionary[request.TestResultId];
            Guid guid = resultUpdateResponse2 != null ? resultUpdateResponse2.LastUpdatedBy : managementRequestContext.UserTeamFoundationId;
            testResultRequest3.LastUpdatedBy = guid;
            TestPointOutcomeUpdateFromTestResultRequest testResultRequest4 = testResultRequest1;
            Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse resultUpdateResponse3 = dictionary[request.TestResultId];
            int testPlanId = resultUpdateResponse3 != null ? resultUpdateResponse3.TestPlanId : 0;
            testResultRequest4.TestPlanId = testPlanId;
            if (request.TestCaseResult != null)
            {
              testResultRequest1.ResolutionStateId = request.TestCaseResult.ResolutionStateId == -1 ? 0 : request.TestCaseResult.ResolutionStateId;
              testResultRequest1.TestPointId = request.TestCaseResult.TestPointId;
              testResultRequest1.FailureType = new byte?(request.TestCaseResult.FailureType);
              testResultRequest1.State = new byte?(request.TestCaseResult.State);
              testResultRequest1.Outcome = new byte?(request.TestCaseResult.Outcome);
            }
            testResultRequestList.Add(testResultRequest1);
          }
          catch (Exception ex)
          {
            context.TraceException("DualWriteTestPointOutcome", ex);
            return (IList<TestPointOutcomeUpdateFromTestResultRequest>) testResultRequestList;
          }
        }
      }
      return (IList<TestPointOutcomeUpdateFromTestResultRequest>) testResultRequestList;
    }

    public static void UpdateWebApiResult(
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> targetObject,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> shallowObject)
    {
      if (targetObject == null || shallowObject == null)
        return;
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> dictionary = shallowObject.ToDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, int>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, int>) (res => res.Id));
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult1 in (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) targetObject)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult2 = testCaseResult1;
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult3 = dictionary[testCaseResult1.Id];
        DateTime dateTime = testCaseResult3 != null ? testCaseResult3.LastUpdatedDate : DateTime.UtcNow;
        testCaseResult2.LastUpdatedDate = dateTime;
      }
    }

    public static void UpdateLegacyWebApiResult(
      IList<LegacyTestCaseResult> targetObject,
      IList<LegacyTestCaseResult> shallowObject)
    {
      if (targetObject == null || shallowObject == null)
        return;
      Dictionary<int, LegacyTestCaseResult> dictionary = shallowObject.ToDictionary<LegacyTestCaseResult, int>((Func<LegacyTestCaseResult, int>) (res => res.TestPointId));
      foreach (LegacyTestCaseResult legacyTestCaseResult1 in (IEnumerable<LegacyTestCaseResult>) targetObject)
      {
        LegacyTestCaseResult legacyTestCaseResult2 = legacyTestCaseResult1;
        LegacyTestCaseResult legacyTestCaseResult3 = dictionary[legacyTestCaseResult1.TestPointId];
        DateTime dateTime = legacyTestCaseResult3 != null ? legacyTestCaseResult3.LastUpdated : DateTime.UtcNow;
        legacyTestCaseResult2.LastUpdated = dateTime;
        LegacyTestCaseResult legacyTestCaseResult4 = legacyTestCaseResult1;
        LegacyTestCaseResult legacyTestCaseResult5 = dictionary[legacyTestCaseResult1.TestPointId];
        int testResultId = legacyTestCaseResult5 != null ? legacyTestCaseResult5.TestResultId : 0;
        legacyTestCaseResult4.TestResultId = testResultId;
        LegacyTestCaseResult legacyTestCaseResult6 = legacyTestCaseResult1;
        LegacyTestCaseResult legacyTestCaseResult7 = dictionary[legacyTestCaseResult1.TestPointId];
        int testRunId = legacyTestCaseResult7 != null ? legacyTestCaseResult7.TestRunId : 0;
        legacyTestCaseResult6.TestRunId = testRunId;
      }
    }

    public static void SetTestManagementContext(TestManagementRequestContext context) => TestPointOutcomeUpdateRequestConverter.tcmContext = context;

    public static TestManagementRequestContext getTestManagementContext(IVssRequestContext context) => TestPointOutcomeUpdateRequestConverter.tcmContext ?? new TestManagementRequestContext(context);
  }
}
