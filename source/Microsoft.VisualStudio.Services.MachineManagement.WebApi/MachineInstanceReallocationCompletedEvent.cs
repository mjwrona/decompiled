// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineInstanceReallocationCompletedEvent
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachineInstanceReallocationCompletedEvent : MachinePoolOrchestrationEvent
  {
    internal MachineInstanceReallocationCompletedEvent(
      string poolTypeName,
      string poolName,
      string instanceName)
      : base("MachineInstanceReallocationCompleted")
    {
      this.InstanceName = instanceName;
      this.PoolName = poolName;
      this.PoolTypeName = poolTypeName;
    }

    internal MachineInstanceReallocationCompletedEvent(string poolName, string instanceName)
      : base("MachineInstanceReallocationCompleted")
    {
      this.InstanceName = instanceName;
      this.PoolName = poolName;
      this.PoolTypeName = (string) null;
    }

    internal MachineInstanceReallocationCompletedEvent()
      : base("MachineInstanceReallocationCompleted")
    {
      this.InstanceName = (string) null;
      this.PoolName = (string) null;
      this.PoolTypeName = (string) null;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string PoolTypeName { get; private set; }

    [DataMember]
    public string PoolName { get; private set; }

    [DataMember]
    public string InstanceName { get; private set; }
  }
}
