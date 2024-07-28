// Decompiled with JetBrains decompiler
// Type: Nest.ParentIdQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ParentIdQueryDescriptor<T> : 
    QueryDescriptorBase<ParentIdQueryDescriptor<T>, IParentIdQuery>,
    IParentIdQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => ParentIdQuery.IsConditionless((IParentIdQuery) this);

    Nest.Id IParentIdQuery.Id { get; set; }

    bool? IParentIdQuery.IgnoreUnmapped { get; set; }

    RelationName IParentIdQuery.Type { get; set; }

    public ParentIdQueryDescriptor<T> Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<IParentIdQuery, Nest.Id>) ((a, v) => a.Id = v));

    public ParentIdQueryDescriptor<T> Type(RelationName type) => this.Assign<RelationName>(type, (Action<IParentIdQuery, RelationName>) ((a, v) => a.Type = v));

    public ParentIdQueryDescriptor<T> Type<TChild>() => this.Assign<System.Type>(typeof (TChild), (Action<IParentIdQuery, System.Type>) ((a, v) => a.Type = (RelationName) v));

    public ParentIdQueryDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<IParentIdQuery, bool?>) ((a, v) => a.IgnoreUnmapped = v));
  }
}
