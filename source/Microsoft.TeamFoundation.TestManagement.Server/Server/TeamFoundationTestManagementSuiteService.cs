// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementSuiteService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementSuiteService : 
    TfsTestManagementService,
    ITeamFoundationTestManagementSuiteService,
    IVssFrameworkService
  {
    public void UpdateSyncStatus(
      IVssRequestContext requestContext,
      GuidAndString projectId,
      Dictionary<int, DateTime> suitesLastSyncedMap)
    {
      this.ExecuteAction(requestContext, "TeamFoundationTestManagementSuiteService.UpdateSyncStatus", (Action) (() =>
      {
        ArgumentUtility.CheckForNull<Dictionary<int, DateTime>>(suitesLastSyncedMap, nameof (suitesLastSyncedMap), "Test Results");
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(requestContext))
          planningDatabase.UpdateSuiteSyncStatus(projectId.GuidId, suitesLastSyncedMap);
      }), 1015005, "TestManagement", "BusinessLayer");
    }

    public List<TestSuiteRecord> QueryTestSuitesByChangedDate(
      IVssRequestContext requestContext,
      int projectId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      string str = "TeamFoundationTestManagementSuiteService.QueryTestSuitesByChangedDate";
      try
      {
        requestContext.TraceEnter(1015005, "TestManagement", "BusinessLayer", str);
        List<TestSuiteRecord> testSuiteRecordList = new List<TestSuiteRecord>();
        using (PerfManager.Measure(requestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          using (TestPlanningDatabase replicaAwareComponent = TestPlanningDatabase.CreateReadReplicaAwareComponent(requestContext))
            testSuiteRecordList = replicaAwareComponent.QueryTestSuitesByChangedDate(projectId, batchSize, fromDate, out toDate, dataSource);
        }
        return testSuiteRecordList;
      }
      finally
      {
        requestContext.TraceLeave(1015005, "TestManagement", "BusinessLayer", str);
      }
    }
  }
}
