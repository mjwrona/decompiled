// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryChangesForChangeset
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryChangesForChangeset : VersionControlCommand
  {
    private StreamingCollection<Change> m_changes;
    private int m_pageSize;
    private VersionedItemComponent m_db;
    private DateTime m_changesetCreationDate;
    private ObjectBinder<Change> m_changesBinder;
    private ResultCollection m_results;
    private UrlSigner m_signer;
    private int m_changesEnqueued;
    private CommandQueryChangesForChangeset.State m_state;
    private PropertyMerger<Change> m_attributeMerger;
    private PropertyMerger<Change> m_propertyMerger;
    private bool m_includeMergeSourceInfo;
    private bool m_hasMoreProperties;
    private bool m_hasMoreAttributes;

    public CommandQueryChangesForChangeset(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(int changeset, bool generateDownloadUrls, int pageSize, ItemSpec lastItem) => this.Execute(changeset, generateDownloadUrls, pageSize, lastItem, (string[]) null, (string[]) null, false);

    public void Execute(
      int changeset,
      bool generateDownloadUrls,
      int pageSize,
      ItemSpec lastItem,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      bool includeMergeSourceInfo)
    {
      if (lastItem != null)
      {
        this.m_versionControlRequestContext.Validation.check((IValidatable) lastItem, nameof (lastItem), false);
        lastItem.requireServerItem();
      }
      if (changeset <= 0 || changeset > this.m_versionControlRequestContext.VersionControlService.GetLatestChangeset(this.m_versionControlRequestContext))
        throw new ChangesetNotFoundException(changeset);
      if (pageSize < 0)
        throw new ArgumentOutOfRangeException(nameof (pageSize));
      if (pageSize == 0)
        pageSize = int.MaxValue;
      this.m_pageSize = pageSize;
      this.m_changes = new StreamingCollection<Change>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_changesEnqueued = 0;
      this.m_state = CommandQueryChangesForChangeset.State.Changes;
      this.m_includeMergeSourceInfo = includeMergeSourceInfo;
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      if (generateDownloadUrls)
        this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_results = this.m_db.QueryChangesForChangeset(changeset, lastItem);
      ObjectBinder<DateTime> current = this.m_results.GetCurrent<DateTime>();
      current.MoveNext();
      this.m_changesetCreationDate = current.Current;
      this.m_results.NextResult();
      this.m_changesBinder = this.m_results.GetCurrent<Change>();
      if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
        this.m_propertyMerger = new PropertyMerger<Change>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
      if (itemAttributeFilters != null && itemAttributeFilters.Length != 0)
        this.m_attributeMerger = new PropertyMerger<Change>(this.m_versionControlRequestContext, itemAttributeFilters, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      bool flag1 = false;
      if (this.m_state == CommandQueryChangesForChangeset.State.Changes)
      {
        bool flag2 = true;
        List<Change> changes = new List<Change>();
        while (!this.IsCacheFull && this.m_changesEnqueued < this.m_pageSize && (flag2 = this.m_changesBinder.MoveNext()))
        {
          Change current = this.m_changesBinder.Current;
          if (current.Item.HasPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read))
          {
            current.Item.CheckinDate = this.m_changesetCreationDate;
            ++this.m_changesEnqueued;
            this.m_changes.Enqueue(current);
            if (this.m_signer != null)
              this.m_signer.SignObject((ISignable) current.Item);
            if (this.m_includeMergeSourceInfo && (current.ChangeType & (ChangeType.Rename | ChangeType.Branch | ChangeType.Merge)) != ChangeType.None)
            {
              current.MergeSources = new List<MergeSource>();
              changes.Add(current);
            }
          }
        }
        if (this.m_signer != null)
          this.m_signer.FlushDeferredSignatures();
        if (changes.Count > 0)
        {
          using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
          {
            using (ResultCollection resultCollection = versionedItemComponent.QueryMergesForChangeset(changes[0].Item.ChangesetId, changes))
            {
              ObjectBinder<MergeSource> current = resultCollection.GetCurrent<MergeSource>();
              while (current.MoveNext())
              {
                if (current.Current.SequenceId < changes.Count && this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.Current.ItemPathPair))
                  changes[current.Current.SequenceId].MergeSources.Add(current.Current);
              }
            }
          }
        }
        if (!(flag1 | flag2) || this.m_changesEnqueued == this.m_pageSize)
          this.m_changes.IsComplete = true;
        this.m_state = CommandQueryChangesForChangeset.State.ChangeProperties;
      }
      if (this.m_state == CommandQueryChangesForChangeset.State.ChangeProperties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_changes);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandQueryChangesForChangeset.State.ChangeAttributes;
      }
      if (this.m_state != CommandQueryChangesForChangeset.State.ChangeAttributes)
        return;
      if (this.m_attributeMerger != null)
      {
        if (!this.m_hasMoreAttributes)
          this.m_attributeMerger.Execute(this.m_changes);
        this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
      }
      this.m_state = this.m_hasMoreProperties ? CommandQueryChangesForChangeset.State.ChangeProperties : (this.m_hasMoreAttributes ? CommandQueryChangesForChangeset.State.ChangeAttributes : (!this.m_changes.IsComplete ? CommandQueryChangesForChangeset.State.Changes : CommandQueryChangesForChangeset.State.Complete));
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
      if (this.m_results != null)
      {
        this.m_results.Dispose();
        this.m_results = (ResultCollection) null;
      }
      if (this.m_propertyMerger != null)
      {
        this.m_propertyMerger.Dispose();
        this.m_propertyMerger = (PropertyMerger<Change>) null;
      }
      if (this.m_attributeMerger == null)
        return;
      this.m_attributeMerger.Dispose();
      this.m_attributeMerger = (PropertyMerger<Change>) null;
    }

    public StreamingCollection<Change> Changes => this.m_changes;

    private enum State
    {
      Changes,
      ChangeAttributes,
      ChangeProperties,
      Complete,
    }
  }
}
