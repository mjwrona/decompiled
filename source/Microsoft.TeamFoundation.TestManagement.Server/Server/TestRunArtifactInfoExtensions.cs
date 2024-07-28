// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRunArtifactInfoExtensions
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TestRunArtifactInfoExtensions
  {
    internal static void UpdateSession(
      ref this TestRunArtifactInfo info,
      int sessionId,
      TfsTestManagementRequestContext context)
    {
      if (sessionId == info.SessionId)
        return;
      info.SessionId = 0;
      info.ProjectName = string.Empty;
      info.Project = GuidAndString.Empty;
      ProjectInfo projectInfo = new ProjectInfo();
      Guid projectId = Guid.Empty;
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create((TestManagementRequestContext) context))
      {
        context.TraceAndDebugAssert("BusinessLayer", sessionId != 0, "TestRunArtifactInfo.Update: Invalid TestRunId and SessionId");
        projectId = planningDatabase.GetProjectForSession(sessionId);
      }
      if (projectId != Guid.Empty)
      {
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(projectId);
        info.Project = new GuidAndString(projectFromGuid.Uri, projectFromGuid.Id);
        info.ProjectName = projectFromGuid.Name;
      }
      info.HasViewRunPermission = context.SecurityManager.HasViewTestResultsPermission((TestManagementRequestContext) context, info.Project.String);
      info.SessionId = sessionId;
    }
  }
}
