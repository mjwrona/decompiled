// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationComponent4
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
  public class HostMigrationComponent4 : HostMigrationComponent3
  {
    private static readonly SqlMetaData[] typ_StorageMigration4 = new SqlMetaData[7]
    {
      new SqlMetaData("MigrationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Id", SqlDbType.VarChar, 63L),
      new SqlMetaData("Uri", SqlDbType.VarChar, 4096L),
      new SqlMetaData("VsoArea", SqlDbType.VarChar, 256L),
      new SqlMetaData("StorageType", SqlDbType.TinyInt),
      new SqlMetaData("ShardIndex", SqlDbType.Int),
      new SqlMetaData("FilterKey", SqlDbType.VarChar, 4096L)
    };

    public override void CleanupMigrations(int ageInDays)
    {
      this.PrepareStoredProcedure("Migration.prc_CleanupMigrations");
      this.BindInt("@ageInDays", ageInDays);
      this.ExecuteNonQuery();
    }

    protected override SqlParameter BindStorageMigrationTable(
      string parameterName,
      IEnumerable<StorageMigration> rows)
    {
      rows = rows ?? Enumerable.Empty<StorageMigration>();
      System.Func<StorageMigration, SqlDataRecord> selector = (System.Func<StorageMigration, SqlDataRecord>) (state =>
      {
        SqlDataRecord record = new SqlDataRecord(HostMigrationComponent4.typ_StorageMigration4);
        record.SetGuid(0, state.MigrationId);
        record.SetString(1, state.Id);
        record.SetString(2, state.Uri);
        record.SetString(3, state.VsoArea);
        record.SetByte(4, (byte) state.StorageType);
        if (state.ShardIndex.HasValue)
          record.SetInt32(5, state.ShardIndex.Value);
        record.SetString(6, state.FilterKey, BindStringBehavior.Unchanged);
        return record;
      });
      return this.BindTable(parameterName, "Migration.typ_StorageMigration4", rows.Select<StorageMigration, SqlDataRecord>(selector));
    }

    protected override ObjectBinder<StorageMigration> CreatStorageMigrationColumns() => (ObjectBinder<StorageMigration>) new StorageMigrationColumns4();
  }
}
