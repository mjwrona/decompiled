// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsFuzzyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class IntervalsFuzzyDescriptor : 
    DescriptorBase<IntervalsFuzzyDescriptor, IIntervalsFuzzy>,
    IIntervalsFuzzy,
    IIntervalsNoFilter
  {
    string IIntervalsFuzzy.Analyzer { get; set; }

    int? IIntervalsFuzzy.PrefixLength { get; set; }

    bool? IIntervalsFuzzy.Transpositions { get; set; }

    Nest.Fuzziness IIntervalsFuzzy.Fuzziness { get; set; }

    string IIntervalsFuzzy.Term { get; set; }

    Field IIntervalsFuzzy.UseField { get; set; }

    public IntervalsFuzzyDescriptor Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IIntervalsFuzzy, string>) ((a, v) => a.Analyzer = v));

    public IntervalsFuzzyDescriptor PrefixLength(int? prefixLength) => this.Assign<int?>(prefixLength, (Action<IIntervalsFuzzy, int?>) ((a, v) => a.PrefixLength = v));

    public IntervalsFuzzyDescriptor Transpositions(bool? transpositions = true) => this.Assign<bool?>(transpositions, (Action<IIntervalsFuzzy, bool?>) ((a, v) => a.Transpositions = v));

    public IntervalsFuzzyDescriptor Fuzziness(Nest.Fuzziness fuzziness) => this.Assign<Nest.Fuzziness>(fuzziness, (Action<IIntervalsFuzzy, Nest.Fuzziness>) ((a, v) => a.Fuzziness = v));

    public IntervalsFuzzyDescriptor Term(string term) => this.Assign<string>(term, (Action<IIntervalsFuzzy, string>) ((a, v) => a.Term = v));

    public IntervalsFuzzyDescriptor UseField<T>(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<IIntervalsFuzzy, Expression<Func<T, object>>>) ((a, v) => a.UseField = (Field) (Expression) v));

    public IntervalsFuzzyDescriptor UseField(Field useField) => this.Assign<Field>(useField, (Action<IIntervalsFuzzy, Field>) ((a, v) => a.UseField = v));
  }
}
