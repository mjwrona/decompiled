// Decompiled with JetBrains decompiler
// Type: Nest.RollupJobConfiguration
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class RollupJobConfiguration
  {
    [DataMember(Name = "cron")]
    public string Cron { get; internal set; }

    [DataMember(Name = "groups")]
    public IRollupGroupings Groups { get; internal set; }

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "index_pattern")]
    public string IndexPattern { get; internal set; }

    [DataMember(Name = "metrics")]
    public IEnumerable<IRollupFieldMetric> Metrics { get; internal set; }

    [DataMember(Name = "page_size")]
    public long? PageSize { get; internal set; }

    [DataMember(Name = "rollup_index")]
    public IndexName RollupIndex { get; internal set; }

    [DataMember(Name = "timeout")]
    public Time Timeout { get; internal set; }
  }
}
