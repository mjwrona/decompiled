// Decompiled with JetBrains decompiler
// Type: Nest.MatchPhraseQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class MatchPhraseQuery : FieldNameQueryBase, IMatchPhraseQuery, IFieldNameQuery, IQuery
  {
    public string Analyzer { get; set; }

    public string Query { get; set; }

    public int? Slop { get; set; }

    protected override bool Conditionless => MatchPhraseQuery.IsConditionless((IMatchPhraseQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.MatchPhrase = (IMatchPhraseQuery) this;

    internal static bool IsConditionless(IMatchPhraseQuery q) => q.Field.IsConditionless() || q.Query.IsNullOrEmpty();
  }
}
