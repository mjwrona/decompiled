// Decompiled with JetBrains decompiler
// Type: Nest.ReindexSource
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ReindexSource : IReindexSource
  {
    public Indices Index { get; set; }

    public QueryContainer Query { get; set; }

    public IRemoteSource Remote { get; set; }

    public int? Size { get; set; }

    public ISlicedScroll Slice { get; set; }

    [Obsolete("Deprecated in 7.6.0. Instead consider using query filtering to find the desired subset of data.")]
    public IList<ISort> Sort { get; set; }

    public Fields Source { get; set; }
  }
}
