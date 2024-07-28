// Decompiled with JetBrains decompiler
// Type: Nest.Aliases
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class Aliases : 
    IsADictionaryBase<IndexName, IAlias>,
    IAliases,
    IIsADictionary<IndexName, IAlias>,
    IDictionary<IndexName, IAlias>,
    ICollection<KeyValuePair<IndexName, IAlias>>,
    IEnumerable<KeyValuePair<IndexName, IAlias>>,
    IEnumerable,
    IIsADictionary
  {
    public Aliases()
    {
    }

    public Aliases(IDictionary<IndexName, IAlias> container)
      : base(container)
    {
    }

    public Aliases(Dictionary<IndexName, IAlias> container)
      : base((IDictionary<IndexName, IAlias>) container)
    {
    }

    public void Add(IndexName index, IAlias alias) => this.BackingDictionary.Add(index, alias);
  }
}
