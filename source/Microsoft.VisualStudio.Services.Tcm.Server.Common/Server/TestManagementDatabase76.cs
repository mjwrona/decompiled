// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase76
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase76 : TestManagementDatabase75
  {
    internal TestManagementDatabase76(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase76()
    {
    }

    internal override List<TestResultRecord> QueryTestResultsByTestRunChangedDate(
      int dataspaceId,
      int runBatchSize,
      int resultBatchSize,
      string prBranchName,
      TestResultWatermark fromWatermark,
      out TestResultWatermark toWatermark,
      TestArtifactSource dataSource,
      bool includeFlakyData = false)
    {
      return this.QueryTestResultsByTestRunChangedDateInternal(dataspaceId, runBatchSize, resultBatchSize, prBranchName, fromWatermark, out toWatermark, dataSource, includeFlakyData);
    }
  }
}
