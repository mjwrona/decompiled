// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase55
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase55 : TestManagementDatabase54
  {
    public override void UpdateBuildCoverage(
      int buildConfigurationId,
      Coverage coverage,
      int coverageChangeId,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_UpdateBuildCoverage");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@buildConfigurationId", buildConfigurationId);
      this.BindByte("@state", coverage.State);
      this.BindInt("@coverageChangeId", coverageChangeId);
      this.BindStringPreserveNull("@errorLog", coverage.LastError, int.MaxValue, SqlDbType.NVarChar);
      this.BindModuleCoverageTypeTable1("@moduleInfo", (IEnumerable<ModuleCoverage>) coverage.Modules);
      this.BindFunctionCoverageTypeTable("@functionInfo", (IEnumerable<FunctionCoverage>) this.GetFunctionCoverages(coverage));
      this.ExecuteNonQuery();
    }

    internal override void UpdateTestRunCoverage(int testRunId, Coverage coverage, Guid projectId)
    {
      this.PrepareStoredProcedure("prc_UpdateTestRunCoverage");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindByte("@state", coverage.State);
      this.BindStringPreserveNull("@errorLog", coverage.LastError, int.MaxValue, SqlDbType.NVarChar);
      this.BindModuleCoverageTypeTable1("@moduleInfo", (IEnumerable<ModuleCoverage>) coverage.Modules);
      this.BindFunctionCoverageTypeTable("@functionInfo", (IEnumerable<FunctionCoverage>) this.GetFunctionCoverages(coverage));
      this.ExecuteNonQuery();
    }

    public override List<PointLastResult> FilterPointsOnOutcome(
      Guid projectGuid,
      int planId,
      List<int> pointIds,
      List<byte> pointOutcomes,
      List<byte> resultStates)
    {
      this.PrepareStoredProcedure("TestResult.prc_FilterPointOutcome");
      this.BindInt("@planId", planId);
      this.BindIdTypeTable("@pointIds", (IEnumerable<int>) pointIds);
      this.BindTestManagement_TinyIntTypeTable("@pointOutcomes", (IEnumerable<byte>) pointOutcomes);
      this.BindTestManagement_TinyIntTypeTable("@resultStates", (IEnumerable<byte>) resultStates);
      List<PointLastResult> pointLastResultList = new List<PointLastResult>();
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("PointId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("LastUpdated");
      while (reader.Read())
        pointLastResultList.Add(new PointLastResult()
        {
          PointId = sqlColumnBinder1.GetInt32((IDataReader) reader),
          LastUpdatedDate = sqlColumnBinder2.GetDateTime((IDataReader) reader)
        });
      return pointLastResultList;
    }

    internal TestManagementDatabase55(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase55()
    {
    }
  }
}
