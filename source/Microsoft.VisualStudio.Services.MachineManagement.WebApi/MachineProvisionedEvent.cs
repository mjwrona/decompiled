// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineProvisionedEvent
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  internal sealed class MachineProvisionedEvent : MachineInstanceOrchestrationEvent
  {
    internal MachineProvisionedEvent()
      : base("Provisioned")
    {
    }

    public MachineProvisionedEvent(int poolId, string instanceName)
      : base("Provisioned", poolId, instanceName)
    {
    }
  }
}
