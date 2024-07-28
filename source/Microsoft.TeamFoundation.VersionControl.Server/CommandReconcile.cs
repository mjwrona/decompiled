// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandReconcile
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandReconcile : VersionControlCommand
  {
    private VersionedItemComponent m_db;
    private ResultCollection m_rc;
    private ObjectBinder<Failure> m_failureBinder;
    private ObjectBinder<PendingChange> m_pendingChangeBinder;
    private ReconcileResult m_result;
    private Workspace m_workspace;
    private LocalPendingChange[] m_pendingChanges;
    private ServerItemLocalVersionUpdate[] m_localVersionUpdates;

    public CommandReconcile(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute(
      Workspace workspace,
      Guid pendingChangeSignature,
      LocalPendingChange[] pendingChanges,
      ServerItemLocalVersionUpdate[] localVersionUpdates,
      bool clearLocalVersionTable,
      bool throwOnProjectRenamed)
    {
      this.m_workspace = workspace;
      this.m_pendingChanges = pendingChanges;
      this.m_localVersionUpdates = localVersionUpdates;
      if (!this.SecurityWrapper.HasWorkspacePermission(this.m_versionControlRequestContext, 2, this.m_workspace) && !this.SecurityWrapper.HasWorkspacePermission(this.m_versionControlRequestContext, 8, this.m_workspace))
        this.SecurityWrapper.CheckWorkspacePermission(this.m_versionControlRequestContext, 2, this.m_workspace);
      if (this.m_pendingChanges != null)
      {
        foreach (LocalPendingChange pendingChange in this.m_pendingChanges)
          pendingChange.Workspace = this.m_workspace;
      }
      this.m_versionControlRequestContext.Validation.check((IValidatable[]) this.m_pendingChanges, nameof (pendingChanges), true);
      this.m_versionControlRequestContext.Validation.check((IValidatable[]) this.m_localVersionUpdates, nameof (localVersionUpdates), true);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_rc = this.m_db.ReconcileLocalWorkspace(this.m_workspace, pendingChangeSignature, pendingChanges, this.m_localVersionUpdates, clearLocalVersionTable, throwOnProjectRenamed, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
      ObjectBinder<ReconcileResult> current = this.m_rc.GetCurrent<ReconcileResult>();
      current.MoveNext();
      this.m_result = current.Current;
      this.m_result.Failures = new StreamingCollection<Failure>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_result.NewPendingChanges = new StreamingCollection<PendingChange>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_rc.NextResult();
      this.m_failureBinder = this.m_rc.GetCurrent<Failure>();
      this.ContinueExecution();
    }

    public override void ContinueExecution()
    {
      if (this.m_failureBinder != null)
      {
        while (this.m_failureBinder.MoveNext())
        {
          this.m_result.Failures.Enqueue(this.m_failureBinder.Current);
          if (this.IsCacheFull)
            return;
        }
        this.m_result.Failures.IsComplete = true;
        this.m_failureBinder = (ObjectBinder<Failure>) null;
        this.m_rc.NextResult();
        this.m_pendingChangeBinder = this.m_rc.GetCurrent<PendingChange>();
      }
      if (this.m_pendingChangeBinder == null)
        return;
      while (this.m_pendingChangeBinder.MoveNext())
      {
        this.m_result.NewPendingChanges.Enqueue(this.m_pendingChangeBinder.Current);
        if (this.IsCacheFull)
          return;
      }
      this.m_result.NewPendingChanges.IsComplete = true;
      this.m_pendingChangeBinder = (ObjectBinder<PendingChange>) null;
    }

    public ReconcileResult Result => this.m_result;

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
  }
}
