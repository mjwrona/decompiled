// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationNotificationBinder
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class TeamFoundationNotificationBinder : ObjectBinder<TeamFoundationNotification>
  {
    protected SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");
    protected SqlColumnBinder NotificationMetadataColumn = new SqlColumnBinder("NotificationMetadata");
    protected SqlColumnBinder SubscriptionChannelColumn = new SqlColumnBinder("SubscriptionChannel");
    protected SqlColumnBinder SubscriptionMatcherColumn = new SqlColumnBinder("SubscriptionMatcher");
    protected SqlColumnBinder EventTypeNameColumn = new SqlColumnBinder("EventTypeName");
    protected SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    protected SqlColumnBinder RetryCountColumn = new SqlColumnBinder("RetryCount");
    protected SqlColumnBinder EventIdColumn = new SqlColumnBinder("EventId");
    protected SqlColumnBinder EventColumn = new SqlColumnBinder("Event");
    protected SqlColumnBinder EventTypeColumn = new SqlColumnBinder("EventType");
    protected SqlColumnBinder SubscriptionIdColumn = new SqlColumnBinder("SubscriptionId");
    protected SqlColumnBinder SubscriptionEventTypeColumn = new SqlColumnBinder("SubscriptionEventType");
    protected SqlColumnBinder SubscriptionExpressionColumn = new SqlColumnBinder("SubscriptionExpression");
    protected SqlColumnBinder SubscriptionSubscriberIdColumn = new SqlColumnBinder("SubscriptionSubscriberId");
    protected SqlColumnBinder SubscriptionTagColumn = new SqlColumnBinder("SubscriptionClassification");
    protected SqlColumnBinder SubscriptionAddressColumn = new SqlColumnBinder("SubscriptionAddress");
    protected SqlColumnBinder SubscriberOverrideIdColumn = new SqlColumnBinder("SubscriberOverrideId");
    protected SqlColumnBinder ChannelColumn = new SqlColumnBinder("Channel");
    protected SqlColumnBinder MetadataColumn = new SqlColumnBinder("Metadata");
    protected SqlColumnBinder CreatedTimeColumn = new SqlColumnBinder("CreatedTime");
    protected SqlColumnBinder EventCreatedTimeColumn = new SqlColumnBinder("EventCreatedTime");
    protected SqlColumnBinder ProcessQueueColumn = new SqlColumnBinder("ProcessQueue");
    protected SqlColumnBinder BisEventColumn = new SqlColumnBinder("BisEvent");

    internal TeamFoundationNotificationBinder(TeamFoundationSqlResourceComponent component) => this.Component = component;

    internal TeamFoundationSqlResourceComponent Component { get; set; }

    internal IVssRequestContext RequestContext { get; set; }

    protected override TeamFoundationNotification Bind() => this.BindNotification();

    protected virtual TeamFoundationNotification BindNotification()
    {
      string eventType = this.EventTypeNameColumn.GetString((IDataReader) this.Reader, true);
      string eventData = this.EventColumn.GetString((IDataReader) this.Reader, true);
      TeamFoundationEvent teamFoundationEvent = (TeamFoundationEvent) null;
      if (!string.IsNullOrEmpty(eventType))
      {
        if (!string.IsNullOrEmpty(eventData))
        {
          try
          {
            teamFoundationEvent = TeamFoundationEventFactory.GetEvent(eventType, eventData);
            teamFoundationEvent.Id = this.EventIdColumn.GetInt32((IDataReader) this.Reader);
            teamFoundationEvent.CreatedTime = this.EventCreatedTimeColumn.GetDateTime((IDataReader) this.Reader);
          }
          catch (Exception ex)
          {
            teamFoundationEvent = (TeamFoundationEvent) null;
          }
        }
      }
      return new TeamFoundationNotification()
      {
        Id = this.IdColumn.GetInt32((IDataReader) this.Reader),
        Attempts = 5 - this.RetryCountColumn.GetInt32((IDataReader) this.Reader),
        Event = teamFoundationEvent,
        Channel = this.ChannelColumn.GetString((IDataReader) this.Reader, false),
        Metadata = this.MetadataColumnBinder.GetString((IDataReader) this.Reader, true) ?? string.Empty,
        CreatedTime = this.CreatedTimeColumn.GetDateTime((IDataReader) this.Reader),
        ProcessQueue = this.ProcessQueueColumn.GetString((IDataReader) this.Reader, string.Empty),
        SingleSubscriberOverrideId = this.SubscriberOverrideIdColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty),
        BisEvent = this.BisEventColumn.GetInt32((IDataReader) this.Reader, -2, -1)
      };
    }

    internal static void PostBind(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationNotification> notifications)
    {
      foreach (TeamFoundationNotification notification in notifications)
      {
        string str = (string) null;
        if (!string.IsNullOrEmpty(notification.Metadata))
        {
          try
          {
            notification.DeliveryDetails = JsonConvert.DeserializeObject<NotificationDeliveryDetails>(notification.Metadata, NotificationsSerialization.JsonSerializerSettings);
          }
          catch (Exception ex)
          {
            str = ExceptionUtil.FormatException(ex);
            requestContext.Trace(1002025, TraceLevel.Error, "Notifications", "SqlComponent", "Notification {0} bind exception: {1}", (object) notification.Id, (object) str);
          }
        }
        if (notification.DeliveryDetails == null)
        {
          notification.DeliveryDetails = new NotificationDeliveryDetails();
          notification.DeliveryDetails.Description = str ?? "Missing metadata";
        }
        if (!notification.SingleSubscriberOverrideId.Equals(Guid.Empty) && !notification.DeliveryDetails.Recipients.Any<NotificationRecipient>())
          notification.DeliveryDetails.Recipients.Add(new NotificationRecipient()
          {
            Id = notification.SingleSubscriberOverrideId,
            Address = notification.DeliveryDetails.DeliveryAddress
          });
      }
    }

    protected ref SqlColumnBinder MetadataColumnBinder => ref this.NotificationMetadataColumn;
  }
}
