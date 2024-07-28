// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMWorkItemBase
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal abstract class TCMWorkItemBase
  {
    private static Dictionary<string, string> WhitelistedFieldNameDefaults = new Dictionary<string, string>()
    {
      {
        "Microsoft.Sync.ProjSrv.IsLinkedToProjSrv",
        "No"
      },
      {
        "Microsoft.Sync.ProjSrv.Submit",
        "No"
      }
    };

    internal virtual WorkItemUpdateData GetUpdateDataForCreate(
      TestManagementRequestContext context,
      string teamProjectName,
      IList<TestExternalLink> externalLinks,
      IList<int> witIdsToLink,
      bool byPass = false)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.GetUpdateDataForCreate");
      this.PopulateFieldsBeforeCreate(context, teamProjectName, byPass);
      List<WorkItemLinkInfo> witLinks = new List<WorkItemLinkInfo>();
      if (witIdsToLink != null && witIdsToLink.Count > 0)
        witLinks.AddRange((IEnumerable<WorkItemLinkInfo>) this.CreateWorkItemLinkInfo(witIdsToLink, 1, ServerResources.RelatedLinkComment, true));
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.GetUpdateDataForCreate");
      return WorkItemUpdateData.CreateWorkItemUpdateData(this.WitTypeName, this.State, true, externalLinks, (IList<WorkItemLinkInfo>) witLinks, false, false, WitOperationType.WitFieldUpdate);
    }

    internal virtual void Create(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      IList<TestExternalLink> externalLinks,
      IList<int> witIdsToLink,
      bool byPass = false)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.Create");
      WorkItemUpdateData updateDataForCreate = this.GetUpdateDataForCreate(context, teamProjectName, externalLinks, witIdsToLink, byPass);
      this.CreateWorkItem(context, teamProjectName, projectId, updateDataForCreate.ExternalLinks, updateDataForCreate.WitLinks, byPass);
      this.PopulateFieldsAfterCreate();
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.Create");
    }

    protected virtual void CreateWorkItem(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      IList<TestExternalLink> externalLinks,
      IList<WorkItemLinkInfo> witLinks,
      bool byPass = false)
    {
      this.CreateNewOrUpdateExistingWorkItem(context, teamProjectName, projectId, true, externalLinks, witLinks, false, false, WitOperationType.WitFieldUpdate, byPass);
    }

    internal UpdatedProperties GetUpdatedProperties(TfsTestManagementRequestContext context)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.GetUpdatedProperties");
      UpdatedProperties updatedProperties = new UpdatedProperties();
      updatedProperties.Id = this.Id;
      updatedProperties.Revision = this.Revision;
      updatedProperties.LastUpdated = this.LastUpdated;
      updatedProperties.LastUpdatedBy = this.LastUpdatedBy;
      updatedProperties.LastUpdatedByName = this.LastUpdatedByName;
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.GetUpdatedProperties");
      return updatedProperties;
    }

    internal virtual string ToWorkItemState(
      TestManagementRequestContext context,
      string projectUri,
      byte tcmState,
      bool autoResolveMultiStateMappingAmbiguity,
      bool isUpgrade = false)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.ToWorkItemState");
      string workItemState = (string) null;
      if (!isUpgrade)
        workItemState = ProcessConfigurationHelper.GetWorkItemStateFromProcessConfiguration(context, projectUri, tcmState, autoResolveMultiStateMappingAmbiguity, this.CategoryRefName);
      if (string.IsNullOrWhiteSpace(workItemState))
        workItemState = this.GetDefaultWorkItemState(context, tcmState);
      if (string.IsNullOrWhiteSpace(workItemState))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CouldNotFindProperStateMappingInProcessConfig, (object) this.ToMetaState(tcmState).ToString()));
      context.TraceInfo("BusinessLayer", "category:'{0}' state:'{1}'", (object) this.CategoryRefName, (object) workItemState);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.ToWorkItemState");
      return workItemState;
    }

    internal virtual byte ToTcmState(
      TestManagementRequestContext context,
      string projectUri,
      string workItemState)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.ToTcmState");
      byte defaultTcmState = this.DefaultTCMState;
      TestManagementProcessConfig processConfiguration = ProcessConfigurationHelper.GetProcessConfiguration(context, projectUri, this.CategoryRefName);
      byte tcmState = this.FromMetaState(this.GetMetaStates(context, processConfiguration, workItemState));
      context.TraceInfo("BusinessLayer", "'{0}' mapped to '{1}' for category '{2}'", (object) workItemState, (object) tcmState, (object) this.CategoryRefName);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.ToTcmState");
      return tcmState;
    }

    internal virtual void Update(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      IdAndRev witIdAndRev,
      CoreWorkItemUpdateFields existingWorkItemFieldValues,
      IList<TestExternalLink> externalLinks,
      IList<WorkItemLinkInfo> witLinks,
      WitOperationType witOperationType,
      bool byPass = false,
      bool suppressNotifications = false,
      bool isSuiteRenameScenario = false)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.Update");
      WorkItemUpdateData updateDataForSave = this.GetUpdateDataForSave(context, teamProjectName, witIdAndRev, existingWorkItemFieldValues, externalLinks, witLinks, witOperationType, byPass);
      this.CreateNewOrUpdateExistingWorkItem(context, teamProjectName, projectId, updateDataForSave.IsNew, updateDataForSave.ExternalLinks, updateDataForSave.WitLinks, updateDataForSave.HasStateChanged, updateDataForSave.UpdateLinksOnly, updateDataForSave.WitOperationType, byPass, suppressNotifications, isSuiteRenameScenario);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.Update");
    }

    internal virtual WorkItemUpdateData GetUpdateDataForSave(
      TestManagementRequestContext context,
      string teamProjectName,
      IdAndRev witIdAndRev,
      CoreWorkItemUpdateFields existingWorkItemFieldValues,
      IList<TestExternalLink> externalLinks,
      IList<WorkItemLinkInfo> witLinks,
      WitOperationType witOperationType,
      bool byPass = false)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.GetUpdateDataForSave");
      this.Id = witIdAndRev.Id;
      this.Revision = witIdAndRev.Revision;
      bool witStateChanged = false;
      this.PopulateFieldsBeforeUpdate(context, teamProjectName, ref existingWorkItemFieldValues, witOperationType, byPass);
      switch (witOperationType)
      {
        case WitOperationType.WitFieldUpdate:
          this.ValidateStateTransition(context, teamProjectName, existingWorkItemFieldValues.State, out witStateChanged);
          break;
        case WitOperationType.UpdateAudit:
          this.WitTypeName = existingWorkItemFieldValues.WokItemType;
          break;
      }
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.GetUpdateDataForSave");
      return WorkItemUpdateData.CreateWorkItemUpdateData(this.WitTypeName, this.State, false, externalLinks, witLinks, witStateChanged, false, witOperationType);
    }

    protected virtual void ValidateStateTransition(
      TestManagementRequestContext context,
      string teamProjectName,
      string fromState,
      out bool witStateChanged)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.ValidateStateTransition");
      witStateChanged = false;
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      if (!string.Equals(fromState, this.State, StringComparison.CurrentCultureIgnoreCase) && service.ValidateStateTransition(context, teamProjectName, fromState, this.State, this.WitTypeName))
        witStateChanged = true;
      context.TraceInfo("BusinessLayer", "TCMWorkItemBase.ValidateStateTransition state changed: " + witStateChanged.ToString());
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.ValidateStateTransition");
    }

    protected virtual void PopulateFieldsBeforeUpdate(
      TestManagementRequestContext context,
      string teamProjectName,
      ref CoreWorkItemUpdateFields existingWorkItemFieldValues,
      WitOperationType witOperationType,
      bool byPass)
    {
      this.PopulateWitFields(context, witOperationType, byPass);
      if (witOperationType == WitOperationType.TcmFieldUpdate || existingWorkItemFieldValues != null)
        return;
      existingWorkItemFieldValues = TCMWorkItemBase.FetchCoreWorkItemUpdateFields(context, teamProjectName, this.Id, this.CategoryRefName);
    }

    internal static CoreWorkItemUpdateFields FetchCoreWorkItemUpdateFields(
      TestManagementRequestContext context,
      string teamProjectName,
      int workItemId,
      string categoryRefName)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.FetchCoreWorkItemUpdateFields");
      CoreWorkItemUpdateFields itemUpdateFields = new CoreWorkItemUpdateFields();
      TCMWorkItemBase tcmWorkItem = TCMWorkItemBase.CreateTCMWorkItem(context, categoryRefName);
      tcmWorkItem.Id = workItemId;
      tcmWorkItem.TeamProjectName = teamProjectName;
      TCMWorkItemBase workItem = TCMWorkItemBase.GetWorkItem(context, tcmWorkItem, CoreWorkItemUpdateFields.CoreWorkItemFields, false);
      itemUpdateFields.AreaPath = workItem.AreaPath;
      itemUpdateFields.IterationPath = workItem.Iteration;
      itemUpdateFields.State = workItem.State;
      itemUpdateFields.WokItemType = workItem.WitTypeName;
      itemUpdateFields.Title = workItem.Title;
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.FetchCoreWorkItemUpdateFields");
      return itemUpdateFields;
    }

    protected virtual void SetStartState(
      TestManagementRequestContext context,
      string teamProjectName,
      bool byPass)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.SetStartState");
      if (string.IsNullOrWhiteSpace(this.State))
        this.State = this.GetStartState(context, teamProjectName);
      if (!byPass && !string.Equals(this.GetStartState(context, teamProjectName), this.State, StringComparison.CurrentCultureIgnoreCase))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CannotCreateWorkItemInInvalidState, (object) this.WitTypeName, (object) this.State, (object) this.GetStartState(context, teamProjectName)));
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.SetStartState");
    }

    internal virtual string GetStartState(TestManagementRequestContext context, string projectName)
    {
      context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.GetStartState");
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      if (string.IsNullOrEmpty(this.StartState))
      {
        if (!string.IsNullOrEmpty(this.WitTypeName))
          this.StartState = service.GetWorkItemStartState(context, projectName, this.WitTypeName);
        else
          context.TraceError("BusinessLayer", "workItemType is null or empty");
      }
      context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.GetStartState");
      return this.StartState;
    }

    protected virtual void CreateNewOrUpdateExistingWorkItem(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      bool isNew,
      IList<TestExternalLink> externalLinks,
      IList<WorkItemLinkInfo> witLinks,
      bool witStateChanged,
      bool updateLinksOnly,
      WitOperationType witOperationType,
      bool byPass = false,
      bool suppressNotifications = false,
      bool isSuiteRenameScenario = false)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.CreateNewOrUpdateExistingWorkItem");
      WorkItemUpdateContext itemUpdateContext = WorkItemUpdateContext.CreateWorkItemUpdateContext(context, teamProjectName, projectId, byPass, suppressNotifications: suppressNotifications, isSuiteRenameScenario: isSuiteRenameScenario);
      WorkItemUpdateData workItemUpdateData = WorkItemUpdateData.CreateWorkItemUpdateData(this.WitTypeName, this.State, isNew, externalLinks, witLinks, witStateChanged, updateLinksOnly, witOperationType);
      TCMWorkItemBase.BulkUpdate(context, itemUpdateContext, workItemUpdateData, new List<TCMWorkItemBase>()
      {
        this
      }, (MigrationLogger) null);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.CreateNewOrUpdateExistingWorkItem");
    }

    internal IList<WorkItemLinkInfo> CreateWorkItemLinkInfo(
      IList<int> targetIds,
      int linkType,
      string comment,
      bool isLocked)
    {
      IList<WorkItemLinkInfo> workItemLinkInfo1 = (IList<WorkItemLinkInfo>) new List<WorkItemLinkInfo>();
      foreach (int targetId in (IEnumerable<int>) targetIds)
      {
        WorkItemLinkInfo workItemLinkInfo2 = new WorkItemLinkInfo();
        workItemLinkInfo2.FieldId = 37;
        workItemLinkInfo2.LinkType = linkType;
        workItemLinkInfo2.SourceId = this.Id;
        workItemLinkInfo2.TargetId = targetId;
        workItemLinkInfo2.IsLocked = isLocked;
        workItemLinkInfo2.Comment = comment;
        workItemLinkInfo1.Add(workItemLinkInfo2);
      }
      return workItemLinkInfo1;
    }

    internal virtual Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate CreateWorkItemUpdateData(
      WorkItemUpdateContext witUpdateContext,
      WorkItemUpdateData witUpdate,
      MigrationLogger logger)
    {
      witUpdateContext.Context.TraceEnter("BusinessLayer", "TCMWorkItemBase.CreateWorkItemUpdateData");
      Dictionary<string, object> witFields = (Dictionary<string, object>) null;
      List<LinkInfo> linkInfoList1 = new List<LinkInfo>();
      List<LinkInfo> linkInfoList2 = new List<LinkInfo>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate workItemUpdateData = new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate();
      List<WorkItemLinkUpdate> linkUpdateData = (List<WorkItemLinkUpdate>) null;
      List<WorkItemResourceLinkUpdate> resourceLinkUpdateData = (List<WorkItemResourceLinkUpdate>) null;
      witUpdateContext.Context.TraceVerbose("BusinessLayer", "TCMWorkItemBase.CreateWorkItemUpdateData id:{0} witType:{1}", (object) witUpdate.WitIdAndRev.Id, (object) this.WitTypeName);
      if (witUpdate.WitOperationType != WitOperationType.TcmFieldUpdate)
        this.CreateWitLinks((TestManagementRequestContext) witUpdateContext.Context, witUpdate.ExternalLinks, witUpdate.WitLinks, witUpdate.IsNew, logger, out linkUpdateData, out resourceLinkUpdateData);
      if (!witUpdate.UpdateLinksOnly)
      {
        witFields = this.CreateWitFields((TestManagementRequestContext) witUpdateContext.Context, witUpdateContext.TeamProjectName, witUpdateContext.ProjectGuidAndString, witUpdate.HasStateChanged, witUpdate.IsNew, witUpdate.WitOperationType, witUpdateContext.IsUpgradeContext, witUpdateContext.isSuiteRenameScenario);
        if (witUpdate.WitOperationType != WitOperationType.TcmFieldUpdate)
          this.PopulateRequiredFields((TestManagementRequestContext) witUpdateContext.Context, witUpdateContext.TeamProjectName, witUpdateContext.ProjectGuidAndString, witUpdate.IsNew ? -1 : witUpdate.WitIdAndRev.Id, this.WitTypeName, ref witFields);
      }
      workItemUpdateData.LinkUpdates = (IEnumerable<WorkItemLinkUpdate>) linkUpdateData;
      workItemUpdateData.ResourceLinkUpdates = (IEnumerable<WorkItemResourceLinkUpdate>) resourceLinkUpdateData;
      workItemUpdateData.Fields = (IEnumerable<KeyValuePair<string, object>>) witFields.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (f => f.Key.ToString((IFormatProvider) CultureInfo.CurrentCulture)), (Func<KeyValuePair<string, object>, object>) (f => f.Value));
      witUpdateContext.Context.TraceLeave("BusinessLayer", "TCMWorkItemBase.CreateWorkItemUpdateData");
      return workItemUpdateData;
    }

    internal virtual JsonPatchDocument CreateWorkItemJsonPatchDocument(
      WorkItemUpdateContext witUpdateContext,
      WorkItemUpdateData witUpdate,
      MigrationLogger logger)
    {
      witUpdateContext.Context.TraceEnter("BusinessLayer", "TCMWorkItemBase.CreateWorkItemJsonPatchDocument");
      Dictionary<string, object> witFields = (Dictionary<string, object>) null;
      List<LinkInfo> linkInfoList1 = new List<LinkInfo>();
      List<LinkInfo> linkInfoList2 = new List<LinkInfo>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate workItemUpdate = new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate();
      List<WorkItemLinkUpdate> workItemLinkUpdateList = (List<WorkItemLinkUpdate>) null;
      witUpdateContext.Context.TraceVerbose("BusinessLayer", "TCMWorkItemBase.CreateWorkItemJsonPatchDocument id:{0} witType:{1}", (object) witUpdate.WitIdAndRev.Id, (object) this.WitTypeName);
      List<WorkItemRelation> relations = new List<WorkItemRelation>();
      if (!witUpdate.UpdateLinksOnly)
      {
        witFields = this.CreateWitFields((TestManagementRequestContext) witUpdateContext.Context, witUpdateContext.TeamProjectName, witUpdateContext.ProjectGuidAndString, witUpdate.HasStateChanged, witUpdate.IsNew, witUpdate.WitOperationType, witUpdateContext.IsUpgradeContext);
        if (witUpdate.WitOperationType != WitOperationType.TcmFieldUpdate)
          this.PopulateRequiredFields((TestManagementRequestContext) witUpdateContext.Context, witUpdateContext.TeamProjectName, witUpdateContext.ProjectGuidAndString, witUpdate.IsNew ? -1 : witUpdate.WitIdAndRev.Id, this.WitTypeName, ref witFields);
      }
      workItemUpdate.LinkUpdates = (IEnumerable<WorkItemLinkUpdate>) workItemLinkUpdateList;
      workItemUpdate.ResourceLinkUpdates = (IEnumerable<WorkItemResourceLinkUpdate>) null;
      workItemUpdate.Fields = (IEnumerable<KeyValuePair<string, object>>) witFields.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (f => f.Key.ToString((IFormatProvider) CultureInfo.CurrentCulture)), (Func<KeyValuePair<string, object>, object>) (f => f.Value));
      witUpdateContext.Context.TraceLeave("BusinessLayer", "TCMWorkItemBase.CreateWorkItemJsonPatchDocument");
      return WorkHelper.ConvertToJsonPatchDocument(witUpdateContext.Context.RequestContext, (IDictionary<string, object>) witFields, (IList<WorkItemRelation>) relations);
    }

    internal static void BulkUpdate(
      TestManagementRequestContext context,
      WorkItemUpdateContext witUpdateContext,
      WorkItemUpdateData witUpdateData,
      List<TCMWorkItemBase> tcmWorkItems,
      MigrationLogger logger)
    {
      witUpdateContext.Context.TraceEnter("BusinessLayer", "TCMWorkItemBase.BulkUpdate");
      List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> tcmWorkItemsWithUpdataData = new List<Tuple<TCMWorkItemBase, WorkItemUpdateData>>();
      foreach (TCMWorkItemBase tcmWorkItem in tcmWorkItems)
      {
        WorkItemUpdateData workItemUpdateData = new WorkItemUpdateData();
        workItemUpdateData.CopyProperties(witUpdateData);
        tcmWorkItemsWithUpdataData.Add(new Tuple<TCMWorkItemBase, WorkItemUpdateData>(tcmWorkItem, workItemUpdateData));
      }
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.BulkUpdateUsingServerOM"))
        TCMWorkItemBase.BulkUpdateWithServerOM(witUpdateContext, tcmWorkItemsWithUpdataData, logger);
      else
        TCMWorkItemBase.BulkUpdate(witUpdateContext, tcmWorkItemsWithUpdataData, logger);
      witUpdateContext.Context.TraceLeave("BusinessLayer", "TCMWorkItemBase.BulkUpdate");
    }

    internal static void BulkUpdateWithServerOM(
      WorkItemUpdateContext witUpdateContext,
      List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> tcmWorkItemsWithUpdataData,
      MigrationLogger logger)
    {
      using (PerfManager.Measure(witUpdateContext.Context.RequestContext, "CrossService", TraceUtils.GetActionName("BulkUpdate", "WorkItem")))
      {
        witUpdateContext.Context.TraceEnter("BusinessLayer", "TCMWorkItemBase.BulkUpdate");
        TeamFoundationWorkItemService service = witUpdateContext.Context.RequestContext.GetService<TeamFoundationWorkItemService>();
        int num = 0;
        TCMWorkItemBase.LogInfo(logger, string.Format("TCMWorkItemBase.BulkUpdate Creating workitems. Total Workitems to be created: {0}", (object) tcmWorkItemsWithUpdataData.Count));
        for (int count = 0; count < tcmWorkItemsWithUpdataData.Count; count += 200)
        {
          List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> list = tcmWorkItemsWithUpdataData.Skip<Tuple<TCMWorkItemBase, WorkItemUpdateData>>(count).Take<Tuple<TCMWorkItemBase, WorkItemUpdateData>>(200).ToList<Tuple<TCMWorkItemBase, WorkItemUpdateData>>();
          List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdateList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>();
          int clonedEntityTempId = WITConstants.NewClonedEntityTempId;
          foreach (Tuple<TCMWorkItemBase, WorkItemUpdateData> tuple in list)
          {
            Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate updateData = (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate) null;
            tuple.Item2.WitIdAndRev = tuple.Item2.IsNew ? new IdAndRev(-1, 0) : new IdAndRev(tuple.Item1.Id, tuple.Item1.Revision);
            tuple.Item2.TemporaryId = tuple.Item2.IsNew ? clonedEntityTempId : -tuple.Item1.Id;
            tuple.Item1.PopulateDataAndUpdatePackage(witUpdateContext, tuple.Item2, logger, out updateData);
            if (tuple.Item2.IsNew)
            {
              updateData.Id = clonedEntityTempId;
              --clonedEntityTempId;
            }
            else
            {
              updateData.Id = tuple.Item1.Id;
              updateData.Rev = tuple.Item1.Revision;
            }
            workItemUpdateList.Add(updateData);
          }
          TCMWorkItemBase.LogInfo(logger, "TCMWorkItemBase.BulkUpdate Calling TeamFoundationWorkItemService.UpdateWorkItems().");
          witUpdateContext.Context.TraceVerbose("BusinessLayer", "TCMWorkItemBase.BulkUpdate Calling TeamFoundationWorkItemService.UpdateWorkItems().");
          List<WorkItemUpdateResult> results = witUpdateContext.BypassRulesValidation ? service.UpdateWorkItems(witUpdateContext.Context.RequestContext, (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>) workItemUpdateList, true, false, false, (IReadOnlyCollection<int>) null, witUpdateContext.SuppressNotifications, false, false, false).ToList<WorkItemUpdateResult>() : service.UpdateWorkItems(witUpdateContext.Context.RequestContext, (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>) workItemUpdateList, WorkItemUpdateRuleExecutionMode.Full, false, false, false, (IReadOnlyCollection<int>) null, witUpdateContext.SuppressNotifications, false, false).ToList<WorkItemUpdateResult>();
          WorkItemUpdateResult itemUpdateResult = results.Find((Predicate<WorkItemUpdateResult>) (r => r.Exception != null));
          if (itemUpdateResult != null)
          {
            witUpdateContext.Context.TraceError("BusinessLayer", itemUpdateResult.Exception.ToString());
            throw WitToTcmExceptionConverter.Convert((Exception) itemUpdateResult.Exception);
          }
          TCMWorkItemBase.OnBulkUpdateComplete((TestManagementRequestContext) witUpdateContext.Context, results, list);
          foreach (Tuple<TCMWorkItemBase, WorkItemUpdateData> tuple in list)
          {
            if (tuple.Item2.IsNew)
              tuple.Item1.PopulateFieldsAfterCreate();
          }
          ++num;
          TCMWorkItemBase.LogInfo(logger, string.Format("Batch {0} completed.", (object) num));
        }
        witUpdateContext.Context.TraceLeave("BusinessLayer", "TCMWorkItemBase.BulkUpdate");
      }
    }

    internal static void BulkUpdate(
      WorkItemUpdateContext witUpdateContext,
      List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> tcmWorkItemsWithUpdataData,
      MigrationLogger logger)
    {
      using (PerfManager.Measure(witUpdateContext.Context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (BulkUpdate), "WorkItem")))
      {
        witUpdateContext.Context.TraceEnter("BusinessLayer", "TCMWorkItemBase.BulkUpdate");
        IWorkItemRemotableService service = witUpdateContext.Context.RequestContext.GetService<IWorkItemRemotableService>();
        witUpdateContext.Context.RequestContext.GetService<TeamFoundationWorkItemService>();
        int num = 0;
        TCMWorkItemBase.LogInfo(logger, string.Format("TCMWorkItemBase.BulkUpdate Creating workitems. Total Workitems to be created: {0}", (object) tcmWorkItemsWithUpdataData.Count));
        for (int count = 0; count < tcmWorkItemsWithUpdataData.Count; count += 200)
        {
          List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> list = tcmWorkItemsWithUpdataData.Skip<Tuple<TCMWorkItemBase, WorkItemUpdateData>>(count).Take<Tuple<TCMWorkItemBase, WorkItemUpdateData>>(200).ToList<Tuple<TCMWorkItemBase, WorkItemUpdateData>>();
          List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> batchOftcmWorkItemsForCreate = new List<Tuple<TCMWorkItemBase, WorkItemUpdateData>>();
          List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> batchOftcmWorkItemsForUpdate = new List<Tuple<TCMWorkItemBase, WorkItemUpdateData>>();
          IList<CreateWitRequest> createWitRequests = (IList<CreateWitRequest>) new List<CreateWitRequest>();
          IList<UpdateWitRequest> updateWitRequests = (IList<UpdateWitRequest>) new List<UpdateWitRequest>();
          int clonedEntityTempId = WITConstants.NewClonedEntityTempId;
          foreach (Tuple<TCMWorkItemBase, WorkItemUpdateData> tuple in list)
          {
            tuple.Item2.WitIdAndRev = tuple.Item2.IsNew ? new IdAndRev(-1, 0) : new IdAndRev(tuple.Item1.Id, tuple.Item1.Revision);
            tuple.Item2.TemporaryId = tuple.Item2.IsNew ? clonedEntityTempId : -tuple.Item1.Id;
            if (tuple.Item2.IsNew)
            {
              CreateWitRequest createData = (CreateWitRequest) null;
              tuple.Item1.PopulateDataAndUpdatePackage(witUpdateContext, tuple.Item2, logger, out createData);
              tuple.Item2.WitIdAndRev = new IdAndRev(-1, 0);
              tuple.Item2.TemporaryId = clonedEntityTempId;
              --clonedEntityTempId;
              createWitRequests.Add(createData);
              batchOftcmWorkItemsForCreate.Add(tuple);
            }
            else
            {
              UpdateWitRequest updateData = (UpdateWitRequest) null;
              tuple.Item1.PopulateDataAndUpdatePackage(witUpdateContext, tuple.Item2, logger, out updateData);
              tuple.Item2.WitIdAndRev = new IdAndRev(tuple.Item1.Id, tuple.Item1.Revision);
              tuple.Item2.TemporaryId = -tuple.Item1.Id;
              updateWitRequests.Add(updateData);
              batchOftcmWorkItemsForUpdate.Add(tuple);
            }
          }
          TCMWorkItemBase.LogInfo(logger, "TCMWorkItemBase.BulkUpdate Calling TeamFoundationWorkItemService.UpdateWorkItems().");
          witUpdateContext.Context.TraceVerbose("BusinessLayer", "TCMWorkItemBase.BulkUpdate Calling TeamFoundationWorkItemService.UpdateWorkItems().");
          if (createWitRequests != null && createWitRequests.Count > 0)
          {
            IEnumerable<WitBatchResponse> workItems = service.CreateWorkItems(witUpdateContext.Context.RequestContext, witUpdateContext.TeamProjectName, createWitRequests, witUpdateContext.BypassRulesValidation, witUpdateContext.SuppressNotifications);
            TCMWorkItemBase.HandleException(witUpdateContext, service, workItems);
            TCMWorkItemBase.OnBulkCreateComplete((TestManagementRequestContext) witUpdateContext.Context, (IList<WitBatchResponse>) workItems.ToList<WitBatchResponse>(), (IList<Tuple<TCMWorkItemBase, WorkItemUpdateData>>) batchOftcmWorkItemsForCreate);
          }
          if (updateWitRequests != null && updateWitRequests.Count > 0)
          {
            IEnumerable<WitBatchResponse> witBatchResponses = service.UpdateWorkItems(witUpdateContext.Context.RequestContext, updateWitRequests, witUpdateContext.BypassRulesValidation, witUpdateContext.SuppressNotifications);
            TCMWorkItemBase.HandleException(witUpdateContext, service, witBatchResponses);
            TCMWorkItemBase.OnBulkUpdateComplete((TestManagementRequestContext) witUpdateContext.Context, (IList<WitBatchResponse>) witBatchResponses.ToList<WitBatchResponse>(), batchOftcmWorkItemsForUpdate);
          }
          foreach (Tuple<TCMWorkItemBase, WorkItemUpdateData> tuple in list)
          {
            if (tuple.Item2.IsNew)
              tuple.Item1.PopulateFieldsAfterCreate();
          }
          ++num;
          TCMWorkItemBase.LogInfo(logger, string.Format("Batch {0} completed.", (object) num));
        }
        witUpdateContext.Context.TraceLeave("BusinessLayer", "TCMWorkItemBase.BulkUpdate");
      }
    }

    private static void HandleException(
      WorkItemUpdateContext witUpdateContext,
      IWorkItemRemotableService workItemRemotableService,
      IEnumerable<WitBatchResponse> createResults)
    {
      int successCode = 200;
      WitBatchResponse witBatchResponse = createResults.ToList<WitBatchResponse>().Find((Predicate<WitBatchResponse>) (r => r.Code != successCode));
      if (witBatchResponse != null)
      {
        Exception e = JsonConvert.DeserializeObject<Exception>(witBatchResponse.Body);
        witUpdateContext.Context.TraceError("BusinessLayer", e.ToString());
        throw WitToTcmExceptionConverter.Convert(e);
      }
    }

    protected virtual void PopulateDataAndUpdatePackage(
      WorkItemUpdateContext witUpdateContext,
      WorkItemUpdateData witUpdateData,
      MigrationLogger logger,
      out Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate updateData)
    {
      updateData = this.CreateWorkItemUpdateData(witUpdateContext, witUpdateData, logger);
    }

    protected virtual void PopulateDataAndUpdatePackage(
      WorkItemUpdateContext witUpdateContext,
      WorkItemUpdateData witUpdateData,
      MigrationLogger logger,
      out CreateWitRequest createData)
    {
      JsonPatchDocument jsonPatchDocument = this.CreateWorkItemJsonPatchDocument(witUpdateContext, witUpdateData, logger);
      createData = new CreateWitRequest()
      {
        Id = witUpdateData.TemporaryId,
        TypeName = witUpdateData.WitTypeName,
        document = jsonPatchDocument
      };
    }

    protected virtual void PopulateDataAndUpdatePackage(
      WorkItemUpdateContext witUpdateContext,
      WorkItemUpdateData witUpdateData,
      MigrationLogger logger,
      out UpdateWitRequest updateData)
    {
      JsonPatchDocument jsonPatchDocument = this.CreateWorkItemJsonPatchDocument(witUpdateContext, witUpdateData, logger);
      updateData = new UpdateWitRequest()
      {
        Id = witUpdateData.WitIdAndRev.Id,
        Rev = witUpdateData.WitIdAndRev.Revision,
        document = jsonPatchDocument
      };
    }

    internal static void OnBulkCreateComplete(
      TestManagementRequestContext context,
      IList<WitBatchResponse> results,
      IList<Tuple<TCMWorkItemBase, WorkItemUpdateData>> batchOftcmWorkItemsForCreate)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.OnBulkCreateComplete");
      TCMWorkItemBase.ProcessBulkUpdates(context, results, batchOftcmWorkItemsForCreate);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.OnBulkCreateComplete");
    }

    internal static void OnBulkUpdateComplete(
      TestManagementRequestContext context,
      IList<WitBatchResponse> results,
      List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> batchOftcmWorkItemsForUpdate)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.OnBulkUpdateComplete");
      TCMWorkItemBase.ProcessBulkUpdates(context, results, (IList<Tuple<TCMWorkItemBase, WorkItemUpdateData>>) batchOftcmWorkItemsForUpdate);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.OnBulkUpdateComplete");
    }

    internal static void OnBulkUpdateComplete(
      TestManagementRequestContext context,
      List<WorkItemUpdateResult> results,
      List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> tcmWorkItemsWithUpdataData)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.OnBulkUpdateComplete");
      TCMWorkItemBase.ProcessBulkUpdates(context, results, tcmWorkItemsWithUpdataData);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.OnBulkUpdateComplete");
    }

    internal static void ProcessBulkUpdates(
      TestManagementRequestContext context,
      List<WorkItemUpdateResult> results,
      List<Tuple<TCMWorkItemBase, WorkItemUpdateData>> tcmWorkItemsWithUpdataData)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.ProcessBulkUpdates");
      foreach (Tuple<TCMWorkItemBase, WorkItemUpdateData> tuple in tcmWorkItemsWithUpdataData)
      {
        Tuple<TCMWorkItemBase, WorkItemUpdateData> tcmWorkItemWithUpdataData = tuple;
        DateTime lastUpdated = DateTime.UtcNow;
        WorkItemUpdateResult itemUpdateResult = !tcmWorkItemWithUpdataData.Item2.IsNew ? results.Find((Predicate<WorkItemUpdateResult>) (r => r.UpdateId == tcmWorkItemWithUpdataData.Item1.Id)) : results.Find((Predicate<WorkItemUpdateResult>) (r => r.UpdateId == tcmWorkItemWithUpdataData.Item2.TemporaryId));
        if (itemUpdateResult != null)
        {
          int id = itemUpdateResult.Id;
          int rev = itemUpdateResult.Rev;
          if (itemUpdateResult.Fields.ContainsKey(-4.ToString()))
            lastUpdated = (DateTime) itemUpdateResult.Fields[-4.ToString()];
          tcmWorkItemWithUpdataData.Item1.OnWitUpdateComplete(context, id, rev, lastUpdated);
        }
      }
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.ProcessBulkUpdates");
    }

    internal static void ProcessBulkUpdates(
      TestManagementRequestContext context,
      IList<WitBatchResponse> results,
      IList<Tuple<TCMWorkItemBase, WorkItemUpdateData>> batchOftcmWorkItemsForCreate)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.ProcessBulkUpdates");
      for (int index = 0; index < batchOftcmWorkItemsForCreate.Count; ++index)
      {
        int? nullable1 = new int?(-1);
        int? nullable2 = new int?(0);
        DateTime utcNow = DateTime.UtcNow;
        Tuple<TCMWorkItemBase, WorkItemUpdateData> tuple = batchOftcmWorkItemsForCreate[index];
        WitBatchResponse result = results[index];
        if (result != null)
        {
          Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem = JsonConvert.DeserializeObject<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>(result.Body);
          nullable1 = workItem.Id;
          nullable2 = workItem.Rev;
          IDictionary<string, object> fields1 = workItem.Fields;
          int num = -4;
          string key1 = num.ToString();
          if (fields1.ContainsKey(key1))
          {
            IDictionary<string, object> fields2 = workItem.Fields;
            num = -4;
            string key2 = num.ToString();
            utcNow = (DateTime) fields2[key2];
          }
          tuple.Item1.OnWitUpdateComplete(context, nullable1.Value, nullable2.Value, utcNow);
        }
      }
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.ProcessBulkUpdates");
    }

    protected virtual void OnWitUpdateComplete(
      TestManagementRequestContext context,
      int id,
      int revision,
      DateTime lastUpdated)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.OnWitUpdateComplete");
      this.Id = id;
      this.Revision = revision;
      this.LastUpdated = lastUpdated;
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.OnWitUpdateComplete");
    }

    protected virtual Dictionary<string, object> CreateWitFields(
      TestManagementRequestContext context,
      string teamprojectName,
      GuidAndString targetProject,
      bool witStateChanged,
      bool isNew,
      WitOperationType witOperationType,
      bool isUpgrade = false,
      bool isSuiteRenameScenario = false)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.CreateWitFields");
      Dictionary<string, object> witFields = new Dictionary<string, object>();
      WITCreator witCreator = new WITCreator(context);
      if (witOperationType == WitOperationType.WitFieldUpdate)
      {
        this.CheckValueAndAddWitField(witCreator, context, witFields, "System.Title", this.Title, targetProject);
        if (!isSuiteRenameScenario)
        {
          string o = this.Description == null ? string.Empty : this.Description;
          witFields["System.Description"] = isUpgrade ? (object) HtmlConverter.ConvertToHtml((object) o, true) : (object) o;
        }
        this.CheckValueAndAddWitField(witCreator, context, witFields, "System.AreaPath", this.AreaPath, targetProject);
        this.CheckValueAndAddWitField(witCreator, context, witFields, "System.IterationPath", this.Iteration, targetProject);
        this.CheckValueAndAddWitField(witCreator, context, witFields, "System.State", this.State, targetProject);
        if (isNew)
        {
          this.CheckValueAndAddWitField(witCreator, context, witFields, "System.WorkItemType", this.WitTypeName, targetProject);
          this.CheckValueAndAddWitField(witCreator, context, witFields, "System.CreatedBy", this.CreatedByDistinctName, targetProject);
        }
      }
      this.CheckValueAndAddWitField(witCreator, context, witFields, "System.ChangedBy", this.LastUpdatedByDistinctName, targetProject);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.CreateWitFields");
      return witFields;
    }

    internal static TCMWorkItemBase CreateTCMWorkItem(
      TestManagementRequestContext context,
      string workItemCategoryRefName)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.CreateTCMWorkItem");
      TCMWorkItemBase tcmWorkItem = !string.Equals(WitCategoryRefName.TestPlan, workItemCategoryRefName, StringComparison.InvariantCultureIgnoreCase) ? (TCMWorkItemBase) new TestSuiteWorkItem() : (TCMWorkItemBase) new TestPlanWorkItem();
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.CreateTCMWorkItem");
      return tcmWorkItem;
    }

    internal byte GetDefaultTcmState() => this.DefaultTCMState;

    internal static void ResolveDisplayNames(
      TestManagementRequestContext context,
      Dictionary<string, Guid> identitiesMap)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.ResolveDisplayNames");
      List<string> list = identitiesMap.Keys.ToList<string>();
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> searchUsersByName in IdentityHelper.SearchUsersByNames(context, list))
        identitiesMap[searchUsersByName.Key] = searchUsersByName.Value.Id;
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.ResolveDisplayNames");
    }

    internal static bool WorkItemExists(
      TestManagementRequestContext context,
      string projectName,
      int workItemIdToFetch,
      string workItemCategoryRefName,
      out bool projectOrCategoryMismatch)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TCMWorkItemBase.WorkItemExists"))
      {
        projectOrCategoryMismatch = false;
        TestManagementRequestContext context1 = context;
        string projectName1 = projectName;
        List<int> workItemIds = new List<int>();
        workItemIds.Add(workItemIdToFetch);
        string[] workItemFields = new string[1]
        {
          "System.Id"
        };
        string workItemCategoryRefName1 = workItemCategoryRefName;
        List<TCMWorkItemBase> workItems = TCMWorkItemBase.GetWorkItems(context1, projectName1, workItemIds, workItemFields, workItemCategoryRefName1, false, true);
        if (workItems == null || workItems.Count != 1)
          return false;
        projectOrCategoryMismatch = (TCMWorkItemBase.FilterWorkItems(context, projectName, workItemCategoryRefName, new List<TCMWorkItemBase>()
        {
          workItems[0]
        }).Count == 0 ? 1 : 0) != 0;
        return true;
      }
    }

    internal static TCMWorkItemBase GetWorkItem(
      TestManagementRequestContext context,
      TCMWorkItemBase workItemToFetch,
      string[] workItemFields,
      bool isFilteringRequired)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.GetWorkItem");
      TestManagementRequestContext context1 = context;
      string teamProjectName = workItemToFetch.TeamProjectName;
      List<int> workItemIds = new List<int>();
      workItemIds.Add(workItemToFetch.Id);
      string[] workItemFields1 = workItemFields;
      string categoryRefName = workItemToFetch.CategoryRefName;
      int num = isFilteringRequired ? 1 : 0;
      List<TCMWorkItemBase> workItems = TCMWorkItemBase.GetWorkItems(context1, teamProjectName, workItemIds, workItemFields1, categoryRefName, num != 0);
      if (workItems != null && workItems.Count == 1)
      {
        TCMWorkItemBase workItem = workItems[0];
        context.TraceLeave("BusinessLayer", "TCMWorkItemBase.GetWorkItem");
        return workItem;
      }
      bool projectOrCategoryMismatch = false;
      if (TCMWorkItemBase.WorkItemExists(context, workItemToFetch.TeamProjectName, workItemToFetch.Id, workItemToFetch.CategoryRefName, out projectOrCategoryMismatch) && !projectOrCategoryMismatch)
      {
        context.TraceError("BusinessLayer", "Access Denied: No valid workitem object could be fetched for category:'{0}' id:'{1}'", (object) workItemToFetch.CategoryRefName, (object) workItemToFetch.Id);
        throw new AccessDeniedException(ServerResources.CannotViewWorkItems);
      }
      context.TraceError("BusinessLayer", "No valid workitem object could be fetched for category:'{0}' id:'{1}'", (object) workItemToFetch.CategoryRefName, (object) workItemToFetch.Id);
      throw new TestObjectNotFoundException(context.RequestContext, workItemToFetch.Id, workItemToFetch.TCMObjectType);
    }

    internal static List<TCMWorkItemBase> GetWorkItems(
      TestManagementRequestContext context,
      string projectName,
      List<int> workItemIds,
      string workItemCategoryRefName,
      bool skipPermissionCheck = false)
    {
      TCMWorkItemBase tcmWorkItem = TCMWorkItemBase.CreateTCMWorkItem(context, workItemCategoryRefName);
      return TCMWorkItemBase.GetWorkItems(context, projectName, workItemIds, tcmWorkItem.WitFields, workItemCategoryRefName, true, skipPermissionCheck);
    }

    internal static KeyValuePair<string, List<TCMWorkItemBase>> GetUpdatedWorkItems(
      TestManagementRequestContext context,
      string workItemCategoryRefName,
      DateTime? startTime,
      string continuationToken,
      bool skipPermissionCheck = false)
    {
      TCMWorkItemBase tcmWorkItem = TCMWorkItemBase.CreateTCMWorkItem(context, workItemCategoryRefName);
      return TCMWorkItemBase.GetUpdatedWorkItems(context, tcmWorkItem.WitFields, workItemCategoryRefName, startTime, continuationToken, skipPermissionCheck);
    }

    internal static List<TCMWorkItemBase> GetWorkItems(
      TestManagementRequestContext context,
      string projectName,
      List<int> workItemIds,
      string[] workItemFields,
      string workItemCategoryRefName,
      bool isFilteringRequired,
      bool skipPermissionCheck = false)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.GetWorkItems");
      List<TCMWorkItemBase> tcmWits = new List<TCMWorkItemBase>();
      if (workItemIds != null && workItemIds.Count > 0 && workItemFields != null)
      {
        TCMWorkItemBase.CreateTCMWorkItem(context, workItemCategoryRefName);
        foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem in context.RequestContext.GetService<IWitHelper>().GetWorkItems(context.RequestContext, workItemIds, ((IEnumerable<string>) workItemFields).ToList<string>(), skipPermissionCheck, projectName))
        {
          TCMWorkItemBase tcmWorkItem = TCMWorkItemBase.CreateTCMWorkItem(context, workItemCategoryRefName);
          tcmWorkItem.FromWorkItem(context, workItem);
          tcmWits.Add(tcmWorkItem);
        }
        if (isFilteringRequired)
          tcmWits = TCMWorkItemBase.FilterWorkItems(context, projectName, workItemCategoryRefName, tcmWits);
      }
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.GetWorkItems");
      return tcmWits;
    }

    internal static KeyValuePair<string, List<TCMWorkItemBase>> GetUpdatedWorkItems(
      TestManagementRequestContext context,
      string[] workItemFields,
      string workItemCategoryRefName,
      DateTime? startTime,
      string continuationToken,
      bool skipPermissionCheck = false)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.GetWorkItems");
      List<TCMWorkItemBase> tcmWorkItemBaseList = new List<TCMWorkItemBase>();
      string key = (string) null;
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      if (workItemFields != null)
      {
        TCMWorkItemBase tcmWorkItem1 = TCMWorkItemBase.CreateTCMWorkItem(context, workItemCategoryRefName);
        ReportingWorkItemRevisionsBatch updatedWorkitems = service.GetUpdatedWorkitems(context, workItemFields, tcmWorkItem1.CategoryName, startTime, continuationToken, skipPermissionCheck);
        if (updatedWorkitems != null)
        {
          key = updatedWorkitems.ContinuationToken;
          foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItemFieldData in updatedWorkitems.Values)
          {
            TCMWorkItemBase tcmWorkItem2 = TCMWorkItemBase.CreateTCMWorkItem(context, workItemCategoryRefName);
            tcmWorkItem2.FromWorkItem(context, workItemFieldData);
            tcmWorkItemBaseList.Add(tcmWorkItem2);
          }
        }
      }
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.GetWorkItems");
      return new KeyValuePair<string, List<TCMWorkItemBase>>(key, tcmWorkItemBaseList);
    }

    private static List<TCMWorkItemBase> FilterWorkItems(
      TestManagementRequestContext context,
      string projectName,
      string workItemCategoryRefName,
      List<TCMWorkItemBase> tcmWits)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.FilterWorkItems");
      List<TCMWorkItemBase> tcmWits1 = TCMWorkItemBase.RemoveWorkItemsNotInCategory(context, projectName, workItemCategoryRefName, tcmWits);
      List<TCMWorkItemBase> tcmWorkItemBaseList = TCMWorkItemBase.RemoveWorkItemsNotInProject(context, projectName, tcmWits1);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.FilterWorkItems");
      return tcmWorkItemBaseList;
    }

    private static List<TCMWorkItemBase> RemoveWorkItemsNotInCategory(
      TestManagementRequestContext context,
      string projectName,
      string workItemCategoryRefName,
      List<TCMWorkItemBase> tcmWits)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TCMWorkItemBase.RemoveWorkItemsNotInCategory");
        List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory> itemTypeCategories = context.RequestContext.GetService<IWitHelper>().GetWorkItemTypeCategories(context.RequestContext, projectName, (IEnumerable<string>) new List<string>()
        {
          workItemCategoryRefName
        });
        if (itemTypeCategories == null || itemTypeCategories.Count != 1)
          return new List<TCMWorkItemBase>();
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory itemTypeCategory = itemTypeCategories[0];
        Dictionary<int, TCMWorkItemBase> dictionary = new Dictionary<int, TCMWorkItemBase>();
        foreach (TCMWorkItemBase tcmWit1 in tcmWits)
        {
          TCMWorkItemBase tcmWit = tcmWit1;
          IEnumerable<WorkItemTypeReference> workItemTypes = itemTypeCategory.WorkItemTypes;
          if ((workItemTypes != null ? (workItemTypes.Any<WorkItemTypeReference>((Func<WorkItemTypeReference, bool>) (type => TFStringComparer.WorkItemTypeName.Equals(type.Name, tcmWit.WitTypeName))) ? 1 : 0) : 0) != 0)
            dictionary[tcmWit.Id] = tcmWit;
        }
        return dictionary.Values.ToList<TCMWorkItemBase>();
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TCMWorkItemBase.RemoveWorkItemsNotInCategory");
      }
    }

    private static List<TCMWorkItemBase> RemoveWorkItemsNotInProject(
      TestManagementRequestContext context,
      string projectName,
      List<TCMWorkItemBase> tcmWits)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.RemoveWorkItemsNotInProject");
      List<TCMWorkItemBase> tcmWorkItemBaseList = new List<TCMWorkItemBase>();
      foreach (TCMWorkItemBase tcmWit in tcmWits)
      {
        if (string.Equals(tcmWit.TeamProjectName, projectName, StringComparison.OrdinalIgnoreCase))
          tcmWorkItemBaseList.Add(tcmWit);
      }
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.RemoveWorkItemsNotInProject");
      return tcmWorkItemBaseList;
    }

    protected virtual void FromWorkItem(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItemFieldData)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.FromWorkItem");
      this.Id = this.ExtractFieldValueFromPayload<int>(context, "System.Id", workItemFieldData);
      this.Title = this.ExtractFieldValueFromPayload<string>(context, "System.Title", workItemFieldData);
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      IdentityRef valueFromPayload1 = this.ExtractFieldValueFromPayload<IdentityRef>(context, "System.AssignedTo", workItemFieldData);
      if (valueFromPayload1 != null)
        this.OwnerName = CommonUtils.DistinctDisplayName(valueFromPayload1);
      IdentityRef valueFromPayload2 = this.ExtractFieldValueFromPayload<IdentityRef>(context, "System.CreatedBy", workItemFieldData);
      if (valueFromPayload2 != null)
        this.CreatedByName = CommonUtils.DistinctDisplayName(valueFromPayload2);
      this.WitTypeName = this.ExtractFieldValueFromPayload<string>(context, "System.WorkItemType", workItemFieldData);
      this.EncodedDescription = this.ExtractFieldValueFromPayload<string>(context, "System.Description", workItemFieldData);
      this.Description = HttpUtility.HtmlDecode(this.EncodedDescription);
      this.State = this.ExtractFieldValueFromPayload<string>(context, "System.State", workItemFieldData);
      string valueFromPayload3 = this.ExtractFieldValueFromPayload<string>(context, "System.IterationPath", workItemFieldData);
      this.Iteration = service.TranslateCSSPath(valueFromPayload3);
      string valueFromPayload4 = this.ExtractFieldValueFromPayload<string>(context, "System.AreaPath", workItemFieldData);
      this.AreaPath = service.TranslateCSSPath(valueFromPayload4);
      this.AreaUri = string.IsNullOrEmpty(this.AreaPath) ? this.AreaPath : context.AreaPathsCache.GetCssNodeAndThrow(context, this.AreaPath).Uri;
      this.Revision = this.ExtractFieldValueFromPayload<int>(context, "System.Rev", workItemFieldData);
      this.LastUpdated = this.ExtractFieldValueFromPayload<DateTime>(context, "System.ChangedDate", workItemFieldData);
      IdentityRef valueFromPayload5 = this.ExtractFieldValueFromPayload<IdentityRef>(context, "System.ChangedBy", workItemFieldData);
      if (valueFromPayload5 != null)
        this.LastUpdatedByName = CommonUtils.DistinctDisplayName(valueFromPayload5);
      this.TeamProjectName = this.ExtractFieldValueFromPayload<string>(context, "System.TeamProject", workItemFieldData);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.FromWorkItem");
    }

    protected T ExtractFieldValueFromPayload<T>(
      TestManagementRequestContext context,
      string fieldRefName,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItemFieldData)
    {
      try
      {
        T valueFromPayload = default (T);
        context.TraceEnter("BusinessLayer", nameof (ExtractFieldValueFromPayload));
        if (workItemFieldData != null)
        {
          try
          {
            object obj;
            workItemFieldData.Fields.TryGetValue(fieldRefName, out obj);
            valueFromPayload = (!(typeof (T) == typeof (int)) ? (T) obj : (T) (ValueType) Convert.ToInt32(obj)) ?? default (T);
          }
          catch (Exception ex)
          {
            context.TraceWarning("BusinessLayer", string.Format("Exception thrown while trying to fetch:{0} and cast to type:{1}", (object) fieldRefName, (object) typeof (T).ToString()));
          }
        }
        context.TraceInfo("BusinessLayer", string.Format("Return value-> data type:{0}, fieldName:{1}, value:{2}", (object) typeof (T).ToString(), (object) fieldRefName, (object) valueFromPayload));
        return valueFromPayload;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", nameof (ExtractFieldValueFromPayload));
      }
    }

    protected Dictionary<int, List<string>> GetRequiredFieldsWithPrepopulatedValues(
      TestManagementRequestContext context,
      List<int> workItemIds,
      List<string> requiredFields)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.GetRequiredFieldsWithPrepopulatedValues");
      Dictionary<int, List<string>> prepopulatedValues = new Dictionary<int, List<string>>();
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems = context.RequestContext.GetService<IWitHelper>().GetWorkItems(context.RequestContext, workItemIds, requiredFields);
      if (workItems != null)
      {
        foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem in workItems)
        {
          prepopulatedValues.Add(workItem.Id.Value, new List<string>());
          List<string> list = workItem.Fields.Keys.ToList<string>();
          foreach (string requiredField in requiredFields)
          {
            if (list.Contains(requiredField))
              prepopulatedValues[workItem.Id.Value].Add(requiredField);
          }
        }
      }
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.GetRequiredFieldsWithPrepopulatedValues");
      return prepopulatedValues;
    }

    protected virtual void PopulateRequiredFields(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString targetProject,
      int workItemId,
      string workItemTypeName,
      ref Dictionary<string, object> witFields)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.PopulateRequiredFields");
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      List<string> requiredFieldIds;
      Dictionary<string, string> fieldTdToDefaultValueMap;
      Dictionary<string, ServerDefaultFieldValue> fieldTdToServerDefaultValueMap;
      context.RequestContext.GetService<IWitHelper>().GetRequiredFieldsData(new TfsTestManagementRequestContext(context.RequestContext), teamProjectName, workItemTypeName, out requiredFieldIds, out fieldTdToDefaultValueMap, out fieldTdToServerDefaultValueMap);
      Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
      if (workItemId > 0)
      {
        foreach (string str in requiredFieldIds)
          stringList1.Add(str);
        TestManagementRequestContext context1 = context;
        List<int> workItemIds = new List<int>();
        workItemIds.Add(workItemId);
        List<string> requiredFields = stringList1;
        dictionary = this.GetRequiredFieldsWithPrepopulatedValues(context1, workItemIds, requiredFields);
      }
      else
        dictionary.Add(workItemId, new List<string>());
      foreach (string key in requiredFieldIds)
      {
        if (!witFields.ContainsKey(key) && (workItemId <= 0 || !dictionary.ContainsKey(workItemId) || !dictionary[workItemId].Contains(key)))
        {
          if (fieldTdToDefaultValueMap.ContainsKey(key))
            witFields[key] = (object) fieldTdToDefaultValueMap[key];
          else if (fieldTdToServerDefaultValueMap.ContainsKey(key))
            witFields[key] = (object) fieldTdToServerDefaultValueMap[key];
          else
            stringList2.Add(key);
        }
      }
      List<string> stringList3 = new List<string>();
      if (stringList2.Count > 0)
      {
        foreach (string key1 in stringList2)
        {
          string empty = string.Empty;
          string key2 = !stringList1.Contains(key1) ? key1 : key1;
          if (context.IsFeatureEnabled("TestManagement.Server.IgnoreWhitelistedFields") && TCMWorkItemBase.WhitelistedFieldNameDefaults.ContainsKey(key2))
            witFields[key1] = (object) TCMWorkItemBase.WhitelistedFieldNameDefaults[key2];
          else
            stringList3.Add(key2);
        }
      }
      if (stringList3.Any<string>())
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.RequiredFieldValueAbsent, (object) string.Join(",", (IEnumerable<string>) stringList3), (object) this.WitTypeName));
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.PopulateRequiredFields");
    }

    protected void CheckValueAndAddWitField(
      WITCreator witCreator,
      TestManagementRequestContext context,
      Dictionary<string, object> witFields,
      string fieldName,
      string fieldValue,
      GuidAndString targetProject)
    {
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      if (fieldValue == null)
        return;
      service.AddWitField(witCreator, context, witFields, fieldName, fieldValue, targetProject);
    }

    protected virtual void CreateWitLinks(
      TestManagementRequestContext context,
      IList<TestExternalLink> externalLinks,
      IList<WorkItemLinkInfo> witLinks,
      bool isNew,
      MigrationLogger logger,
      out List<WorkItemLinkUpdate> linkUpdateData,
      out List<WorkItemResourceLinkUpdate> resourceLinkUpdateData)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.CreateWitLinks");
      linkUpdateData = new List<WorkItemLinkUpdate>();
      resourceLinkUpdateData = new List<WorkItemResourceLinkUpdate>();
      context.TraceVerbose("BusinessLayer", "TCMWorkItemBase.CreateWitLinks Creating links for " + this.WitTypeName);
      if (witLinks != null && witLinks.Count > 0)
        linkUpdateData.AddRange((IEnumerable<WorkItemLinkUpdate>) witLinks.Select<WorkItemLinkInfo, WorkItemLinkUpdate>((Func<WorkItemLinkInfo, WorkItemLinkUpdate>) (l =>
        {
          return new WorkItemLinkUpdate()
          {
            SourceWorkItemId = l.SourceId,
            TargetWorkItemId = l.TargetId,
            UpdateType = LinkUpdateType.Add,
            LinkType = l.LinkType,
            Locked = new bool?(false)
          };
        })).ToList<WorkItemLinkUpdate>());
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.CreateWitLinks");
    }

    protected virtual void PopulateWitFields(
      TestManagementRequestContext context,
      WitOperationType witOperationType,
      bool byPass)
    {
      this.OwnerName = context.UserTeamFoundationName;
      this.OwnerDistinctName = context.UserDistinctTeamFoundationName;
      this.CreatedByName = context.UserTeamFoundationName;
      this.CreatedByDistinctName = context.UserDistinctTeamFoundationName;
      this.LastUpdatedBy = context.UserTeamFoundationId;
      this.LastUpdatedByName = context.UserTeamFoundationName;
      this.LastUpdatedByDistinctName = context.UserDistinctTeamFoundationName;
    }

    protected virtual void PopulateFieldsBeforeCreate(
      TestManagementRequestContext context,
      string teamProjectName,
      bool byPass)
    {
      this.WitTypeName = context.RequestContext.GetService<IWitHelper>().GetDefaultWorkItemTypeInCategory(context, teamProjectName, this.CategoryRefName);
      this.SetStartState(context, teamProjectName, byPass);
      this.PopulateWitFields(context, WitOperationType.WitFieldUpdate, byPass);
    }

    protected abstract void PopulateFieldsAfterCreate();

    protected abstract StateTypeEnum ToMetaState(byte tcmState);

    internal abstract byte FromMetaState(StateTypeEnum metaState);

    protected abstract StateTypeEnum GetMetaStatesFromDefaultMap(
      TestManagementRequestContext context,
      string workItemState);

    internal abstract string GetDefaultWorkItemState(
      TestManagementRequestContext context,
      byte tcmState);

    internal abstract byte GetDefaultTcmState(
      TestManagementRequestContext context,
      string workItemState);

    internal abstract IList<string> GetDefaultWorkItemStates(TestManagementRequestContext context);

    internal abstract List<StateTypeEnumAndStateString> GetDefaultStatesMap();

    protected abstract Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State[] GetMappedWitStates(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig);

    protected abstract void PopulateWitId(
      TestManagementRequestContext context,
      int id,
      int revision);

    internal abstract byte GetTCMState(
      IReadOnlyCollection<WorkItemStateDefinition> possibleStates,
      string workItemState);

    internal virtual IList<string> GetWorkItemStates(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig)
    {
      return projProcConfig != null ? this.GetWorkItemStatesFromProcessConfiguration(context, projProcConfig) : this.GetDefaultWorkItemStates(context);
    }

    internal virtual IList<string> GetWorkItemState(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig,
      byte tcmState)
    {
      IList<string> workItemState;
      if (projProcConfig != null)
        workItemState = this.GetWorkItemStatesFromProcessConfiguration(context, projProcConfig, tcmState);
      else
        workItemState = (IList<string>) new List<string>()
        {
          this.GetDefaultWorkItemState(context, tcmState)
        };
      return workItemState;
    }

    protected StateTypeEnum GetMetaStates(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig,
      string workItemState)
    {
      return projProcConfig != null ? this.GetMetaStatesFromProcessConfiguration(context, projProcConfig, workItemState) : this.GetMetaStatesFromDefaultMap(context, workItemState);
    }

    internal virtual IList<string> GetWorkItemStatesFromProcessConfiguration(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig,
      byte tcmState)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.GetWorkItemStatesFromProcessConfiguration");
      StateTypeEnum metaState = this.ToMetaState(tcmState);
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State[] mappedWitStates = this.GetMappedWitStates(context, projProcConfig);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.GetWorkItemStatesFromProcessConfiguration");
      Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, bool> predicate = (Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, bool>) (stateInfo => stateInfo.Type == metaState);
      return (IList<string>) ((IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>) mappedWitStates).Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>(predicate).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, string>) (stateInfo => stateInfo.Value)).ToList<string>();
    }

    internal virtual IList<string> GetWorkItemStatesFromProcessConfiguration(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.GetWorkItemStatesFromProcessConfiguration");
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State[] mappedWitStates = this.GetMappedWitStates(context, projProcConfig);
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.GetWorkItemStatesFromProcessConfiguration");
      return (IList<string>) ((IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>) mappedWitStates).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, string>) (stateInfo => stateInfo.Value)).ToList<string>();
    }

    protected virtual StateTypeEnum GetMetaStatesFromProcessConfiguration(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig,
      string workItemState)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.GetMetaStatesFromProcessConfiguration");
      StateTypeEnum processConfiguration = (StateTypeEnum) 0;
      IEnumerable<StateTypeEnum> source = ((IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>) this.GetMappedWitStates(context, projProcConfig)).Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, bool>) (stateInfo => string.Equals(stateInfo.Value, workItemState, StringComparison.CurrentCultureIgnoreCase))).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, StateTypeEnum>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, StateTypeEnum>) (stateInfo => stateInfo.Type));
      if (source != null && source.Count<StateTypeEnum>() > 0)
        processConfiguration = source.First<StateTypeEnum>();
      context.TraceInfo("BusinessLayer", "MetaState: {0}", (object) processConfiguration.ToString());
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.GetMetaStatesFromProcessConfiguration");
      return processConfiguration;
    }

    internal virtual string GetWorkItemStateFromProcessConfiguration(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig,
      byte tcmState,
      bool autoResolveMultiStateMappingAmbiguity)
    {
      if (projProcConfig != null)
      {
        IList<string> processConfiguration = this.GetWorkItemStatesFromProcessConfiguration(context, projProcConfig, tcmState);
        if (processConfiguration.Count > 0)
        {
          if (processConfiguration.Count <= 1 || autoResolveMultiStateMappingAmbiguity)
            return processConfiguration.First<string>();
          if (this.CategoryRefName == WitCategoryRefName.TestPlan)
            throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.MultipleTestPlanWitStatesMappedToSameMetaStateException, (object) this.ToMetaState(tcmState)));
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.MultipleTestSuiteWitStatesMappedToSameMetaStateException, (object) this.ToMetaState(tcmState)));
        }
      }
      return (string) null;
    }

    protected static void LogInfo(MigrationLogger logger, string message) => logger?.Log(TraceLevel.Info, message);

    protected virtual void CopyProperties(
      TestManagementRequestContext context,
      TCMWorkItemBase tcmWit)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.CopyProperties");
      this.Id = tcmWit.Id;
      this.Title = tcmWit.Title;
      this.OwnerName = tcmWit.OwnerName;
      this.OwnerDistinctName = tcmWit.OwnerDistinctName;
      this.CreatedByName = tcmWit.CreatedByName;
      this.CreatedByDistinctName = tcmWit.CreatedByDistinctName;
      this.WitTypeName = tcmWit.WitTypeName;
      this.State = tcmWit.State;
      this.Iteration = tcmWit.Iteration;
      this.AreaPath = tcmWit.AreaPath;
      this.AreaUri = tcmWit.AreaUri;
      this.Revision = tcmWit.Revision;
      this.LastUpdated = tcmWit.LastUpdated;
      this.LastUpdatedByName = tcmWit.LastUpdatedByName;
      this.LastUpdatedByDistinctName = tcmWit.LastUpdatedByDistinctName;
      this.TeamProjectName = tcmWit.TeamProjectName;
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.CopyProperties");
    }

    internal virtual void ValidateProcessConfigMappingExistsForAllWorkItemStates(
      TestManagementRequestContext context,
      string projectUri,
      string projectName,
      string witType)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.ValidateProcessConfigMappingExistsForAllWorkItemStates");
      IList<string> workItemStates = ProcessConfigurationHelper.GetWorkItemStates(context, projectUri, (byte) 0, this.CategoryRefName);
      IList<string> states = context.RequestContext.GetService<IWitHelper>().GetStates(context, projectName, witType);
      if (states.Except<string>((IEnumerable<string>) workItemStates).Count<string>() != 0)
      {
        string str = string.Join(",", (IEnumerable<string>) states);
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CouldNotFindStateMappingsInProjectProcessConfig, (object) witType, (object) this.ProcessConfigCategoryName, (object) str));
      }
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.ValidateProcessConfigMappingExistsForAllWorkItemStates");
    }

    internal virtual void ValidateDefaultStateMapsToInProgressMetaState(
      TestManagementRequestContext context,
      string projectUri,
      string projectName,
      string witType)
    {
      context.TraceEnter("BusinessLayer", "TCMWorkItemBase.ValidateDefaultStateMapsToInProgressMetaState");
      string workItemStartState = context.RequestContext.GetService<IWitHelper>().GetWorkItemStartState(context, projectName, witType);
      if (!ProcessConfigurationHelper.GetWorkItemStates(context, projectUri, this.DefaultTCMState, this.CategoryRefName).Contains(workItemStartState))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DefaultStateNotMappedToInProgressInProjectProcessConfig, (object) workItemStartState, (object) witType));
      context.TraceLeave("BusinessLayer", "TCMWorkItemBase.ValidateDefaultStateMapsToInProgressMetaState");
    }

    public int Id { get; set; }

    public int Revision { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string EncodedDescription { get; set; }

    public string AreaPath { get; set; }

    public string Iteration { get; set; }

    public string State { get; set; }

    public Guid LastUpdatedBy { get; set; }

    public string LastUpdatedByName { get; set; }

    public string LastUpdatedByDistinctName { get; set; }

    public string OwnerName { get; set; }

    public string OwnerDistinctName { get; set; }

    public DateTime LastUpdated { get; set; }

    public string TeamFieldName { get; set; }

    public string TeamFieldDefaultValue { get; set; }

    public string WitTypeName { get; set; }

    public abstract string ProcessConfigCategoryName { get; }

    public string CreatedByName { get; set; }

    public string CreatedByDistinctName { get; set; }

    public string Reason { get; set; }

    public string AreaUri { get; set; }

    public string TeamProjectName { get; set; }

    public string CategoryRefName { get; protected set; }

    public string CategoryName { get; protected set; }

    public string StartState { get; protected set; }

    protected abstract string[] WitFields { get; }

    protected abstract byte DefaultTCMState { get; }

    protected ObjectTypes TCMObjectType { get; set; }

    internal static string GetFirstNonEmptyValue(string firstValue, string secondValue) => !string.IsNullOrEmpty(firstValue) ? firstValue : secondValue;
  }
}
