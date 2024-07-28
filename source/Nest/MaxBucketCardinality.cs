// Decompiled with JetBrains decompiler
// Type: Nest.MaxBucketCardinality
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class MaxBucketCardinality : 
    IsADictionaryBase<Field, long>,
    IMaxBucketCardinality,
    IIsADictionary<Field, long>,
    IDictionary<Field, long>,
    ICollection<KeyValuePair<Field, long>>,
    IEnumerable<KeyValuePair<Field, long>>,
    IEnumerable,
    IIsADictionary
  {
    public MaxBucketCardinality()
    {
    }

    public MaxBucketCardinality(IDictionary<Field, long> container)
      : base(container)
    {
    }

    public MaxBucketCardinality(Dictionary<Field, long> container)
      : base((IDictionary<Field, long>) container)
    {
    }

    public void Add(Field field, long cardinality) => this.BackingDictionary.Add(field, cardinality);
  }
}
