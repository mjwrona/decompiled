// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  [DebuggerDisplay("{Id} Status:{Status} Result:{Result}")]
  public class Notification
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public Guid SubscriptionId { get; set; }

    [DataMember]
    public Guid SubscriberId { get; set; }

    [DataMember]
    public Guid EventId { get; set; }

    [DataMember]
    public NotificationStatus Status { get; set; }

    [DataMember]
    public NotificationResult Result { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public DateTime ModifiedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public NotificationDetails Details { get; set; }
  }
}
