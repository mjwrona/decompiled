// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DiagnosticLogMetadata
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class DiagnosticLogMetadata
  {
    public DiagnosticLogMetadata(
      string agentName,
      int agentId,
      int poolId,
      string phaseName,
      string fileName,
      string phaseResult)
    {
      this.AgentName = agentName;
      this.AgentId = agentId;
      this.PoolId = poolId;
      this.PhaseName = phaseName;
      this.FileName = fileName;
      this.PhaseResult = phaseResult;
    }

    [DataMember]
    public string AgentName { get; set; }

    [DataMember]
    public int AgentId { get; set; }

    [DataMember]
    public int PoolId { get; set; }

    [DataMember]
    public string PhaseName { get; set; }

    [DataMember]
    public string FileName { get; set; }

    [DataMember]
    public string PhaseResult { get; set; }
  }
}
