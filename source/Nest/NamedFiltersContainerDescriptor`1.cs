// Decompiled with JetBrains decompiler
// Type: Nest.NamedFiltersContainerDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class NamedFiltersContainerDescriptor<T> : 
    IsADictionaryDescriptorBase<NamedFiltersContainerDescriptor<T>, INamedFiltersContainer, string, IQueryContainer>
    where T : class
  {
    public NamedFiltersContainerDescriptor()
      : base((INamedFiltersContainer) new NamedFiltersContainer())
    {
    }

    public NamedFiltersContainerDescriptor<T> Filter(string name, IQueryContainer filter) => this.Assign(name, filter);

    public NamedFiltersContainerDescriptor<T> Filter(
      string name,
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign(name, selector != null ? (IQueryContainer) selector(new QueryContainerDescriptor<T>()) : (IQueryContainer) null);
    }

    public NamedFiltersContainerDescriptor<T> Filter<TOther>(
      string name,
      Func<QueryContainerDescriptor<TOther>, QueryContainer> selector)
      where TOther : class
    {
      return this.Assign(name, selector != null ? (IQueryContainer) selector(new QueryContainerDescriptor<TOther>()) : (IQueryContainer) null);
    }
  }
}
