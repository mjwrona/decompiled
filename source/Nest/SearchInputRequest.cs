// Decompiled with JetBrains decompiler
// Type: Nest.SearchInputRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class SearchInputRequest : ISearchInputRequest
  {
    public ISearchRequest Body { get; set; }

    public IEnumerable<IndexName> Indices { get; set; }

    public IIndicesOptions IndicesOptions { get; set; }

    public Elasticsearch.Net.SearchType? SearchType { get; set; }

    public ISearchTemplateRequest Template { get; set; }
  }
}
