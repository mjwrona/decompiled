// Decompiled with JetBrains decompiler
// Type: Nest.FunctionScoreFunctionDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class FunctionScoreFunctionDescriptorBase<TDescriptor, TInterface, T> : 
    DescriptorBase<TDescriptor, TInterface>,
    IScoreFunction
    where TDescriptor : FunctionScoreFunctionDescriptorBase<TDescriptor, TInterface, T>, TInterface, IScoreFunction
    where TInterface : class, IScoreFunction
    where T : class
  {
    QueryContainer IScoreFunction.Filter { get; set; }

    double? IScoreFunction.Weight { get; set; }

    public TDescriptor Filter(
      Func<QueryContainerDescriptor<T>, QueryContainer> filterSelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(filterSelector, (Action<TInterface, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Filter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public TDescriptor Weight(double? weight) => this.Assign<double?>(weight, (Action<TInterface, double?>) ((a, v) => a.Weight = v));
  }
}
