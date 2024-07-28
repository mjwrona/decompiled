// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ParallelAsync
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal static class ParallelAsync
  {
    public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body) => Task.WhenAll(Partitioner.Create<T>(source).GetPartitions(dop).Select<IEnumerator<T>, Task>((Func<IEnumerator<T>, Task>) (partition => Task.Factory.StartNewOnCurrentTaskSchedulerAsync<Task>((Func<Task>) (() =>
    {
      using (partition)
      {
        while (partition.MoveNext())
          await body(partition.Current);
      }
    }), TaskCreationOptions.DenyChildAttach).Unwrap())));
  }
}
