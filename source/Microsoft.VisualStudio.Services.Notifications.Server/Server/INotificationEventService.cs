// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.INotificationEventService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [DefaultServiceImplementation(typeof (NotificationEventService))]
  public interface INotificationEventService : IVssFrameworkService
  {
    List<NotificationEventType> GetEventTypes(
      IVssRequestContext requestContext,
      EventTypeQueryFlags queryFlags = EventTypeQueryFlags.None);

    Dictionary<string, NotificationEventType> GetKeyedEventTypes(IVssRequestContext requestContext);

    NotificationEventType GetEventType(
      IVssRequestContext requestContext,
      string eventType,
      EventTypeQueryFlags queryFlags = EventTypeQueryFlags.None);

    EventSerializerType GetSerializationFormatForEvent(
      IVssRequestContext requestContext,
      string eventType);

    IEnumerable<NotificationEventPublisher> GetPublishers(
      IVssRequestContext requestContext,
      EventPublisherQueryFlags queryFlags = EventPublisherQueryFlags.None);

    IEnumerable<NotificationEventType> GetPublisherEventTypes(
      IVssRequestContext requestContext,
      string publisherId);

    void PublishEvent(IVssRequestContext requestContext, VssNotificationEvent theEvent);

    void PublishEvents(
      IVssRequestContext requestContext,
      IEnumerable<VssNotificationEvent> theEvents,
      bool allowDuringServicing = false);

    void PublishSystemEvent(IVssRequestContext requestContext, VssNotificationEvent theEvent);

    void PublishSystemEvents(
      IVssRequestContext requestContext,
      IEnumerable<VssNotificationEvent> theEvents,
      bool allowDuringServicing = false);

    bool IsValidEventType(IVssRequestContext requestContext, string eventType);

    IList<NotificationEventField> GetInputValues(
      IVssRequestContext requestContext,
      string eventType,
      FieldValuesQuery query);
  }
}
