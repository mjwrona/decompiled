// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandUnshelve
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandUnshelve : VersionControlCommand
  {
    private VersionedItemComponent m_db;
    private ResultCollection m_rc;
    private UrlSigner m_urlSigner;
    private StreamingCollection<GetOperation> m_operations;
    private StreamingCollection<Conflict> m_conflicts;
    private List<Failure> m_failures;
    private Shelveset m_shelveset;
    private bool m_merge;
    private PropertyMerger<GetOperation> m_attributeMerger;
    private PropertyMerger<GetOperation> m_propertyMerger;
    private CommandUnshelve.State m_state;
    private ObjectBinder<Conflict> m_conflictsBinder;
    private Workspace m_workspace;
    private StreamingCollection<ArtifactPropertyValue> m_shelvesetPropertiesResults;
    private ArtifactPropertyValue m_shelvesetPropertiesArtifact;
    private TeamFoundationDataReader m_shelvesetPropertiesDataReader;
    private bool m_hasMoreProperties;
    private bool m_hasMoreAttributes;

    public CommandUnshelve(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute(
      string shelvesetName,
      string shelvesetOwnerName,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      string[] itemAttributeFilters,
      string[] itemPropertyFilters,
      string[] shelvesetPropertyNameFilters,
      bool merge)
    {
      this.Execute(shelvesetName, shelvesetOwnerName, 0, workspaceName, workspaceOwner, items, itemAttributeFilters, itemPropertyFilters, shelvesetPropertyNameFilters, merge);
    }

    internal void Execute(
      string shelvesetName,
      string shelvesetOwnerName,
      int shelvesetVersion,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      string[] itemAttributeFilters,
      string[] itemPropertyFilters,
      string[] shelvesetPropertyNameFilters,
      bool merge)
    {
      this.m_workspace = Workspace.QueryWorkspace(this.m_versionControlRequestContext, workspaceName, workspaceOwner);
      this.SecurityWrapper.CheckWorkspacePermission(this.m_versionControlRequestContext, 2, this.m_workspace);
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      try
      {
        identity = IdentityHelper.FindIdentity(this.RequestContext, shelvesetOwnerName, true, true);
      }
      catch (Microsoft.VisualStudio.Services.Identity.IdentityNotFoundException ex)
      {
        this.m_versionControlRequestContext.RequestContext.TraceException(700070, TraceLevel.Info, TraceArea.Unshelve, TraceLayer.Command, (Exception) ex);
        throw new ShelvesetNotFoundException(shelvesetName, shelvesetOwnerName);
      }
      ClientTrace.Publish(this.RequestContext, "Unshelve");
      if (shelvesetVersion < 0)
        throw new ShelvesetNotFoundException(shelvesetName, shelvesetOwnerName);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_urlSigner = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_operations = new StreamingCollection<GetOperation>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_failures = new List<Failure>();
      this.m_conflicts = new StreamingCollection<Conflict>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_merge = merge;
      List<PendChangeSecurity> pendChangeSecurityList = new List<PendChangeSecurity>();
      int num1 = 0;
      int num2 = 0;
      bool flag1 = false;
      bool flag2 = false;
      List<string> serverItems;
      using (PendChangeSecurityDbPagingManager securityDbPagingManager = new PendChangeSecurityDbPagingManager(this.m_versionControlRequestContext))
      {
        using (ItemsToUnshelveEnumerator itemsToUnshelve = new ItemsToUnshelveEnumerator(this.m_versionControlRequestContext, items, this.m_workspace, shelvesetName, identity.Id, this.m_failures))
        {
          serverItems = itemsToUnshelve.ServerItems;
          try
          {
            this.m_rc = this.m_db.PendUnshelve(shelvesetName, identity.Id, shelvesetVersion, this.m_workspace.Name, this.m_workspace.OwnerId, merge, (IEnumerable<ItemPathPair>) itemsToUnshelve, securityDbPagingManager.TransactionId, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
          }
          catch (AbortStreamingParameterException ex)
          {
            this.m_versionControlRequestContext.RequestContext.TraceException(700071, TraceLevel.Warning, TraceArea.Unshelve, TraceLayer.Command, (Exception) ex);
            this.m_operations.IsComplete = true;
            this.m_conflicts.IsComplete = true;
            return;
          }
        }
        this.m_failures.AddRange((IEnumerable<Failure>) this.m_rc.GetCurrent<Failure>().Items);
        if (this.m_failures.Count > 0)
        {
          this.m_operations.IsComplete = true;
          this.m_conflicts.IsComplete = true;
          return;
        }
        this.m_rc.NextResult();
        ObjectBinder<PendChangeSecurity> current = this.m_rc.GetCurrent<PendChangeSecurity>();
        while (current.MoveNext())
        {
          ++num1;
          PendChangeSecurity.UpdateLockLevel(this.m_versionControlRequestContext, this.m_workspace, current.Current, LockLevel.Unchanged);
          PendChangeSecurity.UpdatePermissions(this.m_versionControlRequestContext, current.Current, this.m_failures, false);
          flag2 |= !securityDbPagingManager.EnqueueIfNeeded(current.Current);
          if (current.Current.FailedSecurity)
          {
            ++num2;
            flag1 |= current.Current.PermissionFailureNotReported;
          }
        }
        securityDbPagingManager.Completed();
      }
      if (num2 == num1 && this.m_failures.Count == 0 && num1 > 0)
      {
        if (items == null)
          serverItems.Add("$/");
        for (int index = 0; index < serverItems.Count; ++index)
          this.m_failures.Add(new Failure((Exception) new ShelvedChangeNotFoundException(serverItems[index])));
        this.m_operations.IsComplete = true;
        this.m_conflicts.IsComplete = true;
      }
      else
      {
        if (flag1 & flag2)
          this.m_failures.Add(new Failure()
          {
            Code = RepositoryFailureCodes.UnshelveReadPermissionWarning,
            Severity = SeverityType.Warning,
            Message = Resources.Format("UnshelveReadPermissionWarning", (object) shelvesetName, (object) this.m_versionControlRequestContext.RequestContext.GetRequestingUserDisplayName())
          });
        this.m_rc.NextResult();
        ObjectBinder<Shelveset> current = this.m_rc.GetCurrent<Shelveset>();
        current.MoveNext();
        this.m_shelveset = current.Current;
        if (this.m_shelveset == null)
          throw new ShelvesetNotFoundException(shelvesetName, shelvesetOwnerName);
        string identityName;
        string displayName;
        this.SecurityWrapper.FindIdentityNames(this.RequestContext, this.m_shelveset.ownerId, out identityName, out displayName);
        this.m_shelveset.Owner = identityName;
        this.m_shelveset.OwnerDisplayName = displayName;
        this.m_rc.NextResult();
        if (this.m_rc.GetCurrent<CheckinNoteFieldValue>().Items.Count > 0)
          this.m_shelveset.CheckinNote.Values = this.m_rc.GetCurrent<CheckinNoteFieldValue>().Items.ToArray();
        this.m_rc.NextResult();
        this.m_shelveset.Links = this.m_rc.GetCurrent<VersionControlLink>().Items.ToArray();
        this.m_rc.NextResult();
        this.m_failures.AddRange((IEnumerable<Failure>) this.m_rc.GetCurrent<Failure>().Items);
        this.m_rc.NextResult();
        this.m_failures.AddRange((IEnumerable<Failure>) this.m_rc.GetCurrent<Failure>().Items);
        this.m_rc.NextResult();
        Warning.createFailuresFromWarnings(this.m_versionControlRequestContext, (IList) this.m_rc.GetCurrent<Warning>().Items, (IList) this.m_failures);
        this.m_rc.NextResult();
        AffectedItems affectedItems = this.m_rc.GetCurrent<AffectedItems>().Items[0];
        this.m_shelveset.ChangesExcluded = affectedItems.total - affectedItems.affected != 0;
        this.m_rc.NextResult();
        if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
          this.m_propertyMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
        if (itemAttributeFilters != null && itemAttributeFilters.Length != 0)
          this.m_attributeMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemAttributeFilters, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
        if (shelvesetPropertyNameFilters != null && shelvesetPropertyNameFilters.Length != 0)
        {
          this.m_shelvesetPropertiesDataReader = this.m_versionControlRequestContext.RequestContext.GetService<TeamFoundationPropertyService>().GetProperties(this.m_versionControlRequestContext.RequestContext, this.m_shelveset.GetArtifactSpec(VersionControlPropertyKinds.Shelveset), (IEnumerable<string>) shelvesetPropertyNameFilters);
          this.m_shelvesetPropertiesResults = this.m_shelvesetPropertiesDataReader.Current<StreamingCollection<ArtifactPropertyValue>>();
          this.m_shelveset.Properties = new StreamingCollection<PropertyValue>((Command) this)
          {
            HandleExceptions = false
          };
        }
        this.m_state = CommandUnshelve.State.ShelvesetProperties;
        this.ContinueExecution();
        if (!this.IsCacheFull)
          return;
        this.RequestContext.PartialResultsReady();
      }
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandUnshelve.State.ShelvesetProperties)
      {
        bool flag = false;
        if (this.m_shelvesetPropertiesResults != null)
        {
          if (this.m_shelvesetPropertiesArtifact == null && this.m_shelvesetPropertiesResults.MoveNext())
            this.m_shelvesetPropertiesArtifact = this.m_shelvesetPropertiesResults.Current;
          if (this.m_shelvesetPropertiesArtifact != null)
          {
            while (!this.IsCacheFull && (flag = this.m_shelvesetPropertiesArtifact.PropertyValues.MoveNext()))
              this.m_shelveset.Properties.Enqueue(this.m_shelvesetPropertiesArtifact.PropertyValues.Current);
          }
          this.m_shelveset.Properties.IsComplete = !flag;
        }
        if (!flag)
          this.m_state = CommandUnshelve.State.GetOps;
      }
      if (this.m_state == CommandUnshelve.State.GetOps)
      {
        bool flag = true;
        ObjectBinder<GetOperation> current1 = this.m_rc.GetCurrent<GetOperation>();
        while (!this.IsCacheFull && (flag = current1.MoveNext()))
        {
          GetOperation current2 = current1.Current;
          this.m_urlSigner.SignObject((ISignable) current2);
          this.m_operations.Enqueue(current2);
        }
        this.m_urlSigner.FlushDeferredSignatures();
        if (!flag)
        {
          this.m_operations.IsComplete = true;
          this.m_state = CommandUnshelve.State.Conflicts;
        }
        this.m_state = CommandUnshelve.State.ItemProperties;
      }
      if (this.m_state == CommandUnshelve.State.ItemProperties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_operations);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandUnshelve.State.ItemAttributes;
      }
      if (this.m_state == CommandUnshelve.State.ItemAttributes)
      {
        if (this.m_attributeMerger != null)
        {
          if (!this.m_hasMoreAttributes)
            this.m_attributeMerger.Execute(this.m_operations);
          this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
        }
        this.m_state = this.m_hasMoreProperties ? CommandUnshelve.State.ItemProperties : (this.m_hasMoreAttributes ? CommandUnshelve.State.ItemAttributes : (!this.m_operations.IsComplete ? CommandUnshelve.State.GetOps : CommandUnshelve.State.Conflicts));
      }
      if (this.m_state == CommandUnshelve.State.Conflicts && !this.IsCacheFull)
      {
        if (this.m_merge)
        {
          if (this.m_conflictsBinder == null)
          {
            this.m_rc.NextResult();
            this.m_conflictsBinder = this.m_rc.GetCurrent<Conflict>();
          }
          bool flag = true;
          while (!this.IsCacheFull && (flag = this.m_conflictsBinder.MoveNext()))
          {
            Conflict current = this.m_conflictsBinder.Current;
            current.TheirShelvesetOwnerName = this.m_shelveset.OwnerDisplayName;
            this.m_urlSigner.SignObject((ISignable) current);
            this.m_conflicts.Enqueue(current);
          }
          this.m_urlSigner.FlushDeferredSignatures();
          if (!flag)
          {
            this.m_conflicts.IsComplete = !flag;
            this.m_state = CommandUnshelve.State.Flags;
          }
        }
        else
        {
          this.m_conflicts.IsComplete = true;
          this.m_state = CommandUnshelve.State.Flags;
        }
      }
      if (this.m_state != CommandUnshelve.State.Flags)
        return;
      this.m_rc.NextResult();
      this.Flags = this.m_rc.GetCurrent<ChangePendedFlags>().Items[0];
      if ((this.Flags & ChangePendedFlags.WorkingFolderMappingsUpdated) != ChangePendedFlags.Unknown)
        Workspace.RemoveFromCache(this.m_versionControlRequestContext, this.m_workspace.OwnerId, this.m_workspace.Name);
      this.m_state = CommandUnshelve.State.Complete;
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
      if (this.m_urlSigner != null)
      {
        this.m_urlSigner.Dispose();
        this.m_urlSigner = (UrlSigner) null;
      }
      if (this.m_rc != null)
      {
        this.m_rc.Dispose();
        this.m_rc = (ResultCollection) null;
      }
      if (this.m_propertyMerger != null)
      {
        this.m_propertyMerger.Dispose();
        this.m_propertyMerger = (PropertyMerger<GetOperation>) null;
      }
      if (this.m_attributeMerger != null)
      {
        this.m_attributeMerger.Dispose();
        this.m_attributeMerger = (PropertyMerger<GetOperation>) null;
      }
      if (this.m_shelvesetPropertiesDataReader == null)
        return;
      this.m_shelvesetPropertiesDataReader.Dispose();
      this.m_shelvesetPropertiesDataReader = (TeamFoundationDataReader) null;
    }

    public StreamingCollection<GetOperation> GetOperations => this.m_operations;

    public StreamingCollection<Conflict> Conflicts => this.m_conflicts;

    public List<Failure> Failures => this.m_failures;

    public Shelveset Shelveset => this.m_shelveset;

    public ChangePendedFlags Flags { get; set; }

    private enum State
    {
      GetOps,
      ItemAttributes,
      ItemProperties,
      Conflicts,
      Flags,
      ShelvesetProperties,
      Complete,
    }
  }
}
