// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase34
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase34 : TestPlanningDatabase33
  {
    internal TestPlanningDatabase34(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase34()
    {
    }

    internal override Dictionary<int, ServerTestSuite> FetchTestSuites(
      TestManagementRequestContext context,
      IdAndRev[] suiteIds,
      List<int> deletedIds,
      bool includeTesters,
      out Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "Database", "SuiteDatabase.FetchTestSuites"))
        {
          context.TraceEnter("Database", "TestPlanningDatabase.FetchTestSuites");
          this.PrepareStoredProcedure("prc_FetchTestSuites");
          this.BindIdAndRevTypeTable("@idsTable", (IEnumerable<IdAndRev>) suiteIds);
          this.BindBoolean("@includeTesters", includeTesters);
          SqlDataReader reader = this.ExecuteReader();
          List<ServerTestSuite> serverTestSuiteList = new List<ServerTestSuite>();
          TestPlanningDatabase.FetchTestSuitesColumns testSuitesColumns = new TestPlanningDatabase.FetchTestSuitesColumns();
          Dictionary<int, Guid> dictionary1 = new Dictionary<int, Guid>();
          projectsSuitesMap = new Dictionary<Guid, List<ServerTestSuite>>();
          while (reader.Read())
          {
            int dataspaceId;
            ServerTestSuite serverTestSuite = testSuitesColumns.bind(reader, out dataspaceId);
            if (dictionary1.ContainsKey(dataspaceId))
            {
              Guid key = dictionary1[dataspaceId];
              projectsSuitesMap[key].Add(serverTestSuite);
            }
            else
            {
              Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
              dictionary1[dataspaceId] = dataspaceIdentifier;
              projectsSuitesMap[dataspaceIdentifier] = new List<ServerTestSuite>();
              projectsSuitesMap[dataspaceIdentifier].Add(serverTestSuite);
            }
            serverTestSuiteList.Add(serverTestSuite);
          }
          Dictionary<int, ServerTestSuite> dictionary2 = new Dictionary<int, ServerTestSuite>();
          foreach (ServerTestSuite serverTestSuite in serverTestSuiteList)
          {
            serverTestSuite.ServerEntries.Clear();
            dictionary2[serverTestSuite.Id] = serverTestSuite;
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_FetchTestSuites");
          new TestPlanningDatabase.IdsPropertyColumns().bind(reader, deletedIds);
          if (dictionary2.Count > 0)
          {
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_FetchTestSuites");
            TestSuiteEntry lastEntry = (TestSuiteEntry) null;
            List<TestPointAssignment> testPointAssignmentList = new List<TestPointAssignment>();
            TestPlanningDatabase.TestSuiteEntryColumns suiteEntryColumns = new TestPlanningDatabase.TestSuiteEntryColumns();
            HashSet<TestSuiteEntry> testSuiteEntrySet = new HashSet<TestSuiteEntry>();
            while (reader.Read())
            {
              TestSuiteEntry testSuiteEntry = suiteEntryColumns.bind(reader, lastEntry, testPointAssignmentList);
              ServerTestSuite serverTestSuite;
              if (testSuiteEntry != lastEntry && dictionary2.TryGetValue(testSuiteEntry.ParentSuiteId, out serverTestSuite))
              {
                if (!testSuiteEntry.IsTestCaseEntry)
                  testSuiteEntrySet.Add(testSuiteEntry);
                serverTestSuite.ServerEntries.Add(testSuiteEntry);
                if (testSuiteEntry.EntryType == (byte) 1)
                  ++serverTestSuite.TestCaseCount;
              }
              lastEntry = testSuiteEntry;
            }
            if (lastEntry != null && testPointAssignmentList.Any<TestPointAssignment>())
              lastEntry.PointAssignments = testPointAssignmentList.ToArray();
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_FetchTestSuites");
            TestPlanningDatabase.TestSuiteConfigurationColumns configurationColumns = new TestPlanningDatabase.TestSuiteConfigurationColumns();
            ServerTestSuite serverTestSuite1;
            while (reader.Read())
            {
              int suiteId;
              int configurationId;
              string configurationName;
              configurationColumns.bind(reader, out suiteId, out configurationId, out configurationName);
              if (dictionary2.TryGetValue(suiteId, out serverTestSuite1))
              {
                serverTestSuite1.DefaultConfigurations.Add(configurationId);
                serverTestSuite1.DefaultConfigurationNames.Add(configurationName);
              }
            }
            if (reader.NextResult())
            {
              TestPlanningDatabase.FetchTestSuitesTesterColumns suitesTesterColumns = new TestPlanningDatabase.FetchTestSuitesTesterColumns();
              while (reader.Read())
              {
                int suiteId;
                Guid tester;
                suitesTesterColumns.bind(reader, out suiteId, out tester);
                if (dictionary2.TryGetValue(suiteId, out serverTestSuite1))
                  serverTestSuite1.DefaultTesters.Add(tester);
              }
            }
          }
          return dictionary2;
        }
      }
      finally
      {
        context.TraceLeave("Database", "TestPlanningDatabase.FetchTestSuites");
      }
    }

    internal override UpdatedProperties UpdateSuite(
      Guid projectGuid,
      ServerTestSuite suite,
      Guid updatedBy,
      IEnumerable<TestCaseAndOwner> entries,
      TestSuiteSource type)
    {
      try
      {
        this.PrepareStoredProcedure("prc_UpdateSuite");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", suite.ParentId);
        this.BindInt("@suiteId", suite.Id);
        this.BindStringPreserveNull("@title", suite.Title, 256, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@description", suite.Description, int.MaxValue, SqlDbType.NVarChar);
        this.BindStringPreserveNull("@query", suite.ConvertedQueryString, int.MaxValue, SqlDbType.NVarChar);
        this.BindString("@witStatus", suite.Status, 256, false, SqlDbType.NVarChar);
        this.BindInt("@revision", suite.Revision);
        this.BindTestCaseAndOwnerTypeTable("@testCaseAndOwnerTable", entries);
        this.BindBoolean("@inheritConfigs", suite.InheritDefaultConfigurations);
        this.BindGuidTable("@testersIdTable", (IEnumerable<Guid>) suite.DefaultTesters);
        this.BindIdTypeTable("@configIdsTable", (IEnumerable<int>) suite.DefaultConfigurations);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindDateTime("@lastUpdated", suite.LastUpdated);
        this.BindByte("@clientType", (byte) type);
        this.BindNullableByte("@queryMigrationState", (byte) suite.QueryMigrationState, (byte) 2);
        this.ExecuteReader();
        return new UpdatedProperties()
        {
          Revision = suite.Revision,
          LastUpdated = suite.LastUpdated,
          LastUpdatedBy = updatedBy
        };
      }
      catch (SqlException ex)
      {
        this.HandleDuplicateSuiteEntryError(ex);
        throw;
      }
    }

    internal override List<ServerTestSuite> FetchTestSuitesForPlan(
      TestManagementRequestContext context,
      Guid projectGuid,
      int planId,
      int rootSuiteId,
      bool includeOnlyL1,
      bool includeTester,
      out Dictionary<Guid, List<ServerTestSuite>> projectsSuitesMap)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "Database", "SuiteDatabase.FetchTestSuitesForPlan"))
        {
          context.TraceEnter("Database", "SuiteDatabase.FetchTestSuitesForPlan");
          this.PrepareStoredProcedure("prc_FetchTestSuitesForPlan");
          this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
          this.BindInt("@planId", planId);
          this.BindBoolean("@includeTesters", includeTester);
          SqlDataReader reader = this.ExecuteReader();
          return this.ReadSuiteMetaData(context, "prc_FetchTestSuitesForPlan", reader, includeTester, out projectsSuitesMap);
        }
      }
      finally
      {
        context.TraceLeave("Database", "SuiteDatabase.FetchTestSuitesForPlan");
      }
    }
  }
}
