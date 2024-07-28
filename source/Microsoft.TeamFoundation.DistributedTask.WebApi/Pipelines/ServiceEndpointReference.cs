// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ServiceEndpointReference : ResourceReference
  {
    public ServiceEndpointReference()
    {
    }

    private ServiceEndpointReference(ServiceEndpointReference referenceToCopy)
      : base((ResourceReference) referenceToCopy)
    {
      this.Id = referenceToCopy.Id;
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    public ServiceEndpointReference Clone() => new ServiceEndpointReference(this);

    public override string ToString() => base.ToString() ?? this.Id.ToString("D");
  }
}
