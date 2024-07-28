// Decompiled with JetBrains decompiler
// Type: Nest.SmoothingModelContainerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SmoothingModelContainerDescriptor : SmoothingModelContainer
  {
    private SmoothingModelContainerDescriptor Assign<TValue>(
      TValue value,
      Action<ISmoothingModelContainer, TValue> assigner)
    {
      return Fluent.Assign<SmoothingModelContainerDescriptor, ISmoothingModelContainer, TValue>(this, value, assigner);
    }

    public SmoothingModelContainerDescriptor StupidBackoff(
      Func<StupidBackoffSmoothingModelDescriptor, IStupidBackoffSmoothingModel> selector)
    {
      return this.Assign<Func<StupidBackoffSmoothingModelDescriptor, IStupidBackoffSmoothingModel>>(selector, (Action<ISmoothingModelContainer, Func<StupidBackoffSmoothingModelDescriptor, IStupidBackoffSmoothingModel>>) ((a, v) => a.StupidBackoff = v != null ? v.InvokeOrDefault<StupidBackoffSmoothingModelDescriptor, IStupidBackoffSmoothingModel>(new StupidBackoffSmoothingModelDescriptor()) : (IStupidBackoffSmoothingModel) null));
    }

    public SmoothingModelContainerDescriptor LinearInterpolation(
      Func<LinearInterpolationSmoothingModelDescriptor, ILinearInterpolationSmoothingModel> selector)
    {
      return this.Assign<Func<LinearInterpolationSmoothingModelDescriptor, ILinearInterpolationSmoothingModel>>(selector, (Action<ISmoothingModelContainer, Func<LinearInterpolationSmoothingModelDescriptor, ILinearInterpolationSmoothingModel>>) ((a, v) => a.LinearInterpolation = v != null ? v.InvokeOrDefault<LinearInterpolationSmoothingModelDescriptor, ILinearInterpolationSmoothingModel>(new LinearInterpolationSmoothingModelDescriptor()) : (ILinearInterpolationSmoothingModel) null));
    }

    public SmoothingModelContainerDescriptor Laplace(
      Func<LaplaceSmoothingModelDescriptor, ILaplaceSmoothingModel> selector)
    {
      return this.Assign<Func<LaplaceSmoothingModelDescriptor, ILaplaceSmoothingModel>>(selector, (Action<ISmoothingModelContainer, Func<LaplaceSmoothingModelDescriptor, ILaplaceSmoothingModel>>) ((a, v) => a.Laplace = v != null ? v.InvokeOrDefault<LaplaceSmoothingModelDescriptor, ILaplaceSmoothingModel>(new LaplaceSmoothingModelDescriptor()) : (ILaplaceSmoothingModel) null));
    }
  }
}
