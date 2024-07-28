// Decompiled with JetBrains decompiler
// Type: Nest.ProcessStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ProcessStats
  {
    [DataMember(Name = "cpu")]
    public ProcessStats.CPUStats CPU { get; internal set; }

    [DataMember(Name = "mem")]
    public ProcessStats.MemoryStats Memory { get; internal set; }

    [DataMember(Name = "open_file_descriptors")]
    public int OpenFileDescriptors { get; internal set; }

    [DataMember(Name = "timestamp")]
    public long Timestamp { get; internal set; }

    [DataContract]
    public class CPUStats
    {
      [DataMember(Name = "percent")]
      public int Percent { get; internal set; }

      [DataMember(Name = "sys")]
      public string System { get; internal set; }

      [DataMember(Name = "sys_in_millis")]
      public long SystemInMilliseconds { get; internal set; }

      [DataMember(Name = "total")]
      public string Total { get; internal set; }

      [DataMember(Name = "total_in_millis")]
      public long TotalInMilliseconds { get; internal set; }

      [DataMember(Name = "user")]
      public string User { get; internal set; }

      [DataMember(Name = "user_in_millis")]
      public long UserInMilliseconds { get; internal set; }
    }

    [DataContract]
    public class MemoryStats
    {
      [DataMember(Name = "resident")]
      public string Resident { get; internal set; }

      [DataMember(Name = "resident_in_bytes")]
      public long ResidentInBytes { get; internal set; }

      [DataMember(Name = "share")]
      public string Share { get; internal set; }

      [DataMember(Name = "share_in_bytes")]
      public long ShareInBytes { get; internal set; }

      [DataMember(Name = "total_virtual")]
      public string TotalVirtual { get; internal set; }

      [DataMember(Name = "total_virtual_in_bytes")]
      public long TotalVirtualInBytes { get; internal set; }
    }
  }
}
