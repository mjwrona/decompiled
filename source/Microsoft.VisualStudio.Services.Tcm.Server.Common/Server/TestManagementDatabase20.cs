// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase20
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase20 : TestManagementDatabase19
  {
    internal TestManagementDatabase20(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase20()
    {
    }

    public override UpdatedRunProperties UpdateTestRun(
      Guid projectId,
      TestRun run,
      Guid updatedBy,
      ReleaseReference releaseRef = null,
      BuildConfiguration buildRef = null,
      bool skipRunStateTransitionCheck = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_UpdateTestRun");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", run.TestRunId);
      this.BindStringPreserveNull("@title", run.Title, 256, SqlDbType.NVarChar);
      this.BindGuidPreserveNull("@owner", run.Owner);
      this.BindByte("@state", run.State, (byte) 0);
      this.BindNullableDateTime("@dueDate", run.DueDate);
      this.BindNullableInt("@iterationId", run.IterationId, 0);
      this.BindStringPreserveNull("@controller", run.Controller, 256, SqlDbType.NVarChar);
      this.BindStringPreserveNull("@errorMessage", run.ErrorMessage, 512, SqlDbType.NVarChar);
      this.BindNullableDateTime("@dateStarted", run.StartDate);
      this.BindNullableDateTime("@dateCompleted", run.CompleteDate);
      this.BindInt("@testMessageLogId", run.TestMessageLogId);
      this.BindInt("@testSettingsId", run.TestSettingsId);
      this.BindInt("@publicTestSettingsId", run.PublicTestSettingsId);
      this.BindGuid("@testEnvironmentId", run.TestEnvironmentId);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindByte("@postProcessState", run.PostProcessState, (byte) 0);
      this.BindInt("@version", run.Version);
      this.BindInt("@revision", run.Revision);
      this.BindBoolean("@isBvt", run.IsBvt);
      this.BindStringPreserveNull("@comment", run.Comment, 1048576, SqlDbType.NVarChar);
      this.BindByte("@substate", run.Substate, (byte) 0);
      this.BindString("@testEnvironmentUrl", !run.RunHasDtlEnvironment || run.DtlTestEnvironment == null ? (string) null : run.DtlTestEnvironment.Url, 2048, true, SqlDbType.NVarChar);
      this.BindString("@autEnvironmentUrl", !run.RunHasDtlEnvironment || run.DtlAutEnvironment == null ? (string) null : run.DtlAutEnvironment.Url, 2048, true, SqlDbType.NVarChar);
      this.BindString("@dtlCsmParameters", !run.RunHasDtlEnvironment || run.CsmParameters == null ? (string) null : run.CsmParameters, 2048, true, SqlDbType.NVarChar);
      this.BindBoolean("@skipRunStateTransitionCheck", skipRunStateTransitionCheck);
      this.BindString("@dropLocation", run.DropLocation, 260, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedRunProperties updatedRunProperties = reader.Read() ? new TestManagementDatabase20.UpdatedPropertyColumns().bindUpdatedRunProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestRun");
      updatedRunProperties.LastUpdatedBy = updatedBy;
      if (updatedRunProperties.IsRunCompleted)
        run.CompleteDate = updatedRunProperties.CompleteDate;
      return updatedRunProperties;
    }

    private new class UpdatedPropertyColumns
    {
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      internal SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      internal SqlColumnBinder IsRunStarted = new SqlColumnBinder(nameof (IsRunStarted));
      internal SqlColumnBinder IsRunCompleted = new SqlColumnBinder(nameof (IsRunCompleted));
      internal SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));

      internal UpdatedRunProperties bindUpdatedRunProperties(SqlDataReader reader)
      {
        UpdatedRunProperties updatedRunProperties = new UpdatedRunProperties();
        updatedRunProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        updatedRunProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        updatedRunProperties.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        updatedRunProperties.IsRunStarted = this.IsRunStarted.GetBoolean((IDataReader) reader);
        updatedRunProperties.IsRunCompleted = this.IsRunCompleted.GetBoolean((IDataReader) reader);
        updatedRunProperties.CompleteDate = this.CompleteDate.GetDateTime((IDataReader) reader);
        return updatedRunProperties;
      }
    }
  }
}
