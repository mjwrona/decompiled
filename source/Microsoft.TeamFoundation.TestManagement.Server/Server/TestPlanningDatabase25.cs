// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase25
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase25 : TestPlanningDatabase24
  {
    internal TestPlanningDatabase25(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase25()
    {
    }

    public override List<TestPoint> QueryTestPoints(Dictionary<string, List<object>> parametersMap) => this.GetTestPoints(parametersMap, "TestManagement.prc_QueryPoints");

    public override List<TestPoint> QueryTestPointsWithLastResults(
      Dictionary<string, List<object>> parametersMap)
    {
      return this.GetTestPoints(parametersMap, "TestManagement.prc_QueryPointsWithLastResults", true);
    }

    protected List<TestPoint> GetTestPoints(
      Dictionary<string, List<object>> parametersMap,
      string sprocName,
      bool includeLastResultDetails = false)
    {
      int[] ids1 = (int[]) null;
      int[] ids2 = (int[]) null;
      int[] ids3 = (int[]) null;
      int[] ids4 = (int[]) null;
      int[] ids5 = (int[]) null;
      int[] ids6 = (int[]) null;
      int[] ids7 = (int[]) null;
      int[] ids8 = (int[]) null;
      Guid[] rows = (Guid[]) null;
      byte[] states1 = (byte[]) null;
      byte[] states2 = (byte[]) null;
      byte[] states3 = (byte[]) null;
      if (parametersMap.ContainsKey("PointId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids1 = Array.ConvertAll<object, int>(parametersMap["PointId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("PlanId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids2 = Array.ConvertAll<object, int>(parametersMap["PlanId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("SuiteId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids3 = Array.ConvertAll<object, int>(parametersMap["SuiteId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("RecursiveSuiteId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids4 = Array.ConvertAll<object, int>(parametersMap["RecursiveSuiteId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("ConfigurationId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids5 = Array.ConvertAll<object, int>(parametersMap["ConfigurationId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("TestCaseId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids6 = Array.ConvertAll<object, int>(parametersMap["TestCaseId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("LastTestRunId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids7 = Array.ConvertAll<object, int>(parametersMap["LastTestRunId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("LastTestResultId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids8 = Array.ConvertAll<object, int>(parametersMap["LastTestResultId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("AssignedTo"))
      {
        Guid tester = Guid.Empty;
        rows = parametersMap["AssignedTo"].Where<object>((System.Func<object, bool>) (x => Guid.TryParse(x.ToString(), out tester) && tester != Guid.Empty)).Select<object, Guid>((System.Func<object, Guid>) (x => Guid.Parse(x.ToString()))).ToArray<Guid>();
        this.BindGuidTable("@testers", (IEnumerable<Guid>) rows);
      }
      if (parametersMap.ContainsKey("State"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        states1 = Array.ConvertAll<object, byte>(parametersMap["State"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte = new Converter<object, byte>(Convert.ToByte)));
      }
      if (parametersMap.ContainsKey("LastResultOutcome"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        states2 = Array.ConvertAll<object, byte>(parametersMap["LastResultOutcome"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte = new Converter<object, byte>(Convert.ToByte)));
      }
      if (parametersMap.ContainsKey("LastResultState"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        states3 = Array.ConvertAll<object, byte>(parametersMap["LastResultState"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte = new Converter<object, byte>(Convert.ToByte)));
      }
      this.PrepareStoredProcedure(sprocName);
      this.BindIdTypeTable("@pointIds", (IEnumerable<int>) ids1);
      this.BindIdTypeTable("@planIds", (IEnumerable<int>) ids2);
      this.BindIdTypeTable("@suiteIds", (IEnumerable<int>) ids3);
      this.BindIdTypeTable("@recursiveSuiteIds", (IEnumerable<int>) ids4);
      this.BindIdTypeTable("@configurationIds", (IEnumerable<int>) ids5);
      this.BindIdTypeTable("@testCaseIds", (IEnumerable<int>) ids6);
      this.BindIdTypeTable("@lastTestRunIds", (IEnumerable<int>) ids7);
      this.BindIdTypeTable("@lastTestResultIds", (IEnumerable<int>) ids8);
      this.BindGuidTable("@testers", (IEnumerable<Guid>) rows);
      this.BindTestManagement_TinyIntTypeTable("@states", (IEnumerable<byte>) states1);
      this.BindTestManagement_TinyIntTypeTable("@lastResultOutcomes", (IEnumerable<byte>) states2);
      this.BindTestManagement_TinyIntTypeTable("@lastResultStates", (IEnumerable<byte>) states3);
      SqlDataReader reader = this.ExecuteReader();
      List<TestPoint> testPoints = new List<TestPoint>();
      TestPlanningDatabase7.QueryTestPointColumns testPointColumns = new TestPlanningDatabase7.QueryTestPointColumns();
      while (reader.Read())
        testPoints.Add(testPointColumns.bind(reader, includeLastResultDetails));
      return testPoints;
    }

    public override List<TestPointStatistic> QueryTestPointStatistics(
      Dictionary<string, List<object>> parametersMap)
    {
      int[] ids1 = (int[]) null;
      int[] ids2 = (int[]) null;
      int[] ids3 = (int[]) null;
      int[] ids4 = (int[]) null;
      int[] ids5 = (int[]) null;
      int[] ids6 = (int[]) null;
      int[] ids7 = (int[]) null;
      int[] ids8 = (int[]) null;
      Guid[] rows = (Guid[]) null;
      byte[] states1 = (byte[]) null;
      byte[] states2 = (byte[]) null;
      byte[] states3 = (byte[]) null;
      if (parametersMap.ContainsKey("PointId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids1 = Array.ConvertAll<object, int>(parametersMap["PointId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("PlanId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids2 = Array.ConvertAll<object, int>(parametersMap["PlanId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("SuiteId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids3 = Array.ConvertAll<object, int>(parametersMap["SuiteId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("RecursiveSuiteId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids4 = Array.ConvertAll<object, int>(parametersMap["RecursiveSuiteId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("ConfigurationId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids5 = Array.ConvertAll<object, int>(parametersMap["ConfigurationId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("TestCaseId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids6 = Array.ConvertAll<object, int>(parametersMap["TestCaseId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("LastTestRunId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids7 = Array.ConvertAll<object, int>(parametersMap["LastTestRunId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("LastTestResultId"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids8 = Array.ConvertAll<object, int>(parametersMap["LastTestResultId"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("AssignedTo"))
      {
        Guid tester = Guid.Empty;
        rows = parametersMap["AssignedTo"].Where<object>((System.Func<object, bool>) (x => Guid.TryParse(x.ToString(), out tester) && tester != Guid.Empty)).Select<object, Guid>((System.Func<object, Guid>) (x => Guid.Parse(x.ToString()))).ToArray<Guid>();
        this.BindGuidTable("@testers", (IEnumerable<Guid>) rows);
      }
      if (parametersMap.ContainsKey("State"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        states1 = Array.ConvertAll<object, byte>(parametersMap["State"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte = new Converter<object, byte>(Convert.ToByte)));
      }
      if (parametersMap.ContainsKey("LastResultOutcome"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        states2 = Array.ConvertAll<object, byte>(parametersMap["LastResultOutcome"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte = new Converter<object, byte>(Convert.ToByte)));
      }
      if (parametersMap.ContainsKey("LastResultState"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        states3 = Array.ConvertAll<object, byte>(parametersMap["LastResultState"].ToArray(), TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte ?? (TestPlanningDatabase25.\u003C\u003EO.\u003C1\u003E__ToByte = new Converter<object, byte>(Convert.ToByte)));
      }
      this.PrepareStoredProcedure("TestManagement.prc_QueryPointStatistics");
      this.BindIdTypeTable("@pointIds", (IEnumerable<int>) ids1);
      this.BindIdTypeTable("@planIds", (IEnumerable<int>) ids2);
      this.BindIdTypeTable("@suiteIds", (IEnumerable<int>) ids3);
      this.BindIdTypeTable("@recursiveSuiteIds", (IEnumerable<int>) ids4);
      this.BindIdTypeTable("@configurationIds", (IEnumerable<int>) ids5);
      this.BindIdTypeTable("@testCaseIds", (IEnumerable<int>) ids6);
      this.BindIdTypeTable("@lastTestRunIds", (IEnumerable<int>) ids7);
      this.BindIdTypeTable("@lastTestResultIds", (IEnumerable<int>) ids8);
      this.BindGuidTable("@testers", (IEnumerable<Guid>) rows);
      this.BindTestManagement_TinyIntTypeTable("@states", (IEnumerable<byte>) states1);
      this.BindTestManagement_TinyIntTypeTable("@lastResultOutcomes", (IEnumerable<byte>) states2);
      this.BindTestManagement_TinyIntTypeTable("@lastResultStates", (IEnumerable<byte>) states3);
      SqlDataReader reader = this.ExecuteReader();
      List<TestPointStatistic> testPointStatisticList = new List<TestPointStatistic>();
      TestPlanningDatabase.TestPointStatisticColumns statisticColumns = new TestPlanningDatabase.TestPointStatisticColumns();
      while (reader.Read())
        testPointStatisticList.Add(statisticColumns.Bind(reader));
      return testPointStatisticList;
    }

    internal override TestPlan UpdateTestPlan(
      TestManagementRequestContext context,
      Guid projectGuid,
      string auditUser,
      TestPlan plan,
      Guid updatedBy,
      TestExternalLink[] links,
      string oldTitle,
      int suiteRevision)
    {
      try
      {
        context.TraceEnter("Database", "TestPlanDatabase.UpdateTestPlan");
        Validator.CheckStartEndDatesInOrder(plan.StartDate, plan.EndDate);
        this.PrepareStoredProcedure("prc_UpdatePlan");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@planId", plan.PlanId);
        this.BindStringPreserveNull("@oldName", oldTitle, 256, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@name", plan.Name, 256, SqlDbType.NVarChar);
        this.BindByte("@planState", plan.State);
        this.BindStringPreserveNull("@buildUri", plan.BuildUri, 64, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@buildDefinition", plan.BuildDefinition, 260, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@buildQuality", plan.BuildQuality, 256, SqlDbType.NVarChar);
        this.BindInt("@manualTestSettingsId", plan.TestSettingsId);
        this.BindInt("@automatedTestSettingsId", plan.AutomatedTestSettingsId);
        this.BindGuid("@manualTestEnvironmentId", plan.ManualTestEnvironmentId);
        this.BindGuid("@automatedTestEnvironmentId", plan.AutomatedTestEnvironmentId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindDateTime("@lastUpdated", plan.LastUpdated);
        this.BindInt("@suiteRevision", suiteRevision);
        this.BindInt("@buildDefinitionId", plan.BuildDefinitionId);
        int? parameterValue1 = new int?();
        int? parameterValue2 = new int?();
        if (plan.ReleaseEnvDef != null)
        {
          parameterValue1 = new int?(plan.ReleaseEnvDef.ReleaseDefinitionId);
          parameterValue2 = new int?(plan.ReleaseEnvDef.ReleaseEnvDefinitionId);
        }
        this.BindNullableInt("@releaseDefinitionId", parameterValue1);
        this.BindNullableInt("@releaseEnvDefinitionId", parameterValue2);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          throw new UnexpectedDatabaseResultException("prc_UpdatePlan");
        TestPlanningDatabase.UpdateTestPlansColumns testPlansColumns = new TestPlanningDatabase.UpdateTestPlansColumns();
        plan.PreviousBuildUri = testPlansColumns.PreviousBuildUri.GetString((IDataReader) reader, true);
        plan.BuildTakenDate = testPlansColumns.BuildTakenDate.GetDateTime((IDataReader) reader);
        plan.LastUpdatedBy = updatedBy;
        return plan;
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanDatabase.UpdateTestPlan");
      }
    }

    public override Dictionary<Guid, Dictionary<string, List<int>>> GetPlansHavingBuildDefinitionNamesWithoutId(
      int top)
    {
      this.PrepareStoredProcedure("prc_GetPlansHavingBuildDefinitionNamesWithoutId");
      this.BindInt("@top", top);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<Guid, Dictionary<string, List<int>>> projectToTestPlanMap = new Dictionary<Guid, Dictionary<string, List<int>>>();
      TestPlanningDatabase25.PlanBuildDefinitionNameColumns definitionNameColumns = new TestPlanningDatabase25.PlanBuildDefinitionNameColumns();
      while (reader.Read())
        definitionNameColumns.bind(reader, (TestPlanningDatabase) this, projectToTestPlanMap);
      return projectToTestPlanMap;
    }

    public override void PopulateBuildDefinitionIdInPlan(
      Guid projectGuid,
      Dictionary<int, List<int>> planIds)
    {
      this.PrepareStoredProcedure("prc_PopulateBuildDefinitionIdsForPlan");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindPlanIdToBuildDefinitionTypeTable("@buildDefinitionReferenceTable", planIds);
      this.ExecuteNonQuery();
    }

    internal override Dictionary<int, SuitePointCount> QuerySuitePointCounts2(
      TestManagementRequestContext context,
      Guid projectGuid,
      int planId,
      List<string> suiteStates,
      List<byte> pointStates,
      List<byte> pointOutcomes,
      List<Guid> assignedTesters,
      List<int> configurationIds)
    {
      this.PrepareStoredProcedure("TestManagement.prc_QuerySuitePointCounts");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@planId", planId);
      this.BindNameTypeTable("@suiteStates", (IEnumerable<string>) suiteStates);
      this.BindTestManagement_TinyIntTypeTable("@pointStates", (IEnumerable<byte>) pointStates);
      this.BindTestManagement_TinyIntTypeTable("@pointOutcomes", (IEnumerable<byte>) pointOutcomes);
      this.BindGuidTable("@assignedTesters", (IEnumerable<Guid>) assignedTesters);
      this.BindInt32TypeTable("@configurationIds", (IEnumerable<int>) configurationIds);
      SqlDataReader reader = this.ExecuteReader();
      TestPlanningDatabase25.QuerySuitePointCountsColumns2 pointCountsColumns2 = new TestPlanningDatabase25.QuerySuitePointCountsColumns2();
      Dictionary<int, SuitePointCount> dictionary = new Dictionary<int, SuitePointCount>();
      while (reader.Read())
      {
        int int32_1 = pointCountsColumns2.SuiteId.GetInt32((IDataReader) reader);
        int int32_2 = pointCountsColumns2.PointCount.GetInt32((IDataReader) reader);
        SuitePointCount suitePointCount = new SuitePointCount()
        {
          SuiteId = int32_1,
          PointCount = int32_2
        };
        dictionary.Add(int32_1, suitePointCount);
      }
      return dictionary;
    }

    internal override void CloneAfnStrip(
      int opId,
      int cloneTestCaseId,
      int tfsFileId,
      long uncompressedLength,
      string comment,
      bool changeCounterInterval = false)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "CloneAfnStrip: OpId {0}, TestCaseId {1}, FileId {2}, Length {3}", (object) opId, (object) cloneTestCaseId, (object) tfsFileId, (object) uncompressedLength);
      CloneOperationInformation operationInformation = new CloneOperationInformation();
      try
      {
        this.PrepareStoredProcedure("TestManagement.prc_CloneAfnStrip");
        this.BindInt("@opId", opId);
        this.BindInt("@cloneTestCaseId", cloneTestCaseId);
        this.BindInt("@tfsFileId", tfsFileId);
        this.BindLong("@uncompressedLength", uncompressedLength);
        this.BindStringPreserveNull("@cloneTitle", comment, -1, SqlDbType.NVarChar);
        this.BindBinary("@emptyAutomatedTestNameHash", this.GetSHA256Hash(string.Empty), 32, SqlDbType.VarBinary);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    protected class PlanBuildDefinitionNameColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder BuildDefinitionName = new SqlColumnBinder(nameof (BuildDefinitionName));

      internal void bind(
        SqlDataReader reader,
        TestPlanningDatabase db,
        Dictionary<Guid, Dictionary<string, List<int>>> projectToTestPlanMap)
      {
        int int32_1 = this.DataspaceId.GetInt32((IDataReader) reader);
        Guid dataspaceIdentifier = db.GetDataspaceIdentifier(int32_1);
        int int32_2 = this.PlanId.GetInt32((IDataReader) reader);
        string lower = this.BuildDefinitionName.GetString((IDataReader) reader, false).ToLower();
        if (!projectToTestPlanMap.ContainsKey(dataspaceIdentifier))
          projectToTestPlanMap[dataspaceIdentifier] = new Dictionary<string, List<int>>();
        if (!projectToTestPlanMap[dataspaceIdentifier].ContainsKey(lower))
          projectToTestPlanMap[dataspaceIdentifier][lower] = new List<int>();
        projectToTestPlanMap[dataspaceIdentifier][lower].Add(int32_2);
      }
    }

    private class QuerySuitePointCountsColumns2
    {
      internal SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      internal SqlColumnBinder PointCount = new SqlColumnBinder(nameof (PointCount));
    }
  }
}
