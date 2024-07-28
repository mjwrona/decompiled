// Decompiled with JetBrains decompiler
// Type: Nest.CatDatafeedsRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatDatafeedsRecord : ICatRecord
  {
    [DataMember(Name = "assignment_explanation")]
    public string AssignmentExplanation { get; internal set; }

    [DataMember(Name = "buckets.count")]
    public string BucketsCount { get; internal set; }

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "node.address")]
    public string NodeAddress { get; internal set; }

    [DataMember(Name = "node.ephemeral_id")]
    public string NodeEphemeralId { get; internal set; }

    [DataMember(Name = "node.id")]
    public string NodeId { get; internal set; }

    [DataMember(Name = "node.name")]
    public string NodeName { get; internal set; }

    [DataMember(Name = "search.bucket_avg")]
    public string SearchBucketAvg { get; internal set; }

    [DataMember(Name = "search.count")]
    public string SearchCount { get; internal set; }

    [DataMember(Name = "search.exp_avg_hour")]
    public string SearchExpAvgHour { get; internal set; }

    [DataMember(Name = "search.time")]
    public string SearchTime { get; internal set; }

    [DataMember(Name = "state")]
    public DatafeedState State { get; internal set; }
  }
}
