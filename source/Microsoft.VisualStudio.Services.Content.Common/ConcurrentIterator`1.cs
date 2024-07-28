// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIterator`1
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ConcurrentIterator<TValue> : ConcurrentIterator<object, TValue>
  {
    private static readonly object[] SingleObjectArray = new object[1];

    public ConcurrentIterator(
      int? boundedCapacity,
      CancellationToken cancellationToken,
      Func<TryAddValueAsyncFunc<TValue>, CancellationToken, Task> producerTask)
      : base((IEnumerable<object>) ConcurrentIterator<TValue>.SingleObjectArray, boundedCapacity, cancellationToken, (Func<object, TryAddValueAsyncFunc<TValue>, CancellationToken, Task>) ((dummyObject, valueAdderAsync, cancelToken) => producerTask(valueAdderAsync, cancelToken)))
    {
    }

    public ConcurrentIterator(IEnumerable<TValue> items)
      : this(items.GetEnumerator())
    {
    }

    public ConcurrentIterator(IEnumerator<TValue> items)
      : base((IEnumerable<object>) ConcurrentIterator<TValue>.SingleObjectArray, new int?(1), CancellationToken.None, (Func<object, TryAddValueAsyncFunc<TValue>, CancellationToken, Task>) ((dummyObject, valueAdderAsync, cancelToken) => ConcurrentIterator<TValue>.ValueAdderAsync(valueAdderAsync, items)))
    {
    }

    private static async Task ValueAdderAsync(
      TryAddValueAsyncFunc<TValue> valueAdderAsync,
      IEnumerator<TValue> items)
    {
      while (items.MoveNext())
      {
        if (!await valueAdderAsync(items.Current).ConfigureAwait(false))
          break;
      }
    }
  }
}
