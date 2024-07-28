// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendChangesNotification
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class PendChangesNotification
  {
    private long m_contextId;
    private string m_ownerName;
    private string m_userName;
    private string m_workspaceName;
    private ExpandedChange[] m_expandedChanges;

    internal PendChangesNotification(
      long contextId,
      Microsoft.VisualStudio.Services.Identity.Identity user,
      string workspaceName,
      string ownerName,
      List<ExpandedChange> expandedChanges)
    {
      this.m_contextId = contextId;
      this.User = user;
      this.m_userName = user == null ? (string) null : IdentityHelper.GetDomainUserName(user);
      this.m_workspaceName = workspaceName;
      this.m_ownerName = ownerName;
      this.m_expandedChanges = expandedChanges.ToArray();
    }

    public long ContextId => this.m_contextId;

    public string OwnerName => this.m_ownerName;

    public Microsoft.VisualStudio.Services.Identity.Identity User { get; private set; }

    [Obsolete("Use the User property instead")]
    [Browsable(false)]
    public string UserName => this.m_userName;

    public string WorkspaceName => this.m_workspaceName;

    public ExpandedChange[] PendingChanges => this.m_expandedChanges;
  }
}
