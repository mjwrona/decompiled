// Decompiled with JetBrains decompiler
// Type: Nest.XPackUsageResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class XPackUsageResponse : ResponseBase
  {
    [DataMember(Name = "sql")]
    public SqlUsage Sql { get; internal set; }

    [DataMember(Name = "rollup")]
    public XPackUsage Rollup { get; internal set; }

    [Obsolete("Changed to Transform in 7.5.0")]
    [DataMember(Name = "data_frame")]
    public XPackUsage DataFrame { get; internal set; }

    [DataMember(Name = "flattened")]
    public FlattenedUsage Flattened { get; internal set; }

    [DataMember(Name = "data_science")]
    public XPackUsage DataScience { get; internal set; }

    [DataMember(Name = "ilm")]
    public IlmUsage IndexLifecycleManagement { get; internal set; }

    [DataMember(Name = "ccr")]
    public CcrUsage Ccr { get; internal set; }

    [DataMember(Name = "watcher")]
    public AlertingUsage Alerting { get; internal set; }

    [DataMember(Name = "graph")]
    public XPackUsage Graph { get; internal set; }

    [DataMember(Name = "logstash")]
    public XPackUsage Logstash { get; internal set; }

    [DataMember(Name = "ml")]
    public MachineLearningUsage MachineLearning { get; internal set; }

    [DataMember(Name = "monitoring")]
    public MonitoringUsage Monitoring { get; internal set; }

    [DataMember(Name = "security")]
    public SecurityUsage Security { get; internal set; }

    [DataMember(Name = "transform")]
    public XPackUsage Transform { get; internal set; }

    [DataMember(Name = "vectors")]
    public VectorUsage Vectors { get; internal set; }

    [DataMember(Name = "voting_only")]
    public XPackUsage VotingOnly { get; internal set; }

    [DataMember(Name = "slm")]
    public SlmUsage SnapshotLifecycleManagement { get; internal set; }

    [DataMember(Name = "enrich")]
    public XPackUsage Enrich { get; set; }

    [DataMember(Name = "spatial")]
    public XPackUsage Spatial { get; internal set; }

    [DataMember(Name = "analytics")]
    public AnalyticsUsage Analytics { get; internal set; }
  }
}
