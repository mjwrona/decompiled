// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.TeamFoundationVersionControlService
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class TeamFoundationVersionControlService : IVssFrameworkService
  {
    private object m_initLock = new object();
    private object m_workspaceAccessLock = new object();
    private DateTime m_earliestChangesetDate;
    private WorkspaceCache m_workspaceCache;
    private IntegrationInterface m_integrationInterface;
    private FileTypeManager m_fileTypeManager;
    private TeamProjectFolder m_teamProjectFolder;
    private IVssRegistryService m_serviceSettings;
    private ITeamFoundationSqlNotificationService m_sqlNotificationService;
    private Dictionary<string, object> m_cache;
    private ReaderWriterLock m_lock = new ReaderWriterLock();
    private LocalItemExclusionSet m_defaultLocalItemExclusionSet;
    private const string c_registrySettingsPath = "/Service/VersionControl/Settings/";
    private const TicketType c_downloadTicketTypeDefault = TicketType.RSATicket;

    internal AdministrationComponent GetAdministrationComponent(
      VersionControlRequestContext vcRequestContext)
    {
      AdministrationComponent component = vcRequestContext.RequestContext.CreateComponent<AdministrationComponent>("VersionControl");
      component.VersionControlRequestContext = vcRequestContext;
      return component;
    }

    internal UpdateStatisticsComponent GetUpdateStatisticsComponent(
      VersionControlRequestContext vcRequestContext)
    {
      UpdateStatisticsComponent component = vcRequestContext.RequestContext.CreateComponent<UpdateStatisticsComponent>("VersionControl");
      component.VersionControlRequestContext = vcRequestContext;
      return component;
    }

    internal CodeChurnComponent GetCodeChurnComponent(VersionControlRequestContext vcRequestContext)
    {
      CodeChurnComponent component = vcRequestContext.RequestContext.CreateComponent<CodeChurnComponent>("VersionControl");
      component.VersionControlRequestContext = vcRequestContext;
      return component;
    }

    internal ConfigurationResourceComponent GetConfigurationResourceComponent(
      VersionControlRequestContext vcRequestContext)
    {
      ConfigurationResourceComponent component = vcRequestContext.RequestContext.CreateComponent<ConfigurationResourceComponent>("VersionControl");
      component.VersionControlRequestContext = vcRequestContext;
      return component;
    }

    internal ContentComponent GetContentComponent(VersionControlRequestContext vcRequestContext)
    {
      ContentComponent component = vcRequestContext.RequestContext.CreateComponent<ContentComponent>("VersionControl");
      component.VersionControlRequestContext = vcRequestContext;
      return component;
    }

    internal LabelComponent GetLabelComponent(VersionControlRequestContext vcRequestContext)
    {
      LabelComponent component = vcRequestContext.RequestContext.CreateComponent<LabelComponent>("VersionControl");
      component.VersionControlRequestContext = vcRequestContext;
      return component;
    }

    internal CheckinNoteComponent GetCheckinNoteComponent(
      VersionControlRequestContext vcRequestContext)
    {
      CheckinNoteComponent component = vcRequestContext.RequestContext.CreateComponent<CheckinNoteComponent>("VersionControl");
      component.VersionControlRequestContext = vcRequestContext;
      return component;
    }

    public void CreateAnnotation(
      IVssRequestContext requestContext,
      string annotationName,
      string annotatedItem,
      int version,
      string annotationValue,
      string comment,
      bool overwrite)
    {
      Annotation.CreateAnnotation(this.GetVersionControlRequestContext(requestContext), annotationName, annotatedItem, version, annotationValue, comment, overwrite);
    }

    public void DeleteAnnotation(
      IVssRequestContext requestContext,
      string annotationName,
      string annotatedItem,
      int version,
      string annotationValue)
    {
      Annotation.DeleteAnnotation(this.GetVersionControlRequestContext(requestContext), annotationName, annotatedItem, version, annotationValue);
    }

    public List<Annotation> QueryAnnotation(
      IVssRequestContext requestContext,
      string annotationName,
      string annotatedItem,
      int version)
    {
      return Annotation.QueryAnnotation(this.GetVersionControlRequestContext(requestContext), annotationName, annotatedItem, version);
    }

    internal List<ItemPair> GetChurnItemPairs(
      IVssRequestContext requestContext,
      int changeset,
      string lastServerItem,
      int batchSize)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryChurnItemPairs disposableObject = (CommandQueryChurnItemPairs) null;
      try
      {
        disposableObject = new CommandQueryChurnItemPairs(controlRequestContext);
        disposableObject.Execute(changeset, lastServerItem, batchSize);
        using (TeamFoundationDataReader foundationDataReader = new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.ItemPairs
        }))
          return new List<ItemPair>(foundationDataReader.CurrentEnumerable<ItemPair>());
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    internal List<ItemPair> GetFailedItemPairs(IVssRequestContext requestContext, int batchSize)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryChurnItemPairs disposableObject = (CommandQueryChurnItemPairs) null;
      try
      {
        disposableObject = new CommandQueryChurnItemPairs(controlRequestContext);
        disposableObject.Execute(batchSize);
        using (TeamFoundationDataReader foundationDataReader = new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.ItemPairs
        }))
          return new List<ItemPair>(foundationDataReader.CurrentEnumerable<ItemPair>());
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public void AddConflict(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      ConflictType conflictType,
      int itemId,
      int versionFrom,
      int pendingChangeId,
      string sourceLocalItem,
      string targetLocalItem,
      int reason)
    {
      this.AddConflict(requestContext, workspaceName, ownerName, conflictType, itemId, versionFrom, pendingChangeId, sourceLocalItem, targetLocalItem, reason, PathLength.Length259);
    }

    public void AddConflict(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      ConflictType conflictType,
      int itemId,
      int versionFrom,
      int pendingChangeId,
      string sourceLocalItem,
      string targetLocalItem,
      int reason,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      validation.checkVersionNumber(versionFrom, nameof (versionFrom));
      ArgumentUtility.CheckForBothStringsNullOrEmpty(sourceLocalItem, nameof (sourceLocalItem), targetLocalItem, nameof (targetLocalItem));
      validation.checkLocalItem(sourceLocalItem, nameof (sourceLocalItem), true, false, true, false);
      validation.checkLocalItem(targetLocalItem, nameof (targetLocalItem), true, false, true, false);
      Conflict.AddConflict(controlRequestContext, Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName), conflictType, itemId, versionFrom, pendingChangeId, sourceLocalItem, targetLocalItem, reason);
    }

    public TeamFoundationDataReader QueryConflicts(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      ItemSpec[] items)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      controlRequestContext.RequestContext.TraceEnter(7000303, TraceArea.Conflicts, TraceLayer.WebService, "QueryConflicts_TFVCService");
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      validation.check((IValidatable[]) items, nameof (items), true);
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName);
      CommandQueryConflicts disposableObject = (CommandQueryConflicts) null;
      try
      {
        disposableObject = new CommandQueryConflicts(controlRequestContext);
        disposableObject.Execute(workspace, items);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Conflicts
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
      finally
      {
        controlRequestContext.RequestContext.TraceLeave(7000304, TraceArea.Conflicts, TraceLayer.WebService, "QueryConflicts_TFVCService");
      }
    }

    public void RemoveLocalConflict(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      int conflictId)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      Conflict.RemoveLocalConflict(controlRequestContext, Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName), conflictId);
    }

    public void UpdatePendingState(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      PendingState[] updates)
    {
      this.UpdatePendingState(requestContext, workspaceName, workspaceOwner, updates, PathLength.Length259);
    }

    public void UpdatePendingState(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      PendingState[] updates,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), false);
      PendingState.Update(controlRequestContext, Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner), updates);
    }

    public TeamFoundationDataReader Resolve(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      int conflictId,
      Resolution resolution,
      string newPath,
      int encoding,
      LockLevel lockLevel,
      string[] itemAttributeFilters)
    {
      return this.Resolve(requestContext, workspaceName, ownerName, conflictId, resolution, newPath, encoding, lockLevel, (PropertyValue[]) null, (string[]) null, itemAttributeFilters, true);
    }

    public TeamFoundationDataReader Resolve(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
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
      return this.Resolve(requestContext, workspaceName, ownerName, conflictId, resolution, newPath, encoding, lockLevel, newProperties, itemPropertyFilters, itemAttributeFilters, disallowPropertyChangesOnAutoMerge, PathLength.Length259);
    }

    public TeamFoundationDataReader Resolve(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      int conflictId,
      Resolution resolution,
      string newPath,
      int encoding,
      LockLevel lockLevel,
      PropertyValue[] newProperties,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      bool disallowPropertyChangesOnAutoMerge,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      PathLength serverPathLength = controlRequestContext.MaxSupportedServerPathLength;
      validation.checkItem(ref newPath, nameof (newPath), true, false, this.GetAllow8Dot3Paths(controlRequestContext), true, serverPathLength);
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName);
      CommandResolve disposableObject = (CommandResolve) null;
      try
      {
        disposableObject = new CommandResolve(controlRequestContext);
        disposableObject.Execute(workspace, conflictId, resolution, newPath, encoding, lockLevel, newProperties, itemPropertyFilters, itemAttributeFilters, disallowPropertyChangesOnAutoMerge);
        if (!string.IsNullOrEmpty(newPath))
          Workspace.RemoveFromCache(controlRequestContext, workspace.OwnerId, workspace.Name);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[4]
        {
          (object) disposableObject.Operations,
          (object) disposableObject.UndoOperations,
          (object) disposableObject.Conflicts,
          (object) disposableObject.Flags
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader LabelItem(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      VersionControlLabel label,
      LabelItemSpec[] labelSpecs,
      LabelChildOption children)
    {
      return this.LabelItem(requestContext, workspaceName, workspaceOwner, label, labelSpecs, children, PathLength.Length259);
    }

    public TeamFoundationDataReader LabelItem(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      VersionControlLabel label,
      LabelItemSpec[] labelSpecs,
      LabelChildOption children,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      bool hasLocalItems;
      validation.checkLabelItemSpecs(labelSpecs, nameof (labelSpecs), out hasLocalItems, true);
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), !hasLocalItems);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), !hasLocalItems);
      validation.check((IValidatable) label, nameof (label), false);
      List<Failure> failures;
      return new TeamFoundationDataReader(new object[2]
      {
        (object) VersionControlLabel.LabelItem(controlRequestContext, Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner), label, labelSpecs, children, out failures),
        (object) failures
      });
    }

    public TeamFoundationDataReader UnlabelItem(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      string labelName,
      string labelScope,
      ItemSpec[] items,
      VersionSpec version)
    {
      return this.UnlabelItem(requestContext, workspaceName, workspaceOwner, labelName, labelScope, items, version, PathLength.Length259);
    }

    public TeamFoundationDataReader UnlabelItem(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      string labelName,
      string labelScope,
      ItemSpec[] items,
      VersionSpec version,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.checkLabelName(labelName, nameof (labelName), false, false);
      validation.checkServerItem(ref labelScope, nameof (labelScope), true, false, true, false, controlRequestContext.MaxSupportedServerPathLength);
      validation.check((IValidatable[]) items, nameof (items), false);
      validation.checkVersionSpec(version, nameof (version), false);
      Workspace localWorkspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      List<Failure> failures = new List<Failure>();
      return new TeamFoundationDataReader(new object[2]
      {
        (object) VersionControlLabel.UnlabelItem(controlRequestContext, localWorkspace, labelName, labelScope, items, version, failures),
        (object) failures
      });
    }

    public TeamFoundationDataReader QueryLabels(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      string labelName,
      string labelScope,
      string owner,
      string filterItem,
      VersionSpec versionFilterItem,
      bool includeItems,
      bool generateDownloadUrls)
    {
      return this.QueryLabels(requestContext, workspaceName, workspaceOwner, labelName, labelScope, owner, filterItem, versionFilterItem, includeItems, generateDownloadUrls, PathLength.Length259);
    }

    public TeamFoundationDataReader QueryLabels(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      string labelName,
      string labelScope,
      string owner,
      string filterItem,
      VersionSpec versionFilterItem,
      bool includeItems,
      bool generateDownloadUrls,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.checkLabelName(labelName, nameof (labelName), true, true);
      validation.checkServerItem(ref labelScope, nameof (labelScope), true, false, true, false, controlRequestContext.MaxSupportedServerPathLength);
      validation.checkIdentity(ref owner, nameof (owner), true);
      validation.checkItem(ref filterItem, nameof (filterItem), true, false, true, false, controlRequestContext.MaxSupportedServerPathLength);
      validation.checkVersionSpec(versionFilterItem, nameof (versionFilterItem), filterItem == null);
      CommandQueryLabels disposableObject = (CommandQueryLabels) null;
      try
      {
        disposableObject = new CommandQueryLabels(controlRequestContext);
        disposableObject.Execute(workspaceName, workspaceOwner, labelName, labelScope, owner, filterItem, versionFilterItem, includeItems, generateDownloadUrls);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Labels
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public List<LabelResult> DeleteLabel(
      IVssRequestContext requestContext,
      string labelName,
      string labelScope)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkLabelName(labelName, nameof (labelName), false, false);
      validation.checkServerItem(ref labelScope, nameof (labelScope), false, false, true, false, controlRequestContext.MaxSupportedServerPathLength);
      return VersionControlLabel.DeleteLabel(controlRequestContext, labelName, labelScope);
    }

    public TeamFoundationDataReader QueryHistory(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec itemSpec,
      VersionSpec versionItem,
      string user,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      int maxCount,
      bool includeFiles,
      bool generateDownloadUrls,
      bool slotMode,
      bool sortAscending)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.check((IValidatable) itemSpec, nameof (itemSpec), false);
      validation.checkVersionSpec(versionItem, nameof (versionItem), false, false);
      validation.checkIdentity(ref user, nameof (user), true);
      validation.checkVersionSpec(versionFrom, nameof (versionFrom), true, true);
      validation.checkVersionSpec(versionTo, nameof (versionTo), true, false);
      Workspace localWorkspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      CommandQueryHistory disposableObject = (CommandQueryHistory) null;
      try
      {
        disposableObject = new CommandQueryHistory(controlRequestContext);
        disposableObject.Execute(user, itemSpec, localWorkspace, versionItem, versionFrom, versionTo, maxCount, includeFiles, generateDownloadUrls, slotMode, sortAscending);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Changesets
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader CompareLabels(
      IVssRequestContext requestContext,
      string startLabelName,
      string startLabelScope,
      string endLabelName,
      string endLabelScope,
      int minChangeSet,
      int maxCount,
      bool includeItems = false)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkLabelName(startLabelName, nameof (startLabelName), false, false);
      validation.checkServerItem(ref startLabelScope, nameof (startLabelScope), true, false, true, false, controlRequestContext.MaxSupportedServerPathLength);
      validation.checkLabelName(endLabelName, nameof (endLabelName), false, false);
      validation.checkServerItem(ref endLabelScope, nameof (endLabelScope), true, false, true, false, controlRequestContext.MaxSupportedServerPathLength);
      validation.checkVersionNumber(minChangeSet, nameof (minChangeSet));
      CommandCompareLabels disposableObject = (CommandCompareLabels) null;
      try
      {
        disposableObject = new CommandCompareLabels(controlRequestContext);
        disposableObject.Execute(startLabelName, startLabelScope, endLabelName, endLabelScope, minChangeSet, maxCount, includeItems);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Changesets
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public Workspace CreateWorkspace(IVssRequestContext requestContext, Workspace workspace)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      controlRequestContext.Validation.check((IValidatable) workspace, nameof (workspace), true);
      return Workspace.CreateWorkspace(controlRequestContext, workspace);
    }

    public void DeleteWorkspace(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName, true, false, false, false).DeleteWorkspace(controlRequestContext);
    }

    public Workspace UpdateWorkspace(
      IVssRequestContext requestContext,
      string oldWorkspaceName,
      string ownerName,
      Workspace newWorkspace,
      int supportedFeatures)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      validation.checkWorkspaceName(oldWorkspaceName, nameof (oldWorkspaceName), false);
      validation.check((IValidatable) newWorkspace, nameof (newWorkspace), false);
      SupportedFeatures clientFeatures = (SupportedFeatures) supportedFeatures;
      return Workspace.UpdateWorkspace(controlRequestContext, oldWorkspaceName, ownerName, newWorkspace, clientFeatures);
    }

    public Workspace QueryWorkspace(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName)
    {
      return this.QueryWorkspace(requestContext, workspaceName, ownerName, true, false, false);
    }

    public Workspace QueryWorkspace(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      bool useCache)
    {
      return this.QueryWorkspace(requestContext, workspaceName, ownerName, useCache, false, false);
    }

    public Workspace QueryWorkspace(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      bool useCache,
      bool hideLocalWorkspaces,
      bool hideWorkspaceWithOptions,
      bool throwOnUnrecognizedVSID = true)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      return Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName, useCache, hideLocalWorkspaces, hideWorkspaceWithOptions, throwOnUnrecognizedVSID);
    }

    public List<Workspace> QueryWorkspaces(
      IVssRequestContext requestContext,
      string ownerName,
      string computer,
      int permissionsFilter)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkIdentity(ref ownerName, nameof (ownerName), true);
      validation.checkComputerName(computer, nameof (computer), true);
      validation.emptyToNull(ref computer);
      validation.emptyToNull(ref ownerName);
      return Workspace.QueryWorkspaces(controlRequestContext, ownerName, computer, permissionsFilter);
    }

    public Guid QueryPendingChangeSignature(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      return Workspace.QueryPendingChangeSignature(controlRequestContext, workspaceName, ownerName);
    }

    public void PromotePendingWorkspaceMappings(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      int projectNotificationId)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName);
      if (!workspace.IsLocal)
        throw new LocalWorkspaceRequiredException(workspaceName, ownerName);
      if (!this.SecurityWrapper.HasWorkspacePermission(controlRequestContext, 2, workspace) && !this.SecurityWrapper.HasWorkspacePermission(controlRequestContext, 8, workspace))
        this.SecurityWrapper.CheckWorkspacePermission(controlRequestContext, 2, workspace);
      using (VersionedItemComponent versionedItemComponent = controlRequestContext.VersionControlService.GetVersionedItemComponent(controlRequestContext))
        versionedItemComponent.PromotePendingWorkspaceMappings(workspace, projectNotificationId);
    }

    public TeamFoundationDataReader ReconcileLocalWorkspace(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      Guid pendingChangeSignature,
      LocalPendingChange[] pendingChanges,
      ServerItemLocalVersionUpdate[] localVersionUpdates,
      bool clearLocalVersionTable)
    {
      return this.ReconcileLocalWorkspace(requestContext, workspaceName, ownerName, pendingChangeSignature, pendingChanges, localVersionUpdates, clearLocalVersionTable, false, PathLength.Length259);
    }

    public TeamFoundationDataReader ReconcileLocalWorkspace(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      Guid pendingChangeSignature,
      LocalPendingChange[] pendingChanges,
      ServerItemLocalVersionUpdate[] localVersionUpdates,
      bool clearLocalVersionTable,
      bool throwOnProjectRenamed,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      Workspace workspace = !Guid.TryParse(ownerName, out Guid _) ? Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName) : Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName, true, false, false, false);
      if (!workspace.IsLocal)
        throw new LocalWorkspaceRequiredException(workspaceName, ownerName);
      CommandReconcile disposableObject = (CommandReconcile) null;
      try
      {
        disposableObject = new CommandReconcile(controlRequestContext);
        disposableObject.Execute(workspace, pendingChangeSignature, pendingChanges, localVersionUpdates, clearLocalVersionTable, throwOnProjectRenamed);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Result
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public List<Failure> CheckPendingChanges(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      string[] serverItems)
    {
      return this.CheckPendingChanges(requestContext, workspaceName, ownerName, serverItems, PathLength.Length259);
    }

    public List<Failure> CheckPendingChanges(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      string[] serverItems,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      validation.checkServerItems(serverItems, nameof (serverItems), false, false, false, true, false, false, controlRequestContext.MaxSupportedServerPathLength);
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName);
      List<Failure> failures = new List<Failure>();
      PendingChange.CheckPendingChanges(controlRequestContext, workspace, serverItems, failures);
      return failures;
    }

    public TeamFoundationDataReader PendChanges(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      ChangeRequest[] changes,
      int pendChangesOptions,
      int supportedFeatures,
      string[] itemAttributeFilters,
      bool allowLocalWorkspace = false)
    {
      return this.PendChanges(requestContext, workspaceName, ownerName, changes, pendChangesOptions, supportedFeatures, (string[]) null, itemAttributeFilters, allowLocalWorkspace);
    }

    public TeamFoundationDataReader PendChanges(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      ChangeRequest[] changes,
      int pendChangesOptions,
      int supportedFeatures,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      bool allowLocalWorkspace = false)
    {
      return this.PendChanges(requestContext, workspaceName, ownerName, changes, pendChangesOptions, supportedFeatures, itemPropertyFilters, itemAttributeFilters, allowLocalWorkspace, PathLength.Length259);
    }

    public TeamFoundationDataReader PendChanges(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      ChangeRequest[] changes,
      int pendChangesOptions,
      int supportedFeatures,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      bool allowLocalWorkspace,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      PendChangesOptions options = (PendChangesOptions) pendChangesOptions;
      SupportedFeatures supportedFeatures1 = (SupportedFeatures) supportedFeatures;
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      validation.check((IValidatable[]) changes, nameof (changes), false);
      Validation.checkPendChangesOptions(options, supportedFeatures1, nameof (pendChangesOptions));
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName);
      if (!allowLocalWorkspace && workspace.IsLocal)
        throw new NotPermittedForLocalWorkspaceException(workspace.Name, workspace.OwnerDisplayName);
      CommandPendChanges disposableObject = (CommandPendChanges) null;
      try
      {
        disposableObject = new CommandPendChanges(controlRequestContext);
        disposableObject.Execute(workspace, changes, options, supportedFeatures1, itemPropertyFilters, itemAttributeFilters);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[3]
        {
          (object) disposableObject.Changes,
          (object) disposableObject.Failures,
          (object) disposableObject.Flags
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryPendingSets(
      IVssRequestContext requestContext,
      string localWorkspaceName,
      string localWorkspaceOwner,
      string queryWorkspaceName,
      string ownerName,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      bool maskLocalWorkspaces = true,
      string[] itemPropertyFilters = null,
      bool includePendingChangeSignature = false)
    {
      return this.QueryPendingSets(requestContext, localWorkspaceName, localWorkspaceOwner, queryWorkspaceName, ownerName, itemSpecs, generateDownloadUrls, PathLength.Length259, maskLocalWorkspaces, itemPropertyFilters, includePendingChangeSignature);
    }

    public TeamFoundationDataReader QueryPendingSets(
      IVssRequestContext requestContext,
      string localWorkspaceName,
      string localWorkspaceOwner,
      string queryWorkspaceName,
      string ownerName,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      PathLength maxClientPathLength,
      bool maskLocalWorkspaces = true,
      string[] itemPropertyFilters = null,
      bool includePendingChangeSignature = false)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(localWorkspaceName, nameof (localWorkspaceName), true);
      validation.checkIdentity(ref localWorkspaceOwner, nameof (localWorkspaceOwner), true);
      validation.checkWorkspaceName(queryWorkspaceName, nameof (queryWorkspaceName), true);
      validation.checkIdentity(ref ownerName, nameof (ownerName), true);
      validation.emptyToRoot(ref itemSpecs);
      validation.check((IValidatable[]) itemSpecs, nameof (itemSpecs), false);
      Workspace localWorkspace = Workspace.QueryWorkspace(controlRequestContext, localWorkspaceName, localWorkspaceOwner);
      CommandQueryPendingSets disposableObject = (CommandQueryPendingSets) null;
      try
      {
        disposableObject = new CommandQueryPendingSets(controlRequestContext);
        disposableObject.Execute(localWorkspace, ownerName, itemSpecs, queryWorkspaceName, PendingSetType.Workspace, generateDownloadUrls, int.MaxValue, (string) null, false, maskLocalWorkspaces, itemPropertyFilters, includePendingChangeSignature);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[2]
        {
          (object) disposableObject.PendingSets,
          (object) disposableObject.Failures
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public PendingChange[] QueryPendingChangesById(
      IVssRequestContext requestContext,
      int[] pendingChangeIds,
      bool generateDownloadUrls)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      int[] pendingChangeIds1 = pendingChangeIds != null && pendingChangeIds.Length != 0 ? pendingChangeIds : throw new ArgumentNullException(nameof (pendingChangeIds));
      int num = generateDownloadUrls ? 1 : 0;
      return PendingChange.QueryPendingChangesById(controlRequestContext, pendingChangeIds1, num != 0);
    }

    public TeamFoundationDataReader QueryPendingChangesForWorkspace(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      int pageSize,
      string lastChange,
      bool includeMergeInfo,
      bool maskLocalWorkspaces = false,
      string[] itemPropertyFilters = null)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), false);
      validation.emptyToRoot(ref itemSpecs);
      validation.check((IValidatable[]) itemSpecs, nameof (itemSpecs), false);
      validation.checkServerItem(ref lastChange, nameof (lastChange), true, false, true, true, controlRequestContext.MaxSupportedServerPathLength);
      Workspace localWorkspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      CommandQueryPendingSets disposableObject = (CommandQueryPendingSets) null;
      try
      {
        disposableObject = new CommandQueryPendingSets(controlRequestContext);
        disposableObject.Execute(localWorkspace, workspaceOwner, itemSpecs, workspaceName, PendingSetType.Workspace, generateDownloadUrls, pageSize, lastChange, includeMergeInfo, maskLocalWorkspaces, itemPropertyFilters, false);
        StreamingCollection<PendingChange> streamingCollection1;
        if (!disposableObject.PendingSets.MoveNext())
          streamingCollection1 = new StreamingCollection<PendingChange>((Command) disposableObject)
          {
            HandleExceptions = false
          };
        else
          streamingCollection1 = disposableObject.PendingSets.Current.PendingChanges;
        StreamingCollection<PendingChange> streamingCollection2 = streamingCollection1;
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[2]
        {
          (object) streamingCollection2,
          (object) disposableObject.Failures
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader UndoPendingChanges(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      ItemSpec[] items,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      bool localWorkspaceBehavior = true)
    {
      return this.UndoPendingChanges(requestContext, workspaceName, ownerName, items, itemPropertyFilters, itemAttributeFilters, localWorkspaceBehavior, PathLength.Length259);
    }

    public TeamFoundationDataReader UndoPendingChanges(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      ItemSpec[] items,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      bool localWorkspaceBehavior,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      validation.emptyToRoot(ref items);
      validation.check((IValidatable[]) items, nameof (items), false);
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName);
      if (!localWorkspaceBehavior && !workspace.IsLocal)
        throw new LocalWorkspaceRequiredException(workspace.Name, workspace.OwnerDisplayName);
      CommandUndoPendingChanges disposableObject = (CommandUndoPendingChanges) null;
      try
      {
        disposableObject = new CommandUndoPendingChanges(controlRequestContext);
        disposableObject.Execute(workspace, items, itemPropertyFilters, itemAttributeFilters, localWorkspaceBehavior);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[3]
        {
          (object) disposableObject.UndoneChanges,
          (object) disposableObject.Failures,
          (object) disposableObject.Flags
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public AdminRepositoryInfo QueryRepositoryInformation(IVssRequestContext requestContext)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      this.SecurityWrapper.CheckGlobalPermission(controlRequestContext, GlobalPermissions.AdminConfiguration);
      using (AdministrationComponent administrationComponent = this.GetAdministrationComponent(controlRequestContext))
        return administrationComponent.QueryRepositoryInformation();
    }

    public void OptimizeDatabase(IVssRequestContext requestContext)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      this.SecurityWrapper.CheckGlobalPermission(controlRequestContext, GlobalPermissions.AdminConfiguration);
      using (AdministrationComponent administrationComponent = this.GetAdministrationComponent(controlRequestContext))
        administrationComponent.OptimizeDatabase();
    }

    public void GenerateRepositoryKey(IVssRequestContext requestContext)
    {
      this.SecurityWrapper.CheckGlobalPermission(this.GetVersionControlRequestContext(requestContext), GlobalPermissions.AdminConfiguration);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<ITeamFoundationSigningService>().RegenerateKey(vssRequestContext, ProxyConstants.ProxySigningKey);
    }

    public TeamFoundationDataReader QueryChangeset(
      IVssRequestContext requestContext,
      int changesetId,
      bool includeChanges,
      bool generateDownloadUrls,
      bool includeSourceRenames)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryChangeset disposableObject = (CommandQueryChangeset) null;
      try
      {
        disposableObject = new CommandQueryChangeset(controlRequestContext);
        disposableObject.Execute(changesetId, includeChanges, generateDownloadUrls, int.MaxValue, (string[]) null, (string[]) null, (string[]) null, VersionedItemPermissions.Read, includeSourceRenames);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Changeset
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryChangesetExtended(
      IVssRequestContext requestContext,
      int changesetId,
      bool includeChanges,
      bool generateDownloadUrls,
      string[] changesetPropertyFilters)
    {
      return this.QueryChangesetExtended(requestContext, changesetId, includeChanges, generateDownloadUrls, changesetPropertyFilters, (string[]) null, (string[]) null);
    }

    public TeamFoundationDataReader QueryChangesetExtended(
      IVssRequestContext requestContext,
      int changesetId,
      bool includeChanges,
      bool generateDownloadUrls,
      string[] changesetPropertyFilters,
      string[] itemAttributeFilters,
      string[] itemPropertyFilters)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      CommandQueryChangeset disposableObject = (CommandQueryChangeset) null;
      try
      {
        disposableObject = new CommandQueryChangeset(controlRequestContext);
        disposableObject.Execute(changesetId, includeChanges, generateDownloadUrls, int.MaxValue, changesetPropertyFilters, itemAttributeFilters, itemPropertyFilters, VersionedItemPermissions.Read, true);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Changeset
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryChangesForChangeset(
      IVssRequestContext requestContext,
      int changesetId,
      bool generateDownloadUrls,
      int pageSize,
      ItemSpec lastItem,
      string[] itemAttributeFilters,
      bool includeMergeSourceInfo)
    {
      return this.QueryChangesForChangeset(requestContext, changesetId, generateDownloadUrls, pageSize, lastItem, (string[]) null, itemAttributeFilters, includeMergeSourceInfo);
    }

    public TeamFoundationDataReader QueryChangesForChangeset(
      IVssRequestContext requestContext,
      int changesetId,
      bool generateDownloadUrls,
      int pageSize,
      ItemSpec lastItem,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      bool includeMergeSourceInfo)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryChangesForChangeset disposableObject = (CommandQueryChangesForChangeset) null;
      try
      {
        disposableObject = new CommandQueryChangesForChangeset(controlRequestContext);
        disposableObject.Execute(changesetId, generateDownloadUrls, pageSize, lastItem, itemPropertyFilters, itemAttributeFilters, includeMergeSourceInfo);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Changes
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public IEnumerable<Changeset> QueryChangesets(
      IVssRequestContext requestContext,
      int[] changesets)
    {
      if (requestContext.GetService<TeamFoundationResourceManagementService>().GetServiceVersion(requestContext, "VersionedItem", DatabaseCategories.VersionControl).Version < 10)
        return (IEnumerable<Changeset>) Array.Empty<Changeset>();
      VersionControlRequestContext vcRequestContext = new VersionControlRequestContext(requestContext);
      using (VersionedItemComponent versionedItemComponent = vcRequestContext.VersionControlService.GetVersionedItemComponent(vcRequestContext))
        return (IEnumerable<Changeset>) versionedItemComponent.QueryChangesets(((IEnumerable<int>) changesets).Distinct<int>().ToArray<int>()).GetCurrent<Changeset>().Items;
    }

    public virtual ChangesetChangeTypeSummary QuerySummaryForChangeset(
      IVssRequestContext requestContext,
      int changeset)
    {
      VersionControlRequestContext controlRequestContext = new VersionControlRequestContext(requestContext);
      bool flag1 = false;
      bool flag2 = false;
      if (!controlRequestContext.RequestContext.IsSystemContext)
      {
        using (CommandQueryChangesForChangeset disposableObject = new CommandQueryChangesForChangeset(controlRequestContext.Elevate()))
        {
          disposableObject.Execute(changeset, false, 0, (ItemSpec) null);
          using (TeamFoundationDataReader foundationDataReader = new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
          {
            (object) disposableObject.Changes
          }))
          {
            foreach (Change change in foundationDataReader.Current<StreamingCollection<Change>>())
            {
              flag1 = true;
              if (change.Item.HasPermission(controlRequestContext, VersionedItemPermissions.Read))
              {
                flag2 = true;
                break;
              }
            }
          }
        }
        if (flag1 && !flag2)
          throw new ResourceAccessException(controlRequestContext.RequestContext.GetUserId().ToString(), "Read", Resources.Format("AtLeastOneItemInChangeset", (object) changeset));
      }
      using (VersionedItemComponent versionedItemComponent = controlRequestContext.VersionControlService.GetVersionedItemComponent(controlRequestContext))
        return versionedItemComponent.QuerySummaryForChangeset(changeset);
    }

    internal TeamFoundationDataReader QueryChangeSetOwners(
      IVssRequestContext requestContext,
      bool includeCounts)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryChangeSetOwners disposableObject = (CommandQueryChangeSetOwners) null;
      try
      {
        disposableObject = new CommandQueryChangeSetOwners(controlRequestContext);
        disposableObject.Execute(includeCounts);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Owners
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public int GetLatestChangeset(IVssRequestContext requestContext) => this.GetLatestChangeset(new VersionControlRequestContext(requestContext, this));

    internal int GetLatestChangeset(VersionControlRequestContext vcRequestContext)
    {
      if (vcRequestContext.LatestChangeset == 0)
      {
        using (VersionedItemComponent versionedItemComponent = vcRequestContext.VersionControlService.GetVersionedItemComponent(vcRequestContext))
          vcRequestContext.LatestChangeset = versionedItemComponent.FindLatestChangeset();
      }
      return vcRequestContext.LatestChangeset;
    }

    public List<Changeset> QueryChangesetRange(
      IVssRequestContext requestContext,
      IEnumerable<TfvcMappingFilter> mappings,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      int maxCount,
      bool includeFiles)
    {
      VersionControlRequestContext vcRequestContext = new VersionControlRequestContext(requestContext, this);
      if (versionFrom == null)
        versionFrom = (VersionSpec) new ChangesetVersionSpec(1);
      if (versionTo == null)
        versionTo = (VersionSpec) new LatestVersionSpec();
      if (mappings == null)
        throw new ArgumentNullException(nameof (mappings));
      List<Mapping> mappingList = new List<Mapping>();
      foreach (TfvcMappingFilter mapping in mappings)
      {
        if (mapping.ServerPath == null)
          throw new ArgumentNullException("ServerPath");
        RecursionType depth = RecursionType.Full;
        string serverItem = mapping.ServerPath;
        WorkingFolderType type = WorkingFolderType.Map;
        string str = "/*";
        if (serverItem.EndsWith(str, StringComparison.OrdinalIgnoreCase))
        {
          depth = RecursionType.OneLevel;
          serverItem = !serverItem.Equals("$/*", StringComparison.OrdinalIgnoreCase) ? serverItem.Substring(0, serverItem.Length - str.Length) : "$/";
        }
        if (mapping.Exclude)
        {
          depth = RecursionType.Full;
          type = WorkingFolderType.Cloak;
        }
        mappingList.Add(new Mapping(serverItem, type, (int) depth));
      }
      using (VersionedItemComponent versionedItemComponent = vcRequestContext.VersionControlService.GetVersionedItemComponent(vcRequestContext))
        return versionedItemComponent.QueryChangesetRange((IEnumerable<Mapping>) mappingList, versionFrom, versionTo, includeFiles, maxCount);
    }

    public DateTime GetEarliestChangesetTime(IVssRequestContext requestContext)
    {
      VersionControlRequestContext vcRequestContext = new VersionControlRequestContext(requestContext.Elevate());
      if (this.m_earliestChangesetDate == DateTime.MinValue)
      {
        lock (this.m_initLock)
        {
          if (this.m_earliestChangesetDate == DateTime.MinValue)
          {
            using (VersionedItemComponent versionedItemComponent = vcRequestContext.VersionControlService.GetVersionedItemComponent(vcRequestContext))
              this.m_earliestChangesetDate = Changeset.LoadChangeSet(versionedItemComponent.QueryChangeset(1, false, true), 1).CreationDate;
          }
        }
      }
      return this.m_earliestChangesetDate;
    }

    public void UpdateChangeset(
      IVssRequestContext requestContext,
      int changeset,
      string comment,
      CheckinNote checkinNote)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      controlRequestContext.Validation.checkChangesetComment(comment, nameof (comment), true);
      Changeset.UpdateChangeset(controlRequestContext, changeset, comment, checkinNote);
    }

    public void AddProxy(IVssRequestContext requestContext, ProxyInfo proxy)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      controlRequestContext.Validation.check((IValidatable) proxy, nameof (proxy), false);
      ProxyInfo.AddProxy(controlRequestContext, proxy);
    }

    public ProxyInfo[] QueryProxies(IVssRequestContext requestContext, string[] proxyUrls)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      if (proxyUrls != null && proxyUrls.Length != 0)
      {
        for (int index = 0; index < proxyUrls.Length; ++index)
          validation.checkProxyUrl(proxyUrls[index], "proxyUrls[" + index.ToString() + "]", false);
      }
      return ProxyInfo.QueryProxies(controlRequestContext, proxyUrls);
    }

    public void DeleteProxy(IVssRequestContext requestContext, string proxyUrl)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      controlRequestContext.Validation.checkUrl(proxyUrl, nameof (proxyUrl), false);
      ProxyInfo.DeleteProxy(controlRequestContext, proxyUrl);
    }

    public TeamFoundationDataReader Rollback(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      VersionSpec itemVersion,
      VersionSpec from,
      VersionSpec to,
      int rollbackOptions,
      LockLevel lockLevel,
      string[] itemAttributeFilters)
    {
      return this.Rollback(requestContext, workspaceName, workspaceOwner, items, itemVersion, from, to, rollbackOptions, lockLevel, (string[]) null, itemAttributeFilters);
    }

    public TeamFoundationDataReader Rollback(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      VersionSpec itemVersion,
      VersionSpec from,
      VersionSpec to,
      int rollbackOptions,
      LockLevel lockLevel,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters)
    {
      return this.Rollback(requestContext, workspaceName, workspaceOwner, items, itemVersion, from, to, rollbackOptions, lockLevel, itemPropertyFilters, itemAttributeFilters, PathLength.Length259);
    }

    public TeamFoundationDataReader Rollback(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      VersionSpec itemVersion,
      VersionSpec from,
      VersionSpec to,
      int rollbackOptions,
      LockLevel lockLevel,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      RollbackOptions options = (RollbackOptions) rollbackOptions;
      CommandRollback disposableObject = (CommandRollback) null;
      try
      {
        disposableObject = new CommandRollback(controlRequestContext);
        disposableObject.Execute(workspaceName, workspaceOwner, items, itemVersion, from, to, options, lockLevel, itemPropertyFilters, itemAttributeFilters);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[4]
        {
          (object) disposableObject.Operations,
          (object) disposableObject.Conflicts,
          (object) disposableObject.Failures,
          (object) disposableObject.Flags
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader CheckIn(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      string[] serverItems,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      int checkinOptions,
      bool deferCheckIn,
      int checkInTicket)
    {
      return this.CheckIn(requestContext, workspaceName, ownerName, serverItems, info, checkinNotificationInfo, checkinOptions, deferCheckIn, checkInTicket, true);
    }

    internal TeamFoundationDataReader CheckIn(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      string[] serverItems,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      int checkinOptions,
      bool deferCheckIn,
      int checkInTicket,
      bool returnFailures)
    {
      return this.CheckIn(requestContext, workspaceName, ownerName, serverItems, info, checkinNotificationInfo, checkinOptions, deferCheckIn, checkInTicket, returnFailures, PathLength.Length259);
    }

    internal TeamFoundationDataReader CheckIn(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      string[] serverItems,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      int checkinOptions,
      bool deferCheckIn,
      int checkInTicket,
      bool returnFailures,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      CheckInOptions2 checkinOptions1 = (CheckInOptions2) checkinOptions;
      CommandCheckInWorkspace disposableObject = (CommandCheckInWorkspace) null;
      try
      {
        disposableObject = new CommandCheckInWorkspace(controlRequestContext);
        CheckinResult checkinResult = disposableObject.Execute(workspaceName, ownerName, serverItems, checkinNotificationInfo, checkinOptions1, info, deferCheckIn, checkInTicket);
        return returnFailures ? new TeamFoundationDataReader((IDisposable) disposableObject, new object[3]
        {
          (object) checkinResult,
          (object) disposableObject.Conflicts,
          (object) disposableObject.Failures
        }) : new TeamFoundationDataReader((IDisposable) disposableObject, new object[2]
        {
          (object) checkinResult,
          (object) disposableObject.Conflicts
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader CheckInShelveset(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName,
      string changesetOwner,
      CheckinNotificationInfo checkinNotificationInfo,
      int checkinOptions)
    {
      return this.CheckInShelveset(requestContext, shelvesetName, ownerName, 0, changesetOwner, checkinNotificationInfo, checkinOptions);
    }

    internal TeamFoundationDataReader CheckInShelveset(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName,
      int shelvesetVersion,
      string changesetOwner,
      CheckinNotificationInfo checkinNotificationInfo,
      int checkinOptions)
    {
      return this.CheckInShelveset(requestContext, shelvesetName, ownerName, shelvesetVersion, changesetOwner, checkinNotificationInfo, checkinOptions, true);
    }

    internal TeamFoundationDataReader CheckInShelveset(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName,
      int shelvesetVersion,
      string changesetOwner,
      CheckinNotificationInfo checkinNotificationInfo,
      int checkinOptions,
      bool returnFailures)
    {
      return this.CheckInShelveset(requestContext, shelvesetName, ownerName, shelvesetVersion, changesetOwner, checkinNotificationInfo, checkinOptions, returnFailures, PathLength.Length259);
    }

    internal TeamFoundationDataReader CheckInShelveset(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName,
      int shelvesetVersion,
      string changesetOwner,
      CheckinNotificationInfo checkinNotificationInfo,
      int checkinOptions,
      bool returnFailures,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      CheckInOptions2 checkinOptions1 = (CheckInOptions2) checkinOptions;
      CommandCheckInShelveset disposableObject = (CommandCheckInShelveset) null;
      try
      {
        disposableObject = new CommandCheckInShelveset(controlRequestContext);
        CheckinResult checkinResult = disposableObject.Execute(shelvesetName, ownerName, shelvesetVersion, changesetOwner, checkinNotificationInfo, checkinOptions1);
        return returnFailures ? new TeamFoundationDataReader((IDisposable) disposableObject, new object[3]
        {
          (object) checkinResult,
          (object) disposableObject.Conflicts,
          (object) disposableObject.Failures
        }) : new TeamFoundationDataReader((IDisposable) disposableObject, new object[2]
        {
          (object) checkinResult,
          (object) disposableObject.Conflicts
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public List<Failure> Shelve(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      string[] serverItems,
      Shelveset shelveset,
      bool replace)
    {
      return this.Shelve(requestContext, workspaceName, workspaceOwner, serverItems, shelveset, replace, PathLength.Length259);
    }

    public List<Failure> Shelve(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      string[] serverItems,
      Shelveset shelveset,
      bool replace,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkServerItems(serverItems, nameof (serverItems), true, false, false, true, false, false, controlRequestContext.MaxSupportedServerPathLength);
      validation.check((IValidatable) shelveset, nameof (shelveset), false);
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      List<Failure> failures = new List<Failure>();
      shelveset.Shelve(controlRequestContext, workspace, (IEnumerable<string>) serverItems, replace, failures);
      return failures;
    }

    public void DeleteShelveset(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName)
    {
      this.DeleteShelveset(requestContext, shelvesetName, ownerName, -1);
    }

    internal void DeleteShelveset(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName,
      int shelvesetVersion)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkShelvesetName(shelvesetName, nameof (shelvesetName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      ArgumentUtility.CheckForOutOfRange(shelvesetVersion, nameof (shelvesetVersion), -1);
      Shelveset.Delete(controlRequestContext, shelvesetName, ownerName, shelvesetVersion);
    }

    public void UpdateShelvesetCreationTime(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName,
      DateTime? cutOffDate)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkShelvesetName(shelvesetName, nameof (shelvesetName), true);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      using (VersionedItemComponent versionedItemComponent = controlRequestContext.VersionControlService.GetVersionedItemComponent(controlRequestContext))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = TfvcIdentityHelper.FindIdentity(controlRequestContext.RequestContext, ownerName);
        if (!controlRequestContext.RequestContext.IsSystemContext)
        {
          IdentityDescriptor userContext = controlRequestContext.RequestContext.UserContext;
          if (!IdentityDescriptorComparer.Instance.Equals(identity.Descriptor, userContext))
            controlRequestContext.VersionControlService.SecurityWrapper.CheckGlobalPermission(controlRequestContext, GlobalPermissions.AdminShelvesets);
        }
        versionedItemComponent.UpdateShelvesetCreationTime(shelvesetName, identity.Id, cutOffDate);
      }
    }

    public TeamFoundationDataReader Unshelve(
      IVssRequestContext requestContext,
      string shelvesetName,
      string shelvesetOwner,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      string[] itemAttributeFilters)
    {
      return this.Unshelve(requestContext, shelvesetName, shelvesetOwner, workspaceName, workspaceOwner, items, (string[]) null, itemAttributeFilters, (string[]) null, false);
    }

    public TeamFoundationDataReader Unshelve(
      IVssRequestContext requestContext,
      string shelvesetName,
      string shelvesetOwner,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      string[] shelvesetPropertyNameFilters,
      bool merge)
    {
      return this.Unshelve(requestContext, shelvesetName, shelvesetOwner, workspaceName, workspaceOwner, items, itemPropertyFilters, itemAttributeFilters, shelvesetPropertyNameFilters, merge, PathLength.Length259);
    }

    public TeamFoundationDataReader Unshelve(
      IVssRequestContext requestContext,
      string shelvesetName,
      string shelvesetOwner,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      string[] shelvesetPropertyNameFilters,
      bool merge,
      PathLength maxClientPathLength)
    {
      return this.Unshelve(requestContext, shelvesetName, shelvesetOwner, 0, workspaceName, workspaceOwner, items, itemPropertyFilters, itemAttributeFilters, shelvesetPropertyNameFilters, merge, maxClientPathLength);
    }

    internal TeamFoundationDataReader Unshelve(
      IVssRequestContext requestContext,
      string shelvesetName,
      string shelvesetOwner,
      int shelvesetVersion,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      string[] shelvesetPropertyNameFilters,
      bool merge,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkShelvesetName(shelvesetName, nameof (shelvesetName), false);
      validation.checkIdentity(ref shelvesetOwner, nameof (shelvesetOwner), false);
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), false);
      validation.check((IValidatable[]) items, nameof (items), true);
      ArgumentUtility.CheckForOutOfRange(shelvesetVersion, nameof (shelvesetVersion), -1);
      CommandUnshelve disposableObject = (CommandUnshelve) null;
      try
      {
        disposableObject = new CommandUnshelve(controlRequestContext);
        disposableObject.Execute(shelvesetName, shelvesetOwner, shelvesetVersion, workspaceName, workspaceOwner, items, itemAttributeFilters, itemPropertyFilters, shelvesetPropertyNameFilters, merge);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[5]
        {
          (object) disposableObject.Shelveset,
          (object) disposableObject.Failures,
          (object) disposableObject.GetOperations,
          (object) disposableObject.Conflicts,
          (object) disposableObject.Flags
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public void UpdateShelveset(
      IVssRequestContext requestContext,
      string shelvesetName,
      string shelvesetOwner,
      Shelveset updatedShelveset)
    {
      Validation validation = this.GetVersionControlRequestContext(requestContext).Validation;
      validation.checkShelvesetName(shelvesetName, nameof (shelvesetName), false);
      validation.checkIdentity(ref shelvesetOwner, nameof (shelvesetOwner), false);
      validation.checkNotNull((object) updatedShelveset, nameof (updatedShelveset));
      List<Shelveset> shelvesetList = this.QueryShelvesets(requestContext, shelvesetName, shelvesetOwner, 0);
      Shelveset shelveset = shelvesetList.Count >= 1 ? shelvesetList[0] : throw new ShelvesetNotFoundException(shelvesetName, shelvesetOwner);
      if (!string.Equals(shelveset.Comment, updatedShelveset.Comment, StringComparison.Ordinal) || !string.Equals(shelveset.Name, updatedShelveset.Name, StringComparison.Ordinal) || !VssStringComparer.UserName.Equals(shelveset.Owner, updatedShelveset.Owner) || !string.Equals(shelveset.PolicyOverrideComment, updatedShelveset.PolicyOverrideComment, StringComparison.Ordinal) || !VersionControlLink.AreEqual(shelveset.Links, updatedShelveset.Links) || shelveset.CheckinNote == null && updatedShelveset.CheckinNote != null || !shelveset.CheckinNote.AreMemberWiseEqual((object) updatedShelveset.CheckinNote))
        throw new NotSupportedException(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Get("ShelvesetUpdateOperationsNotSuppoerted"));
      requestContext.GetService<TeamFoundationPropertyService>().SetProperties(requestContext, shelveset.GetArtifactSpec(VersionControlPropertyKinds.Shelveset), (IEnumerable<PropertyValue>) updatedShelveset.Properties);
    }

    public TeamFoundationDataReader QueryShelvedChanges(
      IVssRequestContext requestContext,
      string localWorkspaceName,
      string localWorkspaceOwner,
      string shelvesetName,
      string ownerName,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls)
    {
      return this.QueryShelvedChanges(requestContext, localWorkspaceName, localWorkspaceOwner, shelvesetName, ownerName, itemSpecs, generateDownloadUrls, (string[]) null);
    }

    public TeamFoundationDataReader QueryShelvedChanges(
      IVssRequestContext requestContext,
      string localWorkspaceName,
      string localWorkspaceOwner,
      string shelvesetName,
      string ownerName,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      string[] itemPropertyFilters)
    {
      return this.QueryShelvedChanges(requestContext, localWorkspaceName, localWorkspaceOwner, shelvesetName, ownerName, 0, itemSpecs, generateDownloadUrls, itemPropertyFilters);
    }

    internal TeamFoundationDataReader QueryShelvedChanges(
      IVssRequestContext requestContext,
      string localWorkspaceName,
      string localWorkspaceOwner,
      string shelvesetName,
      string ownerName,
      int shelvesetVersion,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      string[] itemPropertyFilters)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(localWorkspaceName, nameof (localWorkspaceName), true);
      validation.checkIdentity(ref localWorkspaceOwner, nameof (localWorkspaceOwner), true);
      validation.checkShelvesetName(shelvesetName, nameof (shelvesetName), true);
      validation.checkIdentity(ref ownerName, nameof (ownerName), true);
      validation.emptyToRoot(ref itemSpecs);
      validation.check((IValidatable[]) itemSpecs, nameof (itemSpecs), false);
      ArgumentUtility.CheckForOutOfRange(shelvesetVersion, nameof (shelvesetVersion), -1);
      Workspace localWorkspace = Workspace.QueryWorkspace(controlRequestContext, localWorkspaceName, localWorkspaceOwner);
      CommandQueryPendingSets disposableObject = (CommandQueryPendingSets) null;
      try
      {
        disposableObject = new CommandQueryPendingSets(controlRequestContext);
        disposableObject.Execute(localWorkspace, ownerName, itemSpecs, shelvesetName, shelvesetVersion, PendingSetType.Shelveset, generateDownloadUrls, int.MaxValue, (string) null, false, false, itemPropertyFilters, false);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[2]
        {
          (object) disposableObject.PendingSets,
          (object) disposableObject.Failures
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public List<Shelveset> QueryShelvesets(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName)
    {
      return this.QueryShelvesets(requestContext, shelvesetName, ownerName, 0);
    }

    public TeamFoundationDataReader QueryShelvesets(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName,
      string[] propertyNameFilters)
    {
      return this.QueryShelvesets(requestContext, shelvesetName, ownerName, 0, propertyNameFilters);
    }

    internal List<Shelveset> QueryShelvesets(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName,
      int shelvesetVersion)
    {
      using (TeamFoundationDataReader foundationDataReader = this.QueryShelvesets(requestContext, shelvesetName, ownerName, shelvesetVersion, (string[]) null))
        return foundationDataReader.Current<List<Shelveset>>();
    }

    internal TeamFoundationDataReader QueryShelvesets(
      IVssRequestContext requestContext,
      string shelvesetName,
      string ownerName,
      int shelvesetVersion,
      string[] propertyNameFilters)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      if (requestContext.IsFeatureEnabled("SourceControl.IgnoreInvalidShelvesetName") && ownerName == null && requestContext.UserAgent.Contains("devenv.exe") && !WorkspaceSpec.IsLegalName(shelvesetName))
        return new TeamFoundationDataReader(new object[1]
        {
          (object) new List<Shelveset>()
        });
      Validation validation = controlRequestContext.Validation;
      validation.checkShelvesetName(shelvesetName, nameof (shelvesetName), true);
      validation.checkIdentity(ref ownerName, nameof (ownerName), true);
      CommandQueryShelvesets disposableObject = (CommandQueryShelvesets) null;
      try
      {
        disposableObject = new CommandQueryShelvesets(controlRequestContext);
        disposableObject.Execute(shelvesetName, ownerName, shelvesetVersion, propertyNameFilters);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Shelvesets
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader CreateBranch(
      IVssRequestContext requestContext,
      string sourcePath,
      string targetPath,
      VersionSpec version,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      List<Mapping> mappings)
    {
      return this.CreateBranch(requestContext, sourcePath, targetPath, version, info, checkinNotificationInfo, mappings, true);
    }

    internal TeamFoundationDataReader CreateBranch(
      IVssRequestContext requestContext,
      string sourcePath,
      string targetPath,
      VersionSpec version,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      List<Mapping> mappings,
      bool returnFailures)
    {
      return this.CreateBranch(requestContext, sourcePath, targetPath, version, info, checkinNotificationInfo, mappings, returnFailures, PathLength.Length259);
    }

    internal TeamFoundationDataReader CreateBranch(
      IVssRequestContext requestContext,
      string sourcePath,
      string targetPath,
      VersionSpec version,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      List<Mapping> mappings,
      bool returnFailures,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      CommandBranch disposableObject = (CommandBranch) null;
      try
      {
        disposableObject = new CommandBranch(controlRequestContext);
        disposableObject.Execute(sourcePath, targetPath, version, info, checkinNotificationInfo, mappings, returnFailures);
        return returnFailures ? new TeamFoundationDataReader((IDisposable) disposableObject, new object[2]
        {
          (object) disposableObject.Failures,
          (object) disposableObject.Result
        }) : new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Result
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public BranchRelative[][] QueryBranches(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      VersionSpec version)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.check((IValidatable[]) items, nameof (items), false);
      validation.checkVersionSpec(version, nameof (version), true, false);
      return BranchRelative.QueryBranches(controlRequestContext, Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner), items, version);
    }

    public void DeleteBranchObject(IVssRequestContext requestContext, ItemIdentifier rootItem) => this.DeleteBranchObject(requestContext, rootItem, PathLength.Length259);

    public void DeleteBranchObject(
      IVssRequestContext requestContext,
      ItemIdentifier rootItem,
      PathLength maxClientPathLength)
    {
      BranchObject.Delete(this.GetVersionControlRequestContext(requestContext, maxClientPathLength), rootItem);
    }

    public TeamFoundationDataReader QueryBranchObjects(
      IVssRequestContext requestContext,
      ItemIdentifier item,
      RecursionType recursion)
    {
      return this.QueryBranchObjects(requestContext, item, recursion, PathLength.Length259);
    }

    public TeamFoundationDataReader QueryBranchObjects(
      IVssRequestContext requestContext,
      ItemIdentifier item,
      RecursionType recursion,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      CommandQueryBranchObjects disposableObject = (CommandQueryBranchObjects) null;
      try
      {
        disposableObject = (CommandQueryBranchObjects) new CommandQueryBranchObjectsHierarchy(controlRequestContext, item, recursion);
        disposableObject.Execute();
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.BranchObjects
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryBranchObjectsByPath(
      IVssRequestContext requestContext,
      ItemIdentifier item,
      VersionSpec version)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryBranchObjectsByPath disposableObject = (CommandQueryBranchObjectsByPath) null;
      try
      {
        disposableObject = new CommandQueryBranchObjectsByPath(controlRequestContext, item, version);
        disposableObject.Execute();
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.BranchObjects
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryBranchObjectOwnership(
      IVssRequestContext requestContext,
      int[] changesets,
      ItemSpec pathFilter)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryBranchObjectOwnership disposableObject = (CommandQueryBranchObjectOwnership) null;
      try
      {
        disposableObject = new CommandQueryBranchObjectOwnership(controlRequestContext);
        disposableObject.Execute(changesets, pathFilter);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.BranchObjectOwnership
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public void UpdateBranchObject(
      IVssRequestContext requestContext,
      BranchProperties branchProperties,
      bool updateExisting)
    {
      this.UpdateBranchObject(requestContext, branchProperties, updateExisting, PathLength.Length259);
    }

    public void UpdateBranchObject(
      IVssRequestContext requestContext,
      BranchProperties branchProperties,
      bool updateExisting,
      PathLength maxClientPathLength)
    {
      BranchObject.Update(this.GetVersionControlRequestContext(requestContext, maxClientPathLength), branchProperties, updateExisting);
    }

    public TeamFoundationDataReader Merge(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      ItemSpec target,
      VersionSpec from,
      VersionSpec to,
      LockLevel lockLevel,
      MergeOptionsEx mergeOptions,
      string[] itemAttributeFilters)
    {
      return this.Merge(requestContext, workspaceName, workspaceOwner, source, target, from, to, lockLevel, mergeOptions, itemAttributeFilters, (string[]) null);
    }

    public TeamFoundationDataReader Merge(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      ItemSpec target,
      VersionSpec from,
      VersionSpec to,
      LockLevel lockLevel,
      MergeOptionsEx mergeOptions,
      string[] itemAttributeFilters,
      string[] itemPropertyFilters)
    {
      return this.Merge(requestContext, workspaceName, workspaceOwner, source, target, from, to, lockLevel, mergeOptions, itemAttributeFilters, itemPropertyFilters, PathLength.Length259);
    }

    public TeamFoundationDataReader Merge(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      ItemSpec target,
      VersionSpec from,
      VersionSpec to,
      LockLevel lockLevel,
      MergeOptionsEx mergeOptions,
      string[] itemAttributeFilters,
      string[] itemPropertyFilters,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.check((IValidatable) source, nameof (source), false);
      target.SetValidationOptions(this.GetAllow8Dot3Paths(controlRequestContext), true, true);
      validation.check((IValidatable) target, nameof (target), false);
      validation.checkVersionSpec(from, nameof (from), true, true);
      validation.checkVersionSpec(to, nameof (to), true, false);
      if (source.isWildcard)
        throw new WildcardNotAllowedException("WildcardNotAllowedForMergeSource", Array.Empty<object>());
      if (target.isWildcard)
        throw new WildcardNotAllowedException("WildcardNotAllowedForMergeTarget", Array.Empty<object>());
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      CommandMerge disposableObject = (CommandMerge) null;
      try
      {
        disposableObject = new CommandMerge(controlRequestContext);
        disposableObject.Execute(workspace, source, target, from, to, mergeOptions, lockLevel, false, true, itemPropertyFilters, itemAttributeFilters);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[4]
        {
          (object) disposableObject.Operations,
          (object) disposableObject.Failures,
          (object) disposableObject.Conflicts,
          (object) disposableObject.Flags
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public List<MergeCandidate> QueryMergeCandidates(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      ItemSpec target,
      MergeOptionsEx options)
    {
      return this.QueryMergeCandidates(requestContext, workspaceName, workspaceOwner, source, target, options, PathLength.Length259);
    }

    public List<MergeCandidate> QueryMergeCandidates(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      ItemSpec target,
      MergeOptionsEx options,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.check((IValidatable) source, nameof (source), false);
      validation.check((IValidatable) target, nameof (target), false);
      if (source.isWildcard)
        throw new WildcardNotAllowedException("WildcardNotAllowedForMergeSource", Array.Empty<object>());
      if (target.isWildcard)
        throw new WildcardNotAllowedException("WildcardNotAllowedForMergeTarget", Array.Empty<object>());
      return MergeCandidate.QueryMergeCandidates(controlRequestContext, Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner), source, target, options);
    }

    public TeamFoundationDataReader QueryMerges(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      VersionSpec versionSource,
      ItemSpec target,
      VersionSpec versionTarget,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      int maxChangesets,
      bool showAll)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.check((IValidatable) source, nameof (source), true);
      validation.checkVersionSpec(versionSource, nameof (versionSource), source == null, false);
      validation.check((IValidatable) target, nameof (target), true);
      validation.checkVersionSpec(versionTarget, nameof (versionTarget), target == null, false);
      validation.checkVersionSpec(versionFrom, nameof (versionFrom), true, true);
      validation.checkVersionSpec(versionTo, nameof (versionTo), true, false);
      if (source != null && source.isWildcard)
        throw new WildcardNotAllowedException("WildcardNotAllowedForMergeSource", Array.Empty<object>());
      if (target != null && target.isWildcard)
        throw new WildcardNotAllowedException("WildcardNotAllowedForMergeTarget", Array.Empty<object>());
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      CommandQueryMerges disposableObject = (CommandQueryMerges) null;
      try
      {
        disposableObject = new CommandQueryMerges(controlRequestContext);
        disposableObject.Execute(workspace, source, versionSource, target, versionTarget, versionFrom, versionTo, maxChangesets, showAll);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[2]
        {
          (object) disposableObject.MergedChangesets,
          (object) disposableObject.Changesets
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryMergesWithDetails(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      VersionSpec versionSource,
      ItemSpec target,
      VersionSpec versionTarget,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      int maxChangesets,
      bool showAll)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.check((IValidatable) source, nameof (source), true);
      validation.checkVersionSpec(versionSource, nameof (versionSource), source == null, false);
      validation.check((IValidatable) target, nameof (target), true);
      validation.checkVersionSpec(versionTarget, nameof (versionTarget), target == null, false);
      validation.checkVersionSpec(versionFrom, nameof (versionFrom), true, true);
      validation.checkVersionSpec(versionTo, nameof (versionTo), true, false);
      if (source != null && source.isWildcard)
        throw new WildcardNotAllowedException("WildcardNotAllowedForMergeSource", Array.Empty<object>());
      if (target != null && target.isWildcard)
        throw new WildcardNotAllowedException("WildcardNotAllowedForMergeTarget", Array.Empty<object>());
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      CommandQueryMergesWithDetails disposableObject = (CommandQueryMergesWithDetails) null;
      try
      {
        disposableObject = new CommandQueryMergesWithDetails(controlRequestContext);
        disposableObject.Execute(workspace, source, versionSource, target, versionTarget, versionFrom, versionTo, maxChangesets, showAll);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Details
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryMergesExtended(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec target,
      VersionSpec versionTarget,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      QueryMergesExtendedOptions options)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryMergesExtended disposableObject = (CommandQueryMergesExtended) null;
      try
      {
        disposableObject = new CommandQueryMergesExtended(controlRequestContext);
        StreamingCollection<ExtendedMerge> streamingCollection = disposableObject.Execute(workspaceName, workspaceOwner, target, versionTarget, versionFrom, versionTo, options);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) streamingCollection
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader TrackMerges(
      IVssRequestContext requestContext,
      int[] sourceChangesets,
      ItemIdentifier sourceItem,
      List<ItemIdentifier> targetItems,
      ItemSpec pathFilter)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      if (((IEnumerable<int>) sourceChangesets).Distinct<int>().Count<int>() != sourceChangesets.Length)
        throw new ArgumentException(Resources.Get("SourceChangesetsCannotContainDuplicates"));
      foreach (int sourceChangeset in sourceChangesets)
        VersionSpecCommon.ValidateNumber((VersionSpecFactory) VersionSpec.ServerVersionSpecFactory, sourceChangeset);
      CommandTrackMerges disposableObject = (CommandTrackMerges) null;
      try
      {
        disposableObject = new CommandTrackMerges(controlRequestContext);
        disposableObject.Execute(sourceChangesets, sourceItem, targetItems, pathFilter);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[2]
        {
          (object) disposableObject.Merges,
          (object) disposableObject.PartialTargets
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryMergeRelationships(
      IVssRequestContext requestContext,
      string serverItem)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryMergeRelationships disposableObject = (CommandQueryMergeRelationships) null;
      try
      {
        disposableObject = new CommandQueryMergeRelationships(controlRequestContext);
        disposableObject.Execute(serverItem);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.MergeRelationships
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    internal TeamFoundationDataReader QueryDestroyableItems(
      IVssRequestContext requestContext,
      string serverFolder,
      VersionSpec version,
      DeletedState deleted)
    {
      CommandQueryDestroyableItems disposableObject = new CommandQueryDestroyableItems(this.GetVersionControlRequestContext(requestContext));
      disposableObject.Execute(serverFolder, version, deleted);
      return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
      {
        (object) disposableObject.Items
      });
    }

    public TeamFoundationDataReader QueryItems(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      VersionSpec version,
      DeletedState deletedState,
      ItemType itemType,
      bool generateDownloadUrls,
      int options)
    {
      return this.QueryItems(requestContext, workspaceName, workspaceOwner, items, version, deletedState, itemType, generateDownloadUrls, options, (string[]) null, (string[]) null);
    }

    public TeamFoundationDataReader QueryItems(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      VersionSpec version,
      DeletedState deletedState,
      ItemType itemType,
      bool generateDownloadUrls,
      int options,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.emptyToRoot(ref items);
      validation.check((IValidatable[]) items, nameof (items), false);
      validation.checkVersionSpec(version, nameof (version), false);
      validation.checkRequestSize((IValidatable[]) items, nameof (items), this.GetMaxInputsLightRequest(controlRequestContext));
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      CommandQueryItems<ItemSet, Item> disposableObject = (CommandQueryItems<ItemSet, Item>) null;
      try
      {
        disposableObject = new CommandQueryItems<ItemSet, Item>(controlRequestContext);
        disposableObject.Execute(workspace, items, version, deletedState, itemType, generateDownloadUrls, options, VersionedItemPermissions.Read, (ContinueExecutionCompletedCallback) null, itemPropertyFilters, itemAttributeFilters);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.ItemSets
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryItemsPaged(
      IVssRequestContext requestContext,
      ItemSpec scopeItem,
      int changesetId,
      int pageSize,
      ItemSpec continuationItem,
      int options,
      out string lastItemPaged)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryItemsPaged disposableObject = (CommandQueryItemsPaged) null;
      try
      {
        disposableObject = new CommandQueryItemsPaged(controlRequestContext);
        disposableObject.Execute(scopeItem, changesetId, pageSize, continuationItem, options, out lastItemPaged);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Items
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryItemsByChangesetPaged(
      IVssRequestContext requestContext,
      ItemSpec scopeItem,
      int baseChangesetId,
      int targetChangesetId,
      int pageSize,
      ItemSpec continuationItem,
      out string lastItemPaged)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandQueryItemsByChangesetPaged disposableObject = (CommandQueryItemsByChangesetPaged) null;
      try
      {
        disposableObject = new CommandQueryItemsByChangesetPaged(controlRequestContext);
        disposableObject.Execute(scopeItem, baseChangesetId, targetChangesetId, pageSize, continuationItem, out lastItemPaged);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.ByChangesetItems
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TfvcFileStats QueryTfvcFileStats(IVssRequestContext requestContext, ItemSpec scopeItem)
    {
      using (CommandQueryTfvcFileStats queryTfvcFileStats = new CommandQueryTfvcFileStats(this.GetVersionControlRequestContext(requestContext)))
        return queryTfvcFileStats.Execute(scopeItem);
    }

    public TeamFoundationDataReader QueryWorkspaceItems(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      DeletedState deletedState,
      ItemType itemType,
      bool generateDownloadUrls,
      int options)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), false);
      validation.emptyToRoot(ref items);
      validation.check((IValidatable[]) items, nameof (items), false);
      validation.checkRequestSize((IValidatable[]) items, nameof (items), this.GetMaxInputsLightRequest(controlRequestContext));
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      CommandQueryItems<WorkspaceItemSet, WorkspaceItem> disposableObject = (CommandQueryItems<WorkspaceItemSet, WorkspaceItem>) null;
      try
      {
        disposableObject = new CommandQueryItems<WorkspaceItemSet, WorkspaceItem>(controlRequestContext);
        WorkspaceVersionSpec workspaceVersionSpec = new WorkspaceVersionSpec(workspaceName, workspaceOwner, (string) null);
        disposableObject.Execute(workspace, items, (VersionSpec) workspaceVersionSpec, deletedState, itemType, generateDownloadUrls, options);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.ItemSets
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public Item QueryItemById(
      IVssRequestContext requestContext,
      int itemId,
      int changeSet,
      bool generateDownloadUrl)
    {
      return Item.QueryItem(this.GetVersionControlRequestContext(requestContext), itemId, new ChangesetVersionSpec(changeSet), generateDownloadUrl);
    }

    public Item[] QueryItemsById(
      IVssRequestContext requestContext,
      int[] itemIds,
      int changeSet,
      bool generateDownloadUrls,
      int options)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      int[] itemIds1 = itemIds != null && itemIds.Length != 0 ? itemIds : throw new ArgumentNullException(nameof (itemIds));
      ChangesetVersionSpec versionTo = new ChangesetVersionSpec(changeSet);
      int num = generateDownloadUrls ? 1 : 0;
      int options1 = options;
      return Item.QueryItems(controlRequestContext, itemIds1, versionTo, num != 0, options1);
    }

    public TeamFoundationDataReader QueryItemsExtended(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      DeletedState deletedState,
      ItemType itemType,
      int options,
      int timeoutMinutes = 5)
    {
      return this.QueryItemsExtended(requestContext, workspaceName, workspaceOwner, items, deletedState, itemType, options, (string[]) null, timeoutMinutes);
    }

    public TeamFoundationDataReader QueryItemsExtended(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      DeletedState deletedState,
      ItemType itemType,
      int options,
      string[] itemPropertyFilters,
      int timeoutMinutes = 5)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.emptyToRoot(ref items);
      validation.check((IValidatable[]) items, nameof (items), false);
      validation.checkRequestSize((IValidatable[]) items, nameof (items), this.GetMaxInputsLightRequest(controlRequestContext));
      Workspace localWorkspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      CommandQueryItemsExtended disposableObject = (CommandQueryItemsExtended) null;
      try
      {
        disposableObject = new CommandQueryItemsExtended(controlRequestContext);
        disposableObject.Execute(localWorkspace, items, deletedState, itemType, options, itemPropertyFilters, timeoutMinutes);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.ExtendedItems
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public CodeMetrics QueryCodeMetrics(
      IVssRequestContext requestContext,
      Guid projectId,
      int startingTimeBucket,
      int endTimeBucket)
    {
      using (CommandQueryCodeMetrics queryCodeMetrics = new CommandQueryCodeMetrics(new VersionControlRequestContext(requestContext)))
        return queryCodeMetrics.Execute(projectId, startingTimeBucket, endTimeBucket);
    }

    public void DeleteCodeMetrics(IVssRequestContext requestContext, int timePeriodsToRetain)
    {
      using (CommandDeleteCodeMetrics deleteCodeMetrics = new CommandDeleteCodeMetrics(new VersionControlRequestContext(requestContext)))
        deleteCodeMetrics.Execute(timePeriodsToRetain);
    }

    public void UpdateLocalVersion(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      BaseLocalVersionUpdate[] updates)
    {
      this.UpdateLocalVersion(requestContext, workspaceName, ownerName, updates, PathLength.Length259);
    }

    public void UpdateLocalVersion(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      BaseLocalVersionUpdate[] updates,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      BaseLocalVersionUpdate.UpdateLocalVersion(controlRequestContext, Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName), updates);
    }

    public TeamFoundationDataReader QueryLocalVersions(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.check((IValidatable[]) itemSpecs, nameof (itemSpecs), false);
      validation.checkRequestSize((IValidatable[]) itemSpecs, nameof (itemSpecs), this.GetMaxInputsLightRequest(controlRequestContext));
      CommandQueryLocalVersions disposableObject = (CommandQueryLocalVersions) null;
      try
      {
        disposableObject = new CommandQueryLocalVersions(controlRequestContext);
        disposableObject.Execute(workspaceName, workspaceOwner, itemSpecs);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Items
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader Destroy(
      IVssRequestContext requestContext,
      ItemSpec item,
      VersionSpec versionSpec,
      VersionSpec stopAtSpec,
      int flags)
    {
      return this.Destroy(requestContext, item, versionSpec, stopAtSpec, flags, PathLength.Length259);
    }

    public TeamFoundationDataReader Destroy(
      IVssRequestContext requestContext,
      ItemSpec item,
      VersionSpec versionSpec,
      VersionSpec stopAtSpec,
      int flags,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      string str = item.Item;
      ref string local = ref str;
      int serverPathLength = (int) controlRequestContext.MaxSupportedServerPathLength;
      validation.checkItem(ref local, nameof (item), false, false, true, false, (PathLength) serverPathLength);
      if ((flags & 32) != 0)
        throw new ArgumentException(Resources.Get("DeleteWorkspaceStateInvalidForDestroy"), nameof (flags));
      CommandDestroy disposableObject = (CommandDestroy) null;
      try
      {
        disposableObject = new CommandDestroy(controlRequestContext);
        Failure[] failures;
        StreamingCollection<PendingSet> pendingChanges;
        StreamingCollection<PendingSet> shelvedChanges;
        Item[] objArray = disposableObject.Execute(item, versionSpec, stopAtSpec, flags, out failures, out pendingChanges, out shelvedChanges);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[4]
        {
          (object) objArray,
          (object) failures,
          (object) pendingChanges,
          (object) shelvedChanges
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader Get(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      GetRequest[] requests,
      int maxResults,
      GetOptions getOptions,
      string[] itemAttributeFilters)
    {
      return this.Get(requestContext, workspaceName, ownerName, requests, maxResults, getOptions, (string[]) null, itemAttributeFilters);
    }

    public TeamFoundationDataReader Get(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      GetRequest[] requests,
      int maxResults,
      GetOptions getOptions,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters)
    {
      return this.Get(requestContext, workspaceName, ownerName, requests, maxResults, getOptions, itemPropertyFilters, itemAttributeFilters, PathLength.Length259);
    }

    public TeamFoundationDataReader Get(
      IVssRequestContext requestContext,
      string workspaceName,
      string ownerName,
      GetRequest[] requests,
      int maxResults,
      GetOptions getOptions,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), false);
      validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      validation.check((IValidatable[]) requests, nameof (requests), false);
      Validation.checkGetOptions(getOptions == GetOptions.GetAll, maxResults);
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, ownerName);
      CommandGet disposableObject = (CommandGet) null;
      try
      {
        disposableObject = new CommandGet(controlRequestContext);
        disposableObject.Execute(workspace, requests, getOptions, maxResults, itemPropertyFilters, itemAttributeFilters);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.GetOperations
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public void CreateCheckinNoteDefinition(
      IVssRequestContext requestContext,
      string associatedServerItem,
      CheckinNoteFieldDefinition[] checkinNoteFields)
    {
      this.CreateCheckinNoteDefinition(requestContext, associatedServerItem, checkinNoteFields, PathLength.Length259);
    }

    public void CreateCheckinNoteDefinition(
      IVssRequestContext requestContext,
      string associatedServerItem,
      CheckinNoteFieldDefinition[] checkinNoteFields,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      Validation validation = controlRequestContext.Validation;
      validation.checkServerItem(ref associatedServerItem, nameof (associatedServerItem), false, false, true, false, controlRequestContext.MaxSupportedServerPathLength);
      validation.check((IValidatable[]) checkinNoteFields, nameof (checkinNoteFields), true);
      CheckinNote.CreateDefinition(controlRequestContext, associatedServerItem, checkinNoteFields);
    }

    public List<CheckinNoteFieldDefinition> QueryCheckinNoteDefinition(
      IVssRequestContext requestContext,
      string[] associatedServerItem)
    {
      return CheckinNote.QueryDefinition(this.GetVersionControlRequestContext(requestContext), associatedServerItem);
    }

    public void UpdateCheckinNoteFieldName(
      IVssRequestContext requestContext,
      string path,
      string existingFieldName,
      string newFieldName)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkServerItem(ref path, nameof (path), false, false, true, false, controlRequestContext.MaxSupportedServerPathLength);
      validation.checkFieldName(existingFieldName, nameof (existingFieldName), false);
      validation.checkFieldName(newFieldName, nameof (newFieldName), false);
      CheckinNote.UpdateCheckinNoteFieldName(controlRequestContext, path, existingFieldName, newFieldName);
    }

    public List<string> QueryCheckinNoteFieldNames(IVssRequestContext requestContext) => CheckinNote.QueryCheckinNoteFieldNames(this.GetVersionControlRequestContext(requestContext));

    public TeamFoundationDataReader UpdateGlobalSecurity(
      IVssRequestContext requestContext,
      PermissionChange[] changes)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      controlRequestContext.Validation.check((IValidatable[]) changes, nameof (changes), false);
      List<Failure> failures = new List<Failure>();
      return new TeamFoundationDataReader(new object[2]
      {
        (object) this.SecurityWrapper.UpdateGlobalSecurity(controlRequestContext, changes, failures),
        (object) failures
      });
    }

    public TeamFoundationDataReader UpdateItemSecurity(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      SecurityChange[] changes)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.check((IValidatable[]) changes, nameof (changes), false);
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      List<Failure> failures = new List<Failure>();
      return new TeamFoundationDataReader(new object[2]
      {
        (object) this.SecurityWrapper.UpdateItemSecurity(controlRequestContext, workspace, changes, failures),
        (object) failures
      });
    }

    public void RefreshIdentityDisplayName(IVssRequestContext requestContext) => requestContext.GetService<IdentityService>().IdentityServiceInternal().RefreshIdentity(requestContext, (IdentityDescriptor) null);

    public List<string> QueryEffectiveGlobalPermissions(
      IVssRequestContext requestContext,
      string identityName)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      controlRequestContext.Validation.checkIdentity(ref identityName, nameof (identityName), false);
      return this.SecurityWrapper.QueryEffectiveGlobalPermissions(controlRequestContext, requestContext.UserContext, identityName);
    }

    public List<string> QueryEffectiveItemPermissions(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      string item,
      string identityName)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.checkItem(ref item, nameof (item), false, false, true, false, controlRequestContext.MaxSupportedServerPathLength);
      validation.checkIdentity(ref identityName, nameof (identityName), false);
      Workspace localWorkspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      ItemPathPair serverItem = ItemSpec.toServerItem(requestContext, item, localWorkspace, true);
      return this.SecurityWrapper.QueryEffectiveItemPermissions(controlRequestContext, serverItem, identityName);
    }

    public GlobalSecurity QueryGlobalPermissions(
      IVssRequestContext requestContext,
      string[] identityNames)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      controlRequestContext.Validation.checkIdentities(identityNames, nameof (identityNames), true);
      return this.SecurityWrapper.QueryGlobalPermissions(controlRequestContext, identityNames);
    }

    public TeamFoundationDataReader QueryItemPermissions(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      string[] identityNames)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.emptyToRoot(ref itemSpecs);
      validation.check((IValidatable[]) itemSpecs, nameof (itemSpecs), false);
      validation.checkIdentities(identityNames, nameof (identityNames), true);
      validation.emptyToNull(ref identityNames);
      Workspace workspace = Workspace.QueryWorkspace(controlRequestContext, workspaceName, workspaceOwner);
      List<Failure> failures = new List<Failure>();
      return new TeamFoundationDataReader(new object[2]
      {
        (object) this.SecurityWrapper.QueryItemPermissions(controlRequestContext, workspace, itemSpecs, identityNames, failures),
        (object) failures
      });
    }

    public TeamFoundationDataReader GetChangesetProperty(
      IVssRequestContext requestContext,
      int changesetId,
      string[] propertyNameFilters)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      CommandGetChangesetProperty disposableObject = (CommandGetChangesetProperty) null;
      try
      {
        disposableObject = new CommandGetChangesetProperty(controlRequestContext);
        disposableObject.Execute(changesetId, propertyNameFilters);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.First
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    public void SetChangesetProperty(
      IVssRequestContext requestContext,
      int changesetId,
      PropertyValue[] propertyValues)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      controlRequestContext.Validation.checkNotNull((object) propertyValues, nameof (propertyValues));
      using (CommandSetChangesetProperty changesetProperty = new CommandSetChangesetProperty(controlRequestContext))
        changesetProperty.Execute(changesetId, propertyValues);
    }

    [Obsolete("Please use QueryItems to retrieve versioned item attributes.", false)]
    public TeamFoundationDataReader GetVersionedItemProperty(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      VersionSpec versionSpec,
      DeletedState deletedState,
      ItemType itemType,
      string[] propertyNameFilters)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.emptyToRoot(ref itemSpecs);
      validation.checkVersionSpec(versionSpec, nameof (versionSpec), false);
      validation.checkNotNull((object) propertyNameFilters, nameof (propertyNameFilters));
      CommandGetVersionedItemProperty disposableObject = (CommandGetVersionedItemProperty) null;
      try
      {
        disposableObject = new CommandGetVersionedItemProperty(controlRequestContext);
        disposableObject.Execute(workspaceName, workspaceOwner, itemSpecs, versionSpec, deletedState, itemType, propertyNameFilters);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[1]
        {
          (object) disposableObject.Result
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    [Obsolete("Please use SetVersionedItemAttribute to set attributes on versioned items.", false)]
    public void SetVersionedItemProperty(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      VersionSpec versionSpec,
      DeletedState deletedState,
      ItemType itemType,
      PropertyValue[] propertyValues)
    {
      this.SetVersionedItemAttribute(requestContext, workspaceName, workspaceOwner, itemSpecs, versionSpec, deletedState, itemType, propertyValues);
    }

    public void SetVersionedItemAttribute(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      VersionSpec versionSpec,
      DeletedState deletedState,
      ItemType itemType,
      PropertyValue[] propertyValues)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      validation.emptyToRoot(ref itemSpecs);
      validation.check((IValidatable[]) itemSpecs, nameof (itemSpecs), false);
      validation.checkVersionSpec(versionSpec, nameof (versionSpec), true);
      validation.checkNotNull((object) propertyValues, nameof (propertyValues));
      using (CommandSetVersionedItemProperty versionedItemProperty = new CommandSetVersionedItemProperty(controlRequestContext))
        versionedItemProperty.Execute(workspaceName, workspaceOwner, itemSpecs, versionSpec, deletedState, itemType, propertyValues);
    }

    public List<PathRestriction> FilterChangeset(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity[] userIdentities,
      int changeset)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      List<PathRestriction> restrictions = (List<PathRestriction>) null;
      this.IntegrationInterface.FilterChangeset(controlRequestContext, userIdentities, changeset, this.GetIntegrationAlertChangeCountLimit(controlRequestContext), out restrictions);
      return restrictions;
    }

    public List<PathRestriction> FilterShelveset(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity[] userIdentities,
      string shelvesetName,
      string shelvesetOwner)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      List<PathRestriction> restrictions = (List<PathRestriction>) null;
      this.IntegrationInterface.FilterShelveset(controlRequestContext, userIdentities, shelvesetName, shelvesetOwner, this.GetIntegrationAlertChangeCountLimit(controlRequestContext), out restrictions);
      return restrictions;
    }

    internal Item GetLatestItemVersion(IVssRequestContext requestContext, string artifactUri)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUri);
      if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "LatestItemVersion"))
        return this.QueryItemById(requestContext, int.Parse(artifactId.ToolSpecificId, (IFormatProvider) CultureInfo.InvariantCulture), int.MaxValue, true);
      throw new InvalidArtifactTypeException(artifactId.ArtifactType);
    }

    internal Item GetVersionedItem(IVssRequestContext requestContext, string artifactUri)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUri);
      if (!VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "VersionedItem"))
        throw new InvalidArtifactTypeException(artifactId.ArtifactType);
      if (string.IsNullOrEmpty(artifactId.ToolSpecificId))
        throw new ArtifactIdentifierRequiredException();
      string serverItem;
      int changeset;
      int deletionId;
      VersionedItemUri.Decode(artifactId.ToolSpecificId, out serverItem, out changeset, out deletionId, out UriType _);
      ItemSpec itemSpec = new ItemSpec(serverItem, RecursionType.None, deletionId);
      controlRequestContext.Validation.check((IValidatable) itemSpec, "serverPath", false);
      if (VersionControlPath.IsWildcard(serverItem))
        throw new WildcardNotAllowedException("WildcardNotAllowedException", new object[1]
        {
          (object) "artifactMoniker"
        });
      using (CommandQueryItems<ItemSet, Item> commandQueryItems = new CommandQueryItems<ItemSet, Item>(controlRequestContext))
      {
        commandQueryItems.Execute((Workspace) null, new ItemSpec[1]
        {
          itemSpec
        }, (VersionSpec) new ChangesetVersionSpec(changeset), (DeletedState) (deletionId == 0 ? 0 : 1), ItemType.Any, true, 0);
        StreamingCollection<ItemSet> itemSets = commandQueryItems.ItemSets;
        ItemSet itemSet = itemSets.MoveNext() ? itemSets.Current : throw new ItemNotFoundException(artifactId.ToolSpecificId);
        if (!itemSet.Items.MoveNext())
          throw new ItemNotFoundException(artifactId.ToolSpecificId);
        Item current = itemSet.Items.Current;
        if (deletionId != current.DeletionId)
          throw new ItemNotFoundException(artifactId.ToolSpecificId);
        return current;
      }
    }

    public TeamFoundationDataReader QueryFileContents(
      IVssRequestContext requestContext,
      Item serverItem)
    {
      this.GetVersionControlRequestContext(requestContext).Validation.checkNotNull((object) serverItem, nameof (serverItem));
      Stream stream = (Stream) null;
      try
      {
        byte[] hashValue;
        long contentLength;
        CompressionType compressionType;
        stream = requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) serverItem.fileId, false, out hashValue, out contentLength, out compressionType);
        return new TeamFoundationDataReader((IEnumerable<IDisposable>) new IDisposable[1]
        {
          (IDisposable) stream
        }, new object[4]
        {
          (object) stream,
          (object) hashValue,
          (object) contentLength,
          (object) compressionType
        });
      }
      catch (Exception ex)
      {
        stream?.Dispose();
        throw;
      }
    }

    public TeamFoundationDataReader QueryFileContents(
      IVssRequestContext requestContext,
      string serverPath,
      int deletionId,
      VersionSpec version)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkServerItem(ref serverPath, nameof (serverPath), false, false, this.GetAllow8Dot3Paths(controlRequestContext), true, controlRequestContext.MaxSupportedServerPathLength);
      validation.checkVersionSpec(version, nameof (version), false);
      Item serverItem;
      using (CommandQueryItems<ItemSet, Item> commandQueryItems = new CommandQueryItems<ItemSet, Item>(controlRequestContext))
      {
        ItemSpec[] itemSpecs = new ItemSpec[1]
        {
          new ItemSpec(serverPath, RecursionType.None, deletionId)
        };
        commandQueryItems.Execute((Workspace) null, itemSpecs, version, deletionId > 0 ? DeletedState.Deleted : DeletedState.NonDeleted, ItemType.File, true, 0);
        List<Item> objList = new List<Item>((IEnumerable<Item>) new List<ItemSet>((IEnumerable<ItemSet>) commandQueryItems.ItemSets)[0].Items);
        serverItem = objList.Count == 1 ? objList[0] : throw new ItemNotFoundException(serverPath);
      }
      return this.QueryFileContents(requestContext, serverItem);
    }

    public void DownloadFile(
      IVssRequestContext requestContext,
      string serverPath,
      int deletionId,
      VersionSpec version,
      string localFileName)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkServerItem(ref serverPath, nameof (serverPath), false, false, this.GetAllow8Dot3Paths(controlRequestContext), true, controlRequestContext.MaxSupportedServerPathLength);
      validation.checkVersionSpec(version, nameof (version), false);
      validation.checkLocalItem(localFileName, nameof (localFileName), false, false, this.GetAllow8Dot3Paths(controlRequestContext), true);
      try
      {
        using (TeamFoundationDataReader foundationDataReader = this.QueryFileContents(requestContext, serverPath, deletionId, version))
        {
          using (FileStream fileStream = File.Create(localFileName))
          {
            Stream stream = foundationDataReader.Current<Stream>();
            using (ByteArray byteArray = new ByteArray(65536))
            {
              int count;
              while ((count = stream.Read(byteArray.Bytes, 0, byteArray.Bytes.Length)) > 0)
              {
                fileStream.Write(byteArray.Bytes, 0, count);
                fileStream.Flush();
              }
            }
          }
        }
      }
      catch (Exception ex1)
      {
        requestContext.TraceException(700164, TraceArea.Download, TraceLayer.BusinessLogic, ex1);
        try
        {
          FileSpec.DeleteFile(localFileName);
        }
        catch (Exception ex2)
        {
        }
        throw;
      }
    }

    public void UploadFile(
      IVssRequestContext requestContext,
      string workspaceName,
      string workspaceOwner,
      string serverItem,
      byte[] hash,
      Stream fileStream,
      long fileLength,
      long compressedLength,
      long offsetFrom,
      CompressionType compressionType)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      Validation validation = controlRequestContext.Validation;
      validation.checkWorkspaceName(workspaceName, "wsname", false);
      validation.checkIdentity(ref workspaceOwner, "wsowner", false);
      validation.checkServerItem(ref serverItem, "item", false, false, true, false, controlRequestContext.MaxSupportedServerPathLength);
      validation.checkStream(fileStream, nameof (fileStream), false);
      UploadHandler.UploadFile(controlRequestContext, workspaceName, workspaceOwner, serverItem, hash, fileStream, fileLength, compressedLength, offsetFrom, compressionType);
    }

    public void CreateTeamProjectFolder(
      IVssRequestContext requestContext,
      TeamProjectFolderOptions teamProjectOptions)
    {
      this.CreateTeamProjectFolder(requestContext, teamProjectOptions, PathLength.Length259);
    }

    public void CreateTeamProjectFolder(
      IVssRequestContext requestContext,
      TeamProjectFolderOptions teamProjectOptions,
      PathLength maxClientPathLength)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext, maxClientPathLength);
      teamProjectOptions.TeamProject = ProjectInfo.NormalizeProjectName(teamProjectOptions.TeamProject, "TeamProject", true);
      this.GetTeamProjectFolder(controlRequestContext).CreateProjectFolder(controlRequestContext, teamProjectOptions);
    }

    public bool DeleteTeamProjectFolder(
      IVssRequestContext requestContext,
      string teamProject,
      string teamProjectUri,
      bool startCleanup,
      bool deleteWorkspaceState)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      return this.GetTeamProjectFolder(controlRequestContext).DeleteProjectFolder(controlRequestContext, teamProject, teamProjectUri, startCleanup, deleteWorkspaceState);
    }

    public RepositoryProperties GetRepositoryProperties(IVssRequestContext requestContext) => new RepositoryProperties()
    {
      Id = requestContext.ServiceHost.InstanceId,
      LatestChangesetId = this.GetLatestChangeset(requestContext),
      DownloadKey = requestContext.GetService<ITeamFoundationSigningService>().GetPublicKey(requestContext, ProxyConstants.ProxySigningKey, out int _)
    };

    public List<FileType> QueryFileTypes(IVssRequestContext requestContext)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      return this.GetFileTypeManager(controlRequestContext).Query(controlRequestContext);
    }

    public void SetFileTypes(IVssRequestContext requestContext, FileType[] fileTypes)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      controlRequestContext.Validation.check((IValidatable[]) fileTypes, nameof (fileTypes), false);
      this.GetFileTypeManager(controlRequestContext).SetFileTypes(controlRequestContext, fileTypes);
    }

    public void ResetCheckinDates(IVssRequestContext requestContext, DateTime lastCheckinDate)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      lastCheckinDate = lastCheckinDate.ToUniversalTime();
      controlRequestContext.VersionControlService.SecurityWrapper.CheckGlobalPermission(controlRequestContext, GlobalPermissions.AdminConfiguration);
      using (VersionedItemComponent versionedItemComponent = controlRequestContext.VersionControlService.GetVersionedItemComponent(controlRequestContext))
        versionedItemComponent.ResetCheckinDates(lastCheckinDate);
    }

    public ServerSettings GetServerSettings(IVssRequestContext requestContext)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      return new ServerSettings()
      {
        DefaultWorkspaceLocationEnum = this.GetDefaultWorkspaceLocation(controlRequestContext),
        DefaultLocalItemExclusionSet = this.GetDefaultLocalItemExclusionSet(controlRequestContext),
        AllowAsynchronousCheckoutInServerWorkspaces = this.GetAllowAsynchronousCheckoutInServerWorkspaces(controlRequestContext),
        MaxAllowedServerPathLength = (int) this.GetMaxAllowedServerPathLength(controlRequestContext),
        StableHashString = this.GetServerSettingsStableHashString(requestContext)
      };
    }

    internal string GetServerSettingsStableHashString(IVssRequestContext requestContext)
    {
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      string str = this.GetRawDefaultLocalItemExclusionSet(controlRequestContext) ?? string.Empty;
      return Convert.ToBase64String(Encoding.Unicode.GetBytes(ClrHashUtil.GetStringHashOrcas64(this.GetDefaultWorkspaceLocation(controlRequestContext).ToString() + str + this.GetAllowAsynchronousCheckoutInServerWorkspaces(controlRequestContext).ToString() + this.GetMaxAllowedServerPathLength(controlRequestContext).ToString()).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
    }

    public void SetServerSettings(IVssRequestContext requestContext, ServerSettings settings)
    {
      ArgumentUtility.CheckForNull<ServerSettings>(settings, nameof (settings));
      VersionControlRequestContext controlRequestContext = this.GetVersionControlRequestContext(requestContext);
      if (settings.MaxAllowedServerPathLength > 399)
        settings.MaxAllowedServerPathLength = 399;
      else if (!Enum.IsDefined(typeof (PathLength), (object) settings.MaxAllowedServerPathLength))
        settings.MaxAllowedServerPathLength = 0;
      controlRequestContext.VersionControlService.SecurityWrapper.CheckGlobalPermission(controlRequestContext, GlobalPermissions.AdminConfiguration);
      this.SetDefaultWorkspaceLocation(controlRequestContext, settings.DefaultWorkspaceLocationEnum);
      if (settings.DefaultLocalItemExclusionSet != null)
        this.SetDefaultLocalItemExclusionList(controlRequestContext, settings.DefaultLocalItemExclusionSet);
      if (settings.AllowAsynchronousCheckoutInServerWorkspaces != this.GetAllowAsynchronousCheckoutInServerWorkspaces(controlRequestContext))
      {
        if (settings.AllowAsynchronousCheckoutInServerWorkspaces)
        {
          using (VersionedItemComponent versionedItemComponent = controlRequestContext.VersionControlService.GetVersionedItemComponent(controlRequestContext))
            versionedItemComponent.ConvertCheckoutLocks();
        }
        this.SetAllowAsynchronousCheckoutInServerWorkspaces(controlRequestContext, settings.AllowAsynchronousCheckoutInServerWorkspaces);
      }
      PathLength serverPathLength1 = (PathLength) settings.MaxAllowedServerPathLength;
      PathLength serverPathLength2 = this.GetMaxAllowedServerPathLength(controlRequestContext);
      if (serverPathLength1 == PathLength.Unspecified || serverPathLength1 == serverPathLength2)
        return;
      if (serverPathLength1 < serverPathLength2)
        throw new NotSupportedException(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Get("CannotDecreaseMaxPathLength"));
      this.SetMaxAllowedServerPathLength(controlRequestContext, serverPathLength1);
    }

    internal FileTypeManager GetFileTypeManager(VersionControlRequestContext vcRequestContext)
    {
      if (this.m_fileTypeManager == null)
      {
        lock (this.m_initLock)
        {
          if (this.m_fileTypeManager == null)
            this.m_fileTypeManager = new FileTypeManager(vcRequestContext.Elevate());
        }
      }
      return this.m_fileTypeManager;
    }

    internal TeamProjectFolder GetTeamProjectFolder(VersionControlRequestContext vcRequestContext)
    {
      if (this.m_teamProjectFolder == null)
      {
        lock (this.m_initLock)
        {
          if (this.m_teamProjectFolder == null)
            this.m_teamProjectFolder = new TeamProjectFolder(vcRequestContext.Elevate());
        }
      }
      return this.m_teamProjectFolder;
    }

    internal IdentityService GetIdentityService(VersionControlRequestContext vcRequestContext) => vcRequestContext.Elevate().RequestContext.GetService<IdentityService>();

    internal ITeamFoundationPropertyService GetPropertyService(
      VersionControlRequestContext vcRequestContext)
    {
      return vcRequestContext.Elevate().RequestContext.GetService<ITeamFoundationPropertyService>();
    }

    internal IVssServiceHost ServiceHost { get; private set; }

    internal SecurityManager SecurityWrapper { get; private set; }

    internal IntegrationInterface IntegrationInterface
    {
      get
      {
        if (this.m_integrationInterface == null)
        {
          lock (this.m_initLock)
          {
            if (this.m_integrationInterface == null)
              this.m_integrationInterface = new IntegrationInterface();
          }
        }
        return this.m_integrationInterface;
      }
    }

    internal bool IsMidTierFileCacheEnabled(IVssRequestContext requestContext) => requestContext.Elevate().GetService<TeamFoundationFileCacheService>().IsVCCacheEnabled;

    internal IVersionControlWebCache<WorkspaceInternal> GetWorkspaceCache(
      VersionControlRequestContext vcRequestContext)
    {
      if (this.IsWorkspaceCache(vcRequestContext))
      {
        if (this.m_workspaceCache == null)
        {
          lock (this.m_workspaceAccessLock)
          {
            if (this.IsWorkspaceCache(vcRequestContext))
            {
              if (this.m_workspaceCache == null)
                this.m_workspaceCache = new WorkspaceCache(vcRequestContext);
            }
          }
        }
      }
      else if (this.m_workspaceCache != null)
      {
        lock (this.m_workspaceAccessLock)
        {
          if (!this.IsWorkspaceCache(vcRequestContext))
          {
            if (this.m_workspaceCache != null)
            {
              this.m_workspaceCache.Unload(vcRequestContext.RequestContext);
              this.m_workspaceCache = (WorkspaceCache) null;
            }
          }
        }
      }
      return (IVersionControlWebCache<WorkspaceInternal>) this.m_workspaceCache;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      lock (this.m_initLock)
      {
        try
        {
          this.ServiceHost = systemRequestContext.ServiceHost;
          this.StartSettings(systemRequestContext);
          VersionControlRequestContext controlRequestContext = new VersionControlRequestContext(systemRequestContext, this);
          this.SecurityWrapper = new SecurityManager(systemRequestContext);
        }
        catch (Exception ex)
        {
          systemRequestContext.TraceException(700165, TraceArea.General, TraceLayer.BusinessLogic, ex);
          this.ServiceEndInternal(systemRequestContext);
          throw;
        }
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.ServiceEndInternal(systemRequestContext);

    private void ServiceEndInternal(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      lock (this.m_initLock)
      {
        this.StopSettings(systemRequestContext);
        lock (this.m_workspaceAccessLock)
        {
          if (this.m_workspaceCache != null)
          {
            this.m_workspaceCache.Unload(systemRequestContext);
            this.m_workspaceCache = (WorkspaceCache) null;
          }
        }
        if (this.m_fileTypeManager != null)
          this.m_fileTypeManager.Unload(systemRequestContext);
        if (this.m_teamProjectFolder != null)
          this.m_teamProjectFolder.Unload(systemRequestContext);
        this.ServiceHost = (IVssServiceHost) null;
        this.m_fileTypeManager = (FileTypeManager) null;
        this.m_teamProjectFolder = (TeamProjectFolder) null;
        this.SecurityWrapper = (SecurityManager) null;
        this.m_integrationInterface = (IntegrationInterface) null;
        this.m_earliestChangesetDate = DateTime.MinValue;
        this.ReleaseVersionControlRequestContext(systemRequestContext);
      }
    }

    internal VersionControlRequestContext GetVersionControlRequestContext(
      IVssRequestContext requestContext,
      PathLength maxClientPathLength = PathLength.Unspecified)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      VersionControlRequestContext controlRequestContext = requestContext.GetVersionControlRequestContext();
      controlRequestContext.MaxSupportedServerPathLength = maxClientPathLength;
      return controlRequestContext;
    }

    internal void ReleaseVersionControlRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.Items.ContainsKey("VersionControlRequestContext"))
        return;
      requestContext.Items.Remove("VersionControlRequestContext");
    }

    internal bool GetAllow8Dot3Paths(VersionControlRequestContext vcRequestContext) => this.GetValue<bool>(vcRequestContext, "allow8Dot3Paths", false);

    internal int GetMaxItemsPerRequest(VersionControlRequestContext vcRequestContext) => this.GetValueGreaterThanZero(vcRequestContext, "MaxItemsPerRequest", int.MaxValue);

    internal int GetMaxInputsHeavyRequest(VersionControlRequestContext vcRequestContext) => this.GetValueGreaterThanZero(vcRequestContext, "MaxInputsPerHeavyRequest", 64);

    internal int GetMaxInputsLightRequest(VersionControlRequestContext vcRequestContext) => this.GetValueGreaterThanZero(vcRequestContext, "MaxInputsPerLightRequest", int.MaxValue);

    internal int GetMaxRowsEvaluated(VersionControlRequestContext vcRequestContext) => this.GetValueGreaterThanZero(vcRequestContext, "MaxRowsEvaluated", int.MaxValue);

    internal long GetDeltaMaxFileSize(VersionControlRequestContext vcRequestContext) => this.GetValue<long>(vcRequestContext, "DeltaMaxFileSize", 16777216L);

    internal int GetChangesetArtifactChangesLimit(VersionControlRequestContext vcRequestContext) => this.GetValue<int>(vcRequestContext, "ArtifactChangeLimit", 1000);

    internal int GetShelvesetArtifactChangesLimit(VersionControlRequestContext vcRequestContext) => this.GetValue<int>(vcRequestContext, "ArtifactShelvesetChangeLimit", 1000);

    internal int GetSprocRecompileThreshold(VersionControlRequestContext vcRequestContext) => this.GetValue<int>(vcRequestContext, "sprocRecompileThreshold", 10000);

    internal bool GetValidateFileContentsOnUpload(VersionControlRequestContext vcRequestContext) => this.GetValue<bool>(vcRequestContext, "ValidateFileContents", true);

    internal int GetXmlParameterChunkThreshold(VersionControlRequestContext vcRequestContext) => this.GetValueGreaterThanZero(vcRequestContext, "XmlParameterChunkThreshold", 50000);

    internal int GetCacheLimit(VersionControlRequestContext vcRequestContext) => this.GetValue<int>(vcRequestContext, "CacheLimit", 0);

    internal int GetCacheLimitPercent(VersionControlRequestContext vcRequestContext) => this.GetValue<int>(vcRequestContext, "CacheLimitPercent", 0);

    internal int GetCacheDeletionPercent(VersionControlRequestContext vcRequestContext) => this.GetValue<int>(vcRequestContext, "CacheDeletionPercent", 10);

    internal int GetStatisticsPersistTime(VersionControlRequestContext vcRequestContext) => this.GetValueGreaterThanZero(vcRequestContext, "StatisticsPersistTime", 1);

    internal int GetIntegrationEvaluationChangeCountLimit(
      VersionControlRequestContext vcRequestContext)
    {
      return this.GetValue<int>(vcRequestContext, "AlertEvaluationChangeLimit", 250);
    }

    internal int GetIntegrationAlertChangeCountLimit(VersionControlRequestContext vcRequestContext) => this.GetValue<int>(vcRequestContext, "AlertChangeLimit", 25);

    internal bool GetDisableMultipleRenames(VersionControlRequestContext vcRequestContext) => this.GetValue<bool>(vcRequestContext, "disableMultipleRenames", false);

    internal bool GetDisableResetCheckinDateEmptyValidation(
      VersionControlRequestContext vcRequestContext)
    {
      return this.GetValue<bool>(vcRequestContext, "DisableResetCheckinDateEmptyValidation", false);
    }

    internal WorkspaceLocation GetDefaultWorkspaceLocation(
      VersionControlRequestContext vcRequestContext)
    {
      return this.GetValue<WorkspaceLocation>(vcRequestContext, "DefaultWorkspaceLocation", WorkspaceLocation.Local);
    }

    internal void SetDefaultWorkspaceLocation(
      VersionControlRequestContext vcRequestContext,
      WorkspaceLocation location)
    {
      this.SetValue<WorkspaceLocation>(vcRequestContext, "DefaultWorkspaceLocation", location);
    }

    internal bool GetAllowAsynchronousCheckoutInServerWorkspaces(
      VersionControlRequestContext vcRequestContext)
    {
      return this.GetValue<bool>(vcRequestContext, "AllowAsynchronousCheckoutInServerWorkspaces", false);
    }

    internal void SetAllowAsynchronousCheckoutInServerWorkspaces(
      VersionControlRequestContext vcRequestContext,
      bool value)
    {
      this.SetValue<bool>(vcRequestContext, "AllowAsynchronousCheckoutInServerWorkspaces", value);
    }

    internal bool GetLogDestroyEvents(VersionControlRequestContext vcRequestContext) => this.GetValue<bool>(vcRequestContext, "LogDestroyEvents", true);

    internal PathLength GetMaxAllowedServerPathLength(VersionControlRequestContext vcRequestContext)
    {
      PathLength serverPathLength = this.GetValue<PathLength>(vcRequestContext, "MaxServerPathSize", PathLength.Length399);
      if (serverPathLength == PathLength.Unspecified)
        serverPathLength = PathLength.Length399;
      return serverPathLength;
    }

    internal void SetMaxAllowedServerPathLength(
      VersionControlRequestContext vcRequestContext,
      PathLength value)
    {
      this.SetValue<PathLength>(vcRequestContext, "MaxServerPathSize", value);
    }

    internal string GetRawDefaultLocalItemExclusionSet(VersionControlRequestContext vcRequestContext) => this.GetValue<string>(vcRequestContext, "DefaultLocalItemExclusionSet", (string) null);

    internal LocalItemExclusionSet GetDefaultLocalItemExclusionSet(
      VersionControlRequestContext vcRequestContext)
    {
      string itemExclusionSet = this.GetRawDefaultLocalItemExclusionSet(vcRequestContext);
      if (string.IsNullOrEmpty(itemExclusionSet))
        return this.m_defaultLocalItemExclusionSet;
      try
      {
        return TeamFoundationSerializationUtility.Deserialize<LocalItemExclusionSet>(itemExclusionSet);
      }
      catch (Exception ex)
      {
        vcRequestContext.RequestContext.TraceException(700166, TraceArea.Settings, TraceLayer.BusinessLogic, ex);
      }
      return (LocalItemExclusionSet) null;
    }

    internal void SetDefaultLocalItemExclusionList(
      VersionControlRequestContext vcRequestContext,
      LocalItemExclusionSet exclusions)
    {
      ArgumentUtility.CheckForNull<LocalItemExclusionSet>(exclusions, nameof (exclusions));
      exclusions.Watermark = Guid.NewGuid();
      this.SetValue<string>(vcRequestContext, "DefaultLocalItemExclusionSet", TeamFoundationSerializationUtility.SerializeToString<LocalItemExclusionSet>(exclusions));
    }

    internal Guid DebugDataspace(VersionControlRequestContext vcRequestContext) => this.GetValue<Guid>(vcRequestContext, nameof (DebugDataspace), Guid.Empty);

    private void StartSettings(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (TeamFoundationVersionControlService));
      this.m_serviceSettings = systemRequestContext.GetService<IVssRegistryService>();
      ArgumentUtility.CheckForNull<IVssRegistryService>(this.m_serviceSettings, "systemRequestContext.GetService<IVssRegistryService>()");
      this.m_sqlNotificationService = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      ArgumentUtility.CheckForNull<ITeamFoundationSqlNotificationService>(this.m_sqlNotificationService, "systemRequestContext.GetService<ITeamFoundationSqlNotificationService>()");
      this.m_cache = new Dictionary<string, object>();
      string[] strArray = Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Get("DefaultLocalItemExclusions").Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries);
      this.m_defaultLocalItemExclusionSet = new LocalItemExclusionSet();
      this.m_defaultLocalItemExclusionSet.Exclusions = strArray;
      this.m_defaultLocalItemExclusionSet.Watermark = Guid.Parse("03930288-94C3-4D25-B9A2-D31BB232848A");
      this.RegisterNotification(systemRequestContext);
    }

    private void StopSettings(IVssRequestContext systemRequestContext)
    {
      try
      {
        this.m_lock.AcquireWriterLock(-1);
        this.UnregisterNotification(systemRequestContext);
        this.m_cache = (Dictionary<string, object>) null;
        this.m_serviceSettings = (IVssRegistryService) null;
      }
      finally
      {
        if (this.m_lock.IsWriterLockHeld)
          this.m_lock.ReleaseWriterLock();
      }
    }

    private string GetValue(VersionControlRequestContext vcRequestContext, string name) => this.GetValue<string>(vcRequestContext, name, (string) null);

    private T GetValue<T>(
      VersionControlRequestContext vcRequestContext,
      string name,
      T defaultValue)
    {
      vcRequestContext.RequestContext.Trace(700167, TraceLevel.Verbose, TraceArea.Settings, TraceLayer.BusinessLogic, "Entering {0}: {1} {2}", (object) "Settings.GetValue", (object) name, (object) defaultValue);
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      object obj = (object) null;
      try
      {
        this.m_lock.AcquireReaderLock(-1);
        if (this.m_cache == null)
          throw new InvalidOperationException(Resources.Get("VersionControlServiceNotRunning"));
        if (this.m_cache.TryGetValue(name, out obj))
        {
          vcRequestContext.RequestContext.Trace(700168, TraceLevel.Verbose, TraceArea.Settings, TraceLayer.BusinessLogic, "Leaving {0}: Found value in cache: {1}", (object) "Settings.GetValue", obj);
          return (T) obj;
        }
      }
      finally
      {
        if (this.m_lock.IsReaderLockHeld)
          this.m_lock.ReleaseReaderLock();
      }
      vcRequestContext.RequestContext.Trace(700169, TraceLevel.Verbose, TraceArea.Settings, TraceLayer.BusinessLogic, "Setting is not in cache, look in database");
      try
      {
        this.m_lock.AcquireWriterLock(-1);
        if (this.m_cache == null)
          throw new InvalidOperationException(Resources.Get("VersionControlServiceNotRunning"));
        if (this.m_cache.TryGetValue(name, out obj))
        {
          vcRequestContext.RequestContext.Trace(700170, TraceLevel.Verbose, TraceArea.Settings, TraceLayer.BusinessLogic, "Leaving {0}: Found value in cache: {1}", (object) "Settings.GetValue", obj);
          return (T) obj;
        }
        if (typeof (T).IsEnum)
        {
          string s = this.m_serviceSettings.GetValue(vcRequestContext.Elevate().RequestContext, (RegistryQuery) ("/Service/VersionControl/Settings/" + name), false, (string) null);
          obj = (object) defaultValue;
          if (!string.IsNullOrEmpty(s))
          {
            try
            {
              obj = (object) TFCommonUtil.EnumFromString<T>(s);
            }
            catch (Exception ex)
            {
              vcRequestContext.RequestContext.TraceException(700171, TraceLevel.Warning, TraceArea.Linking, TraceLayer.BusinessLogic, ex);
            }
          }
        }
        else
          obj = (object) this.m_serviceSettings.GetValue<T>(vcRequestContext.Elevate().RequestContext, (RegistryQuery) ("/Service/VersionControl/Settings/" + name), defaultValue);
        this.m_cache[name] = obj;
      }
      finally
      {
        if (this.m_lock.IsWriterLockHeld)
          this.m_lock.ReleaseWriterLock();
      }
      vcRequestContext.RequestContext.Trace(700172, TraceLevel.Verbose, TraceArea.Settings, TraceLayer.BusinessLogic, "Leaving {0}: Caching value: {1}", (object) "Settings.GetValue", obj);
      return (T) obj;
    }

    private void SetValue<T>(VersionControlRequestContext vcRequestContext, string name, T value)
    {
      vcRequestContext.RequestContext.Trace(700173, TraceLevel.Verbose, TraceArea.Settings, TraceLayer.BusinessLogic, "Entering {0}: {1} {2}", (object) "Settings.SetValue", (object) name, (object) value);
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      try
      {
        this.m_lock.AcquireWriterLock(-1);
        this.m_cache.Remove(name);
        this.m_serviceSettings.SetValue<T>(vcRequestContext.Elevate().RequestContext, "/Service/VersionControl/Settings/" + name, value);
      }
      finally
      {
        if (this.m_lock.IsWriterLockHeld)
          this.m_lock.ReleaseWriterLock();
      }
      vcRequestContext.RequestContext.TraceLeave(700174, TraceArea.Settings, TraceLayer.BusinessLogic, "Settings.SetValue");
    }

    private int GetValueGreaterThanZero(
      VersionControlRequestContext vcRequestContext,
      string name,
      int defaultValue)
    {
      ArgumentUtility.CheckForOutOfRange(defaultValue, nameof (defaultValue), 1);
      int valueGreaterThanZero = this.GetValue<int>(vcRequestContext, name, defaultValue);
      if (valueGreaterThanZero <= 0)
        valueGreaterThanZero = defaultValue;
      return valueGreaterThanZero;
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.Trace(700175, TraceLevel.Verbose, TraceArea.Settings, TraceLayer.BusinessLogic, "Entering OnSettingsChanged");
      try
      {
        this.m_lock.AcquireWriterLock(-1);
        foreach (RegistryEntry changedEntry in changedEntries)
          this.m_cache.Remove(changedEntry.Path.Substring("/Service/VersionControl/Settings/".Length));
      }
      finally
      {
        if (this.m_lock.IsWriterLockHeld)
          this.m_lock.ReleaseWriterLock();
      }
      requestContext.TraceLeave(700176, TraceArea.Settings, TraceLayer.BusinessLogic, "Settings.OnSettingsChanged");
    }

    private void RegisterNotification(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), "/Service/VersionControl/Settings/*");

    private void UnregisterNotification(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));

    private bool IsWorkspaceCache(VersionControlRequestContext vcRequestContext) => this.GetValue<bool>(vcRequestContext, "WorkspaceCache", true);

    internal TeamProjectFolderComponent GetTeamProjectFolderComponent(
      VersionControlRequestContext vcRequestContext)
    {
      TeamProjectFolderComponent component = vcRequestContext.RequestContext.CreateComponent<TeamProjectFolderComponent>("VersionControl");
      component.VersionControlRequestContext = vcRequestContext;
      return component;
    }

    internal VersionedItemComponent GetVersionedItemComponent(
      VersionControlRequestContext vcRequestContext)
    {
      VersionedItemComponent component = vcRequestContext.RequestContext.CreateComponent<VersionedItemComponent>("VersionControl");
      component.VersionControlRequestContext = vcRequestContext;
      return component;
    }

    internal WorkspaceComponent GetWorkspaceComponent(VersionControlRequestContext vcRequestContext)
    {
      WorkspaceComponent component = vcRequestContext.RequestContext.CreateComponent<WorkspaceComponent>("VersionControl");
      component.VersionControlRequestContext = vcRequestContext;
      return component;
    }
  }
}
