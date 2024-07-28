// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase58
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase58 : TestPlanningDatabase57
  {
    internal TestPlanningDatabase58(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase58()
    {
    }

    internal override Dictionary<int, ServerSuite> FetchServerSuites(
      TestManagementRequestContext context,
      IdAndRev[] suiteIds,
      List<int> deletedIds,
      bool includeTesters,
      out Dictionary<Guid, List<ServerSuite>> projectsSuitesMap)
    {
      try
      {
        using (PerfManager.Measure(context.RequestContext, "Database", "SuiteDatabase.FetchServerSuites"))
        {
          context.TraceEnter("Database", "TestPlanningDatabase.FetchServerSuites");
          this.PrepareStoredProcedure("prc_FetchTestSuites");
          this.BindIdAndRevTypeTable("@idsTable", (IEnumerable<IdAndRev>) suiteIds);
          this.BindBoolean("@includeTesters", includeTesters);
          SqlDataReader reader = this.ExecuteReader();
          List<ServerSuite> serverSuiteList = new List<ServerSuite>();
          TestPlanningDatabase58.FetchServerSuitesColumns serverSuitesColumns = new TestPlanningDatabase58.FetchServerSuitesColumns();
          Dictionary<int, Guid> dictionary1 = new Dictionary<int, Guid>();
          projectsSuitesMap = new Dictionary<Guid, List<ServerSuite>>();
          while (reader.Read())
          {
            int dataspaceId;
            ServerSuite serverSuite = serverSuitesColumns.bind(reader, out dataspaceId);
            if (dictionary1.ContainsKey(dataspaceId))
            {
              Guid key = dictionary1[dataspaceId];
              projectsSuitesMap[key].Add(serverSuite);
            }
            else
            {
              Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
              dictionary1[dataspaceId] = dataspaceIdentifier;
              projectsSuitesMap[dataspaceIdentifier] = new List<ServerSuite>();
              projectsSuitesMap[dataspaceIdentifier].Add(serverSuite);
            }
            serverSuiteList.Add(serverSuite);
          }
          Dictionary<int, ServerSuite> dictionary2 = new Dictionary<int, ServerSuite>();
          foreach (ServerSuite serverSuite in serverSuiteList)
          {
            serverSuite.suiteEntries.Clear();
            serverSuite.ServerEntries.Clear();
            dictionary2[serverSuite.Id] = serverSuite;
          }
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_FetchTestSuites");
          new TestPlanningDatabase.IdsPropertyColumns().bind(reader, deletedIds);
          if (dictionary2.Count > 0)
          {
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_FetchTestSuites");
            ServerSuiteEntry lastEntry = (ServerSuiteEntry) null;
            List<PointAssignment> points = new List<PointAssignment>();
            TestPlanningDatabase58.ServerSuiteEntryColumns suiteEntryColumns = new TestPlanningDatabase58.ServerSuiteEntryColumns();
            HashSet<ServerSuiteEntry> serverSuiteEntrySet = new HashSet<ServerSuiteEntry>();
            while (reader.Read())
            {
              ServerSuiteEntry serverSuiteEntry = suiteEntryColumns.bind(reader, lastEntry, points);
              ServerSuite serverSuite;
              if (serverSuiteEntry != lastEntry && dictionary2.TryGetValue(serverSuiteEntry.ParentSuiteId, out serverSuite))
              {
                if (!serverSuiteEntry.IsTestCaseEntry)
                  serverSuiteEntrySet.Add(serverSuiteEntry);
                serverSuite.suiteEntries.Add(serverSuiteEntry);
                if (serverSuiteEntry.EntryType == (byte) 1)
                  ++serverSuite.TestCaseCount;
              }
              lastEntry = serverSuiteEntry;
            }
            if (lastEntry != null && points.Count > 0)
              lastEntry.NewPointAssignments = points.ToArray();
            ServerSuite serverSuite1;
            if (lastEntry != null && dictionary2.TryGetValue(lastEntry.ParentSuiteId, out serverSuite1))
            {
              for (int index1 = 0; index1 < serverSuite1.suiteEntries.Count; ++index1)
              {
                int length = serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).NewPointAssignments != null ? serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).NewPointAssignments.Length : 0;
                TestPointAssignment[] testPointAssignmentArray = new TestPointAssignment[length];
                for (int index2 = 0; index2 < length; ++index2)
                  testPointAssignmentArray[index2] = new TestPointAssignment()
                  {
                    AssignedTo = ((IEnumerable<PointAssignment>) serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).NewPointAssignments).ElementAt<PointAssignment>(index2).AssignedTo,
                    AssignedToName = ((IEnumerable<PointAssignment>) serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).NewPointAssignments).ElementAt<PointAssignment>(index2).AssignedToName,
                    ConfigurationId = ((IEnumerable<PointAssignment>) serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).NewPointAssignments).ElementAt<PointAssignment>(index2).ConfigurationId,
                    ConfigurationName = ((IEnumerable<PointAssignment>) serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).NewPointAssignments).ElementAt<PointAssignment>(index2).ConfigurationName,
                    TestCaseId = ((IEnumerable<PointAssignment>) serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).NewPointAssignments).ElementAt<PointAssignment>(index2).TestCaseId
                  };
                serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).PointAssignments = testPointAssignmentArray;
                serverSuite1.ServerEntries.Add(new TestSuiteEntry()
                {
                  EntryId = serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).EntryId,
                  ParentSuiteId = serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).ParentSuiteId,
                  EntryType = serverSuite1.suiteEntries.ElementAt<ServerSuiteEntry>(index1).EntryType,
                  PointAssignments = testPointAssignmentArray
                });
              }
            }
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_FetchTestSuites");
            TestPlanningDatabase.TestSuiteConfigurationColumns configurationColumns = new TestPlanningDatabase.TestSuiteConfigurationColumns();
            while (reader.Read())
            {
              int suiteId;
              int configurationId;
              string configurationName;
              configurationColumns.bind(reader, out suiteId, out configurationId, out configurationName);
              ServerSuite serverSuite2;
              if (dictionary2.TryGetValue(suiteId, out serverSuite2))
              {
                serverSuite2.DefaultConfigurations.Add(configurationId);
                serverSuite2.DefaultConfigurationNames.Add(configurationName);
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

    protected class FetchServerSuitesColumns
    {
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder ParentSuiteId = new SqlColumnBinder(nameof (ParentSuiteId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder SuiteType = new SqlColumnBinder(nameof (SuiteType));
      private SqlColumnBinder RequirementId = new SqlColumnBinder(nameof (RequirementId));
      private SqlColumnBinder Query = new SqlColumnBinder(nameof (Query));
      private SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
      private SqlColumnBinder InheritConfigs = new SqlColumnBinder(nameof (InheritConfigs));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder LastPopulated = new SqlColumnBinder(nameof (LastPopulated));
      private SqlColumnBinder LastError = new SqlColumnBinder(nameof (LastError));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder LastSynced = new SqlColumnBinder(nameof (LastSynced));

      internal virtual ServerSuite bind(SqlDataReader reader, out int dataspaceId)
      {
        ServerSuite serverSuite = new ServerSuite();
        serverSuite.Id = this.SuiteId.GetInt32((IDataReader) reader);
        serverSuite.PlanId = this.PlanId.GetInt32((IDataReader) reader);
        serverSuite.ParentId = this.ParentSuiteId.GetInt32((IDataReader) reader);
        serverSuite.Title = this.Title.GetString((IDataReader) reader, false);
        serverSuite.Description = this.Description.GetString((IDataReader) reader, false);
        serverSuite.QueryString = this.Query.GetString((IDataReader) reader, true);
        serverSuite.ConvertedQueryString = this.Query.GetString((IDataReader) reader, true);
        serverSuite.Status = this.Status.GetString((IDataReader) reader, false);
        serverSuite.InheritDefaultConfigurations = this.InheritConfigs.GetBoolean((IDataReader) reader);
        serverSuite.Revision = this.Revision.GetInt32((IDataReader) reader);
        serverSuite.LastPopulated = this.LastPopulated.GetDateTime((IDataReader) reader);
        serverSuite.LastError = this.LastError.GetString((IDataReader) reader, true);
        serverSuite.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        serverSuite.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        serverSuite.RequirementId = this.RequirementId.GetInt32((IDataReader) reader);
        serverSuite.SuiteType = this.SuiteType.GetByte((IDataReader) reader);
        serverSuite.LastSynced = this.LastSynced.ColumnExists((IDataReader) reader) ? this.LastSynced.GetDateTime((IDataReader) reader) : DateTime.MinValue;
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        return serverSuite;
      }
    }

    protected class ServerSuiteEntryColumns
    {
      private SqlColumnBinder ParentSuiteId = new SqlColumnBinder(nameof (ParentSuiteId));
      private SqlColumnBinder ChildId = new SqlColumnBinder(nameof (ChildId));
      private SqlColumnBinder EntryType = new SqlColumnBinder(nameof (EntryType));
      private SqlColumnBinder ConfigurationId = new SqlColumnBinder(nameof (ConfigurationId));
      private SqlColumnBinder AssignedTo = new SqlColumnBinder(nameof (AssignedTo));
      private SqlColumnBinder ConfigurationName = new SqlColumnBinder(nameof (ConfigurationName));
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));

      internal ServerSuiteEntry bind(
        SqlDataReader reader,
        ServerSuiteEntry lastEntry,
        List<PointAssignment> points)
      {
        int int32_1 = this.ParentSuiteId.GetInt32((IDataReader) reader);
        int int32_2 = this.ChildId.GetInt32((IDataReader) reader);
        byte num = this.EntryType.GetByte((IDataReader) reader);
        int int32_3 = this.ConfigurationId.GetInt32((IDataReader) reader, 0);
        Guid guid = this.AssignedTo.GetGuid((IDataReader) reader, true);
        string str = this.ConfigurationName.GetString((IDataReader) reader, true);
        int int32_4 = this.PointId.ColumnExists((IDataReader) reader) ? this.PointId.GetInt32((IDataReader) reader, 0) : 0;
        if (lastEntry != null)
        {
          if (lastEntry.ParentSuiteId == int32_1 && lastEntry.EntryId == int32_2 && (int) lastEntry.EntryType == (int) num && int32_3 != 0)
          {
            List<PointAssignment> pointAssignmentList = points;
            PointAssignment pointAssignment = new PointAssignment();
            pointAssignment.TestCaseId = int32_2;
            pointAssignment.ConfigurationId = int32_3;
            pointAssignment.ConfigurationName = str;
            pointAssignment.AssignedTo = guid;
            pointAssignment.PointId = int32_4;
            pointAssignmentList.Add(pointAssignment);
            return lastEntry;
          }
          if (points.Count > 0)
          {
            lastEntry.NewPointAssignments = points.ToArray();
            points.Clear();
          }
        }
        ServerSuiteEntry serverSuiteEntry = new ServerSuiteEntry();
        serverSuiteEntry.ParentSuiteId = int32_1;
        serverSuiteEntry.EntryId = int32_2;
        serverSuiteEntry.EntryType = num;
        if (int32_3 <= 0)
          return serverSuiteEntry;
        List<PointAssignment> pointAssignmentList1 = points;
        PointAssignment pointAssignment1 = new PointAssignment();
        pointAssignment1.TestCaseId = int32_2;
        pointAssignment1.ConfigurationId = int32_3;
        pointAssignment1.ConfigurationName = str;
        pointAssignment1.AssignedTo = guid;
        pointAssignment1.PointId = int32_4;
        pointAssignmentList1.Add(pointAssignment1);
        return serverSuiteEntry;
      }
    }
  }
}
