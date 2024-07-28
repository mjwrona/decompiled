// Decompiled with JetBrains decompiler
// Type: Nest.CustomSimilarity
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class CustomSimilarity : 
    IsADictionaryBase<string, object>,
    ICustomSimilarity,
    ISimilarity,
    IIsADictionary<string, object>,
    IDictionary<string, object>,
    ICollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable,
    IIsADictionary
  {
    public CustomSimilarity(string type)
    {
      if (string.IsNullOrEmpty(type))
        return;
      this.Type = type;
    }

    internal CustomSimilarity(IDictionary<string, object> container)
      : base(container)
    {
    }

    internal CustomSimilarity(Dictionary<string, object> container)
      : base((IDictionary<string, object>) container)
    {
    }

    public string Type
    {
      get => this["type"] as string;
      set => this.Add("type", (object) value);
    }

    public void Add(string key, object value) => this.BackingDictionary.Add(key, value);
  }
}
