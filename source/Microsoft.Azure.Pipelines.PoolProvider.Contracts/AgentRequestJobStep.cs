// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Contracts.AgentRequestJobStep
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2F8171A-7EDF-4EAC-B6BB-DAF285412F1E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Contracts.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.PoolProvider.Contracts
{
  [DataContract]
  public class AgentRequestJobStep
  {
    [DataMember(EmitDefaultValue = false)]
    public AgentRequestJobStep.TaskAgentJobStepType Type { get; set; }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public bool Enabled { get; set; }

    [DataMember]
    public string Condition { get; set; }

    [DataMember]
    public bool ContinueOnError { get; set; }

    [DataMember]
    public int TimeoutInMinutes { get; set; }

    [DataMember]
    public int RetryCountOnTaskFailure { get; set; }

    [DataMember]
    public AgentRequestJobTask Task { get; set; }

    [DataMember]
    public IDictionary<string, string> Env { get; set; }

    [DataMember]
    public IDictionary<string, string> Inputs { get; set; }

    [DataContract]
    public enum TaskAgentJobStepType
    {
      [DataMember] Task = 1,
      [DataMember] Action = 2,
      [DataMember] Script = 3,
    }
  }
}
