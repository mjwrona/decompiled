// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionsQuery
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  public class SubscriptionsQuery
  {
    [DataMember(EmitDefaultValue = false)]
    public string PublisherId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid SubscriberId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ConsumerId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ConsumerActionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string EventType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<InputFilter> PublisherInputFilters { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<InputFilter> ConsumerInputFilters { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Subscription> Results { get; set; }
  }
}
