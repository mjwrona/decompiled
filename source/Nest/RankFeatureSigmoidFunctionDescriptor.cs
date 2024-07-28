// Decompiled with JetBrains decompiler
// Type: Nest.RankFeatureSigmoidFunctionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RankFeatureSigmoidFunctionDescriptor : 
    DescriptorBase<RankFeatureSigmoidFunctionDescriptor, IRankFeatureSigmoidFunction>,
    IRankFeatureSigmoidFunction,
    IRankFeatureFunction
  {
    float IRankFeatureSigmoidFunction.Exponent { get; set; }

    float IRankFeatureSigmoidFunction.Pivot { get; set; }

    public RankFeatureSigmoidFunctionDescriptor Exponent(float exponent) => this.Assign<float>(exponent, (Action<IRankFeatureSigmoidFunction, float>) ((a, v) => a.Exponent = v));

    public RankFeatureSigmoidFunctionDescriptor Pivot(float pivot) => this.Assign<float>(pivot, (Action<IRankFeatureSigmoidFunction, float>) ((a, v) => a.Pivot = v));
  }
}
