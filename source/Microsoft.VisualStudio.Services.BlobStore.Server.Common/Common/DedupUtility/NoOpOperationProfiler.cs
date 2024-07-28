// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.NoOpOperationProfiler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public class NoOpOperationProfiler : IOperationProfiler, IDisposable
  {
    public int TimeElapsedInSecond => 0;

    public void AddCategory(ProfilingCategory category)
    {
    }

    public void AddExternalCounter(ExternalCounter extCounter)
    {
    }

    public void AddDerivedCounter(DerivedCounter drvCounter)
    {
    }

    public void Increment(ProfilingCategory category)
    {
    }

    public void Decrement(ProfilingCategory category)
    {
    }

    public void Update(ProfilingCategory category, long val)
    {
    }

    public void Reset()
    {
    }

    public IProfilingResult GetResult(ProfilingCategory category) => (IProfilingResult) new NoOpProfilingResult();

    public IProfilingResult GetResult(string category) => (IProfilingResult) new NoOpProfilingResult();

    public void Dispose()
    {
    }
  }
}
