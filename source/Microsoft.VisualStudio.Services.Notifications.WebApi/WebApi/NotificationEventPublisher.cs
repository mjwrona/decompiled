// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationEventPublisher
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class NotificationEventPublisher
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Url { get; set; }

    [DataMember]
    public SubscriptionManagement SubscriptionManagementInfo { get; set; }

    public NotificationEventPublisher() => this.SubscriptionManagementInfo = new SubscriptionManagement();
  }
}
