// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility.ContextBasedConcurrencyEvaluator
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility
{
  public class ContextBasedConcurrencyEvaluator : IBottomUpConcurrencyEvaluator
  {
    private IDedupVisitorOuterContext context;

    internal int MaxConcurrency { get; private set; }

    public int Evaluate() => (int) Math.Ceiling((double) this.MaxConcurrency / (double) this.context.RemainingRoots);

    public ContextBasedConcurrencyEvaluator(int maxConcurrency, IDedupVisitorOuterContext context)
    {
      this.MaxConcurrency = maxConcurrency;
      this.context = context;
    }
  }
}
