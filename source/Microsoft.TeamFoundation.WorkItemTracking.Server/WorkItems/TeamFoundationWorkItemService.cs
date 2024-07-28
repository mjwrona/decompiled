// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.TeamFoundationWorkItemService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ServiceModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.SecretsScan;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Telemetry;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Mru;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  public class TeamFoundationWorkItemService : 
    ITeamFoundationWorkItemService,
    IVssFrameworkService,
    IDisposable
  {
    private readonly string m_attachmentHandler = "AttachFileHandler.ashx";
    private readonly string m_witPath = "WorkItemTracking";
    private readonly string m_witPathWithDefaultCollection = "DefaultCollection/WorkItemTracking";
    private readonly string[] targetTags = new string[12]
    {
      "div",
      "p",
      "li",
      "td",
      "ul",
      "ol",
      "h1",
      "h2",
      "h3",
      "h4",
      "h5",
      "h6"
    };
    private readonly IWorkItemChangedEventNotifier m_updateEventNotifier = (IWorkItemChangedEventNotifier) new WorkItemChangedEventNotifier();
    private readonly string c_IterationId = "System.IterationId";
    private readonly string c_IterationPath = "System.IterationPath";
    private readonly string c_TargetDate = "Microsoft.VSTS.Scheduling.TargetDate";
    private readonly IReadOnlyCollection<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType> m_whitelistedOutboundLinks = (IReadOnlyCollection<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType>) new Microsoft.TeamFoundation.Framework.Server.OutboundLinkType[3]
    {
      new Microsoft.TeamFoundation.Framework.Server.OutboundLinkType("Branch", "Git", "Ref"),
      new Microsoft.TeamFoundation.Framework.Server.OutboundLinkType("Tag", "Git", "Ref"),
      new Microsoft.TeamFoundation.Framework.Server.OutboundLinkType("Source Code File", "VersionControl", "VersionedItem")
    };
    internal const string c_fileGuidSegment = "?FileNameGUID=";
    internal const string c_projectAttachmentGuidSeparator = "/";
    public const int PageWorkItemsDefaultBatchSize = 200;
    private IDisposableReadOnlyList<ILongTextValueConverter> m_longTextValueConverters;
    private const int c_guidLength = 36;
    private static readonly int s_fileGuidSegmentLength = "?FileNameGUID=".Length;
    private static readonly int s_projectAttachmentGuidSeparatorLength = "/".Length;
    private const string AllMyWorkItemsQuery = "SELECT [System.Id], [System.AreaId] FROM WorkItems WHERE [System.AssignedTo] = @me ORDER BY [System.ChangedDate] DESC";
    private WorkItemChangedEventServiceBusPublisher m_serviceBusEventPublisher;

    public IEnumerable<WorkItemUpdateResult> DeleteWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      bool skipNotifications = false,
      bool skipTestWorkItems = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      return !workItemIds.Any<int>() ? Enumerable.Empty<WorkItemUpdateResult>() : requestContext.TraceBlock<IEnumerable<WorkItemUpdateResult>>(904841, 904842, "Services", "WorkItemService", nameof (DeleteWorkItems), (Func<IEnumerable<WorkItemUpdateResult>>) (() =>
      {
        List<WorkItem> list = this.GetWorkItems(requestContext, workItemIds, true, true, false, TeamFoundationWorkItemService.ShouldLoadTags(requestContext), WorkItemRetrievalMode.NonDeleted, WorkItemErrorPolicy.Fail, false, false, false, new DateTime?()).ToList<WorkItem>();
        Dictionary<Guid, HashSet<string>> projectToNonRemovableTypesMap = (Dictionary<Guid, HashSet<string>>) null;
        Dictionary<int, string> workItemLinkTypeMap = (Dictionary<int, string>) null;
        this.PrepareWorkItemTypeCheck(requestContext, (IEnumerable<WorkItem>) list, out projectToNonRemovableTypesMap, out workItemLinkTypeMap, true);
        List<WorkItemUpdate> source = new List<WorkItemUpdate>();
        List<WorkItemUpdateResult> itemUpdateResultList = new List<WorkItemUpdateResult>();
        foreach (WorkItem workItem in list)
        {
          WorkItemTrackingServiceException nonDeletableException;
          if (this.TryCheckWorkItemIsDeletable(requestContext, workItem, projectToNonRemovableTypesMap, workItemLinkTypeMap, skipTestWorkItems, out nonDeletableException))
          {
            WorkItemUpdate forDeleteOrRestore = this.CreateWorkItemUpdateForDeleteOrRestore(requestContext.WitContext(), workItem, WorkItemUpdateMode.Delete);
            forDeleteOrRestore.LinkUpdates = this.CreateWorkItemLinkUpdatesForDeleteOrRestore(requestContext.WitContext(), (WorkItemRevision) workItem, WorkItemUpdateMode.Delete);
            forDeleteOrRestore.ResourceLinkUpdates = this.CreateWorkItemResourceLinkUpdatesForDeleteOrRestore((WorkItemRevision) workItem, WorkItemUpdateMode.Delete);
            source.Add(forDeleteOrRestore);
          }
          else
          {
            WorkItemUpdateResult itemUpdateResult = new WorkItemUpdateResult()
            {
              Id = workItem.Id,
              UpdateId = workItem.Id,
              Rev = workItem.Revision
            };
            itemUpdateResult.AddException((TeamFoundationServiceException) nonDeletableException);
            itemUpdateResultList.Add(itemUpdateResult);
          }
        }
        if (source.Any<WorkItemUpdate>())
          itemUpdateResultList = this.UpdateWorkItemsInternal(requestContext.WitContext(), (IEnumerable<WorkItemUpdate>) source, WorkItemUpdateRuleExecutionMode.Bypass, updateMode: WorkItemUpdateMode.Delete, skipAllNotifications: skipNotifications).Concat<WorkItemUpdateResult>((IEnumerable<WorkItemUpdateResult>) itemUpdateResultList).ToList<WorkItemUpdateResult>();
        if (requestContext.WitContext().ServerSettings.EventsEnabled)
        {
          try
          {
            requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<ITeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((context, ignored) => context.GetService<ITeamFoundationEventService>().PublishNotification(context, (object) new WorkItemsDeletedNotification())));
          }
          catch (Exception ex)
          {
            requestContext.TraceException(904851, "Services", "WorkItemService", ex);
          }
        }
        Dictionary<int, WorkItemUpdateResult> resultMap = itemUpdateResultList.ToDictionary<WorkItemUpdateResult, int, WorkItemUpdateResult>((Func<WorkItemUpdateResult, int>) (key => key.Id), (Func<WorkItemUpdateResult, WorkItemUpdateResult>) (value => value));
        return workItemIds.Select<int, WorkItemUpdateResult>((Func<int, WorkItemUpdateResult>) (id => resultMap[id]));
      }));
    }

    public IEnumerable<WorkItemUpdateResult> RestoreWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      return !workItemIds.Any<int>() ? Enumerable.Empty<WorkItemUpdateResult>() : requestContext.TraceBlock<IEnumerable<WorkItemUpdateResult>>(904843, 904844, "Services", "WorkItemService", nameof (RestoreWorkItems), (Func<IEnumerable<WorkItemUpdateResult>>) (() =>
      {
        List<WorkItem> list1 = this.GetWorkItems(requestContext, workItemIds.Distinct<int>(), true, true, true, TeamFoundationWorkItemService.ShouldLoadTags(requestContext), WorkItemRetrievalMode.Deleted, WorkItemErrorPolicy.Fail, false, false, false, new DateTime?()).ToList<WorkItem>();
        try
        {
          Dictionary<int, WorkItemUpdate> dictionary1 = new Dictionary<int, WorkItemUpdate>();
          foreach (WorkItem workItem in list1)
          {
            WorkItemRevision revision = workItem.Revisions[workItem.Revision - 2];
            WorkItemUpdate forDeleteOrRestore = this.CreateWorkItemUpdateForDeleteOrRestore(requestContext.WitContext(), workItem, WorkItemUpdateMode.Restore);
            forDeleteOrRestore.LinkUpdates = this.CreateWorkItemLinkUpdatesForDeleteOrRestore(requestContext.WitContext(), revision, WorkItemUpdateMode.Restore);
            forDeleteOrRestore.ResourceLinkUpdates = this.CreateWorkItemResourceLinkUpdatesForDeleteOrRestore(revision, WorkItemUpdateMode.Restore);
            dictionary1.Add(workItem.Id, forDeleteOrRestore);
          }
          IEnumerable<WorkItemUpdate> values = (IEnumerable<WorkItemUpdate>) dictionary1.Values;
          IEnumerable<WorkItemUpdateResult> source1 = this.UpdateWorkItemsInternal(requestContext.WitContext(), values, WorkItemUpdateRuleExecutionMode.Bypass, updateMode: WorkItemUpdateMode.Restore, includeInRecentActivity: true);
          List<WorkItemUpdateResult> list2 = source1.Where<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (r => r.Exception != null)).ToList<WorkItemUpdateResult>();
          if (!list2.Any<WorkItemUpdateResult>())
            return source1;
          Dictionary<int, WorkItemUpdateResult> dictionary2 = source1.ToDictionary<WorkItemUpdateResult, int>((Func<WorkItemUpdateResult, int>) (r => r.Id));
          WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, WorkItemRestoreTelemetry.Feature, (object) list2, (object) false);
          List<WorkItemUpdate> source2 = new List<WorkItemUpdate>();
          foreach (WorkItemUpdateResult itemUpdateResult in list2)
          {
            WorkItemUpdate workItemUpdate;
            if (dictionary1.TryGetValue(itemUpdateResult.Id, out workItemUpdate))
            {
              workItemUpdate.LinkUpdates = (IEnumerable<WorkItemLinkUpdate>) new List<WorkItemLinkUpdate>();
              workItemUpdate.Fields.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value)).Add("System.History", (object) Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemRestoreLinksUnrecoverableComment((object) (workItemUpdate.Rev + 1)));
              source2.Add(workItemUpdate);
            }
          }
          if (source2.Any<WorkItemUpdate>())
          {
            IEnumerable<WorkItemUpdateResult> source3 = this.UpdateWorkItemsInternal(requestContext.WitContext(), (IEnumerable<WorkItemUpdate>) source2, WorkItemUpdateRuleExecutionMode.Bypass, updateMode: WorkItemUpdateMode.Restore, includeInRecentActivity: true);
            foreach (WorkItemUpdateResult itemUpdateResult in source3)
              dictionary2[itemUpdateResult.Id] = itemUpdateResult;
            List<WorkItemUpdateResult> list3 = source3.Where<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (r => r.Exception != null)).ToList<WorkItemUpdateResult>();
            if (list3.Any<WorkItemUpdateResult>())
              WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, WorkItemRestoreTelemetry.Feature, (object) list3, (object) true);
          }
          if (requestContext.WitContext().ServerSettings.EventsEnabled)
          {
            try
            {
              requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<TeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((context, ignored) => context.GetService<ITeamFoundationEventService>().PublishNotification(context, (object) new WorkItemsRestoredNotification())));
            }
            catch (Exception ex)
            {
              requestContext.TraceException(904852, "Services", "WorkItemService", ex);
            }
          }
          return (IEnumerable<WorkItemUpdateResult>) dictionary2.Values;
        }
        catch (Exception ex)
        {
          throw new WorkItemInvalidRestoreRequestException(workItemIds, ex);
        }
      }));
    }

    public void DestroyWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      bool skipNotifications = false,
      bool skipTestWorkItems = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      if (!workItemIds.Any<int>())
        return;
      requestContext.TraceBlock(904500, 904510, "Services", "WorkItemService", nameof (DestroyWorkItems), (Action) (() =>
      {
        workItemIds = (IEnumerable<int>) workItemIds.Distinct<int>().ToArray<int>();
        List<string> fields = new List<string>()
        {
          "System.Id",
          "System.TeamProject"
        };
        if (skipTestWorkItems)
          fields.Add("System.WorkItemType");
        IEnumerable<WorkItemFieldData> workItemFieldValues = this.GetWorkItemFieldValues(requestContext, workItemIds, (IEnumerable<string>) fields, 16, new DateTime?(), 200, WorkItemRetrievalMode.All, false, false);
        Dictionary<int, \u003C\u003Ef__AnonymousType58<Guid, string>> workItemIdDictionary = workItemFieldValues.ToDictionary((Func<WorkItemFieldData, int>) (wifv => wifv.Id), wifv => new
        {
          ProjectId = wifv.GetProjectGuid(requestContext),
          WorkItemType = wifv.WorkItemType
        });
        IEnumerable<int> ints1 = workItemIds.Where<int>((Func<int, bool>) (id => !workItemIdDictionary.ContainsKey(id)));
        if (ints1.Any<int>())
          throw new WorkItemDestroyException(ints1);
        if (skipTestWorkItems)
        {
          List<int> intList = new List<int>();
          foreach (int workItemId in workItemIds)
          {
            var data;
            if (workItemIdDictionary.TryGetValue(workItemId, out data) && CommonWITUtils.IsTestWorkItem(requestContext, data.ProjectId, data.WorkItemType))
              intList.Add(workItemId);
          }
          if (intList.Any<int>())
            throw new WorkItemDestroyException((IEnumerable<int>) intList);
        }
        if (workItemFieldValues.Any<WorkItemFieldData>((Func<WorkItemFieldData, bool>) (wifv => !requestContext.WitContext().WorkItemProjectPermissionChecker.HasWorkItemPermission(wifv.AreaId, AuthorizationProjectPermissions.WorkItemPermanentlyDelete))))
          throw new WorkItemDestroyException(workItemIds);
        int batchSize = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/WorkItemDestroyBatchSize", 2048);
        IEnumerable<int> ints2;
        using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
          ints2 = component.DestroyWorkItems((IVssIdentity) requestContext.GetUserIdentity(), (IEnumerable<int>) workItemIds.ToArray<int>(), batchSize);
        if (ints2.Any<int>())
          throw new WorkItemDestroyException(ints2);
        WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
        Dictionary<int, Guid> workItemProjectIdDictionary = workItemIdDictionary.ToDictionary<KeyValuePair<int, \u003C\u003Ef__AnonymousType58<Guid, string>>, int, Guid>(item => item.Key, item => item.Value.ProjectId);
        if (skipNotifications)
        {
          if (workItemFieldValues.Any<WorkItemFieldData>((Func<WorkItemFieldData, bool>) (wifv => !requestContext.WitContext().WorkItemProjectPermissionChecker.HasWorkItemPermission(wifv.AreaId, AuthorizationProjectPermissions.SuppressNotifications))))
            throw new WorkItemUnauthorizedSuppressNotificationsException();
        }
        else
          this.TryFireWorkItemDestroyedEvents(witRequestContext, this.m_serviceBusEventPublisher, workItemProjectIdDictionary);
        if (witRequestContext.ServerSettings.EventsEnabled)
        {
          try
          {
            requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<ITeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((context, ignored) => context.GetService<ITeamFoundationEventService>().PublishNotification(context, (object) new WorkItemsDestroyedNotification(workItemProjectIdDictionary))));
          }
          catch (Exception ex)
          {
            requestContext.TraceException(904501, "Services", "WorkItemService", ex);
          }
        }
        requestContext.GetService<ITeamFoundationTaggingService>().DeleteTagHistoryForArtifacts<int>(requestContext.Elevate(), WorkItemArtifactKinds.WorkItem, workItemIds.Select<int, TagArtifact<int>>((Func<int, TagArtifact<int>>) (wiid => new TagArtifact<int>(workItemProjectIdDictionary[wiid], wiid))));
      }));
    }

    internal virtual IEnumerable<WorkItemChangedEventExtended> TryFireWorkItemDestroyedEvents(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemChangedEventServiceBusPublisher serviceBusEventPublisher,
      Dictionary<int, Guid> workItemIdProjectIdDictionary)
    {
      return WorkItemChangedEvent.TryFireWorkItemDestroyedEvents(witRequestContext, serviceBusEventPublisher, workItemIdProjectIdDictionary);
    }

    internal void CheckIsDeletedFieldValue(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState,
      WorkItemUpdateMode updateMode)
    {
      if (updateMode != WorkItemUpdateMode.Normal || !updateState.HasFieldUpdate(-404))
        return;
      object fieldValue1 = updateState.FieldData.GetFieldValue(witRequestContext.RequestContext, -404, true);
      object fieldValue2 = updateState.FieldData.GetFieldValue(witRequestContext.RequestContext, -404, false);
      if ((!updateState.Update.IsNew || !(fieldValue2 is bool flag1) || !flag1) && (updateState.Update.IsNew || !(fieldValue1 is bool flag2) || flag2 == (bool) fieldValue2))
        return;
      updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemInvalidUpdateToIsDeletedFieldException(updateState.Id));
    }

    internal HashSet<string> GetNonDeletableWorkItemTypes(
      IVssRequestContext requestContext,
      Guid projectGuid)
    {
      return new HashSet<string>(this.GetCodeReviewAndFeedbackWorkItemTypeNames(requestContext.GetService<IWorkItemTypeCategoryService>().LegacyGetWorkItemTypeCategories(requestContext, projectGuid, true)), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
    }

    private IEnumerable<string> GetTestWorkItemTypeNames(
      IEnumerable<WorkItemTypeCategory> categories)
    {
      IEnumerable<string> first = (IEnumerable<string>) new HashSet<string>();
      WorkItemTypeCategory itemTypeCategory1 = categories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryReferenceName.Equals(c.ReferenceName, "Microsoft.HiddenCategory")));
      if (itemTypeCategory1 != null)
        first = first.Union<string>(itemTypeCategory1.WorkItemTypeNames);
      WorkItemTypeCategory itemTypeCategory2 = categories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryReferenceName.Equals(c.ReferenceName, "Microsoft.TestCaseCategory")));
      if (itemTypeCategory2 != null)
        first = first.Union<string>(itemTypeCategory2.WorkItemTypeNames);
      WorkItemTypeCategory itemTypeCategory3 = categories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryReferenceName.Equals(c.ReferenceName, "Microsoft.TestPlanCategory")));
      if (itemTypeCategory3 != null)
        first = first.Union<string>(itemTypeCategory3.WorkItemTypeNames);
      WorkItemTypeCategory itemTypeCategory4 = categories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryReferenceName.Equals(c.ReferenceName, "Microsoft.TestSuiteCategory")));
      if (itemTypeCategory4 != null)
        first = first.Union<string>(itemTypeCategory4.WorkItemTypeNames);
      return first;
    }

    private IEnumerable<string> GetCodeReviewAndFeedbackWorkItemTypeNames(
      IEnumerable<WorkItemTypeCategory> categories)
    {
      IEnumerable<string> first = (IEnumerable<string>) new HashSet<string>();
      WorkItemTypeCategory itemTypeCategory1 = categories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryReferenceName.Equals(c.ReferenceName, "Microsoft.FeedbackRequestCategory")));
      if (itemTypeCategory1 != null)
        first = first.Union<string>(itemTypeCategory1.WorkItemTypeNames);
      WorkItemTypeCategory itemTypeCategory2 = categories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryReferenceName.Equals(c.ReferenceName, "Microsoft.FeedbackResponseCategory")));
      if (itemTypeCategory2 != null)
        first = first.Union<string>(itemTypeCategory2.WorkItemTypeNames);
      WorkItemTypeCategory itemTypeCategory3 = categories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryReferenceName.Equals(c.ReferenceName, "Microsoft.CodeReviewRequestCategory")));
      if (itemTypeCategory3 != null)
        first = first.Union<string>(itemTypeCategory3.WorkItemTypeNames);
      WorkItemTypeCategory itemTypeCategory4 = categories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryReferenceName.Equals(c.ReferenceName, "Microsoft.CodeReviewResponseCategory")));
      if (itemTypeCategory4 != null)
        first = first.Union<string>(itemTypeCategory4.WorkItemTypeNames);
      return first;
    }

    private WorkItemUpdate CreateWorkItemUpdateForDeleteOrRestore(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItem workItem,
      WorkItemUpdateMode updateMode)
    {
      if (updateMode == WorkItemUpdateMode.Normal)
        throw new WorkItemInvalidUpdateToIsDeletedFieldException(workItem.Id);
      return new WorkItemUpdate()
      {
        Id = workItem.Id,
        Rev = workItem.Revision,
        FieldData = (WorkItemFieldData) workItem,
        Fields = (IEnumerable<KeyValuePair<string, object>>) new Dictionary<string, object>()
        {
          {
            "System.IsDeleted",
            (object) (updateMode == WorkItemUpdateMode.Delete)
          }
        }
      };
    }

    private IEnumerable<WorkItemLinkUpdate> CreateWorkItemLinkUpdatesForDeleteOrRestore(
      WorkItemTrackingRequestContext witContext,
      WorkItemRevision item,
      WorkItemUpdateMode updateMode)
    {
      if (updateMode == WorkItemUpdateMode.Normal)
        throw new WorkItemInvalidUpdateToIsDeletedFieldException(item.Id);
      List<WorkItemLinkUpdate> forDeleteOrRestore = new List<WorkItemLinkUpdate>();
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo workItemLink in item.WorkItemLinks)
      {
        MDWorkItemLinkType linkType;
        if (witContext.LinkService.TryGetLinkTypeById(witContext.RequestContext, workItemLink.LinkType, out linkType))
        {
          if (!linkType.IsRemote || updateMode == WorkItemUpdateMode.Delete)
          {
            WorkItemLinkUpdate workItemLinkUpdate1 = new WorkItemLinkUpdate();
            workItemLinkUpdate1.SourceWorkItemId = workItemLink.SourceId;
            workItemLinkUpdate1.TargetWorkItemId = workItemLink.TargetId;
            workItemLinkUpdate1.Locked = new bool?(workItemLink.IsLocked);
            workItemLinkUpdate1.LinkType = workItemLink.LinkType;
            workItemLinkUpdate1.Comment = workItemLink.Comment;
            workItemLinkUpdate1.UpdateType = updateMode == WorkItemUpdateMode.Delete ? LinkUpdateType.Delete : LinkUpdateType.Add;
            workItemLinkUpdate1.RemoteHostId = workItemLink.RemoteHostId;
            workItemLinkUpdate1.RemoteProjectId = workItemLink.RemoteProjectId;
            workItemLinkUpdate1.RemoteStatus = linkType.IsRemote ? new RemoteStatus?(RemoteStatus.PendingDelete) : workItemLink.RemoteStatus;
            workItemLinkUpdate1.RemoteStatusMessage = workItemLink.RemoteStatusMessage;
            workItemLinkUpdate1.RemoteWatermark = workItemLink.RemoteWatermark;
            WorkItemLinkUpdate workItemLinkUpdate2 = workItemLinkUpdate1;
            forDeleteOrRestore.Add(workItemLinkUpdate2);
          }
        }
        else
        {
          var data = new
          {
            SourceWorkItemId = workItemLink.SourceId,
            TargetWorkItemId = workItemLink.TargetId,
            LinkTypeId = workItemLink.LinkType
          };
          WorkItemTrackingTelemetry.TraceCustomerIntelligence(witContext.RequestContext, WorkItemRestoreTelemetry.Feature, (object) data);
        }
      }
      return (IEnumerable<WorkItemLinkUpdate>) forDeleteOrRestore;
    }

    private IEnumerable<WorkItemResourceLinkUpdate> CreateWorkItemResourceLinkUpdatesForDeleteOrRestore(
      WorkItemRevision item,
      WorkItemUpdateMode updateMode)
    {
      if (updateMode == WorkItemUpdateMode.Normal)
        throw new WorkItemInvalidUpdateToIsDeletedFieldException(item.Id);
      List<WorkItemResourceLinkUpdate> forDeleteOrRestore = new List<WorkItemResourceLinkUpdate>();
      foreach (WorkItemResourceLinkInfo resourceLink in item.ResourceLinks)
      {
        if (resourceLink.ResourceType == ResourceLinkType.ArtifactLink)
        {
          WorkItemResourceLinkUpdate resourceLinkUpdate1 = new WorkItemResourceLinkUpdate();
          resourceLinkUpdate1.SourceWorkItemId = resourceLink.SourceId;
          resourceLinkUpdate1.Name = resourceLink.Name;
          resourceLinkUpdate1.Location = resourceLink.Location;
          resourceLinkUpdate1.Comment = resourceLink.Comment;
          resourceLinkUpdate1.Type = new ResourceLinkType?(resourceLink.ResourceType);
          resourceLinkUpdate1.UpdateType = updateMode == WorkItemUpdateMode.Delete ? LinkUpdateType.Delete : LinkUpdateType.Add;
          WorkItemResourceLinkUpdate resourceLinkUpdate2 = resourceLinkUpdate1;
          if (updateMode == WorkItemUpdateMode.Delete)
            resourceLinkUpdate2.ResourceId = new int?(resourceLink.ResourceId);
          forDeleteOrRestore.Add(resourceLinkUpdate2);
        }
      }
      return (IEnumerable<WorkItemResourceLinkUpdate>) forDeleteOrRestore;
    }

    private bool TryCheckWorkItemIsDeletable(
      IVssRequestContext requestContext,
      WorkItem item,
      Dictionary<Guid, HashSet<string>> projectToNonDeletableTypesMap,
      Dictionary<int, string> workItemLinkTypeMap,
      bool skipTestWorkItems,
      out WorkItemTrackingServiceException nonDeletableException)
    {
      bool hasNonRemovableLink = false;
      nonDeletableException = (WorkItemTrackingServiceException) null;
      if (skipTestWorkItems)
      {
        Guid projectGuid = item.GetProjectGuid(requestContext);
        if (CommonWITUtils.IsTestWorkItem(requestContext, projectGuid, item.WorkItemType))
        {
          nonDeletableException = (WorkItemTrackingServiceException) new WorkItemNonDeletableException(item.Id, item.WorkItemType);
          return false;
        }
      }
      string nonRemovableReason;
      if (this.TryCheckWorkItemIsRemovable(requestContext, item, projectToNonDeletableTypesMap, workItemLinkTypeMap, out hasNonRemovableLink, out nonRemovableReason))
        return true;
      nonDeletableException = !hasNonRemovableLink ? (WorkItemTrackingServiceException) new WorkItemNonDeletableException(item.Id, nonRemovableReason) : (WorkItemTrackingServiceException) new WorkItemNonDeletableLinkException(item.Id, nonRemovableReason);
      return false;
    }

    private bool TryCheckWorkItemIsRemovable(
      IVssRequestContext requestContext,
      WorkItem item,
      Dictionary<Guid, HashSet<string>> projectToNonDeletableTypesMap,
      Dictionary<int, string> workItemLinkTypeMap,
      out bool hasNonRemovableLink,
      out string nonRemovableReason,
      bool calledFromMovableCheck = false)
    {
      nonRemovableReason = (string) null;
      hasNonRemovableLink = false;
      Guid projectGuid = item.GetProjectGuid(requestContext);
      WorkItemTrackingLinkService service = requestContext.GetService<WorkItemTrackingLinkService>();
      if (projectToNonDeletableTypesMap[projectGuid].Contains(item.WorkItemType))
      {
        nonRemovableReason = item.WorkItemType;
        return false;
      }
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo workItemLink in item.WorkItemLinks)
      {
        int key = workItemLink.TargetId == item.Id ? workItemLink.SourceId : workItemLink.TargetId;
        MDWorkItemLinkType linkTypeById = service.GetLinkTypeById(requestContext, workItemLink.LinkType);
        WorkItemLinkTypeEnd linkTypeEndById = service.GetLinkTypeEndById(requestContext, workItemLink.LinkType);
        if (linkTypeById.IsRemote)
        {
          if (!calledFromMovableCheck)
          {
            nonRemovableReason = linkTypeEndById.IsForwardEnd ? linkTypeById.ForwardEndName.ToLower() : linkTypeById.ReverseEndName.ToLower();
            hasNonRemovableLink = true;
            return false;
          }
        }
        else if (projectToNonDeletableTypesMap[projectGuid].Contains(workItemLinkTypeMap[key]))
        {
          nonRemovableReason = workItemLinkTypeMap[key];
          hasNonRemovableLink = true;
          return false;
        }
      }
      return true;
    }

    private void PrepareWorkItemTypeCheck(
      IVssRequestContext requestContext,
      IEnumerable<WorkItem> workItems,
      out Dictionary<Guid, HashSet<string>> projectToNonRemovableTypesMap,
      out Dictionary<int, string> workItemLinkTypeMap,
      bool isWorkItemsDelete,
      Dictionary<int, WorkItemUpdateState> updateStateDictionary = null)
    {
      projectToNonRemovableTypesMap = new Dictionary<Guid, HashSet<string>>();
      workItemLinkTypeMap = new Dictionary<int, string>();
      List<int> source1 = new List<int>();
      WorkItemTrackingLinkService service = requestContext.GetService<WorkItemTrackingLinkService>();
      foreach (WorkItem workItem in workItems)
      {
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo workItemLink in workItem.WorkItemLinks)
        {
          if (!service.GetLinkTypeById(requestContext, workItemLink.LinkType).IsRemote)
            source1.Add(workItemLink.TargetId == workItem.Id ? workItemLink.SourceId : workItemLink.TargetId);
        }
        Guid projectGuid = workItem.GetProjectGuid(requestContext);
        if (!projectToNonRemovableTypesMap.ContainsKey(projectGuid))
        {
          if (isWorkItemsDelete)
          {
            HashSet<string> deletableWorkItemTypes = this.GetNonDeletableWorkItemTypes(requestContext, projectGuid);
            projectToNonRemovableTypesMap.Add(projectGuid, deletableWorkItemTypes);
          }
          else
          {
            bool hasTeamProjectChanged = updateStateDictionary != null && updateStateDictionary[workItem.Id].HasTeamProjectChanged;
            HashSet<string> movableWorkItemTypes = this.GetNonMovableWorkItemTypes(requestContext, projectGuid, hasTeamProjectChanged);
            projectToNonRemovableTypesMap.Add(projectGuid, movableWorkItemTypes);
          }
        }
      }
      IEnumerable<int> source2 = source1.Distinct<int>();
      int count1 = 200;
      for (int count2 = 0; count2 < source2.Count<int>(); count2 += count1)
      {
        IEnumerable<int> workItemIds = source2.Skip<int>(count2).Take<int>(count1);
        foreach (WorkItemFieldData workItemFieldData in (IEnumerable<WorkItemFieldData>) this.GetWorkItemFieldValues(requestContext, workItemIds, (IEnumerable<int>) new int[1]
        {
          25
        }, 0, new DateTime?(), 200, WorkItemRetrievalMode.NonDeleted, false, false).ToList<WorkItemFieldData>())
          workItemLinkTypeMap.Add(workItemFieldData.Id, workItemFieldData.WorkItemType);
      }
    }

    private static bool ShouldLoadTags(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WorkItemTracking.Server.AllowTagsInAlerts") && requestContext.IsFeatureEnabled("WorkItemTracking.Server.FireWorkItemsChangedWithAllCustomFields");

    public virtual IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      int permissionsToCheck,
      bool includeCustomFields = true,
      bool includeTextFields = true,
      bool includeTags = true,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false,
      bool includeCountFields = true)
    {
      return (IEnumerable<WorkItemFieldData>) requestContext.TraceBlock<IEnumerable<WorkItemRevision>>(904100, 904119, 904118, "Services", "WorkItemService", "GetWorkItemFieldData", (Func<IEnumerable<WorkItemRevision>>) (() => this.GetWorkItemFieldValues(requestContext, workItemIds, requestContext.GetIdentityDisplayType(), permissionsToCheck, includeCustomFields, includeTextFields, includeTags, includeCountFields, useWorkItemIdentity: useWorkItemIdentity)));
    }

    public IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<int> fields,
      int permissionsToCheck = 16,
      DateTime? asOf = null,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false)
    {
      return this.GetWorkItemFieldValues(requestContext, workItemIds, fields, requestContext.GetIdentityDisplayType(), permissionsToCheck, asOf, batchSize, workItemRetrievalMode, suppressCustomerIntelligence, useWorkItemIdentity);
    }

    public IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> fields,
      int permissionsToCheck = 16,
      DateTime? asOf = null,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      IFieldTypeDictionary fieldDict = requestContext.WitContext().FieldDictionary;
      IEnumerable<FieldEntry> fieldEntries = fields.Select<string, FieldEntry>((Func<string, FieldEntry>) (fname => fieldDict.GetField(fname)));
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs = workItemIds.Select<int, WorkItemIdRevisionPair>((Func<int, WorkItemIdRevisionPair>) (id => new WorkItemIdRevisionPair()
      {
        Id = id
      }));
      IEnumerable<FieldEntry> fields1 = fieldEntries;
      int identityDisplayType = (int) requestContext.GetIdentityDisplayType();
      int permissionsToCheck1 = permissionsToCheck;
      DateTime? asOf1 = asOf;
      int batchSize1 = batchSize;
      int num1 = (int) workItemRetrievalMode;
      int num2 = suppressCustomerIntelligence ? 1 : 0;
      bool flag = useWorkItemIdentity;
      Guid? projectId = new Guid?();
      int num3 = flag ? 1 : 0;
      return this.GetWorkItemFieldValuesInternal(requestContext1, workItemIdRevPairs, fields1, (IdentityDisplayType) identityDisplayType, permissionsToCheck1, false, asOf1, batchSize1, (WorkItemRetrievalMode) num1, num2 != 0, projectId: projectId, useWorkItemIdentity: num3 != 0);
    }

    public IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> fields,
      Guid? projectId,
      int permissionsToCheck = 16,
      DateTime? asOf = null,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      IFieldTypeDictionary fieldDict = requestContext.WitContext().FieldDictionary;
      IEnumerable<FieldEntry> fields1 = fields.Select<string, FieldEntry>((Func<string, FieldEntry>) (fname => fieldDict.GetField(fname)));
      return this.GetWorkItemFieldValuesInternal(requestContext, workItemIds.Select<int, WorkItemIdRevisionPair>((Func<int, WorkItemIdRevisionPair>) (id => new WorkItemIdRevisionPair()
      {
        Id = id
      })), fields1, requestContext.GetIdentityDisplayType(), permissionsToCheck, false, asOf, batchSize, workItemRetrievalMode, suppressCustomerIntelligence, projectId: projectId, useWorkItemIdentity: useWorkItemIdentity);
    }

    public IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<FieldEntry> fields,
      int permissionsToCheck = 16,
      DateTime? asOf = null,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs = workItemIds.Select<int, WorkItemIdRevisionPair>((Func<int, WorkItemIdRevisionPair>) (id => new WorkItemIdRevisionPair()
      {
        Id = id
      }));
      IEnumerable<FieldEntry> fields1 = fields;
      int identityDisplayType = (int) requestContext.GetIdentityDisplayType();
      int permissionsToCheck1 = permissionsToCheck;
      DateTime? asOf1 = asOf;
      int batchSize1 = batchSize;
      int num1 = (int) workItemRetrievalMode;
      int num2 = suppressCustomerIntelligence ? 1 : 0;
      bool flag = useWorkItemIdentity;
      Guid? projectId = new Guid?();
      int num3 = flag ? 1 : 0;
      return this.GetWorkItemFieldValuesInternal(requestContext1, workItemIdRevPairs, fields1, (IdentityDisplayType) identityDisplayType, permissionsToCheck1, false, asOf1, batchSize1, (WorkItemRetrievalMode) num1, num2 != 0, projectId: projectId, useWorkItemIdentity: num3 != 0);
    }

    public IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<int> fields,
      int permissionsToCheck = 16,
      int batchSize = 200,
      bool suppressCustomerIntelligence = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemIdRevisionPair>>(workItemIdRevPairs, nameof (workItemIdRevPairs));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      IFieldTypeDictionary fieldDict = requestContext.WitContext().FieldDictionary;
      return this.GetWorkItemFieldValuesInternal(requestContext, workItemIdRevPairs.Where<WorkItemIdRevisionPair>((Func<WorkItemIdRevisionPair, bool>) (p => p.Revision <= 0)), fields.Select<int, FieldEntry>((Func<int, FieldEntry>) (fieldId => fieldDict.GetField(fieldId))), requestContext.GetIdentityDisplayType(), permissionsToCheck, false, new DateTime?(), batchSize, WorkItemRetrievalMode.NonDeleted, suppressCustomerIntelligence).Concat<WorkItemFieldData>(this.GetWorkItemFieldValuesInternal(requestContext, workItemIdRevPairs.Where<WorkItemIdRevisionPair>((Func<WorkItemIdRevisionPair, bool>) (p => p.Revision > 0)), fields.Select<int, FieldEntry>((Func<int, FieldEntry>) (fieldId => fieldDict.GetField(fieldId))), requestContext.GetIdentityDisplayType(), permissionsToCheck, true, new DateTime?(), batchSize, WorkItemRetrievalMode.NonDeleted, suppressCustomerIntelligence));
    }

    public virtual IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<string> fields,
      int permissionsToCheck = 16,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool disableProjectionLevelThree = false,
      bool useWorkItemIdentity = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemIdRevisionPair>>(workItemIdRevPairs, nameof (workItemIdRevPairs));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      IFieldTypeDictionary fieldDict = requestContext.WitContext().FieldDictionary;
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs1 = workItemIdRevPairs.Where<WorkItemIdRevisionPair>((Func<WorkItemIdRevisionPair, bool>) (p => p.Revision <= 0));
      IEnumerable<FieldEntry> fields1 = fields.Select<string, FieldEntry>((Func<string, FieldEntry>) (fname => fieldDict.GetField(fname)));
      int identityDisplayType1 = (int) requestContext.GetIdentityDisplayType();
      int permissionsToCheck1 = permissionsToCheck;
      DateTime? asOf1 = new DateTime?();
      int batchSize1 = batchSize;
      int num1 = (int) workItemRetrievalMode;
      int num2 = suppressCustomerIntelligence ? 1 : 0;
      int num3 = disableProjectionLevelThree ? 1 : 0;
      bool flag1 = useWorkItemIdentity;
      Guid? projectId1 = new Guid?();
      int num4 = flag1 ? 1 : 0;
      IEnumerable<WorkItemFieldData> fieldValuesInternal1 = this.GetWorkItemFieldValuesInternal(requestContext1, workItemIdRevPairs1, fields1, (IdentityDisplayType) identityDisplayType1, permissionsToCheck1, false, asOf1, batchSize1, (WorkItemRetrievalMode) num1, num2 != 0, num3 != 0, projectId1, num4 != 0);
      IVssRequestContext requestContext2 = requestContext;
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs2 = workItemIdRevPairs.Where<WorkItemIdRevisionPair>((Func<WorkItemIdRevisionPair, bool>) (p => p.Revision > 0));
      IEnumerable<FieldEntry> fields2 = fields.Select<string, FieldEntry>((Func<string, FieldEntry>) (fname => fieldDict.GetField(fname)));
      int identityDisplayType2 = (int) requestContext.GetIdentityDisplayType();
      int permissionsToCheck2 = permissionsToCheck;
      DateTime? asOf2 = new DateTime?();
      int batchSize2 = batchSize;
      int num5 = (int) workItemRetrievalMode;
      int num6 = suppressCustomerIntelligence ? 1 : 0;
      int num7 = disableProjectionLevelThree ? 1 : 0;
      bool flag2 = useWorkItemIdentity;
      Guid? projectId2 = new Guid?();
      int num8 = flag2 ? 1 : 0;
      IEnumerable<WorkItemFieldData> fieldValuesInternal2 = this.GetWorkItemFieldValuesInternal(requestContext2, workItemIdRevPairs2, fields2, (IdentityDisplayType) identityDisplayType2, permissionsToCheck2, true, asOf2, batchSize2, (WorkItemRetrievalMode) num5, num6 != 0, num7 != 0, projectId2, num8 != 0);
      return fieldValuesInternal1.Concat<WorkItemFieldData>(fieldValuesInternal2);
    }

    public IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<FieldEntry> fields,
      int permissionsToCheck = 16,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false,
      bool disableProjectionLevelThree = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemIdRevisionPair>>(workItemIdRevPairs, nameof (workItemIdRevPairs));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs1 = workItemIdRevPairs.Where<WorkItemIdRevisionPair>((Func<WorkItemIdRevisionPair, bool>) (p => p.Revision <= 0));
      IEnumerable<FieldEntry> fields1 = fields;
      int identityDisplayType1 = (int) requestContext.GetIdentityDisplayType();
      int permissionsToCheck1 = permissionsToCheck;
      DateTime? asOf1 = new DateTime?();
      int batchSize1 = batchSize;
      int num1 = (int) workItemRetrievalMode;
      int num2 = suppressCustomerIntelligence ? 1 : 0;
      bool flag1 = useWorkItemIdentity;
      int num3 = disableProjectionLevelThree ? 1 : 0;
      Guid? projectId1 = new Guid?();
      int num4 = flag1 ? 1 : 0;
      IEnumerable<WorkItemFieldData> fieldValuesInternal1 = this.GetWorkItemFieldValuesInternal(requestContext1, workItemIdRevPairs1, fields1, (IdentityDisplayType) identityDisplayType1, permissionsToCheck1, false, asOf1, batchSize1, (WorkItemRetrievalMode) num1, num2 != 0, num3 != 0, projectId1, num4 != 0);
      IVssRequestContext requestContext2 = requestContext;
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs2 = workItemIdRevPairs.Where<WorkItemIdRevisionPair>((Func<WorkItemIdRevisionPair, bool>) (p => p.Revision > 0));
      IEnumerable<FieldEntry> fields2 = fields;
      int identityDisplayType2 = (int) requestContext.GetIdentityDisplayType();
      int permissionsToCheck2 = permissionsToCheck;
      DateTime? asOf2 = new DateTime?();
      int batchSize2 = batchSize;
      int num5 = (int) workItemRetrievalMode;
      int num6 = suppressCustomerIntelligence ? 1 : 0;
      bool flag2 = useWorkItemIdentity;
      int num7 = disableProjectionLevelThree ? 1 : 0;
      Guid? projectId2 = new Guid?();
      int num8 = flag2 ? 1 : 0;
      IEnumerable<WorkItemFieldData> fieldValuesInternal2 = this.GetWorkItemFieldValuesInternal(requestContext2, workItemIdRevPairs2, fields2, (IdentityDisplayType) identityDisplayType2, permissionsToCheck2, true, asOf2, batchSize2, (WorkItemRetrievalMode) num5, num6 != 0, num7 != 0, projectId2, num8 != 0);
      return fieldValuesInternal1.Concat<WorkItemFieldData>(fieldValuesInternal2);
    }

    public virtual IEnumerable<string> GetAllowedValues(
      IVssRequestContext requestContext,
      int fieldId)
    {
      return this.GetAllowedValues(requestContext, fieldId, (string) null);
    }

    public virtual IEnumerable<string> GetAllowedValues(
      IVssRequestContext requestContext,
      int fieldId,
      string project)
    {
      return this.GetAllowedValues(requestContext, fieldId, project, (IEnumerable<string>) null, false);
    }

    public virtual IEnumerable<string> GetAllowedValues(
      IVssRequestContext requestContext,
      int fieldId,
      string projectName,
      IEnumerable<string> workItemTypeNames,
      bool excludeIdentities = false)
    {
      requestContext.TraceEnter(900103, "Services", "MetadataService", nameof (GetAllowedValues));
      requestContext.Trace(900152, TraceLevel.Verbose, "Services", "MetadataService", "FieldId: {0}", (object) fieldId);
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!CommonWITUtils.HasReadRulesPermission(requestContext))
        throw new ReadAllowedValuesNotAuthorizedException();
      bool flag = false;
      IReadOnlyCollection<string> allowedValues1 = (IReadOnlyCollection<string>) new List<string>();
      int? projectId1 = new int?();
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
      {
        IWorkItemTrackingProcessService service1 = requestContext.GetService<IWorkItemTrackingProcessService>();
        Guid processId = Guid.Empty;
        if (!string.IsNullOrEmpty(projectName))
        {
          IProjectService service2 = requestContext.GetService<IProjectService>();
          Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project = service2.GetProject(requestContext, projectName);
          Guid projectId2 = service2.GetProjectId(requestContext, projectName);
          projectId1 = new int?(requestContext.WitContext().TreeService.GetTreeNode(projectId2, projectId2).Id);
          ProcessDescriptor processDescriptor;
          if (requestContext.GetService<IWorkItemTrackingProcessService>().TryGetProjectProcessDescriptor(requestContext, project.Id, out processDescriptor))
          {
            processId = processDescriptor.TypeId;
            flag = !processDescriptor.IsCustom;
          }
          if (flag && service1.TryGetAllowedValues(requestContext, processId, fieldId, workItemTypeNames, out allowedValues1))
            return (IEnumerable<string>) allowedValues1;
        }
        else
          service1.TryGetAllowedValues(requestContext, processId, fieldId, workItemTypeNames, out allowedValues1);
      }
      IEnumerable<string> strings = (IEnumerable<string>) new List<string>();
      if (!flag)
      {
        if (!string.IsNullOrEmpty(projectName))
        {
          Guid projectId3 = requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName);
          projectId1 = new int?(requestContext.WitContext().TreeService.GetTreeNode(projectId3, projectId3).Id);
        }
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        {
          IDictionary<int, IEnumerable<string>> allowedValues2 = component.GetAllowedValues((IEnumerable<int>) new int[1]
          {
            fieldId
          }, projectId1, projectName, workItemTypeNames, excludeIdentities: (excludeIdentities ? 1 : 0) != 0);
          strings = allowedValues2 != null ? allowedValues2.FirstOrDefault<KeyValuePair<int, IEnumerable<string>>>().Value : (IEnumerable<string>) null;
        }
      }
      requestContext.TraceLeave(900104, "Services", "MetadataService", nameof (GetAllowedValues));
      return (strings ?? Enumerable.Empty<string>()).Union<string>((IEnumerable<string>) allowedValues1 ?? Enumerable.Empty<string>());
    }

    private IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<int> fields,
      IdentityDisplayType identityDisplayType,
      int permissionsToCheck,
      DateTime? asOf = null,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false,
      bool checkForSoftDeletedProjects = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) fields, nameof (fields));
      IFieldTypeDictionary fieldDict = requestContext.WitContext().FieldDictionary;
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs = workItemIds.Select<int, WorkItemIdRevisionPair>((Func<int, WorkItemIdRevisionPair>) (id => new WorkItemIdRevisionPair()
      {
        Id = id
      }));
      IEnumerable<FieldEntry> fields1 = fields.Select<int, FieldEntry>((Func<int, FieldEntry>) (fieldId => fieldDict.GetField(fieldId)));
      int num1 = (int) identityDisplayType;
      int permissionsToCheck1 = permissionsToCheck;
      DateTime? asOf1 = asOf;
      int num2 = (int) workItemRetrievalMode;
      int num3 = suppressCustomerIntelligence ? 1 : 0;
      bool flag1 = useWorkItemIdentity;
      bool flag2 = checkForSoftDeletedProjects;
      Guid? projectId = new Guid?();
      int num4 = flag1 ? 1 : 0;
      int num5 = flag2 ? 1 : 0;
      return this.GetWorkItemFieldValuesInternal(requestContext1, workItemIdRevPairs, fields1, (IdentityDisplayType) num1, permissionsToCheck1, false, asOf1, 200, (WorkItemRetrievalMode) num2, num3 != 0, projectId: projectId, useWorkItemIdentity: num4 != 0, checkForSoftDeletedProjects: num5 != 0);
    }

    private IEnumerable<WorkItemFieldData> GetWorkItemFieldValuesBatch(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<FieldEntry> fields,
      int permissionsToCheck,
      bool byRevision,
      DateTime? asOf,
      IdentityDisplayType identityDisplayType,
      WorkItemRetrievalMode workItemRetrievalMode,
      bool suppressCustomerIntelligence,
      bool disableProjectionLevelThree = false,
      Guid? projectId = null,
      bool useWorkItemIdentity = false,
      bool checkForSoftDeletedProjects = false)
    {
      this.CheckForNegativeWorkItemId(workItemIdRevPairs.Select<WorkItemIdRevisionPair, int>((Func<WorkItemIdRevisionPair, int>) (revPair => revPair.Id)));
      int wideTableProjectionLevel = -1;
      HashSet<int> wideFields = (HashSet<int>) null;
      HashSet<int> longFields = (HashSet<int>) null;
      HashSet<int> textFields = (HashSet<int>) null;
      int fetchWorkItemsCount = 0;
      int returnWorkItemsCount = 0;
      PerformanceScenarioHelper perfHelper = new PerformanceScenarioHelper(requestContext, nameof (TeamFoundationWorkItemService), nameof (GetWorkItemFieldValuesBatch));
      IEnumerable<WorkItemFieldData> fieldValuesBatch1 = requestContext.TraceBlock<IEnumerable<WorkItemFieldData>>(904120, 904139, 904138, "Services", "WorkItemService", nameof (GetWorkItemFieldValuesBatch), (Func<IEnumerable<WorkItemFieldData>>) (() =>
      {
        if (asOf.HasValue && !CommonWITUtils.HasReadHistoricalWorkItemResourcesPermission(requestContext))
          throw new WorkItemUnauthorizedHistoricalDataAccessException();
        if (!workItemIdRevPairs.Any<WorkItemIdRevisionPair>())
          return Enumerable.Empty<WorkItemFieldData>();
        wideFields = new HashSet<int>() { -3, -2 };
        longFields = new HashSet<int>();
        textFields = new HashSet<int>();
        bool flag1 = false;
        foreach (FieldEntry field in fields)
        {
          switch (field.FieldId)
          {
            case -105:
            case -104:
            case -56:
            case -55:
            case -54:
            case -53:
            case -52:
            case -51:
            case -50:
              wideFields.Add(-104);
              continue;
            case -49:
            case -48:
            case -47:
            case -46:
            case -45:
            case -44:
            case -43:
            case -42:
            case -12:
            case -7:
            case -2:
              wideFields.Add(-2);
              continue;
            case -15:
            case -14:
              continue;
            case 80:
              flag1 = true;
              continue;
            default:
              FieldStorageTarget storageTarget = field.StorageTarget;
              if ((storageTarget & FieldStorageTarget.WideTable) != FieldStorageTarget.Unknown)
              {
                wideFields.Add(field.FieldId);
                continue;
              }
              if ((storageTarget & FieldStorageTarget.LongTable) != FieldStorageTarget.Unknown)
              {
                longFields.Add(field.FieldId);
                continue;
              }
              if ((storageTarget & FieldStorageTarget.LongTexts) != FieldStorageTarget.Unknown)
              {
                textFields.Add(field.FieldId);
                continue;
              }
              longFields.Add(field.FieldId);
              continue;
          }
        }
        disableProjectionLevelThree |= WorkItemTrackingFeatureFlags.IsProjectionLevelThreeDisabledForAll(requestContext);
        int maxLongTextSize = requestContext.WitContext().ServerSettings.MaxLongTextSize;
        int useTableVarThreshold = 0;
        ICollection<WorkItemFieldValues> records;
        using (perfHelper.Measure("GetWorkItemFieldValues_DB"))
        {
          using (WorkItemComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
            records = (ICollection<WorkItemFieldValues>) replicaAwareComponent.GetWorkItemFieldValues(workItemIdRevPairs, (IEnumerable<int>) wideFields, (IEnumerable<int>) longFields, (IEnumerable<int>) textFields, byRevision, asOf, maxLongTextSize, identityDisplayType, disableProjectionLevelThree, out wideTableProjectionLevel, useTableVarThreshold).ToArray<WorkItemFieldValues>();
        }
        fetchWorkItemsCount = records.Count;
        if (workItemRetrievalMode != WorkItemRetrievalMode.All)
        {
          bool isDeleted = workItemRetrievalMode == WorkItemRetrievalMode.Deleted;
          if (byRevision || asOf.HasValue)
          {
            records = (ICollection<WorkItemFieldValues>) records.Where<WorkItemFieldValues>((Func<WorkItemFieldValues, bool>) (fv => fv.LatestIsDeleted == isDeleted || fv.Fields.GetValueOrDefault<bool>(-404) == isDeleted)).ToList<WorkItemFieldValues>();
          }
          else
          {
            List<int> filteredWorkItemIds = records.GroupBy<WorkItemFieldValues, int>((Func<WorkItemFieldValues, int>) (fieldValueRecord => fieldValueRecord.Id)).Select<IGrouping<int, WorkItemFieldValues>, WorkItemFieldValues>((Func<IGrouping<int, WorkItemFieldValues>, WorkItemFieldValues>) (fieldValueGroup => fieldValueGroup.OrderByDescending<WorkItemFieldValues, int>((Func<WorkItemFieldValues, int>) (record => record.Rev)).First<WorkItemFieldValues>())).Where<WorkItemFieldValues>((Func<WorkItemFieldValues, bool>) (revision => revision.Fields.GetValueOrDefault<bool>(-404) == isDeleted)).Select<WorkItemFieldValues, int>((Func<WorkItemFieldValues, int>) (revision => revision.Id)).ToList<int>();
            records = (ICollection<WorkItemFieldValues>) records.Where<WorkItemFieldValues>((Func<WorkItemFieldValues, bool>) (workItemFieldValue => filteredWorkItemIds.Any<int>((Func<int, bool>) (y => y == workItemFieldValue.Id)))).ToList<WorkItemFieldValues>();
          }
        }
        CommonWITUtils.Assert(requestContext, 909700, "Services", "WorkItemService", (Func<bool>) (() => workItemIdRevPairs.Count<WorkItemIdRevisionPair>() >= records.Count), (Func<string>) (() => "Unexpected record count"));
        List<SecuredWorkItemFieldValues> filteredRecords = new List<SecuredWorkItemFieldValues>();
        IPermissionCheckHelper permChecker = requestContext.WitContext().WorkItemPermissionChecker;
        using (perfHelper.Measure("PermissionChecks"))
        {
          IEnumerable<Guid> source = (IEnumerable<Guid>) null;
          foreach (WorkItemFieldValues workItemFieldValues in (IEnumerable<WorkItemFieldValues>) records)
          {
            WorkItemFieldValues record = workItemFieldValues;
            string token = (string) null;
            if (permissionsToCheck == 0)
            {
              filteredRecords.Add(new SecuredWorkItemFieldValues()
              {
                WorkItemFieldValues = record
              });
            }
            else
            {
              bool flag2 = permChecker.HasWorkItemPermission(record.LatestAreaId, permissionsToCheck);
              if (!WorkItemTrackingFeatureFlags.IsDanglingLinkDeletionDisabled(requestContext) && !flag2 & checkForSoftDeletedProjects)
              {
                if (source == null)
                  source = requestContext.GetService<PlatformProjectService>().GetSoftDeletedProjects(requestContext).Select<Microsoft.TeamFoundation.Core.WebApi.ProjectInfo, Guid>((Func<Microsoft.TeamFoundation.Core.WebApi.ProjectInfo, Guid>) (proj => proj.Id));
                if (source.Any<Guid>((Func<Guid, bool>) (id => id == record.ProjectId)))
                  flag2 = true;
              }
              if (flag2 && this.TryGetSecurityToken(requestContext, projectId, record.LatestAreaId, out token))
                filteredRecords.Add(new SecuredWorkItemFieldValues()
                {
                  WorkItemFieldValues = record,
                  SecurityToken = token
                });
            }
          }
        }
        using (perfHelper.Measure("FixTestValuesAndTags"))
        {
          if (textFields.Any<int>())
          {
            foreach (SecuredWorkItemFieldValues workItemFieldValues in filteredRecords)
              this.FixTextFieldValues(requestContext, workItemFieldValues.WorkItemFieldValues);
          }
          if (flag1)
          {
            if (filteredRecords.Any<SecuredWorkItemFieldValues>())
              requestContext.TraceBlock(904125, 904129, 904128, "Services", "WorkItemService", "GetWorkItemFieldValuesBatch.PopulateTags", (Action) (() =>
              {
                ITeamFoundationTaggingService service = requestContext.GetService<ITeamFoundationTaggingService>();
                ITreeDictionary treeService = requestContext.WitContext().TreeService;
                VersionedTagArtifact<int>[] array3 = filteredRecords.Select<SecuredWorkItemFieldValues, VersionedTagArtifact<int>>((Func<SecuredWorkItemFieldValues, VersionedTagArtifact<int>>) (record => new VersionedTagArtifact<int>(treeService.LegacyGetTreeNode(record.WorkItemFieldValues.LatestAreaId).ProjectId, record.WorkItemFieldValues.Id, record.WorkItemFieldValues.Rev))).ToArray<VersionedTagArtifact<int>>();
                IVssRequestContext requestContext2 = requestContext.Elevate();
                Guid workItem = WorkItemArtifactKinds.WorkItem;
                VersionedTagArtifact<int>[] artifacts = array3;
                ILookup<int, ArtifactTags<int>> lookup = service.GetTagsForVersionedArtifacts<int>(requestContext2, workItem, (ICollection<VersionedTagArtifact<int>>) artifacts).ToLookup<ArtifactTags<int>, int>((Func<ArtifactTags<int>, int>) (artifact => artifact.Artifact.Id));
                foreach (SecuredWorkItemFieldValues workItemFieldValues in filteredRecords)
                {
                  SecuredWorkItemFieldValues record = workItemFieldValues;
                  TagDefinition[] array4 = lookup[record.WorkItemFieldValues.Id].Where<ArtifactTags<int>>((Func<ArtifactTags<int>, bool>) (a => a.Artifact.Version == record.WorkItemFieldValues.Rev)).SelectMany<ArtifactTags<int>, TagDefinition>((Func<ArtifactTags<int>, IEnumerable<TagDefinition>>) (a => a.Tags)).ToArray<TagDefinition>();
                  if (array4.Length != 0)
                  {
                    record.WorkItemFieldValues.Fields[80] = (object) TeamFoundationWorkItemService.TagDefinitionsToString((IEnumerable<TagDefinition>) array4);
                    record.WorkItemFieldValues.TagDefinitions = (IEnumerable<TagDefinition>) array4;
                  }
                }
              }));
          }
        }
        using (perfHelper.Measure("Email"))
        {
          if (useWorkItemIdentity)
          {
            HashSet<WorkItemIdentity> processedWorkItemIdentities = (HashSet<WorkItemIdentity>) null;
            IDictionary<Guid, WorkItemIdentity> identityMap = WorkItemIdentityHelper.EnsureVsidToWorkItemIdentityMap(requestContext, (IEnumerable<Guid>) filteredRecords.SelectMany<SecuredWorkItemFieldValues, Guid>((Func<SecuredWorkItemFieldValues, IEnumerable<Guid>>) (dataset => (IEnumerable<Guid>) dataset.WorkItemFieldValues.IdentityFields.Values)).Distinct<Guid>().ToList<Guid>());
            filteredRecords.ForEach((Action<SecuredWorkItemFieldValues>) (dataset =>
            {
              int latestAreaId = dataset.WorkItemFieldValues.LatestAreaId;
              bool emailReadable = permChecker.HasWorkItemPermission(latestAreaId, 256);
              processedWorkItemIdentities = this.ReplaceWithWorkItemIdentity(requestContext, projectId, dataset.WorkItemFieldValues, identityMap, latestAreaId, emailReadable);
            }));
            WorkItemIdentityHelper.AddIdentitiesToDistinctDisplayNameMap(requestContext, (IEnumerable<WorkItemIdentity>) processedWorkItemIdentities);
          }
          else
            filteredRecords.ForEach((Action<SecuredWorkItemFieldValues>) (dataset =>
            {
              if (permChecker.HasWorkItemPermission(dataset.WorkItemFieldValues.LatestAreaId, 256))
                return;
              CustomerIntelligenceData properties = new CustomerIntelligenceData();
              properties.Add("Error", Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.IdentityEmailNotViewable());
              requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, nameof (GetWorkItemFieldValuesBatch), properties);
            }));
        }
        IEnumerable<WorkItemFieldData> fieldValuesBatch3 = filteredRecords.Select<SecuredWorkItemFieldValues, WorkItemFieldData>((Func<SecuredWorkItemFieldValues, WorkItemFieldData>) (fv =>
        {
          WorkItemFieldData fieldValuesBatch4 = new WorkItemFieldData((IDictionary<int, object>) fv.WorkItemFieldValues.Fields, (IDictionary<int, Guid>) fv.WorkItemFieldValues.IdentityFields, fv.WorkItemFieldValues.ProjectId, fv.WorkItemFieldValues.TagDefinitions, permissionsToCheck, fv.WorkItemFieldValues.WorkItemCommentVersion);
          fieldValuesBatch4.SetSecuredToken(fv.SecurityToken);
          return fieldValuesBatch4;
        }));
        returnWorkItemsCount = filteredRecords.Count;
        return fieldValuesBatch3;
      }));
      if (!suppressCustomerIntelligence)
      {
        WorkItemKpiTracer.TraceKpi(requestContext, (WorkItemTrackingKpi) new PageWorkItemKpi(requestContext, workItemIdRevPairs.Count<WorkItemIdRevisionPair>()), (WorkItemTrackingKpi) new CountKpi(requestContext, "FieldsPaged", fields.Count<FieldEntry>()));
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, WorkItemPageTelemetry.Feature, (object) new WorkItemPageTelemetryParams(fields, workItemIdRevPairs, wideTableProjectionLevel, (IEnumerable<int>) wideFields, (IEnumerable<int>) longFields, (IEnumerable<int>) textFields, byRevision, asOf, requestContext.WitContext().ServerSettings.MaxLongTextSize, identityDisplayType, workItemRetrievalMode, fetchWorkItemsCount, returnWorkItemsCount));
      }
      perfHelper.EndScenario();
      return fieldValuesBatch1;
    }

    private IEnumerable<WorkItemFieldData> GetWorkItemFieldValuesInternal(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<FieldEntry> fields,
      IdentityDisplayType identityDisplayType,
      int permissionsToCheck,
      bool byRevision,
      DateTime? asOf,
      int batchSize,
      WorkItemRetrievalMode workItemRetrievalMode,
      bool suppressCustomerIntelligence,
      bool disableProjectionLevelThree = false,
      Guid? projectId = null,
      bool useWorkItemIdentity = false,
      bool checkForSoftDeletedProjects = false)
    {
      return CommonWITUtils.BatchResponse<WorkItemIdRevisionPair, WorkItemFieldData>((Func<IEnumerable<WorkItemIdRevisionPair>, IEnumerable<WorkItemFieldData>>) (idRevPairs => this.GetWorkItemFieldValuesBatch(requestContext, idRevPairs, fields, permissionsToCheck, byRevision, asOf, identityDisplayType, workItemRetrievalMode, suppressCustomerIntelligence, disableProjectionLevelThree, projectId, useWorkItemIdentity, checkForSoftDeletedProjects)), workItemIdRevPairs.Distinct<WorkItemIdRevisionPair>(), batchSize);
    }

    private IEnumerable<WorkItemRevision> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IdentityDisplayType identityDisplayType,
      int permissionsToCheck,
      bool includeCustomFields = true,
      bool includeTextFields = true,
      bool includeTags = true,
      bool includeCountFields = true,
      bool includeResourceLinks = false,
      bool useWorkItemIdentity = false,
      bool includeWorkItemLinks = false)
    {
      if (!workItemIds.Any<int>())
        return Enumerable.Empty<WorkItemRevision>();
      this.CheckForNegativeWorkItemId(workItemIds);
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<int> workItemIds1 = workItemIds;
      int num1 = includeCountFields ? 1 : 0;
      int num2 = includeCustomFields ? 1 : 0;
      int num3 = includeTextFields ? 1 : 0;
      int num4 = includeResourceLinks ? 1 : 0;
      int num5 = includeWorkItemLinks ? 1 : 0;
      int num6 = includeTags ? 1 : 0;
      int num7 = (int) identityDisplayType;
      bool flag = useWorkItemIdentity;
      DateTime? asOf = new DateTime?();
      DateTime? revisionsSince = new DateTime?();
      Guid? projectId = new Guid?();
      int num8 = flag ? 1 : 0;
      IEnumerable<WorkItemDataset> datasets = this.GetWorkItemDataSets(requestContext1, workItemIds1, num1 != 0, num2 != 0, num3 != 0, num4 != 0, num5 != 0, false, num6 != 0, (IdentityDisplayType) num7, WorkItemRetrievalMode.NonDeleted, asOf, revisionsSince, projectId, num8 != 0);
      if (permissionsToCheck != 0)
        requestContext.TraceBlock(904110, 904114, 904113, "Services", "WorkItemService", "GetWorkItemFieldData.PermissionCheck", (Action) (() =>
        {
          IPermissionCheckHelper permChecker = requestContext.WitContext().WorkItemPermissionChecker;
          datasets = (IEnumerable<WorkItemDataset>) datasets.Where<WorkItemDataset>((Func<WorkItemDataset, bool>) (dataset => permChecker.HasWorkItemPermission(dataset.AreaId, permissionsToCheck))).ToArray<WorkItemDataset>();
        }));
      return (IEnumerable<WorkItemRevision>) datasets.Select<WorkItemDataset, WorkItemRevision>((Func<WorkItemDataset, WorkItemRevision>) (ds => new WorkItemRevision((WorkItemRevisionDataset) ds))).ToArray<WorkItemRevision>();
    }

    private void CheckForNegativeWorkItemId(IEnumerable<int> workItemIds)
    {
      foreach (float workItemId in workItemIds)
        ArgumentUtility.CheckGreaterThanOrEqualToZero(workItemId, "workItemId");
    }

    public virtual IEnumerable<ArtifactUriQueryResult> GetWorkItemIdsForArtifactUris(
      IVssRequestContext requestContext,
      IEnumerable<string> artifactUris,
      DateTime? asOfDate = null,
      Guid? filterUnderProjectId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(artifactUris, nameof (artifactUris));
      if (artifactUris.Any<string>((Func<string, bool>) (uri => uri == null)))
        throw new ArgumentNullException("artifactUri", "Null artifact uri");
      if (!CommonWITUtils.HasCrossProjectQueryArtifactUriPermission(requestContext))
      {
        ArgumentUtility.CheckForNull<Guid>(filterUnderProjectId, nameof (filterUnderProjectId));
        ArgumentUtility.CheckForEmptyGuid(filterUnderProjectId.Value, nameof (filterUnderProjectId));
      }
      if (asOfDate.HasValue)
        asOfDate = new DateTime?(TimeZoneInfo.ConvertTimeToUtc(asOfDate.Value));
      if (!artifactUris.Any<string>())
        return Enumerable.Empty<ArtifactUriQueryResult>();
      IDictionary<string, string> rawToTransformedUriLookup = (IDictionary<string, string>) null;
      artifactUris = requestContext.GetService<IExternalConnectionService>().TransformArtifactUriToStorageFormat(requestContext, artifactUris, out rawToTransformedUriLookup);
      IReadOnlyCollection<UriQueryResultEntry> list;
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        list = (IReadOnlyCollection<UriQueryResultEntry>) component.GetWorkItemIdsForArtifactUris((IReadOnlyCollection<string>) artifactUris.ToList<string>(), asOfDate).ToList<UriQueryResultEntry>();
      this.GetPermissionCheckHelper(requestContext);
      return (IEnumerable<ArtifactUriQueryResult>) list.Select<UriQueryResultEntry, ArtifactUriQueryResult>((Func<UriQueryResultEntry, ArtifactUriQueryResult>) (qre =>
      {
        Dictionary<int, string> workItemToTokensLookup = new Dictionary<int, string>();
        string str = qre.Uri;
        IEnumerable<int> workItemIds = qre.WorkItems.Where<WorkItemQueryResultEntry>((Func<WorkItemQueryResultEntry, bool>) (wie =>
        {
          string token;
          if (!this.TryGetSecurityToken(requestContext, filterUnderProjectId, wie.AreaId, out token) || !this.GetPermissionCheckHelper(requestContext).HasWorkItemPermission(wie.AreaId, 16))
            return false;
          workItemToTokensLookup[wie.Id] = token;
          return true;
        })).Select<WorkItemQueryResultEntry, int>((Func<WorkItemQueryResultEntry, int>) (wie => wie.Id)).Distinct<int>();
        if (rawToTransformedUriLookup != null)
          str = rawToTransformedUriLookup.GetValueOrDefault<string, string>(str, str);
        return new ArtifactUriQueryResult(str, workItemIds, (IDictionary<int, string>) workItemToTokensLookup);
      })).ToList<ArtifactUriQueryResult>();
    }

    public virtual IEnumerable<DestroyedWorkItemQueryResult> GetDestroyedWorkItemIds(
      IVssRequestContext requestContext,
      long rowVersion)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      int syncApiBatchSize = requestContext.WitContext().ServerSettings.WorkItemSyncApiBatchSize;
      IReadOnlyCollection<DestroyedWorkItemQueryResultEntry> list;
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        list = (IReadOnlyCollection<DestroyedWorkItemQueryResultEntry>) component.GetDestroyedWorkItemIds(rowVersion, syncApiBatchSize).ToList<DestroyedWorkItemQueryResultEntry>();
      return (IEnumerable<DestroyedWorkItemQueryResult>) list.Select<DestroyedWorkItemQueryResultEntry, DestroyedWorkItemQueryResult>((Func<DestroyedWorkItemQueryResultEntry, DestroyedWorkItemQueryResult>) (qre => new DestroyedWorkItemQueryResult()
      {
        WorkItemId = qre.WorkItemId,
        RowVersion = qre.RowVersion
      })).ToList<DestroyedWorkItemQueryResult>();
    }

    private void ValidateWorkItemsMovable(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      IEnumerable<WorkItemUpdateState> source = updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (updateState =>
      {
        if (!updateState.Success || updateState.FieldData == null)
          return false;
        return updateState.HasTeamProjectChanged || updateState.HasWorkItemTypeChanged;
      }));
      if (source.Count<WorkItemUpdateState>() <= 0)
        return;
      Dictionary<int, WorkItemUpdateState> dictionary = source.ToDictionary<WorkItemUpdateState, int>((Func<WorkItemUpdateState, int>) (updateState => updateState.Id));
      List<WorkItem> list = this.GetWorkItems(witRequestContext.RequestContext, (IEnumerable<int>) dictionary.Keys, true, true, false, false, WorkItemRetrievalMode.NonDeleted, WorkItemErrorPolicy.Fail, false, false, false, new DateTime?()).ToList<WorkItem>();
      Dictionary<Guid, HashSet<string>> projectToNonRemovableTypesMap = (Dictionary<Guid, HashSet<string>>) null;
      Dictionary<int, string> workItemLinkTypeMap = (Dictionary<int, string>) null;
      this.PrepareWorkItemTypeCheck(witRequestContext.RequestContext, (IEnumerable<WorkItem>) list, out projectToNonRemovableTypesMap, out workItemLinkTypeMap, false, dictionary);
      foreach (WorkItem workItem in list)
      {
        WorkItemUpdateState udpateState = dictionary[workItem.Id];
        WorkItemTrackingServiceException notMovableException;
        if (!this.TryCheckWorkItemIsMovable(witRequestContext.RequestContext, workItem, projectToNonRemovableTypesMap, workItemLinkTypeMap, out notMovableException))
          udpateState.UpdateResult.AddException((TeamFoundationServiceException) notMovableException);
        else if (udpateState.HasWorkItemTypeChanged)
        {
          string targetWorkItemTypeName = string.Empty;
          if (!this.TryCheckTargetWorkItemTypeValid(witRequestContext, udpateState, projectToNonRemovableTypesMap[workItem.GetProjectGuid(witRequestContext.RequestContext)], out targetWorkItemTypeName))
            udpateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTargetTypeNotSupportedException(targetWorkItemTypeName));
        }
      }
    }

    private void ValidateMoveTeamProjectPermissions(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      witRequestContext.RequestContext.TraceBlock(904860, 904869, 904868, "Services", "WorkItemService", "UpdateWorkItems.ValidateMoveTeamProjectPermissions", (Action) (() =>
      {
        foreach (WorkItemUpdateState updateState in updateStates)
        {
          if (updateState.HasTeamProjectChanged && !this.HasWorkItemMovePermission(witRequestContext, updateState))
            updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemUnauthorizedAccessException(updateState.Id, AccessType.Move));
        }
      }));
    }

    private bool HasWorkItemMovePermission(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState)
    {
      int fieldValue = updateState.FieldData.GetFieldValue<int>(witRequestContext, -2, true);
      return witRequestContext.WorkItemProjectPermissionChecker.HasWorkItemPermission(fieldValue, AuthorizationProjectPermissions.WorkItemMove);
    }

    private void ValidateTeamProjectChanges(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      witRequestContext.RequestContext.TraceBlock(904900, 904901, "Services", "WorkItemService", "UpdateWorkItems.ValidateTeamProjectChanges", (Action) (() =>
      {
        int num = WorkItemTrackingFeatureFlags.IsTeamProjectMoveEnabled(witRequestContext.RequestContext) ? 1 : 0;
        if (num != 0)
          this.ValidateMoveTeamProjectPermissions(witRequestContext, updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success && this.HasFieldUpdate(witRequestContext, us, -42))));
        if (num != 0 || WorkItemTrackingFeatureFlags.IsChangeWorkItemTypeEnabled(witRequestContext.RequestContext))
          this.ValidateWorkItemsMovable(witRequestContext, updateStates);
        if (num == 0)
          return;
        foreach (WorkItemUpdateState updateState in updateStates)
        {
          if (updateState.Success && updateState.FieldData != null && updateState.HasTeamProjectChanged)
          {
            IProjectService service = witRequestContext.RequestContext.GetService<IProjectService>();
            string projectName = (string) updateState.Update.Fields.First<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (u => TFStringComparer.WorkItemFieldReferenceName.Equals(u.Key, "System.TeamProject"))).Value;
            Microsoft.TeamFoundation.Core.WebApi.ProjectInfo projectInfo = (Microsoft.TeamFoundation.Core.WebApi.ProjectInfo) null;
            try
            {
              projectInfo = service.GetProject(witRequestContext.RequestContext, projectName);
            }
            catch (ProjectDoesNotExistWithNameException ex)
            {
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTargetProjectDoesNotExistException(projectName));
            }
            catch (TeamFoundationServiceException ex)
            {
              updateState.UpdateResult.AddException(ex);
            }
            if (updateState.Success)
            {
              TreeNode treeNode1;
              if (!this.TryGetTreeNode<Guid>(witRequestContext, projectInfo.Id, out treeNode1))
                updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTargetProjectDoesNotExistException(projectName));
              if (updateState.Success)
              {
                if (updateState.HasAreaChange || updateState.Update.Fields.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (f => witRequestContext.FieldDictionary.GetField(f.Key).FieldId == -2)))
                {
                  TreeNode treeNode2;
                  if (this.TryGetTreeNode<int>(witRequestContext, (int) updateState.FieldData.GetFieldValue(witRequestContext, -2), out treeNode2) && treeNode1.Id != treeNode2.Project.Id)
                    updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemAreaPathDoesNotMatchTargetProjectException());
                }
                else
                  updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTargetAreaPathNotProvidedException());
                if (updateState.HasIterationChange || updateState.Update.Fields.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (f => witRequestContext.FieldDictionary.GetField(f.Key).FieldId == -104)))
                {
                  TreeNode treeNode3;
                  if (this.TryGetTreeNode<int>(witRequestContext, (int) updateState.FieldData.GetFieldValue(witRequestContext, -104), out treeNode3) && treeNode1.Id != treeNode3.Project.Id)
                    updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemIterationPathDoesNotMatchTargetProjectException());
                }
                else
                  updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTargetIterationPathNotProvidedException());
              }
            }
          }
        }
      }));
    }

    private bool TryCheckWorkItemIsMovable(
      IVssRequestContext requestContext,
      WorkItem item,
      Dictionary<Guid, HashSet<string>> projectToNotMovableTypesMap,
      Dictionary<int, string> workItemLinkTypeMap,
      out WorkItemTrackingServiceException notMovableException)
    {
      bool hasNonRemovableLink = false;
      notMovableException = (WorkItemTrackingServiceException) null;
      string nonRemovableReason;
      if (this.TryCheckWorkItemIsRemovable(requestContext, item, projectToNotMovableTypesMap, workItemLinkTypeMap, out hasNonRemovableLink, out nonRemovableReason, true))
        return true;
      notMovableException = !hasNonRemovableLink ? (WorkItemTrackingServiceException) new WorkItemNotMovableException(item.Id, nonRemovableReason) : (WorkItemTrackingServiceException) new WorkItemNotMovableLinkException(item.Id, nonRemovableReason);
      return false;
    }

    internal HashSet<string> GetNonMovableWorkItemTypes(
      IVssRequestContext requestContext,
      Guid projectGuid,
      bool hasTeamProjectChanged)
    {
      IWorkItemTypeCategoryService service = requestContext.GetService<IWorkItemTypeCategoryService>();
      List<string> stringList = new List<string>();
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId = projectGuid;
      IEnumerable<WorkItemTypeCategory> itemTypeCategories = service.LegacyGetWorkItemTypeCategories(requestContext1, projectId, true);
      IEnumerable<string> first = (IEnumerable<string>) new HashSet<string>();
      if (hasTeamProjectChanged)
        first = this.GetTestWorkItemTypeNames(itemTypeCategories);
      IEnumerable<string> workItemTypeNames = this.GetCodeReviewAndFeedbackWorkItemTypeNames(itemTypeCategories);
      return first.Union<string>(workItemTypeNames).ToHashSet<string>();
    }

    private bool IsAttemptedMove(IEnumerable<WorkItemUpdateState> result)
    {
      if (result.Any<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (res => res.FieldData != null && res.Success && res.HasTeamProjectChanged)))
        return true;
      foreach (WorkItemUpdateState workItemUpdateState in result.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (res => !res.Success && res.UpdateResult != null && res.Update != null && !res.Update.IsNew && res.UpdateResult.Exception != null && res.Update.Fields != null && res.Update.Fields.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (r => TFStringComparer.WorkItemFieldReferenceName.Equals(r.Key.ToString(), "System.TeamProject"))) && res.Update.Fields.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (r => TFStringComparer.WorkItemFieldReferenceName.Equals(r.Key.ToString(), "System.AreaPath"))) && res.Update.Fields.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (r => TFStringComparer.WorkItemFieldReferenceName.Equals(r.Key.ToString(), "System.IterationPath"))))))
      {
        Exception baseException = workItemUpdateState.UpdateResult.Exception.GetBaseException();
        if (baseException != null && baseException.GetType() == typeof (WorkItemTrackingAggregateException))
        {
          if (((WorkItemTrackingAggregateException) baseException).AllExceptions.Any<TeamFoundationServiceException>((Func<TeamFoundationServiceException, bool>) (innerException => innerException.GetType() == typeof (WorkItemTargetProjectDoesNotExistException) || innerException.GetType() == typeof (WorkItemTargetProjectDoesNotExistException) || innerException.GetType() == typeof (WorkItemTargetAreaPathNotProvidedException) || innerException.GetType() == typeof (WorkItemTargetIterationPathNotProvidedException) || innerException.GetType() == typeof (WorkItemAreaPathDoesNotMatchTargetProjectException) || innerException.GetType() == typeof (WorkItemIterationPathDoesNotMatchTargetProjectException) || innerException.GetType() == typeof (WorkItemNotMovableException) || innerException.GetType() == typeof (WorkItemNotMovableLinkException) || innerException.GetType() == typeof (WorkItemFieldInvalidTreeNameException) || innerException.GetType() == typeof (WorkItemFieldInvalidTreeIdException) || innerException.GetType() == typeof (WorkItemTargetTypeDoesNotExistException))))
            return true;
        }
        else if (baseException != null && baseException.GetType() == typeof (WorkItemTargetProjectDoesNotExistException) || baseException.GetType() == typeof (WorkItemTargetProjectDoesNotExistException) || baseException.GetType() == typeof (WorkItemTargetAreaPathNotProvidedException) || baseException.GetType() == typeof (WorkItemTargetIterationPathNotProvidedException) || baseException.GetType() == typeof (WorkItemAreaPathDoesNotMatchTargetProjectException) || baseException.GetType() == typeof (WorkItemIterationPathDoesNotMatchTargetProjectException) || baseException.GetType() == typeof (WorkItemNotMovableException) || baseException.GetType() == typeof (WorkItemNotMovableLinkException) || baseException.GetType() == typeof (WorkItemFieldInvalidTreeNameException) || baseException.GetType() == typeof (WorkItemFieldInvalidTreeIdException) || baseException.GetType() == typeof (WorkItemTargetTypeDoesNotExistException))
          return true;
      }
      return false;
    }

    public WorkItemUpdateResult UpdateWorkItem(
      IVssRequestContext requestContext,
      WorkItemUpdate workItemUpdate,
      bool bypassRules = false,
      bool includeInRecentActivity = false,
      bool suppressNotifications = false,
      bool isPermissionCheckRequiredForBypassRules = false,
      bool useWorkItemIdentity = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemUpdate>(workItemUpdate, nameof (workItemUpdate));
      IVssRequestContext requestContext1 = requestContext;
      WorkItemUpdate[] workItemUpdateArray = new WorkItemUpdate[1]
      {
        workItemUpdate
      };
      bool flag = includeInRecentActivity;
      int num1 = bypassRules ? 1 : 0;
      int num2 = flag ? 1 : 0;
      int num3 = suppressNotifications ? 1 : 0;
      int num4 = isPermissionCheckRequiredForBypassRules ? 1 : 0;
      int num5 = useWorkItemIdentity ? 1 : 0;
      return this.UpdateWorkItems(requestContext1, (IEnumerable<WorkItemUpdate>) workItemUpdateArray, num1 != 0, true, num2 != 0, (IReadOnlyCollection<int>) null, num3 != 0, num4 != 0, num5 != 0, false).FirstOrDefault<WorkItemUpdateResult>();
    }

    public WorkItemUpdateResult UpdateWorkItem(
      IVssRequestContext requestContext,
      WorkItemUpdate workItemUpdate,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      bool validateOnly = false,
      bool includeInRecentActivity = false,
      bool suppressNotifications = false,
      bool isPermissionCheckRequiredForBypassRules = false,
      bool useWorkItemIdentity = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemUpdate>(workItemUpdate, nameof (workItemUpdate));
      IVssRequestContext requestContext1 = requestContext;
      WorkItemUpdate[] workItemUpdateArray = new WorkItemUpdate[1]
      {
        workItemUpdate
      };
      int num1 = (int) ruleExecutionMode;
      bool flag = includeInRecentActivity;
      int num2 = validateOnly ? 1 : 0;
      int num3 = flag ? 1 : 0;
      int num4 = suppressNotifications ? 1 : 0;
      int num5 = isPermissionCheckRequiredForBypassRules ? 1 : 0;
      int num6 = useWorkItemIdentity ? 1 : 0;
      return this.UpdateWorkItems(requestContext1, (IEnumerable<WorkItemUpdate>) workItemUpdateArray, (WorkItemUpdateRuleExecutionMode) num1, true, num2 != 0, num3 != 0, (IReadOnlyCollection<int>) null, num4 != 0, num5 != 0, num6 != 0).FirstOrDefault<WorkItemUpdateResult>();
    }

    public virtual IEnumerable<WorkItemUpdateResult> UpdateWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      bool bypassRules = false,
      bool allOrNothing = false,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false,
      bool isPermissionCheckRequiredForBypassRules = false,
      bool useWorkItemIdentity = false,
      bool checkRevisionsLimit = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemUpdates, nameof (workItemUpdates));
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      IEnumerable<WorkItemUpdate> workItemUpdates1 = workItemUpdates;
      bool flag = allOrNothing;
      int num1 = bypassRules ? 0 : 1;
      int num2 = flag ? 1 : 0;
      int num3 = includeInRecentActivity ? 1 : 0;
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity1 = workItemIdsToIncludeInRecentActivity;
      int num4 = suppressNotifications ? 1 : 0;
      int num5 = isPermissionCheckRequiredForBypassRules ? 1 : 0;
      int num6 = useWorkItemIdentity ? 1 : 0;
      int num7 = checkRevisionsLimit ? 1 : 0;
      return this.UpdateWorkItems(witRequestContext, workItemUpdates1, (WorkItemUpdateRuleExecutionMode) num1, num2 != 0, includeInRecentActivity: num3 != 0, workItemIdsToIncludeInRecentActivity: workItemIdsToIncludeInRecentActivity1, suppressNotifications: num4 != 0, isPermissionCheckRequiredForBypassRules: num5 != 0, useWorkItemIdentity: num6 != 0, checkRevisionsLimit: num7 != 0);
    }

    public IEnumerable<WorkItemUpdateResult> UpdateWorkItemsStateOnCheckin(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      string commentOnSave = null,
      bool allOrNothing = false,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemIds, nameof (workItemIds));
      requestContext.WitContext();
      List<WorkItemUpdate> workItemUpdateList = new List<WorkItemUpdate>();
      return requestContext.TraceBlock<IEnumerable<WorkItemUpdateResult>>(904912, 904913, "Services", "WorkItemService", nameof (UpdateWorkItemsStateOnCheckin), (Func<IEnumerable<WorkItemUpdateResult>>) (() =>
      {
        IList<WorkItemStateOnTransition> workItemNextStates = (IList<WorkItemStateOnTransition>) null;
        this.TryGetNextStatesOnCheckin(requestContext, workItemIds, out workItemNextStates);
        return this.UpdateWorkItemsStateOnCheckin(requestContext, workItemIds, workItemNextStates, commentOnSave, allOrNothing, includeInRecentActivity, workItemIdsToIncludeInRecentActivity, suppressNotifications);
      }));
    }

    public virtual IEnumerable<WorkItemUpdateResult> UpdateWorkItemsStateOnCheckin(
      IVssRequestContext requestContext,
      IEnumerable<WorkItem> workItems,
      string commentOnSave = null,
      bool allOrNothing = false,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItems, nameof (workItems));
      requestContext.WitContext();
      List<WorkItemUpdate> workItemUpdateList = new List<WorkItemUpdate>();
      workItems = (IEnumerable<WorkItem>) workItems.Where<WorkItem>((Func<WorkItem, bool>) (wi => wi != null)).ToList<WorkItem>();
      return requestContext.TraceBlock<IEnumerable<WorkItemUpdateResult>>(904916, 904917, "Services", "WorkItemService", nameof (UpdateWorkItemsStateOnCheckin), (Func<IEnumerable<WorkItemUpdateResult>>) (() =>
      {
        IList<WorkItemStateOnTransition> workItemNextStates = (IList<WorkItemStateOnTransition>) null;
        this.TryGetNextStatesOnCheckin(requestContext, workItems, out workItemNextStates);
        return this.UpdateWorkItemsStateOnCheckin(requestContext, (IEnumerable<int>) workItems.Select<WorkItem, int>((Func<WorkItem, int>) (x => x.Id)).ToList<int>(), workItemNextStates, commentOnSave, allOrNothing, includeInRecentActivity, workItemIdsToIncludeInRecentActivity, suppressNotifications);
      }));
    }

    public IEnumerable<WorkItemUpdateResult> UpdateWorkItemsStateOnCheckin(
      IVssRequestContext requestContext,
      IDictionary<int, string> workItemIdToStateMap,
      string commentOnSave = null,
      bool allOrNothing = false,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<int, string>>(workItemIdToStateMap, nameof (workItemIdToStateMap));
      requestContext.WitContext();
      List<WorkItemUpdate> workItemUpdateList = new List<WorkItemUpdate>();
      return requestContext.TraceBlock<IEnumerable<WorkItemUpdateResult>>(904912, 904913, "Services", "WorkItemService", nameof (UpdateWorkItemsStateOnCheckin), (Func<IEnumerable<WorkItemUpdateResult>>) (() => this.UpdateWorkItemsStateOnCheckin(requestContext, (IEnumerable<int>) workItemIdToStateMap.Keys.ToList<int>(), (IList<WorkItemStateOnTransition>) workItemIdToStateMap.Select<KeyValuePair<int, string>, WorkItemStateOnTransition>((Func<KeyValuePair<int, string>, WorkItemStateOnTransition>) (x => new WorkItemStateOnTransition()
      {
        WorkItemId = x.Key,
        NextStateName = x.Value
      })).ToList<WorkItemStateOnTransition>(), commentOnSave, allOrNothing, includeInRecentActivity, workItemIdsToIncludeInRecentActivity, suppressNotifications)));
    }

    public bool TryUpdateWorkItemIdToStateMap(
      IVssRequestContext requestContext,
      string description,
      out IDictionary<int, string> workItemIdToStateMap)
    {
      bool stateMap = false;
      string[] strArray = description.Split('\n');
      workItemIdToStateMap = (IDictionary<int, string>) new Dictionary<int, string>();
      foreach (string source in strArray)
      {
        if (source.Contains<char>('#'))
        {
          int num = source.IndexOf('#');
          string maybeState = Regex.Replace(source.Substring(0, num).Replace(":", "").Trim(), "[\\s+]", " ");
          string str1 = source.Substring(num);
          char[] chArray = new char[1]{ ',' };
          foreach (string str2 in str1.Split(chArray))
          {
            string str3 = str2.Trim();
            if (str3.Contains<char>('#') && str3.Length > 1)
            {
              int index = Regex.Match(str3, "[^0-9]").Index;
              int result;
              WorkItemStateOnTransition nextState;
              if (int.TryParse(str3.Substring(1, index != 0 ? index - 1 : str3.Length - 1), out result) && this.TryGetWorkItemStateOnTransitionFromId(requestContext, result, maybeState, out nextState))
              {
                workItemIdToStateMap.Add(result, nextState.NextStateName);
                stateMap = true;
                CustomerIntelligenceData properties = new CustomerIntelligenceData();
                properties.Add("WorkItemIdInPRDescription", (double) result);
                properties.Add("WorkItemStateInPRDescription", nextState.NextStateName);
                requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, nameof (TryUpdateWorkItemIdToStateMap), properties);
              }
            }
          }
        }
      }
      return stateMap;
    }

    private bool TryGetWorkItemStateOnTransitionFromId(
      IVssRequestContext requestContext,
      int workItemId,
      string maybeState,
      out WorkItemStateOnTransition nextState)
    {
      ITeamFoundationWorkItemService service1 = requestContext.GetService<ITeamFoundationWorkItemService>();
      IProcessWorkItemTypeService service2 = requestContext.GetService<IProcessWorkItemTypeService>();
      IWorkItemTrackingProcessService service3 = requestContext.GetService<IWorkItemTrackingProcessService>();
      IWorkItemStateDefinitionService service4 = requestContext.GetService<IWorkItemStateDefinitionService>();
      IWorkItemTypeService service5 = requestContext.GetService<IWorkItemTypeService>();
      IVssRequestContext requestContext1 = requestContext;
      List<int> workItemIds = new List<int>();
      workItemIds.Add(workItemId);
      DateTime? revisionsSince = new DateTime?();
      WorkItem workItem = service1.GetWorkItems(requestContext1, (IEnumerable<int>) workItemIds, false, false, false, false, errorPolicy: WorkItemErrorPolicy.Omit, revisionsSince: revisionsSince).First<WorkItem>();
      Guid projectGuid = workItem.GetProjectGuid(requestContext);
      nextState = new WorkItemStateOnTransition();
      ProcessDescriptor processDescriptor;
      if (service3.TryGetLatestProjectProcessDescriptor(requestContext, projectGuid, out processDescriptor) && !processDescriptor.IsCustom)
      {
        BaseWorkItemType wit;
        if (!service2.TryGetWorkItemTypeByName(requestContext, processDescriptor.TypeId, workItem.WorkItemType, out wit))
          return false;
        IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions = service4.GetStateDefinitions(requestContext, processDescriptor.TypeId, wit.ReferenceName, true);
        if (this.CheckForStateNameMatch((IList<string>) stateDefinitions.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (ps => !ps.Hidden)).Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (ps => ps.Name)).ToList<string>(), maybeState, workItem.Id, out nextState) || this.CheckForStateCategoryMatch((IEnumerable<WorkItemStateDefinition>) stateDefinitions, maybeState, workItem.Id, out nextState) || this.CheckForKeywordMatch(requestContext, maybeState, workItem, false, out nextState, possibleStates: (IEnumerable<WorkItemStateDefinition>) stateDefinitions, processDescriptorTypeId: new Guid?(processDescriptor.TypeId), baseWitRefName: wit.ReferenceName))
          return true;
      }
      else
      {
        WorkItemType workItemType;
        if (!service5.TryGetWorkItemTypeByName(requestContext, workItem.GetProjectGuid(requestContext), workItem.WorkItemType, out workItemType))
          return false;
        List<string> list = workItemType.GetAdditionalProperties(requestContext).AllowedStates.ToList<string>();
        if (this.CheckForStateNameMatch((IList<string>) list, maybeState, workItem.Id, out nextState) || this.CheckForKeywordMatch(requestContext, maybeState, workItem, true, out nextState, (IEnumerable<string>) list))
          return true;
      }
      nextState = (WorkItemStateOnTransition) null;
      return false;
    }

    private bool CheckForStateNameMatch(
      IList<string> possibleStateNames,
      string maybeState,
      int workItemId,
      out WorkItemStateOnTransition nextState)
    {
      nextState = new WorkItemStateOnTransition();
      string str = possibleStateNames.FirstOrDefault<string>((Func<string, bool>) (ps => TFStringComparer.WorkItemStateName.Equals(ps, maybeState)));
      if (str != null)
      {
        nextState.NextStateName = str;
        nextState.WorkItemId = workItemId;
        return true;
      }
      nextState = (WorkItemStateOnTransition) null;
      return false;
    }

    private bool CheckForStateCategoryMatch(
      IEnumerable<WorkItemStateDefinition> possibleStates,
      string maybeState,
      int workItemId,
      out WorkItemStateOnTransition nextState)
    {
      nextState = new WorkItemStateOnTransition();
      IDictionary<WorkItemStateCategory, IList<string>> dictionary = (IDictionary<WorkItemStateCategory, IList<string>>) new Dictionary<WorkItemStateCategory, IList<string>>()
      {
        {
          WorkItemStateCategory.Proposed,
          (IList<string>) new List<string>()
          {
            "Proposed",
            "Proposes",
            "Propose"
          }
        },
        {
          WorkItemStateCategory.InProgress,
          (IList<string>) new List<string>()
          {
            "InProgress",
            "In Progress"
          }
        },
        {
          WorkItemStateCategory.Completed,
          (IList<string>) new List<string>()
          {
            "Completed",
            "Completes",
            "Complete"
          }
        },
        {
          WorkItemStateCategory.Resolved,
          (IList<string>) new List<string>()
          {
            "Resolved",
            "Resolves",
            "Resolve"
          }
        },
        {
          WorkItemStateCategory.Removed,
          (IList<string>) new List<string>()
          {
            "Removed",
            "Removes",
            "Remove"
          }
        }
      };
      foreach (WorkItemStateCategory itemStateCategory in Enum.GetValues(typeof (WorkItemStateCategory)))
      {
        WorkItemStateCategory stateCategory = itemStateCategory;
        foreach (string str in (IEnumerable<string>) dictionary[stateCategory])
        {
          if (maybeState.Equals(str, StringComparison.OrdinalIgnoreCase))
          {
            WorkItemStateDefinition itemStateDefinition = possibleStates.FirstOrDefault<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (ps => ps.StateCategory == stateCategory && !ps.Hidden));
            if (itemStateDefinition != null)
            {
              nextState.NextStateName = itemStateDefinition.Name;
              nextState.WorkItemId = workItemId;
              return true;
            }
          }
        }
      }
      nextState = (WorkItemStateOnTransition) null;
      return false;
    }

    private bool CheckForKeywordMatch(
      IVssRequestContext requestContext,
      string maybeState,
      WorkItem workItem,
      bool isHostedXML,
      out WorkItemStateOnTransition nextState,
      IEnumerable<string> possibleStateNames = null,
      IEnumerable<WorkItemStateDefinition> possibleStates = null,
      Guid? processDescriptorTypeId = null,
      string baseWitRefName = null)
    {
      nextState = new WorkItemStateOnTransition();
      if (possibleStateNames == null && possibleStates != null)
        possibleStateNames = possibleStates.Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (ps => ps.Name));
      IWorkItemStateDefinitionService service = requestContext.GetService<IWorkItemStateDefinitionService>();
      if (maybeState.Equals("Fixes", StringComparison.OrdinalIgnoreCase) || maybeState.Equals("Fixed", StringComparison.OrdinalIgnoreCase) || maybeState.Equals("Fix", StringComparison.OrdinalIgnoreCase))
      {
        if (!isHostedXML)
        {
          if (service.TryGetNextStateOnCheckinForInheritedProcess(requestContext, processDescriptorTypeId.GetValueOrDefault(), baseWitRefName, workItem.State, out nextState))
          {
            nextState.WorkItemId = workItem.Id;
            return true;
          }
        }
        else if (this.TryGetNextStateOnCheckinForLegacyWit(requestContext, workItem, out nextState))
          return true;
      }
      else if (maybeState.Equals("Closes", StringComparison.OrdinalIgnoreCase) || maybeState.Equals("Closed", StringComparison.OrdinalIgnoreCase) || maybeState.Equals("Close", StringComparison.OrdinalIgnoreCase))
      {
        string str = possibleStateNames.FirstOrDefault<string>((Func<string, bool>) (ps => TFStringComparer.WorkItemStateName.Equals(ps, "Closed")));
        if (!isHostedXML && str == null)
          str = possibleStates.FirstOrDefault<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (ps => ps.StateCategory == WorkItemStateCategory.Completed)).Name;
        if (str != null)
        {
          nextState.NextStateName = str;
          nextState.WorkItemId = workItem.Id;
          return true;
        }
      }
      nextState = (WorkItemStateOnTransition) null;
      return false;
    }

    private IEnumerable<WorkItemUpdateResult> UpdateWorkItemsStateOnCheckin(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IList<WorkItemStateOnTransition> workItemsNextStates,
      string commentOnSave = null,
      bool allOrNothing = false,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IList<WorkItemStateOnTransition>>(workItemsNextStates, nameof (workItemsNextStates));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemIds, nameof (workItemIds));
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      List<WorkItemUpdate> workItemUpdates = new List<WorkItemUpdate>();
      workItemIds.ForEach<int>((Action<int>) (wi => workItemUpdates.Add(new WorkItemUpdate()
      {
        Id = wi
      })));
      if (!workItemsNextStates.Any<WorkItemStateOnTransition>())
        return Enumerable.Empty<WorkItemUpdateResult>();
      foreach (WorkItemStateOnTransition workItemsNextState in (IEnumerable<WorkItemStateOnTransition>) workItemsNextStates)
      {
        WorkItemStateOnTransition workItemNextState = workItemsNextState;
        if (workItemNextState.NextStateName != null)
        {
          List<KeyValuePair<string, object>> keyValuePairList = new List<KeyValuePair<string, object>>();
          keyValuePairList.Add(new KeyValuePair<string, object>(2.ToString(), (object) workItemNextState.NextStateName));
          if (!string.IsNullOrEmpty(commentOnSave))
          {
            KeyValuePair<string, object> keyValuePair = new KeyValuePair<string, object>(54.ToString(), (object) commentOnSave);
            keyValuePairList.Add(keyValuePair);
          }
          workItemUpdates.Where<WorkItemUpdate>((Func<WorkItemUpdate, bool>) (w => w.Id == workItemNextState.WorkItemId)).First<WorkItemUpdate>().Fields = (IEnumerable<KeyValuePair<string, object>>) keyValuePairList;
        }
      }
      return this.UpdateWorkItems(witRequestContext, (IEnumerable<WorkItemUpdate>) workItemUpdates, WorkItemUpdateRuleExecutionMode.Full, allOrNothing, includeInRecentActivity: includeInRecentActivity, workItemIdsToIncludeInRecentActivity: workItemIdsToIncludeInRecentActivity, suppressNotifications: suppressNotifications);
    }

    public IEnumerable<WorkItemUpdateResult> UpdateWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      bool allOrNothing = false,
      bool validateOnly = false,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false,
      bool isPermissionCheckRequiredForBypassRules = false,
      bool useWorkItemIdentity = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemUpdates, nameof (workItemUpdates));
      int maxRevisionCount = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).MaxRevisionsSupportedByUpdateWorkItem;
      WorkItemUpdate workItemUpdate = workItemUpdates.FirstOrDefault<WorkItemUpdate>((Func<WorkItemUpdate, bool>) (w => w.Rev >= maxRevisionCount));
      if (workItemUpdate != null)
        throw new TooManyRevisionsForWorkItemUpdateAPIException(workItemUpdate.Id, workItemUpdate.Rev, maxRevisionCount);
      return this.UpdateWorkItems(requestContext.WitContext(), workItemUpdates, ruleExecutionMode, allOrNothing, validateOnly, includeInRecentActivity, workItemIdsToIncludeInRecentActivity, suppressNotifications, isPermissionCheckRequiredForBypassRules, useWorkItemIdentity);
    }

    public void CreateOrUpdateWorkItemReactionsAggregateCount(
      IVssRequestContext requestContext,
      int workItemId,
      SocialEngagementType socialEngagementType,
      int incrementCounterValue)
    {
      requestContext.TraceBlock(904936, 904937, "Services", "WorkItemService", nameof (CreateOrUpdateWorkItemReactionsAggregateCount), (Action) (() =>
      {
        using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
          component.CreateOrUpdateWorkItemReactionsAggregateCount(workItemId, socialEngagementType, incrementCounterValue);
      }));
    }

    internal IEnumerable<WorkItemUpdateResult> UpdateWorkItems(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode = WorkItemUpdateRuleExecutionMode.ValidationOnly,
      bool allOrNothing = false,
      bool validateOnly = false,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false,
      bool isPermissionCheckRequiredForBypassRules = false,
      bool useWorkItemIdentity = false,
      bool checkRevisionsLimit = false)
    {
      ArgumentUtility.CheckForNull<WorkItemTrackingRequestContext>(witRequestContext, nameof (witRequestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemUpdates, nameof (workItemUpdates));
      WorkItemTrackingRequestContext witRequestContext1 = witRequestContext;
      IEnumerable<WorkItemUpdate> workItemUpdates1 = workItemUpdates;
      int num1 = (int) ruleExecutionMode;
      bool flag = validateOnly;
      int num2 = allOrNothing ? 1 : 0;
      int num3 = flag ? 1 : 0;
      int num4 = includeInRecentActivity ? 1 : 0;
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity1 = workItemIdsToIncludeInRecentActivity;
      int num5 = suppressNotifications ? 1 : 0;
      int num6 = isPermissionCheckRequiredForBypassRules ? 1 : 0;
      int num7 = useWorkItemIdentity ? 1 : 0;
      int num8 = checkRevisionsLimit ? 1 : 0;
      return this.UpdateWorkItemsInternal(witRequestContext1, workItemUpdates1, (WorkItemUpdateRuleExecutionMode) num1, num2 != 0, num3 != 0, includeInRecentActivity: num4 != 0, workItemIdsToIncludeInRecentActivity: workItemIdsToIncludeInRecentActivity1, suppressNotifications: num5 != 0, isPermissionCheckRequiredForBypassRules: num6 != 0, useWorkItemIdentity: num7 != 0, checkRevisionsLimit: num8 != 0);
    }

    public IEnumerable<WorkItemUpdateResult> UpdateWorkItemsRemoteLinkOnly(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      bool suppressQueueRemoteLinkJob = false)
    {
      return this.UpdateWorkItemsInternal(requestContext.WitContext(), workItemUpdates, suppressQueueRemoteLinkJob: suppressQueueRemoteLinkJob);
    }

    private static void CheckAndRevertUnchangedFields(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      foreach (WorkItemUpdateState workItemUpdateState in updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success && us.FieldData != null)))
      {
        WorkItemFieldData fieldData = workItemUpdateState.FieldData;
        if (!fieldData.HasUpdates || !workItemUpdateState.HasResourceLinkUpdatesRequiringNewRevision && fieldData.Updates.Values.All<object>((Func<object, bool>) (v => v is ServerDefaultFieldValue)))
        {
          workItemUpdateState.ClearFieldUpdates();
          workItemUpdateState.FieldData.ClearFieldUpdates();
        }
        else
        {
          foreach (KeyValuePair<int, object> keyValuePair in workItemUpdateState.FieldUpdates.ToList<KeyValuePair<int, object>>())
          {
            int key = keyValuePair.Key;
            if (!fieldData.Updates.ContainsKey(key))
              workItemUpdateState.RemoveFieldUpdate(key);
          }
        }
      }
    }

    private static void CheckAndRevertChangedByFieldIfNeeded(
      IEnumerable<WorkItemUpdateState> updateStates,
      bool isNoHistoryEnabledFieldsSupported)
    {
      if (!isNoHistoryEnabledFieldsSupported)
        return;
      foreach (WorkItemUpdateState workItemUpdateState in updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.HasFieldUpdates && us.FieldUpdates.Any<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (fu => fu.Key == 9)))))
      {
        if (!workItemUpdateState.NeedsNewRevision(true))
          workItemUpdateState.RemoveFieldUpdate(9);
      }
    }

    internal virtual void UpdateValuesAndExecuteRules(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      ResolvedIdentityNamesInfo resolvedNamesInfo,
      bool allOrNothing,
      WorkItemUpdateMode updateMode = WorkItemUpdateMode.Normal)
    {
      witRequestContext.RequestContext.TraceBlock(904305, 904306, "Services", "WorkItemService", nameof (UpdateValuesAndExecuteRules), (Action) (() =>
      {
        IVssRequestContext requestContext = witRequestContext.RequestContext;
        IWorkItemTypeExtensionsMatcher extensionMatcher = requestContext.GetService<WorkItemTypeExtensionService>().GetExtensionMatcher(requestContext, new Guid?(), new Guid?());
        foreach (WorkItemUpdateState updateState1 in updateStates)
        {
          WorkItemUpdateState updateState = updateState1;
          if (updateState.NeedsNewRevision(false))
            this.UpdateDefaultFieldValues(witRequestContext, updateState, ruleExecutionMode);
          if (updateState.Success && updateState.FieldData != null)
            this.PrepareCurrentExtensionFields(witRequestContext, extensionMatcher, updateState);
          IEnumerable<WorkItemFieldRule> workItemFieldRules = (IEnumerable<WorkItemFieldRule>) null;
          if (updateState.Success && updateState.FieldData != null && (updateState.HasFieldUpdates || updateState.FieldData.HasUpdates))
          {
            RuleEngine ruleEngine = (RuleEngine) null;
            RuleEngineConfiguration engineConfiguration = ruleExecutionMode == WorkItemUpdateRuleExecutionMode.Full ? RuleEngineConfiguration.ServerFull : RuleEngineConfiguration.ServerValidationOnly;
            Guid projectId;
            if (updateState.HasTeamProjectChanged)
            {
              IProjectService service = witRequestContext.RequestContext.GetService<IProjectService>();
              string projectName = (string) updateState.Update.Fields.Single<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (u => TFStringComparer.WorkItemFieldReferenceName.Equals(u.Key, "System.TeamProject"))).Value;
              Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project;
              try
              {
                project = service.GetProject(witRequestContext.RequestContext, projectName);
              }
              catch (ProjectDoesNotExistWithNameException ex)
              {
                updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTargetProjectDoesNotExistException(projectName));
                if (allOrNothing)
                  break;
                continue;
              }
              projectId = project.Id;
            }
            else
              projectId = updateState.FieldData.GetProjectGuid(witRequestContext);
            string workItemTypeName;
            if (updateState.HasWorkItemTypeChanged)
            {
              object obj = updateState.Update.Fields.Single<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (u => TFStringComparer.WorkItemFieldReferenceName.Equals(u.Key, "System.WorkItemType"))).Value;
              workItemTypeName = obj is string ? (string) obj : throw new WorkItemFieldInvalidException(updateState.FieldData.Id, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FieldTypeInvalid());
              if (WorkItemTrackingFeatureFlags.IsChangeTestCaseWitEnabled(witRequestContext.RequestContext) && requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(requestContext, projectId).Where<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (x => x.ReferenceName == "Microsoft.TestCaseCategory" || x.ReferenceName == "Microsoft.TestPlanCategory" || x.ReferenceName == "Microsoft.TestSuiteCategory" || x.ReferenceName == "Microsoft.HiddenCategory")).Any<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (x => x.WorkItemTypeNames.Any<string>((Func<string, bool>) (n => n == updateState.FieldData.WorkItemType)))))
                updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTypeNotChangeableException(updateState.FieldData.WorkItemType));
            }
            else
              workItemTypeName = updateState.FieldData.GetFieldValue<string>(witRequestContext, 25);
            WorkItemType workItemType = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(witRequestContext.RequestContext, projectId).FirstOrDefault<WorkItemType>((Func<WorkItemType, bool>) (t => t.Name.Equals(workItemTypeName, StringComparison.OrdinalIgnoreCase)));
            if (workItemType == null)
            {
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTargetTypeDoesNotExistException(workItemTypeName, projectId));
              if (allOrNothing)
                break;
              continue;
            }
            workItemTypeName = workItemType.Name;
            if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(witRequestContext.RequestContext) && workItemType != null && workItemType.IsDisabled && (updateState.Update.IsNew || updateState.HasWorkItemTypeChanged))
            {
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTargetTypeIsDisabledException(workItemTypeName));
              if (allOrNothing)
                break;
              continue;
            }
            if (ruleExecutionMode != WorkItemUpdateRuleExecutionMode.Bypass)
            {
              try
              {
                workItemFieldRules = workItemType.GetAdditionalProperties(witRequestContext.RequestContext).FieldRules;
                IEnumerable<WorkItemFieldRule> executableRules = this.GetExecutableRules(updateState);
                ruleEngine = new RuleEngine(workItemFieldRules.Union<WorkItemFieldRule>(executableRules), engineConfiguration);
              }
              catch (WorkItemTrackingServiceException ex)
              {
                updateState.UpdateResult.AddException((TeamFoundationServiceException) ex);
                if (allOrNothing)
                  break;
                continue;
              }
            }
            try
            {
              switch (ruleExecutionMode)
              {
                case WorkItemUpdateRuleExecutionMode.ValidationOnly:
                  updateState.FieldData.SetFieldUpdates(witRequestContext, updateState.FieldUpdates);
                  this.ExecuteWorkItemRules(witRequestContext, updateState, engineConfiguration, ruleEngine, identityMap, resolvedNamesInfo);
                  if (updateState.FieldUpdates.Any<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (update => update.Key == 25)))
                  {
                    if (workItemTypeName != null)
                    {
                      updateState.FieldData.SetFieldValue(witRequestContext, 25, (object) workItemTypeName);
                      break;
                    }
                    break;
                  }
                  break;
                case WorkItemUpdateRuleExecutionMode.Full:
                  this.ExecuteWorkItemRules(witRequestContext, updateState, engineConfiguration, ruleEngine, identityMap, resolvedNamesInfo);
                  using (IEnumerator<KeyValuePair<int, object>> enumerator = updateState.FieldUpdates.GetEnumerator())
                  {
                    while (enumerator.MoveNext())
                    {
                      KeyValuePair<int, object> current = enumerator.Current;
                      if (current.Key == 25 && workItemTypeName != null)
                        ruleEngine.SetFieldValue(updateState.RuleEvalContext, current.Key, (object) workItemTypeName);
                      else
                        ruleEngine.SetFieldValue(updateState.RuleEvalContext, current.Key, current.Value);
                    }
                    break;
                  }
                default:
                  updateState.FieldData.SetFieldUpdates(witRequestContext, updateState.FieldUpdates);
                  if (updateState.FieldUpdates.Any<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (update => update.Key == 25)))
                  {
                    if (workItemTypeName != null)
                    {
                      updateState.FieldData.SetFieldValue(witRequestContext, 25, (object) workItemTypeName);
                      break;
                    }
                    break;
                  }
                  break;
              }
            }
            catch (TeamFoundationServiceException ex)
            {
              updateState.UpdateResult.AddException(ex);
              if (allOrNothing)
                break;
              continue;
            }
          }
          if (updateState.Success && updateState.FieldData != null)
            this.ExecuteExtensionRules(witRequestContext, extensionMatcher, updateState, updateMode, workItemFieldRules);
          this.ReportRuleErrors(witRequestContext, updateState);
        }
      }));
    }

    internal virtual void Evaluate(WorkItemUpdateState updateState, RuleEngine ruleEngine) => ruleEngine.Evaluate(updateState.RuleEvalContext);

    internal virtual IEnumerable<WorkItemFieldRule> GetExecutableRules(
      WorkItemUpdateState updateState)
    {
      return ((IEnumerable<WorkItemTypeExtension>) updateState.CurrentExtensions).SelectMany<WorkItemTypeExtension, WorkItemFieldRule>((Func<WorkItemTypeExtension, IEnumerable<WorkItemFieldRule>>) (e => e.ExecutableRules));
    }

    internal virtual WorkItemUpdateResultSet Update(
      IVssRequestContext requestContext,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      bool isAdmin,
      int trendDataInterval,
      WorkItemUpdateDataset updateDataset,
      bool isWorkItemRestore = false)
    {
      ArgumentUtility.CheckForNull<WorkItemUpdateDataset>(updateDataset, nameof (updateDataset));
      return requestContext.TraceBlock<WorkItemUpdateResultSet>(904811, 904820, 904815, "Services", "WorkItemService", "UpdateWorkItems.UpdateWorkItemsComponentCall", (Func<WorkItemUpdateResultSet>) (() =>
      {
        int num1 = isWorkItemRestore ? requestContext.WitContext().ServerSettings.WorkItemRestoreLinksLimit : requestContext.WitContext().ServerSettings.WorkItemLinksLimit;
        int remoteLinksLimit = requestContext.WitContext().ServerSettings.WorkItemRemoteLinksLimit;
        using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        {
          WorkItemComponent workItemComponent = component;
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          WorkItemUpdateDataset itemUpdateDataset = updateDataset;
          int num2 = ruleExecutionMode == WorkItemUpdateRuleExecutionMode.Bypass ? 1 : 0;
          int num3 = isAdmin ? 1 : 0;
          int trendInterval = trendDataInterval;
          WorkItemUpdateDataset updateDataset1 = itemUpdateDataset;
          int workItemLinksLimit = num1;
          int workItemRemoteLinksLimit = remoteLinksLimit;
          WorkItemUpdateResultSet itemUpdateResultSet = workItemComponent.UpdateWorkItems((IVssIdentity) userIdentity, num2 != 0, num3 != 0, trendInterval, true, updateDataset1, workItemLinksLimit, workItemRemoteLinksLimit);
          foreach (WorkItemCoreFieldUpdatesRecord coreFieldUpdate in updateDataset.CoreFieldUpdates)
          {
            CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            if (string.Equals(coreFieldUpdate.WorkItemType, "Test Case") && coreFieldUpdate.Id >= 0)
            {
              properties.Add("TestCaseId", (double) coreFieldUpdate.Id);
              properties.Add("TestCaseState", coreFieldUpdate.State);
              service.Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, "UpdateTestCaseDefinition", properties);
            }
          }
          return itemUpdateResultSet;
        }
      }));
    }

    internal IEnumerable<KeyValuePair<int, int>> UpdateReconciledWorkItems(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<WorkItemCustomFieldUpdateRecord> updateRecords,
      Microsoft.VisualStudio.Services.Identity.Identity changedBy,
      bool continueWhenDateTimeShiftDetected = true,
      bool skipWITChangeDateUpdate = false)
    {
      return witRequestContext.RequestContext.TraceBlock<IEnumerable<KeyValuePair<int, int>>>(904821, 904830, 904825, "Services", "WorkItemService", "UpdateWorkItems.UpdateReconciledWorkItems", (Func<IEnumerable<KeyValuePair<int, int>>>) (() =>
      {
        if (!workItemIdRevPairs.Any<WorkItemIdRevisionPair>())
          return Enumerable.Empty<KeyValuePair<int, int>>();
        int trendDataInterval = this.GetTrendDataInterval(witRequestContext.RequestContext);
        using (WorkItemComponent component = witRequestContext.RequestContext.CreateComponent<WorkItemComponent>())
          return (IEnumerable<KeyValuePair<int, int>>) component.UpdateReconciledWorkItems(workItemIdRevPairs, updateRecords, (IVssIdentity) witRequestContext.RequestContext.GetUserIdentity(), (IVssIdentity) changedBy, trendDataInterval, true, continueWhenDateTimeShiftDetected, skipWITChangeDateUpdate).ToList<KeyValuePair<int, int>>();
      }));
    }

    internal virtual IEnumerable<WorkItemUpdateState> CreateUpdateStatesAndValidate(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      WorkItemUpdateMode updateMode,
      bool isNoHistoryEnabledFieldsSupported)
    {
      return UpdateStateCreator.Create(witRequestContext, workItemUpdates, ruleExecutionMode, updateMode, isNoHistoryEnabledFieldsSupported);
    }

    internal virtual Dictionary<int, WorkItemFieldData> ReadWorkItemTipFieldValues(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      bool useWorkItemIdentity,
      bool checkForSoftDeletedProjects = false)
    {
      return witRequestContext.RequestContext.TraceBlock<Dictionary<int, WorkItemFieldData>>(904220, 904229, 904228, "Services", "WorkItemService", "UpdateWorkItems.ReadTip", (Func<Dictionary<int, WorkItemFieldData>>) (() =>
      {
        HashSet<int> source1 = new HashSet<int>();
        HashSet<int> intSet = new HashSet<int>();
        bool flag1 = false;
        foreach (WorkItemUpdateState workItemUpdateState in updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (wus => wus.Success && !wus.Update.IsNew && !wus.Update.HasTipValues)))
        {
          if (workItemUpdateState.HasFieldUpdates || workItemUpdateState.HasResourceLinkUpdates || workItemUpdateState.HasLinkUpdates)
          {
            source1.Add(workItemUpdateState.Id);
            if (workItemUpdateState.HasLinkUpdates)
              flag1 = flag1 || workItemUpdateState.Update.LinkUpdates.Any<WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, bool>) (lu => witRequestContext.LinkService.GetLinkTypeById(witRequestContext.RequestContext, lu.LinkType).IsRemote));
          }
          else
            intSet.Add(workItemUpdateState.Id);
        }
        IEnumerable<WorkItemFieldData> source2;
        if (source1.Any<int>())
        {
          bool flag2 = witRequestContext.RequestContext.IsFeatureEnabled("WorkItemTracking.Server.AllowTagsInAlerts") && witRequestContext.RequestContext.IsFeatureEnabled("WorkItemTracking.Server.FireWorkItemsChangedWithAllCustomFields");
          bool flag3 = updateStates.Any<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.HasResourceLinkUpdates || us.HasHtmlFieldUpdates));
          IVssRequestContext requestContext = witRequestContext.RequestContext;
          HashSet<int> workItemIds = source1;
          int num1 = flag2 ? 1 : (updateStates.Any<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.HasTagsFieldChanges || us.HasTeamProjectChanged)) ? 1 : 0);
          int num2 = flag3 ? 1 : 0;
          bool flag4 = flag1;
          int num3 = useWorkItemIdentity ? 1 : 0;
          int num4 = flag4 ? 1 : 0;
          source2 = (IEnumerable<WorkItemFieldData>) this.GetWorkItemFieldValues(requestContext, (IEnumerable<int>) workItemIds, IdentityDisplayType.ComboDisplayName, 16, includeTags: num1 != 0, includeResourceLinks: num2 != 0, useWorkItemIdentity: num3 != 0, includeWorkItemLinks: num4 != 0);
        }
        else
          source2 = (IEnumerable<WorkItemFieldData>) Enumerable.Empty<WorkItemRevision>();
        Dictionary<int, WorkItemFieldData> dictionary = source2.ToDictionary<WorkItemFieldData, int, WorkItemFieldData>((Func<WorkItemFieldData, int>) (data => data.Id), (Func<WorkItemFieldData, WorkItemFieldData>) (data => data));
        foreach (WorkItemUpdateState workItemUpdateState in updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (wus => wus.Success && wus.HasLinkUpdates)))
        {
          foreach (WorkItemLinkUpdate linkUpdate in workItemUpdateState.Update.LinkUpdates)
          {
            if (!witRequestContext.LinkService.GetLinkTypeById(witRequestContext.RequestContext, linkUpdate.LinkType).IsRemote)
            {
              int num = linkUpdate.SourceWorkItemId == workItemUpdateState.Id ? linkUpdate.TargetWorkItemId : linkUpdate.SourceWorkItemId;
              if (num > 0 && !source1.Contains(num))
                intSet.Add(num);
            }
          }
        }
        IEnumerable<int> first = (IEnumerable<int>) CoreFieldConstants.AlertableCoreFields;
        if (witRequestContext.FieldDictionary.TryGetField(-35, out FieldEntry _))
          first = first.Union<int>((IEnumerable<int>) new int[1]
          {
            -35
          });
        IVssRequestContext requestContext1 = witRequestContext.RequestContext;
        HashSet<int> workItemIds1 = intSet;
        IEnumerable<int> fields = first;
        bool flag5 = useWorkItemIdentity;
        bool flag6 = checkForSoftDeletedProjects;
        DateTime? asOf = new DateTime?();
        int num5 = flag5 ? 1 : 0;
        int num6 = flag6 ? 1 : 0;
        foreach (WorkItemFieldData workItemFieldValue in this.GetWorkItemFieldValues(requestContext1, (IEnumerable<int>) workItemIds1, fields, IdentityDisplayType.ComboDisplayName, 16, asOf, useWorkItemIdentity: num5 != 0, checkForSoftDeletedProjects: num6 != 0))
          dictionary[workItemFieldValue.Id] = workItemFieldValue;
        foreach (WorkItemUpdateState workItemUpdateState in updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (wus => wus.Success && wus.Update.HasTipValues)))
          dictionary[workItemUpdateState.Id] = workItemUpdateState.Update.FieldData;
        WorkItemFieldData workItemFieldData;
        foreach (WorkItemUpdateState workItemUpdateState in updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (wus => wus.Success)))
        {
          if (workItemUpdateState.Update.IsNew)
          {
            if (workItemUpdateState.Update.FieldData == null)
            {
              workItemFieldData = new WorkItemFieldData((IDictionary<int, object>) new Dictionary<int, object>()
              {
                {
                  -3,
                  (object) workItemUpdateState.Update.Id
                },
                {
                  8,
                  (object) 0
                }
              });
              foreach (KeyValuePair<int, object> fieldUpdate in workItemUpdateState.FieldUpdates)
              {
                switch (fieldUpdate.Key)
                {
                  case -105:
                  case -104:
                  case -7:
                  case -2:
                  case 25:
                    workItemFieldData.SetFieldValue(witRequestContext, fieldUpdate.Key, fieldUpdate.Value);
                    continue;
                  default:
                    continue;
                }
              }
              workItemUpdateState.FieldData = workItemFieldData;
              dictionary[workItemUpdateState.Update.Id] = workItemFieldData;
            }
            else
              workItemUpdateState.FieldData = workItemUpdateState.Update.FieldData;
          }
          else if (dictionary.TryGetValue(workItemUpdateState.Update.Id, out workItemFieldData))
          {
            workItemUpdateState.FieldData = workItemFieldData;
            if (workItemFieldData is WorkItemRevision)
              workItemUpdateState.ResourceLinks = (IEnumerable<WorkItemResourceLinkInfo>) (workItemFieldData as WorkItemRevision).ResourceLinks;
          }
          else
            workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemUnauthorizedAccessException(workItemUpdateState.Update.Id, AccessType.Read));
        }
        return dictionary;
      }));
    }

    internal void UpdateDefaultFieldValues(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode)
    {
      try
      {
        WorkItemFieldData fieldData = updateState.FieldData;
        bool flag = ruleExecutionMode == WorkItemUpdateRuleExecutionMode.Bypass;
        if (fieldData == null)
          return;
        if (updateState.Update.IsNew)
        {
          if (!this.HasFieldUpdate(witRequestContext, updateState, 33) || !flag)
            fieldData.SetFieldValue(witRequestContext, 33, (object) new ServerDefaultFieldValue(ServerDefaultType.CallerIdentity));
          if (!this.HasFieldUpdate(witRequestContext, updateState, 32) || !flag)
            fieldData.SetFieldValue(witRequestContext, 32, (object) new ServerDefaultFieldValue(ServerDefaultType.ServerDateTime));
        }
        if (!this.HasFieldUpdate(witRequestContext, updateState, 9) || !flag)
          fieldData.SetFieldValue(witRequestContext, 9, (object) new ServerDefaultFieldValue(ServerDefaultType.CallerIdentity));
        if (this.HasFieldUpdate(witRequestContext, updateState, -4) && flag)
          return;
        fieldData.SetFieldValue(witRequestContext, -4, (object) new ServerDefaultFieldValue(ServerDefaultType.ServerDateTime));
      }
      catch (TeamFoundationServiceException ex)
      {
        updateState.UpdateResult.AddException(ex);
      }
    }

    internal void CheckAndRevertFieldDataUpdatesWithNoNewRevision(WorkItemUpdateState updateState)
    {
      if (updateState.FieldData.Updates.Count == 0 || updateState.HasFieldUpdates)
        return;
      updateState.FieldData.Updates.Clear();
    }

    internal bool HasFieldUpdate(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState,
      int fieldId)
    {
      if (updateState.Update.Fields == null)
        return false;
      string fieldName = witRequestContext.FieldDictionary.GetField(fieldId).ReferenceName;
      return updateState.Update.Fields.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => string.Equals(kvp.Key, fieldName, StringComparison.OrdinalIgnoreCase)));
    }

    internal void CheckWorkItemTypeUpdate(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState)
    {
      if (WorkItemTrackingFeatureFlags.IsChangeWorkItemTypeEnabled(witRequestContext.RequestContext) || updateState.Update.IsNew || !updateState.HasFieldUpdate(25) || TFStringComparer.WorkItemTypeName.Equals(updateState.FieldData.GetFieldValue<string>(witRequestContext, 25), updateState.FieldData.GetFieldValue<string>(witRequestContext, 25, true)))
        return;
      updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidException(updateState.Id, witRequestContext.FieldDictionary.GetField(25).ReferenceName, FieldStatusFlags.InvalidNotOldValue));
    }

    internal void CheckResourceLinkUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState)
    {
      if (!updateState.HasResourceLinkUpdates)
        return;
      List<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType> workArtifactLinks = witRequestContext.RequestContext.TraceBlock<List<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType>>(904904, 904906, 904905, "Services", "WorkItemService", "UpdateWorkItems.GetRegistrationEntries", (Func<List<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType>>) (() =>
      {
        workArtifactLinks = witRequestContext.RequestContext.GetService<IArtifactLinkTypesService>().GetArtifactLinkTypes(witRequestContext.RequestContext, "WorkItemTracking").Where<RegistrationArtifactType>((Func<RegistrationArtifactType, bool>) (artifactType => VssStringComparer.ArtifactType.Equals(artifactType.Name, "WorkItem"))).SelectMany<RegistrationArtifactType, Microsoft.TeamFoundation.Framework.Server.OutboundLinkType>((Func<RegistrationArtifactType, IEnumerable<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType>>) (type => (IEnumerable<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType>) type.OutboundLinkTypes)).ToList<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType>();
        return workArtifactLinks;
      }));
      foreach (WorkItemResourceLinkUpdate resourceLinkUpdate in updateState.Update.ResourceLinkUpdates)
      {
        ResourceLinkType? type = resourceLinkUpdate.Type;
        ResourceLinkType resourceLinkType = ResourceLinkType.ArtifactLink;
        if (type.GetValueOrDefault() == resourceLinkType & type.HasValue && (resourceLinkUpdate.UpdateType == LinkUpdateType.Add || resourceLinkUpdate.UpdateType == LinkUpdateType.Update))
        {
          ArtifactId artifactId;
          try
          {
            artifactId = LinkingUtilities.DecodeUri(resourceLinkUpdate.Location);
          }
          catch (Exception ex)
          {
            updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemResourceLinkException(updateState.Id, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidResourceLinkTarget((object) resourceLinkUpdate.Location)));
            continue;
          }
          if (!this.ResourceLinkInCollection((IEnumerable<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType>) workArtifactLinks, resourceLinkUpdate, artifactId) && !this.ResourceLinkInCollection((IEnumerable<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType>) this.m_whitelistedOutboundLinks, resourceLinkUpdate, artifactId))
            updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemResourceLinkException(updateState.Id, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.UnrecognizedResourceLink((object) resourceLinkUpdate.Name, (object) resourceLinkUpdate.Location)));
        }
      }
    }

    internal void CheckClassificationNode<TNode>(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState,
      int classificationNodeField)
    {
      TreeStructureType treeStructureType;
      if (classificationNodeField == -104 || classificationNodeField == -105)
      {
        treeStructureType = TreeStructureType.Iteration;
      }
      else
      {
        if (classificationNodeField != -2 && classificationNodeField != -7)
          throw new InvalidOperationException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidTreeNodeIdentifier());
        treeStructureType = TreeStructureType.Area;
      }
      if (!updateState.HasFieldUpdate(classificationNodeField))
        return;
      TNode fieldValue1 = updateState.FieldData.GetFieldValue<TNode>(witRequestContext, classificationNodeField);
      string referenceName = witRequestContext.FieldDictionary.GetField(classificationNodeField).ReferenceName;
      TreeNode treeNode1;
      TreeNode treeNode2;
      if (this.TryGetTreeNode<int>(witRequestContext, updateState.FieldData.AreaId, out treeNode1) && this.TryGetTreeNode<int>(witRequestContext, updateState.FieldData.IterationId, out treeNode2) && treeNode1.ProjectId != treeNode2.ProjectId)
      {
        if (updateState.HasTeamProjectChanged)
          return;
        updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidTreeIdException(updateState.Id, referenceName));
      }
      else
      {
        TreeNode treeNode3;
        if (this.TryGetTreeNode<TNode>(witRequestContext, fieldValue1, out treeNode3, treeStructureType))
        {
          if (treeNode3.ProjectId != updateState.FieldData.GetProjectGuid(witRequestContext))
          {
            updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidTreeIdException(updateState.Id, referenceName));
          }
          else
          {
            if (updateState.Update.IsNew || updateState.HasTeamProjectChanged)
              return;
            TNode fieldValue2 = updateState.FieldData.GetFieldValue<TNode>(witRequestContext, classificationNodeField, true);
            TreeNode treeNode4;
            if (!this.TryGetTreeNode<TNode>(witRequestContext, fieldValue2, out treeNode4, treeStructureType) || !(treeNode4.ProjectId != treeNode3.ProjectId))
              return;
            updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidTreeIdException(updateState.Id, referenceName));
          }
        }
        else
          updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidTreeIdException(updateState.Id, referenceName));
      }
    }

    internal virtual void ReportRuleErrors(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState)
    {
      if (updateState.FieldData == null)
        return;
      IRuleEvaluationContext ruleEvalContext = updateState.RuleEvalContext;
      if (!ruleEvalContext.FirstInvalidFieldId.HasValue)
        return;
      IEnumerable<FieldRuleEvalutionStatus> source = ruleEvalContext.RuleEvaluationStatuses.Values.Where<FieldRuleEvalutionStatus>((Func<FieldRuleEvalutionStatus, bool>) (status => (status.Flags & FieldStatusFlags.InvalidMask) != 0));
      updateState.UpdateResult.AddExceptions((IEnumerable<TeamFoundationServiceException>) source.Select<FieldRuleEvalutionStatus, RuleValidationException>((Func<FieldRuleEvalutionStatus, RuleValidationException>) (status => new RuleValidationException(witRequestContext.FieldDictionary.GetField(status.FieldId).ReferenceName, witRequestContext.FieldDictionary.GetField(status.FieldId).Name, status.Flags, status.Value))));
    }

    internal virtual void CheckUpdatePermissions(
      WorkItemTrackingRequestContext witRequestContext,
      Dictionary<int, WorkItemUpdateState> updateStatesMap,
      Dictionary<int, WorkItemFieldData> workItemFieldDataMap,
      WorkItemUpdateMode updateMode = WorkItemUpdateMode.Normal)
    {
      this.CheckWorkItemWritePermissions(witRequestContext, updateStatesMap.Values.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success && !us.HasOnlyLinkUpdates)), updateMode);
      this.ValidateLinkUpdatesAndCheckLinkPermissions(witRequestContext, updateStatesMap, workItemFieldDataMap);
    }

    internal virtual string HandleBlockElementLineBreaks(string htmlText)
    {
      StringBuilder stringBuilder = new StringBuilder(htmlText.Length * 105 / 100 + 5);
      int startIndex1 = 0;
      int num = 0;
      int startIndex2 = -1;
      string tagName;
      bool isOpeningTag;
      bool spaceBefore;
      int tagLength;
      while ((startIndex2 = this.IndexOfTag(htmlText, startIndex2 + 1, out tagLength, out tagName, out isOpeningTag, out spaceBefore)) >= 0)
      {
        if (string.Equals(tagName, "code", StringComparison.OrdinalIgnoreCase))
        {
          if (!isOpeningTag)
            --num;
          else
            ++num;
        }
        else if (!isOpeningTag && num == 0 && !spaceBefore && ((IEnumerable<string>) this.targetTags).Contains<string>(tagName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        {
          stringBuilder.Append(htmlText, startIndex1, startIndex2 - startIndex1);
          stringBuilder.Append(' ');
          stringBuilder.Append(htmlText, startIndex2, tagLength);
          startIndex1 = startIndex2 + tagLength;
        }
      }
      if (startIndex1 == 0)
        return htmlText;
      if (startIndex1 < htmlText.Length)
        stringBuilder.Append(htmlText, startIndex1, htmlText.Length - startIndex1);
      return stringBuilder.ToString();
    }

    private int IndexOfTag(
      string html,
      int startIndex,
      out int tagLength,
      out string tagName,
      out bool isOpeningTag,
      out bool spaceBefore)
    {
      int length = html.Length;
      int num1 = 0;
      spaceBefore = false;
      tagLength = 0;
      tagName = (string) null;
      isOpeningTag = false;
      for (; startIndex < length; ++startIndex)
      {
        char c = html[startIndex];
        if (c != '<')
          spaceBefore = char.IsWhiteSpace(c);
        else
          break;
      }
      if (startIndex >= length)
        return -1;
      int num2 = startIndex++;
      if (startIndex >= length)
        return -1;
      char c1 = html[startIndex];
      isOpeningTag = c1 != '/';
      for (; c1 != '>'; c1 = html[startIndex])
      {
        if (char.IsWhiteSpace(c1))
          num1 = isOpeningTag ? startIndex - num2 - 1 : startIndex - num2 - 2;
        if (++startIndex >= length)
          return -1;
      }
      tagLength = startIndex - num2 + 1;
      int startIndex1 = isOpeningTag ? num2 + 1 : num2 + 2;
      tagName = html.Substring(startIndex1, num1 > 0 ? num1 : (isOpeningTag ? tagLength - 2 : tagLength - 3));
      return num2;
    }

    internal virtual (string text, string fixedText, List<WorkItemResourceLinkUpdateRecord> linkRecords) HandleInlineImages(
      WorkItemTrackingRequestContext witRequestContext,
      int workItemId,
      string text,
      Guid projectGuid,
      WorkItemFieldData fieldData,
      ref int sequenceOrder)
    {
      ArgumentUtility.CheckForNull<string>(text, nameof (text));
      TeamFoundationWorkItemService.UrlReplacementMap urlReplacementMap = witRequestContext.GetOrAddCacheItem<TeamFoundationWorkItemService.UrlReplacementMap>("UrlsToReplace", (Func<TeamFoundationWorkItemService.UrlReplacementMap>) (() => this.BuildUrlReplacementMap(witRequestContext.RequestContext)));
      this.AddProjectAttachmentUrlsToMap(witRequestContext.RequestContext, projectGuid, urlReplacementMap);
      witRequestContext.RequestContext.TraceBlock(904939, 904940, "Services", "WorkItemService", "UpdateWorkItems.HandleInlineImagesReplace", (Action) (() =>
      {
        foreach (KeyValuePair<string, char> keyValuePair in (IEnumerable<KeyValuePair<string, char>>) urlReplacementMap.UrlsToReplace)
          text = text.ReplaceStringWithChar(keyValuePair.Key, keyValuePair.Value);
      }));
      witRequestContext.RequestContext.TraceBlock(904941, 904942, "Services", "WorkItemService", "UpdateWorkItems.HandleInlineImagesRegex", (Action) (() =>
      {
        foreach (KeyValuePair<Regex, char> keyValuePair in (IEnumerable<KeyValuePair<Regex, char>>) urlReplacementMap.UrlsToReplaceUsingRegex)
          text = keyValuePair.Key.Replace(text, string.Format("{0}", (object) keyValuePair.Value));
      }));
      Dictionary<Guid, int> existingInlineImages = new Dictionary<Guid, int>();
      if (fieldData != null && fieldData is WorkItemRevision workItemRevision)
        existingInlineImages = workItemRevision.HiddenResourceLinks.ToDedupedDictionary<WorkItemResourceLinkInfo, Guid, int>((Func<WorkItemResourceLinkInfo, Guid>) (rl => Guid.Parse(rl.Location)), (Func<WorkItemResourceLinkInfo, int>) (rl => rl.ResourceId));
      HashSet<WorkItemResourceLinkUpdateRecord> linkUpdateRecordSet = new HashSet<WorkItemResourceLinkUpdateRecord>((IEqualityComparer<WorkItemResourceLinkUpdateRecord>) InlineImageComparer.Instance);
      linkUpdateRecordSet.AddRange<WorkItemResourceLinkUpdateRecord, HashSet<WorkItemResourceLinkUpdateRecord>>(this.ParseLegacyInlineImages(workItemId, text, projectGuid, existingInlineImages, ref sequenceOrder));
      linkUpdateRecordSet.AddRange<WorkItemResourceLinkUpdateRecord, HashSet<WorkItemResourceLinkUpdateRecord>>(this.ParseProjectInlineImages(workItemId, text, projectGuid, existingInlineImages, ref sequenceOrder));
      string str = text;
      foreach (KeyValuePair<char, string> keyValuePair in (IEnumerable<KeyValuePair<char, string>>) urlReplacementMap.UrlsToFix)
      {
        if (str.Contains<char>(keyValuePair.Key))
          str = str.ReplaceCharWithString(keyValuePair.Key, keyValuePair.Value);
      }
      return (text, str, linkUpdateRecordSet.ToList<WorkItemResourceLinkUpdateRecord>());
    }

    internal virtual TeamFoundationWorkItemService.UrlReplacementMap BuildUrlReplacementMap(
      IVssRequestContext requestContext)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      TeamFoundationWorkItemService.UrlReplacementMap urlReplacementMap = new TeamFoundationWorkItemService.UrlReplacementMap();
      AccessMapping accessMapping = service.DetermineAccessMapping(requestContext);
      string str1 = service.LocationForAccessMapping(requestContext, "WorkitemAttachmentHandler", "WorkItemTracking", accessMapping);
      string str2 = service.LocationForAccessMapping(requestContext, "AttachmentDownload", "TestManagement", accessMapping);
      urlReplacementMap.UrlsToReplace[str1] = '\a';
      urlReplacementMap.UrlsToReplace[str2] = '\b';
      Regex key1 = new Regex(TeamFoundationWorkItemService.GetProjectAttachmentUri(requestContext, new Guid?(Guid.Empty)).AbsoluteUri.Replace(string.Format("{0}", (object) Guid.Empty), "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}"), RegexOptions.IgnoreCase, CommonWorkItemTrackingConstants.RegexMatchTimeout);
      urlReplacementMap.UrlsToReplaceUsingRegex[key1] = '\u0006';
      try
      {
        urlReplacementMap.UrlsToReplace[new Uri(str1).AbsoluteUri] = '\a';
        urlReplacementMap.UrlsToReplace[new Uri(str2).AbsoluteUri] = '\b';
        string key2 = TeamFoundationWorkItemService.MakeUrlSafe(str1);
        if (!string.IsNullOrWhiteSpace(key2))
          urlReplacementMap.UrlsToReplace[key2] = '\a';
        string key3 = TeamFoundationWorkItemService.MakeUrlSafe(str2);
        if (!string.IsNullOrWhiteSpace(key3))
          urlReplacementMap.UrlsToReplace[key3] = '\b';
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (str1.IndexOf(this.m_attachmentHandler, StringComparison.OrdinalIgnoreCase) >= 0)
          {
            if (str1.IndexOf(this.m_witPath, StringComparison.OrdinalIgnoreCase) >= 0)
            {
              if (str1.IndexOf(this.m_witPathWithDefaultCollection, StringComparison.OrdinalIgnoreCase) < 0)
              {
                string str3 = Regex.Replace(str1, this.m_witPath, this.m_witPathWithDefaultCollection, RegexOptions.IgnoreCase);
                urlReplacementMap.UrlsToReplace[str3] = '\a';
                urlReplacementMap.UrlsToReplace[new Uri(str3).AbsoluteUri] = '\a';
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(904308, TraceLevel.Warning, "Services", "WorkItemService", string.Format("Unable to parse WIT or TCM url from access mapping. Wit Url: {0}, Tcm Url: {1}. Message: {2}", (object) str1, (object) str2, (object) ex.Message));
      }
      requestContext.Trace(904308, TraceLevel.Info, "Services", "WorkItemService", string.Join(",", (IEnumerable<string>) urlReplacementMap.UrlsToReplace.Keys));
      urlReplacementMap.UrlsToFix['\a'] = str1;
      urlReplacementMap.UrlsToFix['\b'] = str2;
      if (requestContext.RequestUri() != (Uri) null)
      {
        try
        {
          string absoluteUri = new Uri(requestContext.RequestUri(), new Uri(str1).AbsolutePath).AbsoluteUri;
          urlReplacementMap.UrlsToReplace[absoluteUri] = '\a';
          urlReplacementMap.UrlsToReplace[HttpUtility.UrlDecode(absoluteUri)] = '\a';
          string key4 = TeamFoundationWorkItemService.MakeUrlSafe(absoluteUri);
          if (!string.IsNullOrWhiteSpace(key4))
            urlReplacementMap.UrlsToReplace[key4] = '\a';
        }
        catch (Exception ex)
        {
          requestContext.Trace(904308, TraceLevel.Warning, "Services", "WorkItemService", string.Format("Unable to parse WIT from request url {0}. Message: {1}", (object) str1, (object) ex.Message));
        }
      }
      return urlReplacementMap;
    }

    internal virtual void AddProjectAttachmentUrlsToMap(
      IVssRequestContext requestContext,
      Guid projectGuid,
      TeamFoundationWorkItemService.UrlReplacementMap map)
    {
      Uri projectAttachmentUri = TeamFoundationWorkItemService.GetProjectAttachmentUri(requestContext, new Guid?(projectGuid));
      Uri collectionAttachmentUri = TeamFoundationWorkItemService.GetCollectionAttachmentUri(requestContext);
      if (projectAttachmentUri != (Uri) null)
      {
        map.UrlsToFix['\u0006'] = projectAttachmentUri.AbsoluteUri;
        map.UrlsToReplace[projectAttachmentUri.AbsoluteUri] = '\u0006';
      }
      if (!(collectionAttachmentUri != (Uri) null))
        return;
      map.UrlsToReplace[collectionAttachmentUri.AbsoluteUri] = '\u0006';
    }

    internal static Uri GetProjectAttachmentUri(
      IVssRequestContext requestContext,
      Guid? projectGuid)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      Guid guid = new Guid("E07B5FA4-1499-494D-A496-64B860FD64FF");
      string str = "wit";
      IVssRequestContext requestContext1 = requestContext;
      string serviceType = str;
      Guid identifier = guid;
      var routeValues = new{ project = projectGuid };
      return service.GetResourceUri(requestContext1, serviceType, identifier, (object) routeValues);
    }

    internal static Uri GetCollectionAttachmentUri(IVssRequestContext requestContext)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      Guid guid = new Guid("E07B5FA4-1499-494D-A496-64B860FD64FF");
      string str = "wit";
      IVssRequestContext requestContext1 = requestContext;
      string serviceType = str;
      Guid identifier = guid;
      return service.GetResourceUri(requestContext1, serviceType, identifier, (object) null);
    }

    internal virtual IEnumerable<WorkItemResourceLinkUpdateRecord> ParseLegacyInlineImages(
      int workItemId,
      string text,
      Guid projectGuid,
      Dictionary<Guid, int> existingInlineImages,
      ref int sequenceOrder)
    {
      int length = text.Length;
      int startIndex = 0;
      HashSet<WorkItemResourceLinkUpdateRecord> legacyInlineImages = new HashSet<WorkItemResourceLinkUpdateRecord>((IEqualityComparer<WorkItemResourceLinkUpdateRecord>) InlineImageComparer.Instance);
      int num1;
      for (; startIndex < length && (num1 = text.IndexOf('\a', startIndex)) >= 0 && num1 + TeamFoundationWorkItemService.s_fileGuidSegmentLength + 36 < length; startIndex = num1 + 1)
      {
        Guid result;
        if (text.IndexOf("?FileNameGUID=", num1 + 1, TeamFoundationWorkItemService.s_fileGuidSegmentLength, StringComparison.OrdinalIgnoreCase) == num1 + 1 && Guid.TryParse(text.Substring(num1 + TeamFoundationWorkItemService.s_fileGuidSegmentLength + 1, 36), out result))
        {
          int num2;
          legacyInlineImages.Add(new WorkItemResourceLinkUpdateRecord()
          {
            Order = sequenceOrder++,
            SourceId = workItemId,
            UpdateType = existingInlineImages.TryGetValue(result, out num2) ? LinkUpdateType.Update : LinkUpdateType.Add,
            Location = result.ToString("D"),
            Type = new ResourceLinkType?(ResourceLinkType.InlineImage),
            Name = string.Empty,
            ProjectId = projectGuid,
            ResourceId = new int?(num2)
          });
          num1 += TeamFoundationWorkItemService.s_fileGuidSegmentLength + 36;
        }
      }
      return (IEnumerable<WorkItemResourceLinkUpdateRecord>) legacyInlineImages;
    }

    internal virtual IEnumerable<WorkItemResourceLinkUpdateRecord> ParseProjectInlineImages(
      int workItemId,
      string text,
      Guid projectGuid,
      Dictionary<Guid, int> existingInlineImages,
      ref int sequenceOrder)
    {
      int length = text.Length;
      int startIndex = 0;
      HashSet<WorkItemResourceLinkUpdateRecord> projectInlineImages = new HashSet<WorkItemResourceLinkUpdateRecord>((IEqualityComparer<WorkItemResourceLinkUpdateRecord>) InlineImageComparer.Instance);
      int position;
      for (; startIndex < length && (position = text.IndexOf('\u0006', startIndex)) >= 0; startIndex = position + 1)
      {
        Guid inlineImageId;
        if (this.TryParseProjectScopedInlineImageId(text, length, position, out inlineImageId))
        {
          int num;
          projectInlineImages.Add(new WorkItemResourceLinkUpdateRecord()
          {
            Order = sequenceOrder++,
            SourceId = workItemId,
            UpdateType = existingInlineImages.TryGetValue(inlineImageId, out num) ? LinkUpdateType.Update : LinkUpdateType.Add,
            Location = inlineImageId.ToString("D"),
            Type = new ResourceLinkType?(ResourceLinkType.InlineImage),
            Name = string.Empty,
            ProjectId = projectGuid,
            ResourceId = new int?(num)
          });
          position += TeamFoundationWorkItemService.s_projectAttachmentGuidSeparatorLength + 36;
        }
      }
      return (IEnumerable<WorkItemResourceLinkUpdateRecord>) projectInlineImages;
    }

    private bool TryParseProjectScopedInlineImageId(
      string text,
      int length,
      int position,
      out Guid inlineImageId)
    {
      if (position + TeamFoundationWorkItemService.s_projectAttachmentGuidSeparatorLength + 36 < length && text.IndexOf("/", position + 1, TeamFoundationWorkItemService.s_projectAttachmentGuidSeparatorLength, StringComparison.OrdinalIgnoreCase) == position + 1 && Guid.TryParse(text.Substring(position + TeamFoundationWorkItemService.s_projectAttachmentGuidSeparatorLength + 1, 36), out inlineImageId) || position + TeamFoundationWorkItemService.s_fileGuidSegmentLength + 36 < length && text.IndexOf("?FileNameGUID=", position + 1, TeamFoundationWorkItemService.s_fileGuidSegmentLength, StringComparison.OrdinalIgnoreCase) == position + 1 && Guid.TryParse(text.Substring(position + TeamFoundationWorkItemService.s_fileGuidSegmentLength + 1, 36), out inlineImageId))
        return true;
      inlineImageId = Guid.Empty;
      return false;
    }

    protected virtual int GetTrendDataInterval(IVssRequestContext requestContext) => requestContext.TraceBlock<int>(904621, 904630, 904625, "Services", "WorkItemService", "UpdateWorkItems.GetTrendDataInterval", (Func<int>) (() => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/TrendDataInterval", true, 900)));

    internal static string MakeUrlSafe(string url)
    {
      string str1 = "<a href=\"";
      string str2 = "\"></a>";
      string str3 = SafeHtmlWrapper.MakeSafe(str1 + url + str2);
      return !str3.StartsWith(str1) || !str3.EndsWith(str2) ? string.Empty : str3.Remove(str3.Length - str2.Length, str2.Length).Remove(0, str1.Length);
    }

    internal ResolvedIdentityNamesInfo GetResolvedIdentityNamesInfo(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      ResolvedIdentityNamesInfo resolvedNamesInfo = new ResolvedIdentityNamesInfo();
      requestContext.TraceBlock(904611, 904620, 904615, "Services", "WorkItemService", "UpdateWorkItems.resolvedNamesInfo", (Action) (() => resolvedNamesInfo = requestContext.GetService<IWorkItemIdentityService>().ResolveIdentityFields(witRequestContext, updateStates, ruleExecutionMode == WorkItemUpdateRuleExecutionMode.Bypass)));
      return resolvedNamesInfo;
    }

    internal IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> ResolveReferencedIdentities(
      WorkItemTrackingRequestContext witRequestContext,
      ResolvedIdentityNamesInfo resolvedNamesInfo)
    {
      return witRequestContext.RequestContext.TraceBlock<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>(904571, 904580, 904575, "Services", "WorkItemService", "WorkItemService.ResolveReferencedIdentities", (Func<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>) (() =>
      {
        IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = resolvedNamesInfo.IdentityMap.Value;
        string currentUser = witRequestContext.ServerDefaultValueTransformer.CurrentUser;
        if (!dictionary.ContainsKey(currentUser))
          dictionary[currentUser] = witRequestContext.RequestIdentity;
        return dictionary;
      }));
    }

    private IEnumerable<WorkItemUpdateResult> UpdateWorkItemsInternal(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode = WorkItemUpdateRuleExecutionMode.ValidationOnly,
      bool allOrNothing = false,
      bool validateOnly = false,
      WorkItemUpdateMode updateMode = WorkItemUpdateMode.Normal,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false,
      bool isPermissionCheckRequiredForBypassRules = false,
      bool useWorkItemIdentity = false,
      bool suppressQueueRemoteLinkJob = false,
      bool skipAllNotifications = false,
      bool checkRevisionsLimit = false)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      bool isNoHistoryEnabledFieldsSupported = WorkItemTrackingFeatureFlags.IsNoHistoryEnabledFieldsSupported(witRequestContext.RequestContext);
      WorkItemUpdateDataset updateDataset = (WorkItemUpdateDataset) null;
      WorkItemUpdateResultSet updateResultSet = (WorkItemUpdateResultSet) null;
      Func<WorkItemUpdateState, bool> func1;
      Func<WorkItemUpdateState, bool> func2;
      Func<WorkItemUpdateState, (int, Guid, int)> func3;
      IEnumerable<WorkItemUpdateState> workItemUpdateStates = requestContext.TraceBlock<IEnumerable<WorkItemUpdateState>>(904200, 904399, 904398, "Services", "WorkItemService", "UpdateWorkItems", (Func<IEnumerable<WorkItemUpdateState>>) (() =>
      {
        if (!workItemUpdates.Any<WorkItemUpdate>())
          return Enumerable.Empty<WorkItemUpdateState>();
        bool isAdmin = witRequestContext.IsAdmin();
        this.ConvertUpdateFieldsToRequiredFormat(witRequestContext, workItemUpdates);
        this.CheckForDuplicateFieldUpdates(workItemUpdates);
        IEnumerable<WorkItemUpdateState> updateStates = this.CreateUpdateStatesAndValidate(witRequestContext, (IEnumerable<WorkItemUpdate>) workItemUpdates.ToList<WorkItemUpdate>(), ruleExecutionMode, updateMode, isNoHistoryEnabledFieldsSupported);
        Func<bool> func = (Func<bool>) (() => !allOrNothing || !updateStates.Any<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => !us.Success)));
        if (!func())
          return updateStates;
        Dictionary<int, WorkItemUpdateState> dictionary1 = updateStates.ToDictionary<WorkItemUpdateState, int>((Func<WorkItemUpdateState, int>) (us => us.Id));
        bool checkForSoftDeletedProjects = false;
        if (updateStates.Any<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (u => u.HasLinkUpdates)))
          checkForSoftDeletedProjects = true;
        Dictionary<int, WorkItemFieldData> dictionary2 = this.ReadWorkItemTipFieldValues(witRequestContext, updateStates, ruleExecutionMode, useWorkItemIdentity, checkForSoftDeletedProjects);
        this.CheckEmptyAliases(requestContext, updateStates);
        TeamFoundationWorkItemService.CheckAndRevertChangedByFieldIfNeeded(updateStates, isNoHistoryEnabledFieldsSupported);
        ResolvedIdentityNamesInfo identityNamesInfo = this.GetResolvedIdentityNamesInfo(witRequestContext, updateStates, ruleExecutionMode);
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, WorkItemIdentityTelemetry.Feature, (object) WorkItemIdentityTelemetrySource.Save, (object) identityNamesInfo);
        if (!func())
          return updateStates;
        IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap = this.ResolveReferencedIdentities(witRequestContext, identityNamesInfo);
        this.UpdateValuesAndExecuteRules(witRequestContext, updateStates, ruleExecutionMode, identityMap, identityNamesInfo, allOrNothing, updateMode);
        if (!func())
          return updateStates;
        TeamFoundationWorkItemService.CheckAndRevertUnchangedFields(requestContext, updateStates);
        this.ValidateChangedByIfBypassRules(witRequestContext, updateStates, ruleExecutionMode);
        if (!func())
          return updateStates;
        this.ValidateWorkItemChangedDate((IDictionary<int, WorkItemUpdateState>) dictionary1, (IDictionary<int, WorkItemFieldData>) dictionary2);
        if (!func())
          return updateStates;
        this.CheckWorkItemUpdates(witRequestContext, updateStates, updateMode);
        if (!func())
          return updateStates;
        this.ValidateTeamProjectChanges(witRequestContext, updateStates);
        if (!func())
          return updateStates;
        this.CheckUpdatePermissions(witRequestContext, dictionary1, dictionary2, updateMode);
        if (!func())
          return updateStates;
        if (WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsScanningEnabled(requestContext) && !WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsBypassScanningEnabled(requestContext))
        {
          this.ValidateWorkItemSecretScan(witRequestContext.RequestContext, (IDictionary<int, WorkItemUpdateState>) dictionary1);
          if (!func())
            return updateStates;
        }
        if (ruleExecutionMode == WorkItemUpdateRuleExecutionMode.Bypass & isPermissionCheckRequiredForBypassRules)
        {
          this.CheckBypassPermissions(witRequestContext, updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)));
          if (!func())
            return updateStates;
        }
        if (suppressNotifications | skipAllNotifications)
        {
          this.CheckSuppressNotificationsPermission(witRequestContext, updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)));
          if (!func())
            return updateStates;
        }
        this.ProcessAndValidateTagUpdates(witRequestContext, updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)));
        WorkItemUpdateState[] array = updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)).ToArray<WorkItemUpdateState>();
        if (!((IEnumerable<WorkItemUpdateState>) array).Any<WorkItemUpdateState>() || !func())
          return updateStates;
        foreach (WorkItemUpdateState workItemUpdateState in ((IEnumerable<WorkItemUpdateState>) array).Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => !us.HasTagUpdates && us.HasTagsFieldChanges)))
          workItemUpdateState.RemoveFieldUpdate(80);
        updateDataset = this.PrepareWorkItemUpdateDataset(witRequestContext, (IEnumerable<WorkItemUpdateState>) array, dictionary2, ruleExecutionMode, checkRevisionsLimit);
        if (!func())
          return updateStates;
        if (!validateOnly)
        {
          bool flag = updateDataset.IsEmpty();
          if (!flag)
          {
            int trendDataInterval = this.GetTrendDataInterval(requestContext);
            updateResultSet = this.Update(requestContext, ruleExecutionMode, isAdmin, trendDataInterval, updateDataset, updateMode == WorkItemUpdateMode.Restore);
            requestContext.AddSqlTimings("prc_UpdateWorkItems", updateResultSet.PerformanceTimings);
            this.ApplyUpdateResultDatasetOnUpdateStates(witRequestContext, updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)), dictionary1, updateResultSet, updateDataset, useWorkItemIdentity);
          }
          this.PersistTagUpdates(witRequestContext, updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)), updateResultSet);
          this.PersistExternalArtifactWatermarks(requestContext, updateDataset);
          if (!flag)
          {
            try
            {
              this.UpdateIdentityMru(witRequestContext, identityNamesInfo, updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)));
            }
            catch (Exception ex)
            {
              TeamFoundationEventLog.Default.LogException(requestContext, ex.Message, ex, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
            }
          }
          IEnumerable<WorkItemUpdateState> updateStatesToNotify = flag ? Enumerable.Empty<WorkItemUpdateState>() : (IEnumerable<WorkItemUpdateState>) updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success && !us.IsEmpty())).ToList<WorkItemUpdateState>();
          if (!skipAllNotifications)
            this.TryFireWorkItemChangedEvents(witRequestContext, (IDictionary<int, WorkItemFieldData>) dictionary2, identityMap, updateStatesToNotify, suppressNotifications);
          requestContext.TraceBlock(904898, 904899, "Services", "WorkItemService", "UpdateWorkItems.includeInRecentActivity", (Action) (() =>
          {
            if (!includeInRecentActivity || updateMode != WorkItemUpdateMode.Normal && updateMode != WorkItemUpdateMode.Restore)
              return;
            HashSet<int> workItemIdsFilter = (HashSet<int>) null;
            if (workItemIdsToIncludeInRecentActivity != null)
              workItemIdsFilter = new HashSet<int>((IEnumerable<int>) workItemIdsToIncludeInRecentActivity);
            IEnumerable<WorkItemUpdateState> list3 = (IEnumerable<WorkItemUpdateState>) updateStatesToNotify.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => workItemIdsFilter == null || workItemIdsFilter.Contains(us.FieldData.Id))).ToList<WorkItemUpdateState>();
            if (!list3.Any<WorkItemUpdateState>())
              return;
            WorkItemUpdateState workItemUpdateState3 = list3.FirstOrDefault<WorkItemUpdateState>(func1 ?? (func1 = (Func<WorkItemUpdateState, bool>) (updateState =>
            {
              object fieldValue = updateState.FieldData.GetFieldValue(witRequestContext, -4);
              return fieldValue != null && !(fieldValue is ServerDefaultFieldValue);
            })));
            if (workItemUpdateState3 == null)
              return;
            DateTime modifiedDate = workItemUpdateState3.FieldData.ModifiedDate;
            WorkItemUpdateState workItemUpdateState4 = list3.FirstOrDefault<WorkItemUpdateState>(func2 ?? (func2 = (Func<WorkItemUpdateState, bool>) (updateState =>
            {
              object fieldValue = updateState.FieldData.GetFieldValue(witRequestContext, 9);
              return fieldValue != null && !(fieldValue is ServerDefaultFieldValue);
            })));
            Guid id = witRequestContext.RequestIdentity.Id;
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (workItemUpdateState4 != null && identityMap.TryGetValue(workItemUpdateState4.FieldData.ModifiedBy, out identity))
              id = identity.Id;
            WorkItemRecentActivityType recentActivityType = WorkItemRecentActivityType.Edited;
            List<(int, Guid, int)> list4 = list3.Distinct<WorkItemUpdateState>((IEqualityComparer<WorkItemUpdateState>) new WorkItemUpdateStateIdComparer()).Select<WorkItemUpdateState, (int, Guid, int)>(func3 ?? (func3 = (Func<WorkItemUpdateState, (int, Guid, int)>) (us => (us.FieldData.Id, us.FieldData.GetProjectGuid(requestContext), us.FieldData.AreaId)))).ToList<(int, Guid, int)>();
            requestContext.GetService<WorkItemRecentActivityService>().TryFireWorkItemRecentActivityEvent(requestContext, recentActivityType, (IReadOnlyCollection<(int, Guid, int)>) list4, id, modifiedDate);
          }));
          this.TransformIdentityFieldValuesForLegacyClients(witRequestContext, updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)));
        }
        else
        {
          requestContext.TraceBlock(904631, 904640, 904635, "Services", "WorkItemService", "UpdateWorkItems.ValidatePendingSetMembershipChecks", (Action) (() =>
          {
            using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
              updateResultSet = component.ValidatePendingSetMembershipChecks((IEnumerable<PendingSetMembershipCheckRecord>) updateDataset.PendingSetMembershipChecks);
          }));
          this.ApplyUpdateStateAndResultSetOnUpdateResult(witRequestContext, updateResultSet, updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)), dictionary1, useWorkItemIdentity);
        }
        return updateStates;
      }));
      if (requestContext.IsFeatureEnabled("WorkItemTracking.Server.MoveWorkItems") && this.IsAttemptedMove(workItemUpdateStates))
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, WorkItemMoveTelemetry.Feature, (object) witRequestContext, (object) workItemUpdateStates);
      else if (requestContext.IsFeatureEnabled("WorkItemTracking.Server.ChangeWorkItemType") && workItemUpdateStates.Any<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (res => res.FieldData != null && res.HasWorkItemTypeChanged)))
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, WorkItemChangeTypeTelemetry.Feature, (object) witRequestContext, (object) workItemUpdateStates);
      else if (workItemUpdateStates.Any<WorkItemUpdateState>() && workItemUpdateStates.All<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (r => r.Success)))
      {
        List<WorkItemUpdateState> list = workItemUpdateStates.ToList<WorkItemUpdateState>();
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, ResourceLinkingTelemetry.Feature, (object) list);
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, WorkItemUpdateTelemetry.Feature, (object) witRequestContext, (object) list);
        WorkItemKpiTracer.TraceKpi(requestContext, (WorkItemTrackingKpi) new UpdateWorkItemKpi(requestContext, list.Count), (WorkItemTrackingKpi) new UpdateWorkItemKpi(requestContext, "UpdateWorkItemNoLongText", FieldStorageTarget.LongTexts, (IEnumerable<WorkItemUpdateState>) list), (WorkItemTrackingKpi) new UpdateWorkItemKpi(requestContext, "UpdateWorkItemNoLongTable", FieldStorageTarget.LongTable, (IEnumerable<WorkItemUpdateState>) list), (WorkItemTrackingKpi) new UpdateWorkItemKpi(requestContext, (IEnumerable<WorkItemUpdateState>) list), (WorkItemTrackingKpi) new UpdateWorkItemKpi(requestContext, list.Count, true), (WorkItemTrackingKpi) new CountKpi(requestContext, "UpdateWorkItemCount", list.Count));
      }
      foreach (WorkItemUpdate workItemUpdate in workItemUpdates)
        this.UpdateWorkItemDependencyViolations(requestContext, workItemUpdate);
      TeamFoundationWorkItemService.ReportCodeReviewWorkItemUpdates(requestContext, workItemUpdateStates);
      IEnumerable<WorkItemUpdateResult> itemUpdateResults = workItemUpdateStates.Select<WorkItemUpdateState, WorkItemUpdateResult>((Func<WorkItemUpdateState, WorkItemUpdateResult>) (r => r.UpdateResult));
      this.SetIsCommentingAvailable(witRequestContext, requestContext, workItemUpdateStates, itemUpdateResults);
      try
      {
        bool flag = itemUpdateResults.SelectMany<WorkItemUpdateResult, WorkItemLinkUpdateResult>((Func<WorkItemUpdateResult, IEnumerable<WorkItemLinkUpdateResult>>) (x => (IEnumerable<WorkItemLinkUpdateResult>) x.LinkUpdates)).Any<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (x =>
        {
          if (!x.RemoteHostId.HasValue)
            return false;
          Guid? remoteHostId = x.RemoteHostId;
          Guid empty = Guid.Empty;
          if (!remoteHostId.HasValue)
            return true;
          return remoteHostId.HasValue && remoteHostId.GetValueOrDefault() != empty;
        }));
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        if (!suppressQueueRemoteLinkJob & flag)
          service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            WorkItemTrackingJobs.RemoteLinking
          }, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(904931, "Services", "WorkItemService", ex);
      }
      return itemUpdateResults;
    }

    private void SetIsCommentingAvailable(
      WorkItemTrackingRequestContext witRequestContext,
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdateState> result,
      IEnumerable<WorkItemUpdateResult> workItemUpdateResults)
    {
      foreach (WorkItemUpdateState workItemUpdateState1 in result)
      {
        WorkItemUpdateState workItemUpdateState = workItemUpdateState1;
        int? fieldValue = workItemUpdateState?.FieldData?.GetFieldValue<int>(witRequestContext, -2);
        if (fieldValue.HasValue)
          workItemUpdateResults.Single<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (dv => dv.Id == workItemUpdateState.UpdateResult.Id)).IsCommentingAvailable = new bool?(witRequestContext.WorkItemPermissionChecker.HasWorkItemPermission(fieldValue.Value, 512, 32));
      }
    }

    private static void ReportCodeReviewWorkItemUpdates(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdateState> result)
    {
      try
      {
        Dictionary<Guid, HashSet<string>> dictionary = new Dictionary<Guid, HashSet<string>>();
        IWorkItemTypeCategoryService service = requestContext.GetService<IWorkItemTypeCategoryService>();
        foreach (WorkItemUpdateState workItemUpdateState in result)
        {
          if (workItemUpdateState.FieldData != null)
          {
            Guid projectGuid = workItemUpdateState.FieldData.GetProjectGuid(requestContext);
            string workItemType = workItemUpdateState.FieldData.WorkItemType;
            if (!dictionary.ContainsKey(projectGuid))
            {
              HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
              WorkItemTypeCategory workItemTypeCategory1;
              if (service.TryGetWorkItemTypeCategory(requestContext, projectGuid, "Microsoft.CodeReviewRequestCategory", out workItemTypeCategory1))
                stringSet.UnionWith(workItemTypeCategory1.WorkItemTypeNames);
              WorkItemTypeCategory workItemTypeCategory2;
              if (service.TryGetWorkItemTypeCategory(requestContext, projectGuid, "Microsoft.CodeReviewResponseCategory", out workItemTypeCategory2))
                stringSet.UnionWith(workItemTypeCategory2.WorkItemTypeNames);
              dictionary.Add(projectGuid, stringSet);
            }
            if (dictionary[projectGuid].Contains(workItemType))
              WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, CodeReviewTelemetry.Feature, (object) workItemUpdateState.UpdateResult.Id.ToString(), (object) workItemUpdateState.FieldData.WorkItemType, (object) workItemUpdateState.Update.IsNew);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(904935, "Services", "WorkItemService", ex);
      }
    }

    private void CheckEmptyAliases(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      HashSet<string> emptyAliases = requestContext.WitContext().ServerSettings.EmptyAliases;
      IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
      foreach (WorkItemUpdateState updateState in updateStates)
      {
        if (updateState.Update?.Fields != null)
        {
          foreach (KeyValuePair<string, object> field1 in updateState.Update.Fields)
          {
            FieldEntry field2 = fieldDictionary.GetField(field1.Key);
            if (field2.FieldType == InternalFieldType.History || field2.FieldType == InternalFieldType.Html)
            {
              object text = field1.Value;
              if (text != null && text is WorkItemCommentUpdate itemCommentUpdate)
                text = (object) itemCommentUpdate.Text;
              if (!(text is string) && text != null)
              {
                string message = field2.FieldType != InternalFieldType.History ? Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValueContainsInvalidHTML(text) : Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidHistoryFieldValue(text);
                throw new WorkItemFieldInvalidException(updateState.FieldData.Id, message);
              }
              if (emptyAliases.Contains((string) text))
              {
                updateState.RemoveFieldUpdate(field2.FieldId);
                updateState.AddFieldUpdate(field2.FieldId, (object) "");
              }
            }
          }
        }
      }
    }

    private void ReplaceWithWorkItemIdentity(
      IVssRequestContext requestContext,
      WorkItemUpdateState updateState,
      int areaId,
      bool emailReadable)
    {
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      if (updateState.FieldData == null)
        return;
      WorkItemUpdateResult updateResult = updateState.UpdateResult;
      string token;
      emailReadable &= this.TryGetSecurityToken(requestContext, new Guid?(), areaId, out token);
      updateResult.Fields.ToList<KeyValuePair<string, object>>().ForEach((Action<KeyValuePair<string, object>>) (pair =>
      {
        FieldEntry field;
        if (!witRequestContext.FieldDictionary.TryGetFieldByNameOrId(pair.Key, out field) || !field.IsIdentity || !(pair.Value is string distinctDisplayName2))
          return;
        WorkItemIdentity distinctDisplayName3 = WorkItemIdentityHelper.GetResolvedIdentityFromDistinctDisplayName(requestContext, distinctDisplayName2);
        if (distinctDisplayName3 != null)
        {
          distinctDisplayName3.HasPermission = emailReadable;
          distinctDisplayName3.SecurityToken = token;
          updateResult.Fields[pair.Key] = (object) distinctDisplayName3;
        }
        else
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("NotResolvedWorkitem", (double) updateState.Id);
          IVssRequestContext requestContext1 = requestContext;
          string workItemTracking = CustomerIntelligenceArea.WorkItemTracking;
          CustomerIntelligenceData properties = intelligenceData;
          service.Publish(requestContext1, workItemTracking, nameof (ReplaceWithWorkItemIdentity), properties);
          if (!(!string.IsNullOrEmpty(distinctDisplayName2) & emailReadable))
            throw new WorkItemIdentityNotResolvedException();
          WorkItemIdentity workItemIdentity = new WorkItemIdentity()
          {
            DistinctDisplayName = distinctDisplayName2,
            IdentityRef = (IdentityRef) new ConstantIdentityRef(token, 16, Microsoft.Azure.Boards.CssNodes.AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid)
            {
              Descriptor = new SubjectDescriptor(),
              DisplayName = distinctDisplayName2
            }
          };
        }
      }));
    }

    private void ValidateChangedByIfBypassRules(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode)
    {
      if (ruleExecutionMode != WorkItemUpdateRuleExecutionMode.Bypass)
        return;
      foreach (WorkItemUpdateState updateState in updateStates)
      {
        if (updateState.FieldData != null && updateState.HasFieldUpdate(9) && string.IsNullOrWhiteSpace(updateState.FieldData.GetFieldValue<string>(witRequestContext, 9)))
          updateState.UpdateResult.AddException(new TeamFoundationServiceException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ChangedByEmptyWithBypassRulesException()));
      }
    }

    private void CheckWorkItemUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      WorkItemUpdateMode updateMode)
    {
      witRequestContext.RequestContext.TraceBlock(904902, 904903, "Services", "WorkItemService", "UpdateWorkItems.CheckWorkItemUpdates", (Action) (() =>
      {
        foreach (WorkItemUpdateState updateState in updateStates)
        {
          if (updateState.FieldData != null)
          {
            this.CheckWorkItemTypeUpdate(witRequestContext, updateState);
            if (!witRequestContext.RequestContext.IsFeatureEnabled("WorkItemTracking.Server.WorkItem.AllowInvalidLinkArtifacts"))
              this.CheckResourceLinkUpdates(witRequestContext, updateState);
            this.CheckClassificationNode<int>(witRequestContext, updateState, -104);
            this.CheckClassificationNode<int>(witRequestContext, updateState, -2);
            this.CheckClassificationNode<string>(witRequestContext, updateState, -105);
            this.CheckClassificationNode<string>(witRequestContext, updateState, -7);
            this.CheckIsDeletedFieldValue(witRequestContext, updateState, updateMode);
            if (WorkItemTrackingFeatureFlags.IsFieldsExistInWITCheckEnabled(witRequestContext.RequestContext) && updateState.FieldUpdates.Count<KeyValuePair<int, object>>() > 0)
              this.CheckFieldsExistInWIT(witRequestContext, updateState);
          }
          if (updateState.HasResourceLinkUpdates)
            this.CheckResourceLinkChanges(witRequestContext, updateState);
        }
      }));
    }

    private void CheckFieldsExistInWIT(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      HashSet<string> stringSet = new HashSet<string>();
      Guid projectGuid = updateState.FieldData.GetProjectGuid(requestContext);
      WorkItemType typeByReferenceName = witRequestContext.RequestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(requestContext, projectGuid, updateState.FieldData.WorkItemType);
      IReadOnlyCollection<int> fieldIds = typeByReferenceName.GetFieldIds(requestContext);
      foreach (KeyValuePair<int, object> fieldUpdate in updateState.FieldUpdates)
      {
        FieldEntry field;
        if ((!witRequestContext.FieldDictionary.TryGetField(fieldUpdate.Key, out field) || (field.Usage & InternalFieldUsages.WorkItemTypeExtension) != InternalFieldUsages.WorkItemTypeExtension) && !fieldIds.Contains<int>(fieldUpdate.Key))
          stringSet.Add(witRequestContext.FieldDictionary.GetFieldByNameOrId(fieldUpdate.Key.ToString()).ReferenceName);
      }
      if (!stringSet.Any<string>())
        return;
      updateState.UpdateResult.AddException((TeamFoundationServiceException) new FieldsNotExistInWitException(string.Join(", ", (IEnumerable<string>) stringSet), typeByReferenceName.Name));
    }

    private void CheckResourceLinkChanges(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState)
    {
      this.CheckWorkItemAttachmentsLimit(witRequestContext, updateState);
      this.CheckInvalidHyperlinkProtocol(witRequestContext, updateState);
    }

    private void CheckInvalidHyperlinkProtocol(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState)
    {
      foreach (WorkItemResourceLinkUpdate resourceLinkUpdate in updateState.Update?.ResourceLinkUpdates ?? (IEnumerable<WorkItemResourceLinkUpdate>) new List<WorkItemResourceLinkUpdate>())
      {
        ResourceLinkType? type = resourceLinkUpdate.Type;
        if (type.HasValue)
        {
          type = resourceLinkUpdate.Type;
          ResourceLinkType resourceLinkType = ResourceLinkType.Hyperlink;
          if (type.GetValueOrDefault() == resourceLinkType & type.HasValue && (resourceLinkUpdate.UpdateType == LinkUpdateType.Add || resourceLinkUpdate.UpdateType == LinkUpdateType.Update))
          {
            if (string.IsNullOrWhiteSpace(resourceLinkUpdate.Location))
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new ResourceLinkTargetUnspecifiedException(updateState.Id));
            else if (!TFCommonUtil.IsSafeUrlProtocol(resourceLinkUpdate.Location))
            {
              witRequestContext.RequestContext.Trace(910516, TraceLevel.Warning, "Services", "WorkItemService", resourceLinkUpdate.Location);
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemUnsafeResourceLinkLocationException(resourceLinkUpdate.Location, string.Join(", ", (IEnumerable<string>) UrlConstants.SafeUriSchemesSet)));
            }
          }
        }
      }
    }

    private void CheckWorkItemAttachmentsLimit(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState)
    {
      IEnumerable<WorkItemResourceLinkUpdate> source = updateState.Update.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (resLink =>
      {
        ResourceLinkType? type = resLink.Type;
        ResourceLinkType resourceLinkType = ResourceLinkType.Attachment;
        return type.GetValueOrDefault() == resourceLinkType & type.HasValue;
      }));
      if (!source.Any<WorkItemResourceLinkUpdate>())
        return;
      int num1 = source.Count<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (change => change.UpdateType == LinkUpdateType.Add));
      if (num1 <= 0)
        return;
      int num2 = source.Count<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (change => change.UpdateType == LinkUpdateType.Delete));
      int num3 = updateState.ResourceLinks.Count<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (resLink => resLink.ResourceType == ResourceLinkType.Attachment));
      int num4 = num3 + num1 - num2;
      int workItemAttachments = witRequestContext.ServerSettings.MaxAllowedWorkItemAttachments;
      int num5 = workItemAttachments;
      if (num4 <= num5)
        return;
      WorkItemMaxAllowedAttachmentsLimitExceededException exception = new WorkItemMaxAllowedAttachmentsLimitExceededException(workItemAttachments);
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      witRequestContext.RequestContext.GetService<CustomerIntelligenceService>().Publish(witRequestContext.RequestContext, "WorkItemService", WorkItemUpdateTelemetry.Feature, new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "WorkItemId",
          (object) updateState.Id
        },
        {
          "Rev",
          (object) updateState.Update.Rev
        },
        {
          "ExceptionType",
          (object) exception.GetType().Name
        },
        {
          "ExceptionMessage",
          (object) exception.Message
        },
        {
          "NumOfAttachmentsExisted",
          (object) num3
        },
        {
          "NumOfAttachmentsToAdd",
          (object) num1
        },
        {
          "NumOfAttachmentsToDelete",
          (object) num2
        }
      }));
      updateState.UpdateResult.AddException((TeamFoundationServiceException) exception);
    }

    private bool TryGetTreeNode<TNode>(
      WorkItemTrackingRequestContext witRequestContext,
      TNode identifier,
      out TreeNode treeNode,
      TreeStructureType treeStructureType = TreeStructureType.None)
    {
      if (typeof (TNode) == typeof (int))
        return witRequestContext.TreeService.LegacyTryGetTreeNode(Convert.ToInt32((object) identifier), out treeNode);
      if (typeof (TNode) == typeof (Guid))
        return witRequestContext.TreeService.LegacyTryGetTreeNode(new Guid(Convert.ToString((object) identifier)), out treeNode);
      if (typeof (TNode) == typeof (string))
        return witRequestContext.TreeService.TryGetNodeFromPath(witRequestContext.RequestContext, Convert.ToString((object) identifier), treeStructureType, out treeNode);
      throw new InvalidOperationException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidTreeNodeIdentifier());
    }

    private bool TryCheckTargetWorkItemTypeValid(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState udpateState,
      HashSet<string> notSupportedTargetTypes,
      out string targetWorkItemTypeName)
    {
      targetWorkItemTypeName = string.Empty;
      string fieldName = witRequestContext.FieldDictionary.GetField(25).ReferenceName;
      IEnumerable<KeyValuePair<string, object>> fields = udpateState.Update.Fields;
      object obj = fields != null ? fields.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => TFStringComparer.WorkItemFieldReferenceName.Equals(kvp.Key, fieldName))).Select<KeyValuePair<string, object>, object>((Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value)).FirstOrDefault<object>() : (object) null;
      if (obj != null)
        targetWorkItemTypeName = obj.ToString();
      return !notSupportedTargetTypes.Contains(targetWorkItemTypeName);
    }

    private void ExecuteWorkItemRules(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState,
      RuleEngineConfiguration ruleConfiguration,
      RuleEngine ruleEngine,
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      ResolvedIdentityNamesInfo resolvedNamesInfo)
    {
      if (ruleConfiguration == null)
        throw new ArgumentNullException(nameof (ruleConfiguration));
      witRequestContext.RequestContext.TraceBlock(904240, 904249, 904248, "Services", "WorkItemService", "UpdateWorkItems.ExecuteRules", (Action) (() =>
      {
        try
        {
          updateState.RuleEvalContext.UpdateIdentityMap(identityMap);
          updateState.RuleEvalContext.SetResolvedIdentityNamesInfo(resolvedNamesInfo);
          this.Evaluate(updateState, ruleEngine);
        }
        catch (TeamFoundationServiceException ex)
        {
          updateState.UpdateResult.AddException(ex);
        }
      }));
    }

    private void ExecuteExtensionRules(
      WorkItemTrackingRequestContext witRequestContext,
      IWorkItemTypeExtensionsMatcher extensionMatcher,
      WorkItemUpdateState updateState,
      WorkItemUpdateMode updateMode,
      IEnumerable<WorkItemFieldRule> witRules)
    {
      this.PrepareOldExtensionFields(witRequestContext, extensionMatcher, updateState, updateMode);
      this.PrepareCurrentExtensionFields(witRequestContext, extensionMatcher, updateState, updateMode);
      IRuleEvaluationContext ruleEvalContext = updateState.RuleEvalContext;
      WorkItemTypeExtension[] array1 = ((IEnumerable<WorkItemTypeExtension>) updateState.OldExtensions).Except<WorkItemTypeExtension>((IEnumerable<WorkItemTypeExtension>) updateState.CurrentExtensions).ToArray<WorkItemTypeExtension>();
      if (((IEnumerable<WorkItemTypeExtension>) array1).Any<WorkItemTypeExtension>())
        ExtensionParentFieldCopyUtil.UpdateDetachedExtensions(witRequestContext, (IEnumerable<WorkItemTypeExtension>) array1, updateState);
      if (!((IEnumerable<WorkItemTypeExtension>) updateState.CurrentExtensions).Any<WorkItemTypeExtension>())
        return;
      WorkItemFieldRule[] array2 = ((IEnumerable<WorkItemTypeExtension>) updateState.CurrentExtensions).SelectMany<WorkItemTypeExtension, WorkItemFieldRule>((Func<WorkItemTypeExtension, IEnumerable<WorkItemFieldRule>>) (e => e.ExecutableRules)).ToArray<WorkItemFieldRule>();
      if (WorkItemTrackingFeatureFlags.IsExecuteExtensionRulesFixEnabled(witRequestContext.RequestContext) && witRules != null)
        array2 = ((IEnumerable<WorkItemFieldRule>) array2).GroupJoin(witRules, (Func<WorkItemFieldRule, int>) (fr => fr.FieldId), (Func<WorkItemFieldRule, int>) (fr => fr.FieldId), (extension, rule) => new
        {
          Extension = extension,
          Rule = rule
        }).SelectMany(p => p.Rule.DefaultIfEmpty<WorkItemFieldRule>(), (p, rule) => new
        {
          Extension = p.Extension,
          Rule = rule
        }).Select(pair =>
        {
          WorkItemFieldRule workItemFieldRule1 = (WorkItemFieldRule) pair.Extension.Clone(false);
          IEnumerable<ProhibitedValuesRule> prohibitedValuesRules = pair.Rule?.SelectRules<ProhibitedValuesRule>();
          if (prohibitedValuesRules != null && prohibitedValuesRules.Any<ProhibitedValuesRule>())
          {
            WorkItemFieldRule workItemFieldRule2 = (WorkItemFieldRule) pair.Rule?.Clone(false);
            workItemFieldRule2.RemoveRulesByObjects(workItemFieldRule2 != null ? (ISet<WorkItemRule>) workItemFieldRule2.SelectRules<WorkItemRule>().Where<WorkItemRule>((Func<WorkItemRule, bool>) (r => !(r is RuleBlock) && !((IEnumerable<WorkItemRule>) prohibitedValuesRules).Contains<WorkItemRule>(r))).ToHashSet<WorkItemRule>() : (ISet<WorkItemRule>) null);
            foreach (WorkItemRule subRule in workItemFieldRule2.SubRules)
              workItemFieldRule1.AddRule<WorkItemRule>(subRule);
          }
          return workItemFieldRule1;
        }).ToArray<WorkItemFieldRule>();
      new RuleEngine((IEnumerable<WorkItemFieldRule>) array2, RuleEngineConfiguration.ServerFullNoInverse).Evaluate(ruleEvalContext);
      ExtensionParentFieldCopyUtil.UpdateCurrentExtensions(witRequestContext, (IEnumerable<WorkItemTypeExtension>) updateState.CurrentExtensions, updateState);
    }

    private void PrepareCurrentExtensionFields(
      WorkItemTrackingRequestContext witRequestContext,
      IWorkItemTypeExtensionsMatcher extensionMatcher,
      WorkItemUpdateState updateState,
      WorkItemUpdateMode updateMode = WorkItemUpdateMode.Normal)
    {
      updateState.CurrentExtensions = updateMode != WorkItemUpdateMode.Delete ? updateState.FieldData.GetApplicableExtensions(witRequestContext, extensionMatcher) : Array.Empty<WorkItemTypeExtension>();
      foreach (WorkItemTypeExtension currentExtension in updateState.CurrentExtensions)
        updateState.FieldData.SetFieldValue(witRequestContext, currentExtension.MarkerField.Field, (object) true);
    }

    private void PrepareOldExtensionFields(
      WorkItemTrackingRequestContext witRequestContext,
      IWorkItemTypeExtensionsMatcher extensionMatcher,
      WorkItemUpdateState updateState,
      WorkItemUpdateMode updateMode)
    {
      updateState.OldExtensions = updateState.Update.IsNew || updateMode == WorkItemUpdateMode.Restore ? Array.Empty<WorkItemTypeExtension>() : updateState.FieldData.GetActiveExtensions(witRequestContext, extensionMatcher);
      foreach (WorkItemTypeExtension oldExtension in updateState.OldExtensions)
        updateState.FieldData.SetFieldValue(witRequestContext, oldExtension.MarkerField.Field, (object) false);
    }

    private void CheckWorkItemWritePermissions(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      WorkItemUpdateMode updateMode = WorkItemUpdateMode.Normal)
    {
      witRequestContext.RequestContext.TraceBlock(904230, 904239, 904238, "Services", "WorkItemService", "UpdateWorkItems.CheckWritePermissions", (Action) (() =>
      {
        AccessType accessType = this.GetAccessType(updateMode);
        foreach (WorkItemUpdateState updateState in updateStates)
        {
          if (!this.HasWorkItemWritePermission(witRequestContext, updateState, updateMode))
            updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemUnauthorizedAccessException(updateState.Id, accessType));
          else if (updateMode == WorkItemUpdateMode.Normal && updateState.HasFieldUpdates && updateState.FieldUpdates.Any<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (fu => fu.Key == 54)) && !this.HasNecessaryPermission(512, witRequestContext, updateState, updateMode))
            updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemUnauthorizedCommentsAccessException());
        }
      }));
    }

    private AccessType GetAccessType(WorkItemUpdateMode updateMode)
    {
      if (updateMode == WorkItemUpdateMode.Delete)
        return AccessType.SoftDelete;
      return updateMode == WorkItemUpdateMode.Restore ? AccessType.Restore : AccessType.Write;
    }

    private bool HasNecessaryPermission(
      int permissionToCheck,
      Func<int, int, bool> permissionCheckLogic,
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState,
      WorkItemUpdateMode updateMode = WorkItemUpdateMode.Normal)
    {
      bool flag = updateMode != 0;
      int fieldValue1 = updateState.FieldData.GetFieldValue<int>(witRequestContext, -2);
      if (!permissionCheckLogic(fieldValue1, permissionToCheck) || flag && !witRequestContext.WorkItemProjectPermissionChecker.HasWorkItemPermission(fieldValue1, AuthorizationProjectPermissions.WorkItemSoftDelete))
        return false;
      if (!updateState.Update.IsNew)
      {
        int fieldValue2 = updateState.FieldData.GetFieldValue<int>(witRequestContext, -2, true);
        if (fieldValue2 != fieldValue1 && (!permissionCheckLogic(fieldValue2, permissionToCheck) || flag && !witRequestContext.WorkItemProjectPermissionChecker.HasWorkItemPermission(fieldValue2, AuthorizationProjectPermissions.WorkItemSoftDelete)))
          return false;
      }
      return true;
    }

    private bool HasNecessaryPermission(
      int permissionToCheck1,
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState,
      WorkItemUpdateMode updateMode = WorkItemUpdateMode.Normal)
    {
      return this.HasNecessaryPermission(permissionToCheck1, (Func<int, int, bool>) ((areaId, permissionToCheck2) =>
      {
        bool? itemPermissionState = witRequestContext.WorkItemPermissionChecker.GetWorkItemPermissionState(areaId, permissionToCheck2);
        return !itemPermissionState.HasValue || itemPermissionState.Value;
      }), witRequestContext, updateState, updateMode);
    }

    private bool HasWorkItemWritePermission(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateState updateState,
      WorkItemUpdateMode updateMode = WorkItemUpdateMode.Normal)
    {
      return this.HasNecessaryPermission(32, (Func<int, int, bool>) ((areaId, permissionToCheck) => witRequestContext.WorkItemPermissionChecker.HasWorkItemPermission(areaId, permissionToCheck)), witRequestContext, updateState, updateMode);
    }

    private void CheckBypassPermissions(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      foreach (WorkItemUpdateState updateState in updateStates)
      {
        int fieldValue = updateState.FieldData.GetFieldValue<int>(witRequestContext, -2);
        if (!witRequestContext.WorkItemProjectPermissionChecker.HasWorkItemPermission(fieldValue, AuthorizationProjectPermissions.BypassRules))
          updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemUnauthorizedRuleBypassException());
      }
    }

    private void CheckSuppressNotificationsPermission(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      foreach (WorkItemUpdateState updateState in updateStates)
      {
        int fieldValue = updateState.FieldData.GetFieldValue<int>(witRequestContext, -2);
        if (!witRequestContext.WorkItemProjectPermissionChecker.HasWorkItemPermission(fieldValue, AuthorizationProjectPermissions.SuppressNotifications))
          updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemUnauthorizedSuppressNotificationsException());
      }
    }

    private void ProcessAndValidateTagUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      witRequestContext.RequestContext.TraceBlock(904581, 904590, 904585, "Services", "WorkItemService", "WorkItemService.ProcessAndValidateTagUpdates", (Action) (() =>
      {
        ITeamFoundationTaggingService taggingService = witRequestContext.RequestContext.GetService<ITeamFoundationTaggingService>();
        Dictionary<Guid, HashSet<string>> dictionary1 = new Dictionary<Guid, HashSet<string>>();
        IEnumerable<IGrouping<Guid, TeamFoundationWorkItemService.TagUpdateDescriptor>> tagUpdatesByProject = Enumerable.Empty<IGrouping<Guid, TeamFoundationWorkItemService.TagUpdateDescriptor>>();
        witRequestContext.RequestContext.TraceBlock(904883, 904884, "Services", "WorkItemService", "WorkItemService.ProcessAndValidateTagUpdates.BuildTagUpdatesGrouping", (Action) (() =>
        {
          List<WorkItemUpdateState> list = updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.HasTagsFieldChanges || us.HasTeamProjectChanged)).Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)).ToList<WorkItemUpdateState>();
          List<TeamFoundationWorkItemService.TagUpdateDescriptor> source6 = new List<TeamFoundationWorkItemService.TagUpdateDescriptor>();
          foreach (WorkItemUpdateState workItemUpdateState in list)
          {
            Guid projectGuid = workItemUpdateState.FieldData.GetProjectGuid(witRequestContext);
            string fieldValue = workItemUpdateState.FieldData.GetFieldValue<string>(witRequestContext, 80);
            IEnumerable<string> source7 = !string.IsNullOrEmpty(fieldValue) ? Microsoft.TeamFoundation.Core.WebApi.TaggingHelper.SplitTagText(fieldValue) : Enumerable.Empty<string>();
            int newTagsCount = source7.Count<string>();
            if (newTagsCount > witRequestContext.ServerSettings.MaxWorkItemTagLimit)
              workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTagLimitExceededException(workItemUpdateState.Id, newTagsCount, witRequestContext.ServerSettings.MaxWorkItemTagLimit));
            else
              source6.Add(new TeamFoundationWorkItemService.TagUpdateDescriptor()
              {
                UpdateState = workItemUpdateState,
                ProjectId = projectGuid,
                TagNames = source7
              });
          }
          tagUpdatesByProject = (IEnumerable<IGrouping<Guid, TeamFoundationWorkItemService.TagUpdateDescriptor>>) source6.GroupBy<TeamFoundationWorkItemService.TagUpdateDescriptor, Guid>((Func<TeamFoundationWorkItemService.TagUpdateDescriptor, Guid>) (x => x.ProjectId)).ToList<IGrouping<Guid, TeamFoundationWorkItemService.TagUpdateDescriptor>>();
        }));
        if (!tagUpdatesByProject.Any<IGrouping<Guid, TeamFoundationWorkItemService.TagUpdateDescriptor>>())
          return;
        witRequestContext.RequestContext.TraceBlock(15070001, 15070002, 904585, "Services", "WorkItemService", "WorkItemService.ProcessAndValidateTagUpdates_ConvertToDefinition", (Action) (() =>
        {
          foreach (IGrouping<Guid, TeamFoundationWorkItemService.TagUpdateDescriptor> source8 in tagUpdatesByProject)
          {
            Guid key3 = source8.Key;
            IEnumerable<string> strings = source8.SelectMany<TeamFoundationWorkItemService.TagUpdateDescriptor, string>((Func<TeamFoundationWorkItemService.TagUpdateDescriptor, IEnumerable<string>>) (x => x.TagNames));
            IEnumerable<TagDefinition> source9 = taggingService.EnsureTagDefinitions(witRequestContext.RequestContext, strings, (IEnumerable<Guid>) new Guid[1]
            {
              WorkItemArtifactKinds.WorkItem
            }, key3);
            string key4 = (string) null;
            try
            {
              Dictionary<string, TagDefinition> dictionary2 = source9.ToDictionary<TagDefinition, string>((Func<TagDefinition, string>) (td => td.Name), (IEqualityComparer<string>) VssStringComparer.TagName);
              foreach (TeamFoundationWorkItemService.TagUpdateDescriptor updateDescriptor in (IEnumerable<TeamFoundationWorkItemService.TagUpdateDescriptor>) source8)
              {
                List<TagDefinition> tagDefinitionList = new List<TagDefinition>();
                foreach (string tagName in updateDescriptor.TagNames)
                {
                  key4 = TaggingService.CleanTagName(witRequestContext.RequestContext, tagName);
                  tagDefinitionList.Add(dictionary2[key4]);
                }
                updateDescriptor.UpdateState.CurrentTags = (IEnumerable<TagDefinition>) tagDefinitionList;
                key4 = (string) null;
              }
            }
            catch (ArgumentException ex)
            {
              witRequestContext.RequestContext.GetService<ClientTraceService>().Publish(witRequestContext.RequestContext, "WorkItemService", "ProcessAndValidateWorkItemTagUpdates", new ClientTraceData((IDictionary<string, object>) new Dictionary<string, object>()
              {
                {
                  "ProjectGuid",
                  (object) key3.ToString()
                },
                {
                  "ExceptionType",
                  (object) ex.GetType().Name
                },
                {
                  "ExceptionMessage",
                  (object) ex.Message
                },
                {
                  "ExceptionTrace",
                  (object) ex.StackTrace
                },
                {
                  "AllTagNames",
                  (object) string.Join(",", strings)
                },
                {
                  "ResolvedTagNames",
                  (object) string.Join(",", source9.Select<TagDefinition, string>((Func<TagDefinition, string>) (td => td.Name)))
                }
              }));
              if (ex.Message.Contains("An item with the same key has already been added"))
              {
                string empty = string.Empty;
                IEnumerable<string> source10 = strings.Except<string>(source9.Select<TagDefinition, string>((Func<TagDefinition, string>) (t => t.Name)));
                throw new DuplicateTagNameException("The tag name combination of '" + (source10 == null || !source10.Any<string>() ? source9.GroupBy<TagDefinition, string>((Func<TagDefinition, string>) (t => t.Name)).Where<IGrouping<string, TagDefinition>>((Func<IGrouping<string, TagDefinition>, bool>) (t => t.Count<TagDefinition>() > 1)).First<IGrouping<string, TagDefinition>>().First<TagDefinition>().Name : source10.First<string>()) + "' already exists");
              }
              throw;
            }
            catch (KeyNotFoundException ex)
            {
              witRequestContext.RequestContext.GetService<ClientTraceService>().Publish(witRequestContext.RequestContext, "WorkItemService", "ProcessAndValidateWorkItemTagUpdates", new ClientTraceData((IDictionary<string, object>) new Dictionary<string, object>()
              {
                {
                  "ProjectGuid",
                  (object) key3.ToString()
                },
                {
                  "ExceptionType",
                  (object) ex.GetType().Name
                },
                {
                  "ExceptionMessage",
                  (object) ex.Message
                },
                {
                  "ExceptionTrace",
                  (object) ex.StackTrace
                },
                {
                  "AllTagNames",
                  (object) string.Join(",", strings)
                },
                {
                  "ResolvedTagNames",
                  (object) string.Join(",", source9.Select<TagDefinition, string>((Func<TagDefinition, string>) (td => td.Name)))
                }
              }));
              throw new DuplicateTagNameException("The tag name combination of '" + key4 + "' already exists");
            }
          }
        }));
        witRequestContext.RequestContext.TraceBlock(15070003, 15070004, 904585, "Services", "WorkItemService", "WorkItemService.ProcessAndValidateTagUpdates_CalculateDelta", (Action) (() =>
        {
          TeamFoundationWorkItemService.TagUpdateDescriptor[] array3 = tagUpdatesByProject.SelectMany<IGrouping<Guid, TeamFoundationWorkItemService.TagUpdateDescriptor>, TeamFoundationWorkItemService.TagUpdateDescriptor>((Func<IGrouping<Guid, TeamFoundationWorkItemService.TagUpdateDescriptor>, IEnumerable<TeamFoundationWorkItemService.TagUpdateDescriptor>>) (g => (IEnumerable<TeamFoundationWorkItemService.TagUpdateDescriptor>) g)).Where<TeamFoundationWorkItemService.TagUpdateDescriptor>((Func<TeamFoundationWorkItemService.TagUpdateDescriptor, bool>) (gm => !gm.UpdateState.Update.IsNew)).ToArray<TeamFoundationWorkItemService.TagUpdateDescriptor>();
          ILookup<int, ArtifactTags<int>> lookup = (!((IEnumerable<TeamFoundationWorkItemService.TagUpdateDescriptor>) array3).Any<TeamFoundationWorkItemService.TagUpdateDescriptor>() ? Enumerable.Empty<ArtifactTags<int>>() : taggingService.GetTagsForArtifacts<int>(witRequestContext.RequestContext.Elevate(), WorkItemArtifactKinds.WorkItem, ((IEnumerable<TeamFoundationWorkItemService.TagUpdateDescriptor>) array3).Select<TeamFoundationWorkItemService.TagUpdateDescriptor, TagArtifact<int>>((Func<TeamFoundationWorkItemService.TagUpdateDescriptor, TagArtifact<int>>) (tagData => new TagArtifact<int>(tagData.ProjectId, tagData.UpdateState.Id))))).ToLookup<ArtifactTags<int>, int>((Func<ArtifactTags<int>, int>) (artifact => artifact.Artifact.Id));
          foreach (WorkItemUpdateState workItemUpdateState in tagUpdatesByProject.SelectMany<IGrouping<Guid, TeamFoundationWorkItemService.TagUpdateDescriptor>, WorkItemUpdateState>((Func<IGrouping<Guid, TeamFoundationWorkItemService.TagUpdateDescriptor>, IEnumerable<WorkItemUpdateState>>) (g => g.Select<TeamFoundationWorkItemService.TagUpdateDescriptor, WorkItemUpdateState>((Func<TeamFoundationWorkItemService.TagUpdateDescriptor, WorkItemUpdateState>) (gm => gm.UpdateState)))))
          {
            TagDefinition[] array4 = lookup[workItemUpdateState.Id].SelectMany<ArtifactTags<int>, TagDefinition>((Func<ArtifactTags<int>, IEnumerable<TagDefinition>>) (gm => gm.Tags)).ToArray<TagDefinition>();
            if (((IEnumerable<TagDefinition>) array4).Any<TagDefinition>())
            {
              TagDefinitionComparer comparer = new TagDefinitionComparer();
              workItemUpdateState.RemovedTags = (IEnumerable<TagDefinition>) ((IEnumerable<TagDefinition>) array4).Except<TagDefinition>(workItemUpdateState.CurrentTags, (IEqualityComparer<TagDefinition>) comparer).ToArray<TagDefinition>();
              workItemUpdateState.AddedTags = (IEnumerable<TagDefinition>) workItemUpdateState.CurrentTags.Except<TagDefinition>((IEnumerable<TagDefinition>) array4, (IEqualityComparer<TagDefinition>) comparer).ToArray<TagDefinition>();
            }
            else
              workItemUpdateState.AddedTags = workItemUpdateState.CurrentTags;
          }
        }));
      }));
    }

    private void ValidateLinkUpdatesAndCheckLinkPermissions(
      WorkItemTrackingRequestContext witRequestContext,
      Dictionary<int, WorkItemUpdateState> updateStatesMap,
      Dictionary<int, WorkItemFieldData> workItemFieldDataMap)
    {
      witRequestContext.RequestContext.TraceBlock(904250, 904259, 904258, "Services", "WorkItemService", "UpdateWorkItems.CheckLinkPermissions", (Action) (() =>
      {
        foreach (WorkItemUpdateState updateState in updateStatesMap.Values.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success && us.HasLinkUpdates)))
        {
          foreach (WorkItemLinkUpdate workItemLinkUpdate in updateState.Update.LinkUpdates.Where<WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, bool>) (lu => !witRequestContext.LinkService.GetLinkTypeById(witRequestContext.RequestContext, lu.LinkType).IsRemote)))
          {
            bool isSource = workItemLinkUpdate.SourceWorkItemId == updateState.Id;
            int num = isSource ? workItemLinkUpdate.TargetWorkItemId : workItemLinkUpdate.SourceWorkItemId;
            WorkItemUpdateState workItemUpdateState;
            if (updateStatesMap.TryGetValue(num, out workItemUpdateState) && workItemUpdateState.Update.IsNew && !workItemUpdateState.Success)
            {
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkEndUnauthorizedAccessException(workItemLinkUpdate.LinkType, workItemLinkUpdate.SourceWorkItemId, workItemLinkUpdate.TargetWorkItemId, new WorkItemUnauthorizedAccessException(num, AccessType.Read), !isSource));
              break;
            }
            int areaId;
            if (workItemUpdateState != null && workItemUpdateState.Success)
            {
              areaId = workItemUpdateState.FieldData.AreaId;
            }
            else
            {
              if (num <= 0)
              {
                updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkEndUnauthorizedAccessException(workItemLinkUpdate.LinkType, workItemLinkUpdate.SourceWorkItemId, workItemLinkUpdate.TargetWorkItemId, new WorkItemUnauthorizedAccessException(num, AccessType.Read), !isSource));
                break;
              }
              WorkItemFieldData workItemFieldData;
              if (!workItemFieldDataMap.TryGetValue(num, out workItemFieldData))
              {
                updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkEndUnauthorizedAccessException(workItemLinkUpdate.LinkType, workItemLinkUpdate.SourceWorkItemId, workItemLinkUpdate.TargetWorkItemId, new WorkItemUnauthorizedAccessException(num, AccessType.Read), !isSource));
                break;
              }
              areaId = workItemFieldData.AreaId;
            }
            if (((!updateState.HasOnlyLinkUpdates ? 1 : (this.HasWorkItemWritePermission(witRequestContext, updateState) ? 1 : 0)) | (workItemUpdateState == null || workItemUpdateState.HasOnlyLinkUpdates ? (witRequestContext.WorkItemPermissionChecker.HasWorkItemPermission(areaId, 32) ? 1 : 0) : (true ? 1 : 0))) == 0)
            {
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkEndUnauthorizedAccessException(workItemLinkUpdate.LinkType, workItemLinkUpdate.SourceWorkItemId, workItemLinkUpdate.TargetWorkItemId, new WorkItemUnauthorizedAccessException(updateState.Id, AccessType.Write), isSource));
              break;
            }
          }
        }
      }));
    }

    private WorkItemUpdateDataset PrepareWorkItemUpdateDataset(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      Dictionary<int, WorkItemFieldData> workItemFieldDataMap,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      bool checkRevisionsLimit)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      return requestContext.TraceBlock<WorkItemUpdateDataset>(904260, 904269, 904268, "Services", "WorkItemService", "UpdateWorkItems.CollectValuesForDB", (Func<WorkItemUpdateDataset>) (() =>
      {
        IServerDefaultValueTransformer valueTransformer = witRequestContext.ServerDefaultValueTransformer;
        WorkItemUpdateDataset itemUpdateDataset = new WorkItemUpdateDataset();
        int sequenceOrder = 0;
        int num1 = -1;
        Dictionary<(int, int, int, Guid), Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> collection = new Dictionary<(int, int, int, Guid), Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>();
        if (updateStates.Any<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.HasLinkUpdates)))
        {
          foreach (WorkItemRevision workItemRevision in workItemFieldDataMap.Values.Where<WorkItemFieldData>((Func<WorkItemFieldData, bool>) (w => w is WorkItemRevision)))
          {
            IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> source = workItemRevision.WorkItemLinks.Where<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, bool>) (l => witRequestContext.LinkService.GetLinkTypeById(witRequestContext.RequestContext, l.LinkType).IsRemote));
            collection.AddRange<KeyValuePair<(int, int, int, Guid), Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>, Dictionary<(int, int, int, Guid), Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>((IEnumerable<KeyValuePair<(int, int, int, Guid), Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>) source.ToDedupedDictionary<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, (int, int, int, Guid), Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, (int, int, int, Guid)>) (l => (l.SourceId, l.TargetId, l.LinkType, l.RemoteHostId.Value)), (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>) (l => l)));
          }
        }
        foreach (WorkItemUpdateState updateState1 in updateStates)
        {
          WorkItemUpdateState updateState = updateState1;
          updateState.SetResultRevision();
          bool? hasNewRevision;
          if (!updateState.Update.IsNew && ruleExecutionMode != WorkItemUpdateRuleExecutionMode.Bypass && updateState.Update.Rev > 0 && updateState.Update.Rev != updateState.FieldData.Revision)
          {
            hasNewRevision = updateState.HasNewRevision;
            if (hasNewRevision.Value)
            {
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemRevisionMismatchException(updateState.FieldData.Id));
              continue;
            }
          }
          if (checkRevisionsLimit)
          {
            int byUpdateWorkItem = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).MaxRevisionsSupportedByUpdateWorkItem;
            if (updateState.FieldData.Revision >= byUpdateWorkItem)
            {
              updateState.UpdateResult.AddException((TeamFoundationServiceException) new TooManyRevisionsForWorkItemUpdateAPIException(updateState.Id, updateState.FieldData.Revision, byUpdateWorkItem));
              continue;
            }
          }
          WorkItemFieldData fieldData = updateState.FieldData;
          Guid projectGuid = fieldData.GetProjectGuid(requestContext);
          string distinctDisplayName = valueTransformer.TransformValue<string>(fieldData.GetFieldValue(witRequestContext, 9), InternalFieldType.String);
          DateTime dateTime = valueTransformer.TransformValue<DateTime>(fieldData.GetFieldValue(witRequestContext, -4), InternalFieldType.DateTime);
          hasNewRevision = updateState.HasNewRevision;
          if (hasNewRevision.Value)
            itemUpdateDataset.CoreFieldUpdates.Add(new WorkItemCoreFieldUpdatesRecord(updateState.DBFieldUpdates)
            {
              Id = updateState.Id,
              Revision = updateState.UpdateResult.Rev,
              WorkItemType = fieldData.GetFieldValue<string>(witRequestContext, 25),
              AreaId = fieldData.AreaId,
              IterationId = fieldData.IterationId,
              CreatedBy = valueTransformer.TransformValue<string>(fieldData.GetFieldValue(witRequestContext, 33), InternalFieldType.String),
              CreatedDate = valueTransformer.TransformValue<DateTime>(fieldData.GetFieldValue(witRequestContext, 32), InternalFieldType.DateTime),
              ChangedBy = distinctDisplayName,
              ChangedDate = dateTime,
              State = valueTransformer.TransformValue<string>(fieldData.GetFieldValue(witRequestContext, 2), InternalFieldType.String),
              Reason = valueTransformer.TransformValue<string>(fieldData.GetFieldValue(witRequestContext, 22), InternalFieldType.String),
              AssignedTo = valueTransformer.TransformValue<string>(fieldData.GetFieldValue(witRequestContext, 24), InternalFieldType.String),
              ProjectId = projectGuid,
              IsDeleted = valueTransformer.TransformValue<bool>(fieldData.GetFieldValue(witRequestContext, -404), InternalFieldType.Boolean)
            });
          else
            this.CheckAndRevertFieldDataUpdatesWithNoNewRevision(updateState);
          if (updateState.FieldData.HasUpdates)
          {
            foreach (KeyValuePair<FieldEntry, object> keyValuePair in updateState.FieldData.GetUpdatesByFieldEntry(witRequestContext))
            {
              FieldEntry key = keyValuePair.Key;
              bool flag1 = key.FieldId == 90 || key.FieldId == 91 || key.FieldId == 92;
              if ((!key.IsReadOnly || flag1) && key.FieldId != 80)
              {
                int storageTarget = (int) key.StorageTarget;
                if ((storageTarget & 2) != 0)
                {
                  TrendDataUpdateOption dataUpdateOption = TrendDataUpdateOption.None;
                  if (key.IsUsedInTrendData)
                  {
                    if (TeamFoundationWorkItemService.IsExtensionField(key.FieldId, (IEnumerable<WorkItemTypeExtension>) updateState.CurrentExtensions))
                      dataUpdateOption |= TrendDataUpdateOption.UpdateNewValue;
                    if (TeamFoundationWorkItemService.IsExtensionField(key.FieldId, (IEnumerable<WorkItemTypeExtension>) updateState.OldExtensions))
                      dataUpdateOption |= TrendDataUpdateOption.UpdateOldValue;
                  }
                  object internalValue = FieldValueHelpers.GetInternalValue(valueTransformer.TransformValue(keyValuePair.Value, key.FieldType), key.FieldType);
                  WorkItemCustomFieldUpdateRecord fieldUpdateRecord = new WorkItemCustomFieldUpdateRecord()
                  {
                    WorkItemId = updateState.Update.Id,
                    Field = key,
                    Value = internalValue,
                    TrendOption = dataUpdateOption,
                    ProjectId = projectGuid,
                    NeedsNewRevision = updateState.NeedsNewDataRow(key.FieldId)
                  };
                  itemUpdateDataset.CustomFieldUpdates.Add(fieldUpdateRecord);
                  updateState.DBFieldUpdates[fieldUpdateRecord.Field.FieldId] = fieldUpdateRecord.Value;
                }
                if ((storageTarget & 4) != 0)
                {
                  string text = keyValuePair.Value as string;
                  string str1 = text;
                  if (key.IsHtml && !string.IsNullOrEmpty(text))
                  {
                    string str2;
                    List<WorkItemResourceLinkUpdateRecord> list;
                    (str2, str1, list) = this.HandleInlineImages(witRequestContext, updateState.Id, text, projectGuid, updateState.FieldData, ref sequenceOrder);
                    text = this.HandleBlockElementLineBreaks(str2);
                    if (itemUpdateDataset.ResourceLinkUpdates.Any<WorkItemResourceLinkUpdateRecord>((Func<WorkItemResourceLinkUpdateRecord, bool>) (i =>
                    {
                      ResourceLinkType? type = i.Type;
                      ResourceLinkType resourceLinkType = ResourceLinkType.InlineImage;
                      return type.GetValueOrDefault() == resourceLinkType & type.HasValue;
                    })))
                    {
                      IEnumerable<WorkItemResourceLinkUpdateRecord> inlineImageUpdates = itemUpdateDataset.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdateRecord>((Func<WorkItemResourceLinkUpdateRecord, bool>) (i =>
                      {
                        ResourceLinkType? type = i.Type;
                        ResourceLinkType resourceLinkType = ResourceLinkType.InlineImage;
                        return type.GetValueOrDefault() == resourceLinkType & type.HasValue;
                      }));
                      list = list.Where<WorkItemResourceLinkUpdateRecord>((Func<WorkItemResourceLinkUpdateRecord, bool>) (link => !inlineImageUpdates.Contains<WorkItemResourceLinkUpdateRecord>(link, (IEqualityComparer<WorkItemResourceLinkUpdateRecord>) InlineImageComparer.Instance))).ToList<WorkItemResourceLinkUpdateRecord>();
                    }
                    itemUpdateDataset.ResourceLinkUpdates.AddRange((IEnumerable<WorkItemResourceLinkUpdateRecord>) list);
                  }
                  if (key.FieldId == 54 && !string.IsNullOrEmpty(text) && WorkItemTrackingFeatureFlags.IsCommentServiceDualWriteEnabled(requestContext))
                  {
                    string artifactId = updateState.Update.Id.ToString();
                    string upper = WorkItemIdentityHelper.GetResolvedIdentityFromDistinctDisplayName(requestContext, distinctDisplayName)?.IdentityRef?.Id?.ToUpper();
                    bool flag2 = false;
                    if (WorkItemTrackingFeatureFlags.IsMarkdownDiscussionEnabled(requestContext))
                      flag2 = updateState.HasMarkdownCommentUpdate;
                    string str3 = string.Empty;
                    if (flag2)
                    {
                      ICommentService service = requestContext.GetService<ICommentService>();
                      ProcessComment processComment = new ProcessComment(artifactId, str1, CommentFormat.Markdown);
                      IVssRequestContext requestContext1 = requestContext;
                      Guid projectId = projectGuid;
                      Guid workItem = WorkItemArtifactKinds.WorkItem;
                      ProcessComment comment = processComment;
                      str3 = service.ProcessComment(requestContext1, projectId, workItem, comment)?.RenderedText ?? string.Empty;
                    }
                    WorkItemCommentUpdateRecord commentUpdateRecord = new WorkItemCommentUpdateRecord()
                    {
                      TempId = num1,
                      ArtifactId = artifactId,
                      ArtifactKind = WorkItemArtifactKinds.WorkItem,
                      Text = text,
                      RenderedText = str3,
                      Format = new WorkItemCommentFormat?(flag2 ? WorkItemCommentFormat.Markdown : WorkItemCommentFormat.Html),
                      CreatedOnBehalfOf = upper ?? distinctDisplayName,
                      CreatedOnBehalfDate = new DateTime?(dateTime)
                    };
                    itemUpdateDataset.WorkItemCommentUpdates.Add(commentUpdateRecord);
                    --num1;
                  }
                  WorkItemTextFieldUpdateRecord fieldUpdateRecord = new WorkItemTextFieldUpdateRecord()
                  {
                    WorkItemId = updateState.Update.Id,
                    FieldId = key.FieldId,
                    IsHtml = key.IsHtml,
                    Text = text,
                    Revision = updateState.UpdateResult.Rev,
                    ProjectId = projectGuid,
                    NeedsNewRevision = updateState.NeedsNewDataRow(key.FieldId)
                  };
                  itemUpdateDataset.TextFieldUpdates.Add(fieldUpdateRecord);
                  updateState.DBFieldUpdates[fieldUpdateRecord.FieldId] = (object) str1;
                }
              }
            }
            itemUpdateDataset.CustomFieldUpdates.AddRange(((IEnumerable<WorkItemTypeExtension>) updateState.CurrentExtensions ?? Enumerable.Empty<WorkItemTypeExtension>()).Except<WorkItemTypeExtension>((IEnumerable<WorkItemTypeExtension>) updateState.OldExtensions ?? Enumerable.Empty<WorkItemTypeExtension>()).SelectMany<WorkItemTypeExtension, FieldEntry>((Func<WorkItemTypeExtension, IEnumerable<FieldEntry>>) (e => e.Fields.Select<WorkItemTypeExtensionFieldEntry, FieldEntry>((Func<WorkItemTypeExtensionFieldEntry, FieldEntry>) (ef => ef.Field)).Where<FieldEntry>((Func<FieldEntry, bool>) (f => f.IsUsedInTrendData && !updateState.FieldData.Updates.ContainsKey(f.FieldId))))).Select<FieldEntry, WorkItemCustomFieldUpdateRecord>((Func<FieldEntry, WorkItemCustomFieldUpdateRecord>) (f => new WorkItemCustomFieldUpdateRecord()
            {
              WorkItemId = updateState.Update.Id,
              Field = f,
              Value = (object) TrendDataValue.Increase,
              TrendOption = TrendDataUpdateOption.None,
              ProjectId = projectGuid
            })));
            itemUpdateDataset.CustomFieldUpdates.AddRange(((IEnumerable<WorkItemTypeExtension>) updateState.OldExtensions ?? Enumerable.Empty<WorkItemTypeExtension>()).Except<WorkItemTypeExtension>((IEnumerable<WorkItemTypeExtension>) updateState.CurrentExtensions ?? Enumerable.Empty<WorkItemTypeExtension>()).SelectMany<WorkItemTypeExtension, FieldEntry>((Func<WorkItemTypeExtension, IEnumerable<FieldEntry>>) (e => e.Fields.Select<WorkItemTypeExtensionFieldEntry, FieldEntry>((Func<WorkItemTypeExtensionFieldEntry, FieldEntry>) (ef => ef.Field)).Where<FieldEntry>((Func<FieldEntry, bool>) (f => f.IsUsedInTrendData && !updateState.FieldData.Updates.ContainsKey(f.FieldId))))).Select<FieldEntry, WorkItemCustomFieldUpdateRecord>((Func<FieldEntry, WorkItemCustomFieldUpdateRecord>) (f => new WorkItemCustomFieldUpdateRecord()
            {
              WorkItemId = updateState.Update.Id,
              Field = f,
              Value = (object) TrendDataValue.Decrease,
              TrendOption = TrendDataUpdateOption.None,
              ProjectId = projectGuid
            })));
          }
          IRuleEvaluationContext ruleEvalContext = updateState.RuleEvalContext;
          if (ruleEvalContext != null && ruleEvalContext.FirstFieldRequiresPendingCheck.HasValue)
            itemUpdateDataset.PendingSetMembershipChecks.AddRange(this.PreparePendingSetMembershipCheck(updateState.Id, (IEnumerable<FieldRuleEvalutionStatus>) ruleEvalContext.RuleEvaluationStatuses.Values));
          if (updateState.HasLinkUpdates)
          {
            IEnumerable<Guid> guids = (IEnumerable<Guid>) null;
            foreach (WorkItemLinkUpdate linkUpdate in updateState.Update.LinkUpdates)
            {
              int num2 = linkUpdate.SourceWorkItemId;
              int key = linkUpdate.TargetWorkItemId;
              int id = linkUpdate.LinkType;
              MDWorkItemLinkType linkTypeById = witRequestContext.LinkService.GetLinkTypeById(requestContext, id);
              Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo workItemLinkInfo = (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo) null;
              if (linkTypeById.IsRemote)
                collection.TryGetValue((num2, key, id, linkUpdate.RemoteHostId.Value), out workItemLinkInfo);
              if (key == updateState.Id && !linkUpdate.RemoteHostId.HasValue)
              {
                key = num2;
                num2 = updateState.Id;
                id = linkTypeById.ReverseId;
              }
              Guid projectGuid1 = updateState.FieldData.GetProjectGuid(requestContext, linkUpdate.UpdateType != LinkUpdateType.Add);
              Guid objA = linkTypeById.IsRemote ? Guid.Empty : workItemFieldDataMap[key].GetProjectGuid(requestContext);
              if (!WorkItemTrackingFeatureFlags.IsDanglingLinkDeletionDisabled(requestContext) && !linkTypeById.IsRemote && object.Equals((object) objA, (object) Guid.Empty))
              {
                if (guids == null)
                  guids = requestContext.GetService<PlatformProjectService>().GetSoftDeletedProjects(requestContext).Select<Microsoft.TeamFoundation.Core.WebApi.ProjectInfo, Guid>((Func<Microsoft.TeamFoundation.Core.WebApi.ProjectInfo, Guid>) (proj => proj.Id));
                foreach (Guid guid1 in guids)
                {
                  Guid guid2 = guid1;
                  Guid? projectId = workItemFieldDataMap[key].ProjectId;
                  if ((projectId.HasValue ? (guid2 == projectId.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                  {
                    objA = guid1;
                    break;
                  }
                }
              }
              int areaId1 = updateState.FieldData.AreaId;
              int areaId2 = linkTypeById.IsRemote ? 0 : workItemFieldDataMap[key].AreaId;
              itemUpdateDataset.SequenceOrderCorrelationIdMap[sequenceOrder] = linkUpdate.CorrelationId;
              RemoteStatus? nullable = !linkUpdate.RemoteStatus.HasValue ? (linkUpdate.UpdateType != LinkUpdateType.Update ? (linkUpdate.UpdateType != LinkUpdateType.Delete ? (linkTypeById.IsRemote ? new RemoteStatus?(RemoteStatus.PendingAdd) : new RemoteStatus?()) : new RemoteStatus?(RemoteStatus.PendingDelete)) : new RemoteStatus?(RemoteStatus.PendingUpdate)) : new RemoteStatus?(linkUpdate.RemoteStatus.Value);
              itemUpdateDataset.WorkItemLinkUpdates.Add(new WorkItemLinkUpdateRecord()
              {
                Order = sequenceOrder++,
                UpdateType = linkUpdate.UpdateType,
                SourceProjectId = projectGuid1,
                SourceId = num2,
                SourceAreaId = areaId1,
                TargetProjectId = objA,
                TargetId = key,
                TargetAreaId = areaId2,
                LinkType = id,
                Locked = linkUpdate.Locked,
                Comment = linkUpdate.Comment,
                IsRemote = linkTypeById.IsRemote,
                RemoteHostId = linkUpdate.RemoteHostId,
                RemoteProjectId = linkUpdate.RemoteProjectId,
                RemoteStatus = nullable,
                RemoteWatermark = linkUpdate.RemoteWatermark,
                RemoteStatusMessage = linkUpdate.RemoteStatusMessage,
                TimeStamp = workItemLinkInfo?.TimeStamp
              });
            }
          }
          if (updateState.HasResourceLinkUpdates)
          {
            foreach (WorkItemResourceLinkUpdate resourceLinkUpdate in updateState.Update.ResourceLinkUpdates)
            {
              itemUpdateDataset.SequenceOrderCorrelationIdMap[sequenceOrder] = resourceLinkUpdate.CorrelationId;
              Guid projectGuid2 = updateState.FieldData.GetProjectGuid(requestContext, resourceLinkUpdate.UpdateType != LinkUpdateType.Add);
              itemUpdateDataset.ResourceLinkUpdates.Add(new WorkItemResourceLinkUpdateRecord()
              {
                Order = sequenceOrder++,
                SourceId = updateState.Id,
                UpdateType = resourceLinkUpdate.UpdateType,
                ResourceId = resourceLinkUpdate.ResourceId,
                Type = resourceLinkUpdate.Type,
                Location = resourceLinkUpdate.Location,
                Name = resourceLinkUpdate.Name,
                CreationDate = resourceLinkUpdate.CreationDate,
                LastModifiedDate = resourceLinkUpdate.LastModifiedDate,
                Length = resourceLinkUpdate.Length,
                Comment = resourceLinkUpdate.Comment,
                ProjectId = projectGuid2
              });
            }
          }
          if (updateState.HasTagUpdates)
            updateState.DBFieldUpdates[80] = (object) TeamFoundationWorkItemService.TagDefinitionsToString(updateState.CurrentTags);
          if (updateState.HasTeamProjectChanged)
            itemUpdateDataset.TeamProjectChanges.Add(new WorkItemTeamProjectChangeRecord()
            {
              WorkItemId = updateState.Id,
              Revision = updateState.UpdateResult.Rev,
              SourceProjectId = updateState.FieldData.GetProjectGuid(witRequestContext, true),
              TargetProjectId = updateState.FieldData.GetProjectGuid(witRequestContext)
            });
        }
        return itemUpdateDataset;
      }));
    }

    private IEnumerable<PendingSetMembershipCheckRecord> PreparePendingSetMembershipCheck(
      int workItemId,
      IEnumerable<FieldRuleEvalutionStatus> ruleEvaluationStatusValues)
    {
      List<PendingSetMembershipCheckRecord> membershipCheckRecordList = new List<PendingSetMembershipCheckRecord>();
      foreach (FieldRuleEvalutionStatus ruleEvalutionStatus in ruleEvaluationStatusValues.Where<FieldRuleEvalutionStatus>((Func<FieldRuleEvalutionStatus, bool>) (status => (status.Flags & FieldStatusFlags.PendingListCheck) != 0)))
      {
        FieldRuleEvalutionStatus fieldStatus = ruleEvalutionStatus;
        if (fieldStatus.PendingAllowedValueChecks != null && fieldStatus.PendingAllowedValueChecks.Any<IEnumerable<ConstantSetReference>>())
          membershipCheckRecordList.AddRange(fieldStatus.PendingAllowedValueChecks.SelectMany<IEnumerable<ConstantSetReference>, PendingSetMembershipCheckRecord>((Func<IEnumerable<ConstantSetReference>, int, IEnumerable<PendingSetMembershipCheckRecord>>) ((setReferences, group) => setReferences.Select<ConstantSetReference, PendingSetMembershipCheckRecord>((Func<ConstantSetReference, PendingSetMembershipCheckRecord>) (setRef => new PendingSetMembershipCheckRecord()
          {
            WorkItemId = workItemId,
            FieldId = fieldStatus.FieldId,
            Value = CommonWITUtils.ConvertToStringForRuleCheck(fieldStatus.Value),
            UnionGroup = group,
            SetReference = setRef,
            Prohibited = false
          })))));
        if (fieldStatus.PendingProhibitedValueChecks != null && fieldStatus.PendingProhibitedValueChecks.Any<IEnumerable<ConstantSetReference>>())
          membershipCheckRecordList.AddRange(fieldStatus.PendingProhibitedValueChecks.SelectMany<IEnumerable<ConstantSetReference>, PendingSetMembershipCheckRecord>((Func<IEnumerable<ConstantSetReference>, int, IEnumerable<PendingSetMembershipCheckRecord>>) ((setReferences, group) => setReferences.Select<ConstantSetReference, PendingSetMembershipCheckRecord>((Func<ConstantSetReference, PendingSetMembershipCheckRecord>) (setRef => new PendingSetMembershipCheckRecord()
          {
            WorkItemId = workItemId,
            FieldId = fieldStatus.FieldId,
            Value = CommonWITUtils.ConvertToStringForRuleCheck(fieldStatus.Value),
            UnionGroup = group,
            SetReference = setRef,
            Prohibited = true
          })))));
      }
      return (IEnumerable<PendingSetMembershipCheckRecord>) membershipCheckRecordList;
    }

    private void UpdateIdentityMru(
      WorkItemTrackingRequestContext witRequestContext,
      ResolvedIdentityNamesInfo resolvedNamesInfo,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      witRequestContext.RequestContext.TraceBlock(904300, 904302, 904301, "Services", "WorkItemService", nameof (UpdateIdentityMru), (Action) (() =>
      {
        IVssRequestContext requestContext = witRequestContext.RequestContext;
        IFieldTypeDictionary fieldTypeDictionary = witRequestContext.FieldDictionary;
        IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = resolvedNamesInfo.IdentityMap.Value;
        IDictionary<string, ConstantsSearchRecord[]> ambiguousNamesLookup = resolvedNamesInfo.AmbiguousNamesLookup;
        requestContext.ServiceHost.InstanceId.ToString();
        List<Guid> source = new List<Guid>();
        foreach (WorkItemUpdateState updateState in updateStates)
        {
          foreach (KeyValuePair<int, object> keyValuePair in updateState.FieldUpdates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (pair => fieldTypeDictionary.GetFieldByNameOrId(pair.Key.ToString()).IsIdentity)))
          {
            string key = keyValuePair.Value as string;
            if (!string.IsNullOrEmpty(key))
            {
              if (dictionary.ContainsKey(key))
                source.Add(dictionary[key].Id);
              else if (ambiguousNamesLookup.ContainsKey(key))
                source.AddRange(((IEnumerable<ConstantsSearchRecord>) ambiguousNamesLookup[key]).Select<ConstantsSearchRecord, Guid>((Func<ConstantsSearchRecord, Guid>) (identity => identity.TeamFoundationId)));
            }
          }
        }
        if (!source.Any<Guid>())
          return;
        requestContext.GetService<IdentityMruService>().AddMruIdentities(requestContext.Elevate(), requestContext.GetUserIdentity().Id, IdentityMruService.SharedDefaultContainerId, (IList<Guid>) source.Distinct<Guid>().Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).ToList<Guid>());
      }));
    }

    private object GetServerDefaultFieldValue(
      WorkItemTrackingRequestContext witRequestContext,
      FieldEntry fieldEntry,
      object value)
    {
      if (!(value is ServerDefaultFieldValue))
        return value;
      return ((ServerDefaultFieldValue) value).Type == ServerDefaultType.ServerDateTime ? (object) null : witRequestContext.ServerDefaultValueTransformer.TransformValue(value, fieldEntry.FieldType);
    }

    private void ApplySetMembershipCheckResults(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<PendingSetMembershipCheckResultRecord> setMembershipCheckResults,
      IEnumerable<WorkItemUpdateState> updateStates,
      out bool batchFailed)
    {
      batchFailed = false;
      if (setMembershipCheckResults == null || !setMembershipCheckResults.Any<PendingSetMembershipCheckResultRecord>())
        return;
      ILookup<int, PendingSetMembershipCheckResultRecord> lookup = setMembershipCheckResults.ToLookup<PendingSetMembershipCheckResultRecord, int>((Func<PendingSetMembershipCheckResultRecord, int>) (smcr => smcr.WorkItemId));
      foreach (WorkItemUpdateState updateState in updateStates)
      {
        IEnumerable<int> ints = lookup[updateState.Id].Select<PendingSetMembershipCheckResultRecord, int>((Func<PendingSetMembershipCheckResultRecord, int>) (smcr => smcr.FieldId));
        updateState.RuleEvalContext.ClearPendingListChecks(ints);
        if (ints.Any<int>())
          updateState.UpdateResult.AddExceptions((IEnumerable<TeamFoundationServiceException>) ints.Select<int, RuleValidationException>((Func<int, RuleValidationException>) (fieldId => new RuleValidationException(witRequestContext.FieldDictionary.GetField(fieldId).ReferenceName, witRequestContext.FieldDictionary.GetField(fieldId).Name))));
      }
      batchFailed = true;
    }

    private void FinalizeUpdateResultOnUpdateStates(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      bool batchFailed,
      bool useWorkItemIdentity)
    {
      foreach (WorkItemUpdateState updateState in updateStates)
      {
        if (batchFailed)
        {
          if (updateState.Success)
            updateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemsBatchSaveFailedException());
          updateState.UpdateResult.Id = updateState.Update.Id;
          updateState.UpdateResult.Rev = updateState.Update.Rev;
          updateState.UpdateResult.Fields.Clear();
          updateState.UpdateResult.LinkUpdates.Clear();
          updateState.UpdateResult.ResourceLinkUpdates.Clear();
        }
        else if (updateState.Success)
        {
          Guid[] array1 = ((IEnumerable<WorkItemTypeExtension>) updateState.CurrentExtensions ?? Enumerable.Empty<WorkItemTypeExtension>()).Select<WorkItemTypeExtension, Guid>((Func<WorkItemTypeExtension, Guid>) (e => e.Id)).ToArray<Guid>();
          Guid[] array2 = ((IEnumerable<WorkItemTypeExtension>) updateState.OldExtensions ?? Enumerable.Empty<WorkItemTypeExtension>()).Select<WorkItemTypeExtension, Guid>((Func<WorkItemTypeExtension, Guid>) (e => e.Id)).ToArray<Guid>();
          updateState.UpdateResult.CurrentExtensions = (IEnumerable<Guid>) array1;
          updateState.UpdateResult.AttachedExtensions = (IEnumerable<Guid>) ((IEnumerable<Guid>) array1).Except<Guid>((IEnumerable<Guid>) array2).ToArray<Guid>();
          updateState.UpdateResult.DetachedExtensions = (IEnumerable<Guid>) ((IEnumerable<Guid>) array2).Except<Guid>((IEnumerable<Guid>) array1).ToArray<Guid>();
          IPermissionCheckHelper permissionChecker = witRequestContext.WorkItemPermissionChecker;
          int fieldValue = updateState.FieldData.GetFieldValue<int>(witRequestContext, -2);
          int areaId = fieldValue;
          bool emailReadable = permissionChecker.HasWorkItemPermission(areaId, 256);
          if (useWorkItemIdentity)
            this.ReplaceWithWorkItemIdentity(witRequestContext.RequestContext, updateState, fieldValue, emailReadable);
          else if (!emailReadable)
          {
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add("Error", Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.IdentityEmailNotViewable());
            witRequestContext.RequestContext.GetService<CustomerIntelligenceService>().Publish(witRequestContext.RequestContext, CustomerIntelligenceArea.WorkItemTracking, nameof (FinalizeUpdateResultOnUpdateStates), properties);
          }
          updateState.FieldData.SetFieldUpdates(witRequestContext.RequestContext, (IEnumerable<KeyValuePair<string, object>>) updateState.UpdateResult.Fields);
        }
      }
    }

    private void ApplyUpdateStateAndResultSetOnUpdateResult(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemUpdateResultSet updateResultSet,
      IEnumerable<WorkItemUpdateState> updateStates,
      Dictionary<int, WorkItemUpdateState> updateStatesMap,
      bool useWorkItemIdentity)
    {
      witRequestContext.RequestContext.TraceBlock(904280, 904289, 904288, "Services", "WorkItemService", "UpdateWorkItems.ApplyUpdateStateAndResultSetOnUpdateResult", (Action) (() =>
      {
        updateStates = (IEnumerable<WorkItemUpdateState>) updateStates.ToArray<WorkItemUpdateState>();
        bool batchFailed = false;
        this.ApplySetMembershipCheckResults(witRequestContext, updateResultSet.SetMembershipCheckResults, updateStates, out batchFailed);
        if (!batchFailed)
        {
          foreach (WorkItemUpdateState updateState in updateStates)
          {
            if (!updateState.Success)
            {
              batchFailed = true;
            }
            else
            {
              updateState.RuleEvalContext.ClearPendingListChecks(Enumerable.Empty<int>());
              updateState.UpdateResult.Fields = (IDictionary<string, object>) updateState.FieldData.GetUpdatesByFieldEntry(witRequestContext).ToDictionary<KeyValuePair<FieldEntry, object>, string, object>((Func<KeyValuePair<FieldEntry, object>, string>) (fe => fe.Key.ReferenceName), (Func<KeyValuePair<FieldEntry, object>, object>) (fe => this.GetServerDefaultFieldValue(witRequestContext, fe.Key, fe.Value)));
            }
          }
        }
        this.FinalizeUpdateResultOnUpdateStates(witRequestContext, updateStates, batchFailed, useWorkItemIdentity);
      }));
    }

    internal void ApplyUpdateResultDatasetOnUpdateStates(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      Dictionary<int, WorkItemUpdateState> updateStatesMap,
      WorkItemUpdateResultSet updateResultSet,
      WorkItemUpdateDataset updateDataset,
      bool useWorkItemIdentity)
    {
      witRequestContext.RequestContext.TraceBlock(904801, 904810, 904805, "Services", "WorkItemService", "UpdateWorkItems.ApplyUpdateResultDatasetOnUpdateStates", (Action) (() =>
      {
        updateStates = (IEnumerable<WorkItemUpdateState>) updateStates.ToArray<WorkItemUpdateState>();
        bool batchFailed = false;
        this.ApplySetMembershipCheckResults(witRequestContext, updateResultSet.SetMembershipCheckResults, updateStates, out batchFailed);
        if (!batchFailed)
        {
          Dictionary<int, List<WorkItemCommentUpdateRecord>> dictionary1 = new Dictionary<int, List<WorkItemCommentUpdateRecord>>();
          if (updateResultSet.WorkItemCommentUpdateResults != null)
          {
            foreach (WorkItemCommentUpdateRecord commentUpdateResult in updateResultSet.WorkItemCommentUpdateResults)
            {
              int key = int.Parse(commentUpdateResult.ArtifactId);
              if (dictionary1.ContainsKey(key))
                dictionary1[key].Add(commentUpdateResult);
              else
                dictionary1.Add(key, new List<WorkItemCommentUpdateRecord>()
                {
                  commentUpdateResult
                });
            }
          }
          foreach (WorkItemUpdateState updateState in updateStates)
          {
            updateState.UpdateDate = updateResultSet.ChangedDate;
            updateState.RuleEvalContext.ClearPendingListChecks(Enumerable.Empty<int>());
            List<WorkItemCommentUpdateRecord> source;
            if (dictionary1.TryGetValue(updateState.Id, out source))
              updateState.WorkItemComment = source.FirstOrDefault<WorkItemCommentUpdateRecord>();
          }
          if (updateResultSet.CoreFieldUpdatesResults != null && updateResultSet.CoreFieldUpdatesResults.Any<WorkItemCoreFieldUpdatesResultRecord>())
          {
            foreach (WorkItemCoreFieldUpdatesResultRecord fieldUpdatesResult in updateResultSet.CoreFieldUpdatesResults)
            {
              WorkItemUpdateState updateState;
              if (updateStatesMap.TryGetValue(fieldUpdatesResult.TempId, out updateState))
              {
                if (fieldUpdatesResult.Status != 0)
                {
                  batchFailed = true;
                  TeamFoundationServiceException exception = fieldUpdatesResult.Status != 600122 ? (TeamFoundationServiceException) new WorkItemFieldInvalidException(fieldUpdatesResult.Id, (string) null, FieldStatusFlags.InvalidNotEmpty) : (TeamFoundationServiceException) new WorkItemRevisionMismatchException(fieldUpdatesResult.Id);
                  updateState.UpdateResult.AddException(exception);
                }
                else
                {
                  updateState.UpdateResult.Id = fieldUpdatesResult.Id;
                  updateStatesMap[fieldUpdatesResult.Id] = updateState;
                  foreach (int key in updateState.DBFieldUpdates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (p => p.Value is DateTime && SqlDateTime.MinValue.Value.Equals((DateTime) p.Value))).Select<KeyValuePair<int, object>, int>((Func<KeyValuePair<int, object>, int>) (p => p.Key)).ToArray<int>())
                    updateState.DBFieldUpdates[key] = (object) updateResultSet.ChangedDate;
                  foreach (KeyValuePair<int, object> keyValuePair in updateState.GetFieldUpdatesToBeReported())
                  {
                    object projectName = keyValuePair.Value;
                    if (keyValuePair.Key == -42 && projectName is Guid)
                    {
                      IProjectService service = witRequestContext.RequestContext.GetService<IProjectService>();
                      try
                      {
                        projectName = (object) service.GetProjectName(witRequestContext.RequestContext, (Guid) projectName);
                      }
                      catch
                      {
                      }
                    }
                    updateState.UpdateResult.Fields[keyValuePair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = projectName;
                  }
                  if (updateState.Update.IsNew)
                  {
                    updateState.UpdateResult.Fields[-3.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = (object) updateState.UpdateResult.Id;
                    updateState.UpdateResult.Fields[32.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = (object) updateResultSet.ChangedDate;
                  }
                  this.SetCommonComputedFields(updateState, updateResultSet);
                }
              }
            }
          }
          if (!batchFailed && updateResultSet.ResourceLinkUpdateResults != null && updateResultSet.ResourceLinkUpdateResults.Any<WorkItemResourceLinkUpdateResultRecord>())
          {
            if (WorkItemTrackingFeatureFlags.IsInlineAttachmentExceptionEnabled(witRequestContext.RequestContext))
            {
              foreach (IGrouping<int, WorkItemResourceLinkUpdateResultRecord> grouping in updateResultSet.ResourceLinkUpdateResults.GroupBy<WorkItemResourceLinkUpdateResultRecord, int>((Func<WorkItemResourceLinkUpdateResultRecord, int>) (rlur => rlur.SourceId)))
              {
                WorkItemUpdateState workItemUpdateState;
                if (updateStatesMap.TryGetValue(grouping.Key, out workItemUpdateState))
                {
                  foreach (WorkItemResourceLinkUpdateResultRecord updateResultRecord in (IEnumerable<WorkItemResourceLinkUpdateResultRecord>) grouping)
                  {
                    if (updateResultRecord.Status != 0)
                    {
                      batchFailed = true;
                      int updateId = workItemUpdateState.UpdateResult.UpdateId;
                      if (updateResultRecord.Status == 600141)
                        workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new AttachmentAlreadyExistsException(updateId));
                      else if (updateResultRecord.Status == 602204)
                        workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new AttachmentNotFoundException(updateId));
                      else if (updateResultRecord.Status == 600180)
                        workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new ResourceLinkNotFoundException(updateId));
                      else
                        workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemUnauthorizedAccessException(updateId, AccessType.Write));
                    }
                    else if (updateResultRecord.ResourceType != 49)
                    {
                      string valueOrDefault = updateDataset.SequenceOrderCorrelationIdMap.GetValueOrDefault<int, string>(updateResultRecord.Order, (string) null);
                      List<WorkItemResourceLinkUpdateResult> resourceLinkUpdates = workItemUpdateState.UpdateResult.ResourceLinkUpdates;
                      resourceLinkUpdates.Add(new WorkItemResourceLinkUpdateResult()
                      {
                        CorrelationId = valueOrDefault,
                        ChangeBy = updateResultSet.ChangedById,
                        ChangedDate = updateResultSet.ChangedDate,
                        SourceWorkItemId = updateResultRecord.SourceId,
                        UpdateType = updateResultRecord.UpdateType,
                        Type = (ResourceLinkType) updateResultRecord.ResourceType,
                        ResourceId = updateResultRecord.ResourceId,
                        Location = updateResultRecord.Location
                      });
                    }
                  }
                }
              }
            }
            else
            {
              foreach (IGrouping<int, WorkItemResourceLinkUpdateResultRecord> grouping in updateResultSet.ResourceLinkUpdateResults.Where<WorkItemResourceLinkUpdateResultRecord>((Func<WorkItemResourceLinkUpdateResultRecord, bool>) (rlur => rlur.ResourceType != 49)).GroupBy<WorkItemResourceLinkUpdateResultRecord, int>((Func<WorkItemResourceLinkUpdateResultRecord, int>) (rlur => rlur.SourceId)))
              {
                WorkItemUpdateState workItemUpdateState;
                if (updateStatesMap.TryGetValue(grouping.Key, out workItemUpdateState))
                {
                  foreach (WorkItemResourceLinkUpdateResultRecord updateResultRecord in (IEnumerable<WorkItemResourceLinkUpdateResultRecord>) grouping)
                  {
                    if (updateResultRecord.Status != 0)
                    {
                      batchFailed = true;
                      int updateId = workItemUpdateState.UpdateResult.UpdateId;
                      if (updateResultRecord.Status == 600141)
                        workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new AttachmentAlreadyExistsException(updateId));
                      else if (updateResultRecord.Status == 602204)
                        workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new AttachmentNotFoundException(updateId));
                      else if (updateResultRecord.Status == 600180)
                        workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new ResourceLinkNotFoundException(updateId));
                      else
                        workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemUnauthorizedAccessException(updateId, AccessType.Write));
                    }
                    else
                    {
                      string valueOrDefault = updateDataset.SequenceOrderCorrelationIdMap.GetValueOrDefault<int, string>(updateResultRecord.Order, (string) null);
                      List<WorkItemResourceLinkUpdateResult> resourceLinkUpdates = workItemUpdateState.UpdateResult.ResourceLinkUpdates;
                      resourceLinkUpdates.Add(new WorkItemResourceLinkUpdateResult()
                      {
                        CorrelationId = valueOrDefault,
                        ChangeBy = updateResultSet.ChangedById,
                        ChangedDate = updateResultSet.ChangedDate,
                        SourceWorkItemId = updateResultRecord.SourceId,
                        UpdateType = updateResultRecord.UpdateType,
                        Type = (ResourceLinkType) updateResultRecord.ResourceType,
                        ResourceId = updateResultRecord.ResourceId,
                        Location = updateResultRecord.Location
                      });
                    }
                  }
                }
              }
            }
          }
          if (!batchFailed && updateResultSet.LinkUpdateResults != null && updateResultSet.LinkUpdateResults.Any<WorkItemLinkUpdateResultRecord>())
          {
            List<WorkItemLinkUpdateResultRecord> source = new List<WorkItemLinkUpdateResultRecord>();
            HashSet<(int, int, int, int, int)> valueTupleSet = new HashSet<(int, int, int, int, int)>();
            foreach (WorkItemLinkUpdateResultRecord linkUpdateResult in updateResultSet.LinkUpdateResults)
            {
              valueTupleSet.Add((linkUpdateResult.SourceId, linkUpdateResult.TargetId, linkUpdateResult.LinkType, linkUpdateResult.DataspaceId, linkUpdateResult.TargetDataspaceId));
              source.Add(linkUpdateResult);
            }
            WorkItemTrackingLinkService linkService = witRequestContext.LinkService;
            IVssRequestContext requestContext = witRequestContext.RequestContext;
            foreach (WorkItemLinkUpdateResultRecord linkUpdateResult in updateResultSet.LinkUpdateResults)
            {
              MDWorkItemLinkType linkTypeById = linkService.GetLinkTypeById(requestContext, linkUpdateResult.LinkType);
              int targetId = linkUpdateResult.TargetId;
              int sourceId = linkUpdateResult.SourceId;
              int num = linkUpdateResult.LinkType == linkTypeById.ForwardId ? linkTypeById.ReverseId : linkTypeById.ForwardId;
              int targetDataspaceId = linkUpdateResult.TargetDataspaceId;
              int dataspaceId = linkUpdateResult.DataspaceId;
              (int, int, int, int, int) valueTuple = (targetId, sourceId, num, targetDataspaceId, dataspaceId);
              if (!valueTupleSet.Contains(valueTuple))
              {
                valueTupleSet.Add(valueTuple);
                source.Add(new WorkItemLinkUpdateResultRecord()
                {
                  Order = linkUpdateResult.Order,
                  SourceId = targetId,
                  TargetId = sourceId,
                  DataspaceId = targetDataspaceId,
                  TargetDataspaceId = dataspaceId,
                  LinkType = num,
                  Status = linkUpdateResult.Status,
                  UpdateType = linkUpdateResult.UpdateType,
                  UpdateTypeExecuted = linkUpdateResult.UpdateTypeExecuted,
                  RemoteHostId = linkUpdateResult.RemoteHostId,
                  RemoteProjectId = linkUpdateResult.RemoteProjectId,
                  RemoteStatus = linkUpdateResult.RemoteStatus,
                  RemoteStatusMessage = linkUpdateResult.RemoteStatusMessage != null ? JsonConvert.DeserializeObject<RemoteStatusMessage>(linkUpdateResult.RemoteStatusMessage).StatusMessage : (string) null,
                  RemoteWatermark = linkUpdateResult.RemoteWatermark,
                  Timestamp = linkUpdateResult.Timestamp
                });
              }
            }
            foreach (IGrouping<int, WorkItemLinkUpdateResultRecord> grouping in !CommonWITUtils.IsRemoteLinkingEnabled(witRequestContext.RequestContext) ? source.GroupBy<WorkItemLinkUpdateResultRecord, int>((Func<WorkItemLinkUpdateResultRecord, int>) (lur => lur.SourceId)) : source.Where<WorkItemLinkUpdateResultRecord>((Func<WorkItemLinkUpdateResultRecord, bool>) (lur => lur.DataspaceId > 0)).GroupBy<WorkItemLinkUpdateResultRecord, int>((Func<WorkItemLinkUpdateResultRecord, int>) (lur => lur.SourceId)))
            {
              WorkItemUpdateState workItemUpdateState;
              if (updateStatesMap.TryGetValue(grouping.Key, out workItemUpdateState))
              {
                Dictionary<string, WorkItemLinkUpdate> dictionary2 = workItemUpdateState.Update.LinkUpdates.Where<WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, bool>) (lu => witRequestContext.LinkService.GetLinkTypeById(witRequestContext.RequestContext, lu.LinkType).IsRemote)).ToDictionary<WorkItemLinkUpdate, string, WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, string>) (x => string.Format("{0}{1}{2}{3}", (object) x.SourceWorkItemId, (object) x.TargetWorkItemId, (object) x.RemoteHostId.ToString(), (object) x.LinkType)), (Func<WorkItemLinkUpdate, WorkItemLinkUpdate>) (x => x));
                foreach (WorkItemLinkUpdateResultRecord updateResultRecord in (IEnumerable<WorkItemLinkUpdateResultRecord>) grouping)
                {
                  if (updateResultRecord.Status != 0)
                  {
                    batchFailed = true;
                    string linkName = string.Empty;
                    MDWorkItemLinkType linkType;
                    if (witRequestContext.LinkService.TryGetLinkTypeById(witRequestContext.RequestContext, updateResultRecord.LinkType, out linkType))
                      linkName = updateResultRecord.LinkType == linkType.ForwardId ? linkType.ForwardEndName : linkType.ReverseEndName;
                    int updateId = workItemUpdateState.UpdateResult.UpdateId;
                    int targetId = linkType == null || !linkType.IsRemote ? (updateResultRecord.TargetId < 1 ? updateStatesMap[updateResultRecord.TargetId].UpdateResult.UpdateId : updateResultRecord.TargetId) : (updateResultRecord.DataspaceId == 0 ? updateResultRecord.SourceId : updateResultRecord.TargetId);
                    TeamFoundationServiceException exception;
                    switch (updateResultRecord.Status)
                    {
                      case 600269:
                        exception = (TeamFoundationServiceException) new WorkItemLinkTypeDisabledException(updateId, updateResultRecord.LinkType, updateId, targetId, linkName);
                        break;
                      case 600270:
                        exception = (TeamFoundationServiceException) new WorkItemLinkCircularException(updateId, updateResultRecord.LinkType, updateId, targetId, linkName);
                        break;
                      case 600271:
                        exception = (TeamFoundationServiceException) new WorkItemLinkAddExtraParentException(updateId, updateResultRecord.LinkType, updateId, targetId, linkName, linkType.ReverseEndName);
                        break;
                      case 600274:
                        exception = (TeamFoundationServiceException) new WorkItemLinkNotFoundException(updateId, updateResultRecord.LinkType, linkName, updateId, targetId);
                        break;
                      case 600277:
                        exception = (TeamFoundationServiceException) new WorkItemLinkUnauthorizedAccessException(updateResultRecord.LinkType, AccessType.Write, updateId, targetId);
                        break;
                      case 600313:
                        exception = (TeamFoundationServiceException) new WorkItemRemoteLinksAddWhenPendingDeleteException(updateId, updateResultRecord.LinkType, linkName, updateId, targetId);
                        break;
                      default:
                        exception = (TeamFoundationServiceException) new WorkItemLinkInvalidEndsException(updateId, updateResultRecord.LinkType, linkName, updateId, targetId);
                        break;
                    }
                    workItemUpdateState.UpdateResult.AddException(exception);
                  }
                  else
                  {
                    WorkItemLinkUpdate workItemLinkUpdate;
                    dictionary2.TryGetValue(string.Format("{0}{1}{2}{3}", (object) updateResultRecord.SourceId, (object) updateResultRecord.TargetId, (object) updateResultRecord.RemoteHostId.ToString(), (object) updateResultRecord.LinkType), out workItemLinkUpdate);
                    string valueOrDefault = updateDataset.SequenceOrderCorrelationIdMap.GetValueOrDefault<int, string>(updateResultRecord.Order, (string) null);
                    List<WorkItemLinkUpdateResult> linkUpdates = workItemUpdateState.UpdateResult.LinkUpdates;
                    linkUpdates.Add(new WorkItemLinkUpdateResult()
                    {
                      CorrelationId = valueOrDefault,
                      ChangeBy = updateResultSet.ChangedById,
                      ChangedDate = updateResultSet.ChangedDate,
                      SourceWorkItemId = updateResultRecord.SourceId,
                      TargetWorkItemId = updateResultRecord.TargetId,
                      UpdateType = updateResultRecord.UpdateType,
                      UpdateTypeExecuted = updateResultRecord.UpdateTypeExecuted,
                      LinkType = updateResultRecord.LinkType,
                      RemoteHostId = updateResultRecord.RemoteHostId,
                      RemoteProjectId = updateResultRecord.RemoteProjectId,
                      RemoteStatus = updateResultRecord.RemoteStatus,
                      RemoteStatusMessage = updateResultRecord.RemoteStatusMessage,
                      RemoteWatermark = updateResultRecord.RemoteWatermark,
                      Timestamp = updateResultRecord.Timestamp,
                      RemoteWorkItemTitle = workItemLinkUpdate?.RemoteWorkItemTitle,
                      RemoteWorkItemType = workItemLinkUpdate?.RemoteWorkItemType
                    });
                  }
                }
              }
            }
          }
        }
        if (!updateResultSet.Success)
        {
          CustomerIntelligenceService service = witRequestContext.RequestContext.GetService<CustomerIntelligenceService>();
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("OverallBatchFailedDefaultReason", true);
          intelligenceData.Add("Reason", updateResultSet.FailureReason);
          IVssRequestContext requestContext = witRequestContext.RequestContext;
          string workItemTracking = CustomerIntelligenceArea.WorkItemTracking;
          CustomerIntelligenceData properties = intelligenceData;
          service.Publish(requestContext, workItemTracking, nameof (ApplyUpdateResultDatasetOnUpdateStates), properties);
          batchFailed = true;
        }
        this.FinalizeUpdateResultOnUpdateStates(witRequestContext, updateStates, batchFailed, useWorkItemIdentity);
      }));
    }

    private void TransformIdentityFieldValuesForLegacyClients(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      if (witRequestContext.RequestContext.GetIdentityDisplayType() != IdentityDisplayType.DisplayName)
        return;
      foreach (WorkItemUpdateState updateState1 in updateStates)
      {
        WorkItemUpdateState updateState = updateState1;
        foreach (KeyValuePair<int, object> keyValuePair in updateState.GetFieldUpdatesToBeReported().Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (fu => updateState.RuleEvalContext.IsIdentityField(fu.Key))))
        {
          string distinctDisplayName1 = keyValuePair.Value as string;
          if (!string.IsNullOrWhiteSpace(distinctDisplayName1))
          {
            string distinctDisplayName2 = IdentityHelper.GetDisplayNameFromDistinctDisplayName(distinctDisplayName1);
            updateState.UpdateResult.Fields[keyValuePair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = (object) distinctDisplayName2;
          }
        }
      }
    }

    private void PersistExternalArtifactWatermarks(
      IVssRequestContext requestContext,
      WorkItemUpdateDataset updateDataset)
    {
      if (!updateDataset.ResourceLinkUpdates.Any<WorkItemResourceLinkUpdateRecord>())
        return;
      List<ArtifactId> list = updateDataset.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdateRecord>((Func<WorkItemResourceLinkUpdateRecord, bool>) (rl =>
      {
        ResourceLinkType? type = rl.Type;
        ResourceLinkType resourceLinkType = ResourceLinkType.ArtifactLink;
        if (!(type.GetValueOrDefault() == resourceLinkType & type.HasValue))
          return false;
        return rl.UpdateType == LinkUpdateType.Add || rl.UpdateType == LinkUpdateType.Update;
      })).Select<WorkItemResourceLinkUpdateRecord, ArtifactId>((Func<WorkItemResourceLinkUpdateRecord, ArtifactId>) (rl => LinkingUtilities.DecodeUri(rl.Location))).Where<ArtifactId>((Func<ArtifactId, bool>) (a => a.Tool == "GitHub")).ToList<ArtifactId>();
      if (!list.Any<ArtifactId>())
        return;
      IEnumerable<ArtifactId> source1 = list.Where<ArtifactId>((Func<ArtifactId, bool>) (a => a.ArtifactType == "Commit"));
      IEnumerable<ArtifactId> source2 = list.Where<ArtifactId>((Func<ArtifactId, bool>) (a => a.ArtifactType == "PullRequest"));
      IEnumerable<ArtifactId> source3 = list.Where<ArtifactId>((Func<ArtifactId, bool>) (a => a.ArtifactType == "Issue"));
      ExternalArtifactCollectionWithStatus artifacts = new ExternalArtifactCollectionWithStatus()
      {
        Commits = (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) source1.Select<ArtifactId, (Guid, string)>((Func<ArtifactId, (Guid, string)>) (c => this.ParseExternalArtifactIdentifiers(c.ToolSpecificId))).Where<(Guid, string)>((Func<(Guid, string), bool>) (c => c.repoId != Guid.Empty)).Select<(Guid, string), ExternalArtifactAndHydrationStatus<ExternalGitCommit>>((Func<(Guid, string), ExternalArtifactAndHydrationStatus<ExternalGitCommit>>) (c => new ExternalArtifactAndHydrationStatus<ExternalGitCommit>()
        {
          ExternalArtifact = new ExternalGitCommit()
          {
            Sha = c.numberOrSha,
            Repo = new ExternalGitRepo().SetRepoInternalId(c.repoId)
          },
          HydrationStatus = ExternalArtifactHydrationStatus.Watermark,
          UpdateOnly = false
        })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitCommit>>(),
        PullRequests = (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) source2.Select<ArtifactId, (Guid, string)>((Func<ArtifactId, (Guid, string)>) (pr => this.ParseExternalArtifactIdentifiers(pr.ToolSpecificId))).Where<(Guid, string)>((Func<(Guid, string), bool>) (pr => pr.repoId != Guid.Empty)).Select<(Guid, string), ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>((Func<(Guid, string), ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>) (pr => new ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>()
        {
          ExternalArtifact = new ExternalGitPullRequest()
          {
            Number = pr.numberOrSha,
            Repo = new ExternalGitRepo().SetRepoInternalId(pr.repoId)
          },
          HydrationStatus = ExternalArtifactHydrationStatus.Watermark,
          UpdateOnly = false
        })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitPullRequest>>(),
        Issues = (IEnumerable<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) source3.Select<ArtifactId, (Guid, string)>((Func<ArtifactId, (Guid, string)>) (issue => this.ParseExternalArtifactIdentifiers(issue.ToolSpecificId))).Where<(Guid, string)>((Func<(Guid, string), bool>) (issue => issue.repoId != Guid.Empty)).Select<(Guid, string), ExternalArtifactAndHydrationStatus<ExternalGitIssue>>((Func<(Guid, string), ExternalArtifactAndHydrationStatus<ExternalGitIssue>>) (issue => new ExternalArtifactAndHydrationStatus<ExternalGitIssue>()
        {
          ExternalArtifact = new ExternalGitIssue()
          {
            Number = issue.numberOrSha,
            Repo = new ExternalGitRepo().SetRepoInternalId(issue.repoId)
          },
          HydrationStatus = ExternalArtifactHydrationStatus.Watermark,
          UpdateOnly = false
        })).ToList<ExternalArtifactAndHydrationStatus<ExternalGitIssue>>()
      };
      try
      {
        requestContext.GetService<IExternalArtifactService>().SaveArtifactWatermarks(requestContext, artifacts);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(904938, "Services", "WorkItemService", ex);
      }
    }

    private (Guid repoId, string numberOrSha) ParseExternalArtifactIdentifiers(string toolSpecificId)
    {
      string[] strArray = toolSpecificId.Split(new char[1]
      {
        '/'
      }, 2);
      Guid result;
      return strArray.Length == 2 && Guid.TryParse(strArray[0], out result) && !string.IsNullOrEmpty(strArray[1]) ? (result, strArray[1]) : (Guid.Empty, (string) null);
    }

    private void PersistTagUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      WorkItemUpdateResultSet updateResultSet)
    {
      witRequestContext.RequestContext.TraceBlock(904601, 904610, 904605, "Services", "WorkItemService", "WorkItemService.PersistTagUpdates", (Action) (() =>
      {
        List<ArtifactTagUpdate<int>> artifactTagUpdateList = new List<ArtifactTagUpdate<int>>();
        foreach (WorkItemUpdateState workItemUpdateState in updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success && us.HasTagUpdates)))
        {
          Guid projectGuid1 = workItemUpdateState.FieldData.GetProjectGuid(witRequestContext);
          if (workItemUpdateState.HasTeamProjectChanged)
          {
            Guid projectGuid2 = workItemUpdateState.FieldData.GetProjectGuid(witRequestContext, true);
            ArtifactTags<int> tagsForArtifact = witRequestContext.RequestContext.GetService<ITeamFoundationTaggingService>().GetTagsForArtifact<int>(witRequestContext.RequestContext.Elevate(), WorkItemArtifactKinds.WorkItem, new TagArtifact<int>(projectGuid2, workItemUpdateState.Id));
            if (tagsForArtifact.Tags.Any<TagDefinition>())
              artifactTagUpdateList.Add(new ArtifactTagUpdate<int>()
              {
                Artifact = new VersionedTagArtifact<int>(projectGuid2, workItemUpdateState.UpdateResult.Id, workItemUpdateState.UpdateResult.Rev),
                AddedTagIds = Enumerable.Empty<Guid>(),
                RemovedTagIds = tagsForArtifact.Tags.Select<TagDefinition, Guid>((Func<TagDefinition, Guid>) (t => t.TagId))
              });
          }
          artifactTagUpdateList.Add(new ArtifactTagUpdate<int>()
          {
            Artifact = new VersionedTagArtifact<int>(projectGuid1, workItemUpdateState.UpdateResult.Id, workItemUpdateState.UpdateResult.Rev),
            AddedTagIds = (workItemUpdateState.AddedTags ?? Enumerable.Empty<TagDefinition>()).Select<TagDefinition, Guid>((Func<TagDefinition, Guid>) (td => td.TagId)),
            RemovedTagIds = (workItemUpdateState.RemovedTags ?? Enumerable.Empty<TagDefinition>()).Select<TagDefinition, Guid>((Func<TagDefinition, Guid>) (td => td.TagId))
          });
        }
        if (!artifactTagUpdateList.Any<ArtifactTagUpdate<int>>())
          return;
        ITeamFoundationTaggingService service = witRequestContext.RequestContext.GetService<ITeamFoundationTaggingService>();
        try
        {
          service.UpdateTagsForArtifacts<int>(witRequestContext.RequestContext.Elevate(), WorkItemArtifactKinds.WorkItem, (IEnumerable<ArtifactTagUpdate<int>>) artifactTagUpdateList, new DateTime?(updateResultSet.ChangedDate), new Guid?(witRequestContext.RequestIdentity.Id));
        }
        catch (TeamFoundationServiceException ex)
        {
          foreach (WorkItemUpdateState workItemUpdateState in updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success && us.HasTagUpdates)))
            workItemUpdateState.UpdateResult.AddException(ex);
        }
      }));
    }

    internal virtual IEnumerable<WorkItemChangedEventExtended> TryFireWorkItemChangedEvents(
      WorkItemTrackingRequestContext witRequestContext,
      IDictionary<int, WorkItemFieldData> fieldDataMap,
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      IEnumerable<WorkItemUpdateState> updateStates,
      bool suppressNotifications = false)
    {
      return this.TryFireWorkItemChangedEvents(witRequestContext, fieldDataMap, identityMap, updateStates, this.m_serviceBusEventPublisher, suppressNotifications);
    }

    internal virtual IEnumerable<WorkItemChangedEventExtended> TryFireWorkItemChangedEvents(
      WorkItemTrackingRequestContext witRequestContext,
      IDictionary<int, WorkItemFieldData> fieldDataMap,
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      IEnumerable<WorkItemUpdateState> updateStates,
      WorkItemChangedEventServiceBusPublisher serviceBusEventPublisher,
      bool suppressNotifications = false)
    {
      return WorkItemChangedEvent.TryFireWorkItemChangedEvents(witRequestContext, this.m_serviceBusEventPublisher, this.m_updateEventNotifier, fieldDataMap, identityMap, updateStates, suppressNotifications);
    }

    private void ValidateWorkItemChangedDate(
      IDictionary<int, WorkItemUpdateState> workItemUpdates,
      IDictionary<int, WorkItemFieldData> tipValues)
    {
      foreach (KeyValuePair<int, WorkItemUpdateState> workItemUpdate in (IEnumerable<KeyValuePair<int, WorkItemUpdateState>>) workItemUpdates)
      {
        WorkItemFieldData workItemFieldData;
        if (!workItemUpdate.Value.Update.IsNew && workItemUpdate.Value.HasFieldUpdate(-4) && tipValues.TryGetValue(workItemUpdate.Key, out workItemFieldData))
        {
          KeyValuePair<int, object> keyValuePair = workItemUpdate.Value.FieldUpdates.First<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (f => f.Key == -4));
          DateTime dateTime1 = Convert.ToDateTime(keyValuePair.Value);
          keyValuePair = workItemFieldData.LatestData.First<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (d => d.Key == -4));
          DateTime dateTime2 = Convert.ToDateTime(keyValuePair.Value);
          if (dateTime1 <= dateTime2)
            workItemUpdate.Value.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemDatesNotIncreasingException());
        }
      }
    }

    private void ValidateWorkItemSecretScan(
      IVssRequestContext requestContext,
      IDictionary<int, WorkItemUpdateState> workItemUpdates)
    {
      try
      {
        if (requestContext == null || !requestContext.IsMicrosoftTenant() || workItemUpdates.IsNullOrEmpty<KeyValuePair<int, WorkItemUpdateState>>())
          return;
        WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
        IDictionary<int, string> enumerable;
        using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(trackingRequestContext.ServerSettings.MaxSecretsScanServiceRequestTimeoutInMilliseconds))
          enumerable = new WorkItemUpdateSecretsScanner(cancellationTokenSource.Token).ProcessBatch(requestContext, workItemUpdates, trackingRequestContext.ServerSettings.MaxSecretsScanContentLength);
        if (!WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsBlockingEnabled(requestContext) || enumerable.IsNullOrEmpty<KeyValuePair<int, string>>())
          return;
        List<int> intList = (List<int>) null;
        foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>) enumerable)
        {
          if (workItemUpdates.ContainsKey(keyValuePair.Key))
          {
            workItemUpdates[keyValuePair.Key]?.UpdateResult?.AddException((TeamFoundationServiceException) new WorkItemFieldInvalidException(workItemUpdates[keyValuePair.Key].Id, keyValuePair.Value));
          }
          else
          {
            intList = intList ?? new List<int>();
            intList.Add(keyValuePair.Key);
          }
        }
        if (intList == null)
          return;
        requestContext.GetService<ClientTraceService>().Publish(requestContext, "WorkItemService", "SecretScanWorkItems", new ClientTraceData((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "InvalidBlockingFindingKeys",
            (object) intList
          }
        }));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(904945, "WorkItemService", "SecretScanWorkItems", ex);
      }
    }

    private void SetCommonComputedFields(
      WorkItemUpdateState updateState,
      WorkItemUpdateResultSet updateResultSet)
    {
      updateState.UpdateResult.Fields[8.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = (object) updateState.UpdateResult.Rev;
      updateState.UpdateResult.Fields[-5.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = (object) SharedVariables.FutureDateTimeValue;
      updateState.UpdateResult.Fields[-6.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = (object) updateResultSet.ChangedById;
      updateState.UpdateResult.Fields[3.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = (object) updateResultSet.ChangedDate;
      updateState.UpdateResult.Fields[-4.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = (object) updateResultSet.ChangedDate;
      updateState.UpdateResult.Fields[7.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)] = (object) updateResultSet.Watermark;
    }

    private void ConvertUpdateFieldsToRequiredFormat(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates)
    {
      IFieldTypeDictionary fieldDict = witRequestContext.FieldDictionary;
      foreach (WorkItemUpdate workItemUpdate in workItemUpdates)
      {
        ArgumentUtility.CheckForNull<WorkItemUpdate>(workItemUpdate, "update");
        IEnumerable<KeyValuePair<string, object>> fields = workItemUpdate.Fields;
        workItemUpdate.Fields = fields != null ? (IEnumerable<KeyValuePair<string, object>>) fields.Select<KeyValuePair<string, object>, KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, KeyValuePair<string, object>>) (pair =>
        {
          int result;
          if (int.TryParse(pair.Key, out result))
            return new KeyValuePair<string, object>(fieldDict.GetField(result).ReferenceName, pair.Value);
          FieldEntry field;
          return fieldDict.TryGetField(pair.Key, out field) ? new KeyValuePair<string, object>(field.ReferenceName, pair.Value) : pair;
        })).ToList<KeyValuePair<string, object>>() : (IEnumerable<KeyValuePair<string, object>>) null;
      }
    }

    private void CheckForDuplicateFieldUpdates(IEnumerable<WorkItemUpdate> workItemUpdates)
    {
      foreach (WorkItemUpdate workItemUpdate in workItemUpdates)
      {
        if (workItemUpdate.Fields != null)
        {
          IEnumerable<IGrouping<string, KeyValuePair<string, object>>> source = workItemUpdate.Fields.GroupBy<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (f => f.Key), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).Where<IGrouping<string, KeyValuePair<string, object>>>((Func<IGrouping<string, KeyValuePair<string, object>>, bool>) (g => g.Count<KeyValuePair<string, object>>() > 1));
          if (source.Any<IGrouping<string, KeyValuePair<string, object>>>())
            throw new InvalidOperationException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.MultipleUpdatesToSameField((object) workItemUpdate.Id, (object) source.First<IGrouping<string, KeyValuePair<string, object>>>().Key));
        }
      }
    }

    internal bool UpdateWorkItemDependencyViolations(
      IVssRequestContext requestContext,
      WorkItemUpdate update)
    {
      bool flag = false;
      if (update.Id >= 0 && (!update.LinkUpdates.IsNullOrEmpty<WorkItemLinkUpdate>() || !update.Fields.IsNullOrEmpty<KeyValuePair<string, object>>() && update.Fields.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (x => x.Key.Equals(this.c_IterationId) || x.Key.Equals(this.c_TargetDate) || x.Key.Equals(this.c_IterationPath)))))
      {
        try
        {
          using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
          {
            if (!update.LinkUpdates.IsNullOrEmpty<WorkItemLinkUpdate>())
            {
              List<int> idsToAdd = new List<int>();
              update.LinkUpdates.ForEach<WorkItemLinkUpdate>((Action<WorkItemLinkUpdate>) (link =>
              {
                idsToAdd.Add(link.TargetWorkItemId);
                idsToAdd.Add(link.SourceWorkItemId);
              }));
              component.AddPendingWorkItems(idsToAdd.Distinct<int>().ToList<int>());
            }
            else
              component.AddPendingWorkItem(update.Id);
          }
          ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
          int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/DependencyViolationJobLimit", true, 60);
          IVssRequestContext requestContext1 = requestContext;
          Guid[] jobIds = new Guid[1]
          {
            WorkItemTrackingJobs.UpdateViolations
          };
          int maxDelaySeconds = num;
          service.QueueDelayedJobs(requestContext1, (IEnumerable<Guid>) jobIds, maxDelaySeconds);
          flag = true;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(904931, "Services", "WorkItemService", ex);
        }
      }
      return flag;
    }

    private bool ResourceLinkInCollection(
      IEnumerable<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType> links,
      WorkItemResourceLinkUpdate update,
      ArtifactId artifactId)
    {
      return links.Any<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType>((Func<Microsoft.TeamFoundation.Framework.Server.OutboundLinkType, bool>) (wal => VssStringComparer.LinkName.Equals(update.Name, wal.Name) && VssStringComparer.ArtifactTool.Equals(artifactId.Tool, wal.TargetArtifactTypeTool) && VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, wal.TargetArtifactTypeName)));
    }

    public WorkItemWithActivityFeedEntries GetWorkItemWithActivityFeedEntries(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      HashSet<string> fieldReferenceNames = null,
      int top = 200,
      int skip = 0,
      bool includeLinks = false,
      bool includeAttachments = false,
      bool includeArtifactLinks = false,
      bool returnIdentityRef = true,
      bool includeFieldFlags = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckBoundsInclusive(top, 0, 200, nameof (top));
      ArgumentUtility.CheckForNonnegativeInt(skip, nameof (skip));
      string securityToken;
      WorkItem workItem;
      using (PerformanceTimer.StartMeasure(requestContext, "GetWorkItemWithActivityFeedEntries.GetWorkItemById"))
        workItem = this.GetWorkItemById(requestContext, workItemId, 16, projectId, out securityToken, new DateTime?(), includeAttachments | includeArtifactLinks, includeLinks, true, true, WorkItemRetrievalMode.NonDeleted, false, true, false, new DateTime?());
      if (workItem == null)
        throw new WorkItemNotFoundException(workItemId);
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      Dictionary<string, object> dictionary;
      using (PerformanceTimer.StartMeasure(requestContext, "GetWorkItemWithActivityFeedEntries.GetAllFieldValuesByFieldEntry"))
        dictionary = workItem.GetAllFieldValuesByFieldEntry(witRequestContext, true).Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (kvp => fieldReferenceNames == null || fieldReferenceNames.Contains(kvp.Key.ReferenceName))).ToDictionary<KeyValuePair<FieldEntry, object>, string, object>((Func<KeyValuePair<FieldEntry, object>, string>) (kvp => kvp.Key.ReferenceName), (Func<KeyValuePair<FieldEntry, object>, object>) (kvp => returnIdentityRef && kvp.Key.IsIdentity ? (object) WorkItemIdentityHelper.GetIdentityRef(requestContext, kvp.Value, (ISecuredObject) workItem) : kvp.Value), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      Guid projectGuid = workItem.GetProjectGuid(requestContext);
      List<ActivityFeedEntry> list;
      using (PerformanceTimer.StartMeasure(requestContext, "GetWorkItemWithActivityFeedEntries.GetWorkItemActivityFeedEntries"))
        list = this.GetWorkItemActivityFeedEntries(requestContext, workItem, projectGuid, fieldReferenceNames, returnIdentityRef).Skip<ActivityFeedEntry>(skip).Take<ActivityFeedEntry>(top).ToList<ActivityFeedEntry>();
      using (PerformanceTimer.StartMeasure(requestContext, "GetWorkItemWithActivityFeedEntries.WorkItemWithActivityFeedEntries"))
        return new WorkItemWithActivityFeedEntries(16, securityToken)
        {
          Id = workItem.Id,
          ProjectId = projectGuid,
          LatestFieldValues = dictionary,
          ActivityFeedEntries = list,
          Revision = workItem.Revision,
          Links = includeLinks ? workItem.WorkItemLinks.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, WorkItemLinkedItem>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, WorkItemLinkedItem>) (link => new WorkItemLinkedItem(requestContext, link, (ISecuredObject) workItem))).ToList<WorkItemLinkedItem>() : (List<WorkItemLinkedItem>) null,
          Attachments = includeAttachments ? workItem.ResourceLinks.Where<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType == ResourceLinkType.Attachment)).Select<WorkItemResourceLinkInfo, WorkItemAttachment>((Func<WorkItemResourceLinkInfo, WorkItemAttachment>) (link => new WorkItemAttachment(requestContext, link, workItem))).ToList<WorkItemAttachment>() : (List<WorkItemAttachment>) null,
          ArtifactLinks = includeArtifactLinks ? workItem.ResourceLinks.Where<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType != ResourceLinkType.Attachment)).Select<WorkItemResourceLinkInfo, WorkItemArtifactLink>((Func<WorkItemResourceLinkInfo, WorkItemArtifactLink>) (link => new WorkItemArtifactLink(link, (ISecuredObject) workItem))).ToList<WorkItemArtifactLink>() : (List<WorkItemArtifactLink>) null,
          FieldFlags = includeFieldFlags ? this.GetFieldFlagStatus(requestContext, dictionary, projectGuid) : (Dictionary<string, FieldStatusFlags>) null
        };
    }

    public Dictionary<string, FieldStatusFlags> GetFieldEditStates(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      string workItemTypeName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      WorkItemFieldData workItemFieldData;
      if (workItemId < 0)
      {
        workItemFieldData = new WorkItemFieldData((IDictionary<int, object>) null, projectId);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemTypeName, nameof (workItemTypeName));
      }
      else
      {
        workItemFieldData = this.GetWorkItemFieldValues(requestContext, (IEnumerable<int>) new int[1]
        {
          workItemId
        }, 16, suppressCustomerIntelligence: true, useWorkItemIdentity: true).FirstOrDefault<WorkItemFieldData>();
        if (workItemFieldData == null)
          return (Dictionary<string, FieldStatusFlags>) null;
        workItemTypeName = workItemFieldData.GetFieldValue<string>(requestContext, SystemWorkItemType.ReferenceName);
      }
      WorkItemType typeByReferenceName = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(requestContext, projectId, workItemTypeName);
      AdditionalWorkItemTypeProperties additionalProperties = typeByReferenceName.GetAdditionalProperties(requestContext);
      IRuleEvaluationContext evaluationContext = workItemFieldData.GetRuleEvaluationContext(requestContext);
      new RuleEngine(additionalProperties.FieldRules).Evaluate(evaluationContext);
      return typeByReferenceName.GetFields(requestContext, true).ToDictionary<FieldEntry, string, FieldStatusFlags>((Func<FieldEntry, string>) (x => x.ReferenceName), (Func<FieldEntry, FieldStatusFlags>) (x => evaluationContext.GetFieldFlags(x.FieldId)), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
    }

    public string[] GetValidNextStates(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName,
      int workItemId,
      string fieldRefName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(workItemTypeName, nameof (workItemTypeName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(fieldRefName, nameof (fieldRefName));
      WorkItem workItemById = this.GetWorkItemById(requestContext, workItemId, 16, new DateTime?(), false, false, false, WorkItemRetrievalMode.All, false, true, new Guid?(projectId), false, new DateTime?());
      WorkItemType typeByReferenceName = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(requestContext, projectId, workItemTypeName);
      AdditionalWorkItemTypeProperties additionalProperties = typeByReferenceName.GetAdditionalProperties(requestContext);
      IVssRequestContext requestContext1 = requestContext;
      string fieldReferenceName = fieldRefName;
      string key = workItemById.GetFieldValue(requestContext1, fieldReferenceName).ToString();
      HashSet<string> availableStates = additionalProperties.Transitions.ContainsKey(key) ? additionalProperties.Transitions[key] : (HashSet<string>) null;
      IEnumerable<Microsoft.TeamFoundation.Core.WebApi.ProjectProperty> projectProperties = requestContext.GetService<IProjectService>().GetProjectProperties(requestContext, projectId, ProcessTemplateIdPropertyNames.ProcessTemplateType);
      Microsoft.TeamFoundation.Core.WebApi.ProjectProperty projectProperty = projectProperties != null ? projectProperties.FirstOrDefault<Microsoft.TeamFoundation.Core.WebApi.ProjectProperty>((Func<Microsoft.TeamFoundation.Core.WebApi.ProjectProperty, bool>) (property => property.Name == ProcessTemplateIdPropertyNames.ProcessTemplateType)) : (Microsoft.TeamFoundation.Core.WebApi.ProjectProperty) null;
      if (projectProperty != null && projectProperty.Value != null)
      {
        Guid processId = Guid.Parse((string) projectProperty.Value);
        IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions = requestContext.GetService<IWorkItemStateDefinitionService>().GetStateDefinitions(requestContext, processId, typeByReferenceName.ReferenceName, true);
        if (stateDefinitions != null && stateDefinitions.Any<WorkItemStateDefinition>())
          return stateDefinitions.ToList<WorkItemStateDefinition>().Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (sd =>
          {
            HashSet<string> stringSet = availableStates;
            // ISSUE: explicit non-virtual call
            return stringSet == null || __nonvirtual (stringSet.Contains(sd.Name));
          })).OrderBy<WorkItemStateDefinition, int>((Func<WorkItemStateDefinition, int>) (state => state.Order)).Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)).ToArray<string>();
      }
      HashSet<string> source = availableStates;
      return source == null ? (string[]) null : source.ToArray<string>();
    }

    private Dictionary<string, FieldStatusFlags> GetFieldFlagStatus(
      IVssRequestContext requestContext,
      Dictionary<string, object> latestFieldValues,
      Guid projectId)
    {
      string workItemTypeName = latestFieldValues[SystemWorkItemType.ReferenceName].ToString();
      WorkItemType typeByReferenceName = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(requestContext, projectId, workItemTypeName);
      WorkItemFieldData workItemFieldData = new WorkItemFieldData((IDictionary<int, object>) typeByReferenceName.GetFields(requestContext, true).Where<FieldEntry>((Func<FieldEntry, bool>) (x => latestFieldValues.ContainsKey(x.ReferenceName))).ToDictionary<FieldEntry, int, object>((Func<FieldEntry, int>) (f => f.FieldId), (Func<FieldEntry, object>) (x => latestFieldValues[x.ReferenceName])));
      AdditionalWorkItemTypeProperties additionalProperties = typeByReferenceName.GetAdditionalProperties(requestContext);
      IRuleEvaluationContext evaluationContext = workItemFieldData.GetRuleEvaluationContext(requestContext);
      new RuleEngine(additionalProperties.FieldRules).Evaluate(evaluationContext);
      return typeByReferenceName.GetFields(requestContext, true).ToDictionary<FieldEntry, string, FieldStatusFlags>((Func<FieldEntry, string>) (x => x.ReferenceName), (Func<FieldEntry, FieldStatusFlags>) (x => evaluationContext.GetFieldFlags(x.FieldId)), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
    }

    private IEnumerable<ActivityFeedEntry> GetWorkItemActivityFeedEntries(
      IVssRequestContext requestContext,
      WorkItem workItem,
      Guid projectId,
      HashSet<string> fieldsSet,
      bool returnIdentityRef = true)
    {
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      HashSet<string> longTextFieldsSet = new LongTextFieldValues().GetLongTextFieldValuesSet();
      IEnumerable<WorkItemRevision> allRevisions = workItem.Revisions.Concat<WorkItemRevision>((IEnumerable<WorkItemRevision>) new WorkItemRevision[1]
      {
        (WorkItemRevision) workItem
      });
      Dictionary<DateTime, IDictionary<int, WorkItemResourceLinkInfo>> resourceLinksByRevisedDate = new Dictionary<DateTime, IDictionary<int, WorkItemResourceLinkInfo>>();
      WorkItemRevision previousRevision = (WorkItemRevision) null;
      string previousRevisionTag = (string) null;
      int updateId = 1;
      Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment> commentsMap = TeamFoundationWorkItemService.CreateCommentsMap(requestContext, workItem, projectId, allRevisions);
      foreach (WorkItemRevision workItemRevision in allRevisions)
      {
        WorkItemRevision revision = workItemRevision;
        IEnumerable<ActivityFeedEntry> workItemLinkFeedEntries = this.GetWorkItemLinkUpdates(requestContext, workItem, revision, previousRevision);
        int skipLinks = 0;
        WorkItemArtifactUpdates<string> tags = new WorkItemArtifactUpdates<string>((ISecuredObject) workItem);
        Dictionary<string, object> dictionary = revision.GetAllFieldValuesByFieldEntry(witRequestContext, true).Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (kvp =>
        {
          if (fieldsSet != null && !fieldsSet.Contains(kvp.Key.ReferenceName))
            return false;
          if (kvp.Key.ReferenceName.Equals("System.Tags", StringComparison.OrdinalIgnoreCase))
          {
            string text = kvp.Value?.ToString();
            IEnumerable<string> strings1 = Microsoft.TeamFoundation.Core.WebApi.TaggingHelper.SplitTagText(previousRevisionTag);
            IEnumerable<string> strings2 = Microsoft.TeamFoundation.Core.WebApi.TaggingHelper.SplitTagText(text);
            tags.Added = (IEnumerable<string>) strings2.Except<string>(strings1).ToArray<string>();
            tags.Removed = (IEnumerable<string>) strings1.Except<string>(strings2).ToArray<string>();
            previousRevisionTag = text;
            return false;
          }
          object fieldValue = previousRevision?.GetFieldValue(requestContext, kvp.Key.ReferenceName);
          return kvp.Key.ReferenceName.Equals("System.History", StringComparison.OrdinalIgnoreCase) || !object.Equals(kvp.Value, fieldValue);
        })).ToDictionary<KeyValuePair<FieldEntry, object>, string, object>((Func<KeyValuePair<FieldEntry, object>, string>) (kvp => kvp.Key.ReferenceName), (Func<KeyValuePair<FieldEntry, object>, object>) (kvp =>
        {
          if (longTextFieldsSet.Contains(kvp.Key.ReferenceName))
            return (object) string.Empty;
          return returnIdentityRef && kvp.Key.IsIdentity ? (object) WorkItemIdentityHelper.GetIdentityRef(requestContext, kvp.Value, (ISecuredObject) revision) : kvp.Value;
        }));
        ActivityFeedEntry activityFeedEntry1 = (ActivityFeedEntry) null;
        IEnumerable<ActivityFeedEntry> source = workItemLinkFeedEntries;
        ActivityFeedEntry activityFeedEntry2 = source != null ? source.FirstOrDefault<ActivityFeedEntry>() : (ActivityFeedEntry) null;
        if (activityFeedEntry2 != null && activityFeedEntry2.RevisedDate == revision.AuthorizedDate)
        {
          skipLinks = 1;
          activityFeedEntry1 = activityFeedEntry2;
        }
        WorkItemArtifactUpdates<WorkItemResourceLinkInfo> resourceLinkUpdates = this.GetResourceLinkUpdates(requestContext, workItem, revision, (IDictionary<DateTime, IDictionary<int, WorkItemResourceLinkInfo>>) resourceLinksByRevisedDate);
        WorkItemArtifactUpdates<WorkItemAttachment> attachmentUpdates = TeamFoundationWorkItemService.GetAttachmentUpdates(requestContext, workItem, resourceLinkUpdates);
        WorkItemArtifactUpdates<WorkItemArtifactLink> artifactLinkUpdates = TeamFoundationWorkItemService.GetArtifactLinkUpdates(workItem, resourceLinkUpdates);
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment tipCommentVersion = TeamFoundationWorkItemService.GetTipCommentVersion(commentsMap, revision);
        yield return new ActivityFeedEntry()
        {
          Id = updateId,
          RevisionId = revision.Revision,
          RevisedDate = revision.AuthorizedDate,
          RevisedBy = WorkItemIdentityHelper.GetIdentityRef(WorkItemIdentityHelper.GetResolvedIdentityFromDistinctDisplayName(requestContext, revision.ModifiedBy), (ISecuredObject) workItem),
          Links = activityFeedEntry1?.Links,
          Attachments = attachmentUpdates,
          ArtifactLinks = artifactLinkUpdates,
          Comment = tipCommentVersion,
          Tags = tags,
          FieldValues = dictionary
        };
        ++updateId;
        foreach (ActivityFeedEntry activityFeedEntry3 in workItemLinkFeedEntries.Skip<ActivityFeedEntry>(skipLinks))
        {
          activityFeedEntry3.Id = updateId;
          yield return activityFeedEntry3;
          ++updateId;
        }
        previousRevision = revision;
        workItemLinkFeedEntries = (IEnumerable<ActivityFeedEntry>) null;
      }
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment GetTipCommentVersion(
      Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment> commentsMap,
      WorkItemRevision revision)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment tipCommentVersion = (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment) null;
      WorkItemCommentVersionRecord commentVersion = revision.CommentVersion;
      if ((commentVersion != null ? (commentVersion.Version == 1 ? 1 : 0) : 0) != 0)
        commentsMap.TryGetValue(revision.CommentVersion.CommentId, out tipCommentVersion);
      return tipCommentVersion;
    }

    private static WorkItemArtifactUpdates<WorkItemArtifactLink> GetArtifactLinkUpdates(
      WorkItem workItem,
      WorkItemArtifactUpdates<WorkItemResourceLinkInfo> resourceLinkUpdates)
    {
      return new WorkItemArtifactUpdates<WorkItemArtifactLink>((ISecuredObject) workItem)
      {
        Added = resourceLinkUpdates.Added.Where<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType != ResourceLinkType.Attachment)).Select<WorkItemResourceLinkInfo, WorkItemArtifactLink>((Func<WorkItemResourceLinkInfo, WorkItemArtifactLink>) (link => new WorkItemArtifactLink(link, (ISecuredObject) workItem))),
        Removed = resourceLinkUpdates.Removed.Where<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType != ResourceLinkType.Attachment)).Select<WorkItemResourceLinkInfo, WorkItemArtifactLink>((Func<WorkItemResourceLinkInfo, WorkItemArtifactLink>) (link => new WorkItemArtifactLink(link, (ISecuredObject) workItem))),
        Updated = resourceLinkUpdates.Updated.Where<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType != ResourceLinkType.Attachment)).Select<WorkItemResourceLinkInfo, WorkItemArtifactLink>((Func<WorkItemResourceLinkInfo, WorkItemArtifactLink>) (link => new WorkItemArtifactLink(link, (ISecuredObject) workItem)))
      };
    }

    private static WorkItemArtifactUpdates<WorkItemAttachment> GetAttachmentUpdates(
      IVssRequestContext requestContext,
      WorkItem workItem,
      WorkItemArtifactUpdates<WorkItemResourceLinkInfo> resourceLinkUpdates)
    {
      return new WorkItemArtifactUpdates<WorkItemAttachment>((ISecuredObject) workItem)
      {
        Added = resourceLinkUpdates.Added.Where<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType == ResourceLinkType.Attachment)).Select<WorkItemResourceLinkInfo, WorkItemAttachment>((Func<WorkItemResourceLinkInfo, WorkItemAttachment>) (link => new WorkItemAttachment(requestContext, link, workItem))),
        Removed = resourceLinkUpdates.Removed.Where<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType == ResourceLinkType.Attachment)).Select<WorkItemResourceLinkInfo, WorkItemAttachment>((Func<WorkItemResourceLinkInfo, WorkItemAttachment>) (link => new WorkItemAttachment(requestContext, link, workItem))),
        Updated = resourceLinkUpdates.Updated.Where<WorkItemResourceLinkInfo>((Func<WorkItemResourceLinkInfo, bool>) (link => link.ResourceType == ResourceLinkType.Attachment)).Select<WorkItemResourceLinkInfo, WorkItemAttachment>((Func<WorkItemResourceLinkInfo, WorkItemAttachment>) (link => new WorkItemAttachment(requestContext, link, workItem)))
      };
    }

    private static Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment> CreateCommentsMap(
      IVssRequestContext requestContext,
      WorkItem workItem,
      Guid projectId,
      IEnumerable<WorkItemRevision> allRevisions)
    {
      HashSet<int> hashSet = allRevisions.Where<WorkItemRevision>((Func<WorkItemRevision, bool>) (rev => rev.CommentVersion?.Text != null)).Select<WorkItemRevision, int>((Func<WorkItemRevision, int>) (rev => rev.CommentVersion.CommentId)).ToHashSet<int>();
      if (hashSet.Count == 0)
        return new Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>();
      ICommentService service = requestContext.GetService<ICommentService>();
      CommentsList commentsList = (CommentsList) null;
      using (PerformanceTimer.StartMeasure(requestContext, "CreateCommentsMap.GetComments"))
        commentsList = service.GetComments(requestContext, projectId, WorkItemArtifactKinds.WorkItem, workItem.Id.ToString(), (ISet<int>) hashSet, ExpandOptions.Reactions);
      IReadOnlyCollection<Comment> comments = commentsList.Comments;
      return (comments != null ? comments.Select<Comment, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>((Func<Comment, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>) (comment => Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment.FromComment(requestContext, comment, projectId, 16, ((ISecuredObject) workItem).GetToken()))).ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment, int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment, int>) (workItemComment => workItemComment.CommentId), (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>) (workItemComment => workItemComment)) : (Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>) null) ?? new Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComment>();
    }

    private WorkItemArtifactUpdates<WorkItemResourceLinkInfo> GetResourceLinkUpdates(
      IVssRequestContext requestContext,
      WorkItem workItem,
      WorkItemRevision revision,
      IDictionary<DateTime, IDictionary<int, WorkItemResourceLinkInfo>> resourceLinksByRevisedDate)
    {
      List<WorkItemResourceLinkInfo> resourceLinkInfoList1 = new List<WorkItemResourceLinkInfo>();
      List<WorkItemResourceLinkInfo> resourceLinkInfoList2 = new List<WorkItemResourceLinkInfo>();
      List<WorkItemResourceLinkInfo> resourceLinkInfoList3 = new List<WorkItemResourceLinkInfo>();
      IDictionary<int, WorkItemResourceLinkInfo> dictionary1;
      if (!resourceLinksByRevisedDate.TryGetValue(revision.AuthorizedDate, out dictionary1))
      {
        foreach (WorkItemResourceLinkInfo resourceLink in revision.ResourceLinks)
        {
          IDictionary<int, WorkItemResourceLinkInfo> dictionary2;
          if (!resourceLinksByRevisedDate.TryGetValue(resourceLink.RevisedDate, out dictionary2))
          {
            dictionary2 = (IDictionary<int, WorkItemResourceLinkInfo>) new Dictionary<int, WorkItemResourceLinkInfo>();
            resourceLinksByRevisedDate.Add(resourceLink.RevisedDate, dictionary2);
          }
          if (!dictionary2.ContainsKey(resourceLink.ResourceId))
          {
            dictionary2.Add(resourceLink.ResourceId, resourceLink);
            resourceLinkInfoList1.Add(resourceLink);
          }
        }
      }
      else
      {
        foreach (WorkItemResourceLinkInfo resourceLink in revision.ResourceLinks)
        {
          if (dictionary1.ContainsKey(resourceLink.ResourceId))
          {
            resourceLinkInfoList2.Add(resourceLink);
            dictionary1.Remove(resourceLink.ResourceId);
          }
          else
          {
            IDictionary<int, WorkItemResourceLinkInfo> dictionary3;
            if (!resourceLinksByRevisedDate.TryGetValue(resourceLink.RevisedDate, out dictionary3))
            {
              dictionary3 = (IDictionary<int, WorkItemResourceLinkInfo>) new Dictionary<int, WorkItemResourceLinkInfo>();
              resourceLinksByRevisedDate.Add(resourceLink.RevisedDate, dictionary3);
            }
            if (!dictionary3.ContainsKey(resourceLink.ResourceId))
            {
              dictionary3.Add(resourceLink.ResourceId, resourceLink);
              resourceLinkInfoList1.Add(resourceLink);
            }
          }
        }
        foreach (WorkItemResourceLinkInfo resourceLinkInfo in (IEnumerable<WorkItemResourceLinkInfo>) dictionary1.Values)
          resourceLinkInfoList3.Add(resourceLinkInfo);
      }
      return new WorkItemArtifactUpdates<WorkItemResourceLinkInfo>((ISecuredObject) workItem)
      {
        Added = (IEnumerable<WorkItemResourceLinkInfo>) resourceLinkInfoList1,
        Removed = (IEnumerable<WorkItemResourceLinkInfo>) resourceLinkInfoList3,
        Updated = (IEnumerable<WorkItemResourceLinkInfo>) resourceLinkInfoList2
      };
    }

    private IEnumerable<ActivityFeedEntry> GetWorkItemLinkUpdates(
      IVssRequestContext requestContext,
      WorkItem workItem,
      WorkItemRevision revision,
      WorkItemRevision previousRevision = null)
    {
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      IEnumerable<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>> first = revision.AllLinks.Where<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, bool>) (link => link.AuthorizedDate >= revision.AuthorizedDate)).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>) (link => new Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>(link.AuthorizedDate, link)));
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> workItemLinkInfos = revision.AllLinks == null ? Enumerable.Empty<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>() : (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>) revision.AllLinks;
      if (previousRevision != null && previousRevision.AllLinks.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>())
        workItemLinkInfos = workItemLinkInfos.Union<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>((IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>) previousRevision.AllLinks);
      IEnumerable<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>> second = workItemLinkInfos.Where<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, bool>) (link => link.RevisedDate >= revision.AuthorizedDate && link.RevisedDate < revision.RevisedDate)).SelectMany<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, IEnumerable<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>>) (link => !(link.AuthorizedDate > revision.AuthorizedDate) ? (IEnumerable<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>) new Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>[1]
      {
        new Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>(link.RevisedDate, link)
      } : (IEnumerable<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>) new Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>[2]
      {
        new Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>(link.AuthorizedDate, link),
        new Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>(link.RevisedDate, link)
      }));
      foreach (IGrouping<DateTime, Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>> source1 in (IEnumerable<IGrouping<DateTime, Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>>) first.Union<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>(second).GroupBy<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>, DateTime>((Func<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>, DateTime>) (tuple => tuple.Item1)).OrderBy<IGrouping<DateTime, Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>, DateTime>((Func<IGrouping<DateTime, Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>, DateTime>) (group => group.Key)))
      {
        List<WorkItemLinkedItem> source2 = new List<WorkItemLinkedItem>();
        List<WorkItemLinkedItem> source3 = new List<WorkItemLinkedItem>();
        DateTime key = source1.Key;
        IdentityRef identityRef = (IdentityRef) null;
        foreach (Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> tuple in (IEnumerable<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>>) source1.OrderBy<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>, DateTime>((Func<Tuple<DateTime, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>, DateTime>) (tuple => tuple.Item2.AuthorizedDate)))
        {
          if (!(tuple.Item2.TargetProjectId != Guid.Empty))
          {
            revision.GetProjectGuid(witRequestContext);
          }
          else
          {
            Guid targetProjectId = tuple.Item2.TargetProjectId;
          }
          if (tuple.Item1 == tuple.Item2.AuthorizedDate)
            source2.Add(new WorkItemLinkedItem(requestContext, tuple.Item2, (ISecuredObject) workItem));
          else if (tuple.Item1 == tuple.Item2.RevisedDate)
            source3.Add(new WorkItemLinkedItem(requestContext, tuple.Item2, (ISecuredObject) workItem));
          if (identityRef == null)
            identityRef = WorkItemIdentityHelper.GetIdentityRef(WorkItemIdentityHelper.GetResolvedIdentityFromDistinctDisplayName(requestContext, tuple.Item2.RevisedBy != null ? tuple.Item2.RevisedBy : tuple.Item2.AuthorizedBy), (ISecuredObject) workItem);
        }
        bool flag1 = source2.Any<WorkItemLinkedItem>();
        bool flag2 = source3.Any<WorkItemLinkedItem>();
        if (flag1 | flag2)
          yield return new ActivityFeedEntry()
          {
            RevisionId = revision.Revision,
            RevisedBy = identityRef,
            RevisedDate = key,
            Links = new WorkItemArtifactUpdates<WorkItemLinkedItem>((ISecuredObject) workItem)
            {
              Added = flag1 ? (IEnumerable<WorkItemLinkedItem>) source2 : (IEnumerable<WorkItemLinkedItem>) null,
              Removed = flag2 ? (IEnumerable<WorkItemLinkedItem>) source3 : (IEnumerable<WorkItemLinkedItem>) null
            }
          };
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_longTextValueConverters = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetExtensions<ILongTextValueConverter>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_serviceBusEventPublisher = new WorkItemChangedEventServiceBusPublisher(systemRequestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_longTextValueConverters == null)
        return;
      this.m_longTextValueConverters.Dispose();
      this.m_longTextValueConverters = (IDisposableReadOnlyList<ILongTextValueConverter>) null;
    }

    void IDisposable.Dispose()
    {
    }

    public virtual WorkItem GetWorkItemById(
      IVssRequestContext requestContext,
      int workItemId,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      Guid? projectId = null,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      return this.GetWorkItemById(requestContext, workItemId, 16, includeResourceLinks, includeWorkItemLinks, includeHistory, workItemRetrievalMode, includeInRecentActivity, useWorkItemIdentity, projectId, includeCommentHistory, revisionsSince);
    }

    public virtual WorkItem GetWorkItemById(
      IVssRequestContext requestContext,
      int workItemId,
      int permissionsToCheck,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      Guid? projectId = null,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      return this.GetWorkItemById(requestContext, workItemId, permissionsToCheck, new DateTime?(), includeResourceLinks, includeWorkItemLinks, includeHistory, workItemRetrievalMode, includeInRecentActivity, useWorkItemIdentity, projectId, includeCommentHistory, revisionsSince);
    }

    public virtual WorkItem GetWorkItemById(
      IVssRequestContext requestContext,
      int workItemId,
      int permissionsToCheck,
      Guid projectId,
      out string securityToken,
      DateTime? asOf = null,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      return this.GetWorkItems(requestContext, (IEnumerable<int>) new int[1]
      {
        workItemId
      }, permissionsToCheck, asOf, out securityToken, (includeResourceLinks ? 1 : 0) != 0, (includeWorkItemLinks ? 1 : 0) != 0, (includeHistory ? 1 : 0) != 0, (includeTags ? 1 : 0) != 0, workItemRetrievalMode, includeInRecentActivity: (includeInRecentActivity ? 1 : 0) != 0, projectId: new Guid?(projectId), useWorkItemIdentity: (useWorkItemIdentity ? 1 : 0) != 0, includeCommentHistory: (includeCommentHistory ? 1 : 0) != 0, revisionsSince: revisionsSince).FirstOrDefault<WorkItem>();
    }

    public virtual WorkItem GetWorkItemById(
      IVssRequestContext requestContext,
      int workItemId,
      int permissionsToCheck,
      DateTime? asOf,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      Guid? projectId = null,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(workItemId, "id", 1);
      return this.GetWorkItems(requestContext, (IEnumerable<int>) new int[1]
      {
        workItemId
      }, permissionsToCheck, asOf, (includeResourceLinks ? 1 : 0) != 0, (includeWorkItemLinks ? 1 : 0) != 0, (includeHistory ? 1 : 0) != 0, workItemRetrievalMode: workItemRetrievalMode, includeInRecentActivity: (includeInRecentActivity ? 1 : 0) != 0, projectId: projectId, useWorkItemIdentity: (useWorkItemIdentity ? 1 : 0) != 0, includeCommentHistory: (includeCommentHistory ? 1 : 0) != 0, revisionsSince: revisionsSince).FirstOrDefault<WorkItem>();
    }

    public IEnumerable<WorkItem> GetDeletedWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds)
    {
      return this.GetWorkItems(requestContext, workItemIds, true, true, false, false, WorkItemRetrievalMode.Deleted, WorkItemErrorPolicy.Fail, false, false, false, new DateTime?());
    }

    public IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      return this.GetWorkItems(requestContext, workItemIds, new DateTime?(), includeResourceLinks, includeWorkItemLinks, includeHistory, includeTags, workItemRetrievalMode, errorPolicy, includeInRecentActivity, useWorkItemIdentity, includeCommentHistory, revisionsSince);
    }

    public IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      DateTime? asOf,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<int> workItemIds1 = workItemIds;
      DateTime? asOf1 = asOf;
      int num1 = includeResourceLinks ? 1 : 0;
      int num2 = includeWorkItemLinks ? 1 : 0;
      int num3 = includeHistory ? 1 : 0;
      int num4 = includeTags ? 1 : 0;
      int num5 = (int) workItemRetrievalMode;
      int errorPolicy1 = (int) errorPolicy;
      int num6 = includeInRecentActivity ? 1 : 0;
      bool flag1 = useWorkItemIdentity;
      bool flag2 = includeCommentHistory;
      DateTime? nullable = revisionsSince;
      Guid? projectId = new Guid?();
      int num7 = flag1 ? 1 : 0;
      int num8 = flag2 ? 1 : 0;
      DateTime? revisionsSince1 = nullable;
      return this.GetWorkItems(requestContext1, workItemIds1, 16, asOf1, num1 != 0, num2 != 0, num3 != 0, num4 != 0, (WorkItemRetrievalMode) num5, (WorkItemErrorPolicy) errorPolicy1, num6 != 0, projectId, num7 != 0, num8 != 0, revisionsSince1);
    }

    public IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      DateTime? asOf,
      Guid? projectId,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      return this.GetWorkItems(requestContext, workItemIds, 16, asOf, includeResourceLinks, includeWorkItemLinks, includeHistory, includeTags, workItemRetrievalMode, errorPolicy, includeInRecentActivity, projectId, useWorkItemIdentity, includeCommentHistory, revisionsSince);
    }

    public virtual IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      int permissionsToCheck,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<int> workItemIds1 = workItemIds;
      int permissionsToCheck1 = permissionsToCheck;
      DateTime? asOf = new DateTime?();
      int num1 = includeResourceLinks ? 1 : 0;
      int num2 = includeWorkItemLinks ? 1 : 0;
      int num3 = includeHistory ? 1 : 0;
      int num4 = includeTags ? 1 : 0;
      int num5 = (int) workItemRetrievalMode;
      int errorPolicy1 = (int) errorPolicy;
      int num6 = includeInRecentActivity ? 1 : 0;
      bool flag1 = useWorkItemIdentity;
      bool flag2 = includeCommentHistory;
      DateTime? nullable = revisionsSince;
      Guid? projectId = new Guid?();
      int num7 = flag1 ? 1 : 0;
      int num8 = flag2 ? 1 : 0;
      DateTime? revisionsSince1 = nullable;
      return this.GetWorkItems(requestContext1, workItemIds1, permissionsToCheck1, asOf, num1 != 0, num2 != 0, num3 != 0, num4 != 0, (WorkItemRetrievalMode) num5, (WorkItemErrorPolicy) errorPolicy1, num6 != 0, projectId, num7 != 0, num8 != 0, revisionsSince1);
    }

    public virtual IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      int permissionsToCheck,
      DateTime? asOf,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      Guid? projectId = null,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<int> workItemIds1 = workItemIds;
      int permissionsToCheck1 = permissionsToCheck;
      DateTime? asOf1 = asOf;
      string str;
      ref string local = ref str;
      int num1 = includeResourceLinks ? 1 : 0;
      int num2 = includeWorkItemLinks ? 1 : 0;
      int num3 = includeHistory ? 1 : 0;
      int num4 = includeTags ? 1 : 0;
      int num5 = (int) workItemRetrievalMode;
      int errorPolicy1 = (int) errorPolicy;
      int num6 = includeInRecentActivity ? 1 : 0;
      bool flag1 = useWorkItemIdentity;
      bool flag2 = includeCommentHistory;
      DateTime? nullable = revisionsSince;
      Guid? projectId1 = new Guid?();
      int num7 = flag1 ? 1 : 0;
      int num8 = flag2 ? 1 : 0;
      DateTime? revisionsSince1 = nullable;
      return this.GetWorkItems(requestContext1, workItemIds1, permissionsToCheck1, asOf1, out local, num1 != 0, num2 != 0, num3 != 0, num4 != 0, (WorkItemRetrievalMode) num5, (WorkItemErrorPolicy) errorPolicy1, num6 != 0, projectId1, num7 != 0, num8 != 0, revisionsSince1);
    }

    public virtual IEnumerable<WorkItem> GetWorkItemsPredicate(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      int permissionsToCheck,
      Func<IPermissionCheckHelper, int, int, bool> permissionToCheckPredicate,
      Action<int, AccessType> throwException,
      DateTime? asOf,
      out string securityToken,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      Guid? projectId = null,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      securityToken = (string) null;
      string securityTokenTemp = (string) null;
      if (asOf.HasValue)
        includeHistory = false;
      if (!includeHistory)
        revisionsSince = new DateTime?();
      if (!workItemIds.Any<int>())
        return Enumerable.Empty<WorkItem>();
      foreach (int workItemId in workItemIds)
        ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemIds), 1);
      workItemIds = (IEnumerable<int>) workItemIds.Distinct<int>().ToArray<int>();
      WorkItem[] workItemsPredicate = requestContext.TraceBlock<WorkItem[]>(904400, 904499, 904498, "Services", "WorkItemService", "GetWorkItems", (Func<WorkItem[]>) (() =>
      {
        if (includeHistory && !revisionsSince.HasValue && WorkItemTrackingFeatureFlags.IsRestrictMaxRevisionsSupportedByGetWorkItemHistoryEnabled(requestContext))
        {
          int maxRevisionsAllowed = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).MaxRevisionsSupportedByGetWorkItemHistory;
          IVssRequestContext requestContext1 = requestContext;
          IEnumerable<int> workItemIds1 = workItemIds;
          int identityDisplayType = (int) requestContext.GetIdentityDisplayType();
          int num1 = (int) workItemRetrievalMode;
          DateTime? asOf1 = asOf;
          Guid? nullable = projectId;
          bool flag = useWorkItemIdentity;
          DateTime? revisionsSince1 = new DateTime?();
          Guid? projectId1 = nullable;
          int num2 = flag ? 1 : 0;
          WorkItemDataset workItemDataset = this.GetWorkItemDataSets(requestContext1, workItemIds1, false, false, false, false, false, false, false, (IdentityDisplayType) identityDisplayType, (WorkItemRetrievalMode) num1, asOf1, revisionsSince1, projectId1, num2 != 0).FirstOrDefault<WorkItemDataset>((Func<WorkItemDataset, bool>) (wi => wi.Rev > maxRevisionsAllowed));
          if (workItemDataset != null)
            throw new TooManyRevisionsForHistoryAPIException(workItemDataset.Id, workItemDataset.Rev, maxRevisionsAllowed);
        }
        Dictionary<int, WorkItemDataset> datasets = this.GetWorkItemDataSets(requestContext, workItemIds, false, true, true, includeResourceLinks, includeWorkItemLinks, includeHistory, includeTags, requestContext.GetIdentityDisplayType(), workItemRetrievalMode, asOf, revisionsSince, projectId, useWorkItemIdentity, includeCommentHistory).ToDictionary<WorkItemDataset, int>((Func<WorkItemDataset, int>) (ds => ds.Id));
        requestContext.TraceConditionally(904401, TraceLevel.Info, "Services", "WorkItemService", (Func<string>) (() => string.Format("projectId: {0}. datasets: {1}", (object) projectId, (object) string.Join<int>(", ", (IEnumerable<int>) datasets.Keys))));
        List<WorkItem> workItemList = new List<WorkItem>();
        if (permissionsToCheck != 0)
        {
          AccessType accessType = permissionsToCheck == 16 ? AccessType.Read : AccessType.Write;
          IPermissionCheckHelper permissionChecker = requestContext.WitContext().WorkItemPermissionChecker;
          HashSet<int> intSet = new HashSet<int>();
          foreach (int workItemId in workItemIds)
          {
            int id = workItemId;
            if (!intSet.Contains(id))
            {
              WorkItemDataset dataset;
              bool canReturnWorkItem = datasets.TryGetValue(id, out dataset);
              requestContext.TraceConditionally(904401, TraceLevel.Info, "Services", "WorkItemService", (Func<string>) (() => string.Format("canReturnWorkItem: {0}. id: {1}", (object) canReturnWorkItem, (object) id)));
              if (canReturnWorkItem && !permissionToCheckPredicate(permissionChecker, dataset.LatestAreaId, permissionsToCheck))
              {
                requestContext.TraceConditionally(904401, TraceLevel.Info, "Services", "WorkItemService", (Func<string>) (() => string.Format("LatestAreaId: {0}. permissionsToCheck: {1}", (object) dataset.LatestAreaId, (object) permissionsToCheck)));
                canReturnWorkItem = false;
              }
              if (canReturnWorkItem)
              {
                canReturnWorkItem = this.TryGetSecurityToken(requestContext, projectId, dataset.LatestAreaId, out securityTokenTemp);
                requestContext.TraceConditionally(904401, TraceLevel.Info, "Services", "WorkItemService", (Func<string>) (() => string.Format("canReturnWorkItem: {0}. LatestAreaId: {1}", (object) canReturnWorkItem, (object) dataset.LatestAreaId)));
              }
              if (canReturnWorkItem)
              {
                WorkItem workItem = new WorkItem(dataset);
                workItem.SecureWorkItemWithRevisions(securityTokenTemp, permissionsToCheck);
                workItemList.Add(workItem);
                intSet.Add(id);
              }
              else
              {
                switch (errorPolicy)
                {
                  case WorkItemErrorPolicy.Fail:
                    throwException(id, accessType);
                    continue;
                  case WorkItemErrorPolicy.Omit:
                    workItemList.Add((WorkItem) null);
                    continue;
                  default:
                    continue;
                }
              }
            }
          }
        }
        return workItemList.ToArray();
      }));
      securityToken = securityTokenTemp;
      if (includeHistory || asOf.HasValue)
        TeamFoundationWorkItemService.UpdateHistoryDisabledFields(requestContext, workItemsPredicate);
      if (includeInRecentActivity)
      {
        WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
        List<(int, Guid, int)> list = ((IEnumerable<WorkItem>) workItemsPredicate).Where<WorkItem>((Func<WorkItem, bool>) (workItem => workItem != null)).Distinct<WorkItem>((IEqualityComparer<WorkItem>) new WorkItemIdComparer()).ToList<WorkItem>().Select<WorkItem, (int, Guid, int)>((Func<WorkItem, (int, Guid, int)>) (workItem => (workItem.Id, workItem.GetProjectGuid(witRequestContext), workItem.AreaId))).ToList<(int, Guid, int)>();
        Guid id = witRequestContext.RequestIdentity.Id;
        requestContext.GetService<WorkItemRecentActivityService>().TryFireWorkItemRecentActivityEvent(requestContext, WorkItemRecentActivityType.Visited, (IReadOnlyCollection<(int, Guid, int)>) list, id, DateTime.UtcNow);
      }
      WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, WorkItemTelemetry.Feature, (object) workItemsPredicate, (object) includeResourceLinks, (object) includeWorkItemLinks, (object) includeHistory, (object) includeTags);
      WorkItemKpiTracer.TraceKpi(requestContext, (WorkItemTrackingKpi) new GetWorkItemKpi(requestContext, workItemsPredicate, includeHistory), (WorkItemTrackingKpi) new CountKpi(requestContext, "GetWorkItemCount", workItemsPredicate.Length));
      return (IEnumerable<WorkItem>) workItemsPredicate;
    }

    public virtual IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      int permissionsToCheck1,
      DateTime? asOf,
      out string securityToken,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      Guid? projectId = null,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null)
    {
      return this.GetWorkItemsPredicate(requestContext, workItemIds, permissionsToCheck1, (Func<IPermissionCheckHelper, int, int, bool>) ((permissionsHelper, areaId, permissionsToCheck2) => permissionsHelper.HasWorkItemPermission(areaId, permissionsToCheck2)), (Action<int, AccessType>) ((id, accessType) =>
      {
        throw new WorkItemUnauthorizedAccessException(id, accessType);
      }), asOf, out securityToken, includeResourceLinks, includeWorkItemLinks, includeHistory, includeTags, workItemRetrievalMode, errorPolicy, includeInRecentActivity, projectId, useWorkItemIdentity, includeCommentHistory, revisionsSince);
    }

    public WorkItem GetWorkItemTemplate(
      IVssRequestContext requestContext,
      string projectName,
      string workItemTypeReferenceName,
      bool useWorkItemIdentity = false)
    {
      return requestContext.TraceBlock<WorkItem>(904651, 904660, 904655, "Services", "WorkItemService", nameof (GetWorkItemTemplate), (Func<WorkItem>) (() =>
      {
        WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
        Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext.Elevate(), projectName);
        TreeNode treeNode = trackingRequestContext.TreeService.GetTreeNode(projectId, projectId);
        WorkItemType typeByReferenceName = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(requestContext, treeNode.CssNodeId, workItemTypeReferenceName);
        AdditionalWorkItemTypeProperties additionalProperties = typeByReferenceName.GetAdditionalProperties(requestContext);
        WorkItem workItemTemplate = new WorkItem((IDictionary<int, object>) new Dictionary<int, object>()
        {
          {
            25,
            (object) typeByReferenceName.Name
          },
          {
            -2,
            (object) treeNode.Id
          },
          {
            -104,
            (object) treeNode.Id
          }
        });
        IRuleEvaluationContext evaluationContext = workItemTemplate.GetRuleEvaluationContext(requestContext);
        new RuleEngine(additionalProperties.FieldRules, RuleEngineConfiguration.ServerFull).Evaluate(evaluationContext);
        bool flag1 = requestContext.WitContext().WorkItemPermissionChecker.HasWorkItemPermission(workItemTemplate.AreaId, 256);
        if (useWorkItemIdentity)
        {
          string token;
          bool flag2 = flag1 & this.TryGetSecurityToken(requestContext, new Guid?(projectId), workItemTemplate.AreaId, out token);
          foreach (KeyValuePair<int, object> keyValuePair in workItemTemplate.Updates.ToList<KeyValuePair<int, object>>())
          {
            if (keyValuePair.Value is string && evaluationContext.IsIdentityField(keyValuePair.Key))
            {
              string distinctDisplayName1 = (string) keyValuePair.Value;
              WorkItemIdentity distinctDisplayName2 = WorkItemIdentityHelper.GetResolvedIdentityFromDistinctDisplayName(requestContext, distinctDisplayName1);
              if (distinctDisplayName2 != null)
              {
                distinctDisplayName2.SecurityToken = token;
                distinctDisplayName2.HasPermission = flag2;
                workItemTemplate.Updates[keyValuePair.Key] = (object) distinctDisplayName2;
              }
              else
              {
                CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
                CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
                intelligenceData.Add("NotResolvedWorkitem", (double) workItemTemplate.Id);
                IVssRequestContext requestContext1 = requestContext;
                string workItemTracking = CustomerIntelligenceArea.WorkItemTracking;
                CustomerIntelligenceData properties = intelligenceData;
                service.Publish(requestContext1, workItemTracking, nameof (GetWorkItemTemplate), properties);
                if (!(!string.IsNullOrEmpty(distinctDisplayName1) & flag2))
                  throw new WorkItemIdentityNotResolvedException();
                WorkItemIdentity workItemIdentity = new WorkItemIdentity()
                {
                  DistinctDisplayName = distinctDisplayName1,
                  IdentityRef = (IdentityRef) new ConstantIdentityRef(token, 16, Microsoft.Azure.Boards.CssNodes.AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid)
                  {
                    Descriptor = new SubjectDescriptor(),
                    DisplayName = distinctDisplayName1
                  }
                };
              }
            }
          }
        }
        else if (!flag1)
        {
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("Error", Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.IdentityEmailNotViewable());
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, nameof (GetWorkItemTemplate), properties);
        }
        return workItemTemplate;
      }));
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment GetWorkItemComment(
      IVssRequestContext requestContext,
      int workItemId,
      int revision)
    {
      return this.GetWorkItemCommentImpl(requestContext, new Guid?(), workItemId, revision);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment GetWorkItemComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int revision)
    {
      return this.GetWorkItemCommentImpl(requestContext, new Guid?(projectId), workItemId, revision);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments GetWorkItemComments(
      IVssRequestContext requestContext,
      int workItemId,
      int fromRevision,
      int count,
      CommentSortOrder sort)
    {
      return this.GetWorkItemCommentsImpl(requestContext, new Guid?(), workItemId, fromRevision, count, sort);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments GetWorkItemComments(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int fromRevision,
      int count,
      CommentSortOrder sort)
    {
      return this.GetWorkItemCommentsImpl(requestContext, new Guid?(projectId), workItemId, fromRevision, count, sort);
    }

    public void EvaluateRulesOnFieldValues(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeNameOrId,
      IEnumerable<int> fieldsToEvaluate,
      IDictionary<int, object> fieldValues,
      IDictionary<int, object> fieldUpdates)
    {
      requestContext.TraceBlock(904661, 904670, 904665, "Services", "WorkItemService", nameof (EvaluateRulesOnFieldValues), (Action) (() =>
      {
        WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
        IFieldTypeDictionary fieldDictionary = trackingRequestContext.FieldDictionary;
        IWorkItemTypeService service = requestContext.GetService<IWorkItemTypeService>();
        WorkItemType typeByReferenceName = service.GetWorkItemTypeByReferenceName(requestContext, projectId, workItemTypeNameOrId);
        WorkItemFieldData workItemFieldData = new WorkItemFieldData(fieldValues, projectId);
        IRuleEvaluationContext evaluationContext1 = workItemFieldData.GetRuleEvaluationContext(requestContext);
        RuleEngine ruleEngine = new RuleEngine(service.GetWorkItemTypeByReferenceName(requestContext, projectId, typeByReferenceName.Name).GetAdditionalProperties(requestContext).FieldRules, RuleEngineConfiguration.ServerValidationOnly);
        List<TeamFoundationServiceException> serviceExceptionList = new List<TeamFoundationServiceException>();
        this.SetIdentityEvaluationContext(requestContext, fieldUpdates, fieldValues, evaluationContext1, serviceExceptionList);
        if (serviceExceptionList.Any<TeamFoundationServiceException>())
          throw serviceExceptionList.First<TeamFoundationServiceException>();
        workItemFieldData.SetFieldUpdates(requestContext, (IEnumerable<KeyValuePair<int, object>>) fieldUpdates);
        IRuleEvaluationContext evaluationContext2 = evaluationContext1;
        IEnumerable<int> fieldIds = fieldsToEvaluate;
        ruleEngine.Evaluate(evaluationContext2, fieldIds);
        if (evaluationContext1.FirstFieldRequiresPendingCheck.HasValue)
        {
          IEnumerable<PendingSetMembershipCheckRecord> pendingSetMembershipChecks = this.PreparePendingSetMembershipCheck(0, (IEnumerable<FieldRuleEvalutionStatus>) evaluationContext1.RuleEvaluationStatuses.Values);
          using (WorkItemComponent component = trackingRequestContext.RequestContext.CreateComponent<WorkItemComponent>())
          {
            WorkItemUpdateResultSet itemUpdateResultSet = component.ValidatePendingSetMembershipChecks(pendingSetMembershipChecks);
            if (itemUpdateResultSet.SetMembershipCheckResults.Any<PendingSetMembershipCheckResultRecord>())
              serviceExceptionList.AddRange((IEnumerable<TeamFoundationServiceException>) itemUpdateResultSet.SetMembershipCheckResults.Select<PendingSetMembershipCheckResultRecord, RuleValidationException>((Func<PendingSetMembershipCheckResultRecord, RuleValidationException>) (result => new RuleValidationException(fieldDictionary.GetField(result.FieldId).ReferenceName, fieldDictionary.GetField(result.FieldId).Name))));
          }
        }
        IEnumerable<FieldRuleEvalutionStatus> source = evaluationContext1.RuleEvaluationStatuses.Values.Where<FieldRuleEvalutionStatus>((Func<FieldRuleEvalutionStatus, bool>) (status => (status.Flags & FieldStatusFlags.InvalidMask) != 0));
        serviceExceptionList.AddRange((IEnumerable<TeamFoundationServiceException>) source.Select<FieldRuleEvalutionStatus, RuleValidationException>((Func<FieldRuleEvalutionStatus, RuleValidationException>) (status => new RuleValidationException(fieldDictionary.GetField(status.FieldId).ReferenceName, fieldDictionary.GetField(status.FieldId).Name, status.Flags, status.Value))));
        if (!serviceExceptionList.Any<TeamFoundationServiceException>())
          return;
        if (serviceExceptionList.Count > 1)
          throw new WorkItemTrackingAggregateException((IEnumerable<TeamFoundationServiceException>) serviceExceptionList)
          {
            LeadingException = serviceExceptionList.FirstOrDefault<TeamFoundationServiceException>()
          };
        throw serviceExceptionList.First<TeamFoundationServiceException>();
      }));
    }

    public virtual bool CanAccessWorkItem(
      IVssRequestContext requestContext,
      int workItemId,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted)
    {
      return this.HasWorkItemPermissionImpl(requestContext, workItemId, workItemRetrievalMode, 16, out Guid? _, out string _);
    }

    public virtual bool CanAccessWorkItem(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      WorkItemRetrievalMode workItemRetrievalMode,
      out string securityToken)
    {
      string token = (string) null;
      int num = requestContext.TraceBlock<bool>(904920, 904921, "Services", "WorkItemService", nameof (CanAccessWorkItem), (Func<bool>) (() => this.HasWorkItemPermissionImpl(requestContext, workItemId, workItemRetrievalMode, 16, out Guid? _, out token))) ? 1 : 0;
      securityToken = token;
      return num != 0;
    }

    public virtual bool CanAccessWorkItem(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      WorkItemRetrievalMode workItemRetrievalMode)
    {
      return this.HasWorkItemPermissionImpl(requestContext, workItemId, workItemRetrievalMode, 16, out Guid? _, out string _);
    }

    public virtual bool CanAccessWorkItem(
      IVssRequestContext requestContext,
      int workItemId,
      bool checkProjectReadPermission,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted)
    {
      int num = 16;
      int? areaId;
      Guid? projectId;
      if (!this.HasWorkItemPermissionImplInternal(requestContext, workItemId, workItemRetrievalMode, num, out areaId, out projectId, out string _))
        return false;
      return !checkProjectReadPermission || !areaId.HasValue || !projectId.HasValue || requestContext.GetService<WorkItemTrackingTreeService>().HasAreaPathPermissions(requestContext, requestContext.GetUserIdentity(), projectId.Value, areaId.Value, num);
    }

    public virtual bool HasWorkItemPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      WorkItemRetrievalMode workItemRetrievalMode,
      int permission,
      out string securityToken)
    {
      string token = (string) null;
      int num = requestContext.TraceBlock<bool>(904922, 904923, "Services", "WorkItemService", nameof (HasWorkItemPermission), (Func<bool>) (() => this.HasWorkItemPermissionImpl(requestContext, workItemId, workItemRetrievalMode, permission, out Guid? _, out token))) ? 1 : 0;
      securityToken = token;
      return num != 0;
    }

    public IEnumerable<WorkItemReactionsCount> GetSortedWorkItemReactionsCount(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      SocialEngagementType socialEngagementType)
    {
      return (IEnumerable<WorkItemReactionsCount>) requestContext.TraceBlock<IList<WorkItemReactionsCount>>(904933, 904934, "Services", "WorkItemService", nameof (GetSortedWorkItemReactionsCount), (Func<IList<WorkItemReactionsCount>>) (() =>
      {
        using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
          return component.GetSortedWorkItemReactionsCount(workItemIds, socialEngagementType);
      }));
    }

    public WorkItemChangesResult GetWorkItemsAndLinksChangedDate(
      IVssRequestContext requestContext,
      int linkType)
    {
      return requestContext.TraceBlock<WorkItemChangesResult>(904943, 904944, "Services", "WorkItemService", nameof (GetWorkItemsAndLinksChangedDate), (Func<WorkItemChangesResult>) (() =>
      {
        using (WorkItemChangesComponent component = requestContext.CreateComponent<WorkItemChangesComponent>())
          return component.GetWorkItemsAndLinksChangedDate(linkType);
      }));
    }

    public int GetWorkItemUpdateId(IVssRequestContext requestContext, int workItemId)
    {
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        return component.GetWorkItemUpdateId(workItemId);
    }

    internal bool TryGetNextStatesOnCheckin(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      out IList<WorkItemStateOnTransition> workItemNextStates)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemIds, nameof (workItemIds));
      IEnumerable<WorkItem> workItems = this.GetWorkItems(requestContext, workItemIds, false, false, false, false, WorkItemRetrievalMode.NonDeleted, WorkItemErrorPolicy.Omit, false, false, false, new DateTime?());
      return this.TryGetNextStatesOnCheckin(requestContext, workItems, out workItemNextStates);
    }

    internal bool TryGetNextStatesOnCheckin(
      IVssRequestContext requestContext,
      IEnumerable<WorkItem> workItems,
      out IList<WorkItemStateOnTransition> workItemNextStates)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItems, nameof (workItems));
      workItemNextStates = (IList<WorkItemStateOnTransition>) new List<WorkItemStateOnTransition>();
      IProcessWorkItemTypeService service1 = requestContext.GetService<IProcessWorkItemTypeService>();
      IWorkItemTrackingProcessService service2 = requestContext.GetService<IWorkItemTrackingProcessService>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      foreach (WorkItem workItem in workItems.Where<WorkItem>((Func<WorkItem, bool>) (wi => wi != null)))
      {
        WorkItemStateOnTransition nextState = (WorkItemStateOnTransition) null;
        Guid projectGuid = workItem.GetProjectGuid(requestContext);
        ProcessDescriptor processDescriptor1 = (ProcessDescriptor) null;
        BaseWorkItemType wit = (BaseWorkItemType) null;
        bool processDescriptor2 = service2.TryGetLatestProjectProcessDescriptor(requestContext1, projectGuid, out processDescriptor1);
        IWorkItemStateDefinitionService service3 = requestContext.GetService<IWorkItemStateDefinitionService>();
        if (!processDescriptor2 || service1.TryGetWorkItemTypeByName(requestContext, processDescriptor1.TypeId, workItem.WorkItemType, out wit))
        {
          if (processDescriptor2 && !processDescriptor1.IsCustom)
          {
            service3.TryGetNextStateOnCheckinForInheritedProcess(requestContext, processDescriptor1.TypeId, wit.ReferenceName, workItem.State, out nextState);
            nextState.WorkItemId = workItem.Id;
            if (nextState.ErrorMessage != null)
              nextState.ErrorMessage = string.Format(nextState.ErrorMessage, (object) workItem.Id.ToString());
            workItemNextStates.Add(nextState);
          }
          else
          {
            this.TryGetNextStateOnCheckinForLegacyWit(requestContext, workItem, out nextState);
            workItemNextStates.Add(nextState);
          }
        }
      }
      return workItemNextStates.Count > 0;
    }

    private bool TryGetNextStateOnCheckinForLegacyWit(
      IVssRequestContext requestContext,
      WorkItem workItem,
      out WorkItemStateOnTransition nextState)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItem>(workItem, nameof (workItem));
      CustomerIntelligenceService service1 = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Current State Name", workItem.State);
      nextState = new WorkItemStateOnTransition();
      try
      {
        IWorkItemTypeService service2 = requestContext.GetService<IWorkItemTypeService>();
        WorkItemType workItemType1 = (WorkItemType) null;
        IVssRequestContext requestContext1 = requestContext;
        Guid projectGuid = workItem.GetProjectGuid(requestContext);
        string workItemType2 = workItem.WorkItemType;
        ref WorkItemType local = ref workItemType1;
        if (!service2.TryGetWorkItemTypeByName(requestContext1, projectGuid, workItemType2, out local))
          return false;
        properties.Add("Workitemtype Reference Name", workItemType1.ReferenceName);
        foreach (KeyValuePair<WorkItemTypeTransition, HashSet<string>> action in (IEnumerable<KeyValuePair<WorkItemTypeTransition, HashSet<string>>>) workItemType1.GetAdditionalProperties(requestContext)?.Actions)
        {
          string str = action.Value.Where<string>((Func<string, bool>) (ac => TFStringComparer.WorkItemActionName.Equals(ac, "Microsoft.VSTS.Actions.Checkin"))).FirstOrDefault<string>();
          if (str != null)
          {
            WorkItemTypeTransition key = action.Key;
            properties.Add("Action Value", str);
            if (TFStringComparer.WorkItemStateName.Equals(workItem.State, key.From))
            {
              nextState.WorkItemId = workItem.Id;
              nextState.NextStateName = key.To;
              properties.Add("Next State Name", nextState.NextStateName);
              break;
            }
          }
        }
        if (nextState.NextStateName == null)
        {
          nextState.WorkItemId = workItem.Id;
          nextState.ErrorMessage = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemHasNoValidTransitionXml((object) workItem.WorkItemType, (object) workItem.Id, (object) workItem.State);
          nextState.ErrorCode = "VS1640134";
          properties.Add("Next State Errorcode", nextState.ErrorCode);
        }
        properties.Add("Result ", nextState.NextStateName != "");
        service1.Publish(requestContext, "WorkItemStateDefinitionService", nameof (TryGetNextStateOnCheckinForLegacyWit), properties);
      }
      catch (ProcessWorkItemTypeDoesNotExistException ex)
      {
        requestContext.TraceException(910501, "Services", "WorkItemStateDefinitionService", (Exception) ex);
        return false;
      }
      catch (ProcessWorkItemTypeNotFoundException ex)
      {
        requestContext.TraceException(910501, "Services", "WorkItemStateDefinitionService", (Exception) ex);
        return false;
      }
      return nextState.NextStateName != null;
    }

    public IEnumerable<WorkItemStateOnTransition> GetNextStateOnCheckinWithExceptions(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds)
    {
      IList<WorkItemStateOnTransition> workItemNextStates = (IList<WorkItemStateOnTransition>) null;
      if (!this.TryGetNextStatesOnCheckin(requestContext, workItemIds, out workItemNextStates))
        return Enumerable.Empty<WorkItemStateOnTransition>();
      IList<int> intList = (IList<int>) new List<int>();
      List<WorkItemUpdate> workItemUpdateList = new List<WorkItemUpdate>();
      foreach (WorkItemStateOnTransition stateOnTransition in (IEnumerable<WorkItemStateOnTransition>) workItemNextStates)
      {
        if (stateOnTransition.NextStateName != null)
        {
          intList.Add(stateOnTransition.WorkItemId);
          WorkItemUpdate workItemUpdate = new WorkItemUpdate()
          {
            Id = stateOnTransition.WorkItemId,
            Fields = (IEnumerable<KeyValuePair<string, object>>) new List<KeyValuePair<string, object>>()
            {
              new KeyValuePair<string, object>(2.ToString(), (object) stateOnTransition.NextStateName)
            }
          };
          workItemUpdateList.Add(workItemUpdate);
        }
      }
      if (workItemUpdateList.Count > 0)
      {
        foreach (WorkItemUpdateResult updateWorkItem in this.UpdateWorkItems(requestContext, (IEnumerable<WorkItemUpdate>) workItemUpdateList, WorkItemUpdateRuleExecutionMode.Full, false, true, false, (IReadOnlyCollection<int>) null, false, false, false))
        {
          WorkItemUpdateResult wiResult = updateWorkItem;
          if (wiResult.Exception != null && !string.IsNullOrEmpty(wiResult.Exception.Message))
          {
            WorkItemStateOnTransition stateOnTransition = workItemNextStates.Where<WorkItemStateOnTransition>((Func<WorkItemStateOnTransition, bool>) (nextStateObj => nextStateObj.WorkItemId == wiResult.Id)).First<WorkItemStateOnTransition>();
            IVssRequestContext requestContext1 = requestContext;
            List<int> workItemIds1 = new List<int>();
            workItemIds1.Add(stateOnTransition.WorkItemId);
            DateTime? revisionsSince = new DateTime?();
            IEnumerable<WorkItem> workItems = this.GetWorkItems(requestContext1, (IEnumerable<int>) workItemIds1, false, false, false, false, WorkItemRetrievalMode.NonDeleted, WorkItemErrorPolicy.Omit, false, false, false, revisionsSince);
            stateOnTransition.NextStateName = (string) null;
            stateOnTransition.ErrorMessage = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemRulePreventingTransition((object) "VS1640136", (object) workItems.First<WorkItem>().WorkItemType, (object) wiResult.Id, (object) wiResult.Exception.Message);
            stateOnTransition.ErrorCode = "VS1640136";
          }
        }
      }
      return (IEnumerable<WorkItemStateOnTransition>) workItemNextStates;
    }

    protected virtual IPermissionCheckHelper GetPermissionCheckHelper(
      IVssRequestContext requestContext)
    {
      return requestContext.WitContext().WorkItemPermissionChecker;
    }

    private IEnumerable<WorkItemDataset> GetWorkItemDataSets(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      bool includeCountFields,
      bool includeCustomFields,
      bool includeTextFields,
      bool includeResourceLinks,
      bool includeWorkItemLinks,
      bool includeHistory,
      bool includeTags,
      IdentityDisplayType identityDisplayType,
      WorkItemRetrievalMode workItemRetrievalMode,
      DateTime? asOf = null,
      DateTime? revisionsSince = null,
      Guid? projectId = null,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false)
    {
      return requestContext.TraceBlock<IEnumerable<WorkItemDataset>>(904150, 904169, 904168, "Services", "WorkItemService", nameof (GetWorkItemDataSets), (Func<IEnumerable<WorkItemDataset>>) (() =>
      {
        if (asOf.HasValue && !CommonWITUtils.HasReadHistoricalWorkItemResourcesPermission(requestContext))
          throw new WorkItemUnauthorizedHistoricalDataAccessException();
        if (!workItemIds.Any<int>())
          return Enumerable.Empty<WorkItemDataset>();
        workItemIds = (IEnumerable<int>) workItemIds.Distinct<int>().ToArray<int>();
        WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
        bool includeComments = includeHistory && WorkItemTrackingFeatureFlags.IsCommentServiceReadsFromNewStorageEnabled(requestContext);
        bool includeCommentHistory1 = includeComments & includeCommentHistory;
        IEnumerable<WorkItemDataset> datasets;
        using (WorkItemComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
        {
          using (requestContext.AcquireExemptionLock())
            datasets = (IEnumerable<WorkItemDataset>) replicaAwareComponent.GetWorkItemDatasets(workItemIds, includeCountFields, includeCustomFields, includeTextFields, includeResourceLinks, includeWorkItemLinks, includeHistory, true, witRequestContext.ServerSettings.MaxLongTextSize, witRequestContext.ServerSettings.MaxRevisionLongTextSize, identityDisplayType, asOf, revisionsSince, includeComments, includeCommentHistory1).ToList<WorkItemDataset>();
        }
        if (!CommonWITUtils.IsRemoteLinkingEnabled(requestContext))
        {
          WorkItemTrackingLinkService linkService = witRequestContext.LinkService;
          foreach (WorkItemRevisionDataset itemRevisionDataset in datasets)
            itemRevisionDataset.AllLinks.RemoveAll((Predicate<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>) (link => !linkService.TryGetLinkTypeById(requestContext, link.LinkType, out MDWorkItemLinkType _)));
        }
        if (workItemRetrievalMode != WorkItemRetrievalMode.All)
        {
          bool isDeleted = workItemRetrievalMode == WorkItemRetrievalMode.Deleted;
          datasets = datasets.Where<WorkItemDataset>((Func<WorkItemDataset, bool>) (workItem =>
          {
            if (workItem.Fields.GetValueOrDefault<bool>(-404) == isDeleted)
              return true;
            if (!asOf.HasValue)
              return false;
            WorkItemRevisionDataset itemRevisionDataset = workItem.Revisions.FirstOrDefault<WorkItemRevisionDataset>();
            return itemRevisionDataset != null && itemRevisionDataset.LatestIsDeleted == isDeleted;
          }));
          if (isDeleted)
            datasets = datasets.Where<WorkItemDataset>((Func<WorkItemDataset, bool>) (workItem => !CommonWITUtils.IsTestWorkItem(requestContext, workItem.ProjectId, workItem.Fields.GetValueOrDefault<string>(25))));
          datasets = (IEnumerable<WorkItemDataset>) datasets.ToList<WorkItemDataset>();
        }
        if (includeTextFields)
        {
          foreach (WorkItemDataset workItemData in datasets)
            this.FixDatasetTextFieldValues(requestContext, workItemData);
        }
        IPermissionCheckHelper permChecker = requestContext.WitContext().WorkItemPermissionChecker;
        if (useWorkItemIdentity)
        {
          HashSet<WorkItemIdentity> processedWorkItemIdentities = new HashSet<WorkItemIdentity>();
          IDictionary<Guid, WorkItemIdentity> identityMap = WorkItemIdentityHelper.EnsureVsidToWorkItemIdentityMap(requestContext, (IEnumerable<Guid>) datasets.SelectMany<WorkItemDataset, Guid>((Func<WorkItemDataset, IEnumerable<Guid>>) (dataset => dataset.IdentityFields.Values.Concat<Guid>(dataset.Revisions.SelectMany<WorkItemRevisionDataset, Guid>((Func<WorkItemRevisionDataset, IEnumerable<Guid>>) (rev => (IEnumerable<Guid>) rev.IdentityFields.Values))).Concat<Guid>(dataset.AllLinks.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Guid>) (link => link.AuthorizedByTfid))).Concat<Guid>(dataset.Revisions.SelectMany<WorkItemRevisionDataset, Guid>((Func<WorkItemRevisionDataset, IEnumerable<Guid>>) (rev => rev.AllLinks.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Guid>) (link => link.AuthorizedByTfid))))).Concat<Guid>(dataset.AllLinks.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Guid>) (link => link.RevisedByTfid))).Concat<Guid>(dataset.Revisions.SelectMany<WorkItemRevisionDataset, Guid>((Func<WorkItemRevisionDataset, IEnumerable<Guid>>) (rev => rev.AllLinks.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Guid>) (link => link.RevisedByTfid))))))).Distinct<Guid>().ToList<Guid>());
          datasets.ForEach<WorkItemDataset>((Action<WorkItemDataset>) (dataset =>
          {
            int latestAreaId = dataset.LatestAreaId;
            bool emailReadable = permChecker.HasWorkItemPermission(latestAreaId, 256);
            processedWorkItemIdentities.AddRange<WorkItemIdentity, HashSet<WorkItemIdentity>>((IEnumerable<WorkItemIdentity>) this.ReplaceWithWorkItemIdentity(requestContext, projectId, (WorkItemFieldValues) dataset, identityMap, latestAreaId, emailReadable, dataset.AllLinks));
            foreach (WorkItemRevisionDataset revision in dataset.Revisions)
              processedWorkItemIdentities.AddRange<WorkItemIdentity, HashSet<WorkItemIdentity>>((IEnumerable<WorkItemIdentity>) this.ReplaceWithWorkItemIdentity(requestContext, projectId, (WorkItemFieldValues) revision, identityMap, latestAreaId, emailReadable, revision.AllLinks));
          }));
          WorkItemIdentityHelper.AddIdentitiesToDistinctDisplayNameMap(requestContext, (IEnumerable<WorkItemIdentity>) processedWorkItemIdentities);
        }
        else
          datasets.ForEach<WorkItemDataset>((Action<WorkItemDataset>) (dataset =>
          {
            if (permChecker.HasWorkItemPermission(dataset.LatestAreaId, 256))
              return;
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add("Error", Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.IdentityEmailNotViewable());
            requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, nameof (GetWorkItemDataSets), properties);
          }));
        if (includeTags && datasets.Any<WorkItemDataset>())
          requestContext.TraceBlock(904160, 904164, 904163, "Services", "WorkItemService", "GetWorkItemDataSets.PopulateTags", (Action) (() =>
          {
            ITeamFoundationTaggingService service = requestContext.GetService<ITeamFoundationTaggingService>();
            if (includeHistory || asOf.HasValue)
            {
              VersionedTagArtifact<int>[] array = datasets.SelectMany<WorkItemDataset, VersionedTagArtifact<int>>((Func<WorkItemDataset, IEnumerable<VersionedTagArtifact<int>>>) (dataset =>
              {
                // ISSUE: variable of a compiler-generated type
                TeamFoundationWorkItemService.\u003C\u003Ec__DisplayClass192_1 displayClass1921 = this;
                WorkItemDataset dataset2 = dataset;
                int id = dataset2.Id;
                List<VersionedTagArtifact<int>> workItemDataSets = new List<VersionedTagArtifact<int>>(dataset2.Revisions.Count + 1);
                workItemDataSets.Add(new VersionedTagArtifact<int>(this.GetDataspaceIdentifier(witRequestContext, dataset2.LatestAreaId, dataset2.ProjectId), id, dataset2.Rev));
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                workItemDataSets.AddRange(dataset2.Revisions.Select<WorkItemRevisionDataset, VersionedTagArtifact<int>>((Func<WorkItemRevisionDataset, VersionedTagArtifact<int>>) (revision => new VersionedTagArtifact<int>(displayClass1921.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.GetDataspaceIdentifier(displayClass1921.witRequestContext, dataset2.LatestAreaId, revision.ProjectId), id, revision.Rev))));
                return (IEnumerable<VersionedTagArtifact<int>>) workItemDataSets;
              })).ToArray<VersionedTagArtifact<int>>();
              ILookup<int, ArtifactTags<int>> lookup = service.GetTagsForVersionedArtifacts<int>(requestContext.Elevate(), WorkItemArtifactKinds.WorkItem, (ICollection<VersionedTagArtifact<int>>) array).ToLookup<ArtifactTags<int>, int>((Func<ArtifactTags<int>, int>) (artifact => artifact.Artifact.Id));
              foreach (WorkItemDataset workItemDataset in datasets)
              {
                IEnumerable<ArtifactTags<int>> artifactTagses = lookup[workItemDataset.Id];
                int rev = workItemDataset.Rev;
                Dictionary<int, WorkItemRevisionDataset> dictionary = (Dictionary<int, WorkItemRevisionDataset>) null;
                foreach (ArtifactTags<int> artifactTags in artifactTagses)
                {
                  int version = artifactTags.Artifact.Version;
                  if (version == rev)
                  {
                    workItemDataset.Fields[80] = (object) TeamFoundationWorkItemService.TagDefinitionsToString(artifactTags.Tags);
                  }
                  else
                  {
                    if (dictionary == null)
                      dictionary = workItemDataset.Revisions.ToDictionary<WorkItemRevisionDataset, int>((Func<WorkItemRevisionDataset, int>) (revision => revision.Rev));
                    WorkItemRevisionDataset itemRevisionDataset;
                    if (dictionary.TryGetValue(version, out itemRevisionDataset))
                      itemRevisionDataset.Fields[80] = (object) TeamFoundationWorkItemService.TagDefinitionsToString(artifactTags.Tags);
                  }
                }
              }
            }
            else
            {
              VersionedTagArtifact<int>[] array3 = datasets.Select<WorkItemDataset, VersionedTagArtifact<int>>((Func<WorkItemDataset, VersionedTagArtifact<int>>) (dataset =>
              {
                Guid projectId2 = witRequestContext.TreeService.LegacyGetTreeNode(dataset.LatestAreaId).ProjectId;
                return new VersionedTagArtifact<int>(this.GetDataspaceIdentifier(witRequestContext, dataset.LatestAreaId, dataset.ProjectId), dataset.Id, dataset.Rev);
              })).ToArray<VersionedTagArtifact<int>>();
              ILookup<int, ArtifactTags<int>> lookup = service.GetTagsForVersionedArtifacts<int>(requestContext.Elevate(), WorkItemArtifactKinds.WorkItem, (ICollection<VersionedTagArtifact<int>>) array3).ToLookup<ArtifactTags<int>, int>((Func<ArtifactTags<int>, int>) (artifact => artifact.Artifact.Id));
              foreach (WorkItemDataset workItemDataset in datasets)
              {
                TagDefinition[] array4 = lookup[workItemDataset.Id].SelectMany<ArtifactTags<int>, TagDefinition>((Func<ArtifactTags<int>, IEnumerable<TagDefinition>>) (a => a.Tags)).ToArray<TagDefinition>();
                if (array4.Length != 0)
                  workItemDataset.Fields[80] = (object) TeamFoundationWorkItemService.TagDefinitionsToString((IEnumerable<TagDefinition>) array4);
              }
            }
          }));
        if (!includeCountFields & includeWorkItemLinks && witRequestContext.FieldDictionary.TryGetField(-35, out FieldEntry _))
          datasets.ForEach<WorkItemDataset>((Action<WorkItemDataset>) (dataset => this.AddParentFieldToDataSet(dataset)));
        return datasets;
      }));
    }

    private void AddParentFieldToDataSet(WorkItemDataset dataset)
    {
      this.AddParentFieldToRevisionDataSet((WorkItemRevisionDataset) dataset);
      if (dataset.Revisions == null)
        return;
      dataset.Revisions.ForEach((Action<WorkItemRevisionDataset>) (revDataset => this.AddParentFieldToRevisionDataSet(revDataset)));
    }

    private void AddParentFieldToRevisionDataSet(WorkItemRevisionDataset dataset)
    {
      if (dataset == null || dataset.WorkItemLinks == null)
        return;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo workItemLinkInfo = dataset.WorkItemLinks.FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, bool>) (li => li.LinkType == -2));
      if (workItemLinkInfo == null)
        return;
      dataset.Fields[-35] = (object) workItemLinkInfo.TargetId;
    }

    private HashSet<WorkItemIdentity> ReplaceWithWorkItemIdentity(
      IVssRequestContext requestContext,
      Guid? projectId,
      WorkItemFieldValues fieldValues,
      IDictionary<Guid, WorkItemIdentity> identityMap,
      int latestAreaId,
      bool emailReadable,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> workItemLinks = null)
    {
      HashSet<WorkItemIdentity> processedWorkItemIdentities = new HashSet<WorkItemIdentity>();
      string token;
      emailReadable &= this.TryGetSecurityToken(requestContext, projectId, latestAreaId, out token);
      fieldValues.IdentityFields.ForEach<KeyValuePair<int, Guid>>((Action<KeyValuePair<int, Guid>>) (idField =>
      {
        if (fieldValues.Fields[idField.Key] is WorkItemIdentity)
          return;
        fieldValues.Fields[idField.Key] = (object) this.PopulateWorkItemIdentity(requestContext, identityMap, (string) fieldValues.Fields[idField.Key], idField.Value, token, emailReadable, processedWorkItemIdentities);
      }));
      workItemLinks?.ForEach((Action<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>) (link =>
      {
        this.PopulateWorkItemIdentity(requestContext, identityMap, link.AuthorizedBy, link.AuthorizedByTfid, token, emailReadable, processedWorkItemIdentities);
        this.PopulateWorkItemIdentity(requestContext, identityMap, link.RevisedBy, link.RevisedByTfid, token, emailReadable, processedWorkItemIdentities);
      }));
      return processedWorkItemIdentities;
    }

    private WorkItemIdentity PopulateWorkItemIdentity(
      IVssRequestContext requestContext,
      IDictionary<Guid, WorkItemIdentity> identityMap,
      string distinctDisplayName,
      Guid tfId,
      string token,
      bool emailReadable,
      HashSet<WorkItemIdentity> processedWorkItemIdentities)
    {
      WorkItemIdentity workItemIdentity1 = (WorkItemIdentity) null;
      bool flag = false;
      if (tfId != Guid.Empty && identityMap.TryGetValue(tfId, out workItemIdentity1))
      {
        workItemIdentity1.DistinctDisplayName = distinctDisplayName;
        flag = true;
      }
      else if (tfId != Guid.Empty && workItemIdentity1 == null)
      {
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("NotResolvedVsid", (object) tfId);
        IVssRequestContext requestContext1 = requestContext;
        string workItemTracking = CustomerIntelligenceArea.WorkItemTracking;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, workItemTracking, nameof (PopulateWorkItemIdentity), properties);
      }
      if (!flag)
      {
        string str = emailReadable || tfId == Guid.Empty ? distinctDisplayName : "UnknownUser";
        if (!string.IsNullOrEmpty(str))
        {
          WorkItemIdentity workItemIdentity2 = new WorkItemIdentity();
          workItemIdentity2.DistinctDisplayName = str;
          ConstantIdentityRef constantIdentityRef = new ConstantIdentityRef(token, 16, Microsoft.Azure.Boards.CssNodes.AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
          constantIdentityRef.Descriptor = new SubjectDescriptor();
          constantIdentityRef.DisplayName = str;
          workItemIdentity2.IdentityRef = (IdentityRef) constantIdentityRef;
          workItemIdentity1 = workItemIdentity2;
        }
        else
          workItemIdentity1 = (WorkItemIdentity) null;
      }
      if (workItemIdentity1 != null)
      {
        workItemIdentity1.HasPermission = emailReadable;
        workItemIdentity1.SecurityToken = token;
        processedWorkItemIdentities.Add(workItemIdentity1);
      }
      return workItemIdentity1;
    }

    private Guid GetDataspaceIdentifier(
      WorkItemTrackingRequestContext witRequestContext,
      int latestAreaId,
      Guid projectId)
    {
      return projectId == Guid.Empty ? witRequestContext.TreeService.LegacyGetTreeNode(latestAreaId).ProjectId : projectId;
    }

    private void ProcessWorkItemComment(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment comment,
      Guid? projectGuid,
      string securityToken,
      int requiredPermissions)
    {
      comment.Text = this.ReplaceTextFieldTokens(requestContext, comment.Text, projectGuid);
      if (this.m_longTextValueConverters != null)
      {
        foreach (ILongTextValueConverter textValueConverter in (IEnumerable<ILongTextValueConverter>) this.m_longTextValueConverters)
          comment.Text = textValueConverter.AfterRead(requestContext, "System.History", comment.Text);
      }
      comment.SetSecuredToken(securityToken);
      comment.SetRequiredPermissions(requiredPermissions);
    }

    private void FixDatasetTextFieldValues(
      IVssRequestContext requestContext,
      WorkItemDataset workItemData)
    {
      this.FixTextFieldValues(requestContext, (WorkItemFieldValues) workItemData);
      foreach (WorkItemRevisionDataset revision in workItemData.Revisions)
        this.FixTextFieldValues(requestContext, (WorkItemFieldValues) revision);
    }

    internal virtual string ReplaceTextFieldTokens(
      IVssRequestContext requestContext,
      string text,
      Guid? projectGuid)
    {
      foreach (KeyValuePair<char, string> keyValuePair in requestContext.WitContext().GetOrAddCacheItem<KeyValuePair<char, string>[]>("TokensToReplace", (Func<KeyValuePair<char, string>[]>) (() =>
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        string str = service.LocationForAccessMapping(requestContext, "AttachmentDownload", "TestManagement", service.DetermineAccessMapping(requestContext));
        string absoluteUri = TeamFoundationWorkItemService.GetProjectAttachmentUri(requestContext, projectGuid).AbsoluteUri;
        return new KeyValuePair<char, string>[3]
        {
          new KeyValuePair<char, string>('\a', absoluteUri),
          new KeyValuePair<char, string>('\u0006', absoluteUri),
          new KeyValuePair<char, string>('\b', str)
        };
      })))
        text = text.ReplaceCharWithString(keyValuePair.Key, keyValuePair.Value);
      return text;
    }

    private void FixTextFieldValues(
      IVssRequestContext requestContext,
      WorkItemFieldValues fieldValues)
    {
      IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
      Dictionary<int, string> dictionary = new Dictionary<int, string>();
      foreach (KeyValuePair<int, object> field1 in fieldValues.Fields)
      {
        if (field1.Value is WorkItemLargeTextValue itemLargeTextValue)
        {
          string str = itemLargeTextValue.Text;
          FieldEntry field2;
          if (!itemLargeTextValue.IsProcessed && fieldDictionary.TryGetField(field1.Key, out field2))
          {
            if (field2.IsHtml && !string.IsNullOrWhiteSpace(str))
              str = this.ReplaceTextFieldTokens(requestContext, str, new Guid?(fieldValues.ProjectId));
            if (field2.IsHtml != itemLargeTextValue.IsHtml)
              str = field2.IsHtml ? HtmlConverter.ConvertToHtml((object) str, true) : HtmlConverter.ConvertToPlainText(str);
            if (this.m_longTextValueConverters != null)
            {
              foreach (ILongTextValueConverter textValueConverter in (IEnumerable<ILongTextValueConverter>) this.m_longTextValueConverters)
                str = textValueConverter.AfterRead(requestContext, field2.ReferenceName, str);
            }
            itemLargeTextValue.Text = str;
            itemLargeTextValue.IsProcessed = true;
          }
          dictionary[field1.Key] = str;
        }
      }
      WorkItemCommentVersionRecord itemCommentVersion = fieldValues.WorkItemCommentVersion;
      if (!string.IsNullOrEmpty(itemCommentVersion?.OriginalText))
        itemCommentVersion.OriginalText = this.ReplaceTextFieldTokens(requestContext, itemCommentVersion.OriginalText, new Guid?(fieldValues.ProjectId));
      if (!string.IsNullOrEmpty(itemCommentVersion?.Text))
        itemCommentVersion.Text = this.ReplaceTextFieldTokens(requestContext, itemCommentVersion.Text, new Guid?(fieldValues.ProjectId));
      foreach (KeyValuePair<int, string> keyValuePair in dictionary)
        fieldValues.Fields[keyValuePair.Key] = (object) keyValuePair.Value;
    }

    private static bool IsExtensionField(int fieldId, IEnumerable<WorkItemTypeExtension> extensions) => extensions != null && extensions.Any<WorkItemTypeExtension>((Func<WorkItemTypeExtension, bool>) (e => e.MarkerField.Field.FieldId == fieldId || e.Fields.Any<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (ef => ef.Field.FieldId == fieldId))));

    private static string TagDefinitionsToString(IEnumerable<TagDefinition> tags) => Microsoft.TeamFoundation.Core.WebApi.TaggingHelper.FormatTagsValue((IEnumerable<string>) (tags ?? Enumerable.Empty<TagDefinition>()).Select<TagDefinition, string>((Func<TagDefinition, string>) (td => td.Name)).OrderBy<string, string>((Func<string, string>) (x => x)));

    private static void UpdateHistoryDisabledFields(
      IVssRequestContext requestContext,
      WorkItem[] workItems)
    {
      if (!WorkItemTrackingFeatureFlags.IsNoHistoryEnabledFieldsSupported(requestContext))
        return;
      ISet<int> historyDisabledFieldIds = requestContext.WitContext().FieldDictionary.GetHistoryDisabledFieldIds();
      foreach (WorkItem workItem in ((IEnumerable<WorkItem>) workItems).Where<WorkItem>((Func<WorkItem, bool>) (wi => wi != null)))
      {
        if (workItem.LatestData.Any<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (pair => historyDisabledFieldIds.Contains(pair.Key))) || workItem.IdentitityFields.Any<KeyValuePair<int, Guid>>((Func<KeyValuePair<int, Guid>, bool>) (pair => historyDisabledFieldIds.Contains(pair.Key))))
        {
          IDictionary<int, object> dictionary1 = (IDictionary<int, object>) workItem.LatestData.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (pair => historyDisabledFieldIds.Contains(pair.Key))).ToDictionary<KeyValuePair<int, object>, int, object>((Func<KeyValuePair<int, object>, int>) (pair => pair.Key), (Func<KeyValuePair<int, object>, object>) (pair => pair.Value));
          IDictionary<int, Guid> dictionary2 = (IDictionary<int, Guid>) workItem.IdentitityFields.Where<KeyValuePair<int, Guid>>((Func<KeyValuePair<int, Guid>, bool>) (pair => historyDisabledFieldIds.Contains(pair.Key))).ToDictionary<KeyValuePair<int, Guid>, int, Guid>((Func<KeyValuePair<int, Guid>, int>) (pair => pair.Key), (Func<KeyValuePair<int, Guid>, Guid>) (pair => pair.Value));
          foreach (WorkItemFieldData revision in (IEnumerable<WorkItemRevision>) workItem.Revisions)
          {
            if (dictionary1.Count > 0)
              revision.LatestData = TeamFoundationWorkItemService.MergeOrOverwriteFieldsData<object>((IDictionary<int, object>) revision.LatestData, dictionary1);
            if (dictionary2.Count > 0)
              revision.IdentitityFields = TeamFoundationWorkItemService.MergeOrOverwriteFieldsData<Guid>((IDictionary<int, Guid>) revision.IdentitityFields, dictionary2);
          }
        }
      }
    }

    private static ReadOnlyDictionary<int, T> MergeOrOverwriteFieldsData<T>(
      IDictionary<int, T> mergeIntoData,
      IDictionary<int, T> mergeFromData)
    {
      Dictionary<int, T> dictionary = new Dictionary<int, T>(mergeIntoData);
      foreach (KeyValuePair<int, T> keyValuePair in (IEnumerable<KeyValuePair<int, T>>) mergeFromData)
        dictionary[keyValuePair.Key] = keyValuePair.Value;
      return new ReadOnlyDictionary<int, T>((IDictionary<int, T>) dictionary);
    }

    private void SetIdentityEvaluationContext(
      IVssRequestContext requestContext,
      IDictionary<int, object> fieldUpdates,
      IDictionary<int, object> latestValues,
      IRuleEvaluationContext evaluationContext,
      List<TeamFoundationServiceException> exceptions)
    {
      requestContext.TraceBlock(904671, 904680, 904675, "Services", "WorkItemService", nameof (SetIdentityEvaluationContext), (Action) (() =>
      {
        IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> dictionary = (IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        ResolvedIdentityNamesInfo identityNamesInfo1 = new ResolvedIdentityNamesInfo();
        IEnumerable<WorkItemFieldInvalidException> exceptions1 = (IEnumerable<WorkItemFieldInvalidException>) new List<WorkItemFieldInvalidException>();
        ResolvedIdentityNamesInfo identityNamesInfo2 = requestContext.GetService<IWorkItemIdentityService>().ResolveIdentityFields(requestContext.WitContext(), fieldUpdates, latestValues, out exceptions1);
        IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap = this.ResolveReferencedIdentities(requestContext.WitContext(), identityNamesInfo2);
        exceptions.AddRange((IEnumerable<TeamFoundationServiceException>) exceptions1);
        evaluationContext.UpdateIdentityMap(identityMap);
        evaluationContext.SetResolvedIdentityNamesInfo(identityNamesInfo2);
      }));
    }

    private Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment GetWorkItemCommentImpl(
      IVssRequestContext requestContext,
      Guid? projectId,
      int workItemId,
      int revision)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      ArgumentUtility.CheckForOutOfRange(revision, nameof (revision), 1);
      if (projectId.HasValue)
        ArgumentUtility.CheckForEmptyGuid(projectId.Value, nameof (projectId));
      string securityToken = (string) null;
      if (!this.HasWorkItemPermissionImpl(requestContext, workItemId, WorkItemRetrievalMode.All, 16, out projectId, out securityToken))
        throw new WorkItemNotFoundException(workItemId);
      return requestContext.TraceBlock<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment>(904880, 904882, 904881, "Services", "WorkItemService", (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment>) (() =>
      {
        int num = WorkItemTrackingFeatureFlags.IsLegacyCommentsApiNewStorageEnabled(requestContext) ? 1 : 0;
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment comment = (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment) null;
        if (num != 0)
        {
          using (WorkItemCommentComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemCommentComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
            comment = replicaAwareComponent.GetWorkItemComment(workItemId, revision);
        }
        else
        {
          using (WorkItemComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
            comment = replicaAwareComponent.GetWorkItemComment(workItemId, revision);
        }
        if (comment != null)
        {
          this.ProcessWorkItemComment(requestContext, comment, projectId, securityToken, 16);
          comment.ProjectId = projectId;
        }
        return comment;
      }), nameof (GetWorkItemCommentImpl));
    }

    private Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments GetWorkItemCommentsImpl(
      IVssRequestContext requestContext,
      Guid? projectId,
      int workItemId,
      int fromRevision,
      int count,
      CommentSortOrder sort)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      ArgumentUtility.CheckForOutOfRange(fromRevision, nameof (fromRevision), 1);
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 1);
      if (projectId.HasValue)
        ArgumentUtility.CheckForEmptyGuid(projectId.Value, nameof (projectId));
      string securityToken = (string) null;
      if (!this.HasWorkItemPermissionImpl(requestContext, workItemId, WorkItemRetrievalMode.All, 16, out projectId, out securityToken))
        throw new WorkItemNotFoundException(workItemId);
      return requestContext.TraceBlock<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments>(904870, 904879, 904878, "Services", "WorkItemService", (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments>) (() =>
      {
        int num = WorkItemTrackingFeatureFlags.IsLegacyCommentsApiNewStorageEnabled(requestContext) ? 1 : 0;
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments itemCommentsImpl = (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComments) null;
        if (num != 0)
        {
          using (WorkItemCommentComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemCommentComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
            itemCommentsImpl = replicaAwareComponent.GetWorkItemComments(workItemId, fromRevision, count, sort);
        }
        else
        {
          using (WorkItemComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
            itemCommentsImpl = replicaAwareComponent.GetWorkItemComments(workItemId, fromRevision, count, sort);
        }
        if (itemCommentsImpl != null && itemCommentsImpl.Comments != null)
        {
          foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemComment comment in itemCommentsImpl.Comments)
            this.ProcessWorkItemComment(requestContext, comment, projectId, securityToken, 16);
          itemCommentsImpl.SetSecuredToken(securityToken);
          itemCommentsImpl.SetRequiredPermissions(16);
          itemCommentsImpl.ProjectId = projectId;
        }
        return itemCommentsImpl;
      }), nameof (GetWorkItemCommentsImpl));
    }

    protected virtual bool HasWorkItemPermissionImpl(
      IVssRequestContext requestContext,
      int workItemId,
      WorkItemRetrievalMode workItemRetrievalMode,
      int permissionToCheck,
      out Guid? projectId,
      out string securityToken)
    {
      return this.HasWorkItemPermissionImplInternal(requestContext, workItemId, workItemRetrievalMode, permissionToCheck, out int? _, out projectId, out securityToken);
    }

    private bool HasWorkItemPermissionImplInternal(
      IVssRequestContext requestContext,
      int workItemId,
      WorkItemRetrievalMode workItemRetrievalMode,
      int permissionToCheck,
      out int? areaId,
      out Guid? projectId,
      out string securityToken)
    {
      IEnumerable<WorkItemFieldData> workItemFieldValues = this.GetWorkItemFieldValues(requestContext, (IEnumerable<int>) new int[1]
      {
        workItemId
      }, (IEnumerable<string>) new string[1]
      {
        "System.AreaId"
      }, permissionToCheck, new DateTime?(), 1, workItemRetrievalMode, false, true);
      areaId = new int?();
      projectId = new Guid?();
      securityToken = (string) null;
      if (workItemFieldValues == null || !workItemFieldValues.Any<WorkItemFieldData>())
        return false;
      if (workItemFieldValues.Count<WorkItemFieldData>() == 1)
      {
        areaId = new int?(workItemFieldValues.First<WorkItemFieldData>().AreaId);
        projectId = new Guid?(this.GetProjectId(requestContext, areaId.Value));
        return this.TryGetSecurityToken(requestContext, projectId, areaId.Value, out securityToken);
      }
      requestContext.Trace(904914, TraceLevel.Error, "Services", "WorkItemService", string.Format("Found {0} total areaIds for workItem with id: {1}", (object) workItemFieldValues.Count<WorkItemFieldData>(), (object) workItemId));
      return true;
    }

    private bool TryGetSecurityToken(
      IVssRequestContext requestContext,
      Guid? projectId,
      int areaId,
      out string token)
    {
      if (projectId.HasValue && !CommonWITUtils.CanAccessCrossProjectWorkItems(requestContext))
      {
        Guid projectId1 = this.GetProjectId(requestContext, areaId);
        Guid? nullable = projectId;
        if ((nullable.HasValue ? (projectId1 != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
        {
          token = (string) null;
          return false;
        }
      }
      IPermissionCheckHelper permissionCheckHelper = this.GetPermissionCheckHelper(requestContext);
      token = permissionCheckHelper.GetWorkItemSecurityToken(areaId);
      return true;
    }

    private Guid GetProjectId(IVssRequestContext requestContext, int areaId) => requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNode(requestContext, areaId).ProjectId;

    internal class UrlReplacementMap
    {
      public UrlReplacementMap()
      {
        this.UrlsToFix = (IDictionary<char, string>) new Dictionary<char, string>();
        this.UrlsToReplace = (IDictionary<string, char>) new Dictionary<string, char>();
        this.UrlsToReplaceUsingRegex = (IDictionary<Regex, char>) new Dictionary<Regex, char>();
      }

      public IDictionary<string, char> UrlsToReplace { get; private set; }

      public IDictionary<Regex, char> UrlsToReplaceUsingRegex { get; private set; }

      public IDictionary<char, string> UrlsToFix { get; private set; }
    }

    private struct TagUpdateDescriptor
    {
      internal WorkItemUpdateState UpdateState;
      internal Guid ProjectId;
      internal IEnumerable<string> TagNames;
    }
  }
}
