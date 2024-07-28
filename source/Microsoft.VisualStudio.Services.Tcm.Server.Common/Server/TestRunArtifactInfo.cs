// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRunArtifactInfo
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal struct TestRunArtifactInfo
  {
    private int m_testRunId;
    private int m_sessionId;
    private string m_projectName;
    private bool m_hasReadPermission;
    private GuidAndString m_project;

    internal int TestRunId => this.m_testRunId;

    internal int SessionId
    {
      get => this.m_sessionId;
      set => this.m_sessionId = value;
    }

    internal GuidAndString Project
    {
      get => this.m_project;
      set => this.m_project = value;
    }

    internal string ProjectName
    {
      get => this.m_projectName;
      set => this.m_projectName = value;
    }

    internal bool HasViewRunPermission
    {
      get => this.m_hasReadPermission;
      set => this.m_hasReadPermission = value;
    }

    internal void UpdateRun(int testRunId, TestManagementRequestContext context)
    {
      if (testRunId == this.m_testRunId)
        return;
      this.m_testRunId = 0;
      this.m_projectName = string.Empty;
      this.m_project = GuidAndString.Empty;
      ProjectInfo projectInfo = new ProjectInfo();
      Guid projectId = Guid.Empty;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        if (testRunId != 0)
          projectId = managementDatabase.GetProjectForRun(testRunId);
      }
      if (projectId != Guid.Empty)
      {
        ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(projectId);
        this.m_project = new GuidAndString(projectFromGuid.Uri, projectFromGuid.Id);
        this.m_projectName = projectFromGuid.Name;
      }
      this.m_hasReadPermission = context.SecurityManager.HasViewTestResultsPermission(context, this.m_project.String);
      this.m_testRunId = testRunId;
    }
  }
}
