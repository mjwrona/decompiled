// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.DedupProcessingConfig
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public class DedupProcessingConfig
  {
    public static readonly int DefaultInRootParallelism = Environment.ProcessorCount;
    private int m_procParallelism = DedupProcessingConfig.DefaultInRootParallelism;
    private int m_cap = 10000;
    private IOperationProfiler m_profiler;

    public virtual int DispatchingAndProcessingCapacity
    {
      get => this.m_cap;
      set => this.m_cap = Math.Max(1000, Math.Min(200000, value));
    }

    public virtual int ProcessingParallelism
    {
      get => this.m_procParallelism;
      set
      {
        if (value < 0)
          return;
        this.m_procParallelism = Math.Max(1, Math.Min(DedupProcessingConfig.DefaultInRootParallelism * 16, value));
      }
    }

    public IOperationProfiler Profiler
    {
      get => this.m_profiler ?? (this.m_profiler = (IOperationProfiler) new NoOpOperationProfiler());
      set => this.m_profiler = value;
    }
  }
}
