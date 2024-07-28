// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase65
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase65 : TestManagementDatabase64
  {
    internal TestManagementDatabase65(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase65()
    {
    }

    internal override void MarkTestCaseRefsFlaky(
      Guid projectId,
      List<int> testRunIds,
      List<int> allowedPipelines)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.MarkTestCaseRefsFlaky");
        this.PrepareStoredProcedure("TestResult.prc_MarkTestCaseRefsFlaky");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindIdTypeTable("@testRunIds", (IEnumerable<int>) testRunIds);
        this.BindIdTypeTable("@allowedPipelines", (IEnumerable<int>) allowedPipelines);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.MarkTestCaseRefsFlaky");
      }
    }
  }
}
