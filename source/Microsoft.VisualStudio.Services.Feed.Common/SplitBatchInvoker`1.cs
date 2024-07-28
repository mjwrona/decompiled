// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.SplitBatchInvoker`1
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public class SplitBatchInvoker<TIn>
  {
    private readonly IBatchSplitter batchSplitter;
    private readonly IAsyncInvoker invoker;

    public SplitBatchInvoker(IBatchSplitter batchSplitter, IAsyncInvoker invoker)
    {
      this.batchSplitter = batchSplitter ?? throw new ArgumentNullException(nameof (batchSplitter));
      this.invoker = invoker ?? throw new ArgumentNullException(nameof (invoker));
    }

    public async Task RunAsync(Func<IEnumerable<TIn>, Task> func, IEnumerable<TIn> items)
    {
      IEnumerable<IEnumerable<TIn>> ins1 = this.batchSplitter.Split<TIn>(items);
      List<Task> taskList = new List<Task>();
      foreach (IEnumerable<TIn> ins2 in ins1)
      {
        IEnumerable<TIn> batch = ins2;
        taskList.Add(this.invoker.Invoke((Func<Task>) (() => func(batch))));
      }
      await Task.WhenAll((IEnumerable<Task>) taskList);
    }
  }
}
