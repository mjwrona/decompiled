// Decompiled with JetBrains decompiler
// Type: Nest.GetStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class GetStats
  {
    [DataMember(Name = "current")]
    public long Current { get; set; }

    [DataMember(Name = "exists_time")]
    public string ExistsTime { get; set; }

    [DataMember(Name = "exists_time_in_millis")]
    public long ExistsTimeInMilliseconds { get; set; }

    [DataMember(Name = "exists_total")]
    public long ExistsTotal { get; set; }

    [DataMember(Name = "missing_time")]
    public string MissingTime { get; set; }

    [DataMember(Name = "missing_time_in_millis")]
    public long MissingTimeInMilliseconds { get; set; }

    [DataMember(Name = "missing_total")]
    public long MissingTotal { get; set; }

    [DataMember(Name = "time")]
    public string Time { get; set; }

    [DataMember(Name = "time_in_millis")]
    public long TimeInMilliseconds { get; set; }

    [DataMember(Name = "total")]
    public long Total { get; set; }
  }
}
