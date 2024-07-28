// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.LoopBatchSizeConfigurer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  internal static class LoopBatchSizeConfigurer
  {
    public static void Configure(
      int? batchSize,
      ref int bigLoopBatchSize,
      ref int smallLoopBatchSize,
      int bigLoopMaxBatchSize,
      int bigLoopMinBatchSize,
      int smallLoopMaxBatchSize,
      int smallLoopMinBatchSize)
    {
      if (!batchSize.HasValue || batchSize.Value <= 0)
        return;
      bigLoopBatchSize = batchSize.Value;
      bigLoopBatchSize = Math.Min(bigLoopBatchSize, bigLoopMaxBatchSize);
      bigLoopBatchSize = Math.Max(bigLoopBatchSize, bigLoopMinBatchSize);
      smallLoopBatchSize = Math.Min(smallLoopBatchSize, smallLoopMaxBatchSize);
      smallLoopBatchSize = Math.Max(smallLoopBatchSize, smallLoopMinBatchSize);
      smallLoopBatchSize = Math.Min(smallLoopBatchSize, bigLoopBatchSize);
    }
  }
}
