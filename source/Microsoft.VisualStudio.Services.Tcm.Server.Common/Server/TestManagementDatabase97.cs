// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase97
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase97 : TestManagementDatabase96
  {
    internal TestManagementDatabase97(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase97()
    {
    }

    public override List<TestResultFailureType> QueryTestResultFailureTypes(
      int failureTypeId,
      Guid projectId)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestResultFailureTypes");
      List<TestResultFailureType> resultFailureTypeList = new List<TestResultFailureType>();
      try
      {
        int lazyInitialization = this.GetDataspaceIdWithLazyInitialization(projectId);
        this.PrepareStoredProcedure("prc_QueryTestFailureTypes");
        this.BindNullableInt("@failureTypeId", failureTypeId, -1);
        this.BindInt("@dataspaceId", lazyInitialization);
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase97.TestResultFailureTypeColumns failureTypeColumns = new TestManagementDatabase97.TestResultFailureTypeColumns();
        while (reader.Read())
          resultFailureTypeList.Add(failureTypeColumns.bind(reader));
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceError("Database", "TestResultDatabase.QueryTestResultFailureTypes threw exception. ProjectId: {0}, FailureTypeId: {1}, Exception message: {2}", (object) projectId, (object) failureTypeId, (object) ex.Message);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestResultFailureTypes");
      }
      return resultFailureTypeList;
    }

    public override TestResultFailureType CreateTestResultFailureType(
      string failureType,
      Guid projectId)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.CreateTestResultFailureType");
      TestResultFailureType resultFailureType = new TestResultFailureType();
      try
      {
        this.PrepareStoredProcedure("prc_CreateTestResultFailureType");
        this.BindString("@name", failureType, 256, true, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        SqlDataReader reader = this.ExecuteReader();
        reader.Read();
        return new TestManagementDatabase97.TestResultFailureTypeColumns().bind(reader);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceError("Database", "TestResultDatabase.CreateTestResultFailureType threw exception . ProjectId: {0}, FailureTypeName: {1}, Exception message: {2}", (object) projectId, (object) failureType, (object) ex.Message);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.CreateTestResultFailureType");
      }
    }

    public override bool DeleteTestResultFailureType(
      Guid projectId,
      int failureTypeId,
      Guid auditIdentity)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.DeleteTestResultFailureType");
      try
      {
        this.PrepareStoredProcedure("TestManagement.prc_DeleteTestFailureType");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@failureTypeId", failureTypeId);
        this.BindGuid("@auditIdentity", auditIdentity);
        SqlDataReader reader = this.ExecuteReader();
        reader.Read();
        return new SqlColumnBinder("ISDELETED").GetInt32((IDataReader) reader) == 1;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceError("Database", "TestResultDatabase.DeleteTestResultFailureType threw exception. ProjectId: {0}, FailureTypeId : {1}, Exception message: {2}", (object) projectId, (object) failureTypeId, (object) ex.Message);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.DeleteTestResultFailureType");
      }
    }

    protected class TestResultFailureTypeColumns
    {
      private SqlColumnBinder FailureTypeId = new SqlColumnBinder(nameof (FailureTypeId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));

      internal TestResultFailureType bind(SqlDataReader reader) => new TestResultFailureType()
      {
        Id = this.FailureTypeId.GetInt32((IDataReader) reader),
        Name = this.Name.GetString((IDataReader) reader, false)
      };
    }
  }
}
