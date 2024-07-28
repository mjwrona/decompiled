// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryHistory
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryHistory : VersionControlCommand
  {
    private ItemSpec m_itemSpec;
    private VersionSpec m_versionItem;
    private VersionSpec m_versionFrom;
    private VersionSpec m_versionTo;
    private int m_maxCount;
    private bool m_generateDownloadUrls;
    private bool m_includeFiles;
    private bool m_checkPermissionsOnResults;
    private bool m_slotMode;
    private bool m_sortAscending;
    private Microsoft.VisualStudio.Services.Identity.Identity m_identity;
    private UrlSigner m_signer;
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private ObjectBinder<Change> m_changesBinder;
    private const int c_maximumResultCount = 256;
    private List<Changeset> m_rawChangesets;
    private List<Changeset> m_permittedChangesets;
    private List<PolicyFailureInfo> m_policyFailures;
    private List<CheckinNoteFieldValue> m_checkinNoteValues;
    private bool m_sqlExhausted;
    private int m_minimumRequestCount = 1;
    private int m_currentChangesetIndex;
    private bool m_currentChangesetIsPermitted;
    private int m_maxChangePerChangeSet = int.MaxValue;
    private int m_currentChangeSetChangesCount;
    private DeterminedItem m_determinedItem;
    private bool m_throttle;
    private int m_loopCount;
    private static readonly string s_queryHistoryRegistryPath = "/Configuration/VersionControl/QueryHistory/";
    private static readonly string s_acceptableSecondsRegistryKey = CommandQueryHistory.s_queryHistoryRegistryPath + "AcceptableSeconds";
    private static readonly int s_defaultAcceptableSeconds = 3;
    private static readonly RegistryQuery s_durationQuery = new RegistryQuery(CommandQueryHistory.s_acceptableSecondsRegistryKey);

    public CommandQueryHistory(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      string user,
      ItemSpec itemSpec,
      Workspace localWorkspace,
      VersionSpec versionItem,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      int maxCount,
      bool includeFiles,
      bool generateDownloadUrls,
      bool slotMode,
      bool sortAscending)
    {
      this.m_itemSpec = itemSpec;
      this.m_versionItem = versionItem;
      this.m_versionFrom = versionFrom;
      this.m_versionTo = versionTo;
      this.m_maxCount = maxCount;
      this.m_includeFiles = includeFiles;
      this.m_slotMode = slotMode || this.m_itemSpec.RecursionType != 0;
      this.m_identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      this.m_generateDownloadUrls = generateDownloadUrls;
      this.m_sortAscending = sortAscending;
      this.m_rawChangesets = new List<Changeset>();
      this.m_permittedChangesets = new List<Changeset>();
      if (!string.IsNullOrEmpty(user))
      {
        this.m_identity = TfvcIdentityHelper.FindIdentity(this.RequestContext, user);
        if (this.m_identity == null)
        {
          this.SetComplete();
          return;
        }
      }
      if (versionItem is WorkspaceVersionSpec)
      {
        this.m_itemSpec.ItemPathPair = this.m_itemSpec.toServerItem(this.RequestContext, localWorkspace);
      }
      else
      {
        try
        {
          this.m_itemSpec.ItemPathPair = this.m_itemSpec.toServerItemWithoutMappingRenames(this.m_versionControlRequestContext, localWorkspace, true);
        }
        catch (IllegalServerItemException ex)
        {
        }
      }
      if (!this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, itemSpec.ItemPathPair) || !this.SecurityWrapper.HasItemPermissionForAllChildren(this.m_versionControlRequestContext, VersionedItemPermissions.Read, itemSpec.ItemPathPair) || VersionControlPath.IsWildcard(itemSpec.Item))
      {
        this.m_checkPermissionsOnResults = true;
        if (!this.m_includeFiles)
          this.m_maxChangePerChangeSet = 100;
      }
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("item", (object) this.m_itemSpec?.ItemPathPair.ProjectNamePath);
      ctData.Add(nameof (versionFrom), (object) this.m_versionFrom);
      ctData.Add(nameof (versionTo), (object) this.m_versionTo);
      ctData.Add(nameof (maxCount), (object) this.m_maxCount);
      ctData.Add(nameof (includeFiles), (object) this.m_includeFiles);
      ctData.Add(nameof (sortAscending), (object) this.m_sortAscending);
      ctData.Add("checkPermissions", (object) this.m_checkPermissionsOnResults);
      ctData.Add(nameof (slotMode), (object) this.m_slotMode);
      ctData.Add("recursionType", (object) this.m_itemSpec?.RecursionType);
      ClientTrace.Publish(this.RequestContext, "QueryHistory", ctData);
      this.m_throttle = this.RequestContext.IsFeatureEnabled("Tfvc.SelfThrottle");
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      this.RequestContext.TraceEnter(700285, TraceArea.History, TraceLayer.Command, nameof (ContinueExecution));
      while (!this.IsCacheFull && (this.m_changesBinder != null || this.m_permittedChangesets.Count < this.m_maxCount && !this.m_sqlExhausted))
      {
        if (this.m_changesBinder == null)
        {
          int maxChangesets = Math.Max(this.m_maxCount - this.m_rawChangesets.Count, this.m_minimumRequestCount);
          if (!this.m_slotMode)
          {
            try
            {
              this.m_results = this.m_db.QueryItemHistory(this.m_itemSpec.ItemPathPair, this.m_versionItem, this.m_versionFrom != null ? this.m_versionFrom : (VersionSpec) new ChangesetVersionSpec(1), this.m_versionTo != null ? this.m_versionTo : (VersionSpec) new LatestVersionSpec(), this.m_identity == null ? Guid.Empty : this.m_identity.Id, this.m_includeFiles || this.m_checkPermissionsOnResults, maxChangesets, this.m_sortAscending);
            }
            catch (IdentityNotFoundException ex)
            {
              this.m_versionControlRequestContext.RequestContext.TraceException(700061, TraceLevel.Info, TraceArea.History, TraceLayer.Command, (Exception) ex);
              this.SetComplete();
              return;
            }
          }
          else
          {
            try
            {
              VersionSpec versionFrom = this.m_versionFrom ?? (VersionSpec) new ChangesetVersionSpec(1);
              VersionSpec versionTo = this.m_versionTo ?? (VersionSpec) new LatestVersionSpec();
              int acceptableSeconds = this.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.RequestContext, in CommandQueryHistory.s_durationQuery, 0);
              if (acceptableSeconds < 1)
                acceptableSeconds = CommandQueryHistory.s_defaultAcceptableSeconds;
              this.RequestContext.Trace(700329, TraceLevel.Verbose, TraceArea.History, TraceLayer.Command, "QueryHistoryScore item:{0} version:{1} recursion:{2} versionItem:{3} versionFrom:{4} versionTo:{5} m_maxCount:{6} requestedCount:{7} sortAscending:{8} includeFiles:{9} filterItems:{10} maxChangesPerChangeset:{11}", (object) this.m_itemSpec.Item, (object) this.m_versionItem, (object) this.m_itemSpec.RecursionType, (object) this.m_versionItem, (object) versionFrom, (object) versionTo, (object) this.m_maxCount, (object) maxChangesets, (object) this.m_sortAscending, (object) this.m_includeFiles, (object) this.m_checkPermissionsOnResults, (object) this.m_maxChangePerChangeSet);
              this.m_results = this.m_db.QueryHistoryScore(this.m_itemSpec.ItemPathPair, this.m_versionItem, versionFrom, versionTo, this.m_identity == null ? Guid.Empty : this.m_identity.Id, this.m_itemSpec.RecursionType, this.m_includeFiles || this.m_checkPermissionsOnResults, this.m_slotMode, maxChangesets, this.m_sortAscending, this.m_maxChangePerChangeSet, acceptableSeconds, this.m_itemSpec.isWildcard);
            }
            catch (IdentityNotFoundException ex)
            {
              this.m_versionControlRequestContext.RequestContext.TraceException(700062, TraceLevel.Info, TraceArea.History, TraceLayer.Command, (Exception) ex);
              this.SetComplete();
              return;
            }
            if (this.m_results.GetCurrent<DeterminedItem>().MoveNext())
              this.m_determinedItem = this.m_results.GetCurrent<DeterminedItem>().Current;
            this.m_results.NextResult();
          }
          this.m_minimumRequestCount = Math.Min(this.m_minimumRequestCount * 16, 256);
          this.m_currentChangesetIndex = 0;
          this.m_currentChangesetIsPermitted = false;
          this.m_rawChangesets = this.m_results.GetCurrent<Changeset>().Items;
          this.m_results.NextResult();
          this.m_policyFailures = this.m_results.GetCurrent<PolicyFailureInfo>().Items;
          this.m_results.NextResult();
          this.m_checkinNoteValues = this.m_results.GetCurrent<CheckinNoteFieldValue>().Items;
          if (this.m_includeFiles || this.m_checkPermissionsOnResults)
          {
            this.m_results.NextResult();
            this.m_changesBinder = this.m_results.GetCurrent<Change>();
          }
          this.m_sqlExhausted = this.m_rawChangesets.Count < maxChangesets && this.m_rawChangesets.Count < 256;
          if (!this.m_sqlExhausted)
          {
            if (this.m_sortAscending)
              this.m_versionFrom = (VersionSpec) new ChangesetVersionSpec(this.m_rawChangesets[this.m_rawChangesets.Count - 1].ChangesetId + 1);
            else
              this.m_versionTo = (VersionSpec) new ChangesetVersionSpec(this.m_rawChangesets[this.m_rawChangesets.Count - 1].ChangesetId - 1);
          }
          int index1 = 0;
          for (int index2 = 0; index1 < this.m_rawChangesets.Count && index2 < this.m_policyFailures.Count; ++index1)
          {
            Changeset rawChangeset = this.m_rawChangesets[index1];
            if (this.m_policyFailures[index2].changesetId == rawChangeset.ChangesetId)
            {
              int length = 0;
              while (index2 + length < this.m_policyFailures.Count && this.m_policyFailures[index2 + length].changesetId == rawChangeset.ChangesetId)
                ++length;
              rawChangeset.PolicyOverride.PolicyFailures = new PolicyFailureInfo[length];
              int index3 = 0;
              while (index3 < length)
              {
                rawChangeset.PolicyOverride.PolicyFailures[index3] = this.m_policyFailures[index2];
                ++index3;
                ++index2;
              }
            }
          }
          int index4 = 0;
          for (int index5 = 0; index4 < this.m_rawChangesets.Count && index5 < this.m_checkinNoteValues.Count; ++index4)
          {
            Changeset rawChangeset = this.m_rawChangesets[index4];
            if (this.m_checkinNoteValues[index5].checkinNoteId == rawChangeset.checkinNoteId)
            {
              int length = 0;
              while (index5 + length < this.m_checkinNoteValues.Count && this.m_checkinNoteValues[index5 + length].checkinNoteId == rawChangeset.checkinNoteId)
                ++length;
              rawChangeset.CheckinNote.Values = new CheckinNoteFieldValue[length];
              int index6 = 0;
              while (index6 < length)
              {
                rawChangeset.CheckinNote.Values[index6] = this.m_checkinNoteValues[index5];
                ++index6;
                ++index5;
              }
            }
          }
          this.m_versionControlRequestContext.RequestContext.TraceBlock(700286, 700287, TraceArea.History, TraceLayer.BusinessLogic, "ContinueExecution_ChangesetNameLookup", (Action) (() =>
          {
            foreach (Changeset rawChangeset in this.m_rawChangesets)
            {
              rawChangeset.Changes = new StreamingCollection<Change>((Command) this)
              {
                HandleExceptions = false
              };
              rawChangeset.LookupDisplayNames(this.m_versionControlRequestContext);
            }
          }));
        }
        if (this.m_includeFiles || this.m_checkPermissionsOnResults)
        {
          if (this.m_signer == null && this.m_generateDownloadUrls)
            this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
          bool hasMoreData = true;
          this.m_versionControlRequestContext.RequestContext.TraceBlock(700288, 700289, TraceArea.History, TraceLayer.BusinessLogic, "ContinueExecution_EvaluateAndAttach", (Action) (() =>
          {
            while (!this.IsCacheFull && (hasMoreData = this.m_changesBinder.MoveNext()))
            {
              Change current = this.m_changesBinder.Current;
              Changeset changeset;
              for (changeset = this.m_rawChangesets[this.m_currentChangesetIndex]; current.Item.ChangesetId != changeset.ChangesetId; changeset = this.m_rawChangesets[this.m_currentChangesetIndex])
              {
                this.CheckUserHasReadPermissionForCurrentChangeset();
                changeset.Changes.IsComplete = true;
                ++this.m_currentChangesetIndex;
                this.m_currentChangesetIsPermitted = false;
                this.m_currentChangeSetChangesCount = 0;
                if (this.m_currentChangesetIndex == this.m_rawChangesets.Count)
                {
                  changeset = (Changeset) null;
                  break;
                }
              }
              if (this.m_currentChangesetIndex == this.m_rawChangesets.Count)
              {
                hasMoreData = false;
                break;
              }
              if (this.UserHasReadPermission(current))
              {
                if (!this.m_currentChangesetIsPermitted && !this.AddChangesetToResults(changeset))
                  break;
                if (this.m_includeFiles)
                {
                  current.Item.CheckinDate = changeset.CreationDate;
                  if (this.m_signer != null)
                    this.m_signer.SignObject((ISignable) current.Item);
                  changeset.Changes.Enqueue(current);
                }
              }
              ++this.m_currentChangeSetChangesCount;
            }
          }));
          this.CheckUserHasReadPermissionForCurrentChangeset();
          if (!hasMoreData || this.m_permittedChangesets.Count >= this.m_maxCount)
          {
            for (int currentChangesetIndex = this.m_currentChangesetIndex; currentChangesetIndex < this.m_rawChangesets.Count; ++currentChangesetIndex)
              this.m_rawChangesets[currentChangesetIndex].Changes.IsComplete = true;
            this.m_results.Dispose();
            this.m_results = (ResultCollection) null;
            this.m_changesBinder = (ObjectBinder<Change>) null;
          }
          if (this.m_signer != null)
            this.m_signer.FlushDeferredSignatures();
        }
        else
        {
          foreach (Changeset rawChangeset in this.m_rawChangesets)
          {
            rawChangeset.Changes.IsComplete = true;
            this.m_permittedChangesets.Add(rawChangeset);
          }
        }
      }
      this.RequestContext.TraceLeave(700290, TraceArea.History, TraceLayer.Command, nameof (ContinueExecution));
    }

    private bool UserHasReadPermission(Change change) => (!this.m_currentChangesetIsPermitted || this.m_includeFiles) && this.m_itemSpec.postMatch(change.Item.ServerItem) && change.Item.HasPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read);

    private bool AddChangesetToResults(Changeset changeset)
    {
      if (this.m_permittedChangesets.Count >= this.m_maxCount)
        return false;
      this.m_permittedChangesets.Add(this.m_rawChangesets[this.m_currentChangesetIndex]);
      this.m_currentChangesetIsPermitted = true;
      return true;
    }

    private void CheckUserHasReadPermissionForCurrentChangeset()
    {
      if (this.m_currentChangesetIndex < this.m_rawChangesets.Count && !this.m_currentChangesetIsPermitted && this.m_currentChangeSetChangesCount >= this.m_maxChangePerChangeSet)
      {
        Changeset rawChangeset = this.m_rawChangesets[this.m_currentChangesetIndex];
        using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
        {
          using (ResultCollection resultCollection = versionedItemComponent.QueryChangesForChangeset(rawChangeset.ChangesetId, (ItemSpec) null))
          {
            resultCollection.NextResult();
            while (resultCollection.GetCurrent<Change>().MoveNext())
            {
              Change current = resultCollection.GetCurrent<Change>().Current;
              if (VersionControlPath.IsSubItem(current.Item.ServerItem, this.m_determinedItem.QueryPath) && this.UserHasReadPermission(current))
              {
                this.AddChangesetToResults(rawChangeset);
                break;
              }
            }
          }
        }
      }
      ++this.m_loopCount;
      if (!this.m_throttle || this.m_loopCount < 300 || this.m_loopCount % 100 != 0)
        return;
      Thread.Sleep(500);
    }

    private void SetComplete()
    {
      if (this.m_permittedChangesets == null)
        return;
      for (int index = 0; index < this.m_permittedChangesets.Count; ++index)
      {
        if (this.m_permittedChangesets[index].Changes != null)
          this.m_permittedChangesets[index].Changes.IsComplete = true;
      }
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

    public List<Changeset> Changesets => this.m_permittedChangesets;
  }
}
