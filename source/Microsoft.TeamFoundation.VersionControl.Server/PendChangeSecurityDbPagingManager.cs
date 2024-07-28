// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendChangeSecurityDbPagingManager
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendChangeSecurityDbPagingManager : IDisposable
  {
    private VersionedItemComponent m_db;
    private List<PendChangeSecurity> m_items;

    public PendChangeSecurityDbPagingManager(
      VersionControlRequestContext versionControlRequestContext)
    {
      this.m_db = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext);
      this.TransactionId = this.m_db.AcquirePendingChangeSecurityLock();
      this.m_items = new List<PendChangeSecurity>();
    }

    public bool EnqueueIfNeeded(PendChangeSecurity item)
    {
      if (!item.FailedSecurity && item.LockLevel == LockLevel.Unchanged && !item.FailedPatternMatch && !item.FailedRestrictions)
        return false;
      item.SourceItemToPathPair = new ItemPathPair();
      item.TargetItemPathPair = new ItemPathPair();
      item.TargetSourceItemPathPair = new ItemPathPair();
      this.m_items.Add(item);
      return true;
    }

    public void Dispose() => this.Close(false);

    private void Close(bool completed)
    {
      if (this.m_db == null)
        return;
      if (this.TransactionId > 0)
        this.m_db.ReleasePendingChangeSecurityLock(this.TransactionId, this.m_items, completed);
      this.m_db.Dispose();
      this.m_db = (VersionedItemComponent) null;
    }

    public void Completed() => this.Close(true);

    public int TransactionId { get; private set; }
  }
}
