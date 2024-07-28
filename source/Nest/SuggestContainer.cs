// Decompiled with JetBrains decompiler
// Type: Nest.SuggestContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class SuggestContainer : 
    IsADictionaryBase<string, ISuggestBucket>,
    ISuggestContainer,
    IIsADictionary<string, ISuggestBucket>,
    IDictionary<string, ISuggestBucket>,
    ICollection<KeyValuePair<string, ISuggestBucket>>,
    IEnumerable<KeyValuePair<string, ISuggestBucket>>,
    IEnumerable,
    IIsADictionary
  {
    public SuggestContainer()
    {
    }

    public SuggestContainer(IDictionary<string, ISuggestBucket> container)
      : base(container)
    {
    }

    public SuggestContainer(Dictionary<string, ISuggestBucket> container)
      : base((IDictionary<string, ISuggestBucket>) container)
    {
    }

    public void Add(string name, ISuggestBucket script) => this.BackingDictionary.Add(name, script);
  }
}
