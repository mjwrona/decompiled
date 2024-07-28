// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineAuthorization
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract(Name = "Authorization")]
  public sealed class MachineAuthorization
  {
    [DataMember(IsRequired = true, Order = 0)]
    public byte[] AccessToken { get; set; }

    [DataMember(IsRequired = true, Order = 1)]
    public string PoolType { get; set; }

    [DataMember(IsRequired = true, Order = 2)]
    public string PoolName { get; set; }

    [DataMember(IsRequired = true, Order = 3)]
    public string InstanceName { get; set; }
  }
}
