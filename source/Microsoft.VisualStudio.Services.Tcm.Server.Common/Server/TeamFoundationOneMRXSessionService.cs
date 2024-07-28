// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationOneMRXSessionService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationOneMRXSessionService : 
    TeamFoundationTestManagementService,
    ITeamFoundationOneMRXSessionService,
    IVssFrameworkService
  {
    public long CreateSession(
      TestManagementRequestContext requestContext,
      OneMRXSession session,
      TeamProjectReference teamProject)
    {
      return this.ExecuteAction<long>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.CreateTestSession", (Func<long>) (() => session.CreateSession(requestContext, teamProject.Name)), 1015961, "TestResultsInsights");
    }

    public void CreateConfigAndEnvironment(
      TestManagementRequestContext requestContext,
      IList<TestSessionEnvironment> environments,
      TeamProjectReference teamProject)
    {
      this.ExecuteAction(requestContext.RequestContext, "TeamFoundationTestManagementRunService.CreateConfigAndEnvironment", (Action) (() =>
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
          managementDatabase.CreateConfigurationAndEnvironment(teamProject.Id, environments);
      }), 1015961, "TestResultsInsights");
    }

    public List<OneMRXSession> GetSessionsByBuildId(
      TestManagementRequestContext requestContext,
      int buildId,
      TeamProjectReference teamProject)
    {
      return this.ExecuteAction<List<OneMRXSession>>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.GetTestSessionByBuildId", (Func<List<OneMRXSession>>) (() => OneMRXSession.GetSession(requestContext, teamProject.Name, buildId)), 1015961, "TestResultsInsights");
    }

    public List<Layout> GetSessionLayoutBySessionId(
      TestManagementRequestContext requestContext,
      Guid sessionId,
      TeamProjectReference teamProject)
    {
      return this.ExecuteAction<List<Layout>>(requestContext.RequestContext, "TeamFoundationTestManagementRunService.GetTestSessionByBuildId", (Func<List<Layout>>) (() => OneMRXSession.GetSessionLayout(requestContext, teamProject.Name, sessionId)), 1015961, "TestResultsInsights");
    }
  }
}
