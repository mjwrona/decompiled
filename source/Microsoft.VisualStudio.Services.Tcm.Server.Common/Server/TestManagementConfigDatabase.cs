// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementConfigDatabase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementConfigDatabase : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<TestManagementConfigDatabase>(1, true),
      (IComponentCreator) new ComponentCreator<TestManagementConfigDatabase2>(2, true)
    }, "TestManagement");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static readonly SqlMetaData[] typ_LogStoreStorageAccountTypeTable = new SqlMetaData[4]
    {
      new SqlMetaData("StorageAccountConnectionIndex", SqlDbType.Int),
      new SqlMetaData("StorageAccountStatus", SqlDbType.TinyInt),
      new SqlMetaData("AllocatedTenantCount", SqlDbType.Int),
      new SqlMetaData("MaxTenantCount", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_LogStoreContainerStatusTypeTable = new SqlMetaData[3]
    {
      new SqlMetaData("ContainerId", SqlDbType.VarChar, 256L),
      new SqlMetaData("Status", SqlDbType.Int),
      new SqlMetaData("StatusReason", SqlDbType.VarChar, 256L)
    };

    static TestManagementConfigDatabase()
    {
      TestManagementConfigDatabase.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      TestManagementConfigDatabase.RegisterException(550017, typeof (TestObjectNotFoundException));
      TestManagementConfigDatabase.RegisterException(550008, typeof (TestManagementInvalidOperationException));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) TestManagementConfigDatabase.s_sqlExceptionFactories;

    public virtual int CreateLogStoreMigrationRequest(
      LogStoreMigrationRequest LogStoreMigrationRequest)
    {
      return 0;
    }

    public virtual List<LogStoreMigrationRequest> GetLogStoreMigrationRequest(
      int? MigrationId,
      Guid? HostId)
    {
      return new List<LogStoreMigrationRequest>();
    }

    public virtual void CreateLogStoreContainerMigration(
      int MigrationId,
      IEnumerable<LogStoreContainerCopyDetails> ContainerCopyDetails)
    {
    }

    public virtual void UpdateLogStoreContainerMigration(
      int MigrationId,
      IEnumerable<LogStoreContainerCopyDetails> ContainerCopyDetails)
    {
    }

    public virtual void UpdateLogStoreMigrationRequest(
      LogStoreMigrationRequest LogStoreMigrationRequest)
    {
    }

    public virtual LogStoreMigrationResponse GetLogStoreContainerMigration(
      int MigrationId,
      int Status)
    {
      return new LogStoreMigrationResponse();
    }

    public void CreateLogStoreStorageAccounts(
      IEnumerable<LogStoreStorageAccount> logStoreStorageAccounts)
    {
      this.PrepareStoredProcedure("prc_CreateLogStoreStorageAccounts");
      this.BindTcmStorageConnectionPoolTypeTable("@logStoreStorageAccounts", logStoreStorageAccounts);
      this.ExecuteNonQuery();
    }

    public List<int> ReserveLogStoreStorageAccounts(int reserveCount)
    {
      this.PrepareStoredProcedure("prc_ReserveLogStoreStorageAccounts");
      this.BindInt("@reserveCount", reserveCount);
      SqlDataReader reader = this.ExecuteReader();
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("StorageAccountConnectionIndex");
      List<int> intList = new List<int>();
      while (reader.Read())
        intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader));
      return intList;
    }

    private static void RegisterException(int sqlErrorCode, Type exceptionType) => TestManagementConfigDatabase.s_sqlExceptionFactories.Add(sqlErrorCode, new SqlExceptionFactory(exceptionType));

    protected SqlParameter BindTcmStorageConnectionPoolTypeTable(
      string parameterName,
      IEnumerable<LogStoreStorageAccount> tcmStorageAccounts)
    {
      tcmStorageAccounts = tcmStorageAccounts ?? Enumerable.Empty<LogStoreStorageAccount>();
      return this.BindTable(parameterName, "typ_LogStoreStorageAccountTypeTable", this.BindTcmStorageConnectionPoolTypeTableRows(tcmStorageAccounts));
    }

    private IEnumerable<SqlDataRecord> BindTcmStorageConnectionPoolTypeTableRows(
      IEnumerable<LogStoreStorageAccount> tcmStorageAccounts)
    {
      foreach (LogStoreStorageAccount tcmStorageAccount in tcmStorageAccounts)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementConfigDatabase.typ_LogStoreStorageAccountTypeTable);
        sqlDataRecord.SetInt32(0, tcmStorageAccount.StorageAccountConnectionIndex);
        sqlDataRecord.SetByte(1, (byte) tcmStorageAccount.StorageAccountStatus);
        sqlDataRecord.SetInt32(2, tcmStorageAccount.AllocatedTenantCount);
        sqlDataRecord.SetInt32(3, tcmStorageAccount.MaxTenantCount);
        yield return sqlDataRecord;
      }
    }

    protected SqlParameter BindLogStoreContainerStatusTypeTable(
      string parameterName,
      IEnumerable<LogStoreContainerCopyDetails> containerCopyDetails)
    {
      containerCopyDetails = containerCopyDetails ?? Enumerable.Empty<LogStoreContainerCopyDetails>();
      return this.BindTable(parameterName, "typ_LogStoreContainerStatusTypeTable", this.BindLogStoreContainerStatusTypeTableRows(containerCopyDetails));
    }

    private IEnumerable<SqlDataRecord> BindLogStoreContainerStatusTypeTableRows(
      IEnumerable<LogStoreContainerCopyDetails> containerCopyDetails)
    {
      foreach (LogStoreContainerCopyDetails containerCopyDetail in containerCopyDetails)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TestManagementConfigDatabase.typ_LogStoreContainerStatusTypeTable);
        sqlDataRecord.SetString(0, containerCopyDetail.ContainerName);
        sqlDataRecord.SetInt32(1, (int) containerCopyDetail.Status);
        sqlDataRecord.SetString(2, containerCopyDetail.StatusReason ?? string.Empty);
        yield return sqlDataRecord;
      }
    }
  }
}
