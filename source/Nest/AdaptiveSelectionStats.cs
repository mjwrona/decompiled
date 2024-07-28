// Decompiled with JetBrains decompiler
// Type: Nest.AdaptiveSelectionStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class AdaptiveSelectionStats
  {
    [DataMember(Name = "avg_queue_size")]
    public long AverageQueueSize { get; internal set; }

    [DataMember(Name = "avg_response_time")]
    public string AverageResponseTime { get; internal set; }

    [DataMember(Name = "avg_response_time_ns")]
    public long AverageResponseTimeInNanoseconds { get; internal set; }

    [DataMember(Name = "avg_service_time")]
    public string AverageServiceTime { get; internal set; }

    [DataMember(Name = "avg_service_time_ns")]
    public long AverageServiceTimeInNanoseconds { get; internal set; }

    [DataMember(Name = "outgoing_searches")]
    public long OutgoingSearches { get; internal set; }

    [DataMember(Name = "rank")]
    public string Rank { get; internal set; }
  }
}
