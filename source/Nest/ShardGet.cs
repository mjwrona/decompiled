// Decompiled with JetBrains decompiler
// Type: Nest.ShardGet
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardGet
  {
    [DataMember(Name = "current")]
    public long Current { get; internal set; }

    [DataMember(Name = "exists_time_in_millis")]
    public long ExistsTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "exists_total")]
    public long ExistsTotal { get; internal set; }

    [DataMember(Name = "missing_time_in_millis")]
    public long MissingTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "missing_total")]
    public long MissingTotal { get; internal set; }

    [DataMember(Name = "time_in_millis")]
    public long TimeInMilliseconds { get; internal set; }

    [DataMember(Name = "total")]
    public long Total { get; internal set; }
  }
}
