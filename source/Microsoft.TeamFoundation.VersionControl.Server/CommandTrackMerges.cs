// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandTrackMerges
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandTrackMerges : VersionControlCommand
  {
    private VersionedItemComponent m_db;
    private StreamingCollection<ExtendedMerge> m_merges;
    private ResultCollection m_rc;
    private Dictionary<string, bool> m_inputPaths;
    private StreamingCollection<string> m_partialTargets;
    private CommandTrackMerges.State m_state;

    public CommandTrackMerges(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      int[] sourceChangesets,
      ItemIdentifier sourceItem,
      List<ItemIdentifier> targetItems,
      ItemSpec pathFilter)
    {
      if (sourceChangesets == null)
        throw new ArgumentNullException(nameof (sourceChangesets));
      if (targetItems == null)
        throw new ArgumentNullException(nameof (targetItems));
      if (sourceChangesets.Length > this.m_versionControlRequestContext.VersionControlService.GetMaxInputsHeavyRequest(this.m_versionControlRequestContext))
        throw new ArgumentException(Resources.Format("MaxInputsExceeded", (object) this.m_versionControlRequestContext.VersionControlService.GetMaxInputsHeavyRequest(this.m_versionControlRequestContext)), nameof (sourceChangesets)).Expected("tfvc");
      if (targetItems.Count > this.m_versionControlRequestContext.VersionControlService.GetMaxInputsHeavyRequest(this.m_versionControlRequestContext))
        throw new ArgumentException(Resources.Format("MaxInputsExceeded", (object) this.m_versionControlRequestContext.VersionControlService.GetMaxInputsHeavyRequest(this.m_versionControlRequestContext)), nameof (targetItems));
      sourceItem?.SetValidationOptions(BranchObject.ValidItemOptions, BranchObject.ValidVersionSpecOptions);
      this.m_versionControlRequestContext.Validation.check((IValidatable) sourceItem, nameof (sourceItem), false);
      this.m_merges = new StreamingCollection<ExtendedMerge>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_partialTargets = new StreamingCollection<string>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_inputPaths = new Dictionary<string, bool>((IEqualityComparer<string>) TFStringComparer.VersionControlPath);
      if (!this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, sourceItem.ItemPathPair))
      {
        this.m_merges.IsComplete = true;
        this.m_partialTargets.IsComplete = true;
      }
      else
      {
        this.m_inputPaths.Add(sourceItem.Item, true);
        foreach (ItemIdentifier targetItem in targetItems)
        {
          targetItem?.SetValidationOptions(BranchObject.ValidItemOptions, VersionSpecValidationOptions.Changeset | VersionSpecValidationOptions.Latest);
          this.m_versionControlRequestContext.Validation.check((IValidatable) targetItem, nameof (targetItems), false);
        }
        for (int index = 0; index < targetItems.Count; ++index)
        {
          ItemIdentifier targetItem = targetItems[index];
          if (!this.m_inputPaths.ContainsKey(targetItem.Item))
          {
            if (!this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, targetItem.ItemPathPair))
              targetItems.RemoveAt(index);
            else
              this.m_inputPaths.Add(targetItem.Item, true);
          }
        }
        if (targetItems.Count == 0)
        {
          this.m_merges.IsComplete = true;
          this.m_partialTargets.IsComplete = true;
        }
        else
        {
          if (pathFilter != null)
          {
            pathFilter.SetValidationOptions(true, false, false);
            this.m_versionControlRequestContext.Validation.check((IValidatable) pathFilter, nameof (pathFilter), false);
            pathFilter.requireServerItem();
          }
          this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
          this.m_rc = this.m_db.TrackMerges(sourceChangesets, sourceItem, targetItems, pathFilter);
          this.m_state = CommandTrackMerges.State.ExtendedMerge;
          this.ContinueExecution();
          if (!this.IsCacheFull)
            return;
          this.RequestContext.PartialResultsReady();
        }
      }
    }

    public override void ContinueExecution()
    {
      bool flag = false;
      if (this.m_state == CommandTrackMerges.State.ExtendedMerge)
      {
        ObjectBinder<ExtendedMerge> current1 = this.m_rc.GetCurrent<ExtendedMerge>();
        while (!this.IsCacheFull && (flag = current1.MoveNext()))
        {
          ExtendedMerge current2 = current1.Current;
          if (this.m_inputPaths.ContainsKey(current2.TargetItem.Item) || this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current2.TargetItem.ItemPathPair))
          {
            if (!this.m_inputPaths.ContainsKey(current2.SourceItem.Item.ServerItem) && !this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current2.SourceItem.Item.ItemPathPair))
            {
              current2.SourceChangeset = (ChangesetSummary) null;
              current2.SourceItem = (Change) null;
            }
            if (current2.SourceChangeset != null)
              current2.SourceChangeset.LookupDisplayNames(this.m_versionControlRequestContext);
            current2.TargetChangeset.LookupDisplayNames(this.m_versionControlRequestContext);
            this.m_merges.Enqueue(current2);
          }
        }
        if (flag)
          return;
        this.m_merges.IsComplete = true;
        this.m_rc.NextResult();
        this.m_state = CommandTrackMerges.State.Partial;
      }
      else
      {
        if (this.m_state != CommandTrackMerges.State.Partial)
          return;
        ObjectBinder<ItemPathPair> current3 = this.m_rc.GetCurrent<ItemPathPair>();
        while (!this.IsCacheFull && (flag = current3.MoveNext()))
        {
          ItemPathPair current4 = current3.Current;
          if (this.m_inputPaths.ContainsKey(current4.ProjectNamePath) || this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current4))
            this.m_partialTargets.Enqueue(current4.ProjectNamePath);
        }
        if (flag)
          return;
        this.m_partialTargets.IsComplete = true;
        this.m_state = CommandTrackMerges.State.Complete;
      }
    }

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

    public StreamingCollection<ExtendedMerge> Merges => this.m_merges;

    public StreamingCollection<string> PartialTargets => this.m_partialTargets;

    private enum State
    {
      ExtendedMerge,
      Partial,
      Complete,
    }
  }
}
