// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseTenantUsageDetailedBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseTenantUsageDetailedBinder : ObjectBinder<TeamFoundationDatabaseTenantUsage>
  {
    private SqlColumnBinder TenantPartitionId = new SqlColumnBinder("PartitionId");
    private SqlColumnBinder TenantHostId = new SqlColumnBinder(nameof (TenantHostId));
    private SqlColumnBinder Size = new SqlColumnBinder(nameof (Size));
    private SqlColumnBinder BlobUsage = new SqlColumnBinder(nameof (BlobUsage));

    protected override TeamFoundationDatabaseTenantUsage Bind() => new TeamFoundationDatabaseTenantUsage()
    {
      TenantPartitionId = this.TenantPartitionId.GetInt32((IDataReader) this.Reader),
      TenantHostId = this.TenantHostId.GetGuid((IDataReader) this.Reader),
      Size = this.Size.GetInt64((IDataReader) this.Reader),
      BlobUsage = this.BlobUsage.GetInt64((IDataReader) this.Reader)
    };
  }
}
