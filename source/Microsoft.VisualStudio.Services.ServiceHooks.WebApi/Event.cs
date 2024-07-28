// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Event
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  public class Event
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string EventType { get; set; }

    [DataMember]
    public string PublisherId { get; set; }

    [DataMember]
    public FormattedEventMessage Message { get; set; }

    [DataMember]
    public FormattedEventMessage DetailedMessage { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public object Resource { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResourceVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, ResourceContainer> ResourceContainers { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public SessionToken SessionToken { get; set; }
  }
}
