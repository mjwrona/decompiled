// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.NotificationDetails
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  [DebuggerDisplay("{PublisherId}.{EventType} -> {ConsumerId},{ConsumerActionId}")]
  public class NotificationDetails
  {
    [DataMember]
    public string EventType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Event Event { get; set; }

    [DataMember]
    public string PublisherId { get; set; }

    [DataMember]
    public string ConsumerId { get; set; }

    [DataMember]
    public string ConsumerActionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> ConsumerInputs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> PublisherInputs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Request { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Response { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ErrorMessage { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ErrorDetail { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? QueuedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? DequeuedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? ProcessedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CompletedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public double? RequestDuration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int RequestAttempts { get; set; }
  }
}
