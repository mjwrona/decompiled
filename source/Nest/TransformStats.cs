// Decompiled with JetBrains decompiler
// Type: Nest.TransformStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class TransformStats
  {
    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "state")]
    public string State { get; internal set; }

    [DataMember(Name = "reason")]
    public string Reason { get; internal set; }

    [DataMember(Name = "node")]
    public NodeAttributes Node { get; internal set; }

    [DataMember(Name = "stats")]
    public TransformIndexerStats Stats { get; internal set; }

    [DataMember(Name = "checkpointing")]
    public TransformCheckpointingInfo Checkpointing { get; internal set; }
  }
}
