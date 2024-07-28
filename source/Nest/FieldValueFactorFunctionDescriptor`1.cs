// Decompiled with JetBrains decompiler
// Type: Nest.FieldValueFactorFunctionDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class FieldValueFactorFunctionDescriptor<T> : 
    FunctionScoreFunctionDescriptorBase<FieldValueFactorFunctionDescriptor<T>, IFieldValueFactorFunction, T>,
    IFieldValueFactorFunction,
    IScoreFunction
    where T : class
  {
    double? IFieldValueFactorFunction.Factor { get; set; }

    Nest.Field IFieldValueFactorFunction.Field { get; set; }

    double? IFieldValueFactorFunction.Missing { get; set; }

    FieldValueFactorModifier? IFieldValueFactorFunction.Modifier { get; set; }

    public FieldValueFactorFunctionDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IFieldValueFactorFunction, Nest.Field>) ((a, v) => a.Field = v));

    public FieldValueFactorFunctionDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IFieldValueFactorFunction, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public FieldValueFactorFunctionDescriptor<T> Factor(double? factor) => this.Assign<double?>(factor, (Action<IFieldValueFactorFunction, double?>) ((a, v) => a.Factor = v));

    public FieldValueFactorFunctionDescriptor<T> Modifier(FieldValueFactorModifier? modifier) => this.Assign<FieldValueFactorModifier?>(modifier, (Action<IFieldValueFactorFunction, FieldValueFactorModifier?>) ((a, v) => a.Modifier = v));

    public FieldValueFactorFunctionDescriptor<T> Missing(double? missing) => this.Assign<double?>(missing, (Action<IFieldValueFactorFunction, double?>) ((a, v) => a.Missing = v));
  }
}
