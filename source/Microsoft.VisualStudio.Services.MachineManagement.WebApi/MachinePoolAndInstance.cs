// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolAndInstance
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachinePoolAndInstance
  {
    public MachinePoolAndInstance(MachinePool machinePool, MachineInstance machineInstance)
    {
      this.MachinePool = machinePool;
      this.MachineInstance = machineInstance;
    }

    [DataMember(IsRequired = true)]
    public MachinePool MachinePool { get; set; }

    [DataMember(IsRequired = true)]
    public MachineInstance MachineInstance { get; set; }
  }
}
