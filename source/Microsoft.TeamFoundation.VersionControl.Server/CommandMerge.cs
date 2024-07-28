// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandMerge
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandMerge : VersionControlCommand
  {
    private bool m_silent;
    private Workspace m_workspace;
    private StreamingCollection<GetOperation> m_operations;
    private StreamingCollection<Conflict> m_conflicts;
    private StreamingCollection<Failure> m_failures;
    private UrlSigner m_signer;
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private ObjectBinder<GetOperation> m_operationsBinder;
    private ObjectBinder<Conflict> m_conflictsBinder;
    private ObjectBinder<Failure> m_failuresBinder;
    private List<Failure> m_frontSideFailures;
    private CommandMerge.State m_state;
    private PropertyMerger<GetOperation> m_attributeMerger;
    private PropertyMerger<GetOperation> m_propertyMerger;
    private bool m_hasMoreProperties;
    private bool m_hasMoreAttributes;

    public CommandMerge(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      Workspace workspace,
      ItemSpec source,
      ItemSpec target,
      VersionSpec from,
      VersionSpec to,
      MergeOptionsEx options,
      LockLevel lockLevel,
      bool branch,
      bool signUrls,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters)
    {
      this.SecurityWrapper.CheckWorkspacePermission(this.m_versionControlRequestContext, 2, workspace);
      this.m_workspace = workspace;
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      if (signUrls)
        this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_silent = (options & MergeOptionsEx.Silent) != 0;
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
      ItemPathPair itemPathPair = !(to is WorkspaceVersionSpec) ? source.toServerItemWithoutMappingRenames(this.m_versionControlRequestContext, workspace, false) : source.toServerItem(this.RequestContext, workspace, false);
      ItemPathPair targetPathPair = !(to is WorkspaceVersionSpec) ? target.toServerItemWithoutMappingRenames(this.m_versionControlRequestContext, workspace, true) : target.toServerItem(this.RequestContext, workspace, true);
      targetPathPair = new ItemPathPair(VersionControlPath.GetFullPath(targetPathPair.ProjectNamePath, this.m_versionControlRequestContext.MaxSupportedServerPathLength), targetPathPair.ProjectGuidPath);
      target.toLocalItem(this.RequestContext, workspace);
      if (to == null)
        to = (VersionSpec) new LatestVersionSpec();
      string teamProject = VersionControlPath.GetTeamProject(targetPathPair.ProjectNamePath);
      if (!this.m_versionControlRequestContext.VersionControlService.GetTeamProjectFolder(this.m_versionControlRequestContext).IsValidTeamProject(this.m_versionControlRequestContext, teamProject))
      {
        if (VersionControlPath.IsRootFolder(teamProject))
          throw new CannotChangeRootFolderException();
        throw new TeamProjectNotFoundException(VersionControlPath.GetTeamProjectName(teamProject));
      }
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("workspaceName", (object) workspace?.Name);
      ctData.Add(nameof (source), (object) itemPathPair.ProjectNamePath);
      ctData.Add(nameof (target), (object) targetPathPair.ProjectNamePath);
      ctData.Add("versionFrom", (object) from);
      ctData.Add("versionTo", (object) to);
      ctData.Add(nameof (branch), (object) branch);
      ctData.Add(nameof (options), (object) options);
      ctData.Add("deletionId", (object) source.DeletionId);
      ctData.Add("recursionType", (object) source.RecursionType);
      ClientTrace.Publish(this.RequestContext, "Merge", ctData);
      List<Item> items = new List<Item>();
      int recursionType = (int) source.RecursionType;
      ItemSpec itemSpec = new ItemSpec(itemPathPair, RecursionType.None);
      DeletedState deletedState = DeletedState.Any;
      to.QueryItems(this.m_versionControlRequestContext, itemSpec, workspace, this.m_db, deletedState, ItemType.Any, (IList) items, out string _, out string _, 0);
      if (items.Count == 0)
        throw new ItemNotFoundException(this.m_versionControlRequestContext.RequestContext, workspace, source, to, deletedState);
      items.Clear();
      bool flag1 = false;
      bool flag2 = false;
      bool useRecompileVersion = this.m_versionControlRequestContext.RequestContext.IsFeatureEnabled("Tfvc.UsePendMergeWithRecompile");
      using (PendChangeSecurityDbPagingManager securityDbPagingManager = new PendChangeSecurityDbPagingManager(this.m_versionControlRequestContext))
      {
        this.m_results = this.m_db.PendMerge(workspace, itemPathPair, source.DeletionId, source.RecursionType, targetPathPair, from != null ? from : (VersionSpec) new ChangesetVersionSpec(1), to, options, branch, securityDbPagingManager.TransactionId, this.m_versionControlRequestContext.MaxSupportedServerPathLength, useRecompileVersion);
        ObjectBinder<PendChangeSecurity> current = this.m_results.GetCurrent<PendChangeSecurity>();
        while (current.MoveNext())
        {
          PendChangeSecurity.UpdateLockLevel(this.m_versionControlRequestContext, workspace, current.Current, lockLevel);
          PendChangeSecurity.UpdatePermissions(this.m_versionControlRequestContext, current.Current, this.m_frontSideFailures, branch);
          flag1 |= !securityDbPagingManager.EnqueueIfNeeded(current.Current);
          if (current.Current.FailedSecurity)
            flag2 |= current.Current.PermissionFailureNotReported;
        }
        securityDbPagingManager.Completed();
      }
      if (flag2 & flag1)
        this.m_failures.Enqueue(new Failure()
        {
          Code = RepositoryFailureCodes.MergeBranchSourceReadPermissionWarning,
          Severity = SeverityType.Warning,
          Message = Resources.Format("MergeBranchSourceReadPermissionWarning", (object) itemPathPair.ProjectNamePath, (object) this.m_versionControlRequestContext.RequestContext.GetRequestingUserDisplayName())
        });
      this.m_results.NextResult();
      int num = this.m_results.GetCurrent<Failure>().Items.Count > 0 ? 1 : 0;
      this.m_frontSideFailures.AddRange((IEnumerable<Failure>) this.m_results.GetCurrent<Failure>().Items);
      if (num != 0)
      {
        this.SetComplete();
      }
      else
      {
        if (itemAttributeFilters != null && itemAttributeFilters.Length != 0)
          this.m_attributeMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemAttributeFilters, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
        if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
          this.m_propertyMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
        this.m_state = CommandMerge.State.GetOperations;
        this.ContinueExecution();
        if (!this.IsCacheFull)
          return;
        this.RequestContext.PartialResultsReady();
      }
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandMerge.State.GetOperations)
      {
        if (this.m_silent)
        {
          this.m_state = CommandMerge.State.Failures;
        }
        else
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
            this.m_state = CommandMerge.State.Failures;
          }
          this.m_state = CommandMerge.State.Properties;
        }
      }
      if (this.m_state == CommandMerge.State.Properties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_operations);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandMerge.State.Attributes;
      }
      if (this.m_state == CommandMerge.State.Attributes)
      {
        if (this.m_attributeMerger != null)
        {
          if (!this.m_hasMoreAttributes)
            this.m_attributeMerger.Execute(this.m_operations);
          this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
        }
        this.m_state = this.m_hasMoreProperties ? CommandMerge.State.Properties : (this.m_hasMoreAttributes ? CommandMerge.State.Attributes : (!this.m_operations.IsComplete ? CommandMerge.State.GetOperations : CommandMerge.State.Failures));
      }
      if (this.m_state == CommandMerge.State.Failures && !this.IsCacheFull)
      {
        this.CopyFrontSideFailures();
        if (this.m_failuresBinder == null)
        {
          this.m_results.NextResult();
          this.m_failuresBinder = this.m_results.GetCurrent<Failure>();
        }
        bool flag = true;
        while (!this.IsCacheFull && (flag = this.m_failuresBinder.MoveNext()))
          this.m_failures.Enqueue(this.m_failuresBinder.Current);
        if (!flag)
        {
          this.m_failures.IsComplete = true;
          this.m_state = CommandMerge.State.Conflicts;
        }
      }
      if (this.m_state == CommandMerge.State.Conflicts && !this.IsCacheFull)
      {
        if (this.m_silent)
        {
          this.m_state = CommandMerge.State.Flags;
        }
        else
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
            this.m_state = CommandMerge.State.Flags;
          }
        }
      }
      if (this.m_state != CommandMerge.State.Flags)
        return;
      this.m_results.NextResult();
      this.Flags = this.m_results.GetCurrent<ChangePendedFlags>().Items[0];
      if ((this.Flags & ChangePendedFlags.WorkingFolderMappingsUpdated) != ChangePendedFlags.Unknown)
        Workspace.RemoveFromCache(this.m_versionControlRequestContext, this.m_workspace.OwnerId, this.m_workspace.Name);
      this.m_state = CommandMerge.State.Complete;
    }

    private void CopyFrontSideFailures()
    {
      if (this.m_failures == null || this.m_frontSideFailures == null)
        return;
      foreach (Failure frontSideFailure in this.m_frontSideFailures)
        this.m_failures.Enqueue(frontSideFailure);
      this.m_frontSideFailures.Clear();
    }

    private void SetComplete()
    {
      if (this.m_operations != null)
        this.m_operations.IsComplete = true;
      if (this.m_failures != null)
      {
        this.CopyFrontSideFailures();
        this.m_failures.IsComplete = true;
      }
      if (this.m_conflicts == null)
        return;
      this.m_conflicts.IsComplete = true;
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
      if (this.m_results != null)
      {
        this.m_results.Dispose();
        this.m_results = (ResultCollection) null;
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
      Conflicts,
      Flags,
      Complete,
    }
  }
}
