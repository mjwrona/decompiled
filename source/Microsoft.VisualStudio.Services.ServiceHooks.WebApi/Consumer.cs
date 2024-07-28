// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Consumer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [DataContract]
  [DebuggerDisplay("{Name}")]
  public class Consumer
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ImageUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string InformationUrl { get; set; }

    [DataMember]
    public AuthenticationType AuthenticationType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ExternalConfigurationDescriptor ExternalConfiguration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<InputDescriptor> InputDescriptors { get; set; }

    [DataMember]
    public IList<ConsumerAction> Actions { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }
  }
}
