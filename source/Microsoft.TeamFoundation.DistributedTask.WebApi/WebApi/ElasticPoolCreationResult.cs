// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ElasticPoolCreationResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ElasticPoolCreationResult
  {
    public ElasticPoolCreationResult()
    {
    }

    public ElasticPoolCreationResult(ElasticPool elasticPool, TaskAgentPool agentPool)
    {
      this.elasticPool = elasticPool;
      this.agentPool = agentPool;
    }

    public ElasticPoolCreationResult(
      ElasticPool elasticPool,
      TaskAgentPool agentPool,
      TaskAgentQueue agentQueue)
    {
      this.elasticPool = elasticPool;
      this.agentPool = agentPool;
      this.agentQueue = agentQueue;
    }

    private ElasticPoolCreationResult(ElasticPoolCreationResult resultToBeCloned)
    {
      this.elasticPool = resultToBeCloned.elasticPool?.Clone();
      this.agentPool = resultToBeCloned.agentPool?.Clone();
      this.agentQueue = resultToBeCloned.agentQueue?.Clone();
    }

    [DataMember]
    public ElasticPool elasticPool { get; set; }

    [DataMember]
    public TaskAgentPool agentPool { get; set; }

    [DataMember]
    public TaskAgentQueue agentQueue { get; set; }

    public ElasticPoolCreationResult Clone() => new ElasticPoolCreationResult(this);
  }
}
