// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase18
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase18 : TestManagementDatabase17
  {
    internal TestManagementDatabase18(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase18()
    {
    }

    public override void UpdateTestResultsExtension(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results,
      Guid updatedBy)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestResultDatabase.UpdateTestResultsExtension"))
      {
        try
        {
          this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.UpdateTestResultsExtension");
          this.PrepareStoredProcedure("TestResult.prc_UpdateTestResultsExt");
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindTestExtensionFieldValuesTypeTable("@additionalResultFields", (IEnumerable<Tuple<int, int, TestExtensionField>>) this.GetExtensionFieldsMap(testRunId, results, false));
          this.ExecuteNonQuery();
        }
        finally
        {
          this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.UpdateTestResultsExtension");
        }
      }
    }
  }
}
