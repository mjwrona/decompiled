// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class SnapshotStats
  {
    [DataMember(Name = "incremental")]
    public FileCountSnapshotStats Incremental { get; internal set; }

    [DataMember(Name = "total")]
    public FileCountSnapshotStats Total { get; internal set; }

    [DataMember(Name = "start_time_in_millis")]
    public long StartTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "time_in_millis")]
    public long TimeInMilliseconds { get; internal set; }
  }
}
