// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase81
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
  public class TestManagementDatabase81 : TestManagementDatabase80
  {
    internal override Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData GetTestExecutionReport3(
      Guid projectId,
      int planId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails> testAuthoringDetails,
      List<string> dimensionList)
    {
      this.RequestContext.TraceEnter("Database", "ChartingDatabase.GetTestExecutionReport3");
      Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData executionReport3 = new Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData();
      try
      {
        this.PrepareStoredProcedure("TestManagement.prc_GetTestExecutionReport3");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@planId", planId);
        this.BindTestAuthoringDetails2TypeTableTable("@authoringDetails", (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestAuthoringDetails>) testAuthoringDetails);
        this.BindNameTypeTable("@dimensions", (IEnumerable<string>) dimensionList);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            Dictionary<string, object> dimensionValues = new Dictionary<string, object>();
            foreach (string dimension in dimensionList)
            {
              object obj = new SqlColumnBinder(dimension).GetObject((IDataReader) reader);
              dimensionValues[dimension] = obj;
            }
            long int64 = Convert.ToInt64(new SqlColumnBinder("AggTestsCount").GetInt32((IDataReader) reader));
            executionReport3.AddReportDatarow(dimensionValues, int64);
          }
        }
        return executionReport3;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "ChartingDatabase.GetTestExecutionReport2");
      }
    }

    public override List<PointLastResult> FilterPointsOnOutcome2(
      Guid projectGuid,
      int planId,
      List<int> pointIds,
      List<byte> pointOutcomes,
      List<byte> resultStates)
    {
      this.PrepareStoredProcedure("TestResult.prc_FilterPointOutcome2");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectGuid));
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

    internal TestManagementDatabase81(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase81()
    {
    }

    public override List<TestCaseResult> GetTestCaseResultsByPointIds2(
      Guid projectId,
      int planId,
      List<int> pointIds)
    {
      this.PrepareStoredProcedure("TestResult.prc_GetTestResultsByPointIds2");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@planId", planId);
      this.BindInt32TypeTable("@pointIds", (IEnumerable<int>) pointIds);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.FetchTestResultsColumns testResultsColumns = new TestManagementDatabase.FetchTestResultsColumns();
      List<TestCaseResult> resultsByPointIds2 = new List<TestCaseResult>();
      while (reader.Read())
        resultsByPointIds2.Add(testResultsColumns.bind(reader));
      return resultsByPointIds2;
    }
  }
}
