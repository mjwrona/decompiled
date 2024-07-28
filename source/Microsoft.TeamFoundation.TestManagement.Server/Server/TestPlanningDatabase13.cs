// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase13
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase13 : TestPlanningDatabase12
  {
    private static readonly SqlMetaData[] typ_SuiteEntryOrderTypeTable = new SqlMetaData[2]
    {
      new SqlMetaData("TestCaseId", SqlDbType.Int),
      new SqlMetaData("SequenceNumber", SqlDbType.Int)
    };

    internal override IEnumerable<SuiteEntry> ReorderSuiteEntries(
      Guid projectId,
      int suiteId,
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "SuiteEntryDatabase.ReorderSuiteEntries"))
      {
        this.PrepareStoredProcedure("prc_ReorderSuiteEntries");
        this.BindInt("@suiteId", suiteId);
        this.BindSuiteEntryOrderTypeTable("@testCaseOrder", suiteEntries);
        return this.GetSuiteEntriesFromDb(suiteId);
      }
    }

    protected SqlParameter BindSuiteEntryOrderTypeTable(
      string parameterName,
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      suiteEntries = suiteEntries ?? Enumerable.Empty<SuiteEntryUpdateModel>();
      return this.BindTable(parameterName, "typ_SuiteEntryOrderTypeTable", this.BindSuiteEntryOrderTypeTableRows(suiteEntries));
    }

    private IEnumerable<SqlDataRecord> BindSuiteEntryOrderTypeTableRows(
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      foreach (SuiteEntryUpdateModel suiteEntry in suiteEntries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestPlanningDatabase13.typ_SuiteEntryOrderTypeTable);
        sqlDataRecord.SetInt32(0, suiteEntry.TestCaseId);
        sqlDataRecord.SetInt32(1, suiteEntry.SequenceNumber);
        yield return sqlDataRecord;
      }
    }

    internal TestPlanningDatabase13(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase13()
    {
    }

    internal override List<TestPoint> QueryTestPoints(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.QueryTestPoints"))
        return this.QueryTestPoints("prc_QueryPoints3", whereClause, orderBy, displayNameInGroupList, false);
    }

    internal override List<TestPoint> QueryTestPointsWithLastResults(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList)
    {
      using (PerfManager.Measure(this.RequestContext, "Database", "TestPlanDatabase.QueryTestPointsWithLastResult"))
        return this.QueryTestPoints("prc_QueryPointsWithLastResults2", whereClause, orderBy, displayNameInGroupList, true);
    }

    internal override Session CreateSession(
      Guid projectGuid,
      Session session,
      Guid updatedBy,
      int source)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CreateSession");
        this.BindString("@title", session.Title, 256, false, SqlDbType.NVarChar);
        this.BindGuid("@owner", session.Owner);
        this.BindByte("@state", session.State);
        this.BindString("@buildUri", session.BuildUri, 256, true, SqlDbType.NVarChar);
        this.BindString("@buildNumber", session.BuildNumber, 260, true, SqlDbType.NVarChar);
        this.BindString("@buildPlatform", session.BuildPlatform, 256, true, SqlDbType.NVarChar);
        this.BindString("@buildFlavor", session.BuildFlavor, 256, true, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@testPlanId", session.TestPlanId);
        this.BindString("@controller", session.Controller, 256, true, SqlDbType.NVarChar);
        this.BindInt("@testSettingsId", session.TestSettingsId);
        this.BindInt("@publicTestSettingsId", session.PublicTestSettingsId);
        this.BindGuid("@testEnvironmentId", session.TestEnvironmentId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindLong("@duration", session.Duration);
        this.BindString("@sprint", session.Sprint, 64, true, SqlDbType.NVarChar);
        this.BindString("@computerName", session.ComputerName, 64, true, SqlDbType.NVarChar);
        this.BindInt("@userStoryId", session.UserStoryId);
        this.BindInt("@charterId", session.CharterId);
        this.BindInt("@feedbackId", session.FeedbackId);
        this.BindInt("@configurationId", session.ConfigurationId);
        this.BindString("@configurationName", session.ConfigurationName, 256, true, SqlDbType.NVarChar);
        this.BindInt("@areaId", 0);
        this.BindInt("@source", source);
        this.BindString("@teamField", string.Empty, 256, true, SqlDbType.NVarChar);
        SqlDataReader reader = this.ExecuteReader();
        return reader.Read() ? new TestPlanningDatabase.CreateSessionColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_CreateSession");
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal override TestSession CreateTestSession(TestSession session, string teamField)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CreateSession");
        this.BindString("@title", session.Title, 256, false, SqlDbType.NVarChar);
        this.BindGuid("@owner", new Guid(session.Owner.Id));
        this.BindByte("@state", (byte) session.State);
        this.BindString("@buildUri", (string) null, 256, true, SqlDbType.NVarChar);
        this.BindString("@buildNumber", (string) null, 260, true, SqlDbType.NVarChar);
        this.BindString("@buildPlatform", (string) null, 256, true, SqlDbType.NVarChar);
        this.BindString("@buildFlavor", (string) null, 256, true, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceId(new Guid(session.Project.Id)));
        int result1;
        int.TryParse(session.PropertyBag.GetProperty("TestPlanId"), out result1);
        this.BindInt("@testPlanId", result1);
        this.BindString("@controller", (string) null, 256, true, SqlDbType.NVarChar);
        int result2;
        int.TryParse(session.PropertyBag.GetProperty("TestSettingsId"), out result2);
        this.BindInt("@testSettingsId", result2);
        this.BindInt("@publicTestSettingsId", 0);
        this.BindGuid("@testEnvironmentId", Guid.Empty);
        this.BindGuid("@lastUpdatedBy", new Guid(session.LastUpdatedBy.Id));
        this.BindLong("@duration", 0L);
        this.BindString("@sprint", (string) null, 64, true, SqlDbType.NVarChar);
        this.BindString("@computerName", (string) null, 64, true, SqlDbType.NVarChar);
        int result3;
        int.TryParse(session.PropertyBag.GetProperty("RequirementId"), out result3);
        this.BindInt("@userStoryId", result3);
        this.BindInt("@charterId", 0);
        string property = session.PropertyBag.GetProperty("FeedbackRequestId");
        this.BindInt("@feedbackId", string.IsNullOrEmpty(property) ? 0 : int.Parse(property));
        this.BindInt("@configurationId", 0);
        this.BindString("@configurationName", (string) null, 256, true, SqlDbType.NVarChar);
        this.BindInt("@areaId", int.Parse(session.Area.Id));
        this.BindInt("@source", (int) session.Source);
        this.BindString("@teamField", teamField, 256, true, SqlDbType.NVarChar);
        SqlDataReader reader = this.ExecuteReader();
        return reader.Read() ? ContractHelper.ToTestSession(new TestPlanningDatabase.CreateSessionColumns().bind(reader)) : throw new UnexpectedDatabaseResultException("prc_CreateSession");
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal override TestSession UpdateTestSession(TestSession session)
    {
      try
      {
        this.PrepareStoredProcedure("prc_UpdateSession");
        this.BindInt("@dataspaceId", this.GetDataspaceId(new Guid(session.Project.Id)));
        this.BindInt("@sessionId", session.Id);
        this.BindString("@title", (string) null, 256, true, SqlDbType.NVarChar);
        this.BindNullableGuid("@owner", Guid.Empty);
        this.BindByte("@state", (byte) session.State);
        this.BindString("@controller", (string) null, 256, true, SqlDbType.NVarChar);
        this.BindInt("@testSettingsId", 0);
        this.BindInt("@publicTestSettingsId", 0);
        this.BindGuid("@testEnvironmentId", Guid.Empty);
        this.BindGuid("@lastUpdatedBy", new Guid(session.LastUpdatedBy.Id));
        this.BindStringPreserveNull("@comment", session.Comment, 1048576, SqlDbType.NVarChar);
        this.BindNullableDateTime("@dateStarted", new DateTime?());
        this.BindNullableDateTime("@dateCompleted", new DateTime?());
        this.BindLong("@duration", 0L);
        this.BindString("@sprint", (string) null, 64, true, SqlDbType.NVarChar);
        this.BindString("@computerName", (string) null, 64, true, SqlDbType.NVarChar);
        this.BindInt("@userStoryId", 0);
        this.BindInt("@charterId", 0);
        string property = session.PropertyBag.GetProperty("FeedbackRequestId");
        this.BindInt("@feedbackId", string.IsNullOrEmpty(property) ? 0 : int.Parse(property));
        this.BindInt("@configurationId", 0);
        this.BindString("@configurationName", string.Empty, 256, true, SqlDbType.NVarChar);
        this.BindInt("@revision", session.Revision);
        this.BindXml("@notes", string.Empty);
        this.BindXml("@bookmarks", string.Empty);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedSessionProperties sessionProperties = reader.Read() ? new TestPlanningDatabase.UpdatedSessionPropertyColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateSession");
        sessionProperties.LastUpdatedBy = new Guid(session.LastUpdatedBy.Id);
        session.Revision = sessionProperties.Revision;
        return session;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal override void CreateAssociatedWorkItemsForTestSession(
      int sessionId,
      TestSessionWorkItemReference[] workItemsFilled)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CreateAssociatedWorkItemsForSession");
        this.BindSessionWorkItemLinkTypeTable("@workItemLinksTable", (IEnumerable<KeyValuePair<int, string>>) this.GetListOfUrisForTestSession(sessionId, workItemsFilled));
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal override void CreateExploredWorkItemsForTestSession(
      TestSession session,
      List<TestSessionExploredWorkItemReference> workItemsExplored)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CreateExploredWorkItemsForSession");
        this.BindInt("@dataspaceId", this.GetDataspaceId(new Guid(session.Project.Id)));
        this.BindInt("@sessionId", session.Id);
        this.BindSessionWorkItemExploredLinkTypeTable("@workItemExploredLinksTable", (IEnumerable<TestSessionExploredWorkItemReference>) workItemsExplored);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal override List<int> GetSessionIdsOfTeam(
      string projectId,
      int period,
      Guid sessionOwner,
      List<int> sourceList,
      List<int> stateList,
      bool isTeamFieldAreaPath,
      List<string> teamFieldsOfTeam)
    {
      List<int> sessionIdsOfTeam = new List<int>();
      this.PrepareStoredProcedure("prc_QueryTestSessionIds");
      this.BindInt("@dataspaceId", this.GetDataspaceId(new Guid(projectId)));
      this.BindInt("@period", period);
      this.BindBoolean("@isTeamFieldAreaPath", isTeamFieldAreaPath);
      this.BindNameTypeTable("@teamFieldTable", (IEnumerable<string>) teamFieldsOfTeam);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("SessionId");
      while (reader.Read())
        sessionIdsOfTeam.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      return sessionIdsOfTeam;
    }

    internal override Dictionary<int, TestSession> QueryTestSession(
      TfsTestManagementRequestContext testmanagementRequestContext,
      string projectId,
      List<int> sessionIds,
      List<int> sourceList,
      List<int> stateList,
      ref List<int> workItemRefListForSession,
      ref Dictionary<int, List<TestSessionWorkItemReference>> sessionIdToListOfWorkItemRef,
      ref List<int> exploredItemRefListForSession,
      ref Dictionary<int, List<TestSessionExploredWorkItemReference>> sessionIdToListOfExploredItemRef)
    {
      Dictionary<int, TestSession> dictionary = new Dictionary<int, TestSession>();
      workItemRefListForSession = new List<int>();
      sessionIdToListOfWorkItemRef = new Dictionary<int, List<TestSessionWorkItemReference>>();
      exploredItemRefListForSession = new List<int>();
      sessionIdToListOfExploredItemRef = new Dictionary<int, List<TestSessionExploredWorkItemReference>>();
      try
      {
        this.PrepareStoredProcedure("prc_QueryTestSession");
        this.BindInt("@dataspaceId", this.GetDataspaceId(new Guid(projectId)));
        this.BindIdTypeTable("@sourceTable", (IEnumerable<int>) sourceList);
        this.BindIdTypeTable("@stateTable", (IEnumerable<int>) stateList);
        this.BindIdTypeTable("@sessionIdTable", (IEnumerable<int>) sessionIds);
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
        {
          TestSession testSession = new TestPlanningDatabase.QueryTestSessionColumns().bind((TestManagementRequestContext) testmanagementRequestContext, reader);
          dictionary[testSession.Id] = testSession;
        }
        if (reader.NextResult())
          new TestPlanningDatabase.ReadAssociatedWorkItems().bind((TestManagementRequestContext) testmanagementRequestContext, reader, ref workItemRefListForSession, ref sessionIdToListOfWorkItemRef);
        if (reader.NextResult())
          new TestPlanningDatabase.ReadExploredWorkItems().bind((TestManagementRequestContext) testmanagementRequestContext, reader, ref exploredItemRefListForSession, ref sessionIdToListOfExploredItemRef);
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      return dictionary;
    }

    protected List<KeyValuePair<int, string>> GetListOfUrisForTestSession(
      int testSessionId,
      TestSessionWorkItemReference[] workItemsFilled)
    {
      List<KeyValuePair<int, string>> urisForTestSession = new List<KeyValuePair<int, string>>();
      if (testSessionId <= 0 || workItemsFilled == null)
        return urisForTestSession;
      for (int index = 0; index < workItemsFilled.Length; ++index)
        urisForTestSession.Add(new KeyValuePair<int, string>(testSessionId, "vstfs:///WorkItemTracking/WorkItem/" + (object) workItemsFilled[index].Id));
      return urisForTestSession;
    }
  }
}
