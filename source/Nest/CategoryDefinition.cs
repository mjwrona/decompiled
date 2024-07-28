// Decompiled with JetBrains decompiler
// Type: Nest.CategoryDefinition
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CategoryDefinition
  {
    [DataMember(Name = "category_id")]
    public long CategoryId { get; internal set; }

    [DataMember(Name = "examples")]
    public IReadOnlyCollection<string> Examples { get; internal set; } = EmptyReadOnly<string>.Collection;

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "max_matching_length")]
    public long MaxMatchingLength { get; internal set; }

    [DataMember(Name = "regex")]
    public string Regex { get; internal set; }

    [DataMember(Name = "terms")]
    public string Terms { get; internal set; }
  }
}
