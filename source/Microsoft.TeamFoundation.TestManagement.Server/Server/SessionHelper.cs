// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SessionHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class SessionHelper : TfsRestApiHelper
  {
    internal SessionHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public TestSession CreateSession(string projectId, TestSession testSession, string teamField)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, nameof (projectId), this.RequestContext.ServiceName);
      this.RequestContext.TraceInfo("RestLayer", "SessionHelper.CreateSession projectId = {0}, sessionTitle = {1}, teamField = {2}", (object) projectId, (object) testSession.Title, (object) teamField);
      return this.ExecuteAction<TestSession>("TestSessionHelper.CreateTestSession", (Func<TestSession>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        testSession.Project = new ShallowReference()
        {
          Id = projectId,
          Name = projectReference.Name
        };
        return this.TestManagementSessionService.CreateTestSession(this.RequestContext, testSession, projectReference, teamField);
      }), 1015069, "TestManagement");
    }

    public TestSession UpdateSession(string projectId, TestSession testSession)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, nameof (projectId), this.RequestContext.ServiceName);
      this.RequestContext.TraceInfo("RestLayer", "SessionHelper.UpdateSession projectId = {0}, sessionTitle = {1}", (object) projectId, (object) testSession.Title);
      return this.ExecuteAction<TestSession>("TestSessionHelper.UpdateSession", (Func<TestSession>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        testSession.Project = new ShallowReference()
        {
          Id = projectId,
          Name = projectReference.Name
        };
        return this.TestManagementSessionService.UpdateTestSession(this.RequestContext, testSession, projectReference);
      }), 1015069, "TestManagement");
    }

    public IList<TestSession> GetTestSessions(
      string projectId,
      WebApiTeam team,
      int period,
      bool allSessions,
      bool includeAllProperties,
      List<int> sources,
      List<int> states)
    {
      return (IList<TestSession>) this.ExecuteAction<List<TestSession>>("TestSessionHelper.GetTestSessions", (Func<List<TestSession>>) (() => this.TestManagementSessionService.GetTestSessions(this.RequestContext, this.GetProjectReference(projectId), team, period, allSessions, includeAllProperties, sources, states)), 1015069, "TestManagement");
    }
  }
}
