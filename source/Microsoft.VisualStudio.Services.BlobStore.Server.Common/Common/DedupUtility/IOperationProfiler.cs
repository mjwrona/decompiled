// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.IOperationProfiler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public interface IOperationProfiler : IDisposable
  {
    void AddCategory(ProfilingCategory category);

    void AddExternalCounter(ExternalCounter extCounter);

    void AddDerivedCounter(DerivedCounter extCounter);

    void Increment(ProfilingCategory category);

    void Decrement(ProfilingCategory category);

    void Update(ProfilingCategory category, long val);

    void Reset();

    int TimeElapsedInSecond { get; }

    IProfilingResult GetResult(ProfilingCategory category);

    IProfilingResult GetResult(string category);
  }
}
