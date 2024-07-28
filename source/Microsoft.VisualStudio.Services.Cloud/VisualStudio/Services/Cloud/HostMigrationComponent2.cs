// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationComponent2
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationComponent2 : HostMigrationComponent
  {
    private static readonly SqlMetaData[] typ_GuidStringTable = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.VarChar, 4096L)
    };
    private static readonly SqlMetaData[] typ_SasUpdateTable2 = new SqlMetaData[3]
    {
      new SqlMetaData("MigrationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BlobContainerUri", SqlDbType.VarChar, 4096L),
      new SqlMetaData("SasToken", SqlDbType.VarBinary, 4096L)
    };

    protected override System.Func<StorageMigration, SqlDataRecord> GetContainerMigrationBinder(
      out string type)
    {
      type = "typ_GuidStringTable";
      return (System.Func<StorageMigration, SqlDataRecord>) (row =>
      {
        SqlDataRecord containerMigrationBinder = new SqlDataRecord(HostMigrationComponent2.typ_GuidStringTable);
        containerMigrationBinder.SetGuid(0, row.MigrationId);
        containerMigrationBinder.SetString(1, row.Uri);
        return containerMigrationBinder;
      });
    }

    protected override ObjectBinder<StorageMigration> CreatStorageMigrationColumns() => (ObjectBinder<StorageMigration>) new StorageMigrationColumns2();
  }
}
