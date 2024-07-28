// Decompiled with JetBrains decompiler
// Type: Nest.KeyedBucket`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class KeyedBucket<TKey> : BucketBase
  {
    public KeyedBucket(IReadOnlyDictionary<string, IAggregate> dict)
      : base(dict)
    {
    }

    public long? DocCount { get; set; }

    public long? DocCountErrorUpperBound { get; set; }

    public TKey Key { get; set; }

    public string KeyAsString { get; set; }
  }
}
