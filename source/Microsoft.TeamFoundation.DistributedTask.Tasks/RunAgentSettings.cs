// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.RunAgentSettings
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  [DataContract]
  public sealed class RunAgentSettings
  {
    [DataMember]
    public int MaxProvisionAttempts { get; set; }

    [DataMember]
    public TimeSpan DefaultAgentConnectTimeout { get; set; }

    [DataMember]
    public TimeSpan AgentConnectionRefreshRate { get; set; }

    [DataMember]
    public TimeSpan NoActiveRequestResfreshRate { get; set; }

    [DataMember]
    public TimeSpan ActiveRequestResfreshRate { get; set; }

    [DataMember]
    public int MaxDeprovisionAttempts { get; set; }

    [DataMember]
    public TimeSpan MaxRetryAfter { get; set; }
  }
}
