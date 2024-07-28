// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IEventService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface IEventService
  {
    int SubscribeEvent(string eventType, string filterExpression, DeliveryPreference preferences);

    int SubscribeEvent(
      string eventType,
      string filterExpression,
      DeliveryPreference preferences,
      string classification);

    int SubscribeEvent(
      string userId,
      string eventType,
      string filterExpression,
      DeliveryPreference preferences);

    int SubscribeEvent(
      string userId,
      string eventType,
      string filterExpression,
      DeliveryPreference preferences,
      string classification,
      string projectName = null);

    void UnsubscribeEvent(int subscriptionId, string projectName = null);

    Subscription[] GetAllEventSubscriptions();

    Subscription[] GetAllEventSubscriptions(string classification, string projectName = null);

    Subscription[] GetEventSubscriptions(string user);

    Subscription[] GetEventSubscriptions(IdentityDescriptor user);

    Subscription[] GetEventSubscriptions(string user, string classification, string projectName = null);

    Subscription[] GetEventSubscriptions(
      IdentityDescriptor user,
      string classification,
      string projectName = null);

    void FireEvent(object theEvent);

    void FireEvents(IEnumerable<object> theEvents);

    [Obsolete("Use one of the GetEventSubscriptions() overloads")]
    Subscription[] EventSubscriptions(string userId);

    [Obsolete("Use one of the GetEventSubscriptions() overloads")]
    Subscription[] EventSubscriptions(string userId, string tag);

    [Obsolete("Use the FireEvent() overloads")]
    void FireAsyncEvent(string theEvent);

    [Obsolete("Use the FireEvent() overloads")]
    void FireAsyncEvent(object theEvent);

    [Obsolete("Use the FireEvent() overloads")]
    void FireBulkAsyncEvents(string[] theEvents);

    [Obsolete("Use the FireEvent() overloads")]
    void FireBulkAsyncEvents(object[] theEvents);
  }
}
