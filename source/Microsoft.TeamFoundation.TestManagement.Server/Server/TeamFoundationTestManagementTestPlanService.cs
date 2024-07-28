// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementTestPlanService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TeamFoundationTestManagementTestPlanService : 
    TfsTestManagementService,
    ITeamFoundationTestManagementTestPlanService,
    IVssFrameworkService
  {
    public TeamFoundationTestManagementTestPlanService()
    {
    }

    public TeamFoundationTestManagementTestPlanService(
      TfsTestManagementRequestContext requestContext)
      : base((TestManagementRequestContext) requestContext)
    {
    }

    public List<TestPlan> GetPlanFromIds(
      IVssRequestContext requestContext,
      List<int> testPlanIds,
      string projectName)
    {
      return TestPlan.Fetch((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(requestContext), testPlanIds.Select<int, IdAndRev>((Func<int, IdAndRev>) (planId => new IdAndRev()
      {
        Id = planId
      })).ToArray<IdAndRev>(), new List<int>(), projectName, false, false);
    }

    public List<TestPlanRecord> QueryTestPlansByChangedDate(
      IVssRequestContext requestContext,
      int projectId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      string str = "TeamFoundationTestManagementTestPlanService.QueryTestPlansByChangedDate";
      try
      {
        requestContext.TraceEnter(1015007, "TestManagement", "BusinessLayer", str);
        using (PerfManager.Measure(requestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          using (TestPlanningDatabase replicaAwareComponent = TestPlanningDatabase.CreateReadReplicaAwareComponent(requestContext))
            return replicaAwareComponent.QueryTestPlansByChangedDate(projectId, batchSize, fromDate, out toDate, dataSource);
        }
      }
      finally
      {
        requestContext.TraceLeave(1015005, "TestManagement", "BusinessLayer", str);
      }
    }

    public bool RestoreSoftDeletedTestPlan(
      IVssRequestContext requestContext,
      Guid projectId,
      int testPlanId,
      bool restoreWorkItem,
      bool restoreForTestPlan)
    {
      string methodName = "TeamFoundationTestManagementTestPlanService.RestoreSoftDeletedTestPlan";
      requestContext.TraceEnter(1015051, "TestManagement", "BusinessLayer", methodName);
      try
      {
        using (TestPlanningDatabase replicaAwareComponent = TestPlanningDatabase.CreateReadReplicaAwareComponent(requestContext))
          return replicaAwareComponent.RestoreSoftDeletedTestPlan(projectId, testPlanId, restoreWorkItem, restoreForTestPlan);
      }
      catch (Exception ex)
      {
        requestContext.TraceException("BusinessLayer", ex);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(1015051, "TestManagement", "BusinessLayer", methodName);
      }
    }

    public bool RestoreSoftDeletedTestSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      int testPlanId,
      int testSuiteIdToRecover,
      bool restoreWorkItem,
      bool restoreForTestPlan)
    {
      string methodName = "TeamFoundationTestManagementTestPlanService.RestoreSoftDeletedTestSuite";
      requestContext.TraceEnter(1015052, "TestManagement", "BusinessLayer", methodName);
      try
      {
        using (TestPlanningDatabase replicaAwareComponent = TestPlanningDatabase.CreateReadReplicaAwareComponent(requestContext))
          return replicaAwareComponent.RestoreSoftDeletedTestSuite(projectId, testPlanId, testSuiteIdToRecover, restoreWorkItem, restoreForTestPlan);
      }
      catch (Exception ex)
      {
        requestContext.TraceException("BusinessLayer", ex);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(1015052, "TestManagement", "BusinessLayer", methodName);
      }
    }
  }
}
