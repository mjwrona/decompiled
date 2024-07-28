// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.AgentSpecHealth
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Runtime.Serialization;
using System.Threading;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public class AgentSpecHealth
  {
    private int m_readCount;

    [DataMember]
    public bool DeploymentRequestsAllowed { get; set; }

    [DataMember]
    public bool AgentSpecSupported { get; internal set; }

    [DataMember]
    public bool HadError { get; internal set; }

    [DataMember]
    public int MachinesAvailable { get; set; }

    [DataMember]
    public int WorkersAvailable { get; set; }

    [DataMember]
    public int AllocatedWorkerCount { get; set; }

    [DataMember]
    public int MaxWorkerCount { get; set; }

    [DataMember]
    public int CountHeldInReserve { get; set; }

    public bool IsExpired(int maxReadCount) => this.DeploymentRequestsAllowed && this.AgentSpecSupported && (Interlocked.Increment(ref this.m_readCount) >= maxReadCount || this.MachinesAvailable != 0 && maxReadCount * 2 > this.MachinesAvailable);

    public override string ToString() => string.Format("{0}: {1} ", (object) "DeploymentRequestsAllowed", (object) this.DeploymentRequestsAllowed) + string.Format("{0}: {1} ", (object) "AgentSpecSupported", (object) this.AgentSpecSupported) + string.Format("{0}: {1} ", (object) "HadError", (object) this.HadError) + string.Format("{0}: {1} ", (object) "MachinesAvailable", (object) this.MachinesAvailable) + string.Format("{0}: {1} ", (object) "WorkersAvailable", (object) this.WorkersAvailable) + string.Format("{0}: {1} ", (object) "AllocatedWorkerCount", (object) this.AllocatedWorkerCount) + string.Format("{0}: {1} ", (object) "MaxWorkerCount", (object) this.MaxWorkerCount) + string.Format("{0}: {1} ", (object) "CountHeldInReserve", (object) this.CountHeldInReserve);
  }
}
