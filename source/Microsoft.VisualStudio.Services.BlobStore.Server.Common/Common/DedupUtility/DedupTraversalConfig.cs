// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.DedupTraversalConfig
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public class DedupTraversalConfig : DedupProcessingConfig
  {
    public static readonly int DefaultInRootTracingFrequency = 8192;
    private const int DefaultFrequency = 12;
    private int m_dispParallelism = DedupProcessingConfig.DefaultInRootParallelism;
    private int m_tracingFrequency = 12;

    public bool NoCache { get; set; }

    public bool IgnoreMissingNode { get; set; }

    public bool EnableDiagnostics { get; set; }

    protected internal bool TopDownSkipCollectingPathData { get; protected set; }

    protected internal virtual BottomUpTraversalOption PerformBottomUpTraversal { get; protected set; }

    public virtual int DispatchingParallelism
    {
      get => this.PerformBottomUpTraversal == BottomUpTraversalOption.Disabled ? this.m_dispParallelism : 1;
      set
      {
        if (value < 0)
          return;
        this.m_dispParallelism = Math.Max(1, Math.Min(DedupProcessingConfig.DefaultInRootParallelism * 16, value));
      }
    }

    public virtual int TracingFrequency
    {
      get => this.m_tracingFrequency;
      set
      {
        value = (int) (Math.Log((double) value) / Math.Log(2.0));
        this.m_tracingFrequency = Math.Max(3, Math.Min(16, value));
      }
    }
  }
}
