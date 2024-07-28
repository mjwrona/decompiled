﻿// Decompiled with JetBrains decompiler
// Type: Nest.TermVectorTerm
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class TermVectorTerm
  {
    [DataMember(Name = "doc_freq")]
    public int DocumentFrequency { get; internal set; }

    [DataMember(Name = "term_freq")]
    public int TermFrequency { get; internal set; }

    [DataMember(Name = "score")]
    public double Score { get; internal set; }

    [DataMember(Name = "tokens")]
    public IReadOnlyCollection<Token> Tokens { get; internal set; } = EmptyReadOnly<Token>.Collection;

    [DataMember(Name = "ttf")]
    public int TotalTermFrequency { get; internal set; }
  }
}
