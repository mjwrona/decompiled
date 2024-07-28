// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsWildcardDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class IntervalsWildcardDescriptor : 
    DescriptorBase<IntervalsWildcardDescriptor, IIntervalsWildcard>,
    IIntervalsWildcard,
    IIntervalsNoFilter
  {
    string IIntervalsWildcard.Analyzer { get; set; }

    string IIntervalsWildcard.Pattern { get; set; }

    Field IIntervalsWildcard.UseField { get; set; }

    public IntervalsWildcardDescriptor Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IIntervalsWildcard, string>) ((a, v) => a.Analyzer = v));

    public IntervalsWildcardDescriptor Pattern(string pattern) => this.Assign<string>(pattern, (Action<IIntervalsWildcard, string>) ((a, v) => a.Pattern = v));

    public IntervalsWildcardDescriptor UseField<T>(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<IIntervalsWildcard, Expression<Func<T, object>>>) ((a, v) => a.UseField = (Field) (Expression) v));

    public IntervalsWildcardDescriptor UseField(Field useField) => this.Assign<Field>(useField, (Action<IIntervalsWildcard, Field>) ((a, v) => a.UseField = v));
  }
}
