// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.ParallelPrefetch
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal static class ParallelPrefetch
  {
    public static async Task PrefetchInParallelAsync(
      IEnumerable<IPrefetcher> prefetchers,
      int maxConcurrency,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (prefetchers == null)
        throw new ArgumentNullException(nameof (prefetchers));
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      using (ITrace prefetchTrace = trace.StartChild("Prefetching", TraceComponent.Pagination, TraceLevel.Info))
      {
        HashSet<Task> tasks = new HashSet<Task>();
        IEnumerator<IPrefetcher> prefetchersEnumerator = prefetchers.GetEnumerator();
        for (int index = 0; index < maxConcurrency; ++index)
        {
          if (prefetchersEnumerator.MoveNext())
          {
            IPrefetcher prefetcher = prefetchersEnumerator.Current;
            tasks.Add(Task.Run((Func<Task>) (async () => await prefetcher.PrefetchAsync(prefetchTrace, cancellationToken))));
          }
          else
            break;
        }
        while (tasks.Count != 0)
        {
          Task task = await Task.WhenAny((IEnumerable<Task>) tasks);
          tasks.Remove(task);
          try
          {
            await task;
          }
          catch (object ex)
          {
            try
            {
              await Task.WhenAll((IEnumerable<Task>) tasks);
            }
            catch
            {
            }
            throw;
          }
          if (prefetchersEnumerator.MoveNext())
          {
            IPrefetcher bufferable = prefetchersEnumerator.Current;
            tasks.Add(Task.Run((Func<Task>) (async () => await bufferable.PrefetchAsync(prefetchTrace, cancellationToken))));
          }
        }
        tasks = (HashSet<Task>) null;
        prefetchersEnumerator = (IEnumerator<IPrefetcher>) null;
      }
    }
  }
}
