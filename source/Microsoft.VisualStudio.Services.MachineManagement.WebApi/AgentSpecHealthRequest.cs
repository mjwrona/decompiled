// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.AgentSpecHealthRequest
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public class AgentSpecHealthRequest
  {
    [DataMember(IsRequired = true)]
    public string Geography { get; set; }

    [DataMember(IsRequired = false)]
    public string DeploymentRealm { get; set; }

    [DataMember(IsRequired = true)]
    public AgentSpec AgentSpec { get; set; }

    [DataMember(IsRequired = false)]
    public Guid SourceDeploymentInstance { get; set; }

    [DataMember(IsRequired = false)]
    public bool DisableNestedVirtualization { get; set; }
  }
}
