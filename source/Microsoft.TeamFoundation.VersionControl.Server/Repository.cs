// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Repository
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
  [ClientService(ServiceName = "ISCCProvider", CollectionServiceIdentifier = "b2b178f5-bef9-460d-a5cf-35bcc0281cc4")]
  public class Repository : VersionControlWebService
  {
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
      int reason)
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
        this.EnterMethod(methodInformation);
        this.VersionControlService.AddConflict(this.RequestContext, workspaceName, ownerName, conflictType, itemId, versionFrom, pendingChangeId, sourceLocalItem, targetLocalItem, reason);
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
    public string CheckAuthentication()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (CheckAuthentication), MethodType.LightWeight, EstimatedMethodCost.VeryLow));
        return this.RequestContext.DomainUserName;
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
      CheckinOptions checkinOptions,
      out StreamingCollection<Failure> failures,
      bool deferCheckIn,
      int checkInTicket)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CheckIn), MethodType.ReadWrite, EstimatedMethodCost.High, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<string>(nameof (serverItems), (IList<string>) serverItems);
        methodInformation.AddParameter(nameof (checkinOptions), (object) checkinOptions);
        methodInformation.AddParameter("deferCheckin", (object) deferCheckIn);
        methodInformation.AddParameter(nameof (checkInTicket), (object) checkInTicket);
        info?.RecordInformation(methodInformation);
        checkinNotificationInfo?.RecordInformation(methodInformation);
        this.EnterMethod(methodInformation);
        CheckInOptions2 checkinOptions1 = CheckInOptions2.QueueBuildForGatedCheckIn;
        if ((checkinOptions & CheckinOptions.SuppressEvent) == CheckinOptions.SuppressEvent)
          checkinOptions1 |= CheckInOptions2.SuppressEvent;
        if ((checkinOptions & CheckinOptions.ValidateCheckinOwner) == CheckinOptions.ValidateCheckinOwner)
          checkinOptions1 |= CheckInOptions2.ValidateCheckInOwner;
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.CheckIn(this.RequestContext, workspaceName, ownerName, serverItems, info, checkinNotificationInfo, (int) checkinOptions1, deferCheckIn, checkInTicket, false);
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
    public List<Failure> CheckPendingChanges(
      string workspaceName,
      string ownerName,
      string[] serverItems)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CheckPendingChanges), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<string>(nameof (serverItems), (IList<string>) serverItems);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.CheckPendingChanges(this.RequestContext, workspaceName, ownerName, serverItems);
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
    public void CreateAnnotation(
      string AnnotationName,
      string AnnotatedItem,
      int Version,
      string AnnotationValue,
      string Comment,
      bool Overwrite)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateAnnotation), MethodType.ReadWrite, EstimatedMethodCost.VeryLow);
        methodInformation.AddParameter(nameof (AnnotationName), (object) AnnotationName);
        methodInformation.AddParameter(nameof (AnnotatedItem), (object) AnnotatedItem);
        methodInformation.AddParameter(nameof (Version), (object) Version);
        methodInformation.AddParameter(nameof (AnnotationValue), (object) AnnotationValue);
        methodInformation.AddParameter(nameof (Comment), (object) Comment);
        methodInformation.AddParameter(nameof (Overwrite), (object) Overwrite);
        this.EnterMethod(methodInformation);
        this.VersionControlService.CreateAnnotation(this.RequestContext, AnnotationName, AnnotatedItem, Version, AnnotationValue, Comment, Overwrite);
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
      CheckinNoteFieldDefinition[] checkinNoteFields)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateCheckinNoteDefinition), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter("AssociatedServerItem", (object) associatedServerItem);
        methodInformation.AddArrayParameter<CheckinNoteFieldDefinition>(nameof (checkinNoteFields), (IList<CheckinNoteFieldDefinition>) checkinNoteFields);
        this.EnterMethod(methodInformation);
        this.VersionControlService.CreateCheckinNoteDefinition(this.RequestContext, associatedServerItem, checkinNoteFields);
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
    public Workspace CreateWorkspace(Workspace workspace)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (CreateWorkspace), MethodType.ReadWrite, EstimatedMethodCost.Low);
        if (workspace != null)
        {
          methodInformation.AddParameter("workspace.Name", (object) workspace.Name);
          methodInformation.AddParameter("workspace.OwnerName", (object) workspace.OwnerName);
        }
        this.EnterMethod(methodInformation);
        return this.VersionControlService.CreateWorkspace(this.RequestContext, workspace);
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
    public void DeleteAnnotation(
      string AnnotationName,
      string AnnotatedItem,
      int Version,
      string AnnotationValue)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteAnnotation), MethodType.ReadWrite, EstimatedMethodCost.VeryLow);
        methodInformation.AddParameter(nameof (AnnotationName), (object) AnnotationName);
        methodInformation.AddParameter(nameof (AnnotatedItem), (object) AnnotatedItem);
        methodInformation.AddParameter(nameof (Version), (object) Version);
        methodInformation.AddParameter(nameof (AnnotationValue), (object) AnnotationValue);
        this.EnterMethod(methodInformation);
        this.VersionControlService.DeleteAnnotation(this.RequestContext, AnnotationName, AnnotatedItem, Version, AnnotationValue);
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
    public List<LabelResult> DeleteLabel(string labelName, string labelScope)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteLabel), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (labelName), (object) labelName);
        methodInformation.AddParameter(nameof (labelScope), (object) labelScope);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.DeleteLabel(this.RequestContext, labelName, labelScope);
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
    public void DeleteShelveset(string shelvesetName, string ownerName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteShelveset), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (shelvesetName), (object) shelvesetName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        this.EnterMethod(methodInformation);
        this.VersionControlService.DeleteShelveset(this.RequestContext, shelvesetName, ownerName);
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
    public void DeleteWorkspace(string workspaceName, string ownerName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteWorkspace), MethodType.ReadWrite, EstimatedMethodCost.High, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        this.EnterMethod(methodInformation);
        this.VersionControlService.DeleteWorkspace(this.RequestContext, workspaceName, ownerName);
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Destroy(this.RequestContext, item, versionSpec, stopAtSpec, flags);
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
    public List<StreamingCollection<GetOperation>> Get(
      string workspaceName,
      string ownerName,
      GetRequest[] requests,
      bool force,
      bool noGet,
      int maxResults,
      int options)
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
        methodInformation.AddParameter(nameof (force), (object) force);
        methodInformation.AddParameter(nameof (noGet), (object) noGet);
        methodInformation.AddParameter(nameof (maxResults), (object) maxResults);
        GetOptions getOptions = (GetOptions) options;
        methodInformation.AddParameter(nameof (options), (object) getOptions);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          if (force)
            getOptions |= GetOptions.GetAll;
          if (noGet)
            getOptions |= GetOptions.Preview;
          resource = this.VersionControlService.Get(this.RequestContext, workspaceName, ownerName, requests, maxResults, getOptions, (string[]) null);
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
    public RepositoryProperties GetRepositoryProperties()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (GetRepositoryProperties), MethodType.LightWeight, EstimatedMethodCost.VeryLow));
        return this.VersionControlService.GetRepositoryProperties(this.RequestContext);
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.LabelItem(this.RequestContext, workspaceName, workspaceOwner, label, labelSpecs, children);
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
    public StreamingCollection<GetOperation> Merge(
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      ItemSpec target,
      VersionSpec from,
      VersionSpec to,
      MergeOptions options,
      LockLevel lockLevel,
      int optionsEx,
      out StreamingCollection<Failure> failures,
      out StreamingCollection<Conflict> conflicts)
    {
      try
      {
        EstimatedMethodCost estimatedCost = EstimatedMethodCost.Low;
        if (source != null && source.RecursionType == RecursionType.Full)
          estimatedCost = EstimatedMethodCost.VeryHigh;
        MethodInformation methodInformation = new MethodInformation(nameof (Merge), MethodType.ReadWrite, estimatedCost, TimeSpan.FromMinutes(60.0));
        optionsEx = (int) ((MergeOptions) optionsEx | options);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (source), (object) source);
        methodInformation.AddParameter(nameof (target), (object) target);
        MergeOptionsEx mergeOptionsEx = (MergeOptionsEx) optionsEx;
        methodInformation.AddParameter(nameof (options), (object) mergeOptionsEx);
        methodInformation.AddParameter(nameof (lockLevel), (object) lockLevel);
        if (from != null)
          methodInformation.AddParameter(nameof (from), (object) from);
        if (to != null)
          methodInformation.AddParameter(nameof (to), (object) to);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Merge(this.RequestContext, workspaceName, workspaceOwner, source, target, from, to, lockLevel, mergeOptionsEx, (string[]) null);
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
      out List<Failure> failures)
    {
      try
      {
        string webMethodName = nameof (PendChanges);
        if (changes != null && changes.Length != 0)
          webMethodName = webMethodName + "." + changes[0].RequestType.ToString();
        MethodInformation methodInformation = new MethodInformation(webMethodName, MethodType.ReadWrite, EstimatedMethodCost.Moderate, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (pendChangesOptions), (object) (PendChangesOptions) pendChangesOptions);
        methodInformation.AddParameter(nameof (supportedFeatures), (object) (SupportedFeatures) supportedFeatures);
        methodInformation.AddArrayParameter<ChangeRequest>(nameof (changes), (IList<ChangeRequest>) changes);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.PendChanges(this.RequestContext, workspaceName, ownerName, changes, pendChangesOptions, supportedFeatures, (string[]) null, (string[]) null);
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
    public List<Annotation> QueryAnnotation(
      string annotationName,
      string annotatedItem,
      int version)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryAnnotation), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter("AnnotationName", (object) annotationName);
        methodInformation.AddParameter("AnnotatedItem", (object) annotatedItem);
        methodInformation.AddParameter("Version", (object) version);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryAnnotation(this.RequestContext, annotationName, annotatedItem, version);
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
    public BranchRelative[][] QueryBranches(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      VersionSpec version)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryBranches), MethodType.Normal, EstimatedMethodCost.Moderate, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddParameter(nameof (version), (object) version);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryBranches(this.RequestContext, workspaceName, workspaceOwner, items, version);
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
    public Changeset QueryChangeset(
      int changesetId,
      bool includeChanges,
      bool generateDownloadUrls,
      bool includeSourceRenames)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryChangeset), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changesetId), (object) changesetId);
        methodInformation.AddParameter(nameof (includeChanges), (object) includeChanges);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddParameter(nameof (includeSourceRenames), (object) includeSourceRenames);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryChangeset(this.RequestContext, changesetId, includeChanges, generateDownloadUrls, includeSourceRenames);
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
      ItemSpec lastItem)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryChangesForChangeset), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changesetId), (object) changesetId);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddParameter(nameof (pageSize), (object) pageSize);
        if (lastItem != null)
          methodInformation.AddParameter(nameof (lastItem), (object) lastItem);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryChangesForChangeset(this.RequestContext, changesetId, generateDownloadUrls, pageSize, lastItem, (string[]) null, false);
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
    public List<CheckinNoteFieldDefinition> QueryCheckinNoteDefinition(string[] associatedServerItem)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryCheckinNoteDefinition), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (associatedServerItem), (IList<string>) associatedServerItem);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryCheckinNoteDefinition(this.RequestContext, associatedServerItem);
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
    public List<string> QueryCheckinNoteFieldNames()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (QueryCheckinNoteFieldNames), MethodType.Normal, EstimatedMethodCost.Low));
        return this.VersionControlService.QueryCheckinNoteFieldNames(this.RequestContext);
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
    public StreamingCollection<Conflict> QueryConflicts(
      string workspaceName,
      string ownerName,
      ItemSpec[] items)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryConflicts), MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryConflicts(this.RequestContext, workspaceName, ownerName, items);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<StreamingCollection<Conflict>>();
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
    public List<string> QueryEffectiveGlobalPermissions(string identityName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryEffectiveGlobalPermissions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (identityName), (object) identityName);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryEffectiveGlobalPermissions(this.RequestContext, identityName);
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
    public List<string> QueryEffectiveItemPermissions(
      string workspaceName,
      string workspaceOwner,
      string item,
      string identityName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryEffectiveItemPermissions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (item), (object) item);
        methodInformation.AddParameter(nameof (identityName), (object) identityName);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryEffectiveItemPermissions(this.RequestContext, workspaceName, workspaceOwner, item, identityName);
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
    public List<FileType> QueryFileTypes()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (QueryFileTypes), MethodType.Normal, EstimatedMethodCost.Low));
        return this.VersionControlService.QueryFileTypes(this.RequestContext);
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
    public GlobalSecurity QueryGlobalPermissions(string[] identityNames)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryGlobalPermissions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (identityNames), (IList<string>) identityNames);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryGlobalPermissions(this.RequestContext, identityNames);
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
    public List<Changeset> QueryHistory(
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
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryHistory), MethodType.Normal, EstimatedMethodCost.Moderate, TimeSpan.FromMinutes(60.0));
        if (workspaceOwner != null)
          methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        if (workspaceName != null)
          methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        if (user != null)
          methodInformation.AddParameter(nameof (user), (object) user);
        methodInformation.AddParameter(nameof (itemSpec), (object) itemSpec);
        methodInformation.AddParameter(nameof (versionItem), (object) versionItem);
        if (versionFrom != null)
          methodInformation.AddParameter(nameof (versionFrom), (object) versionFrom);
        if (versionTo != null)
          methodInformation.AddParameter(nameof (versionTo), (object) versionTo);
        methodInformation.AddParameter(nameof (maxCount), (object) maxCount);
        methodInformation.AddParameter(nameof (includeFiles), (object) includeFiles);
        methodInformation.AddParameter(nameof (slotMode), (object) slotMode);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddParameter(nameof (sortAscending), (object) sortAscending);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryHistory(this.RequestContext, workspaceName, workspaceOwner, itemSpec, versionItem, user, versionFrom, versionTo, maxCount, includeFiles, generateDownloadUrls, slotMode, sortAscending);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<List<Changeset>>();
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
      int options)
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
            webMethodName = "QueryItems_FullRecursion";
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryItems(this.RequestContext, workspaceName, workspaceOwner, items, version, deletedState, itemType, generateDownloadUrls, options);
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
    public List<StreamingCollection<LocalVersion>> QueryLocalVersions(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryLocalVersions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (itemSpecs), (IList<ItemSpec>) itemSpecs);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryLocalVersions(this.RequestContext, workspaceName, workspaceOwner, itemSpecs);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<List<StreamingCollection<LocalVersion>>>();
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
      int options)
    {
      try
      {
        int timeoutMinutes = 5;
        MethodInformation methodInformation = new MethodInformation(nameof (QueryItemsExtended), MethodType.Normal, EstimatedMethodCost.Low, TimeSpan.FromMinutes((double) timeoutMinutes));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        methodInformation.AddParameter(nameof (deletedState), (object) deletedState);
        methodInformation.AddParameter(nameof (itemType), (object) itemType);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryItemsExtended(this.RequestContext, workspaceName, workspaceOwner, items, deletedState, itemType, options, timeoutMinutes);
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
    public List<ItemSecurity> QueryItemPermissions(
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] itemSpecs,
      string[] identityNames,
      out List<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryItemPermissions), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (itemSpecs), (IList<ItemSpec>) itemSpecs);
        methodInformation.AddArrayParameter<string>(nameof (identityNames), (IList<string>) identityNames);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryItemPermissions(this.RequestContext, workspaceName, workspaceOwner, itemSpecs, identityNames);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        List<ItemSecurity> itemSecurityList = resource.Current<List<ItemSecurity>>();
        resource.MoveNext();
        failures = resource.Current<List<Failure>>();
        return itemSecurityList;
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
      bool generateDownloadUrls)
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryLabels(this.RequestContext, workspaceName, workspaceOwner, labelName, labelScope, owner, filterItem, versionFilterItem, includeItems, generateDownloadUrls);
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
    public List<MergeCandidate> QueryMergeCandidates(
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      ItemSpec target,
      int options)
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
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryMergeCandidates(this.RequestContext, workspaceName, workspaceOwner, source, target, mergeOptionsEx);
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
    public List<ChangesetMerge> QueryMerges(
      string workspaceName,
      string workspaceOwner,
      ItemSpec source,
      VersionSpec versionSource,
      ItemSpec target,
      VersionSpec versionTarget,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      int maxChangesets,
      bool showAll,
      out List<Changeset> changesets)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryMerges), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        if (source != null)
          methodInformation.AddParameter(nameof (source), (object) source);
        if (versionSource != null)
          methodInformation.AddParameter(nameof (versionSource), (object) versionSource);
        if (target != null)
          methodInformation.AddParameter(nameof (target), (object) target);
        if (versionTarget != null)
          methodInformation.AddParameter(nameof (versionTarget), (object) versionTarget);
        if (versionFrom != null)
          methodInformation.AddParameter(nameof (versionFrom), (object) versionFrom);
        if (versionTo != null)
          methodInformation.AddParameter(nameof (versionTo), (object) versionTo);
        methodInformation.AddParameter(nameof (maxChangesets), (object) maxChangesets);
        methodInformation.AddParameter(nameof (showAll), (object) showAll);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryMerges(this.RequestContext, workspaceName, workspaceOwner, source, versionSource, target, versionTarget, versionFrom, versionTo, maxChangesets, showAll);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        List<ChangesetMerge> changesetMergeList = resource.Current<List<ChangesetMerge>>();
        resource.MoveNext();
        changesets = resource.Current<List<Changeset>>();
        return changesetMergeList;
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
    public ChangesetMergeDetails QueryMergesWithDetails(
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
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryMergesWithDetails), MethodType.Normal, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        if (source != null)
          methodInformation.AddParameter(nameof (source), (object) source);
        if (versionSource != null)
          methodInformation.AddParameter(nameof (versionSource), (object) versionSource);
        if (target != null)
          methodInformation.AddParameter(nameof (target), (object) target);
        if (versionTarget != null)
          methodInformation.AddParameter(nameof (versionTarget), (object) versionTarget);
        if (versionFrom != null)
          methodInformation.AddParameter(nameof (versionFrom), (object) versionFrom);
        if (versionTo != null)
          methodInformation.AddParameter(nameof (versionTo), (object) versionTo);
        methodInformation.AddParameter(nameof (maxChangesets), (object) maxChangesets);
        methodInformation.AddParameter(nameof (showAll), (object) showAll);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryMergesWithDetails(this.RequestContext, workspaceName, workspaceOwner, source, versionSource, target, versionTarget, versionFrom, versionTo, maxChangesets, showAll);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        return resource.Current<ChangesetMergeDetails>();
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryPendingSets(this.RequestContext, localWorkspaceName, localWorkspaceOwner, queryWorkspaceName, ownerName, itemSpecs, generateDownloadUrls);
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryPendingChangesForWorkspace(this.RequestContext, workspaceName, workspaceOwner, itemSpecs, generateDownloadUrls, pageSize, lastChange, false);
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.QueryShelvedChanges(this.RequestContext, localWorkspaceName, localWorkspaceOwner, shelvesetName, ownerName, itemSpecs, generateDownloadUrls);
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
    public List<Shelveset> QueryShelvesets(string shelvesetName, string ownerName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryShelvesets), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (shelvesetName), (object) shelvesetName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.QueryShelvesets(this.RequestContext, shelvesetName, ownerName);
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
        return this.VersionControlService.QueryWorkspace(this.RequestContext, workspaceName, ownerName, false, true, true);
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
        for (int index = parameterArray.Count - 1; index >= 0; --index)
        {
          if (parameterArray[index].IsLocal || (parameterArray[index].Options & 1) != 0)
            parameterArray.RemoveAt(index);
        }
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
    public void RefreshIdentityDisplayName()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (RefreshIdentityDisplayName), MethodType.ReadWrite, EstimatedMethodCost.VeryLow));
        this.VersionControlService.RefreshIdentityDisplayName(this.RequestContext);
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
    public void RemoveLocalConflict(string workspaceName, string ownerName, int conflictId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (RemoveLocalConflict), MethodType.ReadWrite, EstimatedMethodCost.VeryLow);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter(nameof (conflictId), (object) conflictId);
        this.EnterMethod(methodInformation);
        this.VersionControlService.RemoveLocalConflict(this.RequestContext, workspaceName, ownerName, conflictId);
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
      out StreamingCollection<Conflict> resolvedConflicts)
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Resolve(this.RequestContext, workspaceName, ownerName, conflictId, resolution, newPath, encoding, lockLevel, (string[]) null);
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
    public void SetFileTypes(FileType[] fileTypes)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (SetFileTypes), MethodType.Admin, EstimatedMethodCost.VeryLow);
        methodInformation.AddArrayParameter<FileType>(nameof (fileTypes), (IList<FileType>) fileTypes);
        this.EnterMethod(methodInformation);
        this.VersionControlService.SetFileTypes(this.RequestContext, fileTypes);
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
      bool replace)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (Shelve), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<string>(nameof (serverItems), (IList<string>) serverItems);
        methodInformation.AddParameter(nameof (shelveset), (object) (shelveset.Name + ";" + shelveset.Owner));
        methodInformation.AddParameter(nameof (replace), (object) replace);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.Shelve(this.RequestContext, workspaceName, workspaceOwner, serverItems, shelveset, replace);
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
      out List<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UndoPendingChanges), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.UndoPendingChanges(this.RequestContext, workspaceName, ownerName, items, (string[]) null, (string[]) null);
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
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.UnlabelItem(this.RequestContext, workspaceName, workspaceOwner, labelName, labelScope, items, version);
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
    public Shelveset Unshelve(
      string shelvesetName,
      string shelvesetOwner,
      string workspaceName,
      string workspaceOwner,
      ItemSpec[] items,
      out List<Failure> failures,
      out StreamingCollection<GetOperation> getOperations)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (Unshelve), MethodType.ReadWrite, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (shelvesetName), (object) shelvesetName);
        methodInformation.AddParameter(nameof (shelvesetOwner), (object) shelvesetOwner);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<ItemSpec>(nameof (items), (IList<ItemSpec>) items);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.Unshelve(this.RequestContext, shelvesetName, shelvesetOwner, workspaceName, workspaceOwner, items, (string[]) null);
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
    public void UpdateChangeset(int changeset, string comment, CheckinNote checkinNote)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateChangeset), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (changeset), (object) changeset);
        methodInformation.AddParameter(nameof (comment), (object) comment);
        if (checkinNote != null && checkinNote.Values != null && checkinNote.Values.Length != 0)
          methodInformation.AddArrayParameter<CheckinNoteFieldValue>(nameof (checkinNote), (IList<CheckinNoteFieldValue>) checkinNote.Values);
        this.EnterMethod(methodInformation);
        this.VersionControlService.UpdateChangeset(this.RequestContext, changeset, comment, checkinNote);
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
    public void UpdateCheckinNoteFieldName(
      string path,
      string existingFieldName,
      string newFieldName)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateCheckinNoteFieldName), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (path), (object) path);
        methodInformation.AddParameter(nameof (existingFieldName), (object) existingFieldName);
        methodInformation.AddParameter(nameof (newFieldName), (object) newFieldName);
        this.EnterMethod(methodInformation);
        this.VersionControlService.UpdateCheckinNoteFieldName(this.RequestContext, path, existingFieldName, newFieldName);
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
    public List<PermissionChange> UpdateGlobalSecurity(
      PermissionChange[] changes,
      out List<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateGlobalSecurity), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<PermissionChange>(nameof (changes), (IList<PermissionChange>) changes);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.UpdateGlobalSecurity(this.RequestContext, changes);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        List<PermissionChange> permissionChangeList = resource.Current<List<PermissionChange>>();
        resource.MoveNext();
        failures = resource.Current<List<Failure>>();
        return permissionChangeList;
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
    public List<SecurityChange> UpdateItemSecurity(
      string workspaceName,
      string workspaceOwner,
      SecurityChange[] changes,
      out List<Failure> failures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateItemSecurity), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<SecurityChange>(nameof (changes), (IList<SecurityChange>) changes);
        this.EnterMethod(methodInformation);
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = this.VersionControlService.UpdateItemSecurity(this.RequestContext, workspaceName, workspaceOwner, changes);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        List<SecurityChange> securityChangeList = resource.Current<List<SecurityChange>>();
        resource.MoveNext();
        failures = resource.Current<List<Failure>>();
        return securityChangeList;
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
      LocalVersionUpdate[] updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateLocalVersion), MethodType.ReadWrite, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddArrayParameter<LocalVersionUpdate>(nameof (updates), (IList<LocalVersionUpdate>) updates);
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
    public void UpdatePendingState(
      string workspaceName,
      string workspaceOwner,
      PendingState[] updates)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdatePendingState), MethodType.ReadWrite, EstimatedMethodCost.Low, TimeSpan.FromMinutes(60.0));
        methodInformation.AddParameter(nameof (workspaceName), (object) workspaceName);
        methodInformation.AddParameter(nameof (workspaceOwner), (object) workspaceOwner);
        methodInformation.AddArrayParameter<PendingState>(nameof (updates), (IList<PendingState>) updates);
        this.EnterMethod(methodInformation);
        this.VersionControlService.UpdatePendingState(this.RequestContext, workspaceName, workspaceOwner, updates);
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
    public Workspace UpdateWorkspace(
      string oldWorkspaceName,
      string ownerName,
      Workspace newWorkspace,
      int supportedFeatures)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (UpdateWorkspace), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (oldWorkspaceName), (object) oldWorkspaceName);
        methodInformation.AddParameter(nameof (ownerName), (object) ownerName);
        methodInformation.AddParameter("newWorkspace.Name", (object) newWorkspace?.Name);
        methodInformation.AddParameter("newWorkspace.Computer", (object) newWorkspace?.Computer);
        methodInformation.AddParameter("newWorkspace.Comment", (object) newWorkspace?.Comment);
        methodInformation.AddParameter(nameof (supportedFeatures), (object) (SupportedFeatures) supportedFeatures);
        this.EnterMethod(methodInformation);
        return this.VersionControlService.UpdateWorkspace(this.RequestContext, oldWorkspaceName, ownerName, newWorkspace, supportedFeatures);
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
    public PendingChange[] QueryPendingChangesById(
      int[] pendingChangeIds,
      bool generateDownloadUrls)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryPendingChangesById), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (pendingChangeIds), (IList<int>) pendingChangeIds);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        this.EnterMethod(methodInformation);
        if (pendingChangeIds == null || pendingChangeIds.Length == 0)
          throw new ArgumentNullException(nameof (pendingChangeIds));
        return this.VersionControlService.QueryPendingChangesById(this.RequestContext, pendingChangeIds, generateDownloadUrls);
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
    public Item[] QueryItemsById(
      int[] itemIds,
      int changeSet,
      bool generateDownloadUrls,
      int options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (QueryItemsById), MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<int>(nameof (itemIds), (IList<int>) itemIds);
        methodInformation.AddParameter("changeset", (object) changeSet);
        methodInformation.AddParameter(nameof (generateDownloadUrls), (object) generateDownloadUrls);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        if (itemIds == null || itemIds.Length == 0)
          throw new ArgumentNullException(nameof (itemIds));
        return this.VersionControlService.QueryItemsById(this.RequestContext, itemIds, changeSet, generateDownloadUrls, options);
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
    public void CreateTeamProjectFolder(TeamProjectFolderOptions teamProjectOptions)
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
        this.EnterMethod(methodInformation);
        this.VersionControlService.CreateTeamProjectFolder(this.RequestContext, teamProjectOptions);
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
      List<Mapping> mappings)
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
          resource = this.VersionControlService.CreateBranch(this.RequestContext, sourcePath, targetPath, version, info, checkinNotificationInfo, mappings, false);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
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
  }
}
