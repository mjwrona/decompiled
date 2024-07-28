// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility.BottomUpDedupProcessorFactory
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
  public class BottomUpDedupProcessorFactory
  {
    private const int c_maxpar = 128;
    private readonly SerializedBottomUpDedupProcessor iproc;
    private readonly VssRequestPump.Processor m_processor;
    private readonly DedupTraversalConfig m_config;
    private readonly Func<DedupLink, Task> m_processAsync;
    private readonly Action<TraceLevel, string> m_log;
    private readonly bool m_alwaysProcessSerially;

    public BottomUpDedupProcessorFactory(
      VssRequestPump.Processor processor,
      DedupTraversalConfig config,
      bool alwaysProcessSerially,
      Func<DedupLink, Task> processAsync,
      Action<TraceLevel, string> log)
    {
      this.iproc = new SerializedBottomUpDedupProcessor(this.m_processor = processor, this.m_config = config, this.m_processAsync = processAsync, this.m_log = log);
      this.m_alwaysProcessSerially = alwaysProcessSerially;
    }

    public IBottomUpDedupProcessor CreateProcessor(IBottomUpConcurrencyEvaluator evaluator)
    {
      if (this.m_alwaysProcessSerially)
        return (IBottomUpDedupProcessor) this.iproc.Reset();
      int num1 = evaluator.Evaluate();
      if (num1 <= 1)
        return (IBottomUpDedupProcessor) this.iproc.Reset();
      if (num1 >= 128)
        num1 = 128;
      int num2 = num1 * 1000;
      DedupProcessingConfig config = new DedupProcessingConfig()
      {
        DispatchingAndProcessingCapacity = num2,
        ProcessingParallelism = num1,
        Profiler = this.m_config.Profiler
      };
      return (IBottomUpDedupProcessor) new ParallelizedBottomUpDedupProcessor(num1 >= 128 ? int.MaxValue : num2, this.m_processor, config, this.m_processAsync, this.m_log);
    }
  }
}
