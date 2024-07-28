// Decompiled with JetBrains decompiler
// Type: Nest.Analyzers
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class Analyzers : 
    IsADictionaryBase<string, IAnalyzer>,
    IAnalyzers,
    IIsADictionary<string, IAnalyzer>,
    IDictionary<string, IAnalyzer>,
    ICollection<KeyValuePair<string, IAnalyzer>>,
    IEnumerable<KeyValuePair<string, IAnalyzer>>,
    IEnumerable,
    IIsADictionary
  {
    public Analyzers()
    {
    }

    public Analyzers(IDictionary<string, IAnalyzer> container)
      : base(container)
    {
    }

    public Analyzers(Dictionary<string, IAnalyzer> container)
      : base((IDictionary<string, IAnalyzer>) container)
    {
    }

    public void Add(string name, IAnalyzer analyzer) => this.BackingDictionary.Add(name, analyzer);
  }
}
