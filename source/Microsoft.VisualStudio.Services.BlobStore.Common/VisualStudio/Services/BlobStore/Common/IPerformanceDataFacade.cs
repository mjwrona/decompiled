// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.IPerformanceDataFacade
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  internal interface IPerformanceDataFacade : IDisposable
  {
    bool IsInitialized { get; }

    void RegisterCounterSet(
      Guid counterSetGuid,
      string counterSetName,
      CounterSetInstanceType instanceType);

    void AddCounterToSet(
      Guid counterSetGuid,
      int counterId,
      CounterType counterType,
      string counterName,
      int baseId);

    void CreateCounterSetInstance(Guid counterSetGuid);

    void CompleteInitialization();

    PerfCounter GetCounter(string counterSetName, string counterName);

    IReadOnlyCollection<PerfCounter> GetCounters();
  }
}
