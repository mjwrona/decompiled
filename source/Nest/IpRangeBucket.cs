// Decompiled with JetBrains decompiler
// Type: Nest.IpRangeBucket
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class IpRangeBucket : BucketBase
  {
    public IpRangeBucket(IReadOnlyDictionary<string, IAggregate> dict)
      : base(dict)
    {
    }

    public long DocCount { get; set; }

    public string From { get; set; }

    public string Key { get; set; }

    public string To { get; set; }
  }
}
