// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequestStartedEvent
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  internal sealed class MachineRequestStartedEvent : MachineInstanceOrchestrationEvent
  {
    internal MachineRequestStartedEvent()
      : base("RequestStarted")
    {
    }

    public MachineRequestStartedEvent(
      int poolId,
      string instanceName,
      long requestId,
      Guid requestActivityId,
      int requestTimeout)
      : base("RequestStarted", poolId, instanceName)
    {
      this.RequestId = requestId;
      this.RequestActivityId = requestActivityId;
      this.RequestTimeout = requestTimeout;
    }

    [DataMember]
    public long RequestId { get; private set; }

    [DataMember]
    public Guid RequestActivityId { get; private set; }

    [DataMember]
    public int RequestTimeout { get; private set; }
  }
}
