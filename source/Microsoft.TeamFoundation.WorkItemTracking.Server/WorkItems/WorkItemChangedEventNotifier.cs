// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemChangedEventNotifier
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemChangedEventNotifier : IWorkItemChangedEventNotifier
  {
    private ServiceFactory<INotificationEventService> m_notificationEventServiceFactory;
    private const string s_StackRank = "Microsoft.VSTS.Common.StackRank";
    private const string s_BacklogPriority = "Microsoft.VSTS.Common.BacklogPriority";
    private static readonly string[] s_NotificationBlackList = new string[8]
    {
      "System.Rev",
      "System.Watermark",
      "System.ChangedDate",
      "Microsoft.VSTS.Common.StackRank",
      "Microsoft.VSTS.Common.BacklogPriority",
      "System.AuthorizedDate",
      "System.PersonId",
      "System.ChangedBy"
    };

    internal static string[] NotificationBlackList => WorkItemChangedEventNotifier.s_NotificationBlackList;

    public WorkItemChangedEventNotifier() => this.m_notificationEventServiceFactory = (ServiceFactory<INotificationEventService>) (context => context.GetService<INotificationEventService>());

    internal WorkItemChangedEventNotifier(INotificationEventService notificationEventService) => this.m_notificationEventServiceFactory = (ServiceFactory<INotificationEventService>) (context => notificationEventService);

    public void NotifyChanges(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemChangedEventExtended> events)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemChangedEventExtended>>(events, nameof (events));
      if (!events.Any<WorkItemChangedEventExtended>())
        return;
      this.RemoveHistoryDisabledFields(requestContext, events);
      IEnumerable<WorkItemChangedEvent> source = this.FilterValidEvents(requestContext, events);
      if (!source.Any<WorkItemChangedEvent>())
        return;
      INotificationEventService notificationEventService = this.m_notificationEventServiceFactory(requestContext);
      IEnumerable<VssNotificationEvent> notificationEvents = source.Select<WorkItemChangedEvent, VssNotificationEvent>((Func<WorkItemChangedEvent, VssNotificationEvent>) (e => e.ToVssNotificationEvent()));
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<VssNotificationEvent> theEvents = notificationEvents;
      notificationEventService.PublishEvents(requestContext1, theEvents);
    }

    private bool IsHistoryDisabledField(IFieldTypeDictionary fieldDictionary, string referenceName)
    {
      FieldEntry field;
      return fieldDictionary.TryGetFieldByNameOrId(referenceName, out field) && fieldDictionary.GetHistoryDisabledFieldIds().Contains(field.FieldId);
    }

    private void RemoveHistoryDisabledFields(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemChangedEventExtended> events)
    {
      if (!WorkItemTrackingFeatureFlags.IsNoHistoryEnabledFieldsSupported(requestContext))
        return;
      IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
      foreach (WorkItemChangedEvent itemChangedEvent in events.Select<WorkItemChangedEventExtended, WorkItemChangedEvent>((Func<WorkItemChangedEventExtended, WorkItemChangedEvent>) (e => e.LegacyChangedEvent)))
      {
        ChangedFieldsType changedFields = itemChangedEvent.ChangedFields;
        if (changedFields != null)
        {
          if (changedFields.BooleanFields != null && ((IEnumerable<BooleanField>) changedFields.BooleanFields).Any<BooleanField>((Func<BooleanField, bool>) (f => this.IsHistoryDisabledField(fieldDictionary, f.ReferenceName))))
            changedFields.BooleanFields = ((IEnumerable<BooleanField>) changedFields.BooleanFields).Where<BooleanField>((Func<BooleanField, bool>) (f => !this.IsHistoryDisabledField(fieldDictionary, f.ReferenceName))).ToArray<BooleanField>();
          if (changedFields.IntegerFields != null && ((IEnumerable<IntegerField>) changedFields.IntegerFields).Any<IntegerField>((Func<IntegerField, bool>) (f => this.IsHistoryDisabledField(fieldDictionary, f.ReferenceName))))
            changedFields.IntegerFields = ((IEnumerable<IntegerField>) changedFields.IntegerFields).Where<IntegerField>((Func<IntegerField, bool>) (f => !this.IsHistoryDisabledField(fieldDictionary, f.ReferenceName))).ToArray<IntegerField>();
          if (changedFields.StringFields != null && ((IEnumerable<StringField>) changedFields.StringFields).Any<StringField>((Func<StringField, bool>) (f => this.IsHistoryDisabledField(fieldDictionary, f.ReferenceName))))
            changedFields.StringFields = ((IEnumerable<StringField>) changedFields.StringFields).Where<StringField>((Func<StringField, bool>) (f => !this.IsHistoryDisabledField(fieldDictionary, f.ReferenceName))).ToArray<StringField>();
        }
        if (itemChangedEvent.TextFields != null && ((IEnumerable<TextField>) itemChangedEvent.TextFields).Any<TextField>((Func<TextField, bool>) (f => this.IsHistoryDisabledField(fieldDictionary, f.ReferenceName))))
          itemChangedEvent.TextFields = ((IEnumerable<TextField>) itemChangedEvent.TextFields).Where<TextField>((Func<TextField, bool>) (f => !this.IsHistoryDisabledField(fieldDictionary, f.ReferenceName))).ToArray<TextField>();
      }
    }

    private IEnumerable<WorkItemChangedEvent> FilterValidEvents(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemChangedEventExtended> events)
    {
      if (!requestContext.IsFeatureEnabled("WorkItemTracking.Server.NotificationFilter"))
        return (IEnumerable<WorkItemChangedEvent>) events.Select<WorkItemChangedEventExtended, WorkItemChangedEvent>((Func<WorkItemChangedEventExtended, WorkItemChangedEvent>) (x => x.LegacyChangedEvent)).ToArray<WorkItemChangedEvent>();
      int num = events.Count<WorkItemChangedEventExtended>((Func<WorkItemChangedEventExtended, bool>) (e => e.LegacyChangedEvent.SuppressNotifications));
      if (num > 0)
      {
        CustomerIntelligenceData data = new CustomerIntelligenceData();
        data.Add("suppressedEventCount", (double) num);
        this.PublishCI(requestContext, "SuppressNotifications", data);
      }
      IEnumerable<WorkItemChangedEvent> array = (IEnumerable<WorkItemChangedEvent>) events.Where<WorkItemChangedEventExtended>((Func<WorkItemChangedEventExtended, bool>) (x => !this.IsFiltered(x.LegacyChangedEvent))).Select<WorkItemChangedEventExtended, WorkItemChangedEvent>((Func<WorkItemChangedEventExtended, WorkItemChangedEvent>) (x => x.LegacyChangedEvent)).ToArray<WorkItemChangedEvent>();
      return this.FilterBulkLinkUpdates(requestContext, array);
    }

    private IEnumerable<WorkItemChangedEvent> FilterBulkLinkUpdates(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemChangedEvent> eventServiceEvents)
    {
      int notificationThreshold = this.GetLinkUpdateNotificationThreshold(requestContext);
      IEnumerable<WorkItemChangedEvent> itemChangedEvents1 = eventServiceEvents;
      if (notificationThreshold > 0)
      {
        IEnumerable<WorkItemChangedEvent> itemChangedEvents2 = eventServiceEvents.Where<WorkItemChangedEvent>((Func<WorkItemChangedEvent, bool>) (e => !e.HasOnlyLinkUpdates || e.ChangedFields != null));
        int num = eventServiceEvents.Count<WorkItemChangedEvent>() - itemChangedEvents2.Count<WorkItemChangedEvent>();
        if (num > notificationThreshold)
        {
          itemChangedEvents1 = itemChangedEvents2;
          IEnumerable<string> values = eventServiceEvents.Except<WorkItemChangedEvent>(itemChangedEvents2).Select<WorkItemChangedEvent, string>((Func<WorkItemChangedEvent, string>) (t => t.WorkItemId));
          CustomerIntelligenceData data = new CustomerIntelligenceData();
          data.Add("linkEvents", (double) num);
          data.Add("workItemIdsWithLinkUpdates", string.Join(",", values));
          data.Add("linkUpdateNotificationThreshold", (double) notificationThreshold);
          this.PublishCI(requestContext, nameof (FilterBulkLinkUpdates), data);
        }
      }
      return itemChangedEvents1;
    }

    private void PublishCI(
      IVssRequestContext requestContext,
      string feature,
      CustomerIntelligenceData data)
    {
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, feature, data);
    }

    private int GetLinkUpdateNotificationThreshold(IVssRequestContext requestContext) => requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).LinkUpdateNotificationThreshold;

    private bool IsFiltered(WorkItemChangedEvent workItemChangedEvent)
    {
      if (workItemChangedEvent.SuppressNotifications)
        return true;
      if (workItemChangedEvent.AddedFiles != null && ((IEnumerable<AddedFile>) workItemChangedEvent.AddedFiles).Any<AddedFile>() || workItemChangedEvent.DeletedFiles != null && ((IEnumerable<DeletedFile>) workItemChangedEvent.DeletedFiles).Any<DeletedFile>() || workItemChangedEvent.AddedResourceLinks != null && ((IEnumerable<AddedResourceLink>) workItemChangedEvent.AddedResourceLinks).Any<AddedResourceLink>() || workItemChangedEvent.DeletedResourceLinks != null && ((IEnumerable<DeletedResourceLink>) workItemChangedEvent.DeletedResourceLinks).Any<DeletedResourceLink>() || workItemChangedEvent.ChangedResourceLinks != null && ((IEnumerable<ChangedResourceLink>) workItemChangedEvent.ChangedResourceLinks).Any<ChangedResourceLink>() || workItemChangedEvent.AddedRelations != null && ((IEnumerable<AddedRelation>) workItemChangedEvent.AddedRelations).Any<AddedRelation>() || workItemChangedEvent.DeletedRelations != null && ((IEnumerable<DeletedRelation>) workItemChangedEvent.DeletedRelations).Any<DeletedRelation>() || workItemChangedEvent.ChangedRelations != null && ((IEnumerable<ChangedRelation>) workItemChangedEvent.ChangedRelations).Any<ChangedRelation>())
        return false;
      List<string> source = new List<string>();
      if (workItemChangedEvent.ChangedFields != null)
      {
        if (workItemChangedEvent.ChangedFields.BooleanFields != null)
          source.AddRange(((IEnumerable<BooleanField>) workItemChangedEvent.ChangedFields.BooleanFields).Select<BooleanField, string>((Func<BooleanField, string>) (x => x.ReferenceName)));
        if (workItemChangedEvent.ChangedFields.IntegerFields != null)
          source.AddRange(((IEnumerable<IntegerField>) workItemChangedEvent.ChangedFields.IntegerFields).Select<IntegerField, string>((Func<IntegerField, string>) (x => x.ReferenceName)));
        if (workItemChangedEvent.ChangedFields.StringFields != null)
          source.AddRange(((IEnumerable<StringField>) workItemChangedEvent.ChangedFields.StringFields).Select<StringField, string>((Func<StringField, string>) (x => x.ReferenceName)));
      }
      if (workItemChangedEvent.TextFields != null)
        source.AddRange(((IEnumerable<TextField>) workItemChangedEvent.TextFields).Select<TextField, string>((Func<TextField, string>) (x => x.ReferenceName)));
      if (source.Count == 0)
        return true;
      return source.Any<string>((Func<string, bool>) (x => x.Equals("Microsoft.VSTS.Common.StackRank", StringComparison.OrdinalIgnoreCase) || x.Equals("Microsoft.VSTS.Common.BacklogPriority", StringComparison.OrdinalIgnoreCase))) && source.All<string>((Func<string, bool>) (x => ((IEnumerable<string>) WorkItemChangedEventNotifier.s_NotificationBlackList).Contains<string>(x)));
    }
  }
}
