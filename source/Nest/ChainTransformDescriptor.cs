// Decompiled with JetBrains decompiler
// Type: Nest.ChainTransformDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ChainTransformDescriptor : 
    DescriptorBase<ChainTransformDescriptor, IChainTransform>,
    IChainTransform,
    ITransform
  {
    public ChainTransformDescriptor()
    {
    }

    public ChainTransformDescriptor(ICollection<TransformContainer> transforms) => this.Self.Transforms = transforms;

    ICollection<TransformContainer> IChainTransform.Transforms { get; set; }

    public ChainTransformDescriptor Transform(
      Func<TransformDescriptor, TransformContainer> selector)
    {
      if (this.Self.Transforms == null)
        this.Self.Transforms = (ICollection<TransformContainer>) new List<TransformContainer>();
      this.Self.Transforms.AddIfNotNull<TransformContainer>(selector != null ? selector(new TransformDescriptor()) : (TransformContainer) null);
      return this;
    }
  }
}
