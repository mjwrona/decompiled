// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandPendChanges
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandPendChanges : VersionControlCommand
  {
    private Workspace m_workspace;
    private bool m_silent;
    private RequestType m_requestType;
    private StreamingCollection<GetOperation> m_changes;
    private List<Failure> m_failures;
    private UrlSigner m_signer;
    private VersionedItemComponent m_db;
    private ResultCollection m_results;
    private ObjectBinder<GetOperation> m_getBinder;
    private ExpandedChangeDbPagingManager m_changeManager;
    private PendChangesOptions m_options;
    private SupportedFeatures m_supportedFeatures;
    private CommandMerge m_mergeCommand;
    private ChangeRequest[] m_requests;
    private ExpandedChange m_expandedChange;
    private int m_numwarnings;
    private PropertyMerger<GetOperation> m_attributeMerger;
    private PropertyMerger<GetOperation> m_propertyMerger;
    private bool m_hasMoreProperties;
    private bool m_hasMoreAttributes;
    private CommandPendChanges.State m_state;

    public CommandPendChanges(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(
      Workspace workspace,
      ChangeRequest[] requests,
      PendChangesOptions options,
      SupportedFeatures supportedFeatures,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters)
    {
      this.m_workspace = workspace;
      this.m_silent = (options & PendChangesOptions.Silent) != 0;
      this.m_options = options;
      this.m_supportedFeatures = supportedFeatures;
      this.m_changes = new StreamingCollection<GetOperation>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_failures = new List<Failure>();
      List<DeferredQuery> deferredQueryList = new List<DeferredQuery>(requests[0].RequestType == RequestType.Add ? 0 : requests.Length);
      this.m_requestType = requests[0].RequestType;
      this.m_requests = requests;
      int num = 0;
      foreach (ChangeRequest request in requests)
      {
        if (request.RequestType == RequestType.Lock && request.LockLevel == LockLevel.None)
          ++num;
        if (request.RequestType != this.m_requestType)
          throw new InconsistentRequestTypesException(this.m_requestType, request.RequestType);
      }
      bool recursiveRequest = this.m_requestType == RequestType.Branch || this.m_requestType == RequestType.Delete || this.m_requestType == RequestType.Undelete || this.m_requestType == RequestType.Lock && num == 0;
      if (num != requests.Length)
        this.SecurityWrapper.CheckWorkspacePermission(this.m_versionControlRequestContext, 2, this.m_workspace);
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("workspaceName", (object) workspace?.Name);
      ctData.Add("length", (object) requests?.Length);
      ctData.Add("requestType", (object) this.m_requestType);
      ClientTrace.Publish(this.RequestContext, "PendChanges", ctData);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_signer = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      List<Exception> exceptions = new List<Exception>();
      for (int requestIndex = 0; requestIndex < requests.Length; ++requestIndex)
      {
        ChangeRequest request = requests[requestIndex];
        try
        {
          request.Expand(this.m_versionControlRequestContext, this.m_db, this.m_workspace, (IList<DeferredQuery>) deferredQueryList, (IList<Exception>) exceptions, requestIndex);
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (ArgumentException ex)
        {
          this.RequestContext.TraceException(700052, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.Command, (Exception) ex);
          this.m_failures.Add(new Failure((Exception) ex, request.ItemSpec.Item, request.RequestType));
        }
        catch (ApplicationException ex)
        {
          this.RequestContext.TraceException(700053, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.Command, (Exception) ex);
          this.m_failures.Add(new Failure((Exception) ex, request.ItemSpec.Item, request.RequestType));
        }
        finally
        {
          foreach (Exception e in exceptions)
            this.m_failures.Add(new Failure(e, request.ItemSpec.Item, request.RequestType));
          exceptions.Clear();
        }
      }
      DeferredQuery.ExecuteOptimizedQueries(deferredQueryList, this.m_workspace, this.m_db);
      ExpandedChangeEnumerator changeEnum = new ExpandedChangeEnumerator(this.m_versionControlRequestContext, requests, this.m_workspace, this.m_db);
      this.m_changeManager = new ExpandedChangeDbPagingManager(this.m_versionControlRequestContext, changeEnum, recursiveRequest);
      this.m_failures.AddRange((IEnumerable<Failure>) changeEnum.Failures);
      PendChangesNotification notificationEvent = new PendChangesNotification(this.RequestContext.ContextId, this.RequestContext.GetUserIdentity(), this.m_workspace.Name, this.m_workspace.OwnerName, this.m_changeManager.GetFirstPage());
      this.m_versionControlRequestContext.RequestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(this.RequestContext, (object) notificationEvent);
      this.m_changeManager.TryGetNextItem(out this.m_expandedChange);
      if (itemAttributeFilters != null && itemAttributeFilters.Length != 0 && this.m_requestType != RequestType.Branch && this.m_requestType != RequestType.Add)
        this.m_attributeMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemAttributeFilters, (VersionControlCommand) this, VersionControlPropertyKinds.VersionedItem);
      if (itemPropertyFilters != null && itemPropertyFilters.Length != 0)
        this.m_propertyMerger = new PropertyMerger<GetOperation>(this.m_versionControlRequestContext, itemPropertyFilters, (VersionControlCommand) this, VersionControlPropertyKinds.ImmutableVersionedItem);
      if (this.m_requestType == RequestType.Edit && (this.m_options & PendChangesOptions.GetLatestOnCheckout) != PendChangesOptions.None && this.m_versionControlRequestContext.VersionControlService.GetAllowAsynchronousCheckoutInServerWorkspaces(this.m_versionControlRequestContext))
        this.m_failures.Add(new Failure((Exception) new GetLatestOnCheckoutDisabledException())
        {
          Severity = SeverityType.Warning
        });
      this.m_state = CommandPendChanges.State.GetOps;
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      if (this.m_state == CommandPendChanges.State.GetOps)
      {
        while (this.m_expandedChange != null)
        {
          ChangeRequest request1 = this.m_requests[this.m_expandedChange.requestIndex];
          if (this.m_getBinder == null && this.m_mergeCommand == null)
          {
            if (!this.ApplyTeamProjectRestrictions(this.m_expandedChange, this.m_requestType, request1) || !this.HasItemPermission(this.m_expandedChange, request1))
            {
              this.m_changeManager.TryGetNextItem(out this.m_expandedChange);
              continue;
            }
            bool flag = false;
            try
            {
              bool getLatest = false;
              switch (this.m_requestType)
              {
                case RequestType.Add:
                  List<ExpandedChange> expandedChanges = new List<ExpandedChange>(this.m_requests.Length);
                  expandedChanges.Add(this.m_expandedChange);
                  ExpandedChange change1;
                  while (this.m_changeManager.TryGetNextItem(out change1))
                  {
                    if (this.ApplyTeamProjectRestrictions(change1, this.m_requestType, request1) && this.HasItemPermission(change1, request1))
                      expandedChanges.Add(change1);
                  }
                  this.m_results = this.m_db.PendAdd(this.m_workspace, expandedChanges, this.m_requests, this.m_silent, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
                  break;
                case RequestType.Branch:
                  if (this.m_db != null)
                  {
                    this.m_db.Dispose();
                    this.m_db = (VersionedItemComponent) null;
                  }
                  ItemSpec target = new ItemSpec(this.m_expandedChange.targetItemPathPair, request1.ItemSpec.RecursionType);
                  this.m_mergeCommand = new CommandMerge(this.m_versionControlRequestContext);
                  this.m_mergeCommand.Execute(this.m_workspace, request1.ItemSpec, target, (VersionSpec) new ChangesetVersionSpec(1), request1.VersionSpec, this.m_silent ? MergeOptionsEx.Silent : MergeOptionsEx.None, request1.LockLevel, true, false, (string[]) null, (string[]) null);
                  break;
                case RequestType.Edit:
                  getLatest = ChangeRequest.EnforceGetLatestOnCheckout(this.m_versionControlRequestContext, this.m_expandedChange.serverItem, this.m_options, this.m_supportedFeatures);
                  if (getLatest)
                    this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, this.m_expandedChange.itemPathPair);
                  this.m_results = this.m_db.PendEdit(this.m_workspace, this.m_expandedChange.itemPathPair, request1.Encoding, this.m_expandedChange.requiredLockLevel, this.m_silent, getLatest);
                  break;
                case RequestType.Delete:
                  this.m_results = this.m_db.PendDelete(this.m_workspace, this.m_expandedChange.itemPathPair, this.m_expandedChange.requiredLockLevel, this.m_silent);
                  break;
                case RequestType.Lock:
                  LockRequest lockRequest = new LockRequest();
                  lockRequest.TargetServerItem = this.m_expandedChange.serverItem;
                  lockRequest.LockLevel = request1.LockLevel;
                  lockRequest.RequiredLockLevel = this.m_expandedChange.requiredLockLevel;
                  if (request1.LockLevel == LockLevel.None && request1.ItemSpec.RecursionType != RecursionType.None)
                  {
                    List<LockRequest> lockRequestList = new List<LockRequest>();
                    lockRequestList.Add(lockRequest);
                    ExpandedChange change2;
                    while (this.m_changeManager.TryGetNextItem(out change2))
                    {
                      if (this.ApplyTeamProjectRestrictions(change2, this.m_requestType, request1) && this.HasItemPermission(change2, request1))
                        lockRequestList.Add(new LockRequest()
                        {
                          TargetServerItem = change2.serverItem,
                          LockLevel = request1.LockLevel,
                          RequiredLockLevel = change2.requiredLockLevel
                        });
                    }
                    this.m_results = this.m_db.LockItems(this.m_workspace, lockRequestList.ToArray(), this.m_silent, true);
                    break;
                  }
                  try
                  {
                    this.m_results = this.m_db.LockItems(this.m_workspace, new LockRequest[1]
                    {
                      lockRequest
                    }, (this.m_silent ? 1 : 0) != 0, false);
                    break;
                  }
                  catch (NoLockExistsException ex)
                  {
                    if (request1.ItemSpec.RecursionType == RecursionType.None)
                    {
                      throw;
                    }
                    else
                    {
                      this.RequestContext.TraceException(700054, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.Command, (Exception) ex);
                      break;
                    }
                  }
                case RequestType.Rename:
                  this.m_expandedChange.requiredLockLevel = ChangeRequest.EnforceSingleCheckout(this.m_versionControlRequestContext, this.m_workspace, this.m_expandedChange.targetServerItem, this.m_expandedChange.requiredLockLevel);
                  this.m_results = this.m_db.PendRename(this.m_workspace, this.m_expandedChange.itemPathPair, this.m_expandedChange.targetItemPathPair, this.m_expandedChange.requiredLockLevel, this.m_silent, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
                  break;
                case RequestType.Undelete:
                  if (!string.IsNullOrEmpty(this.m_expandedChange.targetServerItem))
                    this.m_expandedChange.requiredLockLevel = ChangeRequest.EnforceSingleCheckout(this.m_versionControlRequestContext, this.m_workspace, this.m_expandedChange.targetServerItem, this.m_expandedChange.requiredLockLevel);
                  this.m_results = this.m_db.PendUndelete(this.m_workspace, this.m_expandedChange.itemPathPair, this.m_expandedChange.targetItemPathPair, this.m_expandedChange.requiredLockLevel, this.m_silent, this.m_versionControlRequestContext.MaxSupportedServerPathLength);
                  break;
                case RequestType.Property:
                  List<ExpandedChange> expandedChangeList1 = new List<ExpandedChange>(this.m_requests.Length);
                  List<ArtifactSpec> artifactSpecList = new List<ArtifactSpec>();
                  this.m_expandedChange.dataspaceId = VersionControlSqlResourceComponent.GetDataspaceIdentifierDebug(this.m_versionControlRequestContext, ProjectUtility.GetProjectIdFromPath(this.m_versionControlRequestContext.RequestContext, this.m_expandedChange.serverItem), this.m_expandedChange.serverItem);
                  expandedChangeList1.Add(this.m_expandedChange);
                  if (this.m_expandedChange.propertyId != -1)
                    artifactSpecList.Add(new ArtifactSpec(VersionControlPropertyKinds.ImmutableVersionedItem, this.m_expandedChange.propertyId, 0, this.m_expandedChange.dataspaceId));
                  int numIdsToReserve = 1;
                  ExpandedChange change3;
                  while (this.m_changeManager.TryGetNextItem(out change3))
                  {
                    change3.dataspaceId = VersionControlSqlResourceComponent.GetDataspaceIdentifierDebug(this.m_versionControlRequestContext, ProjectUtility.GetProjectIdFromPath(this.m_versionControlRequestContext.RequestContext, change3.serverItem), change3.serverItem);
                    expandedChangeList1.Add(change3);
                    if (change3.propertyId != -1)
                      artifactSpecList.Add(new ArtifactSpec(VersionControlPropertyKinds.ImmutableVersionedItem, change3.propertyId, 0, change3.dataspaceId));
                    ++numIdsToReserve;
                  }
                  ObjectBinder<PropertyDataspaceIdPair> current = this.m_db.ReservePropertyIds(numIdsToReserve, 0).GetCurrent<PropertyDataspaceIdPair>();
                  if (!current.MoveNext())
                    throw new TeamFoundationServerException("Error while reserving the property ids");
                  int startRange = current.Current.StartRange;
                  IVssRequestContext requestContext = this.m_versionControlRequestContext.RequestContext;
                  TeamFoundationPropertyService service = requestContext.GetService<TeamFoundationPropertyService>();
                  Dictionary<int, List<PropertyValue>> dictionary = new Dictionary<int, List<PropertyValue>>(artifactSpecList.Count);
                  using (TeamFoundationDataReader properties = service.GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) null))
                  {
                    foreach (ArtifactPropertyValue artifactPropertyValue in properties.Current<StreamingCollection<ArtifactPropertyValue>>())
                    {
                      int key = (int) artifactPropertyValue.Spec.Id[0] << 24 | (int) artifactPropertyValue.Spec.Id[1] << 16 | (int) artifactPropertyValue.Spec.Id[2] << 8 | (int) artifactPropertyValue.Spec.Id[3];
                      dictionary[key] = new List<PropertyValue>((IEnumerable<PropertyValue>) artifactPropertyValue.PropertyValues);
                    }
                  }
                  List<ArtifactPropertyValue> artifactPropertyValueList = new List<ArtifactPropertyValue>();
                  List<ExpandedChange> expandedChangeList2 = new List<ExpandedChange>(expandedChangeList1.Count);
                  foreach (ExpandedChange expandedChange in expandedChangeList1)
                  {
                    ChangeRequest request2 = this.m_requests[expandedChange.requestIndex];
                    ArgumentUtility.CheckForNull<PropertyValue[]>(request2.Properties, "changeRequest.Properties");
                    PropertyValue[] properties = request2.Properties;
                    List<PropertyValue> existingProperties = (List<PropertyValue>) null;
                    dictionary.TryGetValue(expandedChange.propertyId, out existingProperties);
                    PropertyValue[] propertyValueArray = ChangeRequest.MergeProperties((IEnumerable<PropertyValue>) existingProperties, properties);
                    if (propertyValueArray != null && propertyValueArray.Length != 0)
                    {
                      expandedChange.propertyId = startRange++;
                      artifactPropertyValueList.Add(new ArtifactPropertyValue(new ArtifactSpec(VersionControlPropertyKinds.ImmutableVersionedItem, expandedChange.propertyId, 0, expandedChange.dataspaceId), (IEnumerable<PropertyValue>) propertyValueArray));
                      expandedChangeList2.Add(expandedChange);
                    }
                    else if (existingProperties != null)
                    {
                      expandedChange.propertyId = -1;
                      expandedChangeList2.Add(expandedChange);
                    }
                    else if (request2.ItemSpec.RecursionType == RecursionType.None)
                      this.Failures.Add(new Failure()
                      {
                        ServerItem = expandedChange.serverItem,
                        Message = Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format("NoPropertiesToDeleteMessage", (object) expandedChange.serverItem),
                        Severity = SeverityType.Warning
                      });
                  }
                  service.SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueList);
                  this.m_results = this.m_db.PendPropertyChange(this.m_workspace, (IEnumerable<ExpandedChange>) expandedChangeList2, this.m_silent);
                  break;
                default:
                  throw new NotSupportedException(Resources.Format("ChangeTypeNotSupported", (object) this.m_expandedChange.serverItem, (object) request1.RequestType));
              }
              if (this.m_results != null)
              {
                if (!this.m_silent | getLatest)
                  this.m_getBinder = this.m_results.GetCurrent<GetOperation>();
              }
            }
            catch (ItemNotFoundException ex)
            {
              this.RequestContext.TraceException(700055, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.Command, (Exception) ex);
              this.m_failures.Add(new Failure((Exception) new ItemNotFoundException(this.m_versionControlRequestContext.RequestContext, this.m_workspace, request1.ItemSpec, request1.VersionSpec, request1.m_deletedState)));
              flag = true;
            }
            catch (RequestCanceledException ex)
            {
              if (this.m_mergeCommand != null)
              {
                this.m_mergeCommand.Cancel();
                this.m_mergeCommand.Dispose();
                this.m_mergeCommand = (CommandMerge) null;
              }
              throw;
            }
            catch (ApplicationException ex)
            {
              this.RequestContext.TraceException(700056, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.Command, (Exception) ex);
              this.m_failures.Add(new Failure((Exception) ex));
              flag = true;
            }
            catch (Exception ex)
            {
              if (this.m_requestType == RequestType.Branch && this.m_mergeCommand != null)
              {
                if (!(ex is InvalidPathException))
                  TeamFoundationEventLog.Default.LogException(this.m_mergeCommand.RequestContext, FrameworkResources.CommandStopped((object) this.m_mergeCommand.Name), ex);
                this.m_mergeCommand.Cancel();
                this.m_failures.Add(new Failure(ex));
                flag = true;
              }
              else
                throw;
            }
            if (flag && this.m_mergeCommand != null)
            {
              this.m_mergeCommand.Dispose();
              this.m_mergeCommand = (CommandMerge) null;
            }
          }
          bool flag1 = true;
          if (this.m_mergeCommand != null)
          {
            while (!this.IsCacheFull && (flag1 = this.m_mergeCommand.Operations.MoveNext()))
            {
              GetOperation current = this.m_mergeCommand.Operations.Current;
              this.m_signer.SignObject((ISignable) current);
              this.m_changes.Enqueue(current);
            }
          }
          else if (this.m_getBinder != null)
          {
            while (!this.IsCacheFull && (flag1 = this.m_getBinder.MoveNext()))
            {
              GetOperation current = this.m_getBinder.Current;
              this.m_signer.SignObject((ISignable) current);
              this.m_changes.Enqueue(current);
            }
          }
          this.m_signer.FlushDeferredSignatures();
          if (!flag1)
          {
            if (this.m_mergeCommand != null)
            {
              while (this.m_mergeCommand.Failures.MoveNext())
                this.m_failures.Add(this.m_mergeCommand.Failures.Current);
            }
            else if (this.m_getBinder != null)
            {
              if (this.m_results.TryNextResult() && this.m_numwarnings < this.m_versionControlRequestContext.VersionControlService.GetMaxItemsPerRequest(this.m_versionControlRequestContext))
              {
                this.m_numwarnings += this.m_results.GetCurrent<Warning>().Items.Count;
                Warning.createFailuresFromWarnings(this.m_versionControlRequestContext, (IList) this.m_results.GetCurrent<Warning>().Items, (IList) this.m_failures);
              }
              if (this.m_requestType == RequestType.Add)
              {
                this.m_results.NextResult();
                this.m_failures.AddRange((IEnumerable<Failure>) this.m_results.GetCurrent<Failure>().Items);
              }
            }
            if (this.m_requestType == RequestType.Rename)
            {
              this.m_results.NextResult();
              this.Flags |= this.m_results.GetCurrent<ChangePendedFlags>().Items[0];
            }
            if (this.m_mergeCommand != null)
            {
              this.m_mergeCommand.Dispose();
              this.m_mergeCommand = (CommandMerge) null;
            }
            else if (this.m_results != null)
            {
              this.m_results.Dispose();
              this.m_results = (ResultCollection) null;
              this.m_getBinder = (ObjectBinder<GetOperation>) null;
            }
          }
          if (!this.IsCacheFull)
            this.m_changeManager.TryGetNextItem(out this.m_expandedChange);
          else
            break;
        }
        if (this.m_expandedChange == null && (this.Flags & ChangePendedFlags.WorkingFolderMappingsUpdated) != ChangePendedFlags.Unknown)
          Workspace.RemoveFromCache(this.m_versionControlRequestContext, this.m_workspace.OwnerId, this.m_workspace.Name);
        this.m_state = CommandPendChanges.State.Properties;
      }
      if (this.m_state == CommandPendChanges.State.Properties)
      {
        if (this.m_propertyMerger != null)
        {
          if (!this.m_hasMoreProperties)
            this.m_propertyMerger.Execute(this.m_changes);
          this.m_hasMoreProperties = this.m_propertyMerger.TryMergeNextPage();
        }
        this.m_state = CommandPendChanges.State.Attributes;
      }
      if (this.m_state == CommandPendChanges.State.Attributes && this.m_attributeMerger != null)
      {
        if (!this.m_hasMoreAttributes)
          this.m_attributeMerger.Execute(this.m_changes);
        this.m_hasMoreAttributes = this.m_attributeMerger.TryMergeNextPage();
      }
      this.m_state = this.m_hasMoreProperties ? CommandPendChanges.State.Properties : (this.m_hasMoreAttributes ? CommandPendChanges.State.Attributes : (this.m_expandedChange != null ? CommandPendChanges.State.GetOps : CommandPendChanges.State.Complete));
    }

    private bool HasItemPermission(ExpandedChange expandedChange, ChangeRequest request)
    {
      try
      {
        this.CheckItemPermission(expandedChange, request);
        expandedChange.requiredLockLevel = ChangeRequest.EnforceSingleCheckout(this.m_versionControlRequestContext, this.m_workspace, expandedChange.serverItem, request.LockLevel);
        return true;
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (ApplicationException ex)
      {
        this.RequestContext.TraceException(700057, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.Command, (Exception) ex);
        this.m_failures.Add(new Failure((Exception) ex));
        return false;
      }
    }

    private void CheckItemPermission(ExpandedChange expandedChange, ChangeRequest request)
    {
      bool flag = request.RequestType == RequestType.Edit && this.m_versionControlRequestContext.VersionControlService.GetAllowAsynchronousCheckoutInServerWorkspaces(this.m_versionControlRequestContext);
      if (flag && (request.LockLevel == LockLevel.Unchanged || request.LockLevel == LockLevel.None))
        return;
      VersionedItemPermissions permissionRequired1 = VersionedItemPermissions.None;
      VersionedItemPermissions permissionRequired2 = VersionedItemPermissions.PendChange;
      if (request.RequestType == RequestType.Branch)
        permissionRequired1 |= VersionedItemPermissions.Read;
      else if (request.LockLevel == LockLevel.None && request.RequestType == RequestType.Lock)
      {
        if (!this.SecurityWrapper.HasWorkspacePermission(this.m_versionControlRequestContext, 2, this.m_workspace))
          permissionRequired1 |= VersionedItemPermissions.UnlockOther;
      }
      else
        permissionRequired1 |= VersionedItemPermissions.PendChange;
      if (request.RequestType == RequestType.Undelete)
        permissionRequired1 |= VersionedItemPermissions.Read;
      if (request.LockLevel != LockLevel.Unchanged && request.LockLevel != LockLevel.None)
      {
        permissionRequired2 |= VersionedItemPermissions.Lock;
        if (request.RequestType != RequestType.Branch)
          permissionRequired1 |= VersionedItemPermissions.Lock;
      }
      if (flag)
      {
        permissionRequired1 &= ~VersionedItemPermissions.PendChange;
        permissionRequired2 &= ~VersionedItemPermissions.PendChange;
      }
      this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, permissionRequired1, expandedChange.itemPathPair);
      if (expandedChange.targetServerItem != null)
        this.SecurityWrapper.CheckItemPermission(this.m_versionControlRequestContext, permissionRequired2, expandedChange.targetItemPathPair);
      if (request.RequestType == RequestType.Rename || request.RequestType == RequestType.Delete)
      {
        this.SecurityWrapper.CheckItemPermissionForAllChildren(this.m_versionControlRequestContext, VersionedItemPermissions.Read, expandedChange.itemPathPair);
      }
      else
      {
        if (request.RequestType != RequestType.Undelete)
          return;
        this.SecurityWrapper.CheckItemPermissionForAllChildren(this.m_versionControlRequestContext, VersionedItemPermissions.Read | VersionedItemPermissions.PendChange, expandedChange.itemPathPair);
      }
    }

    private bool ApplyTeamProjectRestrictions(
      ExpandedChange expandedChange,
      RequestType requestType,
      ChangeRequest request)
    {
      try
      {
        if (expandedChange.targetServerItem != null)
          this.m_versionControlRequestContext.VersionControlService.GetTeamProjectFolder(this.m_versionControlRequestContext).ValidateChange(this.m_versionControlRequestContext, request.ItemType, requestType, expandedChange.targetServerItem);
        if (expandedChange.targetServerItem == null || requestType == RequestType.Rename)
          this.m_versionControlRequestContext.VersionControlService.GetTeamProjectFolder(this.m_versionControlRequestContext).ValidateChange(this.m_versionControlRequestContext, request.ItemType, requestType, expandedChange.serverItem);
        return true;
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (ApplicationException ex)
      {
        this.RequestContext.TraceException(700058, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.Command, (Exception) ex);
        this.m_failures.Add(new Failure((Exception) ex));
        return false;
      }
    }

    public StreamingCollection<GetOperation> Changes
    {
      get => this.m_changes;
      set => this.m_changes = value;
    }

    public List<Failure> Failures
    {
      get => this.m_failures;
      set => this.m_failures = value;
    }

    public ChangePendedFlags Flags { get; set; }

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
      if (this.m_mergeCommand != null)
      {
        this.m_mergeCommand.Dispose();
        this.m_mergeCommand = (CommandMerge) null;
      }
      if (this.m_changeManager != null)
      {
        this.m_changeManager.Dispose();
        this.m_changeManager = (ExpandedChangeDbPagingManager) null;
      }
      if (this.m_attributeMerger != null)
      {
        this.m_attributeMerger.Dispose();
        this.m_attributeMerger = (PropertyMerger<GetOperation>) null;
      }
      if (this.m_propertyMerger == null)
        return;
      this.m_propertyMerger.Dispose();
      this.m_propertyMerger = (PropertyMerger<GetOperation>) null;
    }

    private enum State
    {
      GetOps,
      Attributes,
      Properties,
      Complete,
    }
  }
}
