// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase45
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase45 : TestManagementDatabase44
  {
    public override void DeleteTestPlanData(Guid projectGuid, int testPlanId)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteTestPlanData");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectGuid));
      this.BindInt("@testPlanId", testPlanId);
      this.ExecuteNonQuery();
    }

    public override void DeleteTestPointData(Guid projectGuid, int testPlanId, List<int> pointIds)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteTestPointData");
      this.BindInt("@testPlanId", testPlanId);
      this.BindInt32TypeTable("@pointIds", (IEnumerable<int>) pointIds);
      this.ExecuteReader();
    }

    internal TestManagementDatabase45(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase45()
    {
    }
  }
}
