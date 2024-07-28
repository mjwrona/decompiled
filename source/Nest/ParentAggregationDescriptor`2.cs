// Decompiled with JetBrains decompiler
// Type: Nest.ParentAggregationDescriptor`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ParentAggregationDescriptor<T, TParent> : 
    BucketAggregationDescriptorBase<ParentAggregationDescriptor<T, TParent>, IParentAggregation, TParent>,
    IParentAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
    where TParent : class
  {
    RelationName IParentAggregation.Type { get; set; } = (RelationName) typeof (T);

    public ParentAggregationDescriptor<T, TParent> Type(RelationName type) => this.Assign<RelationName>(type, (Action<IParentAggregation, RelationName>) ((a, v) => a.Type = v));

    public ParentAggregationDescriptor<T, TParent> Type<TOtherParent>() => this.Assign<System.Type>(typeof (TOtherParent), (Action<IParentAggregation, System.Type>) ((a, v) => a.Type = (RelationName) v));
  }
}
