// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryPendingSets
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryPendingSets : VersionControlCommand
  {
    private PendingSetType m_pendingSetType;
    private string m_queryWorkspaceName;
    private StreamingCollection<PendingSet> m_pendingSets;
    private StreamingCollection<Failure> m_failures;
    private int m_pageSize;
    private UrlSigner m_signer;
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private ObjectBinder<PendingChange> m_pcBinder;
    private ObjectBinder<Failure> m_failureBinder;
    private PendingSet m_currentPendingSet;
    private List<Failure> m_frontSideFailures;
    private CommandQueryPendingSets.State m_state;
    private int m_currentWorkspaceId = -1;
    private int m_pendingChangesEnqueued;
    private bool m_queryIsNotScoped;
    private int m_maxRowsEvaluated;
    private Guid m_callingTeamFoundationId;
    private ItemSpec[] m_itemSpecs;
    private string m_previousItem;
    private int m_totalChanges;
    private bool m_includeMergeSourceInfo;
    private bool m_deniedPermissionToAnItem;
    private PropertyMerger<PendingChange> m_merger;
    private List<StreamingCollection<PendingChange>> m_pendingChangeCollections;
    private bool m_hasMorePendingChangeData = true;
    private bool m_includePendingChangeSignature;

    public CommandQueryPendingSets(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      Workspace localWorkspace,
      string ownerName,
      ItemSpec[] itemSpecs,
      string queryWorkspaceName,
      PendingSetType pendingSetType,
      bool generateDownloadUrls)
    {
      this.Execute(localWorkspace, ownerName, itemSpecs, queryWorkspaceName, pendingSetType, generateDownloadUrls, int.MaxValue, (string) null, false, false, (string[]) null, false);
    }

    public void Execute(
      Workspace localWorkspace,
      string ownerName,
      ItemSpec[] itemSpecs,
      string queryWorkspaceName,
      PendingSetType pendingSetType,
      bool generateDownloadUrls,
      int pageSize,
      string lastChange,
      bool includeMergeSourceInfo,
      bool maskLocalWorkspaces,
      string[] itemPropertyFilters,
      bool includePendingChangeSignature)
    {
      this.Execute(localWorkspace, ownerName, itemSpecs, queryWorkspaceName, 0, pendingSetType, generateDownloadUrls, pageSize, lastChange, includeMergeSourceInfo, maskLocalWorkspaces, itemPropertyFilters, includePendingChangeSignature);
    }

    internal void Execute(
      Workspace localWorkspace,
      string ownerName,
      ItemSpec[] itemSpecs,
      string queryWorkspaceName,
      int queryWorkspaceVersion,
      PendingSetType pendingSetType,
      bool generateDownloadUrls,
      int pageSize,
      string lastChange,
      bool includeMergeSourceInfo,
      bool maskLocalWorkspaces,
      string[] itemPropertyFilters,
      bool includePendingChangeSignature)
    {
      ArgumentUtility.CheckForOutOfRange(queryWorkspaceVersion, nameof (queryWorkspaceVersion), -1);
      if (pageSize < 0)
        throw new ArgumentOutOfRangeException(nameof (pageSize));
      if (pageSize == 0)
        pageSize = int.MaxValue;
      this.m_pageSize = pageSize;
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      if (generateDownloadUrls)
        this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_pendingSetType = pendingSetType;
      this.m_includePendingChangeSignature = includePendingChangeSignature;
      this.m_queryWorkspaceName = queryWorkspaceName;
      this.m_queryIsNotScoped = this.m_pageSize == int.MaxValue && string.IsNullOrEmpty(queryWorkspaceName) && string.IsNullOrEmpty(ownerName);
      this.m_maxRowsEvaluated = this.m_versionControlRequestContext.VersionControlService.GetMaxRowsEvaluated(this.m_versionControlRequestContext);
      this.m_includeMergeSourceInfo = includeMergeSourceInfo;
      this.m_frontSideFailures = new List<Failure>();
      this.m_pendingSets = new StreamingCollection<PendingSet>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_failures = new StreamingCollection<Failure>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_pendingChangeCollections = new List<StreamingCollection<PendingChange>>();
      Guid filterWorkspaceOwner = Guid.Empty;
      if (!string.IsNullOrEmpty(ownerName))
        filterWorkspaceOwner = TfvcIdentityHelper.FindIdentity(this.RequestContext, ownerName).Id;
      if (!this.RequestContext.IsSystemContext)
        this.m_callingTeamFoundationId = this.SecurityWrapper.FindIdentity(this.RequestContext).Id;
      if (!string.IsNullOrEmpty(queryWorkspaceName) && string.IsNullOrEmpty(ownerName))
        throw new OwnerRequiredException();
      if (localWorkspace == null)
      {
        List<ItemSpec> itemSpecList = new List<ItemSpec>();
        foreach (ItemSpec itemSpec in itemSpecs)
        {
          if (itemSpec.isServerItem)
            itemSpecList.Add(itemSpec);
          else
            this.m_frontSideFailures.Add(new Failure((Exception) new ServerItemRequiredException(itemSpec.Item))
            {
              ServerItem = itemSpec.Item,
              RequestType = RequestType.None
            });
        }
        itemSpecs = itemSpecList.ToArray();
      }
      if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
        this.m_merger = new PropertyMerger<PendingChange>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
      this.m_itemSpecs = itemSpecs;
      try
      {
        this.m_results = this.m_db.QueryPendingChanges(localWorkspace, queryWorkspaceName, filterWorkspaceOwner, queryWorkspaceVersion, pendingSetType, itemSpecs, lastChange, maskLocalWorkspaces, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
      }
      catch (RepositoryPathTooLongDetailedException ex)
      {
        throw new LongPathInWorkspaceRequires2012QU1Exception(Resources.Format("RepositoryPathTooLongDueToOtherWorkspace", (object) queryWorkspaceName));
      }
      this.m_state = CommandQueryPendingSets.State.PendingChanges;
      this.m_previousItem = (string) null;
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandQueryPendingSets.State.PendingChanges)
      {
        if (this.m_pcBinder == null)
          this.m_pcBinder = this.m_results.GetCurrent<PendingChange>();
        List<PendingChange> changes = new List<PendingChange>();
        while (!this.IsCacheFull && this.m_pendingChangesEnqueued < this.m_pageSize && (this.m_hasMorePendingChangeData = this.m_pcBinder.MoveNext()))
        {
          if (this.m_queryIsNotScoped && this.m_pendingChangesEnqueued > this.m_maxRowsEvaluated)
          {
            this.m_failures.Add((object) new Failure()
            {
              Message = Resources.Format("MaxRowsEvaluatedException", (object) this.m_maxRowsEvaluated),
              Severity = SeverityType.Error
            });
            this.m_hasMorePendingChangeData = false;
            break;
          }
          PendingChange current1 = this.m_pcBinder.Current;
          if (current1.InputIndex >= this.m_itemSpecs.Length)
          {
            this.m_versionControlRequestContext.RequestContext.Trace(700000, TraceLevel.Error, TraceArea.General, TraceLayer.Command, "InputIndex is {0}, m_itemSpecs size is {1}", (object) current1.InputIndex, (object) this.m_itemSpecs.Length);
            throw new ItemNotFoundException(current1.ServerItem);
          }
          if (this.m_itemSpecs[current1.InputIndex].postMatch(current1.ServerItem) && (current1.pendingSet.workspaceId != this.m_currentWorkspaceId || !VersionControlPath.Equals(this.m_previousItem, current1.ServerItem)))
          {
            if ((PathLength) current1.ServerItem.Length > this.m_versionControlRequestContext.MaxSupportedServerPathLength + 1)
              throw new LongPathInWorkspaceRequires2012QU1Exception(Resources.Format("RepositoryPathTooLongDueToOtherWorkspace", (object) this.m_queryWorkspaceName));
            ++this.m_totalChanges;
            if (!current1.HasPermission(this.m_versionControlRequestContext, this.m_callingTeamFoundationId))
            {
              this.m_deniedPermissionToAnItem = true;
            }
            else
            {
              if (current1.pendingSet.workspaceId != this.m_currentWorkspaceId)
              {
                if (this.m_currentPendingSet != null)
                  this.m_currentPendingSet.PendingChanges.IsComplete = true;
                this.m_currentPendingSet = current1.pendingSet;
                this.m_currentPendingSet.Type = this.m_pendingSetType;
                if (this.m_includePendingChangeSignature && this.m_pendingSetType == PendingSetType.Workspace)
                {
                  using (WorkspaceComponent workspaceComponent = this.m_versionControlRequestContext.VersionControlService.GetWorkspaceComponent(this.m_versionControlRequestContext))
                  {
                    using (ResultCollection resultCollection = workspaceComponent.QueryWorkspaces(this.m_currentPendingSet.OwnerTeamFoundationId, this.m_currentPendingSet.Name, (string) null))
                    {
                      ObjectBinder<WorkspaceInternal> current2 = resultCollection.GetCurrent<WorkspaceInternal>();
                      if (current2.MoveNext())
                      {
                        if (current2.Current.IsLocal)
                          this.m_currentPendingSet.PendingChangeSignature = current2.Current.PendingChangeSignature;
                      }
                    }
                  }
                }
                this.m_currentPendingSet.PendingChanges = new StreamingCollection<PendingChange>((Command) this)
                {
                  HandleExceptions = false
                };
                this.m_pendingChangeCollections.Add(this.m_currentPendingSet.PendingChanges);
                this.m_pendingSets.Enqueue(this.m_currentPendingSet);
                this.m_currentWorkspaceId = current1.pendingSet.workspaceId;
              }
              if (this.m_signer != null)
                this.m_signer.SignObject((ISignable) current1);
              if (this.m_includeMergeSourceInfo && (current1.ChangeType & (ChangeType.Branch | ChangeType.Merge)) != ChangeType.None)
              {
                current1.MergeSources = new List<MergeSource>();
                changes.Add(current1);
              }
              ++this.m_pendingChangesEnqueued;
              this.m_currentPendingSet.PendingChanges.Enqueue(current1);
              this.m_previousItem = current1.ServerItem;
            }
          }
        }
        if (changes.Count > 0)
        {
          using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
          {
            using (ResultCollection resultCollection = versionedItemComponent.QueryMergesForPendingChanges(changes))
            {
              ObjectBinder<MergeSource> current = resultCollection.GetCurrent<MergeSource>();
              while (current.MoveNext())
              {
                int index = current.Current.SequenceId / 2;
                if (index < changes.Count && this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.Current.ItemPathPair))
                  changes[index].MergeSources.Add(current.Current);
              }
            }
          }
        }
        if (this.m_signer != null)
          this.m_signer.FlushDeferredSignatures();
        if (this.m_merger != null && this.m_currentPendingSet != null)
        {
          this.m_merger.Execute(this.m_currentPendingSet.PendingChanges);
          this.m_state = CommandQueryPendingSets.State.Properties;
        }
        else if (!this.m_hasMorePendingChangeData || this.m_pendingChangesEnqueued == this.m_pageSize)
        {
          this.m_pendingSets.IsComplete = true;
          this.m_state = CommandQueryPendingSets.State.Failures;
        }
      }
      if (this.m_state == CommandQueryPendingSets.State.Properties && !this.m_merger.TryMergeNextPage())
      {
        if (this.m_hasMorePendingChangeData && this.m_pendingChangesEnqueued != this.m_pageSize)
        {
          this.m_state = CommandQueryPendingSets.State.PendingChanges;
        }
        else
        {
          this.m_pendingSets.IsComplete = true;
          this.m_state = CommandQueryPendingSets.State.Failures;
        }
      }
      if (this.m_state != CommandQueryPendingSets.State.Failures || this.IsCacheFull)
        return;
      if (this.m_failureBinder == null)
      {
        this.CopyFrontSideFailures();
        if (this.m_pendingChangesEnqueued > 0 && this.m_deniedPermissionToAnItem && this.m_pendingSetType == PendingSetType.Shelveset)
          this.m_failures.Enqueue(new Failure()
          {
            Code = RepositoryFailureCodes.QueryShelvedItemsReadPermissionWarning,
            Severity = SeverityType.Warning,
            Message = Resources.Format("QueryShelvedItemsReadPermissionWarning", (object) this.m_queryWorkspaceName, (object) this.m_versionControlRequestContext.RequestContext.GetRequestingUserDisplayName())
          });
        this.m_results.NextResult();
        this.m_failureBinder = this.m_results.GetCurrent<Failure>();
      }
      bool flag = true;
      while (!this.IsCacheFull && (flag = this.m_failureBinder.MoveNext()))
        this.m_failures.Enqueue(this.m_failureBinder.Current);
      if (flag)
        return;
      this.m_failures.IsComplete = true;
      this.m_state = CommandQueryPendingSets.State.Complete;
    }

    private void CopyFrontSideFailures()
    {
      if (this.m_failures == null || this.m_frontSideFailures == null)
        return;
      foreach (Failure frontSideFailure in this.m_frontSideFailures)
        this.m_failures.Enqueue(frontSideFailure);
      this.m_frontSideFailures.Clear();
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

    public StreamingCollection<PendingSet> PendingSets => this.m_pendingSets;

    public StreamingCollection<Failure> Failures => this.m_failures;

    internal bool CanAccessAllChanges => this.m_totalChanges > 0 && this.m_pendingChangesEnqueued == this.m_totalChanges;

    private enum State
    {
      PendingChanges,
      Properties,
      Failures,
      Complete,
    }
  }
}
