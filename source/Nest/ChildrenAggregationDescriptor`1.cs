// Decompiled with JetBrains decompiler
// Type: Nest.ChildrenAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ChildrenAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<ChildrenAggregationDescriptor<T>, IChildrenAggregation, T>,
    IChildrenAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    RelationName IChildrenAggregation.Type { get; set; } = (RelationName) typeof (T);

    public ChildrenAggregationDescriptor<T> Type(RelationName type) => this.Assign<RelationName>(type, (Action<IChildrenAggregation, RelationName>) ((a, v) => a.Type = v));

    public ChildrenAggregationDescriptor<T> Type<TChildType>() where TChildType : class => this.Assign<System.Type>(typeof (TChildType), (Action<IChildrenAggregation, System.Type>) ((a, v) => a.Type = (RelationName) v));
  }
}
