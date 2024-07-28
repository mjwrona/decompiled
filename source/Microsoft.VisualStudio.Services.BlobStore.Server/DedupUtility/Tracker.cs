// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility.Tracker
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility
{
  internal class Tracker
  {
    private long count;
    private readonly SafeTaskCompletionSource<int> source;

    internal Tracker()
    {
      this.source = new SafeTaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
      this.Increment();
    }

    internal void Increment() => Interlocked.Increment(ref this.count);

    internal void Decrement()
    {
      if (Interlocked.Decrement(ref this.count) != 0L)
        return;
      this.source.TrySetResult(0);
    }

    internal void Abort() => this.source.TrySetResult(-1);

    internal Task Completion => (Task) this.source.Task;
  }
}
