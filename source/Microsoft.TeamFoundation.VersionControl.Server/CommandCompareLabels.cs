// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandCompareLabels
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandCompareLabels : VersionControlCommand
  {
    private string m_startLabelName;
    private string m_startLabelScope;
    private string m_endLabelName;
    private string m_endLabelScope;
    private int m_maxCount;
    private int m_minChangeSet;
    private bool m_includeItems;
    private StreamingCollection<Changeset> m_validChangesets;
    private LabelComponent m_db;
    private ResultCollection m_results;
    private List<Changeset> m_changesets;
    private int m_changesetsIndex;
    private ObjectBinder<Change> m_changesBinder;
    private int m_maximumResultFromDatabaseCount;
    private int m_validChangesetsCount;
    private bool m_hasAdminConfiguration;

    public CommandCompareLabels(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      string startLabelName,
      string startLabelScope,
      string endLabelName,
      string endLabelScope,
      int minChangeSet,
      int maxCount,
      bool includeItems = false)
    {
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetLabelComponent(this.m_versionControlRequestContext);
      this.m_startLabelName = startLabelName;
      this.m_startLabelScope = startLabelScope;
      this.m_endLabelName = endLabelName;
      this.m_endLabelScope = endLabelScope;
      this.m_minChangeSet = minChangeSet;
      this.m_maxCount = maxCount < 1 ? int.MaxValue : maxCount;
      this.m_includeItems = includeItems;
      this.HasMoreInDatabase = true;
      this.m_validChangesets = new StreamingCollection<Changeset>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_hasAdminConfiguration = this.m_versionControlRequestContext.GetPrivilegeSecurity().HasPermission(this.RequestContext, SecurityConstants.GlobalSecurityResource, 32, false);
      this.m_maximumResultFromDatabaseCount = 715827882 > maxCount ? maxCount * 3 : int.MaxValue;
      this.m_maximumResultFromDatabaseCount = Math.Max(this.m_maximumResultFromDatabaseCount, 256);
      this.m_maximumResultFromDatabaseCount = Math.Min(this.m_maximumResultFromDatabaseCount, 10000);
      this.m_changesets = new List<Changeset>();
      this.m_changesetsIndex = 0;
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
label_14:
      while (!this.IsCacheFull && this.m_validChangesetsCount < this.m_maxCount && (this.HasMoreInDatabase || this.HasMoreCachedChanges))
      {
        bool includeFiles = !this.m_hasAdminConfiguration || this.m_includeItems;
        if (!this.HasMoreCachedChanges)
        {
          try
          {
            this.m_results = this.m_db.CompareLabels(this.m_startLabelName, this.m_startLabelScope, this.m_endLabelName, this.m_endLabelScope, includeFiles, this.m_minChangeSet, this.m_maximumResultFromDatabaseCount);
          }
          catch (IdentityNotFoundException ex)
          {
            this.m_versionControlRequestContext.RequestContext.TraceException(700051, TraceLevel.Info, TraceArea.History, TraceLayer.Command, (Exception) ex);
            return;
          }
          this.m_changesets = this.m_results.GetCurrent<Changeset>().Items;
          this.m_changesetsIndex = 0;
          this.HasMoreInDatabase = this.m_changesets.Count >= this.m_maximumResultFromDatabaseCount;
          if (this.m_changesets.Count > 0)
            this.m_minChangeSet = this.m_changesets[this.m_changesets.Count - 1].ChangesetId + 1;
        }
        if (includeFiles)
        {
          this.m_results.NextResult();
          this.m_changesBinder = this.m_results.GetCurrent<Change>();
        }
        if (includeFiles)
          this.m_changesBinder.MoveNext();
        while (true)
        {
          if (!this.IsCacheFull && this.m_changesetsIndex < this.m_changesets.Count && this.m_validChangesetsCount < this.m_maxCount)
          {
            Changeset changeset = this.m_changesets[this.m_changesetsIndex];
            if (this.ShouldIncludeChangeset(changeset))
            {
              changeset.LookupDisplayNames(this.m_versionControlRequestContext);
              this.m_validChangesets.Enqueue(changeset);
              ++this.m_validChangesetsCount;
            }
            ++this.m_changesetsIndex;
          }
          else
            goto label_14;
        }
      }
      if (this.m_validChangesetsCount < this.m_maxCount && (this.HasMoreInDatabase || this.HasMoreCachedChanges))
        return;
      this.m_validChangesets.IsComplete = true;
    }

    private bool ShouldIncludeChangeset(Changeset changeSet)
    {
      if (this.m_hasAdminConfiguration && !this.m_includeItems)
        return true;
      bool flag1 = false;
      bool flag2 = true;
      Change current = this.m_changesBinder.Current;
      while (flag2 && current.Item.ChangesetId < changeSet.ChangesetId)
      {
        flag2 = this.m_changesBinder.MoveNext();
        if (flag2)
          current = this.m_changesBinder.Current;
      }
      while (flag2 && current.Item.ChangesetId == changeSet.ChangesetId)
      {
        if (this.m_hasAdminConfiguration || current.Item.HasPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read))
        {
          flag1 = true;
          if (this.m_includeItems)
            changeSet.Changes.Enqueue(current);
        }
        flag2 = this.m_changesBinder.MoveNext();
        if (flag2)
          current = this.m_changesBinder.Current;
      }
      return flag1;
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Cancel();
        this.m_db.Dispose();
        this.m_db = (LabelComponent) null;
      }
      if (this.m_results == null)
        return;
      this.m_results.Dispose();
      this.m_results = (ResultCollection) null;
    }

    public StreamingCollection<Changeset> Changesets => this.m_validChangesets;

    private bool HasMoreCachedChanges => this.m_changesetsIndex < this.m_changesets.Count;

    private bool HasMoreInDatabase { get; set; }
  }
}
