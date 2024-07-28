// Decompiled with JetBrains decompiler
// Type: Nest.PerFieldAnalyzer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class PerFieldAnalyzer : 
    IsADictionaryBase<Field, string>,
    IPerFieldAnalyzer,
    IIsADictionary<Field, string>,
    IDictionary<Field, string>,
    ICollection<KeyValuePair<Field, string>>,
    IEnumerable<KeyValuePair<Field, string>>,
    IEnumerable,
    IIsADictionary
  {
    public PerFieldAnalyzer()
    {
    }

    public PerFieldAnalyzer(IDictionary<Field, string> container)
      : base(container)
    {
    }

    public PerFieldAnalyzer(Dictionary<Field, string> container)
      : base((IDictionary<Field, string>) container)
    {
    }

    public void Add(Field field, string analyzer) => this.BackingDictionary.Add(field, analyzer);
  }
}
