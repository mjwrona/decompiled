// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolAlias
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachinePoolAlias
  {
    internal MachinePoolAlias()
    {
    }

    public MachinePoolAlias(
      string poolTypeName,
      string poolName,
      string newPoolTypeName,
      string newPoolName)
    {
      this.PoolType = poolTypeName;
      this.PoolName = poolName;
      this.AliasPoolType = newPoolTypeName;
      this.AliasPoolName = newPoolName;
    }

    [DataMember(IsRequired = true)]
    public string PoolType { get; set; }

    [DataMember(IsRequired = true)]
    public string PoolName { get; set; }

    [DataMember(IsRequired = true)]
    public string AliasPoolType { get; set; }

    [DataMember(IsRequired = true)]
    public string AliasPoolName { get; set; }
  }
}
