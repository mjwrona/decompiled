// Decompiled with JetBrains decompiler
// Type: Nest.ShardTransactionLog
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardTransactionLog
  {
    [DataMember(Name = "operations")]
    public long Operations { get; internal set; }

    [DataMember(Name = "size_in_bytes")]
    public long SizeInBytes { get; internal set; }

    [DataMember(Name = "uncommitted_operations")]
    public long UncommittedOperations { get; internal set; }

    [DataMember(Name = "uncommitted_size_in_bytes")]
    public long UncommittedSizeInBytes { get; internal set; }
  }
}
