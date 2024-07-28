// Decompiled with JetBrains decompiler
// Type: Nest.NamedFiltersContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class NamedFiltersContainer : 
    IsADictionaryBase<string, IQueryContainer>,
    INamedFiltersContainer,
    IIsADictionary<string, IQueryContainer>,
    IDictionary<string, IQueryContainer>,
    ICollection<KeyValuePair<string, IQueryContainer>>,
    IEnumerable<KeyValuePair<string, IQueryContainer>>,
    IEnumerable,
    IIsADictionary
  {
    public NamedFiltersContainer()
    {
    }

    public NamedFiltersContainer(IDictionary<string, IQueryContainer> container)
      : base(container)
    {
    }

    public NamedFiltersContainer(Dictionary<string, QueryContainer> container)
      : base((IDictionary<string, IQueryContainer>) container.Select<KeyValuePair<string, QueryContainer>, KeyValuePair<string, QueryContainer>>((Func<KeyValuePair<string, QueryContainer>, KeyValuePair<string, QueryContainer>>) (kv => kv)).ToDictionary<KeyValuePair<string, QueryContainer>, string, IQueryContainer>((Func<KeyValuePair<string, QueryContainer>, string>) (kv => kv.Key), (Func<KeyValuePair<string, QueryContainer>, IQueryContainer>) (kv => (IQueryContainer) kv.Value)))
    {
    }

    public void Add(string name, IQueryContainer filter) => this.BackingDictionary.Add(name, filter);

    public void Add(string name, QueryContainer filter) => this.BackingDictionary.Add(name, (IQueryContainer) filter);
  }
}
