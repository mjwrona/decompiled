// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Utils.ParallelHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Utils
{
  internal static class ParallelHelper
  {
    public static Task ForEachAsync<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, Task> worker,
      int maxParallelTaskCount = 0,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (maxParallelTaskCount <= 0)
        maxParallelTaskCount = 100;
      return Task.WhenAll(Partitioner.Create<TSource>(source).GetPartitions(maxParallelTaskCount).Select<IEnumerator<TSource>, Task>((Func<IEnumerator<TSource>, Task>) (partition => Task.Run((Func<Task>) (() =>
      {
        using (partition)
        {
          while (partition.MoveNext())
          {
            cancellationToken.ThrowIfCancellationRequested();
            await worker(partition.Current).ConfigureAwait(false);
          }
        }
      })))));
    }
  }
}
