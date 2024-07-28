// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Common.AsyncLazy`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Common
{
  internal sealed class AsyncLazy<T> : Lazy<Task<T>>
  {
    public AsyncLazy(T value)
      : base((Func<Task<T>>) (() => Task.FromResult<T>(value)))
    {
    }

    public AsyncLazy(Func<T> valueFactory, CancellationToken cancellationToken)
      : base((Func<Task<T>>) (() => Task.Factory.StartNewOnCurrentTaskSchedulerAsync<T>(valueFactory, cancellationToken)))
    {
    }

    public AsyncLazy(Func<Task<T>> taskFactory, CancellationToken cancellationToken)
      : base((Func<Task<T>>) (() => Task.Factory.StartNewOnCurrentTaskSchedulerAsync<Task<T>>(taskFactory, cancellationToken).Unwrap<T>()))
    {
    }
  }
}
