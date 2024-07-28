// Decompiled with JetBrains decompiler
// Type: Nest.DecayFunctionDescriptorBase`4
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public abstract class DecayFunctionDescriptorBase<TDescriptor, TOrigin, TScale, T> : 
    FunctionScoreFunctionDescriptorBase<TDescriptor, IDecayFunction<TOrigin, TScale>, T>,
    IDecayFunction<TOrigin, TScale>,
    IDecayFunction,
    IScoreFunction
    where TDescriptor : DecayFunctionDescriptorBase<TDescriptor, TOrigin, TScale, T>, IDecayFunction<TOrigin, TScale>
    where T : class
  {
    protected abstract string DecayType { get; }

    double? IDecayFunction.Decay { get; set; }

    string IDecayFunction.DecayType => this.DecayType;

    Nest.Field IDecayFunction.Field { get; set; }

    Nest.MultiValueMode? IDecayFunction.MultiValueMode { get; set; }

    TScale IDecayFunction<TOrigin, TScale>.Offset { get; set; }

    TOrigin IDecayFunction<TOrigin, TScale>.Origin { get; set; }

    TScale IDecayFunction<TOrigin, TScale>.Scale { get; set; }

    public TDescriptor Origin(TOrigin origin) => this.Assign<TOrigin>(origin, (Action<IDecayFunction<TOrigin, TScale>, TOrigin>) ((a, v) => a.Origin = v));

    public TDescriptor Scale(TScale scale) => this.Assign<TScale>(scale, (Action<IDecayFunction<TOrigin, TScale>, TScale>) ((a, v) => a.Scale = v));

    public TDescriptor Offset(TScale offset) => this.Assign<TScale>(offset, (Action<IDecayFunction<TOrigin, TScale>, TScale>) ((a, v) => a.Offset = v));

    public TDescriptor Decay(double? decay) => this.Assign<double?>(decay, (Action<IDecayFunction<TOrigin, TScale>, double?>) ((a, v) => a.Decay = v));

    public TDescriptor MultiValueMode(Nest.MultiValueMode? mode) => this.Assign<Nest.MultiValueMode?>(mode, (Action<IDecayFunction<TOrigin, TScale>, Nest.MultiValueMode?>) ((a, v) => a.MultiValueMode = v));

    public TDescriptor Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDecayFunction<TOrigin, TScale>, Nest.Field>) ((a, v) => a.Field = v));

    public TDescriptor Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IDecayFunction<TOrigin, TScale>, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
  }
}
