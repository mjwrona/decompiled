// Decompiled with JetBrains decompiler
// Type: Nest.AliasDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AliasDescriptor : DescriptorBase<AliasDescriptor, IAlias>, IAlias
  {
    QueryContainer IAlias.Filter { get; set; }

    Nest.Routing IAlias.IndexRouting { get; set; }

    bool? IAlias.IsWriteIndex { get; set; }

    Nest.Routing IAlias.Routing { get; set; }

    Nest.Routing IAlias.SearchRouting { get; set; }

    bool? IAlias.IsHidden { get; set; }

    public AliasDescriptor Filter<T>(
      Func<QueryContainerDescriptor<T>, QueryContainer> filterSelector)
      where T : class
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(filterSelector, (Action<IAlias, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Filter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public AliasDescriptor IndexRouting(Nest.Routing indexRouting) => this.Assign<Nest.Routing>(indexRouting, (Action<IAlias, Nest.Routing>) ((a, v) => a.IndexRouting = v));

    public AliasDescriptor IsWriteIndex(bool? isWriteIndex = true) => this.Assign<bool?>(isWriteIndex, (Action<IAlias, bool?>) ((a, v) => a.IsWriteIndex = v));

    public AliasDescriptor IsHidden(bool? isHidden = true) => this.Assign<bool?>(isHidden, (Action<IAlias, bool?>) ((a, v) => a.IsHidden = v));

    public AliasDescriptor Routing(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<IAlias, Nest.Routing>) ((a, v) => a.Routing = v));

    public AliasDescriptor SearchRouting(Nest.Routing searchRouting) => this.Assign<Nest.Routing>(searchRouting, (Action<IAlias, Nest.Routing>) ((a, v) => a.SearchRouting = v));
  }
}
