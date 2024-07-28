// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.NotificationsQuery
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  public class NotificationsQuery
  {
    [DataMember(EmitDefaultValue = false)]
    public string PublisherId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Guid> SubscriptionIds { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? MaxResults { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? MaxResultsPerSubscription { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public NotificationStatus? Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public NotificationResult? ResultType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? MinCreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? MaxCreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Notification> Results { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Subscription> AssociatedSubscriptions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<NotificationSummary> Summary { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? IncludeDetails { get; set; }
  }
}
