// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryLocalVersions
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryLocalVersions : VersionControlCommand
  {
    private List<StreamingCollection<LocalVersion>> m_items;
    private Workspace m_workspace;
    private ItemSpec[] m_itemSpecs;
    private int m_itemSpecIndex;
    private VersionedItemComponent m_db;
    private ResultCollection m_rc;

    public CommandQueryLocalVersions(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(string workspaceName, string workspaceOwner, ItemSpec[] itemSpecs)
    {
      foreach (ItemSpec itemSpec in itemSpecs)
        itemSpec.requireLocalItem();
      this.m_workspace = Workspace.QueryWorkspace(this.m_versionControlRequestContext, workspaceName, workspaceOwner);
      this.m_itemSpecs = itemSpecs;
      this.m_itemSpecIndex = 0;
      this.m_items = new List<StreamingCollection<LocalVersion>>(this.m_itemSpecs.Length);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.SecurityWrapper.CheckWorkspacePermission(this.m_versionControlRequestContext, 2, this.m_workspace);
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      for (; this.m_itemSpecIndex < this.m_itemSpecs.Length; ++this.m_itemSpecIndex)
      {
        if (this.m_rc == null)
        {
          this.m_items.Add(new StreamingCollection<LocalVersion>((Command) this)
          {
            HandleExceptions = false
          });
          this.m_rc = this.m_db.QueryLocalVersions(this.m_workspace, this.m_itemSpecs[this.m_itemSpecIndex], this.m_versionControlRequestContext.MaxSupportedServerPathLength);
        }
        ObjectBinder<LocalVersion> current = this.m_rc.GetCurrent<LocalVersion>();
        while (!this.IsCacheFull && current.MoveNext())
        {
          if (this.m_itemSpecs[this.m_itemSpecIndex].postMatch(current.Current.Item))
            this.m_items[this.m_itemSpecIndex].Enqueue(current.Current);
        }
        if (this.IsCacheFull)
          break;
        this.m_items[this.m_itemSpecIndex].IsComplete = true;
        this.m_rc.Dispose();
        this.m_rc = (ResultCollection) null;
        this.m_itemSpecs[this.m_itemSpecIndex] = (ItemSpec) null;
      }
    }

    public List<StreamingCollection<LocalVersion>> Items => this.m_items;

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
