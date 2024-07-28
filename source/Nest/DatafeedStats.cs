// Decompiled with JetBrains decompiler
// Type: Nest.DatafeedStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class DatafeedStats
  {
    [DataMember(Name = "assignment_explanation")]
    public string AssignmentExplanation { get; internal set; }

    [DataMember(Name = "datafeed_id")]
    public string DatafeedId { get; internal set; }

    [DataMember(Name = "node")]
    public DiscoveryNode Node { get; internal set; }

    [DataMember(Name = "state")]
    public DatafeedState State { get; internal set; }

    [DataMember(Name = "timing_stats")]
    public DatafeedTimingStats TimingStats { get; internal set; }

    [DataMember(Name = "running_state")]
    public RunningState RunningState { get; internal set; }
  }
}
