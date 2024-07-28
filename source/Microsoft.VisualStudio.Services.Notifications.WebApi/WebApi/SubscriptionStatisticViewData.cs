// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatisticViewData
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
  public class SubscriptionStatisticViewData
  {
    public SubscriptionStatisticViewData()
    {
      this.Statistics = (IDictionary<int, List<NotificationStatistic>>) new Dictionary<int, List<NotificationStatistic>>();
      this.Subscriptions = (IDictionary<string, NotificationSubscription>) new Dictionary<string, NotificationSubscription>();
      this.Events = (IDictionary<string, NotificationEventType>) new Dictionary<string, NotificationEventType>();
    }

    [DataMember]
    public IDictionary<int, List<NotificationStatistic>> Statistics { get; private set; }

    [DataMember]
    public IDictionary<string, NotificationSubscription> Subscriptions { get; private set; }

    [DataMember]
    public IDictionary<string, NotificationEventType> Events { get; private set; }

    [DataMember]
    public bool IsAdmin { get; set; }

    [DataMember]
    public DateTime QueryDate { get; set; }
  }
}
