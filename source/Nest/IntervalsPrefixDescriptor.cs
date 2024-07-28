// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsPrefixDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class IntervalsPrefixDescriptor : 
    DescriptorBase<IntervalsPrefixDescriptor, IIntervalsPrefix>,
    IIntervalsPrefix,
    IIntervalsNoFilter
  {
    string IIntervalsPrefix.Analyzer { get; set; }

    string IIntervalsPrefix.Prefix { get; set; }

    Field IIntervalsPrefix.UseField { get; set; }

    public IntervalsPrefixDescriptor Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IIntervalsPrefix, string>) ((a, v) => a.Analyzer = v));

    public IntervalsPrefixDescriptor Prefix(string prefix) => this.Assign<string>(prefix, (Action<IIntervalsPrefix, string>) ((a, v) => a.Prefix = v));

    public IntervalsPrefixDescriptor UseField<T>(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<IIntervalsPrefix, Expression<Func<T, object>>>) ((a, v) => a.UseField = (Field) (Expression) v));

    public IntervalsPrefixDescriptor UseField(Field useField) => this.Assign<Field>(useField, (Action<IIntervalsPrefix, Field>) ((a, v) => a.UseField = v));
  }
}
