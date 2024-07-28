// Decompiled with JetBrains decompiler
// Type: Nest.IReindexRequest`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public interface IReindexRequest<TSource, TTarget>
    where TSource : class
    where TTarget : class
  {
    int? BackPressureFactor { get; set; }

    Func<IEnumerable<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>> BulkAll { get; set; }

    ICreateIndexRequest CreateIndexRequest { get; set; }

    Func<TSource, TTarget> Map { get; set; }

    bool OmitIndexCreation { get; set; }

    IScrollAllRequest ScrollAll { get; set; }
  }
}
