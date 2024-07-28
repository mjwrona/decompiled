// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Reactive.EmptyAsyncEnumerator`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Reactive
{
  internal sealed class EmptyAsyncEnumerator<T> : IAsyncEnumerator<T>, IAsyncDisposable
  {
    public T Current => default (T);

    public ValueTask DisposeAsync() => new ValueTask();

    public ValueTask<bool> MoveNextAsync() => this.MoveNextAsync((ITrace) NoOpTrace.Singleton);

    public ValueTask<bool> MoveNextAsync(ITrace trace) => new ValueTask<bool>(false);
  }
}
