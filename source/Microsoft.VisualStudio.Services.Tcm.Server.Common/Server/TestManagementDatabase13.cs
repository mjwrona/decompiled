// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase13
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase13 : TestManagementDatabase12
  {
    public override void AddRequirementToTestLinks(
      GuidAndString projectId,
      int workItemId,
      List<TestMethod> testMethods,
      Guid createdBy)
    {
      this.PrepareStoredProcedure("TestResult.prc_AddRequirementToTestLinks");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId.GuidId));
      this.BindInt("@workItemId", workItemId);
      this.BindGuid("@createdBy", createdBy);
      this.BindTestMethodTypeTable("@testMethods", (IEnumerable<TestMethod>) testMethods);
      this.ExecuteReader();
    }

    public override List<int> QueryLinkedRequirementsForTest(Guid projectId, TestMethod testMethod)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryLinkedRequirementsForTest");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@testName", testMethod.Name, 1024, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      List<int> intList = new List<int>();
      TestManagementDatabase13.FetchLinkedRequirementsColumns requirementsColumns = new TestManagementDatabase13.FetchLinkedRequirementsColumns();
      while (reader.Read())
        intList.Add(requirementsColumns.bind(reader));
      return intList;
    }

    public override void DeleteRequirementToTestLink(
      GuidAndString projectId,
      int workItemId,
      TestMethod testMethod,
      Guid deletedBy)
    {
      this.PrepareStoredProcedure("TestResult.prc_DeleteRequirementToTestLinks");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId.GuidId));
      this.BindInt("@workItemId", workItemId);
      this.BindString("@testName", testMethod != null ? testMethod.Name : string.Empty, 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@deletedBy", deletedBy);
      this.ExecuteReader();
    }

    internal TestManagementDatabase13(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase13()
    {
    }

    protected override void BindQueryTestResultTrendParams(Guid projectId, ResultsFilter filter)
    {
      base.BindQueryTestResultTrendParams(projectId, filter);
      string parameterValue = (string) null;
      switch (filter.TestResultsContext.ContextType)
      {
        case TestResultsContextType.Build:
          parameterValue = filter.TestResultsContext.Build.BranchName;
          break;
        case TestResultsContextType.Release:
          parameterValue = filter.Branch;
          break;
      }
      this.BindString("@branchName", parameterValue, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
    }

    public override TestResultHistory QueryTestCaseResultHistory(
      Guid projectId,
      ResultsFilter filter,
      bool isTfvcBranchFilteringEnabled)
    {
      string groupBy = filter.GroupBy;
      int definitionId = 0;
      string definitionFilter = this.GetDefinitionFilter(filter, out definitionId);
      string format = string.Empty;
      if (groupBy.Equals("Branch", StringComparison.OrdinalIgnoreCase))
        format = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestCaseResultHistory;
      else if (groupBy.Equals("Environment", StringComparison.OrdinalIgnoreCase))
        format = TestManagementDynamicSqlBatchStatements.dynprc_QueryTestCaseResultHistoryForEnvironment;
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) definitionFilter, (object) TestManagementDynamicSqlBatchStatements.idynprc_FilterRunContextIdsAndQueryFailingSince);
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement, 0, true, true);
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@automatedTestName", filter.AutomatedTestName, 256, false, SqlDbType.NVarChar);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      if (groupBy.Equals("Branch", StringComparison.OrdinalIgnoreCase))
        this.BindInt("@buildDays", 15);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindInt("@definitionId", definitionId);
      this.BindString("@branchName", filter.Branch, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      TestResultHistory testResultHistory = new TestResultHistory();
      testResultHistory.GroupByField = groupBy;
      Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> dictionary = new Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      Dictionary<string, TestResultHistoryDetailsForGroup> resultsMap = new Dictionary<string, TestResultHistoryDetailsForGroup>();
      Dictionary<TestCaseResultIdentifier, FailingSince> failingSinceMap = new Dictionary<TestCaseResultIdentifier, FailingSince>();
      TestManagementDatabase13.QueryTestCaseResultHistoryColumns resultHistoryColumns1 = new TestManagementDatabase13.QueryTestCaseResultHistoryColumns();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultHistoryColumns1.bindGroupedResultHistory(reader, resultsMap, groupBy);
        if (reader.NextResult())
        {
          TestManagementDatabase13.QueryTestCaseResultHistoryColumns resultHistoryColumns2 = new TestManagementDatabase13.QueryTestCaseResultHistoryColumns();
          while (reader.Read())
            resultHistoryColumns2.bindFailingSinceColumn(reader, failingSinceMap);
        }
      }
      foreach (TestResultHistoryDetailsForGroup historyDetailsForGroup in resultsMap.Values)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult latestResult = historyDetailsForGroup.LatestResult;
        int result;
        if (int.TryParse(latestResult.TestRun.Id, out result))
        {
          TestCaseResultIdentifier key = new TestCaseResultIdentifier(result, latestResult.Id);
          historyDetailsForGroup.LatestResult.FailingSince = failingSinceMap.ContainsKey(key) ? failingSinceMap[key] : (FailingSince) null;
        }
      }
      testResultHistory.ResultsForGroup = (IList<TestResultHistoryDetailsForGroup>) resultsMap.Values.ToList<TestResultHistoryDetailsForGroup>();
      return testResultHistory;
    }

    protected string GetDefinitionFilter(ResultsFilter filter, out int definitionId)
    {
      string definitionFilter = string.Empty;
      definitionId = 0;
      if (filter.TestResultsContext != null)
      {
        if (filter.TestResultsContext.ContextType == TestResultsContextType.Build)
        {
          definitionId = filter.TestResultsContext.Build.DefinitionId;
          definitionFilter = " " + SQLConstants.Operator_AND + " " + "buildConfiguration.BuildDefinitionId" + SQLConstants.Operator_EqualTo + "@definitionId" + " ";
        }
        else if (filter.TestResultsContext.ContextType == TestResultsContextType.Release)
        {
          definitionId = filter.TestResultsContext.Release.DefinitionId;
          definitionFilter = " " + SQLConstants.Operator_AND + " " + "releaseRef.ReleaseDefId" + SQLConstants.Operator_EqualTo + "@definitionId" + " ";
        }
      }
      return definitionFilter;
    }

    protected class FetchLinkedRequirementsColumns
    {
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));

      internal int bind(SqlDataReader reader) => this.WorkItemId.GetInt32((IDataReader) reader);
    }

    protected new class CreateTestRunColumns
    {
      internal SqlColumnBinder testRunId = new SqlColumnBinder("TestRunId");
      internal SqlColumnBinder revision = new SqlColumnBinder("Revision");

      internal TestRun bind(SqlDataReader reader)
      {
        TestRun testRun = new TestRun();
        testRun.TestRunId = this.testRunId.GetInt32((IDataReader) reader);
        testRun.Revision = this.revision.GetInt32((IDataReader) reader);
        return testRun;
      }
    }

    protected class QueryTestCaseResultHistoryColumns
    {
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder ReleaseEnvDefId = new SqlColumnBinder(nameof (ReleaseEnvDefId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder DateCompleted = new SqlColumnBinder(nameof (DateCompleted));
      private SqlColumnBinder DateStarted = new SqlColumnBinder(nameof (DateStarted));
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));
      private SqlColumnBinder FailingSince = new SqlColumnBinder(nameof (FailingSince));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));

      internal void bindGroupedResultHistory(
        SqlDataReader reader,
        Dictionary<string, TestResultHistoryDetailsForGroup> resultsMap,
        string groupByFieldString)
      {
        string str1 = this.BranchName.ColumnExists((IDataReader) reader) ? this.BranchName.GetString((IDataReader) reader, true) : string.Empty;
        int int32_1 = this.ReleaseEnvDefId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvDefId.GetInt32((IDataReader) reader) : 0;
        int int32_2 = this.TestRunId.ColumnExists((IDataReader) reader) ? this.TestRunId.GetInt32((IDataReader) reader) : 0;
        int int32_3 = this.TestResultId.ColumnExists((IDataReader) reader) ? this.TestResultId.GetInt32((IDataReader) reader) : 0;
        double num1 = this.Duration.ColumnExists((IDataReader) reader) ? (double) this.Duration.GetInt64((IDataReader) reader) : 0.0;
        DateTime dateTime1 = this.DateStarted.ColumnExists((IDataReader) reader) ? this.DateStarted.GetDateTime((IDataReader) reader) : new DateTime();
        DateTime dateTime2 = this.DateCompleted.ColumnExists((IDataReader) reader) ? this.DateCompleted.GetDateTime((IDataReader) reader) : new DateTime();
        if (!dateTime1.Equals(new DateTime()) && !dateTime2.Equals(new DateTime()))
          num1 = (dateTime2 - dateTime1).TotalMilliseconds;
        byte num2 = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader) : (byte) 0;
        int int32_4 = this.BuildId.ColumnExists((IDataReader) reader) ? this.BuildId.GetInt32((IDataReader) reader, 0) : 0;
        string str2 = this.BuildNumber.ColumnExists((IDataReader) reader) ? this.BuildNumber.GetString((IDataReader) reader, true) : string.Empty;
        int int32_5 = this.ReleaseId.ColumnExists((IDataReader) reader) ? this.ReleaseId.GetInt32((IDataReader) reader, 0) : 0;
        string str3 = this.ReleaseName.ColumnExists((IDataReader) reader) ? this.ReleaseName.GetString((IDataReader) reader, true) : string.Empty;
        string key = string.Empty;
        object obj = (object) string.Empty;
        if (!string.IsNullOrEmpty(groupByFieldString))
        {
          if (string.Equals(groupByFieldString, "Branch", StringComparison.OrdinalIgnoreCase))
          {
            key = str1;
            obj = (object) key;
          }
          else if (string.Equals(groupByFieldString, "Environment", StringComparison.OrdinalIgnoreCase))
          {
            key = int32_1.ToString();
            obj = (object) new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
            {
              Id = int32_5,
              Name = str3,
              EnvironmentDefinitionId = int32_1
            };
          }
        }
        if (key == null)
        {
          key = string.Empty;
          obj = (object) string.Empty;
        }
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult1 = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult();
        testCaseResult1.TestRun = new ShallowReference()
        {
          Id = int32_2.ToString()
        };
        testCaseResult1.Id = int32_3;
        testCaseResult1.CompletedDate = dateTime2;
        testCaseResult1.DurationInMs = num1;
        testCaseResult1.Outcome = ((TestOutcome) num2).ToString();
        ShallowReference shallowReference1;
        if (int32_4 == 0)
        {
          shallowReference1 = (ShallowReference) null;
        }
        else
        {
          shallowReference1 = new ShallowReference();
          shallowReference1.Id = int32_4.ToString();
          shallowReference1.Name = str2;
        }
        testCaseResult1.Build = shallowReference1;
        ShallowReference shallowReference2;
        if (int32_5 == 0)
        {
          shallowReference2 = (ShallowReference) null;
        }
        else
        {
          shallowReference2 = new ShallowReference();
          shallowReference2.Id = int32_5.ToString();
          shallowReference2.Name = str3;
        }
        testCaseResult1.Release = shallowReference2;
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult2 = testCaseResult1;
        resultsMap.Add(key, new TestResultHistoryDetailsForGroup()
        {
          LatestResult = testCaseResult2
        });
        resultsMap[key].GroupByValue = obj;
      }

      internal void bindFailingSinceColumn(
        SqlDataReader reader,
        Dictionary<TestCaseResultIdentifier, FailingSince> failingSinceMap)
      {
        int int32_1 = this.TestRunId.GetInt32((IDataReader) reader);
        int int32_2 = this.TestResultId.GetInt32((IDataReader) reader);
        string jsonString = this.FailingSince.GetString((IDataReader) reader, true);
        FailingSince convertedObject = (FailingSince) null;
        if (!string.IsNullOrEmpty(jsonString))
          TestResultHelper.TryJsonConvertWithRetry<FailingSince>(jsonString, out convertedObject, true);
        int testResultId = int32_2;
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(int32_1, testResultId);
        failingSinceMap[key] = convertedObject;
      }
    }
  }
}
