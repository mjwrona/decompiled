// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationSubscriptionsViewData
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  [ClientIncludeModel(RestClientLanguages.TypeScript)]
  public class NotificationSubscriptionsViewData
  {
    [DataMember]
    public IDictionary<string, NotificationEventPublisher> Publishers { get; set; }

    [DataMember]
    public IDictionary<string, NotificationEventType> EventTypes { get; set; }

    [DataMember]
    public IList<NotificationSubscription> Subscriptions { get; set; }

    [DataMember]
    public IDictionary<string, SubscriptionScope> Scopes { get; set; }

    [DataMember]
    public IDictionary<string, string> MapEventTypeToPublisherId { get; set; }

    [DataMember]
    public IDictionary<string, List<NotificationSubscriptionTemplate>> MapCategoryIdToSubscriptionTemplates { get; set; }

    [DataMember]
    public IDictionary<string, string> MapCategoryIdToCategoryName { get; set; }

    [DataMember]
    public SubscriptionEvaluationSettings SubsEvaluationSettings { get; set; }

    [DataMember]
    public IDictionary<Guid, NotificationSubscriber> Subscribers { get; set; }
  }
}
