// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryConflicts
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryConflicts : VersionControlCommand
  {
    private StreamingCollection<Conflict> m_conflicts;
    private UrlSigner m_signer;
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private ItemSpec[] m_items;
    private Workspace m_workspace;
    private bool m_returnAll;

    public CommandQueryConflicts(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(Workspace workspace, ItemSpec[] items)
    {
      if (items == null || items.Length == 0)
      {
        items = new ItemSpec[1];
        items[0] = new ItemSpec("$/", RecursionType.Full);
      }
      this.m_workspace = workspace;
      this.m_items = items;
      this.SecurityWrapper.CheckWorkspacePermission(this.m_versionControlRequestContext, 2, this.m_workspace);
      foreach (ItemSpec itemSpec in items)
      {
        if (VersionControlPath.Compare(itemSpec.Item, "$/") == 0 && itemSpec.RecursionType == RecursionType.Full)
        {
          this.m_returnAll = true;
          break;
        }
      }
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_conflicts = new StreamingCollection<Conflict>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_versionControlRequestContext.RequestContext.TraceBlock(700305, 700306, TraceArea.Conflicts, TraceLayer.Command, "QueryConflicts", (Action) (() => this.m_results = this.m_db.QueryConflicts(workspace)));
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      int resolveCount = 0;
      bool flag1 = true;
      ObjectBinder<Conflict> current = this.m_results.GetCurrent<Conflict>();
      while (!this.IsCacheFull && (flag1 = current.MoveNext()))
      {
        Conflict conflict = current.Current;
        bool flag2 = false;
        if (this.m_returnAll)
          flag2 = true;
        if (!flag2)
        {
          if (!string.IsNullOrEmpty(conflict.SourceLocalItem))
          {
            if (Conflict.MatchesFilter(conflict.SourceLocalItem, this.m_items))
              flag2 = true;
          }
          else if (!string.IsNullOrEmpty(conflict.TargetLocalItem) && Conflict.MatchesFilter(conflict.TargetLocalItem, this.m_items))
            flag2 = true;
        }
        if (!flag2)
        {
          if (!string.IsNullOrEmpty(conflict.YourServerItemSource) && Conflict.MatchesFilter(conflict.YourServerItemSource, this.m_items))
            flag2 = true;
          else if (!string.IsNullOrEmpty(conflict.YourServerItem) && (conflict.YourServerItemSource == null || VersionControlPath.Compare(conflict.YourServerItem, conflict.YourServerItemSource) != 0) && Conflict.MatchesFilter(conflict.YourServerItem, this.m_items))
            flag2 = true;
        }
        if (!flag2 && !string.IsNullOrEmpty(conflict.TheirServerItem) && Conflict.MatchesFilter(conflict.TheirServerItem, this.m_items))
          flag2 = true;
        if (flag2)
        {
          if (conflict.Type != ConflictType.Local)
          {
            if (conflict.YourServerItemSource != null && !this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, conflict.YourItemSourcePathPair) || conflict.YourServerItem != null && !this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, conflict.YourItemPathPair))
            {
              this.m_versionControlRequestContext.RequestContext.TraceBlock(700307, 700308, TraceArea.Conflicts, TraceLayer.Command, "QueryConflicts_ResolveDelete", (Action) (() =>
              {
                ++resolveCount;
                using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
                  versionedItemComponent.Resolve(this.m_workspace, conflict.ConflictId, Resolution.DeleteConflict, new ItemPathPair(), 0, -2, new Guid?(), LockLevel.None, false, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
              }));
            }
            else
            {
              conflict.DetermineShelvesetOwnerName(this.m_versionControlRequestContext);
              this.EnqueueConflict(conflict);
            }
          }
          else
          {
            conflict.DetermineShelvesetOwnerName(this.m_versionControlRequestContext);
            this.EnqueueConflict(conflict);
          }
        }
      }
      this.m_signer.FlushDeferredSignatures();
      if (!flag1)
        this.m_conflicts.IsComplete = true;
      if (resolveCount <= 20)
        return;
      this.RequestContext.Trace(700309, TraceLevel.Info, TraceArea.Conflicts, TraceLayer.Command, "Large number of Resolves in QueryConflicts:" + resolveCount.ToString());
    }

    private void EnqueueConflict(Conflict conflict)
    {
      this.m_signer.SignObject((ISignable) conflict);
      this.m_conflicts.Enqueue(conflict);
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Dispose();
        this.m_db = (VersionedItemComponent) null;
      }
      if (this.m_signer != null)
      {
        this.m_signer.Dispose();
        this.m_signer = (UrlSigner) null;
      }
      if (this.m_results == null)
        return;
      this.m_results.Dispose();
      this.m_results = (ResultCollection) null;
    }

    internal StreamingCollection<Conflict> Conflicts => this.m_conflicts;
  }
}
