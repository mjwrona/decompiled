// Decompiled with JetBrains decompiler
// Type: Nest.FuzzyQueryBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class FuzzyQueryBase<TValue, TFuzziness> : 
    FieldNameQueryBase,
    IFuzzyQuery<TValue, TFuzziness>,
    IFuzzyQuery,
    IFieldNameQuery,
    IQuery
  {
    public TFuzziness Fuzziness { get; set; }

    public int? MaxExpansions { get; set; }

    public int? PrefixLength { get; set; }

    public MultiTermQueryRewrite Rewrite { get; set; }

    public bool? Transpositions { get; set; }

    public TValue Value { get; set; }

    protected override bool Conditionless => FuzzyQueryBase.IsConditionless<TValue, TFuzziness>((IFuzzyQuery<TValue, TFuzziness>) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Fuzzy = (IFuzzyQuery) this;
  }
}
