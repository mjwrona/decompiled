// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.SubscriptionsCollectionModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts
{
  [DataContract]
  internal class SubscriptionsCollectionModel
  {
    [DataMember(Name = "subscriptions")]
    public IEnumerable<SubscriptionModel> Subscriptions { get; set; }

    [DataMember(Name = "subscribers")]
    public IEnumerable<SubscriberInfoModel> Subscribers { get; set; }
  }
}
