// Decompiled with JetBrains decompiler
// Type: Nest.EwmaModelDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class EwmaModelDescriptor : 
    DescriptorBase<EwmaModelDescriptor, IEwmaModel>,
    IEwmaModel,
    IMovingAverageModel
  {
    float? IEwmaModel.Alpha { get; set; }

    string IMovingAverageModel.Name { get; } = "ewma";

    public EwmaModelDescriptor Alpha(float? alpha) => this.Assign<float?>(alpha, (Action<IEwmaModel, float?>) ((a, v) => a.Alpha = v));
  }
}
