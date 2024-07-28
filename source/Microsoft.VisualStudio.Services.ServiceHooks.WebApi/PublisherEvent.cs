// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.PublisherEvent
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  public class PublisherEvent
  {
    [DataMember]
    public Event Event { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<InputFilter> PublisherInputFilters { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<VersionedResource> OtherResourceVersions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Subscription Subscription { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> Diagnostics { get; set; }

    [DataMember]
    public bool IsFilteredEvent { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> NotificationData { get; set; }
  }
}
