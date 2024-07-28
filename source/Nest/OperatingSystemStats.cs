// Decompiled with JetBrains decompiler
// Type: Nest.OperatingSystemStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class OperatingSystemStats
  {
    [DataMember(Name = "cpu")]
    public OperatingSystemStats.CPUStats Cpu { get; internal set; }

    [DataMember(Name = "mem")]
    public OperatingSystemStats.ExtendedMemoryStats Memory { get; internal set; }

    [DataMember(Name = "swap")]
    public OperatingSystemStats.MemoryStats Swap { get; internal set; }

    [DataMember(Name = "timestamp")]
    public long Timestamp { get; internal set; }

    [DataContract]
    public class CPUStats
    {
      [DataMember(Name = "load_average")]
      public OperatingSystemStats.CPUStats.LoadAverageStats LoadAverage { get; internal set; }

      [DataMember(Name = "percent")]
      public float Percent { get; internal set; }

      [DataContract]
      public class LoadAverageStats
      {
        [DataMember(Name = "15m")]
        public float? FifteenMinute { get; internal set; }

        [DataMember(Name = "5m")]
        public float? FiveMinute { get; internal set; }

        [DataMember(Name = "1m")]
        public float? OneMinute { get; internal set; }
      }
    }

    [DataContract]
    public class MemoryStats
    {
      [DataMember(Name = "free")]
      public string Free { get; internal set; }

      [DataMember(Name = "free_in_bytes")]
      public long FreeInBytes { get; internal set; }

      [DataMember(Name = "total")]
      public string Total { get; internal set; }

      [DataMember(Name = "total_in_bytes")]
      public long TotalInBytes { get; internal set; }

      [DataMember(Name = "used")]
      public string Used { get; internal set; }

      [DataMember(Name = "used_in_bytes")]
      public long UsedInBytes { get; internal set; }
    }

    [DataContract]
    public class ExtendedMemoryStats : OperatingSystemStats.MemoryStats
    {
      [DataMember(Name = "free_percent")]
      public int FreePercent { get; internal set; }

      [DataMember(Name = "used_percent")]
      public int UsedPercent { get; internal set; }
    }
  }
}
