// Decompiled with JetBrains decompiler
// Type: Nest.HoltWintersModelDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HoltWintersModelDescriptor : 
    DescriptorBase<HoltWintersModelDescriptor, IHoltWintersModel>,
    IHoltWintersModel,
    IMovingAverageModel
  {
    float? IHoltWintersModel.Alpha { get; set; }

    float? IHoltWintersModel.Beta { get; set; }

    float? IHoltWintersModel.Gamma { get; set; }

    string IMovingAverageModel.Name { get; } = "holt_winters";

    bool? IHoltWintersModel.Pad { get; set; }

    int? IHoltWintersModel.Period { get; set; }

    HoltWintersType? IHoltWintersModel.Type { get; set; }

    public HoltWintersModelDescriptor Alpha(float? alpha) => this.Assign<float?>(alpha, (Action<IHoltWintersModel, float?>) ((a, v) => a.Alpha = v));

    public HoltWintersModelDescriptor Beta(float? beta) => this.Assign<float?>(beta, (Action<IHoltWintersModel, float?>) ((a, v) => a.Beta = v));

    public HoltWintersModelDescriptor Gamma(float? gamma) => this.Assign<float?>(gamma, (Action<IHoltWintersModel, float?>) ((a, v) => a.Gamma = v));

    public HoltWintersModelDescriptor Pad(bool? pad = true) => this.Assign<bool?>(pad, (Action<IHoltWintersModel, bool?>) ((a, v) => a.Pad = v));

    public HoltWintersModelDescriptor Period(int? period) => this.Assign<int?>(period, (Action<IHoltWintersModel, int?>) ((a, v) => a.Period = v));

    public HoltWintersModelDescriptor Type(HoltWintersType? type) => this.Assign<HoltWintersType?>(type, (Action<IHoltWintersModel, HoltWintersType?>) ((a, v) => a.Type = v));
  }
}
