// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementConfigDatabase2
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
  public class TestManagementConfigDatabase2 : TestManagementConfigDatabase
  {
    public override int CreateLogStoreMigrationRequest(
      LogStoreMigrationRequest LogStoreMigrationRequest)
    {
      this.PrepareStoredProcedure("prc_CreateLogStoreMigrationRequest");
      this.BindGuid("@hostId", LogStoreMigrationRequest.HostId);
      this.BindInt("@status", (int) LogStoreMigrationRequest.Status);
      this.BindString("@statusReason", LogStoreMigrationRequest.StatusReason, (int) byte.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      SqlParameter sqlParameter = this.BindInt("@migrationId", 0);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      LogStoreMigrationRequest.MigrationId = (int) sqlParameter.Value;
      return (int) sqlParameter.Value;
    }

    public override List<LogStoreMigrationRequest> GetLogStoreMigrationRequest(
      int? MigrationId,
      Guid? HostId)
    {
      this.PrepareStoredProcedure("prc_GetLogStoreMigrationRequest");
      this.BindNullableInt("@migrationId", MigrationId);
      this.BindNullableGuid("@hostId", HostId);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder(nameof (MigrationId));
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder(nameof (HostId));
      SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("Status");
      SqlColumnBinder sqlColumnBinder4 = new SqlColumnBinder("StatusReason");
      List<LogStoreMigrationRequest> migrationRequest = new List<LogStoreMigrationRequest>();
      while (reader.Read())
        migrationRequest.Add(new LogStoreMigrationRequest(sqlColumnBinder1.GetInt32((IDataReader) reader), sqlColumnBinder2.GetGuid((IDataReader) reader), (LogStoreMigrationStatus) sqlColumnBinder3.GetInt32((IDataReader) reader), sqlColumnBinder4.GetString((IDataReader) reader, true)));
      return migrationRequest;
    }

    public override void CreateLogStoreContainerMigration(
      int MigrationId,
      IEnumerable<LogStoreContainerCopyDetails> ContainerCopyDetails)
    {
      this.PrepareStoredProcedure("prc_CreateLogStoreContainerMigration");
      this.BindInt("@migrationId", MigrationId);
      this.BindLogStoreContainerStatusTypeTable("@containerStatus", ContainerCopyDetails);
      this.ExecuteNonQuery();
    }

    public override void UpdateLogStoreContainerMigration(
      int MigrationId,
      IEnumerable<LogStoreContainerCopyDetails> ContainerCopyDetails)
    {
      this.PrepareStoredProcedure("prc_UpdateLogStoreContainerMigration");
      this.BindInt("@migrationId", MigrationId);
      this.BindLogStoreContainerStatusTypeTable("@containerStatus", ContainerCopyDetails);
      this.ExecuteNonQuery();
    }

    public override void UpdateLogStoreMigrationRequest(
      LogStoreMigrationRequest LogStoreMigrationRequest)
    {
      this.PrepareStoredProcedure("prc_UpdateLogStoreMigrationRequest");
      this.BindInt("@migrationId", LogStoreMigrationRequest.MigrationId);
      this.BindInt("@status", (int) LogStoreMigrationRequest.Status);
      this.BindString("@statusReason", LogStoreMigrationRequest.StatusReason, LogStoreMigrationRequest.StatusReason.Length, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override LogStoreMigrationResponse GetLogStoreContainerMigration(
      int MigrationId,
      int Status)
    {
      this.PrepareStoredProcedure("prc_GetLogStoreContainerMigration");
      this.BindInt("@migrationId", MigrationId);
      this.BindNullableInt("@status", new int?(Status));
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder(nameof (MigrationId));
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("ContainerId");
      SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder(nameof (Status));
      SqlColumnBinder sqlColumnBinder4 = new SqlColumnBinder("StatusReason");
      LogStoreContainerCopyResponse containerCopyResponse = new LogStoreContainerCopyResponse();
      LogStoreMigrationResponse containerMigration = new LogStoreMigrationResponse();
      List<LogStoreContainerCopyDetails> containerCopyDetailsList = new List<LogStoreContainerCopyDetails>();
      while (reader.Read())
      {
        containerMigration.MigrationId = sqlColumnBinder1.GetInt32((IDataReader) reader);
        containerCopyDetailsList.Add(new LogStoreContainerCopyDetails()
        {
          ContainerName = sqlColumnBinder2.GetString((IDataReader) reader, true),
          Status = (LogStoreContainerCopyStatus) sqlColumnBinder3.GetInt32((IDataReader) reader),
          StatusReason = sqlColumnBinder4.GetString((IDataReader) reader, true)
        });
      }
      containerMigration.ContainerMigratedDetails = containerCopyResponse;
      return containerMigration;
    }
  }
}
