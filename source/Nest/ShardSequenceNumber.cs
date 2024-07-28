// Decompiled with JetBrains decompiler
// Type: Nest.ShardSequenceNumber
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardSequenceNumber
  {
    [DataMember(Name = "global_checkpoint")]
    public long GlobalCheckpoint { get; internal set; }

    [DataMember(Name = "local_checkpoint")]
    public long LocalCheckpoint { get; internal set; }

    [DataMember(Name = "max_seq_no")]
    public long MaximumSequenceNumber { get; internal set; }
  }
}
