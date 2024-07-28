// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionQueryCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class SubscriptionQueryCondition
  {
    [DataMember]
    public string SubscriptionId { get; set; }

    [DataMember]
    public Guid? SubscriberId { get; set; }

    [DataMember]
    public string Scope { get; set; }

    [DataMember]
    [JsonConverter(typeof (SubscriptionFilterConverter))]
    public ISubscriptionFilter Filter { get; set; }

    [DataMember]
    public SubscriptionFlags? Flags { get; set; }
  }
}
