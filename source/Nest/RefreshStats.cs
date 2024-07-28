// Decompiled with JetBrains decompiler
// Type: Nest.RefreshStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class RefreshStats
  {
    [DataMember(Name = "total")]
    public long Total { get; set; }

    [DataMember(Name = "total_time")]
    public string TotalTime { get; set; }

    [DataMember(Name = "total_time_in_millis")]
    public long TotalTimeInMilliseconds { get; set; }

    [DataMember(Name = "external_total")]
    public long ExternalTotal { get; set; }

    [DataMember(Name = "external_total_time_in_millis")]
    public long ExternalTotalTimeInMilliseconds { get; set; }
  }
}
