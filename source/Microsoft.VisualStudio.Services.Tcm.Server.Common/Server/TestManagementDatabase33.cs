// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase33
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase33 : TestManagementDatabase32
  {
    internal TestManagementDatabase33(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase33()
    {
    }

    public override List<TestRun> QueryTestRuns(
      int testRunId,
      Guid owner,
      string buildUri,
      Guid projectId,
      out Dictionary<int, string> iterationMap,
      out Dictionary<Guid, List<TestRun>> projectsRunsMap,
      int planId = -1,
      int skip = 0,
      int top = 2147483647,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns");
        Dictionary<int, TestRun> dictionary1 = new Dictionary<int, TestRun>();
        if (testRunId == 0)
        {
          this.PrepareStoredProcedure("prc_QueryTestRuns");
          this.BindNullableInt("@testRunId", testRunId, 0);
          this.BindNullableInt("@planId", planId, -1);
          this.BindGuidPreserveNull("@owner", owner);
          this.BindString("@buildUri", buildUri, 256, true, SqlDbType.NVarChar);
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindInt("@skip", skip);
          this.BindInt("@top", top);
        }
        else
        {
          this.PrepareStoredProcedure("TestResult.prc_QueryTestRun");
          this.BindInt("@testRunId", testRunId);
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        }
        TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, Guid> dictionary2 = new Dictionary<int, Guid>();
        iterationMap = new Dictionary<int, string>();
        projectsRunsMap = new Dictionary<Guid, List<TestRun>>();
        while (reader.Read())
        {
          int dataspaceId;
          string iterationUri;
          TestRun testRun = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          if (!string.IsNullOrEmpty(iterationUri))
            iterationMap[testRun.TestRunId] = iterationUri;
          if (dictionary2.ContainsKey(dataspaceId))
          {
            Guid key = dictionary2[dataspaceId];
            projectsRunsMap[key].Add(testRun);
          }
          else
          {
            Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
            dictionary2[dataspaceId] = dataspaceIdentifier;
            projectsRunsMap[dataspaceIdentifier] = new List<TestRun>();
            projectsRunsMap[dataspaceIdentifier].Add(testRun);
          }
          dictionary1.Add(testRun.TestRunId, testRun);
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestRuns");
        TestManagementDatabase.FetchTestRunsExColumns testRunsExColumns = new TestManagementDatabase.FetchTestRunsExColumns();
        while (reader.Read())
        {
          Tuple<int, TestExtensionField> tuple = testRunsExColumns.bind(reader);
          if (dictionary1.ContainsKey(tuple.Item1))
          {
            TestRun testRun = dictionary1[tuple.Item1];
            testRun.CustomFields = testRun.CustomFields ?? new List<TestExtensionField>();
            testRun.CustomFields.Add(tuple.Item2);
          }
        }
        return dictionary1.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns");
      }
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
        if (testRunId == 0)
        {
          this.PrepareStoredProcedure("TestResult.prc_QueryTestRuns3");
          this.BindNullableInt("@testRunId", testRunId, 0);
          this.BindNullableInt("@planId", planId, -1);
          this.BindGuidPreserveNull("@owner", owner);
          this.BindString("@buildUri", buildUri, 256, true, SqlDbType.NVarChar);
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindInt("@skip", skip);
          this.BindInt("@top", top);
        }
        else
        {
          this.PrepareStoredProcedure("TestResult.prc_QueryTestRun");
          this.BindInt("@testRunId", testRunId);
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        }
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
  }
}
