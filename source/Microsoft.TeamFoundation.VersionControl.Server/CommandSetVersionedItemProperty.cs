// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandSetVersionedItemProperty
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandSetVersionedItemProperty : VersionControlCommand
  {
    private CommandQueryItems<ItemSet, Item> m_cmdQueryItems;
    private ItemSpec[] m_itemSpecs;
    private VersionSpec m_versionSpec;

    public CommandSetVersionedItemProperty(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      ItemSpec itemSpec,
      VersionSpec versionSpec,
      DeletedState deletedState,
      ItemType itemType,
      PropertyValue[] propertyValues)
    {
      this.Execute((string) null, (string) null, new ItemSpec[1]
      {
        itemSpec
      }, versionSpec, deletedState, itemType, propertyValues);
    }

    public void Execute(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      VersionSpec versionSpec,
      DeletedState deletedState,
      ItemType itemType,
      PropertyValue[] propertyValues)
    {
      bool unversioned = false;
      if (versionSpec == null)
      {
        unversioned = true;
        versionSpec = (VersionSpec) new LatestVersionSpec();
      }
      this.m_cmdQueryItems = new CommandQueryItems<ItemSet, Item>(this.m_versionControlRequestContext);
      this.m_itemSpecs = itemSpecs;
      this.m_versionSpec = versionSpec;
      this.m_cmdQueryItems.Execute(Workspace.QueryWorkspace(this.m_versionControlRequestContext, workspaceName, workspaceOwner), itemSpecs, versionSpec, deletedState, itemType, false, 0, VersionedItemPermissions.Checkin, new ContinueExecutionCompletedCallback(this.OnCommandQueryItemsDataCompleted), (string[]) null, (string[]) null);
      if (!this.m_cmdQueryItems.HasItems)
        throw new ItemNotFoundException(this.m_versionControlRequestContext.RequestContext, this.m_itemSpecs[0], this.m_versionSpec);
      this.PropertyService.SetProperties(this.RequestContext, (IEnumerable<ArtifactSpec>) new List<ArtifactSpec>((IEnumerable<ArtifactSpec>) new ItemSetToVersionedItemSpecEnumerator(unversioned, (IEnumerable<ItemSet>) this.m_cmdQueryItems.ItemSets)), (IEnumerable<PropertyValue>) propertyValues);
    }

    public override void ContinueExecution()
    {
    }

    internal void OnCommandQueryItemsDataCompleted(Command commandContext)
    {
      if (!this.m_cmdQueryItems.HasItems || this.m_cmdQueryItems.CanAccessAllItems)
        return;
      if (this.m_cmdQueryItems.CanReadAtLeastOneItem)
        throw new ResourceAccessException(this.RequestContext.GetUserId().ToString(), "Checkin", Resources.Get("AllSpecifiedItems"));
      throw new ItemNotFoundException(this.m_versionControlRequestContext.RequestContext, this.m_itemSpecs[0], this.m_versionSpec);
    }

    private ITeamFoundationPropertyService PropertyService => this.m_versionControlRequestContext.VersionControlService.GetPropertyService(this.m_versionControlRequestContext);

    protected override void Dispose(bool disposing)
    {
      if (!disposing || this.m_cmdQueryItems == null)
        return;
      this.m_cmdQueryItems.Dispose();
      this.m_cmdQueryItems = (CommandQueryItems<ItemSet, Item>) null;
    }
  }
}
