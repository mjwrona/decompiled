// Decompiled with JetBrains decompiler
// Type: Nest.SmoothingModelContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SmoothingModelContainer : ISmoothingModelContainer, IDescriptor
  {
    internal SmoothingModelContainer()
    {
    }

    public SmoothingModelContainer(SmoothingModelBase model)
    {
      model.ThrowIfNull<SmoothingModelBase>(nameof (model));
      model.WrapInContainer((ISmoothingModelContainer) this);
    }

    ILaplaceSmoothingModel ISmoothingModelContainer.Laplace { get; set; }

    ILinearInterpolationSmoothingModel ISmoothingModelContainer.LinearInterpolation { get; set; }

    IStupidBackoffSmoothingModel ISmoothingModelContainer.StupidBackoff { get; set; }
  }
}
