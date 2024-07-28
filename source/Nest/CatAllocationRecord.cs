// Decompiled with JetBrains decompiler
// Type: Nest.CatAllocationRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatAllocationRecord : ICatRecord
  {
    [DataMember(Name = "disk.avail")]
    public string DiskAvailable { get; set; }

    [DataMember(Name = "disk.indices")]
    public string DiskIndices { get; set; }

    [DataMember(Name = "disk.percent")]
    public string DiskPercent { get; set; }

    [Obsolete("Use DiskPercent, DiskTotal, DiskAvailable, DiskTotal and DiskIndices")]
    [IgnoreDataMember]
    public string DiskRatio { get; set; }

    [DataMember(Name = "disk.total")]
    public string DiskTotal { get; set; }

    [DataMember(Name = "disk.used")]
    public string DiskUsed { get; set; }

    [DataMember(Name = "host")]
    public string Host { get; set; }

    [DataMember(Name = "ip")]
    public string Ip { get; set; }

    [DataMember(Name = "node")]
    public string Node { get; set; }

    [DataMember(Name = "shards")]
    public string Shards { get; set; }
  }
}
