// Decompiled with JetBrains decompiler
// Type: Nest.RegexpQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class RegexpQuery : FieldNameQueryBase, IRegexpQuery, IFieldNameQuery, IQuery
  {
    public string Flags { get; set; }

    public int? MaximumDeterminizedStates { get; set; }

    public string Value { get; set; }

    public MultiTermQueryRewrite Rewrite { get; set; }

    protected override bool Conditionless => RegexpQuery.IsConditionless((IRegexpQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Regexp = (IRegexpQuery) this;

    internal static bool IsConditionless(IRegexpQuery q) => q.Field.IsConditionless() || q.Value.IsNullOrEmpty();
  }
}
