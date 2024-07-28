// Decompiled with JetBrains decompiler
// Type: Nest.XPackFeatures
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class XPackFeatures
  {
    [DataMember(Name = "analytics")]
    public XPackFeature Analytics { get; internal set; }

    [DataMember(Name = "ccr")]
    public XPackFeature Ccr { get; internal set; }

    [DataMember(Name = "enrich")]
    public XPackFeature Enrich { get; internal set; }

    [Obsolete("Changed to Transform in 7.5.0")]
    [DataMember(Name = "data_frame")]
    public XPackFeature DataFrame { get; internal set; }

    [DataMember(Name = "flattened")]
    public XPackFeature Flattened { get; internal set; }

    [DataMember(Name = "frozen_indices")]
    public XPackFeature FrozenIndices { get; internal set; }

    [DataMember(Name = "data_science")]
    public XPackFeature DataScience { get; internal set; }

    [DataMember(Name = "graph")]
    public XPackFeature Graph { get; internal set; }

    [DataMember(Name = "ilm")]
    public XPackFeature Ilm { get; internal set; }

    [DataMember(Name = "logstash")]
    public XPackFeature Logstash { get; internal set; }

    [DataMember(Name = "ml")]
    public XPackFeature MachineLearning { get; internal set; }

    [DataMember(Name = "monitoring")]
    public XPackFeature Monitoring { get; internal set; }

    [DataMember(Name = "rollup")]
    public XPackFeature Rollup { get; internal set; }

    [DataMember(Name = "security")]
    public XPackFeature Security { get; internal set; }

    [DataMember(Name = "slm")]
    public XPackFeature SnapshotLifecycleManagement { get; internal set; }

    [DataMember(Name = "spatial")]
    public XPackFeature Spatial { get; internal set; }

    [DataMember(Name = "sql")]
    public XPackFeature Sql { get; internal set; }

    [DataMember(Name = "transform")]
    public XPackFeature Transform { get; internal set; }

    [DataMember(Name = "vectors")]
    public XPackFeature Vectors { get; internal set; }

    [DataMember(Name = "voting_only")]
    public XPackFeature VotingOnly { get; internal set; }

    [DataMember(Name = "watcher")]
    public XPackFeature Watcher { get; internal set; }
  }
}
