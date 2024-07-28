// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineInstanceOrchestrationEvent
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  [KnownType(typeof (MachineInstanceOrchestrationEvent))]
  [JsonConverter(typeof (MachineEventJsonConverter))]
  public abstract class MachineInstanceOrchestrationEvent : MachineOrchestrationEvent
  {
    protected MachineInstanceOrchestrationEvent(string name)
      : base(name)
    {
    }

    protected MachineInstanceOrchestrationEvent(string name, int poolId, string instanceName)
      : base(name)
    {
      this.PoolId = poolId;
      this.InstanceName = instanceName;
    }

    [DataMember]
    public int PoolId { get; set; }

    [DataMember]
    public string InstanceName { get; set; }
  }
}
