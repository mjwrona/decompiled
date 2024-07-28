// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.PartitionBinder
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  internal class PartitionBinder : ObjectBinder<Partition>
  {
    private SqlColumnBinder m_isActiveBinder = new SqlColumnBinder("IsActive");
    private SqlColumnBinder m_partitionNumberBinder = new SqlColumnBinder("partition_number");
    private SqlColumnBinder m_columnStoreIndexNameBinder = new SqlColumnBinder("ColumnStoreIndexName");
    private SqlColumnBinder m_tableNameBinder = new SqlColumnBinder("TableName");

    protected override Partition Bind() => new Partition()
    {
      IsActive = this.m_isActiveBinder.ColumnExists((IDataReader) this.Reader) && this.m_isActiveBinder.GetBoolean((IDataReader) this.Reader),
      PartitionId = this.m_partitionNumberBinder.GetInt32((IDataReader) this.Reader),
      ColumnStoreIndexName = this.m_columnStoreIndexNameBinder.GetString((IDataReader) this.Reader, false),
      TableName = this.m_tableNameBinder.GetString((IDataReader) this.Reader, false)
    };
  }
}
