// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  [DebuggerDisplay("ID:{Id}, {EventType} -> {ConsumerId}.{ConsumerActionId}, {CreatedDate.ToLocalTime()}")]
  public class Subscription
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public SubscriptionStatus Status { get; set; }

    [DataMember]
    public string PublisherId { get; set; }

    [DataMember]
    public string EventType { get; set; }

    [DataMember]
    public IdentityRef Subscriber { get; set; }

    [DataMember]
    public string ResourceVersion { get; set; }

    [DataMember]
    public string EventDescription { get; set; }

    [DataMember]
    public string ConsumerId { get; set; }

    [DataMember]
    public string ConsumerActionId { get; set; }

    [DataMember]
    public string ActionDescription { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public byte ProbationRetries { get; set; }

    [DataMember]
    public IdentityRef CreatedBy { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember]
    public DateTime ModifiedDate { get; set; }

    [DataMember]
    public DateTime LastProbationRetryDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> PublisherInputs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> ConsumerInputs { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }

    internal bool IsLocal { get; set; }
  }
}
