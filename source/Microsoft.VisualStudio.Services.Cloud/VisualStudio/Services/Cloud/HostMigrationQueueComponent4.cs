// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationQueueComponent4
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationQueueComponent4 : HostMigrationQueueComponent3
  {
    private static readonly SqlMetaData[] typ_HostMigrationRequest = new SqlMetaData[6]
    {
      new SqlMetaData("HostId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("TargetInstanceName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Priority", SqlDbType.TinyInt),
      new SqlMetaData("Options", SqlDbType.TinyInt),
      new SqlMetaData("TargetDatabaseId", SqlDbType.Int),
      new SqlMetaData("HostsAffectedByTheMove", SqlDbType.Int)
    };

    public override void AddQueueRequests(
      IEnumerable<HostMigrationRequest> hostMigrationRequests)
    {
      ArgumentUtility.CheckForNull<IEnumerable<HostMigrationRequest>>(hostMigrationRequests, nameof (hostMigrationRequests));
      this.PrepareStoredProcedure("Migration.prc_AddQueueRequests");
      this.BindAddQueueRequests(hostMigrationRequests);
      this.ExecuteNonQuery();
    }

    private void BindAddQueueRequests(
      IEnumerable<HostMigrationRequest> hostMigrationRequests)
    {
      System.Func<HostMigrationRequest, SqlDataRecord> selector = (System.Func<HostMigrationRequest, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(HostMigrationQueueComponent4.typ_HostMigrationRequest);
        record.SetGuid(0, row.HostId);
        record.SetString(1, row.TargetInstanceName);
        record.SetByte(2, row.Priority);
        record.SetByte(3, (byte) row.Options);
        record.SetNullableInt32(4, new int?(row.TargetDatabaseId));
        record.SetInt32(5, row.HostsAffectedByTheMove);
        return record;
      });
      this.BindTable("@migrationEntries", "Migration.typ_HostMigrationRequest", hostMigrationRequests.Select<HostMigrationRequest, SqlDataRecord>(selector));
    }
  }
}
