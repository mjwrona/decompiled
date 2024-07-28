// Decompiled with JetBrains decompiler
// Type: Nest.BreakerStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class BreakerStats
  {
    [DataMember(Name = "estimated_size")]
    public string EstimatedSize { get; internal set; }

    [DataMember(Name = "estimated_size_in_bytes")]
    public long EstimatedSizeInBytes { get; internal set; }

    [DataMember(Name = "limit_size")]
    public string LimitSize { get; internal set; }

    [DataMember(Name = "limit_size_in_bytes")]
    public long LimitSizeInBytes { get; internal set; }

    [DataMember(Name = "overhead")]
    public float Overhead { get; internal set; }

    [DataMember(Name = "tripped")]
    public float Tripped { get; internal set; }
  }
}
