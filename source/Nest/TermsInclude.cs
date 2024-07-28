// Decompiled with JetBrains decompiler
// Type: Nest.TermsInclude
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (TermsIncludeFormatter))]
  public class TermsInclude
  {
    public TermsInclude(string pattern) => this.Pattern = pattern;

    public TermsInclude(IEnumerable<string> values) => this.Values = values;

    public TermsInclude(long partition, long numberOfPartitions)
    {
      this.Partition = new long?(partition);
      this.NumberOfPartitions = new long?(numberOfPartitions);
    }

    [DataMember(Name = "num_partitions")]
    public long? NumberOfPartitions { get; set; }

    [DataMember(Name = "partition")]
    public long? Partition { get; set; }

    [IgnoreDataMember]
    public string Pattern { get; set; }

    [IgnoreDataMember]
    public IEnumerable<string> Values { get; set; }
  }
}
