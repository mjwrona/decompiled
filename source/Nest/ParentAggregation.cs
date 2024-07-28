// Decompiled with JetBrains decompiler
// Type: Nest.ParentAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class ParentAggregation : 
    BucketAggregationBase,
    IParentAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal ParentAggregation()
    {
    }

    public ParentAggregation(string name, RelationName type)
      : base(name)
    {
      this.Type = type;
    }

    public RelationName Type { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.Parent = (IParentAggregation) this;
  }
}
