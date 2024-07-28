// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public abstract class WorkspaceException : ServerException
  {
    internal string m_workspaceName;
    internal string m_workspaceOwner;

    protected WorkspaceException(string workspaceName, string workspaceOwner)
    {
      this.m_workspaceName = workspaceName;
      this.m_workspaceOwner = workspaceOwner;
    }

    protected WorkspaceException(string resourceKey, string workspaceName, string workspaceOwner)
      : base(Resources.Format(resourceKey, (object) WorkspaceSpec.Combine(workspaceName, workspaceOwner)))
    {
      this.m_workspaceName = workspaceName;
      this.m_workspaceOwner = workspaceOwner;
    }

    public string WorkspaceName => this.m_workspaceName;

    public string WorkspaceOwner => this.m_workspaceOwner;

    public override void SetFailureInfo(Failure failure)
    {
      base.SetFailureInfo(failure);
      failure.WorkspaceName = this.m_workspaceName;
      failure.WorkspaceOwner = this.m_workspaceOwner;
    }
  }
}
