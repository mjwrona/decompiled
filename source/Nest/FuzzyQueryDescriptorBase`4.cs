// Decompiled with JetBrains decompiler
// Type: Nest.FuzzyQueryDescriptorBase`4
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class FuzzyQueryDescriptorBase<TDescriptor, T, TValue, TFuzziness> : 
    FieldNameQueryDescriptorBase<TDescriptor, IFuzzyQuery<TValue, TFuzziness>, T>,
    IFuzzyQuery<TValue, TFuzziness>,
    IFuzzyQuery,
    IFieldNameQuery,
    IQuery
    where TDescriptor : FieldNameQueryDescriptorBase<TDescriptor, IFuzzyQuery<TValue, TFuzziness>, T>, IFuzzyQuery<TValue, TFuzziness>
    where T : class
  {
    protected override bool Conditionless => FuzzyQueryBase.IsConditionless<TValue, TFuzziness>((IFuzzyQuery<TValue, TFuzziness>) this);

    TFuzziness IFuzzyQuery<TValue, TFuzziness>.Fuzziness { get; set; }

    int? IFuzzyQuery.MaxExpansions { get; set; }

    int? IFuzzyQuery.PrefixLength { get; set; }

    MultiTermQueryRewrite IFuzzyQuery.Rewrite { get; set; }

    bool? IFuzzyQuery.Transpositions { get; set; }

    TValue IFuzzyQuery<TValue, TFuzziness>.Value { get; set; }

    public TDescriptor MaxExpansions(int? maxExpansions) => this.Assign<int?>(maxExpansions, (Action<IFuzzyQuery<TValue, TFuzziness>, int?>) ((a, v) => a.MaxExpansions = v));

    public TDescriptor PrefixLength(int? prefixLength) => this.Assign<int?>(prefixLength, (Action<IFuzzyQuery<TValue, TFuzziness>, int?>) ((a, v) => a.PrefixLength = v));

    public TDescriptor Transpositions(bool? enable = true) => this.Assign<bool?>(enable, (Action<IFuzzyQuery<TValue, TFuzziness>, bool?>) ((a, v) => a.Transpositions = v));

    public TDescriptor Rewrite(MultiTermQueryRewrite rewrite) => this.Assign<MultiTermQueryRewrite>(rewrite, (Action<IFuzzyQuery<TValue, TFuzziness>, MultiTermQueryRewrite>) ((a, v) => a.Rewrite = v));
  }
}
