// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandRollback
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandRollback : VersionControlCommand
  {
    private RollbackOptions m_options;
    private StreamingCollection<GetOperation> m_operations;
    private StreamingCollection<Conflict> m_conflicts;
    private StreamingCollection<Failure> m_failures;
    private int m_failureCount;
    private UrlSigner m_signer;
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private List<Failure> m_frontSideFailures;
    private ObjectBinder<Failure> m_failuresBinder;
    private ObjectBinder<Failure> m_warningsBinder;
    private ObjectBinder<GetOperation> m_operationsBinder;
    private ObjectBinder<Conflict> m_conflictsBinder;
    private CommandRollback.State m_state;
    private Workspace m_workspace;
    private PropertyMerger<GetOperation> m_attributeMerger;
    private PropertyMerger<GetOperation> m_propertyMerger;
    private bool m_hasMoreProperties;
    private bool m_hasMoreAttributes;

    public CommandRollback(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      VersionSpec itemVersion,
      VersionSpec from,
      VersionSpec to,
      RollbackOptions options,
      LockLevel lockLevel,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters)
    {
      this.m_versionControlRequestContext.Validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      this.m_versionControlRequestContext.Validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), false);
      this.m_versionControlRequestContext.Validation.check((IValidatable[]) items, nameof (items), true);
      this.m_versionControlRequestContext.Validation.checkVersionSpec(itemVersion, nameof (itemVersion), true, false);
      this.m_versionControlRequestContext.Validation.checkVersionSpec(from, nameof (from), false, true);
      this.m_versionControlRequestContext.Validation.checkVersionSpec(to, nameof (to), false, false);
      if (items == null)
        items = Array.Empty<ItemSpec>();
      this.m_versionControlRequestContext.Validation.checkRequestSize((IValidatable[]) items, nameof (items), this.m_versionControlRequestContext.VersionControlService.GetMaxInputsHeavyRequest(this.m_versionControlRequestContext));
      if ((options & RollbackOptions.ToVersion) == RollbackOptions.ToVersion && items.Length == 0)
        throw new RollbackInvalidOptionException(Resources.Get("CannotRollbackToVersionWithoutItem"));
      Workspace workspace = Workspace.QueryWorkspace(this.m_versionControlRequestContext, workspaceName, workspaceOwner);
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add(nameof (workspaceName), (object) workspace?.Name);
      ctData.Add(nameof (options), (object) options);
      ctData.Add(nameof (itemVersion), (object) itemVersion);
      ctData.Add(nameof (from), (object) from);
      ctData.Add(nameof (to), (object) to);
      if (items != null)
      {
        ctData.Add("length", (object) items.Length);
        ctData.Add(nameof (items), (object) ((IEnumerable<ItemSpec>) items).Take<ItemSpec>(5).Select<ItemSpec, string>((Func<ItemSpec, string>) (x => x.Item)).ToList<string>());
      }
      ClientTrace.Publish(this.RequestContext, "Rollback", ctData);
      this.SecurityWrapper.CheckWorkspacePermission(this.m_versionControlRequestContext, 2, workspace);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_frontSideFailures = new List<Failure>();
      this.m_operations = new StreamingCollection<GetOperation>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_failures = new StreamingCollection<Failure>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_conflicts = new StreamingCollection<Conflict>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_options = options;
      this.m_failureCount = 0;
      this.m_workspace = workspace;
      bool flag = false;
      if (items.Length != 0)
      {
        if (!items[0].isServerItem)
          itemVersion = (VersionSpec) new WorkspaceVersionSpec(workspace);
        else if (itemVersion == null)
          itemVersion = (VersionSpec) new LatestVersionSpec();
        foreach (ItemSpec itemSpec in items)
        {
          itemSpec.ItemPathPair = !(itemVersion is WorkspaceVersionSpec) ? itemSpec.toServerItemWithoutMappingRenames(this.m_versionControlRequestContext, workspace, false) : itemSpec.toServerItem(this.RequestContext, workspace, false);
          if (itemSpec.isWildcard)
            flag = true;
        }
      }
      List<PendChangeSecurity> pendChangeSecurityList = new List<PendChangeSecurity>();
      using (PendChangeSecurityDbPagingManager securityDbPagingManager = new PendChangeSecurityDbPagingManager(this.m_versionControlRequestContext))
      {
        this.m_results = this.m_db.PendRollback(workspace, items, itemVersion, from, to, options, securityDbPagingManager.TransactionId, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
        ObjectBinder<PendChangeSecurity> current = this.m_results.GetCurrent<PendChangeSecurity>();
        while (current.MoveNext())
        {
          if (flag && !items[current.Current.InputIndex].postMatch(current.Current.TargetSourceServerItem))
            current.Current.FailedPatternMatch = true;
          PendChangeSecurity.UpdateLockLevel(this.m_versionControlRequestContext, workspace, current.Current, lockLevel);
          PendChangeSecurity.UpdatePermissions(this.m_versionControlRequestContext, current.Current, this.m_frontSideFailures, false);
          this.VerifyRootProjectRestrictions(current.Current);
          securityDbPagingManager.EnqueueIfNeeded(current.Current);
        }
        securityDbPagingManager.Completed();
      }
      this.m_results.NextResult();
      if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
        this.m_propertyMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
      if (itemAttributeFilters != null && itemAttributeFilters.Length != 0)
        this.m_attributeMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemAttributeFilters, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
      this.m_state = CommandRollback.State.Failures;
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    private void VerifyRootProjectRestrictions(PendChangeSecurity item)
    {
      if (VersionControlPath.Compare(item.TargetServerItem, "$/") == 0 || VersionControlPath.Compare(item.TargetSourceServerItem, "$/") == 0)
      {
        item.FailedRestrictions = true;
        this.m_frontSideFailures.Add(new Failure((Exception) new CannotChangeRootFolderException()));
      }
      else if (VersionControlPath.IsTeamProject(item.TargetSourceServerItem))
      {
        item.FailedRestrictions = true;
        this.m_frontSideFailures.Add(new Failure((Exception) new InvalidProjectPendingChangeException(item.TargetSourceServerItem)));
      }
      else
      {
        if (!VersionControlPath.IsTeamProject(item.TargetServerItem))
          return;
        item.FailedRestrictions = true;
        this.m_frontSideFailures.Add(new Failure((Exception) new InvalidProjectPendingChangeException(item.TargetServerItem)));
      }
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandRollback.State.Failures && !this.IsCacheFull)
      {
        this.CopyFrontSideFailures();
        if (this.m_failuresBinder == null)
          this.m_failuresBinder = this.m_results.GetCurrent<Failure>();
        bool flag = true;
        while (!this.IsCacheFull && (flag = this.m_failuresBinder.MoveNext()))
        {
          ++this.m_failureCount;
          this.m_failures.Enqueue(this.m_failuresBinder.Current);
        }
        if (!flag)
        {
          this.m_state = this.m_failureCount > 0 ? CommandRollback.State.Complete : CommandRollback.State.GetOperations;
          if (this.m_state == CommandRollback.State.Complete)
          {
            this.m_operations.IsComplete = true;
            this.m_failures.IsComplete = true;
            this.m_conflicts.IsComplete = true;
          }
        }
      }
      if ((this.m_options & RollbackOptions.Silent) == RollbackOptions.Silent && !this.IsCacheFull)
        this.m_state = CommandRollback.State.Warnings;
      if (this.m_state == CommandRollback.State.GetOperations)
      {
        if (this.m_operationsBinder == null)
        {
          this.m_results.NextResult();
          this.m_operationsBinder = this.m_results.GetCurrent<GetOperation>();
        }
        bool flag = true;
        while (!this.IsCacheFull && (flag = this.m_operationsBinder.MoveNext()))
        {
          GetOperation current = this.m_operationsBinder.Current;
          if (this.m_signer != null)
            this.m_signer.SignObject((ISignable) current);
          this.m_operations.Enqueue(current);
        }
        if (this.m_signer != null)
          this.m_signer.FlushDeferredSignatures();
        if (!flag)
        {
          this.m_operations.IsComplete = true;
          this.m_state = CommandRollback.State.Conflicts;
        }
        this.m_state = CommandRollback.State.Properties;
      }
      if (this.m_state == CommandRollback.State.Properties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_operations);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandRollback.State.Attributes;
      }
      if (this.m_state == CommandRollback.State.Attributes)
      {
        if (this.m_attributeMerger != null)
        {
          if (!this.m_hasMoreAttributes)
            this.m_attributeMerger.Execute(this.m_operations);
          this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
        }
        this.m_state = this.m_hasMoreProperties ? CommandRollback.State.Properties : (this.m_hasMoreAttributes ? CommandRollback.State.Attributes : (!this.m_operations.IsComplete ? CommandRollback.State.GetOperations : CommandRollback.State.Conflicts));
      }
      if (this.m_state == CommandRollback.State.Conflicts && !this.IsCacheFull)
      {
        if (this.m_conflictsBinder == null)
        {
          this.m_results.NextResult();
          this.m_conflictsBinder = this.m_results.GetCurrent<Conflict>();
        }
        bool flag = true;
        while (!this.IsCacheFull && (flag = this.m_conflictsBinder.MoveNext()))
        {
          Conflict current = this.m_conflictsBinder.Current;
          if (this.m_signer != null)
            this.m_signer.SignObject((ISignable) current);
          this.m_conflicts.Enqueue(current);
        }
        if (this.m_signer != null)
          this.m_signer.FlushDeferredSignatures();
        if (!flag)
        {
          this.m_conflicts.IsComplete = !flag;
          this.m_state = CommandRollback.State.Warnings;
        }
      }
      if (this.m_state == CommandRollback.State.Warnings)
      {
        if (this.m_warningsBinder == null)
        {
          this.m_results.NextResult();
          this.m_warningsBinder = this.m_results.GetCurrent<Failure>();
        }
        bool flag = true;
        while (!this.IsCacheFull && (flag = this.m_warningsBinder.MoveNext()))
          this.m_failures.Enqueue(this.m_warningsBinder.Current);
        if (!flag)
        {
          this.m_failures.IsComplete = true;
          this.m_state = CommandRollback.State.Flags;
        }
      }
      if (this.m_state != CommandRollback.State.Flags)
        return;
      this.m_results.NextResult();
      this.Flags = this.m_results.GetCurrent<ChangePendedFlags>().Items[0];
      if ((this.Flags & ChangePendedFlags.WorkingFolderMappingsUpdated) == ChangePendedFlags.WorkingFolderMappingsUpdated)
        Workspace.RemoveFromCache(this.m_versionControlRequestContext, this.m_workspace.OwnerId, this.m_workspace.Name);
      this.m_state = CommandRollback.State.Complete;
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
        this.m_db.Dispose();
        this.m_db = (VersionedItemComponent) null;
      }
      if (this.m_results != null)
      {
        this.m_results.Dispose();
        this.m_results = (ResultCollection) null;
      }
      if (this.m_signer != null)
      {
        this.m_signer.Dispose();
        this.m_signer = (UrlSigner) null;
      }
      if (this.m_propertyMerger != null)
      {
        this.m_propertyMerger.Dispose();
        this.m_propertyMerger = (PropertyMerger<GetOperation>) null;
      }
      if (this.m_attributeMerger == null)
        return;
      this.m_attributeMerger.Dispose();
      this.m_attributeMerger = (PropertyMerger<GetOperation>) null;
    }

    internal StreamingCollection<GetOperation> Operations => this.m_operations;

    internal StreamingCollection<Failure> Failures => this.m_failures;

    internal StreamingCollection<Conflict> Conflicts => this.m_conflicts;

    public ChangePendedFlags Flags { get; set; }

    private enum State
    {
      GetOperations,
      Attributes,
      Properties,
      Failures,
      Warnings,
      Conflicts,
      Flags,
      Complete,
    }
  }
}
