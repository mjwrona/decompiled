// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Repository4
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
  [ClientService(ServiceName = "ISCCProvider4", CollectionServiceIdentifier = "FA9FCC37-F9BD-496F-A1B8-CE351F6BFE8A")]
  public class Repository4 : VersionControlWebService
  {
    private static readonly string s_queryItemExtendedRegistryPath = "/Configuration/VersionControl/QueryItemsExtended/TimeoutMinutes";
    private static readonly RegistryQuery s_durationQuery = new RegistryQuery(Repository4.s_queryItemExtendedRegistryPath);

    [WebMethod]
    public CheckinResult CreateBranch(
      string sourcePath,
      string targetPath,
      VersionSpec version,
      Changeset info,
      CheckinNotificationInfo checkinNotificationInfo,
      List<Mapping> mappings,
      out StreamingCollection<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateBranch), MethodType.ReadWrite, EstimatedMethodCost.High, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (sourcePath), (object) sourcePath);
        methodInformation.AddParameter(nameof (targetPath), (object) targetPath);
        methodInformation.AddParameter(nameof (version), (object) version);
        methodInformation.AddArrayParameter<Mapping>(nameof (mappings), (IList<Mapping>) mappings);
        info?.RecordInformation(methodInformation);
        checkinNotificationInfo?.RecordInformation(methodInformation);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.CreateBranch(this.RequestContext, sourcePath, targetPath, version, info, checkinNotificationInfo, mappings);
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
      int checkInTicket)
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
        info?.RecordInformation(methodInformation);
        checkinNotificationInfo?.RecordInformation(methodInformation);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.CheckIn(this.RequestContext, workspaceName, ownerName, serverItems, info, checkinNotificationInfo, checkinOptions, deferCheckIn, checkInTicket);
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
        checkinNotificationInfo?.RecordInformation(methodInformation);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.CheckInShelveset(this.RequestContext, shelvesetName, ownerName, changesetOwner, checkinNotificationInfo, checkinOptions);
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
    public Guid QueryPendingChangeSignature(string workspaceName, string ownerName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryPendingChangeSignature), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryPendingChangeSignature(this.RequestContext, workspaceName, ownerName);
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.PendChanges(this.RequestContext, workspaceName, ownerName, changes, pendChangesOptions, supportedFeatures, itemPropertyFilters, itemAttributeFilters, true);
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
    public StreamingCollection<PendingSet> QueryPendingSetsWithLocalWorkspaces(
      string localWorkspaceName,
      string localWorkspaceOwner,
      string queryWorkspaceName,
      string ownerName,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      string[] itemPropertyFilters,
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryPendingSets(this.RequestContext, localWorkspaceName, localWorkspaceOwner, queryWorkspaceName, ownerName, itemSpecs, generateDownloadUrls, false, itemPropertyFilters, true);
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
    public Workspace QueryWorkspace(string workspaceName, string ownerName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryWorkspace), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryWorkspace(this.RequestContext, workspaceName, ownerName, false, false, false, false);
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
    public List<Workspace> QueryWorkspaces(
      string ownerName,
      string computer,
      int permissionsFilter)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryWorkspaces), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (computer), (object) computer);
        methodInformation.AddParameter(nameof (permissionsFilter), (object) permissionsFilter);
        this.EnterMethod(methodInformation);
        List<Workspace> parameterArray = this.VersionControlService.QueryWorkspaces(this.RequestContext, ownerName, computer, permissionsFilter);
        methodInformation.AddArrayParameter<Workspace>("resultWorkspaces", (IList<Workspace>) parameterArray);
        return parameterArray;
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
      bool clearLocalVersionTable)
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.ReconcileLocalWorkspace(this.RequestContext, workspaceName, ownerName, pendingChangeSignature, pendingChanges, localVersionUpdates, clearLocalVersionTable);
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
    public StreamingCollection<GetOperation> UndoPendingChangesInLocalWorkspace(
      string workspaceName,
      string ownerName,
      ItemSpec[] items,
      out List<Failure> failures,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      out int changePendedFlags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation("UndoPendingChanges", MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.UndoPendingChanges(this.RequestContext, workspaceName, ownerName, items, itemPropertyFilters, itemAttributeFilters, false);
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Unshelve(this.RequestContext, shelvesetName, shelvesetOwner, workspaceName, workspaceOwner, items, itemPropertyFilters, itemAttrbuteFilters, shelvesetPropertyNameFilters, merge);
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
    public void UpdateLocalVersion(
      string workspaceName,
      string ownerName,
      ServerItemLocalVersionUpdate[] updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateLocalVersion), MethodType.ReadWrite, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ServerItemLocalVersionUpdate>(nameof (updates), (IList<ServerItemLocalVersionUpdate>) updates);
        this.EnterMethod(methodInformation);
        this.VersionControlService.UpdateLocalVersion(this.RequestContext, workspaceName, ownerName, (BaseLocalVersionUpdate[]) updates);
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
    public ServerSettings GetServerSettings()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetServerSettings), MethodType.LightWeight, EstimatedMethodCost.Free));
        return this.VersionControlService.GetServerSettings(this.RequestContext);
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
    public void SetServerSettings(ServerSettings settings)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetServerSettings), MethodType.ReadWrite, EstimatedMethodCost.Low);
        if (settings != null)
        {
          methodInformation.AddParameter("settings.DefaultWorkspaceLocation", (object) settings.DefaultWorkspaceLocationEnum);
          methodInformation.AddParameter("settings.DefaultLocalItemExclusionSet", (object) settings.DefaultLocalItemExclusionSet);
        }
        this.EnterMethod(methodInformation);
        this.VersionControlService.SetServerSettings(this.RequestContext, settings);
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
    public void UpdateShelveset(string shelvesetName, string ownerName, Shelveset updatedShelveset)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateShelveset), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (shelvesetName), (object) shelvesetName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        string parameterValue = updatedShelveset == null ? "<null>" : updatedShelveset.Name + ";" + updatedShelveset.Owner;
        methodInformation.AddParameter("shelveset", (object) parameterValue);
        this.EnterMethod(methodInformation);
        this.VersionControlService.UpdateShelveset(this.RequestContext, shelvesetName, ownerName, updatedShelveset);
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
    public List<Shelveset> QueryShelvesets(
      string shelvesetName,
      string ownerName,
      string[] propertyNameFilters)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryShelvesets), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (shelvesetName), (object) shelvesetName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<string>(nameof (propertyNameFilters), (IList<string>) propertyNameFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryShelvesets(this.RequestContext, shelvesetName, ownerName, propertyNameFilters);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<List<Shelveset>>();
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
    public StreamingCollection<WorkspaceItemSet> QueryWorkspaceItems(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      DeletedState deletedState,
      ItemType itemType,
      bool generateDownloadUrls,
      int options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryWorkspaceItems), MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddParameter(nameof (deletedState), (object) deletedState);
        methodInformation.AddParameter(nameof (itemType), (object) itemType);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryWorkspaceItems(this.RequestContext, workspaceName, workspaceOwner, items, deletedState, itemType, generateDownloadUrls, options);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<WorkspaceItemSet>>();
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryPendingSets(this.RequestContext, localWorkspaceName, localWorkspaceOwner, queryWorkspaceName, ownerName, itemSpecs, generateDownloadUrls, itemPropertyFilters: itemPropertyFilters);
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
    public StreamingCollection<PendingChange> QueryPendingChangesForWorkspace(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      int pageSize,
      string lastChange,
      bool includeMergeInfo,
      string[] itemPropertyFilters,
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
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryPendingChangesForWorkspace(this.RequestContext, workspaceName, workspaceOwner, itemSpecs, generateDownloadUrls, pageSize, lastChange, includeMergeInfo, itemPropertyFilters: itemPropertyFilters);
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
    public StreamingCollection<PendingSet> QueryShelvedChanges(
      string localWorkspaceName,
      string localWorkspaceOwner,
      string shelvesetName,
      string ownerName,
      ItemSpec[] itemSpecs,
      bool generateDownloadUrls,
      string[] itemPropertyFilters,
      out StreamingCollection<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryShelvedChanges), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (localWorkspaceName), (object) localWorkspaceName);
        methodInformation.AddParameter(nameof (localWorkspaceOwner), (object) localWorkspaceOwner);
        methodInformation.AddParameter(nameof (shelvesetName), (object) shelvesetName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (itemSpecs), (IList<ItemSpec>) itemSpecs);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryShelvedChanges(this.RequestContext, localWorkspaceName, localWorkspaceOwner, shelvesetName, ownerName, itemSpecs, generateDownloadUrls, itemPropertyFilters);
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
    public List<StreamingCollection<ExtendedItem>> QueryItemsExtended(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      DeletedState deletedState,
      ItemType itemType,
      int options,
      string[] itemPropertyFilters)
    {
      try
      {
        int timeoutMinutes = this.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.RequestContext, in Repository4.s_durationQuery, 5);
        MethodInformation methodInformation = new MethodInformation(nameof (QueryItemsExtended), MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes((double) timeoutMinutes));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddParameter(nameof (deletedState), (object) deletedState);
        methodInformation.AddParameter(nameof (itemType), (object) itemType);
        methodInformation.AddParameter(nameof (options), (object) options);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryItemsExtended(this.RequestContext, workspaceName, workspaceOwner, items, deletedState, itemType, options, itemPropertyFilters, timeoutMinutes);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<List<StreamingCollection<ExtendedItem>>>();
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
    public StreamingCollection<ItemSet> QueryItems(
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
      try
      {
        string webMethodName = nameof (QueryItems);
        foreach (ItemSpec itemSpec in items)
        {
          if (itemSpec.RecursionType == RecursionType.Full && VersionControlPath.Equals(itemSpec.Item, "$/"))
          {
            webMethodName = "QueryItems_FullRecursionAtRoot";
            break;
          }
          if (itemSpec.RecursionType == RecursionType.Full)
            webMethodName = "QueryItem_FullRecursion";
        }
        MethodInformation methodInformation = new MethodInformation(webMethodName, MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddParameter(nameof (version), (object) version);
        methodInformation.AddParameter(nameof (deletedState), (object) deletedState);
        methodInformation.AddParameter(nameof (itemType), (object) itemType);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddParameter(nameof (options), (object) options);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryItems(this.RequestContext, workspaceName, workspaceOwner, items, version, deletedState, itemType, generateDownloadUrls, options, itemPropertyFilters, itemAttributeFilters);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<ItemSet>>();
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Resolve(this.RequestContext, workspaceName, ownerName, conflictId, resolution, newPath, encoding, lockLevel, newProperties, itemPropertyFilters, itemAttributeFilters, false);
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
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.PendChanges(this.RequestContext, workspaceName, ownerName, changes, pendChangesOptions, supportedFeatures, itemPropertyFilters, itemAttributeFilters);
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
        if (from != null)
          methodInformation.AddParameter(nameof (from), (object) from);
        if (to != null)
          methodInformation.AddParameter(nameof (to), (object) to);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Merge(this.RequestContext, workspaceName, workspaceOwner, source, target, from, to, lockLevel, mergeOptionsEx, itemAttributeFilters, itemPropertyFilters);
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
        if (items != null)
          methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Rollback(this.RequestContext, workspaceName, workspaceOwner, items, itemVersion, from, to, rollbackOptions, lockLevel, itemPropertyFilters, itemAttributeFilters);
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
    public StreamingCollection<GetOperation> UndoPendingChanges(
      string workspaceName,
      string ownerName,
      ItemSpec[] items,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      out List<Failure> failures,
      out int changePendedFlags)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UndoPendingChanges), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.UndoPendingChanges(this.RequestContext, workspaceName, ownerName, items, itemPropertyFilters, itemAttributeFilters);
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
    public List<StreamingCollection<GetOperation>> Get(
      string workspaceName,
      string ownerName,
      GetRequest[] requests,
      int maxResults,
      int options,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters)
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Get(this.RequestContext, workspaceName, ownerName, requests, maxResults, getOptions, itemPropertyFilters, itemAttributeFilters);
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
    public StreamingCollection<Change> QueryChangesForChangeset(
      int changesetId,
      bool generateDownloadUrls,
      int pageSize,
      ItemSpec lastItem,
      string[] itemPropertyFilters,
      string[] itemAttributeFilters,
      bool includeMergeSourceInfo)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryChangesForChangeset), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changesetId), (object) changesetId);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        methodInformation.AddParameter(nameof (includeMergeSourceInfo), (object) includeMergeSourceInfo);
        if (lastItem != null)
          methodInformation.AddParameter(nameof (lastItem), (object) lastItem);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryChangesForChangeset(this.RequestContext, changesetId, generateDownloadUrls, pageSize, lastItem, itemPropertyFilters, itemAttributeFilters, includeMergeSourceInfo);
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
    public Changeset QueryChangesetExtended(
      int changesetId,
      bool includeChanges,
      bool generateDownloadUrls,
      string[] changesetPropertyFilters,
      string[] itemAttributeFilters,
      string[] itemPropertyFilters)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryChangesetExtended), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changesetId), (object) changesetId);
        methodInformation.AddParameter(nameof (includeChanges), (object) includeChanges);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddArrayParameter<string>(nameof (changesetPropertyFilters), (IList<string>) changesetPropertyFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemAttributeFilters), (IList<string>) itemAttributeFilters);
        methodInformation.AddArrayParameter<string>(nameof (itemPropertyFilters), (IList<string>) itemPropertyFilters);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryChangesetExtended(this.RequestContext, changesetId, includeChanges, generateDownloadUrls, changesetPropertyFilters, itemAttributeFilters, itemPropertyFilters);
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
    public StreamingCollection<Changeset> CompareLabels(
      string startLabelName,
      string startLabelScope,
      string endLabelName,
      string endLabelScope,
      int minChangeSet,
      int maxCount)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CompareLabels), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (startLabelName), (object) startLabelName);
        methodInformation.AddParameter(nameof (startLabelScope), (object) startLabelScope);
        methodInformation.AddParameter(nameof (endLabelName), (object) endLabelName);
        methodInformation.AddParameter(nameof (endLabelScope), (object) endLabelScope);
        methodInformation.AddParameter(nameof (minChangeSet), (object) minChangeSet);
        methodInformation.AddParameter(nameof (maxCount), (object) maxCount);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.CompareLabels(this.RequestContext, startLabelName, startLabelScope, endLabelName, endLabelScope, minChangeSet, maxCount);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<Changeset>>();
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
