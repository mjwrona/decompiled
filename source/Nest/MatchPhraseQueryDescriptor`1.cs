// Decompiled with JetBrains decompiler
// Type: Nest.MatchPhraseQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MatchPhraseQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<MatchPhraseQueryDescriptor<T>, IMatchPhraseQuery, T>,
    IMatchPhraseQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => MatchPhraseQuery.IsConditionless((IMatchPhraseQuery) this);

    string IMatchPhraseQuery.Analyzer { get; set; }

    string IMatchPhraseQuery.Query { get; set; }

    int? IMatchPhraseQuery.Slop { get; set; }

    public MatchPhraseQueryDescriptor<T> Query(string query) => this.Assign<string>(query, (Action<IMatchPhraseQuery, string>) ((a, v) => a.Query = v));

    public MatchPhraseQueryDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IMatchPhraseQuery, string>) ((a, v) => a.Analyzer = v));

    public MatchPhraseQueryDescriptor<T> Slop(int? slop) => this.Assign<int?>(slop, (Action<IMatchPhraseQuery, int?>) ((a, v) => a.Slop = v));
  }
}
