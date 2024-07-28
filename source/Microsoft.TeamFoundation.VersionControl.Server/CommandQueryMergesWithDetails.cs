// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryMergesWithDetails
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryMergesWithDetails : VersionControlCommand
  {
    private ChangesetMergeDetails m_details;
    private VersionedItemComponent m_db;
    protected ResultCollection m_results;
    private ObjectBinder<ItemMerge> m_miBinder;
    private ObjectBinder<ItemMerge> m_uiBinder;
    protected CommandQueryMergesWithDetails.State m_state;
    protected Dictionary<int, int> m_validChangesets;
    private List<Changeset> m_allChangesets;
    protected int m_maxChangesets;

    public CommandQueryMergesWithDetails(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public virtual void Execute(
      Workspace workspace,
      ItemSpec source,
      VersionSpec versionSource,
      ItemSpec target,
      VersionSpec versionTarget,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      int maxChangesets,
      bool showAll)
    {
      int versionFrom1;
      if (versionFrom != null)
      {
        versionFrom1 = versionFrom.ToChangeset(this.m_versionControlRequestContext.RequestContext);
        if (versionFrom1 == VersionSpec.UnknownChangeset)
          throw new NotSupportedException(nameof (versionFrom));
      }
      else
        versionFrom1 = 1;
      int versionTo1;
      if (versionTo != null)
      {
        versionTo1 = versionTo.ToChangeset(this.m_versionControlRequestContext.RequestContext);
        if (versionTo1 == VersionSpec.UnknownChangeset)
          throw new NotSupportedException(nameof (versionTo));
      }
      else
        versionTo1 = this.m_versionControlRequestContext.VersionControlService.GetLatestChangeset(this.m_versionControlRequestContext);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_details = new ChangesetMergeDetails();
      this.m_details.MergedItems = new StreamingCollection<ItemMerge>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_details.UnmergedItems = new StreamingCollection<ItemMerge>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_details.Changesets = new StreamingCollection<Changeset>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_maxChangesets = maxChangesets;
      ItemPathPair sourceItemPathPair = new ItemPathPair();
      ItemPathPair targetItemPathPair = new ItemPathPair();
      List<Item> items = new List<Item>();
      RecursionType recursive = RecursionType.Full;
      string queryPath;
      string filePattern;
      if (source != null)
      {
        DeletedState deletedState = DeletedState.Any;
        versionSource.QueryItems(this.m_versionControlRequestContext, new ItemSpec(source.Item, RecursionType.None, source.DeletionId), workspace, this.m_db, deletedState, ItemType.Any, (IList) items, out queryPath, out filePattern, 8);
        sourceItemPathPair = items.Count != 0 ? items[0].ItemPathPair : throw new ItemNotFoundException(this.m_versionControlRequestContext.RequestContext, workspace, source, versionSource, deletedState);
        recursive = source.RecursionType;
        if (!this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, items[0].ItemPathPair))
          throw new ItemNotFoundException(this.RequestContext, workspace, source, versionSource, deletedState);
      }
      items.Clear();
      if (target != null)
      {
        DeletedState deletedState = DeletedState.Any;
        versionTarget.QueryItems(this.m_versionControlRequestContext, new ItemSpec(target.Item, RecursionType.None, target.DeletionId), workspace, this.m_db, deletedState, ItemType.Any, (IList) items, out queryPath, out filePattern, 8);
        targetItemPathPair = items.Count != 0 ? items[0].ItemPathPair : throw new ItemNotFoundException(this.m_versionControlRequestContext.RequestContext, workspace, target, versionSource, deletedState);
        recursive = target.RecursionType;
        if (!this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, items[0].ItemPathPair))
          throw new ItemNotFoundException(this.m_versionControlRequestContext.RequestContext, workspace, target, versionSource, deletedState);
      }
      this.m_results = this.m_db.QueryMerges(sourceItemPathPair, versionSource != null ? versionSource : (VersionSpec) new LatestVersionSpec(), source != null ? source.DeletionId : 0, targetItemPathPair, versionTarget != null ? versionTarget : (VersionSpec) new LatestVersionSpec(), target != null ? target.DeletionId : 0, recursive, versionFrom1, versionTo1, showAll);
      this.m_validChangesets = new Dictionary<int, int>();
      this.m_state = CommandQueryMergesWithDetails.State.Changesets;
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandQueryMergesWithDetails.State.Changesets)
      {
        this.m_allChangesets = this.m_results.GetCurrent<Changeset>().Items;
        this.m_state = CommandQueryMergesWithDetails.State.MergedItems;
        this.m_results.NextResult();
        this.m_miBinder = this.m_results.GetCurrent<ItemMerge>();
      }
      if (this.m_state == CommandQueryMergesWithDetails.State.MergedItems)
      {
        bool flag = true;
        while (!this.IsCacheFull && (flag = this.m_miBinder.MoveNext()))
        {
          ItemMerge current = this.m_miBinder.Current;
          if (current.HasPermission(this.m_versionControlRequestContext))
          {
            this.m_details.MergedItems.Enqueue(current);
            if (!this.m_validChangesets.ContainsKey(current.SourceVersionFrom))
              this.m_validChangesets.Add(current.SourceVersionFrom, current.SourceVersionFrom);
            if (!this.m_validChangesets.ContainsKey(current.TargetVersionFrom))
              this.m_validChangesets.Add(current.TargetVersionFrom, current.TargetVersionFrom);
          }
        }
        if (!flag)
        {
          this.m_details.MergedItems.IsComplete = true;
          this.m_state = CommandQueryMergesWithDetails.State.UnmergedItems;
          this.m_results.NextResult();
          this.m_uiBinder = this.m_results.GetCurrent<ItemMerge>();
        }
      }
      if (this.m_state != CommandQueryMergesWithDetails.State.UnmergedItems || this.IsCacheFull)
        return;
      bool flag1 = true;
      while (!this.IsCacheFull && (flag1 = this.m_uiBinder.MoveNext()))
      {
        ItemMerge current = this.m_uiBinder.Current;
        if (current.HasPermission(this.m_versionControlRequestContext))
        {
          this.m_details.UnmergedItems.Enqueue(current);
          if (!this.m_validChangesets.ContainsKey(current.SourceVersionFrom))
            this.m_validChangesets.Add(current.SourceVersionFrom, current.SourceVersionFrom);
          if (!this.m_validChangesets.ContainsKey(current.TargetVersionFrom))
            this.m_validChangesets.Add(current.TargetVersionFrom, current.TargetVersionFrom);
        }
      }
      if (flag1)
        return;
      this.m_details.UnmergedItems.IsComplete = true;
      foreach (Changeset allChangeset in this.m_allChangesets)
      {
        if (this.m_validChangesets.ContainsKey(allChangeset.ChangesetId))
        {
          allChangeset.LookupDisplayNames(this.m_versionControlRequestContext);
          this.m_details.Changesets.Enqueue(allChangeset);
        }
      }
      this.m_details.Changesets.IsComplete = true;
      this.m_state = CommandQueryMergesWithDetails.State.Complete;
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Cancel();
        this.m_db.Dispose();
        this.m_db = (VersionedItemComponent) null;
      }
      if (this.m_results == null)
        return;
      this.m_results.Dispose();
      this.m_results = (ResultCollection) null;
    }

    public virtual ChangesetMergeDetails Details => this.m_details;

    protected enum State
    {
      MergedItems,
      UnmergedItems,
      Changesets,
      Complete,
    }
  }
}
