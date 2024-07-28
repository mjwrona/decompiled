// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.AgentRefreshMessage
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class AgentRefreshMessage
  {
    public static readonly string MessageType = "AgentRefresh";

    [JsonConstructor]
    internal AgentRefreshMessage()
    {
    }

    public AgentRefreshMessage(int agentId, string targetVersion, TimeSpan? timeout = null)
    {
      this.AgentId = agentId;
      this.Timeout = timeout ?? TimeSpan.FromMinutes(60.0);
      this.TargetVersion = targetVersion;
    }

    [DataMember]
    public int AgentId { get; private set; }

    [DataMember]
    public TimeSpan Timeout { get; private set; }

    [DataMember]
    public string TargetVersion { get; private set; }
  }
}
