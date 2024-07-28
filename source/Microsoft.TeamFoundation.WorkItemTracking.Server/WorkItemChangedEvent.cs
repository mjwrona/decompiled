// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemChangedEvent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.RemoteWorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Web;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [NotificationEventBindings(EventSerializerType.Xml, "ms.vss-work.workitem-changed-event")]
  public class WorkItemChangedEvent
  {
    private string workItemId;
    private string workItemType;
    private string portfolioProjectField;
    private string projectNodeIdField;
    private string[] eventSubtypes;
    private string areaPathField;
    private string titleField;
    private string workItemTitleField;
    private string subscriberField;
    private string changerSidField;
    private string changerTeamFoundationIdField;
    private string changerDisplayNameField;
    private string displayUrlField;
    private string timeZoneField;
    private string timeZoneOffsetField;
    private bool suppressNotification;
    private bool hasOnlyLinkUpdates;
    private ChangeTypes changeTypeField;
    private DateTime updateDate;
    private CoreFieldsType coreFieldsField;
    private TextField[] textFieldsField;
    private ChangedFieldsType changedFieldsField;
    private FieldValue[] fieldValues;
    private AddedFile[] addedFilesField;
    private DeletedFile[] deletedFilesField;
    private AddedResourceLink[] addedResourceLinksField;
    private DeletedResourceLink[] deletedResourceLinksField;
    private ChangedResourceLink[] changedResourceLinksField;
    private AddedRelation[] addedRelationsField;
    private DeletedRelation[] deletedRelationsField;
    private ChangedRelation[] changedRelationsField;
    private ResourceTypeData[] addedResourceLinkTypes;
    private ResourceTypeData[] changedResourceLinkTypes;
    private ResourceTypeData[] deletedResourceLinkTypes;
    private string artifactUri;
    private List<EventActor> actors;
    private DateTime changedDateUtc;
    private Guid projectId;
    private string changerIpAddress;
    private WorkItemCommentData workItemComment;

    internal static IEnumerable<WorkItemChangedEventExtended> TryFireWorkItemChangedEvents(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemChangedEventServiceBusPublisher serviceBusEventPublisher,
      IWorkItemChangedEventNotifier updateEventNotifier,
      IDictionary<int, WorkItemFieldData> fieldDataMap,
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      IEnumerable<WorkItemUpdateState> updateStates,
      bool suppressNotifications = false,
      Microsoft.VisualStudio.Services.Identity.Identity changerIdentity = null)
    {
      List<WorkItemChangedEventExtended> eventServiceEvents = (List<WorkItemChangedEventExtended>) null;
      if (updateStates.Any<WorkItemUpdateState>())
        witRequestContext.RequestContext.TraceBlock(904290, 904299, 904298, "Services", "WorkItemService", nameof (TryFireWorkItemChangedEvents), (Action) (() =>
        {
          if (witRequestContext.ServerSettings.EventsEnabled)
          {
            try
            {
              eventServiceEvents = WorkItemChangedEvent.CreateWorkItemChangedEvents(witRequestContext, updateStates, fieldDataMap, identityMap, changerIdentity ?? witRequestContext.RequestIdentity, suppressNotifications).ToList<WorkItemChangedEventExtended>();
              witRequestContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(witRequestContext.RequestContext, (TeamFoundationTaskCallback) ((requestContext, ignored) =>
              {
                // ISSUE: variable of a compiler-generated type
                WorkItemChangedEvent.\u003C\u003Ec__DisplayClass0_0 cDisplayClass00 = this;
                IVssRequestContext requestContext2 = requestContext;
                requestContext2.TraceBlock(909709, 909710, 909711, "Services", "WorkItemService", "WorkItemChangedEventsCallBack", (Action) (() =>
                {
                  ITeamFoundationEventService service = requestContext2.GetService<ITeamFoundationEventService>();
                  service.PublishNotification(requestContext2, (object) new WorkItemsChangedNotification());
                  // ISSUE: reference to a compiler-generated field
                  foreach (WorkItemChangedEventExtended eventServiceEvent in cDisplayClass00.eventServiceEvents)
                    service.PublishNotification(requestContext2, (object) eventServiceEvent.LegacyChangedEvent);
                  if (requestContext2.IsFeatureEnabled("WorkItemTracking.Server.FireWorkItemsChangedWithExtensionsBatchEvent"))
                  {
                    // ISSUE: reference to a compiler-generated field
                    WorkItemsChangedWithExtensionsBatchEvent extensionsBatchEvent = WorkItemChangedEvent.CreateWorkItemsChangedWithExtensionsBatchEvent(requestContext2, (IEnumerable<WorkItemChangedEventExtended>) cDisplayClass00.eventServiceEvents);
                    if (extensionsBatchEvent.Changes.Any<WorkItemChangedEventExtended>())
                      service.PublishNotification(requestContext2, (object) extensionsBatchEvent);
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  cDisplayClass00.updateEventNotifier.NotifyChanges(requestContext2, (IEnumerable<WorkItemChangedEventExtended>) cDisplayClass00.eventServiceEvents);
                  if (!requestContext2.ExecutionEnvironment.IsHostedDeployment || !requestContext2.IsFeatureEnabled("WorkItemTracking.Server.EnableFireEventsToServiceBus"))
                    return;
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  cDisplayClass00.serviceBusEventPublisher.TrySendNotificationsToServiceBus(requestContext2, cDisplayClass00.eventServiceEvents);
                }));
              }));
            }
            catch (Exception ex)
            {
              TeamFoundationEventLog.Default.LogException(witRequestContext.RequestContext, "Fire Work Item failed to queue event", ex, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
            }
          }
          else
            witRequestContext.RequestContext.Trace(904295, TraceLevel.Info, "Services", "WorkItemService", "Events not enabled, no WorkItemChangedEvents fired");
        }));
      return (IEnumerable<WorkItemChangedEventExtended>) eventServiceEvents;
    }

    internal static IEnumerable<WorkItemChangedEventExtended> TryFireWorkItemDestroyedEvents(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemChangedEventServiceBusPublisher serviceBusEventPublisher,
      Dictionary<int, Guid> workItemIdProjectIdDictionary)
    {
      List<WorkItemChangedEventExtended> eventServiceEvents = (List<WorkItemChangedEventExtended>) null;
      witRequestContext.RequestContext.TraceBlock(904501, 904502, 904503, "Services", "WorkItemService", nameof (TryFireWorkItemDestroyedEvents), (Action) (() =>
      {
        if (witRequestContext.ServerSettings.EventsEnabled)
        {
          try
          {
            eventServiceEvents = WorkItemChangedEvent.CreateWorkItemChangedEventsForWorkItemDestroyServiceBusNotifications(workItemIdProjectIdDictionary).ToList<WorkItemChangedEventExtended>();
            witRequestContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(witRequestContext.RequestContext, (TeamFoundationTaskCallback) ((requestContext, ignored) =>
            {
              // ISSUE: variable of a compiler-generated type
              WorkItemChangedEvent.\u003C\u003Ec__DisplayClass1_0 cDisplayClass10 = this;
              IVssRequestContext requestContext2 = requestContext;
              requestContext2.TraceBlock(909721, 909722, 909723, "Services", "WorkItemService", "WorkItemDestroyedEventsCallBack", (Action) (() =>
              {
                if (!requestContext2.ExecutionEnvironment.IsHostedDeployment || !requestContext2.IsFeatureEnabled("WorkItemTracking.Server.EnableFireEventsToServiceBus"))
                  return;
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                cDisplayClass10.serviceBusEventPublisher.TrySendNotificationsToServiceBus(requestContext2, cDisplayClass10.eventServiceEvents, "workitem.destroyed");
              }));
            }));
          }
          catch (Exception ex)
          {
            TeamFoundationEventLog.Default.LogException(witRequestContext.RequestContext, "Fire Work Item failed to queue event", ex, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
          }
        }
        else
          witRequestContext.RequestContext.Trace(904295, TraceLevel.Info, "Services", "WorkItemService", "Events not enabled, no WorkItemChangedEvents fired");
      }));
      return (IEnumerable<WorkItemChangedEventExtended>) eventServiceEvents;
    }

    internal static IEnumerable<WorkItemChangedEventExtended> CreateWorkItemChangedEvents(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates,
      IDictionary<int, WorkItemFieldData> fieldDataDict,
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      Microsoft.VisualStudio.Services.Identity.Identity changerIdentity,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>(identityMap, nameof (identityMap));
      bool includeAllFieldValues = witRequestContext.RequestContext.IsFeatureEnabled("WorkItemTracking.Server.FireWorkItemsChangedWithAllCustomFields");
      return (IEnumerable<WorkItemChangedEventExtended>) witRequestContext.RequestContext.TraceBlock<WorkItemChangedEventExtended[]>(909712, 909713, 909714, "Services", "WorkItemService", nameof (CreateWorkItemChangedEvents), (Func<WorkItemChangedEventExtended[]>) (() =>
      {
        ISet<int> intSet = (ISet<int>) new HashSet<int>(updateStates.Select<WorkItemUpdateState, int>((Func<WorkItemUpdateState, int>) (us => us.UpdateResult.Id)));
        IEnumerable<WorkItemLinkUpdateResult> linkUpdateResults = updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.HasLinkUpdates)).SelectMany<WorkItemUpdateState, WorkItemLinkUpdateResult>((Func<WorkItemUpdateState, IEnumerable<WorkItemLinkUpdateResult>>) (us => (IEnumerable<WorkItemLinkUpdateResult>) us.UpdateResult.LinkUpdates)).Where<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (lu => lu.TargetWorkItemId > 0 && !witRequestContext.LinkService.GetLinkTypeById(witRequestContext.RequestContext, lu.LinkType).IsRemote));
        WorkItemTrackingLinkService linkService = witRequestContext.LinkService;
        IDictionary<int, WorkItemUpdateState> dictionary = (IDictionary<int, WorkItemUpdateState>) new Dictionary<int, WorkItemUpdateState>();
        foreach (WorkItemLinkUpdateResult linkUpdateResult in linkUpdateResults)
        {
          if (!intSet.Contains(linkUpdateResult.TargetWorkItemId))
          {
            int targetWorkItemId = linkUpdateResult.TargetWorkItemId;
            WorkItemUpdateState workItemUpdateState;
            if (!dictionary.TryGetValue(targetWorkItemId, out workItemUpdateState))
            {
              workItemUpdateState = new WorkItemUpdateState(witRequestContext, new WorkItemUpdate()
              {
                Id = targetWorkItemId
              })
              {
                FieldData = fieldDataDict[targetWorkItemId],
                AddedTags = Enumerable.Empty<TagDefinition>(),
                CurrentExtensions = Array.Empty<WorkItemTypeExtension>(),
                CurrentTags = Enumerable.Empty<TagDefinition>(),
                OldExtensions = Array.Empty<WorkItemTypeExtension>(),
                RemovedTags = Enumerable.Empty<TagDefinition>(),
                UpdateDate = linkUpdateResult.ChangedDate
              };
              dictionary.Add(targetWorkItemId, workItemUpdateState);
            }
            MDWorkItemLinkType linkTypeById = linkService.GetLinkTypeById(witRequestContext.RequestContext, linkUpdateResult.LinkType);
            int num = linkTypeById.ForwardId == linkUpdateResult.LinkType ? linkTypeById.ReverseId : linkTypeById.ForwardId;
            List<WorkItemLinkUpdateResult> linkUpdates = workItemUpdateState.UpdateResult.LinkUpdates;
            linkUpdates.Add(new WorkItemLinkUpdateResult()
            {
              ChangeBy = linkUpdateResult.ChangeBy,
              ChangedDate = linkUpdateResult.ChangedDate,
              CorrelationId = linkUpdateResult.CorrelationId,
              LinkType = num,
              SourceWorkItemId = linkUpdateResult.TargetWorkItemId,
              TargetWorkItemId = linkUpdateResult.SourceWorkItemId,
              UpdateType = linkUpdateResult.UpdateType,
              UpdateTypeExecuted = linkUpdateResult.UpdateTypeExecuted
            });
          }
        }
        return updateStates.Union<WorkItemUpdateState>((IEnumerable<WorkItemUpdateState>) dictionary.Values).Select<WorkItemUpdateState, WorkItemChangedEventExtended>((Func<WorkItemUpdateState, WorkItemChangedEventExtended>) (us => WorkItemChangedEvent.CreateWorkItemChangedEvent(witRequestContext, changerIdentity, identityMap, us, includeAllFieldValues, suppressNotifications))).ToArray<WorkItemChangedEventExtended>();
      }));
    }

    internal static WorkItemsChangedWithExtensionsBatchEvent CreateWorkItemsChangedWithExtensionsBatchEvent(
      IVssRequestContext vssRequestContext,
      IEnumerable<WorkItemChangedEventExtended> workItemChangedEvents)
    {
      WorkItemsChangedWithExtensionsBatchEvent extensionsBatchEvent = new WorkItemsChangedWithExtensionsBatchEvent();
      IEnumerable<WorkItemChangedEventExtended> source = workItemChangedEvents.Where<WorkItemChangedEventExtended>((Func<WorkItemChangedEventExtended, bool>) (x => !x.CurrentExtensionIds.IsNullOrEmpty<Guid>() || !x.OldExtensionIds.IsNullOrEmpty<Guid>()));
      int num = source.Count<WorkItemChangedEventExtended>();
      int count = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/WorkItemsChangedWithExtensionsBatchEventMaxSize", true, 200);
      extensionsBatchEvent.Changes = source.Take<WorkItemChangedEventExtended>(count).ToList<WorkItemChangedEventExtended>();
      if (num > count)
      {
        extensionsBatchEvent.OriginalNumberOfEvents = new int?(num);
        vssRequestContext.Trace(904296, TraceLevel.Info, "Services", "WorkItemService", "Events pruned from " + num.ToString() + " to " + count.ToString());
      }
      return extensionsBatchEvent;
    }

    private static WorkItemChangedEventExtended CreateWorkItemChangedEvent(
      WorkItemTrackingRequestContext witRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity changerIdentity,
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      WorkItemUpdateState updateState,
      bool includeAllFieldValues,
      bool suppressNotifications)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>>(identityMap, nameof (identityMap));
      ArgumentUtility.CheckForNull<WorkItemUpdateState>(updateState, nameof (updateState));
      ArgumentUtility.CheckForNull<WorkItemFieldData>(updateState.FieldData, "FieldData");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<WorkItemResourceLinkUpdate> source = updateState.HasResourceLinkUpdates ? updateState.Update.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdate>(WorkItemChangedEvent.\u003C\u003EO.\u003C0\u003E__IsFileAttachment ?? (WorkItemChangedEvent.\u003C\u003EO.\u003C0\u003E__IsFileAttachment = new Func<WorkItemResourceLinkUpdate, bool>(WorkItemChangedEvent.IsFileAttachment))) : Enumerable.Empty<WorkItemResourceLinkUpdate>();
      IEnumerable<WorkItemResourceLinkUpdateResult> resourceLinkUpdatesExceptFiles = updateState.HasResourceLinkUpdates ? updateState.UpdateResult.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (r => !WorkItemChangedEvent.IsFileAttachment(r))) : Enumerable.Empty<WorkItemResourceLinkUpdateResult>();
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
      List<FieldEntry> list1 = ((IEnumerable<int>) CoreFieldConstants.AlertableCoreFields).Select<int, FieldEntry>((Func<int, FieldEntry>) (fid => fieldDictionary.GetField(fid))).ToList<FieldEntry>();
      FieldEntry field1;
      if (fieldDictionary.TryGetField(-35, out field1))
        list1.Add(field1);
      WorkItemFieldData fieldData = updateState.FieldData;
      int nodeId;
      TreeNode projectNode = fieldData.GetProjectNode(witRequestContext, out nodeId);
      string projectNodeId = projectNode != null ? projectNode.CssNodeId.ToString("D") : throw new NodeDoesNotExistException(nodeId.ToString());
      WorkItemChangedEvent.UpdateLinkCounts(requestContext, updateState);
      WorkItemChangedEvent.UpdateParentField(requestContext, updateState);
      IEnumerable<KeyValuePair<FieldEntry, object>> keyValuePairs1 = fieldData.GetUpdatesByFieldEntry(witRequestContext, true).Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (up => (up.Key.Usage & InternalFieldUsages.WorkItemTypeExtension) == InternalFieldUsages.None));
      IEnumerable<KeyValuePair<FieldEntry, object>> keyValuePairs2 = (IEnumerable<KeyValuePair<FieldEntry, object>>) null;
      if (includeAllFieldValues)
        keyValuePairs2 = fieldData.GetAllFieldValuesByFieldEntry(witRequestContext, true).Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (up =>
        {
          if ((up.Key.Usage & InternalFieldUsages.WorkItemTypeExtension) != InternalFieldUsages.None)
            return false;
          return !up.Key.IsLongText || TFStringComparer.WorkItemFieldReferenceName.Equals(up.Key.ReferenceName, "System.Tags");
        }));
      List<EventActor> actors = new List<EventActor>();
      int id = fieldData.Id;
      string artifactUri = WorkItemArtifactUriHelper.GetArtifactUri(id.ToString());
      Dictionary<Guid, string> remoteUrlMap = new Dictionary<Guid, string>();
      WorkItemChangedEvent itemChangedEvent1 = new WorkItemChangedEvent();
      id = fieldData.Id;
      itemChangedEvent1.WorkItemId = id.ToString();
      itemChangedEvent1.WorkItemType = fieldData.WorkItemType;
      itemChangedEvent1.ArtifactUri = artifactUri;
      itemChangedEvent1.AreaPath = "\\" + fieldData.GetAreaPath(requestContext);
      itemChangedEvent1.WorkItemTitle = fieldData.Title;
      itemChangedEvent1.PortfolioProject = projectNode.GetName(requestContext);
      itemChangedEvent1.ChangeType = updateState.Update.IsNew ? ChangeTypes.New : ChangeTypes.Change;
      itemChangedEvent1.TimeZone = System.TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now) ? System.TimeZone.CurrentTimeZone.DaylightName : System.TimeZone.CurrentTimeZone.StandardName;
      itemChangedEvent1.TimeZoneOffset = TFCommonUtil.GetLocalTimeZoneOffset();
      itemChangedEvent1.ChangerSid = changerIdentity.Descriptor.Identifier;
      itemChangedEvent1.ChangerTeamFoundationId = changerIdentity.Id.ToString("D");
      itemChangedEvent1.ChangerIpAddress = requestContext.RemoteIPAddress();
      itemChangedEvent1.ChangerDisplayName = changerIdentity.DisplayName;
      itemChangedEvent1.ProjectNodeId = projectNodeId;
      itemChangedEvent1.CoreFields = new CoreFieldsType()
      {
        IntegerFields = list1.Where<FieldEntry>((Func<FieldEntry, bool>) (field => WorkItemChangedEvent.IsIntegerField(field))).Select<FieldEntry, IntegerField>((Func<FieldEntry, IntegerField>) (stringField => WorkItemChangedEvent.GetIntegerField(witRequestContext, fieldData, stringField, updateState.Update.IsNew))).ToArray<IntegerField>(),
        StringFields = list1.Where<FieldEntry>((Func<FieldEntry, bool>) (field => WorkItemChangedEvent.IsStringField(field))).Select<FieldEntry, StringField>((Func<FieldEntry, StringField>) (stringField => WorkItemChangedEvent.GetStringField(witRequestContext, identityMap, fieldData, stringField, updateState.Update.IsNew, actors, WorkItemChangedEvent.GetTimeZone(requestContext)))).ToArray<StringField>(),
        BooleanFields = list1.Where<FieldEntry>((Func<FieldEntry, bool>) (field => WorkItemChangedEvent.IsBooleanField(field))).Select<FieldEntry, BooleanField>((Func<FieldEntry, BooleanField>) (stringField => WorkItemChangedEvent.GetBooleanField(witRequestContext, fieldData, stringField, updateState.Update.IsNew))).ToArray<BooleanField>()
      };
      itemChangedEvent1.ChangedFields = !keyValuePairs1.Any<KeyValuePair<FieldEntry, object>>() ? (ChangedFieldsType) null : WorkItemChangedEvent.GetChangeFieldsData(witRequestContext, keyValuePairs1, identityMap, updateState, fieldData, actors);
      itemChangedEvent1.FieldValues = keyValuePairs2 == null ? (FieldValue[]) null : WorkItemChangedEvent.GetFieldValues(witRequestContext, WorkItemChangedEvent.GetChangeFieldsData(witRequestContext, keyValuePairs2, identityMap, updateState, fieldData, actors), keyValuePairs2, projectNode, fieldData.WorkItemType);
      itemChangedEvent1.AddedFiles = WorkItemChangedEvent.NullIfEmpty<AddedFile>(source.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (update => update.UpdateType == LinkUpdateType.Add)).Select<WorkItemResourceLinkUpdate, AddedFile>((Func<WorkItemResourceLinkUpdate, AddedFile>) (update => new AddedFile()
      {
        Name = update.Name
      })).ToArray<AddedFile>());
      itemChangedEvent1.DeletedFiles = WorkItemChangedEvent.NullIfEmpty<DeletedFile>(source.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (update => update.UpdateType == LinkUpdateType.Delete)).Select<WorkItemResourceLinkUpdate, DeletedFile>((Func<WorkItemResourceLinkUpdate, DeletedFile>) (update =>
      {
        DeletedFile itemChangedEvent2 = new DeletedFile();
        int? resourceId = update.ResourceId;
        ref int? local = ref resourceId;
        itemChangedEvent2.FileId = local.HasValue ? local.GetValueOrDefault().ToString((IFormatProvider) CultureInfo.InvariantCulture) : (string) null;
        return itemChangedEvent2;
      })).ToArray<DeletedFile>());
      itemChangedEvent1.AddedResourceLinks = WorkItemChangedEvent.NullIfEmpty<AddedResourceLink>(resourceLinkUpdatesExceptFiles.Where<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (update => update.UpdateType == LinkUpdateType.Add)).Select<WorkItemResourceLinkUpdateResult, AddedResourceLink>((Func<WorkItemResourceLinkUpdateResult, AddedResourceLink>) (update => new AddedResourceLink()
      {
        LinkId = update.ResourceId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        Resource = update.Location
      })).ToArray<AddedResourceLink>());
      itemChangedEvent1.DeletedResourceLinks = WorkItemChangedEvent.NullIfEmpty<DeletedResourceLink>(resourceLinkUpdatesExceptFiles.Where<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (update => update.UpdateType == LinkUpdateType.Delete)).Select<WorkItemResourceLinkUpdateResult, DeletedResourceLink>((Func<WorkItemResourceLinkUpdateResult, DeletedResourceLink>) (update => new DeletedResourceLink()
      {
        LinkId = update.ResourceId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        Resource = update.Location
      })).ToArray<DeletedResourceLink>());
      itemChangedEvent1.ChangedResourceLinks = WorkItemChangedEvent.NullIfEmpty<ChangedResourceLink>(resourceLinkUpdatesExceptFiles.Where<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (update => update.UpdateType == LinkUpdateType.Update)).Select<WorkItemResourceLinkUpdateResult, ChangedResourceLink>((Func<WorkItemResourceLinkUpdateResult, ChangedResourceLink>) (update => new ChangedResourceLink()
      {
        LinkId = update.ResourceId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        Resource = update.Location
      })).ToArray<ChangedResourceLink>());
      itemChangedEvent1.AddedRelations = WorkItemChangedEvent.NullIfEmpty<AddedRelation>(WorkItemChangedEvent.GetRelationChanges<AddedRelation>(witRequestContext, (IEnumerable<WorkItemLinkUpdateResult>) updateState.UpdateResult.LinkUpdates, LinkUpdateType.Add, projectNodeId, remoteUrlMap).ToArray<AddedRelation>());
      itemChangedEvent1.DeletedRelations = WorkItemChangedEvent.NullIfEmpty<DeletedRelation>(WorkItemChangedEvent.GetRelationChanges<DeletedRelation>(witRequestContext, (IEnumerable<WorkItemLinkUpdateResult>) updateState.UpdateResult.LinkUpdates, LinkUpdateType.Delete, projectNodeId, remoteUrlMap).ToArray<DeletedRelation>());
      itemChangedEvent1.ChangedRelations = WorkItemChangedEvent.NullIfEmpty<ChangedRelation>(WorkItemChangedEvent.GetRelationChanges<ChangedRelation>(witRequestContext, (IEnumerable<WorkItemLinkUpdateResult>) updateState.UpdateResult.LinkUpdates, LinkUpdateType.Update, projectNodeId, remoteUrlMap).ToArray<ChangedRelation>());
      itemChangedEvent1.Title = string.Empty;
      itemChangedEvent1.Subscriber = string.Empty;
      itemChangedEvent1.ProjectId = fieldData.GetProjectGuid(witRequestContext);
      itemChangedEvent1.SuppressNotifications = suppressNotifications;
      itemChangedEvent1.HasOnlyLinkUpdates = updateState.HasOnlyLinkUpdates;
      itemChangedEvent1.UpdateDate = updateState.UpdateDate;
      WorkItemChangedEvent changedEvent = itemChangedEvent1;
      FieldEntry fieldEntry = list1.FirstOrDefault<FieldEntry>((Func<FieldEntry, bool>) (field => field.ReferenceName == "System.ChangedDate"));
      if (fieldEntry != null && fieldData.GetFieldValue(witRequestContext, fieldEntry.FieldId, false) is DateTime fieldValue)
        changedEvent.ChangedDateUtc = fieldValue;
      IDisposableReadOnlyList<IResourceLinkProvider> extensions = witRequestContext.RequestContext.GetExtensions<IResourceLinkProvider>(ExtensionLifetime.Service);
      changedEvent.AddedResourceLinkTypes = WorkItemChangedEvent.NullIfEmpty<ResourceTypeData>(extensions.SelectMany<IResourceLinkProvider, ResourceTypeData>((Func<IResourceLinkProvider, IEnumerable<ResourceTypeData>>) (lh => lh.ParseResources(resourceLinkUpdatesExceptFiles.Where<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (rlu => rlu.UpdateType == LinkUpdateType.Add)).Select<WorkItemResourceLinkUpdateResult, string>((Func<WorkItemResourceLinkUpdateResult, string>) (rlu => rlu.Location))))).ToArray<ResourceTypeData>());
      changedEvent.ChangedResourceLinkTypes = WorkItemChangedEvent.NullIfEmpty<ResourceTypeData>(extensions.SelectMany<IResourceLinkProvider, ResourceTypeData>((Func<IResourceLinkProvider, IEnumerable<ResourceTypeData>>) (lh => lh.ParseResources(resourceLinkUpdatesExceptFiles.Where<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (rlu => rlu.UpdateType == LinkUpdateType.Update)).Select<WorkItemResourceLinkUpdateResult, string>((Func<WorkItemResourceLinkUpdateResult, string>) (rlu => rlu.Location))))).ToArray<ResourceTypeData>());
      changedEvent.DeletedResourceLinkTypes = WorkItemChangedEvent.NullIfEmpty<ResourceTypeData>(extensions.SelectMany<IResourceLinkProvider, ResourceTypeData>((Func<IResourceLinkProvider, IEnumerable<ResourceTypeData>>) (lh => lh.ParseResources(resourceLinkUpdatesExceptFiles.Where<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (rlu => rlu.UpdateType == LinkUpdateType.Delete)).Select<WorkItemResourceLinkUpdateResult, string>((Func<WorkItemResourceLinkUpdateResult, string>) (rlu => rlu.Location))))).ToArray<ResourceTypeData>());
      ((IEnumerable<StringField>) changedEvent.CoreFields.StringFields).Where<StringField>((Func<StringField, bool>) (x => x.ReferenceName == "System.AuthorizedAs")).First<StringField>().NewValue = NotificationIdentityHelper.FormatIdentityFieldValue(changerIdentity.DisplayName, changerIdentity.Id);
      string str = "System.AuthorizedAs";
      foreach (EventActor eventActor in actors)
      {
        if (eventActor.Role.Equals(str))
        {
          eventActor.Id = changerIdentity.Id;
          break;
        }
      }
      changedEvent.Actors.AddRange((IEnumerable<EventActor>) actors);
      changedEvent.Actors.Add(new EventActor()
      {
        Id = changerIdentity.Id,
        Role = VssNotificationEvent.Roles.Initiator
      });
      int richTextFieldSerializationLength = int.MaxValue;
      if (requestContext.IsFeatureEnabled("WorkItemTracking.Server.TruncateRichTextValuesForNotifications"))
        richTextFieldSerializationLength = witRequestContext.ServerSettings.RichTextFieldSerializationLength;
      List<TextField> list2 = keyValuePairs1.Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (p => p.Key.IsLongText)).Select<KeyValuePair<FieldEntry, object>, TextField>((Func<KeyValuePair<FieldEntry, object>, TextField>) (p => new TextField(richTextFieldSerializationLength)
      {
        Value = (string) p.Value,
        Name = p.Key.Name,
        ReferenceName = p.Key.ReferenceName,
        FieldType = p.Key.FieldType
      })).ToList<TextField>();
      if (updateState.AddedTags != null && updateState.AddedTags.Any<TagDefinition>())
        list2.Add(new TextField(richTextFieldSerializationLength)
        {
          Value = WorkItemChangedEvent.FormatTagsValue(updateState.AddedTags),
          Name = "AddedTags",
          ReferenceName = "System.Tags.Added",
          FieldType = InternalFieldType.PlainText
        });
      if (updateState.RemovedTags != null && updateState.RemovedTags.Any<TagDefinition>())
        list2.Add(new TextField(richTextFieldSerializationLength)
        {
          Value = WorkItemChangedEvent.FormatTagsValue(updateState.RemovedTags),
          Name = "RemovedTags",
          ReferenceName = "System.Tags.Removed",
          FieldType = InternalFieldType.PlainText
        });
      if (updateState.WorkItemComment != null && updateState.WorkItemComment.Version == 1 && !updateState.HasFieldUpdate(54))
      {
        list2.Add(new TextField(richTextFieldSerializationLength)
        {
          Value = updateState.WorkItemComment.TextWithProcessedUrls,
          Name = "History",
          ReferenceName = "System.History",
          FieldType = InternalFieldType.History
        });
        updateState.AddFieldUpdate(54, (object) WorkItemChangedEventProcessor.ProcessNotificationHtml(requestContext, updateState.WorkItemComment.TextWithProcessedUrls));
      }
      changedEvent.TextFields = WorkItemChangedEvent.NullIfEmpty<TextField>(list2.ToArray());
      if (!((IEnumerable<TextField>) changedEvent.TextFields).IsNullOrEmpty<TextField>())
      {
        foreach (TextField textField in changedEvent.TextFields)
          textField.Value = WorkItemChangedEventProcessor.ProcessNotificationHtml(requestContext, textField.Value);
      }
      ChangedFieldsType changedFields = changedEvent.ChangedFields;
      List<string> changedFieldsRefName = keyValuePairs1.Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (x => !x.Key.IsLongText)).Select<KeyValuePair<FieldEntry, object>, string>((Func<KeyValuePair<FieldEntry, object>, string>) (x => x.Key.ReferenceName)).ToList<string>();
      List<string> noSummary = new List<string>()
      {
        "System.Rev",
        "System.ChangedDate",
        "System.AuthorizedDate",
        "System.Watermark"
      };
      changedFieldsRefName.RemoveAll((Predicate<string>) (x => noSummary.Contains(x)));
      int changedTextFieldsRefNameCount = changedEvent.TextFields != null ? changedEvent.TextFields.Length : 0;
      changedEvent.WorkItemComment = WorkItemChangedEvent.CreateWorkItemCommentDataFromUpdateRecord(requestContext, updateState.WorkItemComment);
      int tagsAdded = updateState.AddedTags != null ? updateState.AddedTags.Count<TagDefinition>() : 0;
      int tagsRemoved = updateState.RemovedTags != null ? updateState.RemovedTags.Count<TagDefinition>() : 0;
      int attachmentsAdded = source.Count<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (rlu => rlu.UpdateType == LinkUpdateType.Add));
      int attachmentsRemoved = source.Count<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (rlu => rlu.UpdateType == LinkUpdateType.Delete));
      int linksAdded = resourceLinkUpdatesExceptFiles.Count<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (rlu => rlu.UpdateType == LinkUpdateType.Add));
      linksAdded += changedEvent.AddedRelations != null ? changedEvent.AddedRelations.Length : 0;
      int linksRemoved = resourceLinkUpdatesExceptFiles.Count<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (rlu => rlu.UpdateType == LinkUpdateType.Delete));
      linksRemoved += changedEvent.DeletedRelations != null ? changedEvent.DeletedRelations.Length : 0;
      int linksChanged = resourceLinkUpdatesExceptFiles.Count<WorkItemResourceLinkUpdateResult>((Func<WorkItemResourceLinkUpdateResult, bool>) (rlu => rlu.UpdateType == LinkUpdateType.Update));
      linksChanged += changedEvent.ChangedRelations != null ? changedEvent.ChangedRelations.Length : 0;
      Dictionary<Func<bool>, string> eventPredicates = new Dictionary<Func<bool>, string>()
      {
        {
          (Func<bool>) (() => changedFieldsRefName.Any<string>()),
          "FieldChanged"
        },
        {
          (Func<bool>) (() => updateState.HasFieldUpdate(-404) && (bool) updateState.FieldData.Updates[-404]),
          "Deleted"
        },
        {
          (Func<bool>) (() => updateState.HasFieldUpdate(-404) && !(bool) updateState.FieldData.Updates[-404]),
          "Restored"
        },
        {
          (Func<bool>) (() => updateState.HasTeamProjectChanged),
          "TeamProjectMove"
        },
        {
          (Func<bool>) (() => updateState.HasWorkItemTypeChanged),
          "WorkItemTypeChanged"
        },
        {
          (Func<bool>) (() => updateState.HasFieldUpdate(24)),
          "AssignedToChanged"
        },
        {
          (Func<bool>) (() => updateState.HasFieldUpdate(2)),
          "StateChanged"
        },
        {
          (Func<bool>) (() => changedFieldsRefName.Contains("System.BoardColumn")),
          "BoardColumnChanged"
        },
        {
          (Func<bool>) (() => updateState.HasAreaChange),
          "AreaPathChanged"
        },
        {
          (Func<bool>) (() => updateState.HasIterationChange),
          "IterationPathChanged"
        },
        {
          (Func<bool>) (() => updateState.HasFieldUpdate(1)),
          "TitleChanged"
        },
        {
          (Func<bool>) (() => changedTextFieldsRefNameCount > 0),
          "TextFieldChanged"
        },
        {
          (Func<bool>) (() => updateState.HasFieldUpdate(54)),
          "CommentAdded"
        },
        {
          (Func<bool>) (() => tagsAdded + tagsRemoved > 1),
          "TagsAddedAndRemoved"
        },
        {
          (Func<bool>) (() => tagsAdded == 1),
          "SingleTagAdded"
        },
        {
          (Func<bool>) (() => tagsRemoved == 1),
          "SingleTagRemoved"
        },
        {
          (Func<bool>) (() => attachmentsAdded + attachmentsRemoved > 1),
          "AttachmentsAddedAndRemoved"
        },
        {
          (Func<bool>) (() => attachmentsAdded == 1),
          "SingleAttachmentAdded"
        },
        {
          (Func<bool>) (() => attachmentsRemoved == 1),
          "SingleAttachmentRemoved"
        },
        {
          (Func<bool>) (() => linksAdded + linksRemoved + linksChanged > 1),
          "LinksAddedAndRemoved"
        },
        {
          (Func<bool>) (() => linksAdded == 1),
          "SingleLinkAdded"
        },
        {
          (Func<bool>) (() => linksRemoved == 1),
          "SingleLinkRemoved"
        },
        {
          (Func<bool>) (() => linksChanged == 1),
          "SingleLinkChanged"
        },
        {
          (Func<bool>) (() => linksAdded > 0),
          "LinksAdded"
        },
        {
          (Func<bool>) (() => linksRemoved > 0),
          "LinksRemoved"
        },
        {
          (Func<bool>) (() => changedEvent.WorkItemComment != null && changedEvent.WorkItemComment.IsUpdated),
          "CommentUpdated"
        },
        {
          (Func<bool>) (() => changedEvent.WorkItemComment != null && changedEvent.WorkItemComment.IsDeleted),
          "CommentDeleted"
        }
      };
      changedEvent.EventSubtypes = eventPredicates.Keys.Where<Func<bool>>((Func<Func<bool>, bool>) (x => x())).Select<Func<bool>, string>((Func<Func<bool>, string>) (x => eventPredicates[x])).ToArray<string>();
      changedEvent.DisplayUrl = WorkItemChangedEvent.GetDisplayUrl(requestContext, changedEvent.ProjectNodeId, fieldData.Id, artifactUri);
      WorkItemChangedEventExtended itemChangedEvent3 = new WorkItemChangedEventExtended();
      itemChangedEvent3.LegacyChangedEvent = changedEvent;
      if (requestContext.IsFeatureEnabled("WorkItemTracking.Server.FireWorkItemsChangedWithExtensionsBatchEvent"))
      {
        itemChangedEvent3.CurrentExtensionIds = ((IEnumerable<WorkItemTypeExtension>) updateState.CurrentExtensions ?? Enumerable.Empty<WorkItemTypeExtension>()).Select<WorkItemTypeExtension, Guid>((Func<WorkItemTypeExtension, Guid>) (x => x.Id)).ToList<Guid>();
        itemChangedEvent3.OldExtensionIds = ((IEnumerable<WorkItemTypeExtension>) updateState.OldExtensions ?? Enumerable.Empty<WorkItemTypeExtension>()).Select<WorkItemTypeExtension, Guid>((Func<WorkItemTypeExtension, Guid>) (x => x.Id)).ToList<Guid>();
      }
      return itemChangedEvent3;
    }

    private static IEnumerable<WorkItemChangedEventExtended> CreateWorkItemChangedEventsForWorkItemDestroyServiceBusNotifications(
      Dictionary<int, Guid> workItemProjectIdDictionary)
    {
      List<WorkItemChangedEventExtended> busNotifications = new List<WorkItemChangedEventExtended>();
      foreach (KeyValuePair<int, Guid> workItemProjectId in workItemProjectIdDictionary)
      {
        WorkItemChangedEvent itemChangedEvent = new WorkItemChangedEvent()
        {
          WorkItemId = workItemProjectId.Key.ToString(),
          ProjectId = workItemProjectId.Value,
          ProjectNodeId = workItemProjectId.Value.ToString()
        };
        WorkItemChangedEventExtended changedEventExtended = new WorkItemChangedEventExtended()
        {
          LegacyChangedEvent = itemChangedEvent
        };
        busNotifications.Add(changedEventExtended);
      }
      return (IEnumerable<WorkItemChangedEventExtended>) busNotifications;
    }

    internal static IEnumerable<Relation> GetRelationChanges<Relation>(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemLinkUpdateResult> linkUpdates,
      LinkUpdateType updateType,
      string projectNodeId,
      Dictionary<Guid, string> remoteUrlMap)
      where Relation : IWorkItemChangedRelation, new()
    {
      IEnumerable<WorkItemLinkUpdateResult> source1 = linkUpdates.Where<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (lu =>
      {
        LinkUpdateType? updateTypeExecuted = lu.UpdateTypeExecuted;
        LinkUpdateType linkUpdateType = updateType;
        return updateTypeExecuted.GetValueOrDefault() == linkUpdateType & updateTypeExecuted.HasValue;
      }));
      if (!source1.Any<WorkItemLinkUpdateResult>())
        return Enumerable.Empty<Relation>();
      IVssRequestContext requestContext1 = witRequestContext.RequestContext;
      IEnumerable<WorkItemLinkUpdateResult> source2 = source1.Where<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (x => !witRequestContext.LinkService.GetLinkTypeById(witRequestContext.RequestContext, x.LinkType).IsRemote));
      IEnumerable<int> ints = source2.Select<WorkItemLinkUpdateResult, int>((Func<WorkItemLinkUpdateResult, int>) (up => up.TargetWorkItemId));
      ITeamFoundationWorkItemService service = requestContext1.GetService<ITeamFoundationWorkItemService>();
      IVssRequestContext requestContext2 = requestContext1;
      IEnumerable<int> workItemIds = ints;
      List<string> fields = new List<string>();
      fields.Add("System.Title");
      fields.Add("System.WorkItemType");
      DateTime? asOf = new DateTime?();
      Dictionary<int, WorkItemFieldData> dictionary = service.GetWorkItemFieldValues(requestContext2, workItemIds, (IEnumerable<string>) fields, asOf: asOf, workItemRetrievalMode: WorkItemRetrievalMode.All).ToDictionary<WorkItemFieldData, int>((Func<WorkItemFieldData, int>) (fd => fd.Id));
      if (!dictionary.Any<KeyValuePair<int, WorkItemFieldData>>() && source2.Any<WorkItemLinkUpdateResult>())
        return Enumerable.Empty<Relation>();
      List<Relation> relationChanges = new List<Relation>();
      List<Guid> list = source1.Where<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (x =>
      {
        if (!x.RemoteHostId.HasValue)
          return false;
        Guid? remoteHostId = x.RemoteHostId;
        Guid empty = Guid.Empty;
        if (!remoteHostId.HasValue)
          return true;
        return remoteHostId.HasValue && remoteHostId.GetValueOrDefault() != empty;
      })).Select<WorkItemLinkUpdateResult, Guid>((Func<WorkItemLinkUpdateResult, Guid>) (x => x.RemoteHostId.Value)).ToList<Guid>();
      IDictionary<Guid, RemoteHostContext> remoteHostUrls = RemoteWorkItemHostResolver.GetRemoteHostUrls(requestContext1, (IEnumerable<Guid>) list);
      foreach (WorkItemLinkUpdateResult linkUpdateResult in source1)
      {
        MDWorkItemLinkType linkTypeById = witRequestContext.LinkService.GetLinkTypeById(requestContext1, linkUpdateResult.LinkType);
        WorkItemFieldData workItemFieldData = (WorkItemFieldData) null;
        if (dictionary.TryGetValue(linkUpdateResult.TargetWorkItemId, out workItemFieldData) || linkTypeById.IsRemote && !string.IsNullOrEmpty(linkUpdateResult.RemoteWorkItemTitle) && !string.IsNullOrEmpty(linkUpdateResult.RemoteWorkItemType))
        {
          string workItemId = linkUpdateResult.TargetWorkItemId.ToString();
          RemoteHostContext remoteHostContext = linkTypeById.IsRemote ? remoteHostUrls[linkUpdateResult.RemoteHostId.Value] : (RemoteHostContext) null;
          if (!linkTypeById.IsRemote || remoteHostContext != null && !string.IsNullOrEmpty(remoteHostContext.Url))
            relationChanges.Add(new Relation()
            {
              LinkName = linkUpdateResult.LinkType == linkTypeById.ForwardId ? linkTypeById.ForwardEndName : linkTypeById.ReverseEndName,
              IsBacklogParentLink = linkTypeById.ReferenceName == "System.LinkTypes.Hierarchy" && linkUpdateResult.LinkType == linkTypeById.ReverseId,
              IsBacklogChildLink = linkTypeById.ReferenceName == "System.LinkTypes.Hierarchy" && linkUpdateResult.LinkType == linkTypeById.ForwardId,
              WorkItemId = workItemId,
              Title = linkTypeById.IsRemote ? linkUpdateResult.RemoteWorkItemTitle : workItemFieldData.Title,
              Type = linkTypeById.IsRemote ? linkUpdateResult.RemoteWorkItemType : workItemFieldData.WorkItemType,
              DisplayUrl = WorkItemChangedEvent.GetDisplayUrl(requestContext1, projectNodeId, linkUpdateResult.TargetWorkItemId, WorkItemArtifactUriHelper.GetArtifactUri(workItemId), linkUpdateResult.RemoteHostId, remoteHostContext?.Url, linkUpdateResult.RemoteProjectId)
            });
        }
      }
      return (IEnumerable<Relation>) relationChanges;
    }

    internal static string GetDateFieldString(
      WorkItemTrackingRequestContext witRequestContext,
      DateTime date)
    {
      Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
      date = date.ToLocalTime();
      if (date > calendar.MaxSupportedDateTime)
        date = calendar.MaxSupportedDateTime;
      else if (date < calendar.MinSupportedDateTime)
        date = calendar.MinSupportedDateTime;
      return date.ToString((IFormatProvider) witRequestContext.RequestContext.ServiceHost.GetCulture(witRequestContext.RequestContext));
    }

    internal static string GetLocalDateFieldString(
      WorkItemTrackingRequestContext witRequestContext,
      DateTime date,
      TimeZoneInfo timezone)
    {
      if (timezone == null)
        return (string) null;
      try
      {
        DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(date, timezone);
        IVssRequestContext requestContext = witRequestContext.RequestContext;
        return dateTime.ToString((IFormatProvider) requestContext.ServiceHost.GetCulture(requestContext)) + " " + timezone.DisplayName;
      }
      catch (Exception ex)
      {
        witRequestContext.RequestContext.Trace(904910, TraceLevel.Error, "Services", "WorkItemService", ex.ToString());
        return (string) null;
      }
    }

    private static TimeZoneInfo GetTimeZone(IVssRequestContext requestContext) => requestContext.GetService<ICollectionPreferencesService>().GetCollectionTimeZone(requestContext) ?? TimeZoneInfo.Local;

    private static int FieldValueToInt(object value, out bool isEmptyValue)
    {
      if (value == null)
      {
        isEmptyValue = true;
        return int.MinValue;
      }
      isEmptyValue = false;
      return CommonWITUtils.ConvertValue<int>(value);
    }

    internal static bool FieldValueToBool(object value) => value != null && CommonWITUtils.ConvertValue<bool>(value);

    internal static string FieldValueToString(
      WorkItemTrackingRequestContext witRequestContext,
      object value)
    {
      switch (value)
      {
        case null:
          return string.Empty;
        case DateTime date:
          return WorkItemChangedEvent.GetDateFieldString(witRequestContext, date);
        case Guid guid:
          return guid.ToString("N", (IFormatProvider) CultureInfo.InvariantCulture);
        default:
          return value.ToString();
      }
    }

    internal static string FieldValueToLocalString(
      WorkItemTrackingRequestContext witRequestContext,
      object value,
      TimeZoneInfo timezone)
    {
      return value == null || !(value is DateTime date) ? (string) null : WorkItemChangedEvent.GetLocalDateFieldString(witRequestContext, date, timezone);
    }

    internal static IntegerField GetIntegerField(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData fieldData,
      FieldEntry fieldEntry,
      bool isNew)
    {
      bool isEmptyValue1;
      int num = WorkItemChangedEvent.FieldValueToInt(fieldData.GetFieldValue(witRequestContext, fieldEntry.FieldId, false), out isEmptyValue1);
      bool isEmptyValue2;
      int minValue;
      if (isNew)
      {
        isEmptyValue2 = true;
        minValue = int.MinValue;
      }
      else
        minValue = WorkItemChangedEvent.FieldValueToInt(fieldData.GetFieldValue(witRequestContext, fieldEntry.FieldId, true), out isEmptyValue2);
      if (isEmptyValue1 & isEmptyValue2 && fieldEntry.FieldId == -35)
        return new IntegerField()
        {
          Name = fieldEntry.Name,
          ReferenceName = fieldEntry.ReferenceName,
          OldValue = int.MinValue,
          NewValue = int.MinValue,
          IsOldValueEmpty = isEmptyValue2,
          IsNewValueEmpty = isEmptyValue1
        };
      return new IntegerField()
      {
        Name = fieldEntry.Name,
        ReferenceName = fieldEntry.ReferenceName,
        OldValue = isNew ? int.MinValue : minValue,
        NewValue = isEmptyValue1 ? 0 : num,
        IsOldValueEmpty = isEmptyValue2,
        IsNewValueEmpty = isEmptyValue1
      };
    }

    internal static BooleanField GetBooleanField(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData fieldData,
      FieldEntry fieldEntry,
      bool isNew)
    {
      return new BooleanField()
      {
        Name = fieldEntry.Name,
        ReferenceName = fieldEntry.ReferenceName,
        OldValue = !isNew && WorkItemChangedEvent.FieldValueToBool(fieldData.GetFieldValue(witRequestContext, fieldEntry.FieldId, true)),
        NewValue = CommonWITUtils.ConvertValue<bool>(fieldData.GetFieldValue(witRequestContext, fieldEntry.FieldId, false))
      };
    }

    private static FieldValue[] GetFieldValues(
      WorkItemTrackingRequestContext witRequestContext,
      ChangedFieldsType allFields,
      IEnumerable<KeyValuePair<FieldEntry, object>> fieldsData,
      TreeNode projectNode,
      string workItemTypeName)
    {
      List<FieldValue> fieldValueList = new List<FieldValue>();
      HashSet<string> fieldsAdded = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldFriendlyName);
      fieldValueList.AddRange(((IEnumerable<BooleanField>) allFields.BooleanFields).Select<BooleanField, FieldValue>((Func<BooleanField, FieldValue>) (f => new FieldValue(f.Name, (object) f.NewValue))));
      fieldsAdded.UnionWith(((IEnumerable<BooleanField>) allFields.BooleanFields).Select<BooleanField, string>((Func<BooleanField, string>) (f => f.Name)));
      fieldValueList.AddRange(((IEnumerable<IntegerField>) allFields.IntegerFields).Select<IntegerField, FieldValue>((Func<IntegerField, FieldValue>) (f => new FieldValue(f.Name, (object) f.NewValue))));
      fieldsAdded.UnionWith(((IEnumerable<IntegerField>) allFields.IntegerFields).Select<IntegerField, string>((Func<IntegerField, string>) (f => f.Name)));
      fieldValueList.AddRange(((IEnumerable<StringField>) allFields.StringFields).Select<StringField, FieldValue>((Func<StringField, FieldValue>) (f => new FieldValue(f.Name, (object) f.NewValue)
      {
        IsIdentity = f.IsIdentity
      })));
      fieldsAdded.UnionWith(((IEnumerable<StringField>) allFields.StringFields).Select<StringField, string>((Func<StringField, string>) (f => f.Name)));
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType = witRequestContext.RequestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(witRequestContext.RequestContext, projectNode.ProjectId).FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, bool>) (type => TFStringComparer.WorkItemTypeName.Equals(workItemTypeName, type.Name)));
      if (workItemType != null)
      {
        IEnumerable<FieldEntry> fields = workItemType.GetFields(witRequestContext.RequestContext, true);
        IEnumerable<FieldEntry> source1 = fields.Where<FieldEntry>((Func<FieldEntry, bool>) (f => !fieldsAdded.Contains(f.Name) && (f.Usage & InternalFieldUsages.WorkItemTypeExtension) == InternalFieldUsages.None && !f.IsLongText));
        fieldValueList.AddRange(source1.Select<FieldEntry, FieldValue>((Func<FieldEntry, FieldValue>) (f => new FieldValue(f.Name, (object) null))));
        FieldEntry fieldEntry = fields.FirstOrDefault<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, "System.Tags")));
        if (fieldEntry != null && witRequestContext.RequestContext.IsFeatureEnabled("WorkItemTracking.Server.AllowTagsInAlerts"))
        {
          IEnumerable<KeyValuePair<FieldEntry, object>> source2 = fieldsData.Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.Key.ReferenceName, "System.Tags")));
          if (source2.Count<KeyValuePair<FieldEntry, object>>() == 1)
          {
            string str = source2.First<KeyValuePair<FieldEntry, object>>().Value?.ToString();
            if (!string.IsNullOrEmpty(str))
              fieldValueList.Add(new FieldValue(fieldEntry.Name, (object) ((IEnumerable<string>) str.Split(new char[1]
              {
                ';'
              }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (tag => tag.Trim())).ToArray<string>(), true));
            else
              fieldValueList.Add(new FieldValue(fieldEntry.Name, (object) null, true));
          }
          else
            fieldValueList.Add(new FieldValue(fieldEntry.Name, (object) null, true));
        }
      }
      return fieldValueList.ToArray();
    }

    private static StringField GetStringField(
      WorkItemTrackingRequestContext witRequestContext,
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      WorkItemFieldData fieldData,
      FieldEntry fieldEntry,
      bool isNew,
      List<EventActor> actors,
      TimeZoneInfo timezone)
    {
      object fieldValue1 = fieldData.GetFieldValue(witRequestContext, fieldEntry.FieldId, true);
      string distinctDisplayName1 = isNew ? "" : WorkItemChangedEvent.FieldValueToString(witRequestContext, fieldValue1);
      string localString1 = isNew ? (string) null : WorkItemChangedEvent.FieldValueToLocalString(witRequestContext, fieldValue1, timezone);
      object fieldValue2 = fieldData.GetFieldValue(witRequestContext, fieldEntry.FieldId, false);
      string distinctDisplayName2 = WorkItemChangedEvent.FieldValueToString(witRequestContext, fieldValue2);
      string localString2 = WorkItemChangedEvent.FieldValueToLocalString(witRequestContext, fieldValue2, timezone);
      if (fieldEntry.IsIdentity)
      {
        Guid distinctDisplayName3 = WorkItemChangedEvent.GetTFIDFromDistinctDisplayName(identityMap, distinctDisplayName1);
        distinctDisplayName1 = NotificationIdentityHelper.FormatIdentityFieldValue(IdentityHelper.GetDisplayNameFromDistinctDisplayName(distinctDisplayName1), distinctDisplayName3);
        Guid guid = distinctDisplayName3;
        Guid distinctDisplayName4 = WorkItemChangedEvent.GetTFIDFromDistinctDisplayName(identityMap, distinctDisplayName2);
        distinctDisplayName2 = NotificationIdentityHelper.FormatIdentityFieldValue(IdentityHelper.GetDisplayNameFromDistinctDisplayName(distinctDisplayName2), distinctDisplayName4);
        if (fieldEntry.ReferenceName == "System.AssignedTo" && guid != distinctDisplayName4)
        {
          actors.Add(new EventActor()
          {
            Id = guid,
            Role = fieldEntry.ReferenceName + ".Old"
          });
          actors.Add(new EventActor()
          {
            Id = distinctDisplayName4,
            Role = fieldEntry.ReferenceName + ".New"
          });
        }
        actors.Add(new EventActor()
        {
          Id = distinctDisplayName4,
          Role = fieldEntry.ReferenceName
        });
      }
      bool flag = fieldEntry.FieldId == -7 || fieldEntry.FieldId == -105;
      return new StringField()
      {
        Name = fieldEntry.Name,
        ReferenceName = fieldEntry.ReferenceName,
        OldValue = !flag || isNew ? distinctDisplayName1 : '\\'.ToString() + distinctDisplayName1,
        NewValue = flag ? '\\'.ToString() + distinctDisplayName2 : distinctDisplayName2,
        OldLocalValue = localString1,
        NewLocalValue = localString2,
        IsIdentity = fieldEntry.IsIdentity
      };
    }

    public static string FormatTagsValue(IEnumerable<TagDefinition> tags) => string.Join("; ", (IEnumerable<string>) tags.Select<TagDefinition, string>((Func<TagDefinition, string>) (tag => tag.Name)).OrderBy<string, string>((Func<string, string>) (n => n), (IComparer<string>) StringComparer.CurrentCulture));

    public static bool IsFileAttachment(WorkItemResourceLinkUpdate resourceLinkUpdate)
    {
      ResourceLinkType? type1 = resourceLinkUpdate.Type;
      ResourceLinkType resourceLinkType1 = ResourceLinkType.Attachment;
      if (type1.GetValueOrDefault() == resourceLinkType1 & type1.HasValue)
        return true;
      ResourceLinkType? type2 = resourceLinkUpdate.Type;
      ResourceLinkType resourceLinkType2 = ResourceLinkType.InlineImage;
      return type2.GetValueOrDefault() == resourceLinkType2 & type2.HasValue;
    }

    public static bool IsFileAttachment(
      WorkItemResourceLinkUpdateResult resourceLinkUpdate)
    {
      return resourceLinkUpdate.Type == ResourceLinkType.Attachment || resourceLinkUpdate.Type == ResourceLinkType.InlineImage;
    }

    public static bool IsIntegerField(FieldEntry fieldEntry) => fieldEntry.FieldType == InternalFieldType.Integer;

    public static bool IsBooleanField(FieldEntry fieldEntry) => fieldEntry.FieldType == InternalFieldType.Boolean;

    public static bool IsStringField(FieldEntry fieldEntry) => !WorkItemChangedEvent.IsIntegerField(fieldEntry) && !fieldEntry.IsLongText && !WorkItemChangedEvent.IsBooleanField(fieldEntry);

    private static T[] NullIfEmpty<T>(T[] inputArray) => !((IEnumerable<T>) inputArray).Any<T>() ? (T[]) null : inputArray;

    private static void UpdateParentField(
      IVssRequestContext requestContext,
      WorkItemUpdateState updateState)
    {
      if (!requestContext.WitContext().FieldDictionary.TryGetField(-35, out FieldEntry _))
        return;
      ReadOnlyDictionary<int, object> latestData = updateState.FieldData.LatestData;
      List<KeyValuePair<int, object>> updates = new List<KeyValuePair<int, object>>();
      if (updateState.HasLinkUpdates && updateState.FieldData.Updates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (kvp => kvp.Key == -35)).IsNullOrEmpty<KeyValuePair<int, object>>())
      {
        IEnumerable<WorkItemLinkUpdate> source1 = updateState.Update.LinkUpdates.Where<WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, bool>) (l =>
        {
          if (l.LinkType != -2)
            return false;
          return l.UpdateType == LinkUpdateType.Add || l.UpdateType == LinkUpdateType.Update;
        }));
        IEnumerable<WorkItemLinkUpdate> source2 = updateState.Update.LinkUpdates.Where<WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, bool>) (l => l.LinkType == -2 && l.UpdateType == LinkUpdateType.Delete));
        if (source1.Any<WorkItemLinkUpdate>())
        {
          WorkItemLinkUpdate workItemLinkUpdate = source1.First<WorkItemLinkUpdate>();
          updates.Add(new KeyValuePair<int, object>(-35, (object) workItemLinkUpdate.TargetWorkItemId));
        }
        else if (source2.Any<WorkItemLinkUpdate>())
          updates.Add(new KeyValuePair<int, object>(-35, (object) null));
      }
      else if (updateState.UpdateResult.LinkUpdates.Any<WorkItemLinkUpdateResult>())
      {
        IEnumerable<WorkItemLinkUpdateResult> source3 = updateState.UpdateResult.LinkUpdates.Where<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (l =>
        {
          if (l.LinkType != -2)
            return false;
          return l.UpdateType == LinkUpdateType.Add || l.UpdateType == LinkUpdateType.Update;
        }));
        IEnumerable<WorkItemLinkUpdateResult> source4 = updateState.UpdateResult.LinkUpdates.Where<WorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, bool>) (l => l.LinkType == -2 && l.UpdateType == LinkUpdateType.Delete));
        if (source3.Any<WorkItemLinkUpdateResult>())
        {
          WorkItemLinkUpdateResult linkUpdateResult = source3.First<WorkItemLinkUpdateResult>();
          updates.Add(new KeyValuePair<int, object>(-35, (object) linkUpdateResult.TargetWorkItemId));
        }
        else if (source4.Any<WorkItemLinkUpdateResult>())
          updates.Add(new KeyValuePair<int, object>(-35, (object) null));
      }
      updateState.FieldData.SetFieldUpdates(requestContext, (IEnumerable<KeyValuePair<int, object>>) updates);
    }

    private static void UpdateLinkCounts(
      IVssRequestContext requestContext,
      WorkItemUpdateState updateState)
    {
      ReadOnlyDictionary<int, object> latestData = updateState.FieldData.LatestData;
      List<KeyValuePair<int, object>> updates = new List<KeyValuePair<int, object>>();
      object obj1;
      if (updateState.HasLinkUpdates && latestData.TryGetValue(75, out obj1) && updateState.FieldData.Updates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (kvp => kvp.Key == 75)).IsNullOrEmpty<KeyValuePair<int, object>>())
      {
        int num1 = updateState.Update.LinkUpdates.Where<WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, bool>) (l => l.UpdateType == LinkUpdateType.Add)).Count<WorkItemLinkUpdate>();
        int num2 = updateState.Update.LinkUpdates.Where<WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, bool>) (l => l.UpdateType == LinkUpdateType.Delete)).Count<WorkItemLinkUpdate>();
        updates.Add(new KeyValuePair<int, object>(75, (object) ((int) obj1 + num1 - num2)));
      }
      if (updateState.HasResourceLinkUpdates)
      {
        IEnumerable<WorkItemResourceLinkUpdate> source1 = updateState.Update.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (l =>
        {
          ResourceLinkType? type1 = l.Type;
          ResourceLinkType resourceLinkType1 = ResourceLinkType.Attachment;
          if (type1.GetValueOrDefault() == resourceLinkType1 & type1.HasValue)
            return true;
          ResourceLinkType? type2 = l.Type;
          ResourceLinkType resourceLinkType2 = ResourceLinkType.InlineImage;
          return type2.GetValueOrDefault() == resourceLinkType2 & type2.HasValue;
        }));
        IEnumerable<WorkItemResourceLinkUpdate> source2 = updateState.Update.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (l =>
        {
          ResourceLinkType? type = l.Type;
          ResourceLinkType resourceLinkType = ResourceLinkType.Hyperlink;
          return type.GetValueOrDefault() == resourceLinkType & type.HasValue;
        }));
        IEnumerable<WorkItemResourceLinkUpdate> source3 = updateState.Update.ResourceLinkUpdates.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (l =>
        {
          ResourceLinkType? type = l.Type;
          ResourceLinkType resourceLinkType = ResourceLinkType.ArtifactLink;
          return type.GetValueOrDefault() == resourceLinkType & type.HasValue;
        }));
        object obj2;
        if (source1.Count<WorkItemResourceLinkUpdate>() > 0 && latestData.TryGetValue(-31, out obj2) && updateState.FieldData.Updates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (kvp => kvp.Key == -31)).IsNullOrEmpty<KeyValuePair<int, object>>())
        {
          int num3 = source1.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (l => l.UpdateType == LinkUpdateType.Add)).Count<WorkItemResourceLinkUpdate>();
          int num4 = source1.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (l => l.UpdateType == LinkUpdateType.Delete)).Count<WorkItemResourceLinkUpdate>();
          updates.Add(new KeyValuePair<int, object>(-31, (object) ((int) obj2 + num3 - num4)));
        }
        object obj3;
        if (source2.Count<WorkItemResourceLinkUpdate>() > 0 && latestData.TryGetValue(-32, out obj3) && updateState.FieldData.Updates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (kvp => kvp.Key == -32)).IsNullOrEmpty<KeyValuePair<int, object>>())
        {
          int num5 = source2.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (l => l.UpdateType == LinkUpdateType.Add)).Count<WorkItemResourceLinkUpdate>();
          int num6 = source2.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (l => l.UpdateType == LinkUpdateType.Delete)).Count<WorkItemResourceLinkUpdate>();
          updates.Add(new KeyValuePair<int, object>(-32, (object) ((int) obj3 + num5 - num6)));
        }
        object obj4;
        if (source3.Count<WorkItemResourceLinkUpdate>() > 0 && latestData.TryGetValue(-57, out obj4) && updateState.FieldData.Updates.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (kvp => kvp.Key == -57)).IsNullOrEmpty<KeyValuePair<int, object>>())
        {
          int num7 = source3.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (l => l.UpdateType == LinkUpdateType.Add)).Count<WorkItemResourceLinkUpdate>();
          int num8 = source3.Where<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (l => l.UpdateType == LinkUpdateType.Delete)).Count<WorkItemResourceLinkUpdate>();
          updates.Add(new KeyValuePair<int, object>(-57, (object) ((int) obj4 + num7 - num8)));
        }
      }
      updateState.FieldData.SetFieldUpdates(requestContext, (IEnumerable<KeyValuePair<int, object>>) updates);
    }

    private static ChangedFieldsType GetChangeFieldsData(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<KeyValuePair<FieldEntry, object>> workItemData,
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      WorkItemUpdateState updateState,
      WorkItemFieldData fieldData,
      List<EventActor> actors)
    {
      return new ChangedFieldsType()
      {
        IntegerFields = workItemData.Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (up => WorkItemChangedEvent.IsIntegerField(up.Key))).Select<KeyValuePair<FieldEntry, object>, IntegerField>((Func<KeyValuePair<FieldEntry, object>, IntegerField>) (up => WorkItemChangedEvent.GetIntegerField(witRequestContext, fieldData, up.Key, updateState.Update.IsNew))).ToArray<IntegerField>(),
        StringFields = workItemData.Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (up => WorkItemChangedEvent.IsStringField(up.Key))).Select<KeyValuePair<FieldEntry, object>, StringField>((Func<KeyValuePair<FieldEntry, object>, StringField>) (up => WorkItemChangedEvent.GetStringField(witRequestContext, identityMap, fieldData, up.Key, updateState.Update.IsNew, actors, WorkItemChangedEvent.GetTimeZone(witRequestContext.RequestContext)))).ToArray<StringField>(),
        BooleanFields = workItemData.Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (up => WorkItemChangedEvent.IsBooleanField(up.Key))).Select<KeyValuePair<FieldEntry, object>, BooleanField>((Func<KeyValuePair<FieldEntry, object>, BooleanField>) (up => WorkItemChangedEvent.GetBooleanField(witRequestContext, fieldData, up.Key, updateState.Update.IsNew))).ToArray<BooleanField>()
      };
    }

    private static Guid GetTFIDFromDistinctDisplayName(
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      string distinctDisplayName)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      return !string.IsNullOrEmpty(distinctDisplayName) && identityMap.TryGetValue(distinctDisplayName, out identity) ? identity.Id : Guid.Empty;
    }

    private static string GetDisplayUrl(
      IVssRequestContext requestContext,
      string projectNodeId,
      int workItemId,
      string artifactUri,
      Guid? remoteHostId = null,
      string remoteHostUrl = null,
      Guid? remoteProjectId = null)
    {
      ITswaServerHyperlinkService service = requestContext.GetService<ITswaServerHyperlinkService>();
      string uri;
      using (IVssRequestContext requestContext1 = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, requestContext.ServiceHost.InstanceId, RequestContextType.UserContext))
      {
        try
        {
          if (WorkItemTrackingFeatureFlags.GenerateWorkItemURLsWithProjectContext(requestContext) && !string.IsNullOrEmpty(projectNodeId))
          {
            Uri projectUri = new Uri(Microsoft.TeamFoundation.Core.WebApi.ProjectInfo.GetProjectUri(projectNodeId));
            uri = !remoteHostId.HasValue || string.IsNullOrEmpty(remoteHostUrl) || !remoteProjectId.HasValue ? service.GetWorkItemEditorUrl(requestContext1, projectUri, workItemId).ToString() : service.GetWorkItemEditorUrl(requestContext1, workItemId, remoteHostId.Value, remoteHostUrl, new Guid?(remoteProjectId.Value)).ToString();
          }
          else
          {
            uri = !remoteHostId.HasValue || string.IsNullOrEmpty(remoteHostUrl) ? service.GetWorkItemEditorUrl(requestContext1, workItemId).ToString() : service.GetWorkItemEditorUrl(requestContext1, workItemId, remoteHostId.Value, remoteHostUrl, remoteProjectId).ToString();
            if (!string.IsNullOrEmpty(projectNodeId))
            {
              UriBuilder uriBuilder = new UriBuilder(uri);
              NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
              queryString["projectId"] = !remoteProjectId.HasValue || !(remoteProjectId.Value != Guid.Empty) ? projectNodeId : remoteProjectId.ToString();
              uriBuilder.Query = queryString.ToString();
              uri = uriBuilder.ToString();
            }
          }
        }
        catch (InvalidOperationException ex1)
        {
          try
          {
            uri = requestContext.GetService<TeamFoundationLinkingService>().GetArtifactUrlExternal(requestContext, artifactUri);
          }
          catch (SecurityException ex2)
          {
            uri = string.Empty;
          }
        }
      }
      return uri;
    }

    private static WorkItemCommentData CreateWorkItemCommentDataFromUpdateRecord(
      IVssRequestContext requestContext,
      WorkItemCommentUpdateRecord workItemComment)
    {
      if (workItemComment == null)
        return (WorkItemCommentData) null;
      bool flag = !workItemComment.IsDeleted && workItemComment.Version > 1;
      WorkItemCommentFormat itemCommentFormat = (WorkItemCommentFormat) ((int) workItemComment.Format ?? 1);
      string str = itemCommentFormat != WorkItemCommentFormat.Markdown || string.IsNullOrEmpty(workItemComment.RenderedText) || !WorkItemTrackingFeatureFlags.IsMarkdownDiscussionEnabled(requestContext) ? (flag ? WorkItemChangedEventProcessor.ProcessNotificationHtml(requestContext, workItemComment.TextWithProcessedUrls) : (string) null) : WorkItemChangedEventProcessor.ProcessNotificationHtml(requestContext, workItemComment.RenderedText);
      return new WorkItemCommentData()
      {
        CommentId = workItemComment.CommentId,
        WorkItemId = int.Parse(workItemComment.ArtifactId),
        Version = workItemComment.Version,
        CreatedDate = workItemComment.CreatedDate,
        CreatedBy = workItemComment.CreatedBy,
        CreatedOnBehalfOf = workItemComment.CreatedOnBehalfOf,
        CreatedOnBehalfDate = workItemComment.CreatedOnBehalfDate,
        Format = itemCommentFormat,
        ModifiedDate = workItemComment.ModifiedDate,
        ModifiedBy = workItemComment.ModifiedBy,
        IsDeleted = workItemComment.IsDeleted,
        IsNew = workItemComment.Version == 1,
        IsUpdated = flag,
        Text = str
      };
    }

    public string WorkItemId
    {
      get => this.workItemId;
      set => this.workItemId = value;
    }

    public string WorkItemType
    {
      get => this.workItemType;
      set => this.workItemType = value;
    }

    public string PortfolioProject
    {
      get => this.portfolioProjectField;
      set => this.portfolioProjectField = value;
    }

    public string ProjectNodeId
    {
      get => this.projectNodeIdField;
      set => this.projectNodeIdField = value;
    }

    public string[] EventSubtypes
    {
      get => this.eventSubtypes;
      set => this.eventSubtypes = value;
    }

    public string AreaPath
    {
      get => this.areaPathField;
      set => this.areaPathField = value;
    }

    public string Title
    {
      get => this.titleField;
      set => this.titleField = value;
    }

    public string WorkItemTitle
    {
      get => this.workItemTitleField;
      set => this.workItemTitleField = value;
    }

    public string Subscriber
    {
      get => this.subscriberField;
      set => this.subscriberField = value;
    }

    public string ChangerSid
    {
      get => this.changerSidField;
      set => this.changerSidField = value;
    }

    public string ChangerTeamFoundationId
    {
      get => this.changerTeamFoundationIdField;
      set
      {
        this.changerTeamFoundationIdField = value;
        if (this.Actors.Any<EventActor>((Func<EventActor, bool>) (a => a.Role == VssNotificationEvent.Roles.MainActor)))
          return;
        this.Actors.Insert(0, new EventActor()
        {
          Role = VssNotificationEvent.Roles.MainActor,
          Id = Guid.Parse(value)
        });
      }
    }

    public string ChangerIpAddress
    {
      get => this.changerIpAddress;
      set => this.changerIpAddress = value;
    }

    public string ChangerDisplayName
    {
      get => this.changerDisplayNameField;
      set => this.changerDisplayNameField = value;
    }

    public string DisplayUrl
    {
      get => this.displayUrlField;
      set => this.displayUrlField = value;
    }

    public string TimeZone
    {
      get => this.timeZoneField;
      set => this.timeZoneField = value;
    }

    public string TimeZoneOffset
    {
      get => this.timeZoneOffsetField;
      set => this.timeZoneOffsetField = value;
    }

    public bool SuppressNotifications
    {
      get => this.suppressNotification;
      set => this.suppressNotification = value;
    }

    public bool HasOnlyLinkUpdates
    {
      get => this.hasOnlyLinkUpdates;
      set => this.hasOnlyLinkUpdates = value;
    }

    public ChangeTypes ChangeType
    {
      get => this.changeTypeField;
      set => this.changeTypeField = value;
    }

    public DateTime UpdateDate
    {
      get => this.updateDate;
      set => this.updateDate = value;
    }

    public CoreFieldsType CoreFields
    {
      get => this.coreFieldsField;
      set => this.coreFieldsField = value;
    }

    public TextField[] TextFields
    {
      get => this.textFieldsField;
      set => this.textFieldsField = value;
    }

    public ChangedFieldsType ChangedFields
    {
      get => this.changedFieldsField;
      set => this.changedFieldsField = value;
    }

    public FieldValue[] FieldValues
    {
      get => this.fieldValues;
      set => this.fieldValues = value;
    }

    public AddedFile[] AddedFiles
    {
      get => this.addedFilesField;
      set => this.addedFilesField = value;
    }

    public DeletedFile[] DeletedFiles
    {
      get => this.deletedFilesField;
      set => this.deletedFilesField = value;
    }

    public AddedResourceLink[] AddedResourceLinks
    {
      get => this.addedResourceLinksField;
      set => this.addedResourceLinksField = value;
    }

    public DeletedResourceLink[] DeletedResourceLinks
    {
      get => this.deletedResourceLinksField;
      set => this.deletedResourceLinksField = value;
    }

    public ChangedResourceLink[] ChangedResourceLinks
    {
      get => this.changedResourceLinksField;
      set => this.changedResourceLinksField = value;
    }

    public AddedRelation[] AddedRelations
    {
      get => this.addedRelationsField;
      set => this.addedRelationsField = value;
    }

    public DeletedRelation[] DeletedRelations
    {
      get => this.deletedRelationsField;
      set => this.deletedRelationsField = value;
    }

    public ChangedRelation[] ChangedRelations
    {
      get => this.changedRelationsField;
      set => this.changedRelationsField = value;
    }

    public ResourceTypeData[] AddedResourceLinkTypes
    {
      get => this.addedResourceLinkTypes;
      set => this.addedResourceLinkTypes = value;
    }

    public ResourceTypeData[] ChangedResourceLinkTypes
    {
      get => this.changedResourceLinkTypes;
      set => this.changedResourceLinkTypes = value;
    }

    public ResourceTypeData[] DeletedResourceLinkTypes
    {
      get => this.deletedResourceLinkTypes;
      set => this.deletedResourceLinkTypes = value;
    }

    public WorkItemCommentData WorkItemComment
    {
      get => this.workItemComment;
      set => this.workItemComment = value;
    }

    [XmlIgnore]
    internal string ArtifactUri
    {
      get => this.artifactUri;
      set => this.artifactUri = value;
    }

    [XmlIgnore]
    internal List<EventActor> Actors
    {
      get
      {
        if (this.actors == null)
          this.actors = new List<EventActor>();
        return this.actors;
      }
    }

    [XmlIgnore]
    internal DateTime ChangedDateUtc
    {
      get => this.changedDateUtc;
      set => this.changedDateUtc = value;
    }

    [XmlIgnore]
    internal Guid ProjectId
    {
      get => this.projectId;
      set => this.projectId = value;
    }
  }
}
