// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Reactive.JustAsyncEnumerator`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Reactive
{
  internal sealed class JustAsyncEnumerator<T> : IAsyncEnumerator<T>, IAsyncDisposable
  {
    private readonly IEnumerator<T> enumerator;

    public JustAsyncEnumerator(params T[] items) => this.enumerator = (IEnumerator<T>) ((IEnumerable<T>) items).ToList<T>().GetEnumerator();

    public T Current => this.enumerator.Current;

    public ValueTask DisposeAsync() => new ValueTask();

    public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(this.enumerator.MoveNext());

    public ValueTask<bool> MoveNextAsync(ITrace trace) => new ValueTask<bool>(this.enumerator.MoveNext());
  }
}
