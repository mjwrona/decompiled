// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase24
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase24 : TestPlanningDatabase23
  {
    internal TestPlanningDatabase24(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase24()
    {
    }

    internal override BlockedPointProperties BlockTestPoint(
      Guid projectGuid,
      TestPoint point,
      TestCaseResult result,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("TestManagement.prc_BlockTestPoint");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@pointId", point.PointId);
      this.BindInt("@planId", point.PlanId);
      this.BindInt("@testRunId", result.TestRunId);
      this.BindInt("@testCaseId", result.TestCaseId);
      this.BindByte("@priority", result.Priority);
      this.BindString("@testCaseTitle", result.TestCaseTitle, 256, true, SqlDbType.NVarChar);
      this.BindString("@testCaseArea", result.TestCaseAreaUri, 256, true, SqlDbType.NVarChar);
      this.BindInt("@testCaseRevision", result.TestCaseRevision);
      this.BindString("@automatedTestName", result.AutomatedTestName, 256, false, SqlDbType.NVarChar);
      this.BindString("@automatedTestStorage", result.AutomatedTestStorage, 256, false, SqlDbType.NVarChar);
      this.BindString("@automatedTestType", result.AutomatedTestType, 256, false, SqlDbType.NVarChar);
      this.BindString("@automatedTestId", result.AutomatedTestId, 64, false, SqlDbType.NVarChar);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindInt("@revision", point.Revision);
      this.BindBinary("@automatedTestNameHash", this.GetSHA256Hash(result.AutomatedTestName ?? string.Empty), 32, SqlDbType.VarBinary);
      this.BindBinary("@automatedTestStorageHash", this.GetSHA256Hash(result.AutomatedTestStorage ?? string.Empty), 32, SqlDbType.VarBinary);
      SqlDataReader reader = this.ExecuteReader();
      BlockedPointProperties blockedPointProperties = reader.Read() ? new TestPlanningDatabase.UpdatedPropertyColumns().bindBlockedTestPointProperties(reader) : throw new UnexpectedDatabaseResultException("prc_BlockTestPoint");
      blockedPointProperties.LastTestRunId = result.TestRunId;
      blockedPointProperties.LastUpdatedBy = updatedBy;
      return blockedPointProperties;
    }
  }
}
