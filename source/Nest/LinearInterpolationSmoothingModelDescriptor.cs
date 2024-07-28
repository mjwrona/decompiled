// Decompiled with JetBrains decompiler
// Type: Nest.LinearInterpolationSmoothingModelDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class LinearInterpolationSmoothingModelDescriptor : 
    DescriptorBase<LinearInterpolationSmoothingModelDescriptor, ILinearInterpolationSmoothingModel>,
    ILinearInterpolationSmoothingModel,
    ISmoothingModel
  {
    double? ILinearInterpolationSmoothingModel.BigramLambda { get; set; }

    double? ILinearInterpolationSmoothingModel.TrigramLambda { get; set; }

    double? ILinearInterpolationSmoothingModel.UnigramLambda { get; set; }

    public LinearInterpolationSmoothingModelDescriptor TrigramLambda(double? lambda) => this.Assign<double?>(lambda, (Action<ILinearInterpolationSmoothingModel, double?>) ((a, v) => a.TrigramLambda = v));

    public LinearInterpolationSmoothingModelDescriptor UnigramLambda(double? lambda) => this.Assign<double?>(lambda, (Action<ILinearInterpolationSmoothingModel, double?>) ((a, v) => a.UnigramLambda = v));

    public LinearInterpolationSmoothingModelDescriptor BigramLambda(double? lambda) => this.Assign<double?>(lambda, (Action<ILinearInterpolationSmoothingModel, double?>) ((a, v) => a.BigramLambda = v));
  }
}
