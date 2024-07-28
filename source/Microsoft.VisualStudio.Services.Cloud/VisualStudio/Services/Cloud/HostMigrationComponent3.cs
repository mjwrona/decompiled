// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationComponent3
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationComponent3 : HostMigrationComponent2
  {
    private static readonly SqlMetaData[] typ_ShardingInfo = new SqlMetaData[3]
    {
      new SqlMetaData("MigrationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StorageType", SqlDbType.TinyInt),
      new SqlMetaData("VirtualNodes", SqlDbType.Int)
    };

    public HostMigrationComponent3()
    {
      this.m_storageOnlyName = "@storageOnly";
      this.m_storageMigrationName = "@storageMigrations";
    }

    protected override SourceMigrationColumns CreateSourceMigrationColumns() => (SourceMigrationColumns) new SourceMigrationColumns2();

    protected override TargetMigrationColumns CreateTargetMigrationColumns() => (TargetMigrationColumns) new TargetMigrationColumns2();

    protected override ObjectBinder<StorageMigration> CreatStorageMigrationColumns() => (ObjectBinder<StorageMigration>) new StorageMigrationColumns3();

    protected override ShardingInfoColumns CreateShardingInfoColumns() => new ShardingInfoColumns();

    protected override SqlParameter BindShardingInfoTable(
      string parameterName,
      IEnumerable<ShardingInfo> rows)
    {
      rows = rows ?? Enumerable.Empty<ShardingInfo>();
      System.Func<ShardingInfo, SqlDataRecord> selector = (System.Func<ShardingInfo, SqlDataRecord>) (info =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(HostMigrationComponent3.typ_ShardingInfo);
        sqlDataRecord.SetGuid(0, info.MigrationId);
        sqlDataRecord.SetByte(1, (byte) info.StorageType);
        sqlDataRecord.SetInt32(2, info.VirtualNodes);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Migration.typ_ShardingInfo", rows.Select<ShardingInfo, SqlDataRecord>(selector));
    }
  }
}
