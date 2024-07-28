// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeDetail
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class AnalyzeDetail
  {
    [DataMember(Name = "charfilters")]
    public IReadOnlyCollection<CharFilterDetail> CharFilters { get; internal set; } = EmptyReadOnly<CharFilterDetail>.Collection;

    [DataMember(Name = "custom_analyzer")]
    public bool CustomAnalyzer { get; internal set; }

    [DataMember(Name = "tokenfilters")]
    public IReadOnlyCollection<TokenDetail> Filters { get; internal set; } = EmptyReadOnly<TokenDetail>.Collection;

    [DataMember(Name = "tokenizer")]
    public TokenDetail Tokenizer { get; internal set; }
  }
}
