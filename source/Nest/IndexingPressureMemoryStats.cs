// Decompiled with JetBrains decompiler
// Type: Nest.IndexingPressureMemoryStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class IndexingPressureMemoryStats
  {
    [DataMember(Name = "current")]
    public IndexingLoad Current { get; internal set; }

    [DataMember(Name = "total")]
    public TotalIndexingLoad Total { get; internal set; }

    [DataMember(Name = "limit_in_bytes")]
    public long LimitInBytes { get; internal set; }

    [DataMember(Name = "limit")]
    public string Limit { get; internal set; }
  }
}
