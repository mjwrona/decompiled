// Decompiled with JetBrains decompiler
// Type: Nest.CharFilters
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class CharFilters : 
    IsADictionaryBase<string, ICharFilter>,
    ICharFilters,
    IIsADictionary<string, ICharFilter>,
    IDictionary<string, ICharFilter>,
    ICollection<KeyValuePair<string, ICharFilter>>,
    IEnumerable<KeyValuePair<string, ICharFilter>>,
    IEnumerable,
    IIsADictionary
  {
    public CharFilters()
    {
    }

    public CharFilters(IDictionary<string, ICharFilter> container)
      : base(container)
    {
    }

    public CharFilters(Dictionary<string, ICharFilter> container)
      : base((IDictionary<string, ICharFilter>) container)
    {
    }

    public void Add(string name, ICharFilter analyzer) => this.BackingDictionary.Add(name, analyzer);
  }
}
