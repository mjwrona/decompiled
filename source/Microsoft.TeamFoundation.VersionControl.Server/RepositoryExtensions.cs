// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.RepositoryExtensions
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/ClientServices/03", Description = "DevOps VersionControl ClientServices web service")]
  [ProxyParentClass("VersionControlClientProxy")]
  [ClientService(ServiceName = "ISCCProvider3", CollectionServiceIdentifier = "ec9b0153-ee54-450e-b6e0-664ecb033c99")]
  public class RepositoryExtensions : VersionControlWebService
  {
    [WebMethod]
    public void DeleteBranchObject(ItemIdentifier rootItem)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteBranchObject), MethodType.ReadWrite, EstimatedMethodCost.Low);
        if (rootItem != null)
          methodInformation.AddParameter(nameof (rootItem), (object) rootItem);
        this.EnterMethod(methodInformation);
        this.VersionControlService.DeleteBranchObject(this.RequestContext, rootItem);
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
      RecursionType recursion)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBranchObjects), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (item), (object) item);
        methodInformation.AddParameter(nameof (recursion), (object) recursion);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryBranchObjects(this.RequestContext, item, recursion);
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
    public StreamingCollection<BranchObjectOwnership> QueryBranchObjectOwnership(
      int[] changesets,
      ItemSpec pathFilter)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBranchObjectOwnership), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changesets), (object) changesets);
        if (pathFilter != null)
          methodInformation.AddParameter(nameof (pathFilter), (object) pathFilter);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryBranchObjectOwnership(this.RequestContext, changesets, pathFilter);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<BranchObjectOwnership>>();
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
    public void UpdateBranchObject(BranchProperties branchProperties, bool updateExisting)
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
        this.EnterMethod(methodInformation);
        this.VersionControlService.UpdateBranchObject(this.RequestContext, branchProperties, updateExisting);
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
      out StreamingCollection<Failure> failures,
      bool deferCheckIn,
      int checkInTicket)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CheckIn), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<string>(nameof (serverItems), (IList<string>) serverItems);
        methodInformation.AddParameter(nameof (checkinOptions), (object) (CheckInOptions2) checkinOptions);
        methodInformation.AddParameter("deferCheckin", (object) deferCheckIn);
        methodInformation.AddParameter(nameof (checkInTicket), (object) checkInTicket);
        info?.RecordInformation(methodInformation);
        checkinNotificationInfo?.RecordInformation(methodInformation);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.CheckIn(this.RequestContext, workspaceName, ownerName, serverItems, info, checkinNotificationInfo, checkinOptions, deferCheckIn, checkInTicket, false);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        CheckinResult checkinResult = resource.Current<CheckinResult>();
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
    public void ResetCheckinDates(DateTime lastCheckinDate)
    {
      try
      {
        new MethodInformation(nameof (ResetCheckinDates), MethodType.ReadWrite, EstimatedMethodCost.Low).AddParameter(nameof (lastCheckinDate), (object) lastCheckinDate);
        this.VersionControlService.ResetCheckinDates(this.RequestContext, lastCheckinDate);
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
      out StreamingCollection<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CheckInShelveset), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (shelvesetName), (object) shelvesetName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (changesetOwner), (object) changesetOwner);
        methodInformation.AddParameter(nameof (checkinOptions), (object) (CheckInOptions2) checkinOptions);
        checkinNotificationInfo?.RecordInformation(methodInformation);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.CheckInShelveset(this.RequestContext, shelvesetName, ownerName, 0, changesetOwner, checkinNotificationInfo, checkinOptions, false);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        CheckinResult checkinResult = resource.Current<CheckinResult>();
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
    public ArtifactPropertyValue GetChangesetProperty(int changesetId, string[] propertyNameFilters)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetChangesetProperty), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changesetId), (object) changesetId);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.GetChangesetProperty(this.RequestContext, changesetId, propertyNameFilters);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<ArtifactPropertyValue>();
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
    public void SetChangesetProperty(int changesetId, PropertyValue[] propertyValues)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetChangesetProperty), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changesetId), (object) changesetId);
        methodInformation.AddArrayParameter<PropertyValue>(nameof (propertyValues), (IList<PropertyValue>) propertyValues);
        this.EnterMethod(methodInformation);
        this.VersionControlService.SetChangesetProperty(this.RequestContext, changesetId, propertyValues);
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
    public StreamingCollection<ArtifactPropertyValue> GetVersionedItemProperty(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      VersionSpec versionSpec,
      DeletedState deletedState,
      ItemType itemType,
      string[] propertyNameFilters)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetVersionedItemProperty), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (itemSpecs), (IList<ItemSpec>) itemSpecs);
        methodInformation.AddParameter(nameof (versionSpec), (object) versionSpec);
        methodInformation.AddParameter(nameof (deletedState), (object) deletedState);
        methodInformation.AddParameter(nameof (itemType), (object) itemType);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.GetVersionedItemProperty(this.RequestContext, workspaceName, workspaceOwner, itemSpecs, versionSpec, deletedState, itemType, propertyNameFilters);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<ArtifactPropertyValue>>();
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
    public void SetVersionedItemProperty(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      VersionSpec versionSpec,
      DeletedState deletedState,
      ItemType itemType,
      PropertyValue[] propertyValues)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("SetChangesetProperty", MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (itemSpecs), (IList<ItemSpec>) itemSpecs);
        methodInformation.AddParameter(nameof (versionSpec), (object) versionSpec);
        methodInformation.AddParameter(nameof (deletedState), (object) deletedState);
        methodInformation.AddParameter(nameof (itemType), (object) itemType);
        methodInformation.AddArrayParameter<PropertyValue>(nameof (propertyValues), (IList<PropertyValue>) propertyValues);
        this.EnterMethod(methodInformation);
        this.VersionControlService.SetVersionedItemAttribute(this.RequestContext, workspaceName, workspaceOwner, itemSpecs, versionSpec, deletedState, itemType, propertyValues);
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
    public Changeset QueryChangesetExtended(
      int changesetId,
      bool includeChanges,
      bool generateDownloadUrls,
      string[] propertyNameFilters)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryChangesetExtended), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changesetId), (object) changesetId);
        methodInformation.AddParameter(nameof (includeChanges), (object) includeChanges);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryChangesetExtended(this.RequestContext, changesetId, includeChanges, generateDownloadUrls, propertyNameFilters);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<Changeset>();
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
    public StreamingCollection<Change> QueryChangesForChangeset(
      int changesetId,
      bool generateDownloadUrls,
      int pageSize,
      ItemSpec lastItem,
      string[] propertyNameFilters,
      bool includeMergeSourceInfo)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryChangesForChangeset), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changesetId), (object) changesetId);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        methodInformation.AddParameter(nameof (includeMergeSourceInfo), (object) includeMergeSourceInfo);
        if (lastItem != null)
          methodInformation.AddParameter(nameof (lastItem), (object) lastItem);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryChangesForChangeset(this.RequestContext, changesetId, generateDownloadUrls, pageSize, lastItem, propertyNameFilters, includeMergeSourceInfo);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<Change>>();
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
    public List<StreamingCollection<GetOperation>> Get(
      string workspaceName,
      string ownerName,
      GetRequest[] requests,
      int maxResults,
      int options,
      string[] propertyNameFilters)
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
        MethodInformation methodInformation = new MethodInformation(nameof (Get), MethodType.Normal, estimatedCost);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<GetRequest>(nameof (requests), (IList<GetRequest>) requests);
        methodInformation.AddParameter(nameof (maxResults), (object) maxResults);
        GetOptions getOptions = (GetOptions) options;
        methodInformation.AddParameter(nameof (options), (object) getOptions);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Get(this.RequestContext, workspaceName, ownerName, requests, maxResults, getOptions, propertyNameFilters);
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
    public StreamingCollection<GetOperation> Merge(
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      ItemSpec target,
      VersionSpec from,
      VersionSpec to,
      LockLevel lockLevel,
      int optionsEx,
      out StreamingCollection<Failure> failures,
      out StreamingCollection<Conflict> conflicts,
      string[] propertyNameFilters,
      out int changePendedFlags)
    {
      try
      {
        EstimatedMethodCost estimatedCost = EstimatedMethodCost.Low;
        if (source != null && source.RecursionType == RecursionType.Full)
          estimatedCost = EstimatedMethodCost.VeryHigh;
        MethodInformation methodInformation = new MethodInformation(nameof (Merge), MethodType.ReadWrite, estimatedCost);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (source), (object) source);
        methodInformation.AddParameter(nameof (target), (object) target);
        MergeOptionsEx mergeOptionsEx = (MergeOptionsEx) optionsEx;
        methodInformation.AddParameter("options", (object) mergeOptionsEx);
        methodInformation.AddParameter(nameof (lockLevel), (object) lockLevel);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        if (from != null)
          methodInformation.AddParameter(nameof (from), (object) from);
        if (to != null)
          methodInformation.AddParameter(nameof (to), (object) to);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Merge(this.RequestContext, workspaceName, workspaceOwner, source, target, from, to, lockLevel, mergeOptionsEx, propertyNameFilters);
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
    public StreamingCollection<PendingChange> QueryPendingChangesForWorkspace(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      int pageSize,
      string lastChange,
      bool includeMergeInfo,
      out StreamingCollection<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryPendingChangesForWorkspace), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (itemSpecs), (IList<ItemSpec>) itemSpecs);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddParameter(nameof (lastChange), (object) lastChange);
        methodInformation.AddParameter(nameof (includeMergeInfo), (object) includeMergeInfo);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryPendingChangesForWorkspace(this.RequestContext, workspaceName, workspaceOwner, itemSpecs, generateDownloadUrls, pageSize, lastChange, includeMergeInfo);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<PendingChange> streamingCollection = resource.Current<StreamingCollection<PendingChange>>();
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
    public StreamingCollection<GetOperation> UndoPendingChanges(
      string workspaceName,
      string ownerName,
      ItemSpec[] items,
      out List<Failure> failures,
      string[] propertyNameFilters,
      out int changePendedFlags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UndoPendingChanges), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.UndoPendingChanges(this.RequestContext, workspaceName, ownerName, items, (string[]) null, propertyNameFilters);
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
      out List<Failure> failures,
      out StreamingCollection<GetOperation> getOperations,
      string[] propertyNameFilters)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (Unshelve), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (shelvesetName), (object) shelvesetName);
        methodInformation.AddParameter(nameof (shelvesetOwner), (object) shelvesetOwner);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Unshelve(this.RequestContext, shelvesetName, shelvesetOwner, workspaceName, workspaceOwner, items, propertyNameFilters);
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
    public StreamingCollection<GetOperation> Resolve(
      string workspaceName,
      string ownerName,
      int conflictId,
      Resolution resolution,
      string newPath,
      int encoding,
      LockLevel lockLevel,
      out StreamingCollection<GetOperation> undoOperations,
      out StreamingCollection<Conflict> resolvedConflicts,
      string[] propertyNameFilters,
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
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Resolve(this.RequestContext, workspaceName, ownerName, conflictId, resolution, newPath, encoding, lockLevel, propertyNameFilters);
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
    public StreamingCollection<GetOperation> PendChanges(
      string workspaceName,
      string ownerName,
      ChangeRequest[] changes,
      int pendChangesOptions,
      int supportedFeatures,
      out List<Failure> failures,
      string[] propertyNameFilters,
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
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.PendChanges(this.RequestContext, workspaceName, ownerName, changes, pendChangesOptions, supportedFeatures, (string[]) null, propertyNameFilters);
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
    public StreamingCollection<Failure> SetPendingChangeProperty(
      string workspaceName,
      string workspaceOwner,
      ArtifactPropertyValue[] pendingChangePropertyValues)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetPendingChangeProperty), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ArtifactPropertyValue>(nameof (pendingChangePropertyValues), (IList<ArtifactPropertyValue>) pendingChangePropertyValues);
        this.EnterMethod(methodInformation);
        return new StreamingCollection<Failure>()
        {
          HandleExceptions = false
        };
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
    public ProxyInfo[] QueryProxies(string[] proxyUrls)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryProxies), MethodType.Normal, EstimatedMethodCost.VeryLow);
        methodInformation.AddArrayParameter<string>(nameof (proxyUrls), (IList<string>) proxyUrls);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryProxies(this.RequestContext, proxyUrls);
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
    public void AddProxy(ProxyInfo proxy)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (AddProxy), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (proxy), (object) proxy);
        this.EnterMethod(methodInformation);
        this.VersionControlService.AddProxy(this.RequestContext, proxy);
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
    public void DeleteProxy(string proxyUrl)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteProxy), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (proxyUrl), (object) proxyUrl);
        this.EnterMethod(methodInformation);
        this.VersionControlService.DeleteProxy(this.RequestContext, proxyUrl);
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
    [XmlInclude(typeof (ChangesetVersionSpec))]
    [XmlInclude(typeof (DateVersionSpec))]
    [XmlInclude(typeof (LabelVersionSpec))]
    [XmlInclude(typeof (LatestVersionSpec))]
    [XmlInclude(typeof (WorkspaceVersionSpec))]
    public StreamingCollection<ExtendedMerge> QueryMergesExtended(
      string workspaceName,
      string workspaceOwner,
      ItemSpec target,
      VersionSpec versionTarget,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      int options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryMergesExtended), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        QueryMergesExtendedOptions mergesExtendedOptions = (QueryMergesExtendedOptions) options;
        methodInformation.AddParameter("qmOptions", (object) mergesExtendedOptions);
        this.EnterMethod(methodInformation);
        if (versionFrom != null)
          methodInformation.AddParameter(nameof (versionFrom), (object) versionFrom);
        if (versionTo != null)
          methodInformation.AddParameter(nameof (versionTo), (object) versionTo);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryMergesExtended(this.RequestContext, workspaceName, workspaceOwner, target, versionTarget, versionFrom, versionTo, mergesExtendedOptions);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<ExtendedMerge>>();
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
    public StreamingCollection<ExtendedMerge> TrackMerges(
      int[] sourceChangesets,
      ItemIdentifier sourceItem,
      List<ItemIdentifier> targetItems,
      ItemSpec pathFilter,
      out StreamingCollection<string> partialTargetItems)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (TrackMerges), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddArrayParameter<int>(nameof (sourceChangesets), (IList<int>) sourceChangesets);
        methodInformation.AddParameter(nameof (sourceItem), (object) sourceItem);
        methodInformation.AddArrayParameter<ItemIdentifier>(nameof (targetItems), (IList<ItemIdentifier>) targetItems);
        methodInformation.AddParameter(nameof (pathFilter), (object) pathFilter);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.TrackMerges(this.RequestContext, sourceChangesets, sourceItem, targetItems, pathFilter);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        StreamingCollection<ExtendedMerge> streamingCollection = resource.Current<StreamingCollection<ExtendedMerge>>();
        resource.MoveNext();
        partialTargetItems = resource.Current<StreamingCollection<string>>();
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
    public StreamingCollection<ItemIdentifier> QueryMergeRelationships(string serverItem)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryMergeRelationships), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (serverItem), (object) serverItem);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryMergeRelationships(this.RequestContext, serverItem);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<ItemIdentifier>>();
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
      out StreamingCollection<Conflict> conflicts,
      out StreamingCollection<Failure> failures,
      string[] propertyNameFilters,
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
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        methodInformation.AddParameter("options", (object) (RollbackOptions) rollbackOptions);
        if (items != null)
          methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Rollback(this.RequestContext, workspaceName, workspaceOwner, items, itemVersion, from, to, rollbackOptions, lockLevel, propertyNameFilters);
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
  }
}
