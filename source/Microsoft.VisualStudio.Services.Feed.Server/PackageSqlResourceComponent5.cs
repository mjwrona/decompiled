// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageSqlResourceComponent5
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageSqlResourceComponent5 : PackageSqlResourceComponent4
  {
    protected override int SortablePackageVersionMaxLength => (int) sbyte.MaxValue;

    protected override SqlMetaData[] Typ_PackageVersionUpdate => new SqlMetaData[4]
    {
      new SqlMetaData("PackageId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("NormalizedPackageVersion", SqlDbType.NVarChar, (long) sbyte.MaxValue),
      new SqlMetaData("SortableVersion", SqlDbType.NVarChar, (long) this.SortablePackageVersionMaxLength),
      new SqlMetaData("ProtocolMetadata", SqlDbType.NVarChar, -1L)
    };

    protected override SqlParameter BindPackageVersionBulkUpdateTable(
      string parameterName,
      IEnumerable<PackageVersionUpdate> updates)
    {
      updates = updates ?? Enumerable.Empty<PackageVersionUpdate>();
      System.Func<PackageVersionUpdate, SqlDataRecord> selector = (System.Func<PackageVersionUpdate, SqlDataRecord>) (update =>
      {
        SqlDataRecord record = new SqlDataRecord(this.Typ_PackageVersionUpdate);
        record.SetGuid(0, update.PackageId);
        record.SetString(1, update.NormalizedPackageVersion);
        record.SetString(2, update.SortablePackageVersion, BindStringBehavior.Unchanged);
        record.SetString(3, update.Metadata == null ? (string) null : JsonConvert.SerializeObject((object) update.Metadata), BindStringBehavior.Unchanged);
        return record;
      });
      return this.BindTable(parameterName, "Feed.typ_PackageVersionUpdate3", updates.Select<PackageVersionUpdate, SqlDataRecord>(selector));
    }
  }
}
