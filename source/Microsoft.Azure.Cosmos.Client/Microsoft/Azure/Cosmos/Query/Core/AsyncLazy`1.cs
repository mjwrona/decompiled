// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.AsyncLazy`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core
{
  internal sealed class AsyncLazy<T>
  {
    private readonly Func<ITrace, CancellationToken, Task<T>> valueFactory;
    private T value;

    public AsyncLazy(
      Func<ITrace, CancellationToken, Task<T>> valueFactory)
    {
      this.valueFactory = valueFactory ?? throw new ArgumentNullException(nameof (valueFactory));
    }

    public bool ValueInitialized { get; private set; }

    public async Task<T> GetValueAsync(ITrace trace, CancellationToken cancellationToken)
    {
      if (!this.ValueInitialized)
      {
        this.value = await this.valueFactory(trace, cancellationToken);
        this.ValueInitialized = true;
      }
      return this.value;
    }

    public T Result
    {
      get
      {
        if (!this.ValueInitialized)
          throw new InvalidOperationException("Can not retrieve value before initialization.");
        return this.value;
      }
    }
  }
}
