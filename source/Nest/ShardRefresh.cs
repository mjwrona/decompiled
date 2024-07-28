// Decompiled with JetBrains decompiler
// Type: Nest.ShardRefresh
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ShardRefresh
  {
    [DataMember(Name = "listeners")]
    public long Listeners { get; internal set; }

    [DataMember(Name = "total")]
    public long Total { get; internal set; }

    [DataMember(Name = "total_time_in_millis")]
    public long TotalTimeInMilliseconds { get; internal set; }
  }
}
