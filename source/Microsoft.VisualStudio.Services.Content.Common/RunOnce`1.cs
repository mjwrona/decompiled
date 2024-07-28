// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.RunOnce`1
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class RunOnce<TKey>
  {
    private readonly RunOnce<TKey, RunOnce<TKey>.Void> inner;

    public RunOnce(bool consolidateExceptions) => this.inner = new RunOnce<TKey, RunOnce<TKey>.Void>(consolidateExceptions);

    public RunOnce(bool consolidateExceptions, IEqualityComparer<TKey> comparer) => this.inner = new RunOnce<TKey, RunOnce<TKey>.Void>(consolidateExceptions);

    public Task RunOnceAsync(TKey key, Func<Task> taskFunc) => (Task) this.inner.RunOnceAsync(key, (Func<Task<RunOnce<TKey>.Void>>) (async () =>
    {
      await taskFunc().ConfigureAwait(false);
      return new RunOnce<TKey>.Void();
    }));

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private struct Void
    {
    }
  }
}
