// Decompiled with JetBrains decompiler
// Type: Nest.Normalizers
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class Normalizers : 
    IsADictionaryBase<string, INormalizer>,
    INormalizers,
    IIsADictionary<string, INormalizer>,
    IDictionary<string, INormalizer>,
    ICollection<KeyValuePair<string, INormalizer>>,
    IEnumerable<KeyValuePair<string, INormalizer>>,
    IEnumerable,
    IIsADictionary
  {
    public Normalizers()
    {
    }

    public Normalizers(IDictionary<string, INormalizer> container)
      : base(container)
    {
    }

    public Normalizers(Dictionary<string, INormalizer> container)
      : base((IDictionary<string, INormalizer>) container)
    {
    }

    public void Add(string name, INormalizer analyzer) => this.BackingDictionary.Add(name, analyzer);
  }
}
