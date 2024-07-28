// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryBranchObjects
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal abstract class CommandQueryBranchObjects : VersionControlCommand
  {
    private VersionedItemComponent m_db;
    private ResultCollection m_rc;
    private ObjectBinder<BranchObject> m_branchObjectBinder;
    private StreamingCollection<BranchObject> m_branchObjects;
    private bool m_hasAdminConfiguration;

    protected CommandQueryBranchObjects(VersionControlRequestContext context)
      : base(context)
    {
    }

    protected abstract ResultCollection ExecuteInternal(VersionedItemComponent db);

    protected abstract void SetItemValidationOptionsInternal();

    protected abstract void RecordClientTraceData();

    public void Execute()
    {
      this.ItemPathPair = new ItemPathPair();
      this.m_hasAdminConfiguration = this.m_versionControlRequestContext.GetPrivilegeSecurity().HasPermission(this.m_versionControlRequestContext.RequestContext, SecurityConstants.GlobalSecurityResource, 32);
      this.m_branchObjects = new StreamingCollection<BranchObject>((Command) this)
      {
        HandleExceptions = false
      };
      if (this.Item != null)
      {
        this.ItemPathPair = this.Item.ItemPathPair;
        this.SetItemValidationOptionsInternal();
        this.m_versionControlRequestContext.Validation.check((IValidatable) this.Item, "item", false);
        if (!this.m_hasAdminConfiguration && !this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, this.Item.ItemPathPair, true))
        {
          this.m_branchObjects.IsComplete = true;
          return;
        }
      }
      this.RecordClientTraceData();
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_rc = this.ExecuteInternal(this.m_db);
      this.m_branchObjectBinder = this.m_rc.GetCurrent<BranchObject>();
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      bool flag = false;
      while (!this.IsCacheFull && (flag = this.m_branchObjectBinder.MoveNext()))
      {
        BranchObject current = this.m_branchObjectBinder.Current;
        if (this.m_hasAdminConfiguration || this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.Properties.RootItem.ItemPathPair))
        {
          string identityName;
          string displayName;
          this.m_versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(this.RequestContext, current.Properties.OwnerId, out identityName, out displayName);
          current.Properties.Owner = identityName;
          current.Properties.OwnerDisplayName = displayName;
          if (current.Properties.ParentBranch != null && !this.m_hasAdminConfiguration && !this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.Properties.ParentBranch.ItemPathPair))
            current.Properties.ParentBranch = (ItemIdentifier) null;
          for (int index = 0; index < current.ChildBranches.Count; ++index)
          {
            if (!this.m_hasAdminConfiguration && !this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.ChildBranches[index].ItemPathPair))
              current.ChildBranches.RemoveAt(index);
          }
          for (int index = 0; index < current.RelatedBranches.Count; ++index)
          {
            if (!this.m_hasAdminConfiguration && !this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.RelatedBranches[index].ItemPathPair))
              current.RelatedBranches.RemoveAt(index);
          }
          this.m_branchObjects.Enqueue(current);
        }
      }
      if (flag)
        return;
      this.m_branchObjects.IsComplete = true;
    }

    public StreamingCollection<BranchObject> BranchObjects => this.m_branchObjects;

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

    protected ItemIdentifier Item { get; set; }

    protected ItemPathPair ItemPathPair { get; private set; }
  }
}
