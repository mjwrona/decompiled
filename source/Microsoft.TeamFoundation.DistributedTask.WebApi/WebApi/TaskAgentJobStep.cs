// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentJobStep
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentJobStep
  {
    [DataMember(EmitDefaultValue = false)]
    public TaskAgentJobStep.TaskAgentJobStepType Type { get; set; }

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
    public TaskAgentJobTask Task { get; set; }

    [DataMember]
    public IDictionary<string, string> Env { get; set; }

    [DataMember]
    public IDictionary<string, string> Inputs { get; set; }

    [DataContract]
    public enum TaskAgentJobStepType
    {
      [DataMember] Task = 1,
      [DataMember] Action = 2,
    }
  }
}
