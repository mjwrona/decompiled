// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Repository5
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/ClientServices/03", Description = "DevOps VersionControl ClientServices web service")]
  [ProxyParentClass("VersionControlClientProxy")]
  [ClientService(ServiceName = "ISCCProvider5", CollectionServiceIdentifier = "A25D0656-DA63-4F51-9DA9-800FFF229D1A")]
  public class Repository5 : VersionControlWebService
  {
    [WebMethod]
    public List<StreamingCollection<GetOperation>> Get(
      string workspaceName,
      string ownerName,
      GetRequest[] requests,
      int maxResults,
      int options,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      int maxClientPathLength)
    {
      try
      {
        EstimatedMethodCost estimatedCost = EstimatedMethodCost.Low;
        if (requests != null)
        {
          foreach (GetRequest request in requests)
          {
            if (request.ItemSpec == null || request.ItemSpec.RecursionType == RecursionType.Full)
            {
              estimatedCost = EstimatedMethodCost.Moderate;
              break;
            }
          }
        }
        MethodInformation methodInformation = new MethodInformation(nameof (Get), MethodType.Normal, estimatedCost, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<GetRequest>(nameof (requests), (IList<GetRequest>) requests);
        methodInformation.AddParameter(nameof (maxResults), (object) maxResults);
        GetOptions getOptions = (GetOptions) options;
        methodInformation.AddParameter(nameof (options), (object) getOptions);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Get(this.RequestContext, workspaceName, ownerName, requests, maxResults, getOptions, itemPropertyFilters, itemAttributeFilters, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<List<StreamingCollection<GetOperation>>>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CheckinResult CreateBranch(
      string sourcePath,
      string targetPath,
      VersionSpec version,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      List<Mapping> mappings,
      int maxClientPathLength,
      out StreamingCollection<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateBranch), MethodType.ReadWrite, EstimatedMethodCost.High, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (sourcePath), (object) sourcePath);
        methodInformation.AddParameter(nameof (targetPath), (object) targetPath);
        methodInformation.AddParameter(nameof (version), (object) version);
        methodInformation.AddArrayParameter<Mapping>(nameof (mappings), (IList<Mapping>) mappings);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        info?.RecordInformation(methodInformation);
        checkinNotificationInfo?.RecordInformation(methodInformation);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.CreateBranch(this.RequestContext, sourcePath, targetPath, version, info, checkinNotificationInfo, mappings, true, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        failures = resource.Current<StreamingCollection<Failure>>();
        resource.MoveNext();
        return resource.Current<CheckinResult>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<GetOperation> PendChanges(
      string workspaceName,
      string ownerName,
      ChangeRequest[] changes,
      int pendChangesOptions,
      int supportedFeatures,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      int maxClientPathLength,
      out List<Failure> failures,
      out int changePendedFlags)
    {
      try
      {
        string webMethodName = nameof (PendChanges);
        if (changes != null && changes.Length != 0)
          webMethodName = webMethodName + "." + changes[0].RequestType.ToString();
        MethodInformation methodInformation = new MethodInformation(webMethodName, MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (pendChangesOptions), (object) (PendChangesOptions) pendChangesOptions);
        methodInformation.AddParameter(nameof (supportedFeatures), (object) (SupportedFeatures) supportedFeatures);
        methodInformation.AddArrayParameter<ChangeRequest>(nameof (changes), (IList<ChangeRequest>) changes);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.PendChanges(this.RequestContext, workspaceName, ownerName, changes, pendChangesOptions, supportedFeatures, itemPropertyFilters, itemAttributeFilters, false, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<GetOperation> streamingCollection = resource.Current<StreamingCollection<GetOperation>>();
        resource.MoveNext();
        failures = resource.Current<List<Failure>>();
        resource.MoveNext();
        changePendedFlags = (int) resource.Current<ChangePendedFlags>();
        return streamingCollection;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<GetOperation> PendChangesInLocalWorkspace(
      string workspaceName,
      string ownerName,
      ChangeRequest[] changes,
      int pendChangesOptions,
      int supportedFeatures,
      out List<Failure> failures,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      int maxClientPathLength,
      out int changePendedFlags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("PendChanges", MethodType.ReadWrite, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (pendChangesOptions), (object) (PendChangesOptions) pendChangesOptions);
        methodInformation.AddParameter(nameof (supportedFeatures), (object) (SupportedFeatures) supportedFeatures);
        methodInformation.AddArrayParameter<ChangeRequest>(nameof (changes), (IList<ChangeRequest>) changes);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.PendChanges(this.RequestContext, workspaceName, ownerName, changes, pendChangesOptions, supportedFeatures, itemPropertyFilters, itemAttributeFilters, true, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<GetOperation> streamingCollection = resource.Current<StreamingCollection<GetOperation>>();
        resource.MoveNext();
        failures = resource.Current<List<Failure>>();
        resource.MoveNext();
        changePendedFlags = (int) resource.Current<ChangePendedFlags>();
        return streamingCollection;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CheckinResult CheckIn(
      string workspaceName,
      string ownerName,
      string[] serverItems,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      int checkinOptions,
      out StreamingCollection<Failure> conflicts,
      out StreamingCollection<Failure> failures,
      bool deferCheckIn,
      int checkInTicket,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CheckIn), MethodType.ReadWrite, EstimatedMethodCost.Moderate, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<string>(nameof (serverItems), (IList<string>) serverItems);
        methodInformation.AddParameter(nameof (checkinOptions), (object) (CheckInOptions2) checkinOptions);
        methodInformation.AddParameter("deferCheckin", (object) deferCheckIn);
        methodInformation.AddParameter(nameof (checkInTicket), (object) checkInTicket);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        info?.RecordInformation(methodInformation);
        checkinNotificationInfo?.RecordInformation(methodInformation);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.CheckIn(this.RequestContext, workspaceName, ownerName, serverItems, info, checkinNotificationInfo, checkinOptions, deferCheckIn, checkInTicket, true, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        CheckinResult checkinResult = resource.Current<CheckinResult>();
        resource.MoveNext();
        conflicts = resource.Current<StreamingCollection<Failure>>();
        resource.MoveNext();
        failures = resource.Current<StreamingCollection<Failure>>();
        methodInformation.AddParameter("Changeset", (object) checkinResult.ChangesetId);
        methodInformation.AddParameter("outCheckInTicket", (object) checkinResult.CheckInTicket);
        return checkinResult;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public CheckinResult CheckInShelveset(
      string shelvesetName,
      string ownerName,
      string changesetOwner,
      CheckinNotificationInfo checkinNotificationInfo,
      int checkinOptions,
      int maxClientPathLength,
      out StreamingCollection<Failure> conflicts,
      out StreamingCollection<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CheckInShelveset), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (shelvesetName), (object) shelvesetName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (changesetOwner), (object) changesetOwner);
        methodInformation.AddParameter(nameof (checkinOptions), (object) (CheckInOptions2) checkinOptions);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        checkinNotificationInfo?.RecordInformation(methodInformation);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.CheckInShelveset(this.RequestContext, shelvesetName, ownerName, 0, changesetOwner, checkinNotificationInfo, checkinOptions, true, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        CheckinResult checkinResult = resource.Current<CheckinResult>();
        resource.MoveNext();
        conflicts = resource.Current<StreamingCollection<Failure>>();
        resource.MoveNext();
        failures = resource.Current<StreamingCollection<Failure>>();
        methodInformation.AddParameter("Changeset", (object) checkinResult.ChangesetId);
        return checkinResult;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<Failure> Shelve(
      string workspaceName,
      string workspaceOwner,
      string[] serverItems,
      Shelveset shelveset,
      bool replace,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (Shelve), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<string>(nameof (serverItems), (IList<string>) serverItems);
        methodInformation.AddParameter(nameof (shelveset), (object) (shelveset.Name + ";" + shelveset.Owner));
        methodInformation.AddParameter(nameof (replace), (object) replace);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        return this.VersionControlService.Shelve(this.RequestContext, workspaceName, workspaceOwner, serverItems, shelveset, replace, (PathLength) maxClientPathLength);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Item[] Destroy(
      ItemSpec item,
      VersionSpec versionSpec,
      VersionSpec stopAtSpec,
      int flags,
      int maxClientPathLength,
      out Failure[] failures,
      out StreamingCollection<PendingSet> pendingChanges,
      out StreamingCollection<PendingSet> shelvedChanges)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (Destroy), MethodType.Admin, EstimatedMethodCost.VeryHigh, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (item), (object) item);
        methodInformation.AddParameter(nameof (versionSpec), (object) versionSpec);
        methodInformation.AddParameter(nameof (stopAtSpec), (object) stopAtSpec);
        methodInformation.AddParameter(nameof (flags), (object) flags);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Destroy(this.RequestContext, item, versionSpec, stopAtSpec, flags, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        Item[] objArray = resource.Current<Item[]>();
        resource.MoveNext();
        failures = resource.Current<Failure[]>();
        resource.MoveNext();
        pendingChanges = resource.Current<StreamingCollection<PendingSet>>();
        resource.MoveNext();
        shelvedChanges = resource.Current<StreamingCollection<PendingSet>>();
        return objArray;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<GetOperation> Merge(
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      ItemSpec target,
      VersionSpec from,
      VersionSpec to,
      LockLevel lockLevel,
      int optionsEx,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      int maxClientPathLength,
      out StreamingCollection<Failure> failures,
      out StreamingCollection<Conflict> conflicts,
      out int changePendedFlags)
    {
      try
      {
        EstimatedMethodCost estimatedCost = EstimatedMethodCost.Low;
        if (source != null && source.RecursionType == RecursionType.Full)
          estimatedCost = EstimatedMethodCost.VeryHigh;
        MethodInformation methodInformation = new MethodInformation(nameof (Merge), MethodType.ReadWrite, estimatedCost, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (source), (object) source);
        methodInformation.AddParameter(nameof (target), (object) target);
        MergeOptionsEx mergeOptionsEx = (MergeOptionsEx) optionsEx;
        methodInformation.AddParameter("options", (object) mergeOptionsEx);
        methodInformation.AddParameter(nameof (lockLevel), (object) lockLevel);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        if (from != null)
          methodInformation.AddParameter(nameof (from), (object) from);
        if (to != null)
          methodInformation.AddParameter(nameof (to), (object) to);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Merge(this.RequestContext, workspaceName, workspaceOwner, source, target, from, to, lockLevel, mergeOptionsEx, itemAttributeFilters, itemPropertyFilters, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<GetOperation> streamingCollection = resource.Current<StreamingCollection<GetOperation>>();
        resource.MoveNext();
        failures = resource.Current<StreamingCollection<Failure>>();
        resource.MoveNext();
        conflicts = resource.Current<StreamingCollection<Conflict>>();
        resource.MoveNext();
        changePendedFlags = (int) resource.Current<ChangePendedFlags>();
        return streamingCollection;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void PromotePendingWorkspaceMappings(
      string workspaceName,
      string ownerName,
      int projectNotificationId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (PromotePendingWorkspaceMappings), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (projectNotificationId), (object) projectNotificationId);
        this.EnterMethod(methodInformation);
        this.VersionControlService.PromotePendingWorkspaceMappings(this.RequestContext, workspaceName, ownerName, projectNotificationId);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public ReconcileResult ReconcileLocalWorkspace(
      string workspaceName,
      string ownerName,
      Guid pendingChangeSignature,
      LocalPendingChange[] pendingChanges,
      ServerItemLocalVersionUpdate[] localVersionUpdates,
      bool clearLocalVersionTable,
      bool throwOnProjectRenamed,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReconcileLocalWorkspace), MethodType.ReadWrite, EstimatedMethodCost.Moderate, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (pendingChangeSignature), (object) pendingChangeSignature);
        methodInformation.AddArrayParameter<LocalPendingChange>(nameof (pendingChanges), (IList<LocalPendingChange>) pendingChanges);
        methodInformation.AddArrayParameter<ServerItemLocalVersionUpdate>(nameof (localVersionUpdates), (IList<ServerItemLocalVersionUpdate>) localVersionUpdates);
        methodInformation.AddParameter(nameof (clearLocalVersionTable), (object) clearLocalVersionTable);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.ReconcileLocalWorkspace(this.RequestContext, workspaceName, ownerName, pendingChangeSignature, pendingChanges, localVersionUpdates, clearLocalVersionTable, throwOnProjectRenamed, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<ReconcileResult>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<GetOperation> Resolve(
      string workspaceName,
      string ownerName,
      int conflictId,
      Resolution resolution,
      string newPath,
      int encoding,
      LockLevel lockLevel,
      PropertyValue[] newProperties,
      out StreamingCollection<GetOperation> undoOperations,
      out StreamingCollection<Conflict> resolvedConflicts,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      int maxClientPathLength,
      out int changePendedFlags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (Resolve), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (conflictId), (object) conflictId);
        methodInformation.AddParameter(nameof (resolution), (object) resolution);
        methodInformation.AddParameter(nameof (newPath), (object) newPath);
        methodInformation.AddParameter(nameof (encoding), (object) encoding);
        methodInformation.AddParameter(nameof (lockLevel), (object) lockLevel);
        methodInformation.AddArrayParameter<PropertyValue>(nameof (newProperties), (IList<PropertyValue>) newProperties);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Resolve(this.RequestContext, workspaceName, ownerName, conflictId, resolution, newPath, encoding, lockLevel, newProperties, itemPropertyFilters, itemAttributeFilters, false, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<GetOperation> streamingCollection = resource.Current<StreamingCollection<GetOperation>>();
        resource.MoveNext();
        undoOperations = resource.Current<StreamingCollection<GetOperation>>();
        resource.MoveNext();
        resolvedConflicts = resource.Current<StreamingCollection<Conflict>>();
        resource.MoveNext();
        changePendedFlags = (int) resource.Current<ChangePendedFlags>();
        return streamingCollection;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<GetOperation> UndoPendingChanges(
      string workspaceName,
      string ownerName,
      ItemSpec[] items,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      int maxClientPathLength,
      out List<Failure> failures,
      out int changePendedFlags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UndoPendingChanges), MethodType.ReadWrite, EstimatedMethodCost.Moderate, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.UndoPendingChanges(this.RequestContext, workspaceName, ownerName, items, itemPropertyFilters, itemAttributeFilters, true, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<GetOperation> streamingCollection = resource.Current<StreamingCollection<GetOperation>>();
        resource.MoveNext();
        failures = resource.Current<List<Failure>>();
        resource.MoveNext();
        changePendedFlags = (int) resource.Current<ChangePendedFlags>();
        return streamingCollection;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<GetOperation> UndoPendingChangesInLocalWorkspace(
      string workspaceName,
      string ownerName,
      ItemSpec[] items,
      out List<Failure> failures,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      out int changePendedFlags,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("UndoPendingChanges", MethodType.ReadWrite, EstimatedMethodCost.Moderate, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.UndoPendingChanges(this.RequestContext, workspaceName, ownerName, items, itemPropertyFilters, itemAttributeFilters, false, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<GetOperation> streamingCollection = resource.Current<StreamingCollection<GetOperation>>();
        resource.MoveNext();
        failures = resource.Current<List<Failure>>();
        resource.MoveNext();
        changePendedFlags = (int) resource.Current<ChangePendedFlags>();
        return streamingCollection;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public Shelveset Unshelve(
      string shelvesetName,
      string shelvesetOwner,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      string[] itemPropertyFilters,
      string[] itemAttrbuteFilters,
      string[] shelvesetPropertyNameFilters,
      bool merge,
      int maxClientPathLength,
      out List<Failure> failures,
      out StreamingCollection<GetOperation> getOperations,
      out StreamingCollection<Conflict> conflicts,
      out int changePendedFlags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (Unshelve), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (shelvesetName), (object) shelvesetName);
        methodInformation.AddParameter(nameof (shelvesetOwner), (object) shelvesetOwner);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>("itemAttributeFilters", (IList<string>) itemAttrbuteFilters);
        methodInformation.AddArrayParameter<string>(nameof (shelvesetPropertyNameFilters), (IList<string>) shelvesetPropertyNameFilters);
        methodInformation.AddParameter(nameof (merge), (object) merge);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Unshelve(this.RequestContext, shelvesetName, shelvesetOwner, workspaceName, workspaceOwner, items, itemPropertyFilters, itemAttrbuteFilters, shelvesetPropertyNameFilters, merge, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        Shelveset shelveset = resource.Current<Shelveset>();
        resource.MoveNext();
        failures = resource.Current<List<Failure>>();
        resource.MoveNext();
        getOperations = resource.Current<StreamingCollection<GetOperation>>();
        resource.MoveNext();
        conflicts = resource.Current<StreamingCollection<Conflict>>();
        resource.MoveNext();
        changePendedFlags = (int) resource.Current<ChangePendedFlags>();
        return shelveset;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void AddConflict(
      string workspaceName,
      string ownerName,
      ConflictType conflictType,
      int itemId,
      int versionFrom,
      int pendingChangeId,
      string sourceLocalItem,
      string targetLocalItem,
      int reason,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddConflict), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (conflictType), (object) conflictType);
        methodInformation.AddParameter(nameof (itemId), (object) itemId);
        methodInformation.AddParameter(nameof (versionFrom), (object) versionFrom);
        methodInformation.AddParameter(nameof (pendingChangeId), (object) pendingChangeId);
        methodInformation.AddParameter(nameof (sourceLocalItem), (object) sourceLocalItem);
        methodInformation.AddParameter(nameof (targetLocalItem), (object) targetLocalItem);
        methodInformation.AddParameter(nameof (reason), (object) reason);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        this.VersionControlService.AddConflict(this.RequestContext, workspaceName, ownerName, conflictType, itemId, versionFrom, pendingChangeId, sourceLocalItem, targetLocalItem, reason, (PathLength) maxClientPathLength);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void DeleteBranchObject(ItemIdentifier rootItem, int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBranchObject), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        if (rootItem != null)
          methodInformation.AddParameter(nameof (rootItem), (object) rootItem);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        this.VersionControlService.DeleteBranchObject(this.RequestContext, rootItem, (PathLength) maxClientPathLength);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<BranchObject> QueryBranchObjects(
      ItemIdentifier item,
      RecursionType recursion,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBranchObjects), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (item), (object) item);
        methodInformation.AddParameter(nameof (recursion), (object) recursion);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryBranchObjects(this.RequestContext, item, recursion, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<BranchObject>>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<MergeCandidate> QueryMergeCandidates(
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      ItemSpec target,
      int options,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryMergeCandidates), MethodType.Normal, EstimatedMethodCost.Moderate, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (source), (object) source);
        methodInformation.AddParameter(nameof (target), (object) target);
        MergeOptionsEx mergeOptionsEx = (MergeOptionsEx) options;
        methodInformation.AddParameter(nameof (options), (object) mergeOptionsEx);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        return this.VersionControlService.QueryMergeCandidates(this.RequestContext, workspaceName, workspaceOwner, source, target, mergeOptionsEx, (PathLength) maxClientPathLength);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<GetOperation> Rollback(
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
      int maxClientPathLength,
      out StreamingCollection<Conflict> conflicts,
      out StreamingCollection<Failure> failures,
      out int changePendedFlags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (Rollback), MethodType.ReadWrite, EstimatedMethodCost.High);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (itemVersion), (object) itemVersion);
        methodInformation.AddParameter(nameof (from), (object) from);
        methodInformation.AddParameter(nameof (to), (object) to);
        methodInformation.AddParameter(nameof (lockLevel), (object) lockLevel);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        methodInformation.AddParameter("options", (object) (RollbackOptions) rollbackOptions);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        if (items != null)
          methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Rollback(this.RequestContext, workspaceName, workspaceOwner, items, itemVersion, from, to, rollbackOptions, lockLevel, itemPropertyFilters, itemAttributeFilters, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<GetOperation> streamingCollection = resource.Current<StreamingCollection<GetOperation>>();
        resource.MoveNext();
        conflicts = resource.Current<StreamingCollection<Conflict>>();
        resource.MoveNext();
        failures = resource.Current<StreamingCollection<Failure>>();
        resource.MoveNext();
        changePendedFlags = (int) resource.Current<ChangePendedFlags>();
        return streamingCollection;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<LabelResult> UnlabelItem(
      string workspaceName,
      string workspaceOwner,
      string labelName,
      string labelScope,
      ItemSpec[] items,
      VersionSpec version,
      int maxClientPathLength,
      out List<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UnlabelItem), MethodType.ReadWrite, EstimatedMethodCost.Moderate, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (labelName), (object) labelName);
        methodInformation.AddParameter(nameof (labelScope), (object) labelScope);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddParameter(nameof (version), (object) version);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.UnlabelItem(this.RequestContext, workspaceName, workspaceOwner, labelName, labelScope, items, version, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        List<LabelResult> labelResultList = resource.Current<List<LabelResult>>();
        resource.MoveNext();
        failures = resource.Current<List<Failure>>();
        return labelResultList;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<LabelResult> LabelItem(
      string workspaceName,
      string workspaceOwner,
      VersionControlLabel label,
      LabelItemSpec[] labelSpecs,
      LabelChildOption children,
      int maxClientPathLength,
      out List<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (LabelItem), MethodType.ReadWrite, EstimatedMethodCost.High, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter("labelName", (object) label?.Name);
        methodInformation.AddParameter("labelComment", (object) label?.Comment);
        methodInformation.AddArrayParameter<LabelItemSpec>(nameof (labelSpecs), (IList<LabelItemSpec>) labelSpecs);
        methodInformation.AddParameter(nameof (children), (object) children);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.LabelItem(this.RequestContext, workspaceName, workspaceOwner, label, labelSpecs, children, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        List<LabelResult> labelResultList = resource.Current<List<LabelResult>>();
        resource.MoveNext();
        failures = resource.Current<List<Failure>>();
        return labelResultList;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UpdateLocalVersion(
      string workspaceName,
      string ownerName,
      ServerItemLocalVersionUpdate[] updates,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateLocalVersion), MethodType.ReadWrite, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ServerItemLocalVersionUpdate>(nameof (updates), (IList<ServerItemLocalVersionUpdate>) updates);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        this.VersionControlService.UpdateLocalVersion(this.RequestContext, workspaceName, ownerName, (BaseLocalVersionUpdate[]) updates, (PathLength) maxClientPathLength);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public List<Failure> CheckPendingChanges(
      string workspaceName,
      string ownerName,
      string[] serverItems,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CheckPendingChanges), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<string>(nameof (serverItems), (IList<string>) serverItems);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        return this.VersionControlService.CheckPendingChanges(this.RequestContext, workspaceName, ownerName, serverItems, (PathLength) maxClientPathLength);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UpdatePendingState(
      string workspaceName,
      string workspaceOwner,
      PendingState[] updates,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdatePendingState), MethodType.ReadWrite, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<PendingState>(nameof (updates), (IList<PendingState>) updates);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        this.VersionControlService.UpdatePendingState(this.RequestContext, workspaceName, workspaceOwner, updates, (PathLength) maxClientPathLength);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void CreateCheckinNoteDefinition(
      string associatedServerItem,
      CheckinNoteFieldDefinition[] checkinNoteFields,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateCheckinNoteDefinition), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter("AssociatedServerItem", (object) associatedServerItem);
        methodInformation.AddArrayParameter<CheckinNoteFieldDefinition>(nameof (checkinNoteFields), (IList<CheckinNoteFieldDefinition>) checkinNoteFields);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        this.VersionControlService.CreateCheckinNoteDefinition(this.RequestContext, associatedServerItem, checkinNoteFields, (PathLength) maxClientPathLength);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void CreateTeamProjectFolder(
      TeamProjectFolderOptions teamProjectOptions,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateTeamProjectFolder), MethodType.Admin, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter("teamProject", (object) teamProjectOptions.TeamProject);
        methodInformation.AddParameter("sourceProject", (object) teamProjectOptions.SourceProject);
        methodInformation.AddParameter("comment", (object) teamProjectOptions.Comment);
        methodInformation.AddParameter("Exclusive", (object) teamProjectOptions.ExclusiveCheckout);
        methodInformation.AddParameter("GetLatestOnCheckout", (object) teamProjectOptions.GetLatestOnCheckout);
        methodInformation.AddArrayParameter<CheckinNoteFieldDefinition>("CheckinNotes", (IList<CheckinNoteFieldDefinition>) teamProjectOptions.CheckinNoteDefinition);
        methodInformation.AddArrayParameter<TeamProjectFolderPermission>("Permissions", (IList<TeamProjectFolderPermission>) teamProjectOptions.Permissions);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        this.VersionControlService.CreateTeamProjectFolder(this.RequestContext, teamProjectOptions, (PathLength) maxClientPathLength);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void UpdateBranchObject(
      BranchProperties branchProperties,
      bool updateExisting,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateBranchObject), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (updateExisting), (object) updateExisting);
        if (branchProperties != null)
        {
          methodInformation.AddParameter("branchProperties.RootItem", (object) branchProperties.RootItem);
          methodInformation.AddParameter("branchProperties.Description", (object) branchProperties.Description);
          methodInformation.AddParameter("branchProperties.Owner", (object) branchProperties.Owner);
          methodInformation.AddArrayParameter<Mapping>("branchProperties.BranchMappings", (IList<Mapping>) branchProperties.BranchMappings);
          if (branchProperties.ParentBranch != null)
            methodInformation.AddParameter("branchProperties.ParentBranch", (object) branchProperties.ParentBranch);
        }
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        this.VersionControlService.UpdateBranchObject(this.RequestContext, branchProperties, updateExisting, (PathLength) maxClientPathLength);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<VersionControlLabel> QueryLabels(
      string workspaceName,
      string workspaceOwner,
      string labelName,
      string labelScope,
      string owner,
      string filterItem,
      VersionSpec versionFilterItem,
      bool includeItems,
      bool generateDownloadUrls,
      int maxClientPathLength)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryLabels), MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (labelName), (object) labelName);
        methodInformation.AddParameter(nameof (labelScope), (object) labelScope);
        methodInformation.AddParameter(nameof (owner), (object) owner);
        methodInformation.AddParameter(nameof (filterItem), (object) filterItem);
        methodInformation.AddParameter(nameof (versionFilterItem), (object) versionFilterItem);
        methodInformation.AddParameter(nameof (includeItems), (object) includeItems);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryLabels(this.RequestContext, workspaceName, workspaceOwner, labelName, labelScope, owner, filterItem, versionFilterItem, includeItems, generateDownloadUrls, (PathLength) maxClientPathLength);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<VersionControlLabel>>();
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<PendingSet> QueryPendingSets(
      string localWorkspaceName,
      string localWorkspaceOwner,
      string queryWorkspaceName,
      string ownerName,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      string[] itemPropertyFilters,
      int maxClientPathLength,
      out StreamingCollection<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryPendingSets), MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (localWorkspaceName), (object) localWorkspaceName);
        methodInformation.AddParameter(nameof (localWorkspaceOwner), (object) localWorkspaceOwner);
        methodInformation.AddParameter(nameof (queryWorkspaceName), (object) queryWorkspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (itemSpecs), (IList<ItemSpec>) itemSpecs);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryPendingSets(this.RequestContext, localWorkspaceName, localWorkspaceOwner, queryWorkspaceName, ownerName, itemSpecs, generateDownloadUrls, (PathLength) maxClientPathLength, itemPropertyFilters: itemPropertyFilters);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<PendingSet> streamingCollection = resource.Current<StreamingCollection<PendingSet>>();
        resource.MoveNext();
        failures = resource.Current<StreamingCollection<Failure>>();
        return streamingCollection;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public StreamingCollection<PendingSet> QueryPendingSetsWithLocalWorkspaces(
      string localWorkspaceName,
      string localWorkspaceOwner,
      string queryWorkspaceName,
      string ownerName,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      string[] itemPropertyFilters,
      int maxClientPathLength,
      out StreamingCollection<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryPendingSetsWithLocalWorkspaces), MethodType.Normal, EstimatedMethodCost.Moderate, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (localWorkspaceName), (object) localWorkspaceName);
        methodInformation.AddParameter(nameof (localWorkspaceOwner), (object) localWorkspaceOwner);
        methodInformation.AddParameter(nameof (queryWorkspaceName), (object) queryWorkspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (itemSpecs), (IList<ItemSpec>) itemSpecs);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddParameter(nameof (maxClientPathLength), (object) maxClientPathLength);
        this.EnterMethod(methodInformation);
        maxClientPathLength = Validation.ValidatePathLength(maxClientPathLength, nameof (maxClientPathLength));
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryPendingSets(this.RequestContext, localWorkspaceName, localWorkspaceOwner, queryWorkspaceName, ownerName, itemSpecs, generateDownloadUrls, (PathLength) maxClientPathLength, false, itemPropertyFilters, true);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<PendingSet> streamingCollection = resource.Current<StreamingCollection<PendingSet>>();
        resource.MoveNext();
        failures = resource.Current<StreamingCollection<Failure>>();
        return streamingCollection;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
