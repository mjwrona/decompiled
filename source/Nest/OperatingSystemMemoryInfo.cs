// Decompiled with JetBrains decompiler
// Type: Nest.OperatingSystemMemoryInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class OperatingSystemMemoryInfo
  {
    [DataMember(Name = "free_in_bytes")]
    public long FreeBytes { get; internal set; }

    [DataMember(Name = "free_percent")]
    public int FreePercent { get; internal set; }

    [DataMember(Name = "total_in_bytes")]
    public long TotalBytes { get; internal set; }

    [DataMember(Name = "used_in_bytes")]
    public long UsedBytes { get; internal set; }

    [DataMember(Name = "used_percent")]
    public int UsedPercent { get; internal set; }
  }
}
