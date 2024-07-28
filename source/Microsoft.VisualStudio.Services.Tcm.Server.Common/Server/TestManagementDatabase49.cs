// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase49
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase49 : TestManagementDatabase48
  {
    internal TestManagementDatabase49(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase49()
    {
    }

    internal override TestCaseResult ResetTestResult(
      Guid projectId,
      int testRunId,
      int testResultId,
      Guid updatedBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.ResetTestResult");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("TestResult.prc_ResetTestResult");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@testResultId", testResultId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindInt("@maxCommentSize", TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext));
        this.BindBoolean("@isTcmService", isTcmService);
        SqlDataReader reader = this.ExecuteReader();
        TestCaseResult testCaseResult = reader.Read() ? new TestManagementDatabase.FetchTestResultsColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_ResetTestResult");
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return testCaseResult;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.ResetTestResult");
      }
    }

    public override UpdatedProperties AbortTestRun(
      Guid projectId,
      int testRunId,
      int revision,
      TestRunAbortOptions options,
      byte substate,
      Guid updatedBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.AbortTestRun");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("TestResult.prc_AbortTestRun");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@revision", revision);
        this.BindInt("@options", (int) options);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindByte("@substate", substate);
        this.BindBoolean("@isTcmService", isTcmService);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase24.UpdatedPropertyColumns().BindAbortedRunProperties(reader) : throw new UnexpectedDatabaseResultException("prc_AbortTestRun");
        updatedProperties.LastUpdatedBy = updatedBy;
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return updatedProperties;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.AbortTestRun");
      }
    }

    internal override List<TestCaseResult> QueryTestResultsByPoint(
      Guid projectId,
      int planId,
      int pointId,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestResultsByPoint");
        List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
        this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsByPoint");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@planId", planId);
        this.BindInt("@testPointId", pointId);
        this.BindBoolean("@isTcmService", isTcmService);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase.FetchTestResultsColumns testResultsColumns = new TestManagementDatabase.FetchTestResultsColumns();
        while (reader.Read())
        {
          TestCaseResult testCaseResult = testResultsColumns.bind(reader);
          testCaseResultList.Add(testCaseResult);
        }
        return testCaseResultList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestResultsByPoint");
      }
    }
  }
}
