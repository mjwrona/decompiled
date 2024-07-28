// Decompiled with JetBrains decompiler
// Type: Nest.MultiBucketsPath
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class MultiBucketsPath : 
    IsADictionaryBase<string, string>,
    IMultiBucketsPath,
    IIsADictionary<string, string>,
    IDictionary<string, string>,
    ICollection<KeyValuePair<string, string>>,
    IEnumerable<KeyValuePair<string, string>>,
    IEnumerable,
    IIsADictionary,
    IBucketsPath
  {
    public MultiBucketsPath()
    {
    }

    public MultiBucketsPath(IDictionary<string, string> container)
      : base(container)
    {
    }

    public MultiBucketsPath(Dictionary<string, string> container)
      : base((IDictionary<string, string>) container)
    {
    }

    public void Add(string name, string bucketsPath) => this.BackingDictionary.Add(name, bucketsPath);

    public static implicit operator MultiBucketsPath(Dictionary<string, string> bucketsPath) => new MultiBucketsPath(bucketsPath);
  }
}
