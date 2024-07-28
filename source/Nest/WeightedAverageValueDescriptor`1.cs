// Decompiled with JetBrains decompiler
// Type: Nest.WeightedAverageValueDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class WeightedAverageValueDescriptor<T> : 
    DescriptorBase<WeightedAverageValueDescriptor<T>, IWeightedAverageValue>,
    IWeightedAverageValue
    where T : class
  {
    Nest.Field IWeightedAverageValue.Field { get; set; }

    double? IWeightedAverageValue.Missing { get; set; }

    IScript IWeightedAverageValue.Script { get; set; }

    public WeightedAverageValueDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IWeightedAverageValue, Nest.Field>) ((a, v) => a.Field = v));

    public WeightedAverageValueDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IWeightedAverageValue, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public virtual WeightedAverageValueDescriptor<T> Script(string script) => this.Assign<InlineScript>(new InlineScript(script), (Action<IWeightedAverageValue, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public virtual WeightedAverageValueDescriptor<T> Script(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IWeightedAverageValue, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }

    public WeightedAverageValueDescriptor<T> Missing(double? missing) => this.Assign<double?>(missing, (Action<IWeightedAverageValue, double?>) ((a, v) => a.Missing = v));
  }
}
