// Decompiled with JetBrains decompiler
// Type: Nest.ClusterIngestStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterIngestStats
  {
    [DataMember(Name = "number_of_pipelines")]
    public int NumberOfPipelines { get; internal set; }

    [DataMember(Name = "processor_stats")]
    public IReadOnlyDictionary<string, ClusterProcessorStats> ProcessorStats { get; internal set; }
  }
}
