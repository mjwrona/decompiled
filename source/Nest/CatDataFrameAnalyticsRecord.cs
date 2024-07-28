// Decompiled with JetBrains decompiler
// Type: Nest.CatDataFrameAnalyticsRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatDataFrameAnalyticsRecord : ICatRecord
  {
    [DataMember(Name = "assignment_explanation")]
    public string AssignmentExplanation { get; internal set; }

    [DataMember(Name = "create_time")]
    public string CreateTime { get; internal set; }

    [DataMember(Name = "description")]
    public string Description { get; internal set; }

    [DataMember(Name = "dest_index")]
    public string DestinationIndex { get; internal set; }

    [DataMember(Name = "failure_reason")]
    public string FailureReason { get; internal set; }

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "model_memory_limit")]
    public string ModelMemoryLimit { get; internal set; }

    [DataMember(Name = "node.address")]
    public string NodeAddress { get; internal set; }

    [DataMember(Name = "node.ephemeral_id")]
    public string NodeEphemeralId { get; internal set; }

    [DataMember(Name = "node.id")]
    public string NodeId { get; internal set; }

    [DataMember(Name = "node.name")]
    public string NodeName { get; internal set; }

    [DataMember(Name = "progress")]
    public string Progress { get; internal set; }

    [DataMember(Name = "source_index")]
    public string SourceIndex { get; internal set; }

    [DataMember(Name = "state")]
    public string State { get; internal set; }

    [DataMember(Name = "type")]
    public string Type { get; internal set; }

    [DataMember(Name = "version")]
    public string Version { get; internal set; }
  }
}
