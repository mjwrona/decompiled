// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryBranchObjectOwnership
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryBranchObjectOwnership : VersionControlCommand
  {
    private VersionedItemComponent m_db;
    private ResultCollection m_rc;
    private ObjectBinder<Microsoft.TeamFoundation.VersionControl.Server.BranchObjectOwnership> m_branchObjectOwnershipBinder;
    private StreamingCollection<Microsoft.TeamFoundation.VersionControl.Server.BranchObjectOwnership> m_branchObjectOwnership;

    public CommandQueryBranchObjectOwnership(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute(int[] changesets, ItemSpec pathFilter)
    {
      if (pathFilter != null)
      {
        pathFilter.SetValidationOptions(true, false, false);
        this.m_versionControlRequestContext.Validation.check((IValidatable) pathFilter, nameof (pathFilter), false);
        pathFilter.requireServerItem();
      }
      if (changesets.Length > this.m_versionControlRequestContext.VersionControlService.GetMaxInputsHeavyRequest(this.m_versionControlRequestContext))
        throw new ArgumentException(Resources.Format("MaxInputsExceeded", (object) this.m_versionControlRequestContext.VersionControlService.GetMaxInputsHeavyRequest(this.m_versionControlRequestContext)), nameof (changesets));
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_branchObjectOwnership = new StreamingCollection<Microsoft.TeamFoundation.VersionControl.Server.BranchObjectOwnership>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_rc = this.m_db.QueryBranchObjectOwnership(changesets, pathFilter);
      this.m_branchObjectOwnershipBinder = this.m_rc.GetCurrent<Microsoft.TeamFoundation.VersionControl.Server.BranchObjectOwnership>();
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      bool flag = false;
      while (!this.IsCacheFull && (flag = this.m_branchObjectOwnershipBinder.MoveNext()))
      {
        Microsoft.TeamFoundation.VersionControl.Server.BranchObjectOwnership current = this.m_branchObjectOwnershipBinder.Current;
        if (this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.RootItem.ItemPathPair))
          this.m_branchObjectOwnership.Enqueue(current);
      }
      if (flag)
        return;
      this.m_branchObjectOwnership.IsComplete = true;
    }

    public StreamingCollection<Microsoft.TeamFoundation.VersionControl.Server.BranchObjectOwnership> BranchObjectOwnership => this.m_branchObjectOwnership;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_rc != null)
      {
        this.m_rc.Dispose();
        this.m_rc = (ResultCollection) null;
      }
      if (this.m_db == null)
        return;
      this.m_db.Dispose();
      this.m_db = (VersionedItemComponent) null;
    }
  }
}
