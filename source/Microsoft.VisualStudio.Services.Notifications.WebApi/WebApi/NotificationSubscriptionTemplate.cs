// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationSubscriptionTemplate
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class NotificationSubscriptionTemplate
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = true)]
    [JsonConverter(typeof (SubscriptionFilterConverter))]
    public ISubscriptionFilter Filter { get; set; }

    [DataMember]
    public NotificationEventType NotificationEventInformation { get; set; }

    [DataMember]
    public SubscriptionTemplateType Type { get; set; }
  }
}
