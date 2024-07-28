// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryTfvcFileStats
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryTfvcFileStats : VersionControlCommand
  {
    public CommandQueryTfvcFileStats(VersionControlRequestContext context)
      : base(context)
    {
    }

    public TfvcFileStats Execute(ItemSpec scopeItem)
    {
      try
      {
        this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, scopeItem.ItemPathPair);
        this.SecurityWrapper.CheckItemPermissionForAllChildren(this.m_versionControlRequestContext, VersionedItemPermissions.Read, scopeItem.ItemPathPair);
      }
      catch (Exception ex) when (ex is ResourceAccessException)
      {
        throw new ItemNotFoundException(scopeItem.Item);
      }
      TfvcFileStats tfvcFileStats;
      using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
        tfvcFileStats = versionedItemComponent.QueryTfvcFileStats(scopeItem);
      return tfvcFileStats.ChangesetId != 0 ? tfvcFileStats : throw new ItemNotFoundException(scopeItem.Item);
    }

    protected override void Dispose(bool disposing)
    {
    }
  }
}
