// Decompiled with JetBrains decompiler
// Type: Nest.MatchPhrasePrefixQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MatchPhrasePrefixQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<MatchPhrasePrefixQueryDescriptor<T>, IMatchPhrasePrefixQuery, T>,
    IMatchPhrasePrefixQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => MatchPhrasePrefixQuery.IsConditionless((IMatchPhrasePrefixQuery) this);

    string IMatchPhrasePrefixQuery.Analyzer { get; set; }

    int? IMatchPhrasePrefixQuery.MaxExpansions { get; set; }

    string IMatchPhrasePrefixQuery.Query { get; set; }

    int? IMatchPhrasePrefixQuery.Slop { get; set; }

    Nest.ZeroTermsQuery? IMatchPhrasePrefixQuery.ZeroTermsQuery { get; set; }

    public MatchPhrasePrefixQueryDescriptor<T> Query(string query) => this.Assign<string>(query, (Action<IMatchPhrasePrefixQuery, string>) ((a, v) => a.Query = v));

    public MatchPhrasePrefixQueryDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IMatchPhrasePrefixQuery, string>) ((a, v) => a.Analyzer = v));

    public MatchPhrasePrefixQueryDescriptor<T> MaxExpansions(int? maxExpansions) => this.Assign<int?>(maxExpansions, (Action<IMatchPhrasePrefixQuery, int?>) ((a, v) => a.MaxExpansions = v));

    public MatchPhrasePrefixQueryDescriptor<T> Slop(int? slop) => this.Assign<int?>(slop, (Action<IMatchPhrasePrefixQuery, int?>) ((a, v) => a.Slop = v));

    public MatchPhrasePrefixQueryDescriptor<T> ZeroTermsQuery(Nest.ZeroTermsQuery? zeroTermsQuery) => this.Assign<Nest.ZeroTermsQuery?>(zeroTermsQuery, (Action<IMatchPhrasePrefixQuery, Nest.ZeroTermsQuery?>) ((a, v) => a.ZeroTermsQuery = v));
  }
}
