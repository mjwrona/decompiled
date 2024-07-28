// Decompiled with JetBrains decompiler
// Type: Nest.RankFeatureQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RankFeatureQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<RankFeatureQueryDescriptor<T>, IRankFeatureQuery, T>,
    IRankFeatureQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    IRankFeatureFunction IRankFeatureQuery.Function { get; set; }

    protected override bool Conditionless => RankFeatureQuery.IsConditionless((IRankFeatureQuery) this);

    public RankFeatureQueryDescriptor<T> Saturation(
      Func<RankFeatureSaturationFunctionDescriptor, IRankFeatureSaturationFunction> selector = null)
    {
      return this.Assign<Func<RankFeatureSaturationFunctionDescriptor, IRankFeatureSaturationFunction>>(selector, (Action<IRankFeatureQuery, Func<RankFeatureSaturationFunctionDescriptor, IRankFeatureSaturationFunction>>) ((a, v) => a.Function = (IRankFeatureFunction) v.InvokeOrDefault<RankFeatureSaturationFunctionDescriptor, IRankFeatureSaturationFunction>(new RankFeatureSaturationFunctionDescriptor())));
    }

    public RankFeatureQueryDescriptor<T> Logarithm(
      Func<RankFeatureLogarithmFunctionDescriptor, IRankFeatureLogarithmFunction> selector)
    {
      return this.Assign<Func<RankFeatureLogarithmFunctionDescriptor, IRankFeatureLogarithmFunction>>(selector, (Action<IRankFeatureQuery, Func<RankFeatureLogarithmFunctionDescriptor, IRankFeatureLogarithmFunction>>) ((a, v) => a.Function = v != null ? (IRankFeatureFunction) v(new RankFeatureLogarithmFunctionDescriptor()) : (IRankFeatureFunction) null));
    }

    public RankFeatureQueryDescriptor<T> Sigmoid(
      Func<RankFeatureSigmoidFunctionDescriptor, IRankFeatureSigmoidFunction> selector)
    {
      return this.Assign<Func<RankFeatureSigmoidFunctionDescriptor, IRankFeatureSigmoidFunction>>(selector, (Action<IRankFeatureQuery, Func<RankFeatureSigmoidFunctionDescriptor, IRankFeatureSigmoidFunction>>) ((a, v) => a.Function = v != null ? (IRankFeatureFunction) v(new RankFeatureSigmoidFunctionDescriptor()) : (IRankFeatureFunction) null));
    }

    public RankFeatureQueryDescriptor<T> Linear()
    {
      this.Self.Function = (IRankFeatureFunction) new RankFeatureLinearFunction();
      return this;
    }
  }
}
