// Decompiled with JetBrains decompiler
// Type: Nest.InferenceConfigDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class InferenceConfigDescriptor<T> : 
    DescriptorBase<InferenceConfigDescriptor<T>, IInferenceConfig>,
    IInferenceConfig
  {
    IRegressionInferenceConfig IInferenceConfig.Regression { get; set; }

    IClassificationInferenceConfig IInferenceConfig.Classification { get; set; }

    public InferenceConfigDescriptor<T> Regression(
      Func<RegressionInferenceConfigDescriptor<T>, IRegressionInferenceConfig> selector)
    {
      return this.Assign<Func<RegressionInferenceConfigDescriptor<T>, IRegressionInferenceConfig>>(selector, (Action<IInferenceConfig, Func<RegressionInferenceConfigDescriptor<T>, IRegressionInferenceConfig>>) ((a, v) => a.Regression = v.InvokeOrDefault<RegressionInferenceConfigDescriptor<T>, IRegressionInferenceConfig>(new RegressionInferenceConfigDescriptor<T>())));
    }

    public InferenceConfigDescriptor<T> Classification(
      Func<ClassificationInferenceConfigDescriptor<T>, IClassificationInferenceConfig> selector)
    {
      return this.Assign<Func<ClassificationInferenceConfigDescriptor<T>, IClassificationInferenceConfig>>(selector, (Action<IInferenceConfig, Func<ClassificationInferenceConfigDescriptor<T>, IClassificationInferenceConfig>>) ((a, v) => a.Classification = v.InvokeOrDefault<ClassificationInferenceConfigDescriptor<T>, IClassificationInferenceConfig>(new ClassificationInferenceConfigDescriptor<T>())));
    }
  }
}
