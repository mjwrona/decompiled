// Decompiled with JetBrains decompiler
// Type: Nest.MovingAverageModelDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MovingAverageModelDescriptor : 
    DescriptorBase<MovingAverageModelDescriptor, IDescriptor>
  {
    public IEwmaModel Ewma(Func<EwmaModelDescriptor, IEwmaModel> ewmaSelector = null) => ewmaSelector.InvokeOrDefault<EwmaModelDescriptor, IEwmaModel>(new EwmaModelDescriptor());

    public IHoltLinearModel HoltLinear(
      Func<HoltLinearModelDescriptor, IHoltLinearModel> holtSelector = null)
    {
      return holtSelector.InvokeOrDefault<HoltLinearModelDescriptor, IHoltLinearModel>(new HoltLinearModelDescriptor());
    }

    public IHoltWintersModel HoltWinters(
      Func<HoltWintersModelDescriptor, IHoltWintersModel> holtWintersSelector)
    {
      return holtWintersSelector == null ? (IHoltWintersModel) null : holtWintersSelector(new HoltWintersModelDescriptor());
    }

    public ILinearModel Linear(
      Func<LinearModelDescriptor, ILinearModel> linearSelector = null)
    {
      return linearSelector.InvokeOrDefault<LinearModelDescriptor, ILinearModel>(new LinearModelDescriptor());
    }

    public ISimpleModel Simple(
      Func<SimpleModelDescriptor, ISimpleModel> simpleSelector = null)
    {
      return simpleSelector.InvokeOrDefault<SimpleModelDescriptor, ISimpleModel>(new SimpleModelDescriptor());
    }
  }
}
