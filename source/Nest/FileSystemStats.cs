// Decompiled with JetBrains decompiler
// Type: Nest.FileSystemStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class FileSystemStats
  {
    [DataMember(Name = "data")]
    public IEnumerable<FileSystemStats.DataPathStats> Data { get; internal set; }

    [DataMember(Name = "timestamp")]
    public long Timestamp { get; internal set; }

    [DataMember(Name = "total")]
    public FileSystemStats.TotalFileSystemStats Total { get; internal set; }

    [DataMember(Name = "io_stats")]
    public FileSystemStats.IoStatsContainer IoStats { get; internal set; }

    public class IoStatsContainer
    {
      [DataMember(Name = "devices")]
      public IEnumerable<FileSystemStats.DeviceIoStats> Devices { get; internal set; }

      [DataMember(Name = "total")]
      public FileSystemStats.IoStatistics Total { get; internal set; }
    }

    public class IoStatistics
    {
      [DataMember(Name = "operations")]
      public long Operations { get; internal set; }

      [DataMember(Name = "read_operations")]
      public long ReadOperations { get; internal set; }

      [DataMember(Name = "write_operations")]
      public long WriteOperations { get; internal set; }

      [DataMember(Name = "read_kilobytes")]
      public long ReadKilobytes { get; internal set; }

      [DataMember(Name = "write_kilobytes")]
      public long WriteKilobytes { get; internal set; }

      [DataMember(Name = "io_time_in_millis")]
      public long IoTimeInMilliseconds { get; internal set; }
    }

    public class DeviceIoStats : FileSystemStats.IoStatistics
    {
      [DataMember(Name = "device_name")]
      public string DeviceName { get; internal set; }
    }

    public class TotalFileSystemStats
    {
      [DataMember(Name = "available")]
      public string Available { get; internal set; }

      [DataMember(Name = "available_in_bytes")]
      public long AvailableInBytes { get; internal set; }

      [DataMember(Name = "free")]
      public string Free { get; internal set; }

      [DataMember(Name = "free_in_bytes")]
      public long FreeInBytes { get; internal set; }

      [DataMember(Name = "total")]
      public string Total { get; internal set; }

      [DataMember(Name = "total_in_bytes")]
      public long TotalInBytes { get; internal set; }
    }

    [DataContract]
    public class DataPathStats
    {
      [DataMember(Name = "available")]
      public string Available { get; internal set; }

      [DataMember(Name = "available_in_bytes")]
      public long AvailableInBytes { get; internal set; }

      [DataMember(Name = "disk_queue")]
      public string DiskQueue { get; internal set; }

      [DataMember(Name = "disk_reads")]
      public long DiskReads { get; internal set; }

      [DataMember(Name = "disk_read_size")]
      public string DiskReadSize { get; internal set; }

      [DataMember(Name = "disk_read_size_in_bytes")]
      public long DiskReadSizeInBytes { get; internal set; }

      [DataMember(Name = "disk_writes")]
      public long DiskWrites { get; internal set; }

      [DataMember(Name = "disk_write_size")]
      public string DiskWriteSize { get; internal set; }

      [DataMember(Name = "disk_write_size_in_bytes")]
      public long DiskWriteSizeInBytes { get; internal set; }

      [DataMember(Name = "free")]
      public string Free { get; internal set; }

      [DataMember(Name = "free_in_bytes")]
      public long FreeInBytes { get; internal set; }

      [DataMember(Name = "mount")]
      public string Mount { get; internal set; }

      [DataMember(Name = "path")]
      public string Path { get; internal set; }

      [DataMember(Name = "total")]
      public string Total { get; internal set; }

      [DataMember(Name = "total_in_bytes")]
      public long TotalInBytes { get; internal set; }

      [DataMember(Name = "type")]
      public string Type { get; internal set; }
    }
  }
}
