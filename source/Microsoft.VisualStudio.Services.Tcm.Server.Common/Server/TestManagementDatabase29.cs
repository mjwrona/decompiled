// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase29
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase29 : TestManagementDatabase28
  {
    internal override Dictionary<Guid, List<TestNameRequirementAssociation>> QueryNonMigratedTestToRequirementLinks(
      int batchSize)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryNonMigratedTestToRequirementLinks");
      this.BindInt("@batchSize", batchSize);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<Guid, List<TestNameRequirementAssociation>> requirementLinks = new Dictionary<Guid, List<TestNameRequirementAssociation>>();
      TestManagementDatabase29.FetchNonMigratedTestNameRequirementAssociation requirementAssociation1 = new TestManagementDatabase29.FetchNonMigratedTestNameRequirementAssociation();
      while (reader.Read())
      {
        int dataspaceId = 0;
        TestNameRequirementAssociation requirementAssociation2 = requirementAssociation1.bind(reader, out dataspaceId);
        try
        {
          Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
          if (!requirementLinks.ContainsKey(dataspaceIdentifier))
            requirementLinks[dataspaceIdentifier] = new List<TestNameRequirementAssociation>();
          requirementLinks[dataspaceIdentifier].Add(requirementAssociation2);
        }
        catch (Exception ex)
        {
          this.RequestContext.Trace(0, TraceLevel.Warning, "TestManagement", "Database", string.Format("Project with dataspaceId: {0} not found.", (object) dataspaceId));
        }
      }
      return requirementLinks;
    }

    internal override void UpdateTestToRequirementLinkMigrationStatus(
      Guid projectGuid,
      List<TestNameRequirementAssociation> associations)
    {
      this.PrepareStoredProcedure("TestResult.prc_UpdateTestToRequirementLinkMigrationStatus");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindIdPairTypeTable("@associations", associations.Select<TestNameRequirementAssociation, KeyValuePair<int, int>>((System.Func<TestNameRequirementAssociation, KeyValuePair<int, int>>) (a => new KeyValuePair<int, int>(a.WorkItemId, a.TestMetadataId))));
      this.ExecuteNonQuery();
    }

    internal TestManagementDatabase29(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase29()
    {
    }

    public override List<TestRun> QueryTestRuns3(
      Guid projectId,
      int testRunId,
      Guid owner,
      string buildUri,
      int planId,
      int skip,
      int top,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns3");
        Dictionary<int, TestRun> dictionary = new Dictionary<int, TestRun>();
        this.PrepareStoredProcedure("TestResult.prc_QueryTestRuns3");
        this.BindNullableInt("@testRunId", testRunId, 0);
        this.BindNullableInt("@planId", planId, -1);
        this.BindGuidPreserveNull("@owner", owner);
        this.BindString("@buildUri", buildUri, 256, true, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@skip", skip);
        this.BindInt("@top", top);
        TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
        {
          TestRun testRun = queryTestRunColumns.bind(reader, out int _, out string _);
          dictionary.Add(testRun.TestRunId, testRun);
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("TestResult.prc_QueryTestRuns3");
        TestManagementDatabase.FetchTestRunsExColumns testRunsExColumns = new TestManagementDatabase.FetchTestRunsExColumns();
        while (reader.Read())
        {
          Tuple<int, TestExtensionField> tuple = testRunsExColumns.bind(reader);
          if (dictionary.ContainsKey(tuple.Item1))
          {
            TestRun testRun = dictionary[tuple.Item1];
            testRun.CustomFields = testRun.CustomFields ?? new List<TestExtensionField>();
            testRun.CustomFields.Add(tuple.Item2);
          }
        }
        return dictionary.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns3");
      }
    }

    private class FetchNonMigratedTestNameRequirementAssociation
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));
      private SqlColumnBinder TestMetadataId = new SqlColumnBinder(nameof (TestMetadataId));
      private SqlColumnBinder TestName = new SqlColumnBinder(nameof (TestName));
      private SqlColumnBinder Container = new SqlColumnBinder(nameof (Container));

      internal TestNameRequirementAssociation bind(SqlDataReader reader, out int dataspaceId)
      {
        TestNameRequirementAssociation requirementAssociation = new TestNameRequirementAssociation();
        requirementAssociation.WorkItemId = this.WorkItemId.GetInt32((IDataReader) reader);
        requirementAssociation.TestMetadataId = this.TestMetadataId.GetInt32((IDataReader) reader);
        requirementAssociation.TestName = this.TestName.GetString((IDataReader) reader, false);
        requirementAssociation.Container = this.Container.GetString((IDataReader) reader, false);
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        return requirementAssociation;
      }
    }

    protected class QueryTestRunColumnsByFilters
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder RepoId = new SqlColumnBinder(nameof (RepoId));
      private SqlColumnBinder StartDate = new SqlColumnBinder(nameof (StartDate));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      private SqlColumnBinder TestPlanId = new SqlColumnBinder(nameof (TestPlanId));
      private SqlColumnBinder Guid = new SqlColumnBinder(nameof (Guid));
      private SqlColumnBinder PublicTestSettingsId = new SqlColumnBinder(nameof (PublicTestSettingsId));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder IsAutomated = new SqlColumnBinder(nameof (IsAutomated));
      private SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      private SqlColumnBinder IncompleteTests = new SqlColumnBinder(nameof (IncompleteTests));
      private SqlColumnBinder NotApplicableTests = new SqlColumnBinder(nameof (NotApplicableTests));
      private SqlColumnBinder PassedTests = new SqlColumnBinder(nameof (PassedTests));
      private SqlColumnBinder UnanalyzedTests = new SqlColumnBinder(nameof (UnanalyzedTests));
      private SqlColumnBinder SourceWorkflow = new SqlColumnBinder(nameof (SourceWorkflow));
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder ReleaseDefId = new SqlColumnBinder(nameof (ReleaseDefId));
      private SqlColumnBinder ReleaseEnvDefId = new SqlColumnBinder(nameof (ReleaseEnvDefId));
      private SqlColumnBinder ReleaseRefId = new SqlColumnBinder(nameof (ReleaseRefId));
      private SqlColumnBinder Attempt = new SqlColumnBinder(nameof (Attempt));
      private SqlColumnBinder StageName = new SqlColumnBinder(nameof (StageName));
      private SqlColumnBinder StageAttempt = new SqlColumnBinder(nameof (StageAttempt));
      private SqlColumnBinder PhaseName = new SqlColumnBinder(nameof (PhaseName));
      private SqlColumnBinder PhaseAttempt = new SqlColumnBinder(nameof (PhaseAttempt));
      private SqlColumnBinder JobName = new SqlColumnBinder(nameof (JobName));
      private SqlColumnBinder JobAttempt = new SqlColumnBinder(nameof (JobAttempt));

      internal TestRun bind(SqlDataReader reader)
      {
        TestRun testRun = new TestRun();
        testRun.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRun.Title = this.Title.GetString((IDataReader) reader, false);
        testRun.CreationDate = this.CreationDate.ColumnExists((IDataReader) reader) ? this.CreationDate.GetDateTime((IDataReader) reader) : new DateTime();
        testRun.LastUpdated = this.LastUpdated.ColumnExists((IDataReader) reader) ? this.LastUpdated.GetDateTime((IDataReader) reader) : new DateTime();
        testRun.Owner = this.Owner.ColumnExists((IDataReader) reader) ? this.Owner.GetGuid((IDataReader) reader, false) : new Guid();
        testRun.State = this.State.ColumnExists((IDataReader) reader) ? this.State.GetByte((IDataReader) reader) : (byte) 0;
        testRun.BuildNumber = this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : (string) null;
        testRun.BuildPlatform = this.BuildPlatform.ColumnExists((IDataReader) reader) ? this.BuildPlatform.GetString((IDataReader) reader, true) : (string) null;
        testRun.BuildFlavor = this.BuildFlavor.ColumnExists((IDataReader) reader) ? this.BuildFlavor.GetString((IDataReader) reader, true) : (string) null;
        testRun.StartDate = this.StartDate.ColumnExists((IDataReader) reader) ? this.StartDate.GetDateTime((IDataReader) reader) : new DateTime();
        testRun.CompleteDate = this.CompleteDate.ColumnExists((IDataReader) reader) ? this.CompleteDate.GetDateTime((IDataReader) reader) : new DateTime();
        testRun.TestPlanId = this.TestPlanId.ColumnExists((IDataReader) reader) ? this.TestPlanId.GetInt32((IDataReader) reader) : 0;
        testRun.PublicTestSettingsId = this.PublicTestSettingsId.ColumnExists((IDataReader) reader) ? this.PublicTestSettingsId.GetInt32((IDataReader) reader) : 0;
        testRun.LastUpdatedBy = this.LastUpdatedBy.ColumnExists((IDataReader) reader) ? this.LastUpdatedBy.GetGuid((IDataReader) reader, false) : new Guid();
        testRun.IsAutomated = this.IsAutomated.ColumnExists((IDataReader) reader) && this.IsAutomated.GetBoolean((IDataReader) reader);
        testRun.TotalTests = this.TotalTests.ColumnExists((IDataReader) reader) ? this.TotalTests.GetInt32((IDataReader) reader) : 0;
        testRun.IncompleteTests = this.IncompleteTests.ColumnExists((IDataReader) reader) ? this.IncompleteTests.GetInt32((IDataReader) reader) : 0;
        testRun.NotApplicableTests = this.NotApplicableTests.ColumnExists((IDataReader) reader) ? this.NotApplicableTests.GetInt32((IDataReader) reader) : 0;
        testRun.PassedTests = this.PassedTests.ColumnExists((IDataReader) reader) ? this.PassedTests.GetInt32((IDataReader) reader) : 0;
        testRun.UnanalyzedTests = this.UnanalyzedTests.ColumnExists((IDataReader) reader) ? this.UnanalyzedTests.GetInt32((IDataReader) reader) : 0;
        testRun.SourceWorkflow = this.SourceWorkflow.ColumnExists((IDataReader) reader) ? this.SourceWorkflow.GetString((IDataReader) reader, false) : (string) null;
        testRun.ReleaseReference = new ReleaseReference()
        {
          ReleaseId = this.ReleaseId.ColumnExists((IDataReader) reader) ? this.ReleaseId.GetInt32((IDataReader) reader, 0) : 0,
          ReleaseEnvId = this.ReleaseEnvId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvId.GetInt32((IDataReader) reader, 0) : 0,
          ReleaseDefId = this.ReleaseDefId.ColumnExists((IDataReader) reader) ? this.ReleaseDefId.GetInt32((IDataReader) reader, 0) : 0,
          ReleaseEnvDefId = this.ReleaseEnvDefId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvDefId.GetInt32((IDataReader) reader, 0) : 0,
          ReleaseRefId = this.ReleaseRefId.ColumnExists((IDataReader) reader) ? this.ReleaseRefId.GetInt32((IDataReader) reader, 0) : 0,
          Attempt = this.Attempt.ColumnExists((IDataReader) reader) ? this.Attempt.GetInt32((IDataReader) reader, 0) : 0
        };
        testRun.BuildReference = new BuildConfiguration()
        {
          BuildId = this.BuildId.ColumnExists((IDataReader) reader) ? this.BuildId.GetInt32((IDataReader) reader, 0) : 0,
          BuildNumber = this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : (string) null,
          BuildPlatform = this.BuildPlatform.ColumnExists((IDataReader) reader) ? this.BuildPlatform.GetString((IDataReader) reader, true) : (string) null,
          BuildFlavor = this.BuildFlavor.ColumnExists((IDataReader) reader) ? this.BuildFlavor.GetString((IDataReader) reader, true) : (string) null,
          BuildDefinitionId = this.BuildDefinitionId.ColumnExists((IDataReader) reader) ? this.BuildDefinitionId.GetInt32((IDataReader) reader, 0) : 0,
          BranchName = this.BranchName.ColumnExists((IDataReader) reader) ? this.BranchName.GetString((IDataReader) reader, true) : (string) null,
          BuildConfigurationId = this.BuildConfigurationId.ColumnExists((IDataReader) reader) ? this.BuildConfigurationId.GetInt32((IDataReader) reader, 0) : 0,
          RepositoryId = this.RepoId.ColumnExists((IDataReader) reader) ? this.RepoId.GetString((IDataReader) reader, true) : (string) null
        };
        if (testRun.BuildReference.BuildId > 0)
        {
          testRun.PipelineReference = new PipelineReference()
          {
            PipelineId = testRun.BuildReference.BuildId,
            PipelineDefinitionId = testRun.BuildReference.BuildDefinitionId
          };
          if (this.StageName.ColumnExists((IDataReader) reader))
            testRun.PipelineReference.StageReference = new StageReference()
            {
              StageName = this.StageName.GetString((IDataReader) reader, true),
              Attempt = this.StageAttempt.GetInt32((IDataReader) reader, 0, 0)
            };
          if (this.PhaseName.ColumnExists((IDataReader) reader))
            testRun.PipelineReference.PhaseReference = new PhaseReference()
            {
              PhaseName = this.PhaseName.GetString((IDataReader) reader, true),
              Attempt = this.PhaseAttempt.GetInt32((IDataReader) reader, 0, 0)
            };
          if (this.JobName.ColumnExists((IDataReader) reader))
            testRun.PipelineReference.JobReference = new JobReference()
            {
              JobName = this.JobName.GetString((IDataReader) reader, true),
              Attempt = this.JobAttempt.GetInt32((IDataReader) reader, 0, 0)
            };
        }
        return testRun;
      }
    }
  }
}
