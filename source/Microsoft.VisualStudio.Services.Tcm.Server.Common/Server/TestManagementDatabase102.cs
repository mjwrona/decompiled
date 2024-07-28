// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase102
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
  public class TestManagementDatabase102 : TestManagementDatabase101
  {
    internal TestManagementDatabase102(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase102()
    {
    }

    public override UpdatedRunProperties UpdateTestRunWithCustomFields(
      Guid projectId,
      TestRun run,
      Guid updatedBy,
      ReleaseReference releaseRef = null,
      BuildConfiguration buildRef = null,
      bool skipRunStateTransitionCheck = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_UpdateTestRunWithCustomFields");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", run.TestRunId);
      this.BindStringPreserveNull("@title", run.Title, 256, SqlDbType.NVarChar);
      this.BindGuidPreserveNull("@owner", run.Owner);
      this.BindByte("@state", run.State, (byte) 0);
      this.BindNullableDateTime("@dueDate", run.DueDate);
      this.BindNullableInt("@iterationId", run.IterationId, 0);
      this.BindStringPreserveNull("@controller", run.Controller, 256, SqlDbType.NVarChar);
      this.BindStringPreserveNull("@errorMessage", run.ErrorMessage, 4000, SqlDbType.NVarChar);
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
      List<BuildConfiguration> builds;
      if (buildRef == null)
      {
        builds = (List<BuildConfiguration>) null;
      }
      else
      {
        builds = new List<BuildConfiguration>();
        builds.Add(buildRef);
      }
      this.BindBuildRefTypeTable4("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
      List<ReleaseReference> releases;
      if (releaseRef == null)
      {
        releases = (List<ReleaseReference>) null;
      }
      else
      {
        releases = new List<ReleaseReference>();
        releases.Add(releaseRef);
      }
      this.BindReleaseRefTypeTable4("@releaseRefData", (IEnumerable<ReleaseReference>) releases);
      this.BindPipelineRefTable("@pipelineRefData", (IEnumerable<PipelineReference>) null);
      this.BindString("@sourceWorkflow", run.SourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindTestExtensionFieldValuesTypeTable("@additionalFields", run.CustomFields != null ? run.CustomFields.Select<TestExtensionField, Tuple<int, int, TestExtensionField>>((System.Func<TestExtensionField, Tuple<int, int, TestExtensionField>>) (f => new Tuple<int, int, TestExtensionField>(run.TestRunId, 0, f))) : (IEnumerable<Tuple<int, int, TestExtensionField>>) null);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedRunProperties updatedRunProperties = reader.Read() ? new TestManagementDatabase60.UpdatedPropertyColumns2().bindUpdatedRunProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestRunWithCustomFields");
      updatedRunProperties.LastUpdatedBy = updatedBy;
      if (updatedRunProperties.IsRunCompleted)
        run.CompleteDate = updatedRunProperties.CompleteDate;
      return updatedRunProperties;
    }

    public override List<TestCaseResult> FetchTestResultsByRunMRX(
      Guid projectId,
      int testRunId,
      List<int> resultIds,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResultsByRunMRX");
        this.PrepareStoredProcedure("TestResult.prc_FetchTestResultsByRunMRX");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindBoolean("@includeActionResults", includeActionResults);
        this.BindInt32TypeTable("@idsTable", (IEnumerable<int>) resultIds);
        this.BindInt("@maxCommentSize", TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext));
        List<TestCaseResult> results = new List<TestCaseResult>(resultIds.Count);
        Dictionary<int, TestCaseResult> resultsMap = new Dictionary<int, TestCaseResult>(resultIds.Count);
        TestManagementDatabase50.FetchTestResultsByRunColumns resultsByRunColumns = new TestManagementDatabase50.FetchTestResultsByRunColumns();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          int PlanId = 0;
          string empty = string.Empty;
          TestManagementDatabase50.TestRunMetadataForResultsColumns forResultsColumns = new TestManagementDatabase50.TestRunMetadataForResultsColumns();
          if (reader.Read())
            (PlanId, empty) = forResultsColumns.bind(reader);
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResultsByRunMRX");
          while (reader.Read())
          {
            TestCaseResult testCaseResult = resultsByRunColumns.bind(reader);
            testCaseResult.CustomFields = new List<TestExtensionField>();
            testCaseResult.TestPlanId = PlanId;
            testCaseResult.BuildNumber = empty;
            resultsMap.Add(testCaseResult.TestResultId, testCaseResult);
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResultsByRunMRX");
          TestManagementDatabase.FetchTestResultsExColumns resultsExColumns = new TestManagementDatabase.FetchTestResultsExColumns();
          while (reader.Read())
          {
            Tuple<int, int, TestExtensionField> tuple = resultsExColumns.bind(reader);
            int key = tuple.Item2;
            if (resultsMap.ContainsKey(key))
            {
              if (string.Equals("StackTrace", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].StackTrace = tuple.Item3;
              else if (string.Equals("FailingSince", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string jsonString)
                {
                  FailingSince convertedObject = (FailingSince) null;
                  if (TestResultHelper.TryJsonConvertWithRetry<FailingSince>(jsonString, out convertedObject, true))
                    resultsMap[key].FailingSince = convertedObject;
                }
              }
              else if (string.Equals("Comment", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].Comment = tuple.Item3.Value as string;
              else if (string.Equals("ErrorMessage", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].ErrorMessage = tuple.Item3.Value as string;
              else if (string.Equals("UnsanitizedTestCaseTitle", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string str1)
                  resultsMap[key].TestCaseTitle = str1;
              }
              else if (string.Equals("UnsanitizedAutomatedTestName", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string str2)
                  resultsMap[key].AutomatedTestName = str2;
              }
              else if (string.Equals("MaxReservedSubResultId", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].SubResultCount = (int) tuple.Item3.Value;
              else if (string.Equals("TestResultGroupType", tuple.Item3.Field.Name, StringComparison.OrdinalIgnoreCase))
                resultsMap[key].ResultGroupType = (ResultGroupType) TestManagementServiceUtility.ValidateAndGetEnumValue<ResultGroupType>((string) tuple.Item3.Value, ResultGroupType.None);
              else
                resultsMap[key].CustomFields.Add(tuple.Item3);
            }
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResultsByRunMRX");
          TestManagementDatabase102.FetchTestResultsByRunMRXColumns resultsByRunMrxColumns = new TestManagementDatabase102.FetchTestResultsByRunMRXColumns();
          while (reader.Read())
          {
            Tuple<int, int, TestCaseResult> tuple = resultsByRunMrxColumns.bind(reader);
            int key = tuple.Item2;
            if (resultsMap.ContainsKey(key))
            {
              resultsMap[key].ExecutionNumber = tuple.Item3.ExecutionNumber;
              resultsMap[key].Attempt = tuple.Item3.Attempt;
              resultsMap[key].Locale = tuple.Item3.Locale;
              resultsMap[key].BuildType = tuple.Item3.BuildType;
              resultsMap[key].TestPhase = tuple.Item3.TestPhase;
              resultsMap[key].TopologyId = tuple.Item3.TopologyId;
              resultsMap[key].IsSystemIssue = tuple.Item3.IsSystemIssue;
              resultsMap[key].ExceptionType = tuple.Item3.ExceptionType;
              resultsMap[key].BucketUid = tuple.Item3.BucketUid;
              resultsMap[key].BucketingSystem = tuple.Item3.BucketingSystem;
            }
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResultsByRunMRX");
          while (reader.Read())
          {
            Tuple<int, int, Link<ResultLinkType>> tuple = resultsByRunMrxColumns.bindLink(reader);
            int key = tuple.Item2;
            if (resultsMap.ContainsKey(key))
            {
              if (tuple.Item3 != null && resultsMap[key].Links == null)
                resultsMap[key].Links = new List<Link<ResultLinkType>>();
              resultsMap[key].Links.Add(tuple.Item3);
            }
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResultsByRunMRX");
          while (reader.Read())
          {
            Tuple<int, int, TestResultDimension> tuple = resultsByRunMrxColumns.bindDimension(reader);
            int key = tuple.Item2;
            if (resultsMap.ContainsKey(key))
            {
              if (tuple.Item3 != null && resultsMap[key].Dimensions == null)
                resultsMap[key].Dimensions = new List<TestResultDimension>();
              resultsMap[key].Dimensions.Add(tuple.Item3);
            }
          }
          if (includeActionResults)
          {
            actionResults = new List<TestActionResult>();
            parameters = new List<TestResultParameter>();
            attachments = new List<TestResultAttachment>();
            this.ReadTestActionResults(actionResults, parameters, attachments, reader, "TestResult.prc_FetchTestResultsByRunMRX");
          }
          else
          {
            actionResults = (List<TestActionResult>) null;
            parameters = (List<TestResultParameter>) null;
            attachments = (List<TestResultAttachment>) null;
          }
        }
        resultIds.ForEach((Action<int>) (r =>
        {
          if (!resultsMap.ContainsKey(r))
            return;
          results.Add(resultsMap[r]);
        }));
        return results;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResultsMRX");
      }
    }

    protected class FetchTestResultsByRunMRXColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));

      internal Tuple<int, int, TestCaseResult> bind(SqlDataReader reader)
      {
        int int32_1 = this.TestRunId.GetInt32((IDataReader) reader);
        int int32_2 = this.TestResultId.GetInt32((IDataReader) reader);
        TestCaseResult testCaseResult1 = new TestManagementDatabase102.FetchTestMRXColumns().bind(reader);
        int num = int32_2;
        TestCaseResult testCaseResult2 = testCaseResult1;
        return new Tuple<int, int, TestCaseResult>(int32_1, num, testCaseResult2);
      }

      internal Tuple<int, int, TestResultDimension> bindDimension(SqlDataReader reader)
      {
        int int32_1 = this.TestRunId.GetInt32((IDataReader) reader);
        int int32_2 = this.TestResultId.GetInt32((IDataReader) reader);
        TestResultDimension testResultDimension1 = new TestManagementDatabase102.FetchTestMRXColumns().bindDimension(reader);
        int num = int32_2;
        TestResultDimension testResultDimension2 = testResultDimension1;
        return new Tuple<int, int, TestResultDimension>(int32_1, num, testResultDimension2);
      }

      internal Tuple<int, int, Link<ResultLinkType>> bindLink(SqlDataReader reader)
      {
        int int32_1 = this.TestRunId.GetInt32((IDataReader) reader);
        int int32_2 = this.TestResultId.GetInt32((IDataReader) reader);
        Link<ResultLinkType> link1 = new TestManagementDatabase102.FetchTestMRXColumns().bindLink(reader);
        int num = int32_2;
        Link<ResultLinkType> link2 = link1;
        return new Tuple<int, int, Link<ResultLinkType>>(int32_1, num, link2);
      }
    }

    protected class FetchTestMRXColumns
    {
      private SqlColumnBinder ExecutionNumber = new SqlColumnBinder(nameof (ExecutionNumber));
      private SqlColumnBinder Attempt = new SqlColumnBinder(nameof (Attempt));
      private SqlColumnBinder Locale = new SqlColumnBinder(nameof (Locale));
      private SqlColumnBinder BuildType = new SqlColumnBinder(nameof (BuildType));
      private SqlColumnBinder TestPhase = new SqlColumnBinder(nameof (TestPhase));
      private SqlColumnBinder TopologyId = new SqlColumnBinder(nameof (TopologyId));
      private SqlColumnBinder IsSystemIssue = new SqlColumnBinder(nameof (IsSystemIssue));
      private SqlColumnBinder FailureBucketUId = new SqlColumnBinder("BucketUId");
      private SqlColumnBinder FailureBucketSystem = new SqlColumnBinder("BucketingSystem");
      private SqlColumnBinder ExceptionType = new SqlColumnBinder(nameof (ExceptionType));
      private SqlColumnBinder DimensionName = new SqlColumnBinder(nameof (DimensionName));
      private SqlColumnBinder DimensionValue = new SqlColumnBinder(nameof (DimensionValue));
      private SqlColumnBinder LinkParentType = new SqlColumnBinder("ParentType");
      private SqlColumnBinder LinkDisplayName = new SqlColumnBinder("DisplayName");
      private SqlColumnBinder LinkOperationType = new SqlColumnBinder("OperationType");
      private SqlColumnBinder LinkType = new SqlColumnBinder(nameof (LinkType));
      private SqlColumnBinder LinkUrl = new SqlColumnBinder("Url");

      internal TestCaseResult bind(SqlDataReader reader) => new TestCaseResult()
      {
        ExecutionNumber = this.ExecutionNumber.ColumnExists((IDataReader) reader) ? this.ExecutionNumber.GetInt32((IDataReader) reader) : 0,
        Attempt = this.Attempt.ColumnExists((IDataReader) reader) ? this.Attempt.GetInt32((IDataReader) reader) : 0,
        Locale = this.Locale.ColumnExists((IDataReader) reader) ? this.Locale.GetString((IDataReader) reader, true) : (string) null,
        BuildType = this.BuildType.ColumnExists((IDataReader) reader) ? this.BuildType.GetString((IDataReader) reader, true) : (string) null,
        TestPhase = this.TestPhase.ColumnExists((IDataReader) reader) ? this.TestPhase.GetByte((IDataReader) reader, (byte) 0) : (byte) 0,
        TopologyId = this.TopologyId.ColumnExists((IDataReader) reader) ? this.TopologyId.GetInt32((IDataReader) reader) : 0,
        IsSystemIssue = this.IsSystemIssue.ColumnExists((IDataReader) reader) && this.IsSystemIssue.GetBoolean((IDataReader) reader),
        ExceptionType = this.ExceptionType.ColumnExists((IDataReader) reader) ? this.ExceptionType.GetString((IDataReader) reader, true) : (string) null,
        BucketUid = this.FailureBucketUId.ColumnExists((IDataReader) reader) ? this.FailureBucketUId.GetString((IDataReader) reader, true) : (string) null,
        BucketingSystem = this.FailureBucketSystem.ColumnExists((IDataReader) reader) ? this.FailureBucketSystem.GetString((IDataReader) reader, true) : (string) null
      };

      internal TestResultDimension bindDimension(SqlDataReader reader)
      {
        TestResultDimension testResultDimension = new TestResultDimension();
        if (this.DimensionName.ColumnExists((IDataReader) reader) && this.DimensionValue.ColumnExists((IDataReader) reader))
        {
          testResultDimension.Name = this.DimensionName.GetString((IDataReader) reader, true);
          testResultDimension.Value = this.DimensionValue.GetString((IDataReader) reader, true);
        }
        return testResultDimension;
      }

      internal Link<ResultLinkType> bindLink(SqlDataReader reader) => new Link<ResultLinkType>()
      {
        Url = this.LinkUrl.GetString((IDataReader) reader, true),
        DisplayName = this.LinkDisplayName.GetString((IDataReader) reader, true),
        Type = (ResultLinkType) this.LinkType.GetByte((IDataReader) reader),
        OperationType = (LinkOperationType) this.LinkOperationType.GetByte((IDataReader) reader)
      };
    }
  }
}
