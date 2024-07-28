// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase99
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase99 : TestManagementDatabase98
  {
    internal TestManagementDatabase99(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase99()
    {
    }

    public override int TcmAdhocCleanupJob(
      int batchSize,
      int minTestRunId,
      int maxTestRunId,
      int runLimit)
    {
      this.PrepareStoredProcedure("prc_DeleteAdhocTestRuns");
      this.BindInt("@batchSize", batchSize);
      this.BindInt("@minTestRunId", minTestRunId);
      this.BindInt("@maxTestRunId", maxTestRunId);
      this.BindInt("@runLimit", runLimit);
      return (int) this.ExecuteScalar();
    }
  }
}
