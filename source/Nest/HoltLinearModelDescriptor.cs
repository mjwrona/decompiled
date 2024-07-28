// Decompiled with JetBrains decompiler
// Type: Nest.HoltLinearModelDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HoltLinearModelDescriptor : 
    DescriptorBase<HoltLinearModelDescriptor, IHoltLinearModel>,
    IHoltLinearModel,
    IMovingAverageModel
  {
    float? IHoltLinearModel.Alpha { get; set; }

    float? IHoltLinearModel.Beta { get; set; }

    string IMovingAverageModel.Name { get; } = "holt";

    public HoltLinearModelDescriptor Alpha(float? alpha) => this.Assign<float?>(alpha, (Action<IHoltLinearModel, float?>) ((a, v) => a.Alpha = v));

    public HoltLinearModelDescriptor Beta(float? beta) => this.Assign<float?>(beta, (Action<IHoltLinearModel, float?>) ((a, v) => a.Beta = v));
  }
}
