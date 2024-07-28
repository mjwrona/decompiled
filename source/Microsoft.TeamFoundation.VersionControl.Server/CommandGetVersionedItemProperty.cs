// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandGetVersionedItemProperty
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandGetVersionedItemProperty : VersionControlCommand
  {
    private StreamingCollection<ArtifactPropertyValue> m_result;
    private TeamFoundationDataReader m_dataReader;
    private CommandQueryItems<ItemSet, Item> m_cmdQueryItems;

    internal CommandGetVersionedItemProperty(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    internal void Execute(
      ItemSpec itemSpec,
      VersionSpec versionSpec,
      DeletedState deletedState,
      ItemType itemType,
      string[] propertyFilterNames)
    {
      this.Execute((string) null, (string) null, itemSpec, versionSpec, deletedState, itemType, propertyFilterNames);
    }

    public void Execute(
      string workspaceName,
      string workspaceOwner,
      ItemSpec itemSpec,
      VersionSpec versionSpec,
      DeletedState deletedState,
      ItemType itemType,
      string[] propertyFilterNames)
    {
      this.Execute(workspaceName, workspaceOwner, new ItemSpec[1]
      {
        itemSpec
      }, versionSpec, deletedState, itemType, propertyFilterNames);
    }

    public void Execute(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      VersionSpec versionSpec,
      DeletedState deletedState,
      ItemType itemType,
      string[] propertyFilterNames)
    {
      this.m_cmdQueryItems = new CommandQueryItems<ItemSet, Item>(this.m_versionControlRequestContext);
      bool unversioned = false;
      if (versionSpec == null)
      {
        unversioned = true;
        versionSpec = (VersionSpec) new LatestVersionSpec();
      }
      this.m_cmdQueryItems.Execute(Workspace.QueryWorkspace(this.m_versionControlRequestContext, workspaceName, workspaceOwner), itemSpecs, versionSpec, deletedState, itemType, false, 0, VersionedItemPermissions.Read);
      this.m_dataReader = this.PropertyService.GetProperties(this.RequestContext, (IEnumerable<ArtifactSpec>) new List<ArtifactSpec>((IEnumerable<ArtifactSpec>) new ItemSetToVersionedItemSpecEnumerator(unversioned, (IEnumerable<ItemSet>) this.m_cmdQueryItems.ItemSets)), (IEnumerable<string>) propertyFilterNames);
      this.m_result = this.m_dataReader.Current<StreamingCollection<ArtifactPropertyValue>>();
    }

    public override void ContinueExecution()
    {
    }

    internal StreamingCollection<ArtifactPropertyValue> Result => this.m_result;

    private ITeamFoundationPropertyService PropertyService => this.m_versionControlRequestContext.VersionControlService.GetPropertyService(this.m_versionControlRequestContext);

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_dataReader != null)
      {
        this.m_dataReader.Dispose();
        this.m_dataReader = (TeamFoundationDataReader) null;
      }
      if (this.m_cmdQueryItems == null)
        return;
      this.m_cmdQueryItems.Dispose();
      this.m_cmdQueryItems = (CommandQueryItems<ItemSet, Item>) null;
    }
  }
}
