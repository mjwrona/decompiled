// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.NoopPerformanceDataFacade
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Diagnostics.PerformanceData;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  internal class NoopPerformanceDataFacade : PerformanceDataFacade
  {
    internal NoopPerformanceDataFacade()
      : base(Guid.Empty)
    {
    }

    public override void RegisterCounterSet(
      Guid counterSetGuid,
      string counterSetName,
      CounterSetInstanceType instanceType)
    {
      PerfCounterSet perfCounterSet = new PerfCounterSet(counterSetGuid, counterSetName, instanceType);
      this.setsByGuid.Add(counterSetGuid, perfCounterSet);
      this.setsByName.Add(counterSetName, perfCounterSet);
    }

    public override void AddCounterToSet(
      Guid counterSetGuid,
      int counterId,
      CounterType counterType,
      string counterName,
      int baseId)
    {
      PerfCounterSet set = this.setsByGuid[counterSetGuid];
      set.Add((PerfCounter) new NoopPerfCounter(set, counterId, counterType, counterName, baseId), false);
    }

    public override void CreateCounterSetInstance(Guid counterSetGuid)
    {
    }
  }
}
