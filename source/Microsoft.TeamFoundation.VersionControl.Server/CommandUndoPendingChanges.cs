// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandUndoPendingChanges
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
  internal class CommandUndoPendingChanges : VersionControlCommand
  {
    private Workspace m_workspace;
    private StreamingCollection<GetOperation> m_undoneChanges;
    private List<Failure> m_failures;
    private HashSet<string> m_teamProjectsNotFoundMessages;
    private UrlSigner m_signer;
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private ObjectBinder<GetOperation> m_binder;
    private bool m_pagedRequest;
    private PropertyMerger<GetOperation> m_attributeMerger;
    private PropertyMerger<GetOperation> m_propertyMerger;
    private CommandUndoPendingChanges.State m_state;
    private bool m_hasMoreProperties;
    private bool m_hasMoreAttributes;

    public CommandUndoPendingChanges(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      Workspace workspace,
      ItemSpec[] items,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      bool localWorkspaceBehavior = true)
    {
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_workspace = workspace;
      this.m_failures = new List<Failure>();
      this.m_teamProjectsNotFoundMessages = new HashSet<string>();
      this.m_undoneChanges = new StreamingCollection<GetOperation>((Command) this)
      {
        HandleExceptions = false
      };
      if (items.Length > 200)
        this.RequestContext.Trace(700283, TraceLevel.Info, TraceArea.Undo, TraceLayer.Command, "Perf: Large number of items to undo:" + items.Length.ToString());
      List<ItemPathPair> itemPathPairList = new List<ItemPathPair>();
      List<ItemPathPair> list = new ItemsToUndo(this.m_versionControlRequestContext, items, this.m_db, this.m_workspace, this.m_failures, this.m_teamProjectsNotFoundMessages, localWorkspaceBehavior).ToList();
      this.SetComplete();
      if (!localWorkspaceBehavior || !workspace.IsLocal)
      {
        this.m_results = this.m_db.UndoPendingChange(workspace, (IEnumerable<ItemPathPair>) list, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
        this.m_failures.AddRange((IEnumerable<Failure>) this.m_results.GetCurrent<Failure>().Items);
        this.m_results.NextResult();
        this.m_binder = this.m_results.GetCurrent<GetOperation>();
        if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
          this.m_propertyMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
        if (itemAttributeFilters != null && itemAttributeFilters.Length != 0)
          this.m_attributeMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemAttributeFilters, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
        this.m_state = CommandUndoPendingChanges.State.GetOps;
        this.ContinueExecution();
        if (!this.IsCacheFull)
          return;
        this.RequestContext.PartialResultsReady();
      }
      else
      {
        List<LockRequest> lockRequestList = new List<LockRequest>();
        foreach (ItemPathPair itemPathPair in list)
          lockRequestList.Add(new LockRequest()
          {
            TargetServerItem = itemPathPair.ProjectNamePath,
            LockLevel = LockLevel.None,
            RequiredLockLevel = LockLevel.None
          });
        try
        {
          this.m_results = this.m_db.LockItems(this.m_workspace, lockRequestList.ToArray(), false, false);
          this.m_binder = this.m_results.GetCurrent<GetOperation>();
          while (this.m_binder.MoveNext())
          {
            GetOperation current = this.m_binder.Current;
            this.m_signer.SignObject((ISignable) current);
            this.m_undoneChanges.Enqueue(current);
          }
          this.m_results.Dispose();
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (TeamProjectNotFoundException ex)
        {
          this.RequestContext.TraceException(700278, TraceLevel.Info, TraceArea.Undo, TraceLayer.Command, (Exception) ex);
          if (!this.m_teamProjectsNotFoundMessages.Contains(ex.Message))
          {
            this.m_failures.Add(new Failure((Exception) ex));
            this.m_teamProjectsNotFoundMessages.Add(ex.Message);
          }
        }
        catch (ApplicationException ex)
        {
          this.RequestContext.TraceException(700063, TraceLevel.Info, TraceArea.Undo, TraceLayer.Command, (Exception) ex);
          this.m_failures.Add(new Failure((Exception) ex));
        }
        this.m_signer.FlushDeferredSignatures();
        this.m_undoneChanges.IsComplete = true;
      }
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandUndoPendingChanges.State.GetOps)
      {
        bool flag = true;
        List<GetOperation> undoneItems = new List<GetOperation>();
        while (!this.IsCacheFull && (flag = this.m_binder.MoveNext()))
        {
          GetOperation current = this.m_binder.Current;
          this.m_signer.SignObject((ISignable) current);
          this.m_undoneChanges.Enqueue(current);
          undoneItems.Add(current);
        }
        if (flag)
          this.m_pagedRequest = true;
        this.m_signer.FlushDeferredSignatures();
        if (!flag)
        {
          this.m_undoneChanges.IsComplete = true;
          this.m_state = CommandUndoPendingChanges.State.Flags;
        }
        this.RequestContext.GetService<ITeamFoundationEventService>().PublishNotification(this.RequestContext, (object) new UndoPendingChangesNotification(this.RequestContext.ContextId, this.RequestContext.GetUserIdentity(), this.m_workspace.Name, this.m_workspace.Owner, this.m_workspace.Computer, undoneItems)
        {
          HasAllItems = !this.m_pagedRequest
        });
        this.m_state = CommandUndoPendingChanges.State.Properties;
      }
      if (this.m_state == CommandUndoPendingChanges.State.Properties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_undoneChanges);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandUndoPendingChanges.State.Attributes;
      }
      if (this.m_state == CommandUndoPendingChanges.State.Attributes)
      {
        if (this.m_attributeMerger != null)
        {
          if (!this.m_hasMoreAttributes)
            this.m_attributeMerger.Execute(this.m_undoneChanges);
          this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
        }
        this.m_state = this.m_hasMoreProperties ? CommandUndoPendingChanges.State.Properties : (this.m_hasMoreAttributes ? CommandUndoPendingChanges.State.Attributes : (!this.m_undoneChanges.IsComplete ? CommandUndoPendingChanges.State.GetOps : CommandUndoPendingChanges.State.Flags));
      }
      if (this.m_state != CommandUndoPendingChanges.State.Flags)
        return;
      this.m_results.NextResult();
      this.Flags = this.m_results.GetCurrent<ChangePendedFlags>().Items[0];
      if ((this.Flags & ChangePendedFlags.WorkingFolderMappingsUpdated) != ChangePendedFlags.Unknown)
        Workspace.RemoveFromCache(this.m_versionControlRequestContext, this.m_workspace.OwnerId, this.m_workspace.Name);
      this.m_state = CommandUndoPendingChanges.State.Complete;
    }

    private void SetComplete()
    {
      if (this.m_undoneChanges == null)
        return;
      this.m_undoneChanges.IsComplete = true;
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

    public StreamingCollection<GetOperation> UndoneChanges => this.m_undoneChanges;

    public List<Failure> Failures => this.m_failures;

    public ChangePendedFlags Flags { get; set; }

    private enum State
    {
      GetOps,
      Attributes,
      Properties,
      Flags,
      Complete,
    }
  }
}
