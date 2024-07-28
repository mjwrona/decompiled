// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsMatchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class IntervalsMatchDescriptor : 
    IntervalsDescriptorBase<IntervalsMatchDescriptor, IIntervalsMatch>,
    IIntervalsMatch,
    IIntervals
  {
    string IIntervalsMatch.Analyzer { get; set; }

    int? IIntervalsMatch.MaxGaps { get; set; }

    bool? IIntervalsMatch.Ordered { get; set; }

    string IIntervalsMatch.Query { get; set; }

    Field IIntervalsMatch.UseField { get; set; }

    public IntervalsMatchDescriptor Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IIntervalsMatch, string>) ((a, v) => a.Analyzer = v));

    public IntervalsMatchDescriptor MaxGaps(int? maxGaps) => this.Assign<int?>(maxGaps, (Action<IIntervalsMatch, int?>) ((a, v) => a.MaxGaps = v));

    public IntervalsMatchDescriptor Ordered(bool? ordered = true) => this.Assign<bool?>(ordered, (Action<IIntervalsMatch, bool?>) ((a, v) => a.Ordered = v));

    public IntervalsMatchDescriptor Query(string query) => this.Assign<string>(query, (Action<IIntervalsMatch, string>) ((a, v) => a.Query = v));

    public IntervalsMatchDescriptor UseField<T>(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<IIntervalsMatch, Expression<Func<T, object>>>) ((a, v) => a.UseField = (Field) (Expression) v));

    public IntervalsMatchDescriptor UseField(Field useField) => this.Assign<Field>(useField, (Action<IIntervalsMatch, Field>) ((a, v) => a.UseField = v));
  }
}
