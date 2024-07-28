// Decompiled with JetBrains decompiler
// Type: Nest.MatchPhrasePrefixQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class MatchPhrasePrefixQuery : 
    FieldNameQueryBase,
    IMatchPhrasePrefixQuery,
    IFieldNameQuery,
    IQuery
  {
    public string Analyzer { get; set; }

    public int? MaxExpansions { get; set; }

    public string Query { get; set; }

    public int? Slop { get; set; }

    public Nest.ZeroTermsQuery? ZeroTermsQuery { get; set; }

    protected override bool Conditionless => MatchPhrasePrefixQuery.IsConditionless((IMatchPhrasePrefixQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.MatchPhrasePrefix = (IMatchPhrasePrefixQuery) this;

    internal static bool IsConditionless(IMatchPhrasePrefixQuery q) => q.Field.IsConditionless() || q.Query.IsNullOrEmpty();
  }
}
