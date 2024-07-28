// Decompiled with JetBrains decompiler
// Type: Nest.Filter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class Filter
  {
    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "filter_id")]
    public string FilterId { get; set; }

    [DataMember(Name = "items")]
    public IReadOnlyCollection<string> Items { get; set; } = EmptyReadOnly<string>.Collection;
  }
}
