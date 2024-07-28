// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation.RootsValidationConfig
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation
{
  public class RootsValidationConfig : DedupTraversalConfig
  {
    private int m_parallelism = DedupProcessingConfig.DefaultInRootParallelism;

    public DateTimeOffset? Start { get; set; }

    public DateTimeOffset? End { get; set; }

    public string Scope { get; set; }

    public bool ScanOnly { get; set; }

    public int MaxFailuresBeforeAbortion { get; set; } = 5;

    public bool AdaptiveThreading { get; set; }

    public int RootLevelParallelism
    {
      get => this.m_parallelism;
      set
      {
        if (value < 0)
          return;
        this.m_parallelism = Math.Max(1, Math.Min(Environment.ProcessorCount * 4, value));
      }
    }
  }
}
