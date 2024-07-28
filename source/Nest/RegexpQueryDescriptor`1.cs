// Decompiled with JetBrains decompiler
// Type: Nest.RegexpQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RegexpQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<RegexpQueryDescriptor<T>, IRegexpQuery, T>,
    IRegexpQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => RegexpQuery.IsConditionless((IRegexpQuery) this);

    string IRegexpQuery.Flags { get; set; }

    int? IRegexpQuery.MaximumDeterminizedStates { get; set; }

    string IRegexpQuery.Value { get; set; }

    MultiTermQueryRewrite IRegexpQuery.Rewrite { get; set; }

    public RegexpQueryDescriptor<T> MaximumDeterminizedStates(int? maxDeterminizedStates) => this.Assign<int?>(maxDeterminizedStates, (Action<IRegexpQuery, int?>) ((a, v) => a.MaximumDeterminizedStates = v));

    public RegexpQueryDescriptor<T> Value(string regex) => this.Assign<string>(regex, (Action<IRegexpQuery, string>) ((a, v) => a.Value = v));

    public RegexpQueryDescriptor<T> Flags(string flags) => this.Assign<string>(flags, (Action<IRegexpQuery, string>) ((a, v) => a.Flags = v));

    public RegexpQueryDescriptor<T> Rewrite(MultiTermQueryRewrite rewrite) => this.Assign<MultiTermQueryRewrite>(rewrite, (Action<IRegexpQuery, MultiTermQueryRewrite>) ((a, v) => a.Rewrite = v));
  }
}
