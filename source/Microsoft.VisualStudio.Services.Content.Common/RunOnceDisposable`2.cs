// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.RunOnceDisposable`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class RunOnceDisposable<TKey, TResult> : RunOnce<TKey, TResult>, IDisposable where TResult : IDisposable
  {
    public RunOnceDisposable(bool consolidateExceptions)
      : base(consolidateExceptions)
    {
    }

    public RunOnceDisposable(bool consolidateExceptions, IEqualityComparer<TKey> comparer)
      : base(consolidateExceptions, comparer)
    {
    }

    public void Dispose()
    {
      foreach (KeyValuePair<TKey, Task<TResult>> task in this.TaskDictionary)
      {
        if (task.Value.IsCompleted)
          task.Value.Result.Dispose();
        else
          task.Value.ContinueWith((Action<Task<TResult>>) (t => t.Result.Dispose()));
      }
    }
  }
}
