// Decompiled with JetBrains decompiler
// Type: Nest.SearchProfile
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class SearchProfile
  {
    [DataMember(Name = "collector")]
    public IReadOnlyCollection<Nest.Collector> Collector { get; internal set; } = EmptyReadOnly<Nest.Collector>.Collection;

    [DataMember(Name = "query")]
    public IReadOnlyCollection<QueryProfile> Query { get; internal set; } = EmptyReadOnly<QueryProfile>.Collection;

    [DataMember(Name = "rewrite_time")]
    public long RewriteTime { get; internal set; }
  }
}
