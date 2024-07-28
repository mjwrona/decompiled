// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.UndoPendingChangesNotification
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class UndoPendingChangesNotification
  {
    private long m_contextId;
    private string m_computerName;
    private string m_userName;
    private string m_workspaceName;
    private string m_ownerName;
    private List<string> m_itemsToUndo;
    private List<GetOperation> m_undoneItems;
    private bool m_hasAllItems;

    internal UndoPendingChangesNotification(
      long contextId,
      Microsoft.VisualStudio.Services.Identity.Identity user,
      string workspaceName,
      Microsoft.VisualStudio.Services.Identity.Identity owner,
      string computerName,
      List<string> itemsToUndo)
    {
      this.m_contextId = contextId;
      this.m_computerName = computerName;
      this.Owner = owner;
      this.m_ownerName = owner == null ? (string) null : IdentityHelper.GetDomainUserName(owner);
      this.User = user;
      this.m_userName = user == null ? (string) null : IdentityHelper.GetDomainUserName(user);
      this.m_workspaceName = workspaceName;
      this.m_itemsToUndo = itemsToUndo;
    }

    internal UndoPendingChangesNotification(
      long contextId,
      Microsoft.VisualStudio.Services.Identity.Identity user,
      string workspaceName,
      Microsoft.VisualStudio.Services.Identity.Identity owner,
      string computerName,
      List<GetOperation> undoneItems)
    {
      this.m_contextId = contextId;
      this.m_computerName = computerName;
      this.Owner = owner;
      this.m_ownerName = owner == null ? (string) null : IdentityHelper.GetDomainUserName(owner);
      this.User = user;
      this.m_userName = user == null ? (string) null : IdentityHelper.GetDomainUserName(user);
      this.m_workspaceName = workspaceName;
      this.m_undoneItems = undoneItems;
    }

    public long ContextId => this.m_contextId;

    public string ComputerName => this.m_computerName;

    public Microsoft.VisualStudio.Services.Identity.Identity Owner { get; private set; }

    [Obsolete("Use the Owner property instead")]
    [Browsable(false)]
    public string OwnerName => this.m_ownerName;

    public Microsoft.VisualStudio.Services.Identity.Identity User { get; private set; }

    [Obsolete("Use the User property instead")]
    [Browsable(false)]
    public string UserName => this.m_userName;

    public string WorkspaceName => this.m_workspaceName;

    public IEnumerable<string> ItemsToUndo => (IEnumerable<string>) this.m_itemsToUndo;

    public IEnumerable<GetOperation> UndoneItems => (IEnumerable<GetOperation>) this.m_undoneItems;

    public bool HasAllItems
    {
      get => this.m_hasAllItems;
      set => this.m_hasAllItems = value;
    }
  }
}
