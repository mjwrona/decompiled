// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility.SerializedBottomUpDedupProcessor
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility
{
  public class SerializedBottomUpDedupProcessor : IBottomUpDedupProcessor
  {
    private int c_total = 1000;
    private int counter;
    private DedupProcessor proc;

    internal SerializedBottomUpDedupProcessor(
      VssRequestPump.Processor processor,
      DedupTraversalConfig config,
      Func<DedupLink, Task> processAsync,
      Action<TraceLevel, string> log)
    {
      this.proc = new DedupProcessor(processor, (DedupProcessingConfig) config, processAsync, log, true);
      this.Reset();
    }

    public async Task<bool> SendAsync(DedupLink link)
    {
      await this.proc.RunAsync(link);
      --this.counter;
      return this.counter <= 0;
    }

    public Task WaitForCompletionAsync() => (Task) Task.FromResult<int>(0);

    internal SerializedBottomUpDedupProcessor Reset()
    {
      this.counter = this.c_total;
      return this;
    }

    public int Parallelism => 1;
  }
}
