// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementSuiteEntriesService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementSuiteEntriesService : 
    TfsTestManagementService,
    ITeamFoundationTestManagementSuiteEntriesService,
    IVssFrameworkService
  {
    public TeamFoundationTestManagementSuiteEntriesService()
    {
    }

    public TeamFoundationTestManagementSuiteEntriesService(
      TfsTestManagementRequestContext requestContext)
      : base((TestManagementRequestContext) requestContext)
    {
    }

    public IEnumerable<SuiteEntry> GetSuiteEntriesFromSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      int suiteId)
    {
      using (PerfManager.Measure(requestContext, "BusinessLayer", "TeamFoundationTestManagementSuiteEntriesService.GetSuiteEntriesFromSuite"))
        return this.ExecuteAction<IEnumerable<SuiteEntry>>(requestContext, "TeamFoundationTestManagementRunService.GetSuiteEntriesFromSuite", (Func<IEnumerable<SuiteEntry>>) (() =>
        {
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(requestContext)))
            return planningDatabase.GetSuiteEntries(projectId, suiteId);
        }), 1015201, "TestManagement", "BusinessLayer");
    }

    public IEnumerable<SuiteEntry> ReorderSuiteEntries(
      IVssRequestContext requestContext,
      Guid projectId,
      int suiteId,
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      using (PerfManager.Measure(requestContext, "BusinessLayer", "TeamFoundationTestManagementSuiteEntriesService.ReorderSuiteEntries"))
        return this.ExecuteAction<IEnumerable<SuiteEntry>>(requestContext, "TeamFoundationTestManagementRunService.ReorderSuiteEntries", (Func<IEnumerable<SuiteEntry>>) (() =>
        {
          using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) this.GetTfsTestManagementRequestContext(requestContext)))
            return planningDatabase.ReorderSuiteEntries(projectId, suiteId, suiteEntries);
        }), 1015201, "TestManagement", "BusinessLayer");
    }
  }
}
