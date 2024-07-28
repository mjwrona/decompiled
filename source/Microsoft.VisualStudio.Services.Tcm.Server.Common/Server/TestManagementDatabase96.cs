// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase96
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase96 : TestManagementDatabase95
  {
    internal TestManagementDatabase96(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase96()
    {
    }

    public override TestRun CreateTestRun(
      Guid projectId,
      TestRun testRun,
      Guid updatedBy,
      bool changeCounterInterval = false,
      bool isTcmService = false,
      bool reuseDeletedTestRunId = false,
      int reuseTestRunIdDays = 2)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CreateTestRun");
        this.BindString("@title", testRun.Title, 256, false, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindGuid("@owner", testRun.Owner);
        this.BindByte("@state", testRun.State);
        this.BindString("@dropLocation", testRun.DropLocation, 260, true, SqlDbType.NVarChar);
        this.BindInt("@testPlanId", testRun.TestPlanId);
        this.BindNullableDateTime("@dueDate", testRun.DueDate);
        this.BindInt("@iterationId", testRun.IterationId);
        this.BindString("@controller", testRun.Controller, 256, true, SqlDbType.NVarChar);
        this.BindInt("@testMessageLogId", testRun.TestMessageLogId);
        this.BindInt("@testSettingsId", testRun.TestSettingsId);
        this.BindInt("@publicTestSettingsId", testRun.PublicTestSettingsId);
        this.BindGuid("@testEnvironmentId", testRun.TestEnvironmentId);
        this.BindString("@legacySharePath", testRun.LegacySharePath, 1024, false, SqlDbType.NVarChar);
        this.BindBoolean("@isAutomated", testRun.IsAutomated);
        this.BindByte("@type", testRun.Type);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindInt("@version", testRun.Version);
        List<BuildConfiguration> builds;
        if (testRun.BuildReference == null)
        {
          builds = (List<BuildConfiguration>) null;
        }
        else
        {
          builds = new List<BuildConfiguration>();
          builds.Add(testRun.BuildReference);
        }
        this.BindBuildRefTypeTable4("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
        List<ReleaseReference> releases;
        if (testRun.ReleaseReference == null)
        {
          releases = (List<ReleaseReference>) null;
        }
        else
        {
          releases = new List<ReleaseReference>();
          releases.Add(testRun.ReleaseReference);
        }
        this.BindReleaseRefTypeTable4("@releaseRefData", (IEnumerable<ReleaseReference>) releases);
        List<PipelineReference> pipelineReferenceList;
        if (testRun.PipelineReference == null)
        {
          pipelineReferenceList = (List<PipelineReference>) null;
        }
        else
        {
          pipelineReferenceList = new List<PipelineReference>();
          pipelineReferenceList.Add(testRun.PipelineReference);
        }
        this.BindPipelineRefTable("@pipelineRefData", (IEnumerable<PipelineReference>) pipelineReferenceList);
        this.BindTestExtensionFieldValuesTypeTable("@additionalFields", testRun.CustomFields != null ? testRun.CustomFields.Select<TestExtensionField, Tuple<int, int, TestExtensionField>>((System.Func<TestExtensionField, Tuple<int, int, TestExtensionField>>) (f => new Tuple<int, int, TestExtensionField>(testRun.TestRunId, 0, f))) : (IEnumerable<Tuple<int, int, TestExtensionField>>) null);
        this.BindString("@sourceWorkflow", testRun.SourceWorkflow, 128, false, SqlDbType.NVarChar);
        this.BindBoolean("@isBvt", testRun.IsBvt);
        this.BindString("@testEnvironmentUrl", !testRun.RunHasDtlEnvironment || testRun.DtlTestEnvironment == null ? (string) null : testRun.DtlTestEnvironment.Url, 2048, true, SqlDbType.NVarChar);
        this.BindString("@autEnvironmentUrl", !testRun.RunHasDtlEnvironment || testRun.DtlAutEnvironment == null ? (string) null : testRun.DtlAutEnvironment.Url, 2048, true, SqlDbType.NVarChar);
        this.BindString("@sourceFilter", !testRun.RunHasDtlEnvironment || !testRun.IsAutomated ? (string) null : testRun.Filter.SourceFilter, 1024, true, SqlDbType.NVarChar);
        this.BindString("@TestCaseFilter", !testRun.RunHasDtlEnvironment || !testRun.IsAutomated ? (string) null : testRun.Filter.TestCaseFilter, 2048, true, SqlDbType.NVarChar);
        this.BindBoolean("@changeCounterInterval", changeCounterInterval);
        this.BindBoolean("@isTcmService", isTcmService);
        this.BindBoolean("@reuseDeletedTestRunId", reuseDeletedTestRunId);
        this.BindInt(nameof (reuseTestRunIdDays), reuseTestRunIdDays);
        SqlDataReader reader = this.ExecuteReader();
        return reader.Read() ? new TestManagementDatabase13.CreateTestRunColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_CreateTestRun");
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }
  }
}
