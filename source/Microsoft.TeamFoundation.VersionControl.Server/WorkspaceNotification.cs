// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceNotification
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class WorkspaceNotification
  {
    private string m_computerName;
    private string m_userName;
    private string m_workspaceName;
    private string m_workspaceOwnerName;
    private string m_newWorkspaceName;
    private string m_newWorkspaceComputerName;
    private string m_newWorkspaceOwnerName;

    internal WorkspaceNotification(
      Microsoft.VisualStudio.Services.Identity.Identity user,
      Microsoft.VisualStudio.Services.Identity.Identity workspaceOwner,
      string workspaceName,
      string computerName)
      : this(user, workspaceOwner, workspaceName, computerName, (string) null, (string) null, (Microsoft.VisualStudio.Services.Identity.Identity) null)
    {
    }

    internal WorkspaceNotification(
      Microsoft.VisualStudio.Services.Identity.Identity user,
      Microsoft.VisualStudio.Services.Identity.Identity workspaceOwner,
      string workspaceName,
      string computerName,
      string newWorkspaceName,
      string newWorkspaceComputerName,
      Microsoft.VisualStudio.Services.Identity.Identity newWorkspaceOwner)
    {
      this.User = user;
      this.m_userName = user == null ? (string) null : IdentityHelper.GetDomainUserName(user);
      this.WorkspaceOwner = workspaceOwner;
      this.m_workspaceOwnerName = workspaceOwner == null ? (string) null : IdentityHelper.GetDomainUserName(workspaceOwner);
      this.m_workspaceName = workspaceName;
      this.m_computerName = computerName;
      this.m_newWorkspaceName = newWorkspaceName;
      this.m_newWorkspaceComputerName = newWorkspaceComputerName;
      this.NewWorkspaceOwner = newWorkspaceOwner;
      this.m_newWorkspaceOwnerName = newWorkspaceOwner == null ? (string) null : IdentityHelper.GetDomainUserName(newWorkspaceOwner);
    }

    public string ComputerName => this.m_computerName;

    public Microsoft.VisualStudio.Services.Identity.Identity User { get; private set; }

    [Obsolete("User the User property instead")]
    [Browsable(false)]
    public string UserName => this.m_userName;

    public Microsoft.VisualStudio.Services.Identity.Identity WorkspaceOwner { get; private set; }

    [Obsolete("User the WorkspaceOwner property instead")]
    [Browsable(false)]
    public string WorkspaceOwnerName => this.m_workspaceOwnerName;

    public string WorkspaceName => this.m_workspaceName;

    public string NewWorkspaceName => this.m_newWorkspaceName;

    public string NewWorkspaceComputerName => this.m_newWorkspaceComputerName;

    public Microsoft.VisualStudio.Services.Identity.Identity NewWorkspaceOwner { get; private set; }

    [Obsolete("User the NewWorkspaceOwner property instead")]
    [Browsable(false)]
    public string NewWorkspaceOwnerName => this.m_newWorkspaceOwnerName;
  }
}
