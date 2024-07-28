// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase85
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase85 : TestManagementDatabase84
  {
    internal TestManagementDatabase85(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase85()
    {
    }

    public override int DeleteStaleTestCaseReference(
      int batchSize,
      int staleTestCaseRefSprocExecTimeOutInSec)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.DeleteStaleTestCaseReference");
        this.PrepareStoredProcedure("TestResult.prc_DeleteStaleTestCaseReference", staleTestCaseRefSprocExecTimeOutInSec);
        this.BindInt("@batchSize", batchSize);
        return (int) this.ExecuteScalar();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.DeleteStaleTestCaseReference");
      }
    }
  }
}
