// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryDestroyableItems
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryDestroyableItems : VersionControlCommand
  {
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private ObjectBinder<Item> m_itemBinder;

    public CommandQueryDestroyableItems(VersionControlRequestContext requestContext)
      : base(requestContext)
    {
    }

    public void Execute(string serverFolder, VersionSpec versionSpec, DeletedState deleted)
    {
      Validation validation = this.m_versionControlRequestContext.Validation;
      ArgumentUtility.CheckStringForNullOrEmpty(serverFolder, nameof (serverFolder));
      VersionSpec spec = versionSpec;
      validation.checkVersionSpec(spec, nameof (versionSpec), false);
      switch (versionSpec)
      {
        case LabelVersionSpec _:
        case WorkspaceVersionSpec _:
          throw new ArgumentException("versionSpec cannot be a LabelVersionSpec or a WorkspaceVersionSpec");
        default:
          this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
          this.Items = new StreamingCollection<Item>((Command) this)
          {
            HandleExceptions = false
          };
          this.m_results = this.m_db.QueryDestroyableItems(serverFolder, versionSpec.ToChangeset(this.RequestContext), deleted);
          this.m_itemBinder = this.m_results.GetCurrent<Item>();
          this.ContinueExecution();
          if (!this.IsCacheFull)
            break;
          this.RequestContext.PartialResultsReady();
          break;
      }
    }

    public override void ContinueExecution()
    {
      while (!this.IsCacheFull && this.m_itemBinder.MoveNext())
        this.Items.Enqueue(this.m_itemBinder.Current);
      if (this.IsCacheFull)
        return;
      this.Items.IsComplete = true;
    }

    public StreamingCollection<Item> Items { get; set; }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Dispose();
        this.m_db = (VersionedItemComponent) null;
      }
      if (this.m_results == null)
        return;
      this.m_results.Dispose();
      this.m_results = (ResultCollection) null;
    }
  }
}
