// Decompiled with JetBrains decompiler
// Type: Nest.CompositeAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class CompositeAggregation : 
    BucketAggregationBase,
    ICompositeAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal CompositeAggregation()
    {
    }

    public CompositeAggregation(string name)
      : base(name)
    {
    }

    public CompositeKey After { get; set; }

    public int? Size { get; set; }

    public IEnumerable<ICompositeAggregationSource> Sources { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.Composite = (ICompositeAggregation) this;
  }
}
