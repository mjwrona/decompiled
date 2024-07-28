// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryChangeset
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryChangeset : VersionControlCommand
  {
    private bool m_includeChanges;
    private Changeset m_changeset;
    private VersionedItemComponent m_db;
    private ObjectBinder<Change> m_changesBinder;
    private ResultCollection m_results;
    private UrlSigner m_signer;
    private int m_hasAccessCount;
    private int m_hasReadAccessCount;
    private bool m_allChangesIncluded = true;
    private int m_totalChanges;
    private int m_maxChangesToRetrieve;
    private bool m_hasPropertyRequest;
    private CommandGetChangesetProperty m_commandGetChangesetProperty;
    private VersionedItemPermissions m_requiredPermission;
    private CommandQueryChangeset.State m_state = CommandQueryChangeset.State.Changes;
    private PropertyMerger<Change> m_attributeMerger;
    private PropertyMerger<Change> m_propertyMerger;
    private bool m_hasMoreProperties;
    private bool m_hasMoreAttributes;

    public CommandQueryChangeset(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(int changeset, bool includeChanges, bool generateDownloadUrls) => this.Execute(changeset, includeChanges, generateDownloadUrls, int.MaxValue);

    public void Execute(
      int changeset,
      bool includeChanges,
      bool generateDownloadUrls,
      int maxChangesToRetrieve)
    {
      this.Execute(changeset, includeChanges, generateDownloadUrls, maxChangesToRetrieve, (string[]) null, VersionedItemPermissions.Read);
    }

    public void Execute(
      int changeset,
      bool includeChanges,
      bool generateDownloadUrls,
      int maxChangesToRetrieve,
      string[] changesetPropertyFilters,
      VersionedItemPermissions requiredPermisssion)
    {
      this.Execute(changeset, includeChanges, generateDownloadUrls, maxChangesToRetrieve, changesetPropertyFilters, (string[]) null, (string[]) null, requiredPermisssion, true);
    }

    public void Execute(
      int changeset,
      bool includeChanges,
      bool generateDownloadUrls,
      int maxChangesToRetrieve,
      string[] changesetPropertyFilters,
      string[] itemAttributeFilters,
      string[] itemPropertyFilters,
      VersionedItemPermissions requiredPermisssion,
      bool includeSourceRenames)
    {
      if (changeset <= 0 || changeset > this.m_versionControlRequestContext.VersionControlService.GetLatestChangeset(this.m_versionControlRequestContext))
        throw new ChangesetNotFoundException(changeset);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      if (generateDownloadUrls)
        this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_includeChanges = includeChanges;
      this.m_maxChangesToRetrieve = maxChangesToRetrieve;
      this.m_hasAccessCount = 0;
      this.m_hasPropertyRequest = changesetPropertyFilters != null && changesetPropertyFilters.Length != 0;
      this.m_requiredPermission = requiredPermisssion;
      this.m_state = CommandQueryChangeset.State.Changes;
      this.m_results = this.m_db.QueryChangeset(changeset, true, includeSourceRenames);
      ObjectBinder<Changeset> current = this.m_results.GetCurrent<Changeset>();
      current.MoveNext();
      this.m_changeset = current.Current;
      if (this.m_changeset == null)
        throw new ChangesetNotFoundException(changeset);
      this.m_changeset.LookupDisplayNames(this.m_versionControlRequestContext);
      this.m_changeset.Changes = new StreamingCollection<Change>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_results.NextResult();
      if (this.m_results.GetCurrent<PolicyFailureInfo>().Items.Count > 0)
        this.m_changeset.PolicyOverride.PolicyFailures = this.m_results.GetCurrent<PolicyFailureInfo>().Items.ToArray();
      this.m_results.NextResult();
      if (this.m_results.GetCurrent<CheckinNoteFieldValue>().Items.Count > 0)
        this.m_changeset.CheckinNote.Values = this.m_results.GetCurrent<CheckinNoteFieldValue>().Items.ToArray();
      if (this.m_hasPropertyRequest)
      {
        this.m_commandGetChangesetProperty = new CommandGetChangesetProperty(this.m_versionControlRequestContext);
        this.m_commandGetChangesetProperty.Execute(changeset, changesetPropertyFilters, false);
        this.m_changeset.Properties = new StreamingCollection<PropertyValue>((Command) this)
        {
          HandleExceptions = false
        };
        this.m_state = CommandQueryChangeset.State.ChangesetProperties;
      }
      if (this.m_includeChanges && itemAttributeFilters != null && itemAttributeFilters.Length != 0)
        this.m_attributeMerger = new PropertyMerger<Change>(this.m_versionControlRequestContext, itemAttributeFilters, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
      if (this.m_includeChanges && itemPropertyFilters != null && itemPropertyFilters.Length != 0)
        this.m_propertyMerger = new PropertyMerger<Change>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      bool flag1 = false;
      if (this.m_state == CommandQueryChangeset.State.ChangesetProperties && this.m_hasPropertyRequest)
      {
        bool flag2 = false;
        ArtifactPropertyValue first = this.m_commandGetChangesetProperty.First;
        if (first != null)
        {
          while (!this.IsCacheFull && (flag2 = first.PropertyValues.MoveNext()))
            this.m_changeset.Properties.Enqueue(first.PropertyValues.Current);
        }
        if (!flag2)
        {
          this.m_changeset.Properties.IsComplete = true;
          this.m_commandGetChangesetProperty.Dispose();
          this.m_commandGetChangesetProperty = (CommandGetChangesetProperty) null;
          this.m_state = CommandQueryChangeset.State.Changes;
        }
        flag1 = flag2;
      }
      if (CommandQueryChangeset.State.Changes == this.m_state)
      {
        if (this.m_changesBinder == null)
        {
          this.m_results.NextResult();
          this.m_changesBinder = this.m_results.GetCurrent<Change>();
        }
        bool flag3 = true;
        while (!this.IsCacheFull && (flag3 = this.m_changesBinder.MoveNext()) && this.m_hasAccessCount < this.m_maxChangesToRetrieve)
        {
          Change current = this.m_changesBinder.Current;
          ++this.m_totalChanges;
          if (!current.Item.HasPermission(this.m_versionControlRequestContext, this.m_requiredPermission, this.m_requiredPermission == VersionedItemPermissions.Read))
          {
            if (this.m_requiredPermission == VersionedItemPermissions.Checkin && current.Item.HasPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, true))
              ++this.m_hasReadAccessCount;
            this.m_allChangesIncluded = false;
          }
          else
          {
            ++this.m_hasAccessCount;
            if (this.m_includeChanges)
            {
              current.Item.CheckinDate = this.m_changeset.CreationDate;
              this.m_changeset.Changes.Enqueue(current);
              if (this.m_signer != null)
                this.m_signer.SignObject((ISignable) current.Item);
            }
            else if (this.m_hasAccessCount > 0 && this.m_requiredPermission == VersionedItemPermissions.Read)
            {
              flag3 = false;
              break;
            }
          }
        }
        if (this.m_signer != null)
          this.m_signer.FlushDeferredSignatures();
        bool flag4 = flag1 | flag3;
        if (!flag4 || this.m_hasAccessCount == this.m_maxChangesToRetrieve)
        {
          if (this.m_totalChanges > 0 && this.m_hasAccessCount == 0 && this.m_requiredPermission == VersionedItemPermissions.Read)
            throw new ResourceAccessException(this.RequestContext.GetUserId().ToString(), "Read", Resources.Format("AtLeastOneItemInChangeset", (object) this.m_changeset.ChangesetId));
          if (this.m_includeChanges)
            this.m_changeset.AllChangesIncluded = !flag4 && this.m_allChangesIncluded;
          this.m_changeset.Changes.IsComplete = true;
        }
        this.m_state = CommandQueryChangeset.State.ChangeProperties;
      }
      if (this.m_state == CommandQueryChangeset.State.ChangeProperties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_changeset.Changes);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandQueryChangeset.State.ChangeAttributes;
      }
      if (this.m_state == CommandQueryChangeset.State.ChangeAttributes && this.m_attributeMerger != null)
      {
        if (!this.m_hasMoreAttributes)
          this.m_attributeMerger.Execute(this.m_changeset.Changes);
        this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
      }
      this.m_state = this.m_hasMoreProperties ? CommandQueryChangeset.State.ChangeProperties : (this.m_hasMoreAttributes ? CommandQueryChangeset.State.ChangeAttributes : (!this.m_changeset.Changes.IsComplete ? CommandQueryChangeset.State.Changes : CommandQueryChangeset.State.Complete));
    }

    public Changeset Changeset => this.m_changeset;

    internal bool CanReadAtLeastOneChange => (this.m_requiredPermission == VersionedItemPermissions.Checkin ? this.m_hasReadAccessCount : this.m_hasAccessCount) > 0;

    internal bool CanAccessAllChanges => this.m_totalChanges > 0 && this.m_hasAccessCount == this.m_totalChanges;

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
      if (this.m_results != null)
      {
        this.m_results.Dispose();
        this.m_results = (ResultCollection) null;
      }
      if (this.m_commandGetChangesetProperty != null)
      {
        this.m_commandGetChangesetProperty.Dispose();
        this.m_commandGetChangesetProperty = (CommandGetChangesetProperty) null;
      }
      if (this.m_attributeMerger != null)
      {
        this.m_attributeMerger.Dispose();
        this.m_attributeMerger = (PropertyMerger<Change>) null;
      }
      if (this.m_propertyMerger == null)
        return;
      this.m_propertyMerger.Dispose();
      this.m_propertyMerger = (PropertyMerger<Change>) null;
    }

    private enum State
    {
      ChangesetProperties,
      Changes,
      ChangeProperties,
      ChangeAttributes,
      Complete,
    }
  }
}
