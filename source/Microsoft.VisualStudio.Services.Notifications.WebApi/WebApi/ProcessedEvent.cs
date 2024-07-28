// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.ProcessedEvent
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class ProcessedEvent
  {
    [DataMember]
    public int EventId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ArtifactUri { get; set; }

    [DataMember]
    public List<EventActor> Actors { get; } = new List<EventActor>();

    [DataMember]
    public List<EventActor> Inclusions { get; } = new List<EventActor>();

    [DataMember]
    public List<EventActor> Exclusions { get; } = new List<EventActor>();

    [DataMember(EmitDefaultValue = false)]
    public string AllowedChannels { get; set; }

    [DataMember]
    public Dictionary<Guid, SubscriptionEvaluation> Evaluations { get; } = new Dictionary<Guid, SubscriptionEvaluation>();

    [DataMember]
    public List<GeneratedNotification> Notifications { get; } = new List<GeneratedNotification>();

    [DataMember]
    public ProcessingIdentities DeliveryIdentities { get; set; } = new ProcessingIdentities();
  }
}
