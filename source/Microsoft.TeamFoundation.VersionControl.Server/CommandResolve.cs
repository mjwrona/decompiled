// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandResolve
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandResolve : VersionControlCommand
  {
    private int m_conflictId;
    private Workspace m_workspace;
    private StreamingCollection<GetOperation> m_operations;
    private StreamingCollection<GetOperation> m_undoOperations;
    private StreamingCollection<Conflict> m_resolvedConflicts;
    private UrlSigner m_signer;
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private ObjectBinder<GetOperation> m_operationsBinder;
    private ObjectBinder<GetOperation> m_undoOperationsBinder;
    private CommandResolve.State m_state;
    private PropertyMerger<GetOperation> m_propertyMerger;
    private PropertyMerger<GetOperation> m_attributeMerger;
    private bool m_hasMoreProperties;
    private bool m_hasMoreAttributes;

    public CommandResolve(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      Workspace workspace,
      int conflictId,
      Resolution resolution,
      string newPath,
      int encoding,
      LockLevel lockLevel,
      PropertyValue[] newProperties,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      bool disallowPropertyChangesOnAutoMerge)
    {
      this.m_versionControlRequestContext.VersionControlService.SecurityWrapper.CheckWorkspacePermission(this.m_versionControlRequestContext, 2, workspace);
      this.m_workspace = workspace;
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("workspaceName", (object) workspace?.Name);
      ctData.Add(nameof (conflictId), (object) conflictId);
      ctData.Add(nameof (resolution), (object) resolution);
      ctData.Add(nameof (newPath), (object) newPath);
      ClientTrace.Publish(this.RequestContext, "Resolve", ctData);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_conflictId = conflictId;
      this.m_operations = new StreamingCollection<GetOperation>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_undoOperations = new StreamingCollection<GetOperation>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_resolvedConflicts = new StreamingCollection<Conflict>((Command) this)
      {
        HandleExceptions = false
      };
      ItemPathPair itemPathPair = newPath != null ? ItemSpec.toServerItem(this.RequestContext, newPath, workspace, false) : new ItemPathPair();
      if (!string.IsNullOrEmpty(itemPathPair.ProjectNamePath))
      {
        this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.PendChange, itemPathPair);
        lockLevel = ChangeRequest.EnforceSingleCheckout(this.m_versionControlRequestContext, workspace, itemPathPair.ProjectNamePath, lockLevel);
      }
      int num = -2;
      Guid dataspaceIdentifier = Guid.Empty;
      if (newProperties != null && newProperties.Length != 0)
      {
        ObjectBinder<PropertyDataspaceIdPair> current1 = this.m_db.ReservePropertyIds(1, conflictId).GetCurrent<PropertyDataspaceIdPair>();
        if (current1.MoveNext())
        {
          PropertyDataspaceIdPair current2 = current1.Current;
          num = current2.StartRange;
          dataspaceIdentifier = current2.DataspaceId;
          this.RequestContext.GetService<TeamFoundationPropertyService>().SetProperties(this.RequestContext, new ArtifactSpec(VersionControlPropertyKinds.ImmutableVersionedItem, num, 0, dataspaceIdentifier), (IEnumerable<PropertyValue>) newProperties);
        }
      }
      this.m_results = this.m_db.Resolve(workspace, this.m_conflictId, resolution, itemPathPair, encoding, num, new Guid?(dataspaceIdentifier), lockLevel, disallowPropertyChangesOnAutoMerge, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
      ObjectBinder<ServerException> current = this.m_results.GetCurrent<ServerException>();
      if (current.MoveNext())
        throw current.Current;
      this.m_results.NextResult();
      if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
        this.m_propertyMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
      if (itemAttributeFilters != null && itemAttributeFilters.Length != 0)
        this.m_attributeMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemAttributeFilters, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
      this.m_state = CommandResolve.State.Operations;
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandResolve.State.Operations)
      {
        if (this.m_operationsBinder == null)
          this.m_operationsBinder = this.m_results.GetCurrent<GetOperation>();
        bool flag = true;
        while (!this.IsCacheFull && (flag = this.m_operationsBinder.MoveNext()))
        {
          GetOperation current = this.m_operationsBinder.Current;
          this.m_signer.SignObject((ISignable) current);
          this.m_operations.Enqueue(current);
        }
        if (!flag)
        {
          this.m_operations.IsComplete = true;
          this.m_state = CommandResolve.State.UndoOperations;
        }
        this.m_state = CommandResolve.State.Properties;
      }
      if (this.m_state == CommandResolve.State.Properties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_operations);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandResolve.State.Attributes;
      }
      if (this.m_state == CommandResolve.State.Attributes)
      {
        if (this.m_attributeMerger != null)
        {
          if (!this.m_hasMoreAttributes)
            this.m_attributeMerger.Execute(this.m_operations);
          this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
        }
        this.m_state = this.m_hasMoreProperties ? CommandResolve.State.Properties : (this.m_hasMoreAttributes ? CommandResolve.State.Attributes : (!this.m_operations.IsComplete ? CommandResolve.State.Operations : CommandResolve.State.UndoOperations));
      }
      if (this.m_state == CommandResolve.State.UndoOperations && !this.IsCacheFull)
      {
        if (this.m_undoOperationsBinder == null)
        {
          this.m_results.NextResult();
          this.m_undoOperationsBinder = this.m_results.GetCurrent<GetOperation>();
        }
        bool flag = true;
        while (!this.IsCacheFull && (flag = this.m_undoOperationsBinder.MoveNext()))
        {
          GetOperation current = this.m_undoOperationsBinder.Current;
          this.m_signer.SignObject((ISignable) current);
          this.m_undoOperations.Enqueue(current);
        }
        if (!flag)
        {
          this.m_undoOperations.IsComplete = true;
          this.m_resolvedConflicts.IsComplete = true;
          this.m_state = CommandResolve.State.Flags;
        }
        this.m_state = CommandResolve.State.UndoOperationProperties;
      }
      if (this.m_state == CommandResolve.State.UndoOperationProperties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_undoOperations);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandResolve.State.Attributes;
      }
      if (this.m_state == CommandResolve.State.UndoOperationAttributes)
      {
        if (this.m_attributeMerger != null)
        {
          if (!this.m_hasMoreAttributes)
            this.m_attributeMerger.Execute(this.m_undoOperations);
          this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
        }
        this.m_state = this.m_hasMoreProperties ? CommandResolve.State.UndoOperationProperties : (this.m_hasMoreAttributes ? CommandResolve.State.UndoOperationAttributes : (!this.m_undoOperations.IsComplete ? CommandResolve.State.UndoOperations : CommandResolve.State.Flags));
      }
      if (this.m_state == CommandResolve.State.Flags)
      {
        this.m_results.NextResult();
        this.Flags = this.m_results.GetCurrent<ChangePendedFlags>().Items[0];
        if ((this.Flags & ChangePendedFlags.WorkingFolderMappingsUpdated) != ChangePendedFlags.Unknown)
          Workspace.RemoveFromCache(this.m_versionControlRequestContext, this.m_workspace.OwnerId, this.m_workspace.Name);
        this.m_state = CommandResolve.State.Complete;
      }
      this.m_signer.FlushDeferredSignatures();
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

    internal StreamingCollection<GetOperation> UndoOperations => this.m_undoOperations;

    internal StreamingCollection<Conflict> Conflicts => this.m_resolvedConflicts;

    public ChangePendedFlags Flags { get; set; }

    private enum State
    {
      Operations,
      Attributes,
      Properties,
      UndoOperations,
      UndoOperationAttributes,
      UndoOperationProperties,
      Flags,
      Complete,
    }
  }
}
