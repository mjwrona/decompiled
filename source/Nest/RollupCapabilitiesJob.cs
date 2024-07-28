// Decompiled with JetBrains decompiler
// Type: Nest.RollupCapabilitiesJob
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class RollupCapabilitiesJob
  {
    [DataMember(Name = "fields")]
    public RollupFieldsCapabilitiesDictionary Fields { get; internal set; }

    [DataMember(Name = "index_pattern")]
    public string IndexPattern { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "rollup_index")]
    public string RollupIndex { get; internal set; }
  }
}
