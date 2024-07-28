// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase36
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase36 : TestPlanningDatabase35
  {
    internal TestPlanningDatabase36(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase36()
    {
    }

    public override void SyncTestPoints(
      TestManagementRequestContext context,
      Guid projectGuid,
      UpdatedProperties parentProps)
    {
      try
      {
        context.TraceEnter("Database", "SuiteDatabase.SyncTestPoints");
        this.PrepareStoredProcedure("TestManagement.prc_SyncTestPoints");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@parentSuiteId", parentProps.Id);
        this.BindInt("@parentSuiteRevision", parentProps.Revision);
        this.BindGuid("@lastUpdatedBy", parentProps.LastUpdatedBy);
        this.BindDateTime("@lastUpdated", parentProps.LastUpdated);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        context.TraceLeave("Database", "SuiteDatabase.SyncTestPoints");
        this.MapException(ex);
        throw;
      }
    }

    internal override int BeginCloneOperation(
      List<int> sourceSuiteIds,
      int targetSuiteId,
      Guid projectGuid,
      Guid targetProjectGuid,
      CloneOptions options,
      Guid createdBy,
      ResultObjectType operationType,
      bool changeCounterInterval = false)
    {
      try
      {
        this.PrepareStoredProcedure("prc_BeginCloneOperation");
        this.BindIdTypeTable("@sourceSuiteIdsTable", (IEnumerable<int>) sourceSuiteIds);
        this.BindInt("@targetSuiteId", targetSuiteId);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@targetDataspaceId", this.GetDataspaceId(targetProjectGuid));
        if (options != null && options.RelatedLinkComment != null)
          this.BindString("@linkComment", options.RelatedLinkComment, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        else
          this.BindString("@linkComment", (string) null, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        if (options != null && (!TestPlanningDatabase.IsNullOrEmpty(options.ResolvedFieldDetails) || !string.IsNullOrEmpty(options.DestinationWorkItemType)))
          this.BindXml("@editFieldDetails", this.DictionaryOfOverriddenFieldDetailsToXml(options.ResolvedFieldDetails, options.DestinationWorkItemType));
        else
          this.BindXml("@editFieldDetails", string.Empty);
        if (options != null && options.CloneRequirements)
          this.BindBoolean("@cloneRequirements", options.CloneRequirements);
        else
          this.BindBoolean("@cloneRequirements", false);
        if (options != null && options.CopyAncestorHierarchy)
          this.BindBoolean("@copyRecursively", false);
        else
          this.BindBoolean("@copyRecursively", true);
        this.BindGuid("@createdBy", createdBy);
        this.BindByte("@operationType", (byte) operationType);
        this.BindBoolean("@changeCounterInterval", changeCounterInterval);
        SqlDataReader sqlDataReader = this.ExecuteReader();
        int num = 0;
        while (sqlDataReader.Read())
          num = sqlDataReader.GetInt32(0);
        this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "BeginCloneOperation: OpId iss {0}", (object) num);
        return num;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
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
        this.BindBoolean("@changeCounterInterval", changeCounterInterval);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }
  }
}
