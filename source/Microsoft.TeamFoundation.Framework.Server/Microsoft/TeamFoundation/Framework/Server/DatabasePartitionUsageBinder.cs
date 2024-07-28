// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionUsageBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartitionUsageBinder : ObjectBinder<TeamFoundationDatabasePartitionUsage>
  {
    private SqlColumnBinder m_tenantPartitionIdColumn = new SqlColumnBinder("TenantPartitionId");
    private SqlColumnBinder m_partitionReservedSpaceMBColumn = new SqlColumnBinder("PartitionReservedSpaceMB");
    private SqlColumnBinder m_partitionUsedSpaceMBColumn = new SqlColumnBinder("PartitionUsedSpaceMB");
    private SqlColumnBinder m_partitionBlobSizeMBColumn = new SqlColumnBinder("PartitionBlobSizeMB");

    protected override TeamFoundationDatabasePartitionUsage Bind() => new TeamFoundationDatabasePartitionUsage()
    {
      TenantPartitionId = this.m_tenantPartitionIdColumn.GetInt32((IDataReader) this.Reader),
      PartitionReservedSpaceMB = this.m_partitionReservedSpaceMBColumn.GetDouble((IDataReader) this.Reader),
      PartitionUsedSpaceMB = this.m_partitionUsedSpaceMBColumn.GetDouble((IDataReader) this.Reader),
      PartitionBlobSizeMB = this.m_partitionBlobSizeMBColumn.GetDouble((IDataReader) this.Reader)
    };
  }
}
