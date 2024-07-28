// Decompiled with JetBrains decompiler
// Type: Nest.TokenFilters
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class TokenFilters : 
    IsADictionaryBase<string, ITokenFilter>,
    ITokenFilters,
    IIsADictionary<string, ITokenFilter>,
    IDictionary<string, ITokenFilter>,
    ICollection<KeyValuePair<string, ITokenFilter>>,
    IEnumerable<KeyValuePair<string, ITokenFilter>>,
    IEnumerable,
    IIsADictionary
  {
    public TokenFilters()
    {
    }

    public TokenFilters(IDictionary<string, ITokenFilter> container)
      : base(container)
    {
    }

    public TokenFilters(Dictionary<string, ITokenFilter> container)
      : base((IDictionary<string, ITokenFilter>) container)
    {
    }

    public void Add(string name, ITokenFilter analyzer) => this.BackingDictionary.Add(name, analyzer);
  }
}
