// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.ColumnStoreFragmentationStatisticRowBinder
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  internal class ColumnStoreFragmentationStatisticRowBinder : 
    ObjectBinder<ColumnStoreFragmentationStatisticRow>
  {
    private SqlColumnBinder m_dbNameBinder = new SqlColumnBinder("DBName");
    private SqlColumnBinder m_actionBinder = new SqlColumnBinder("Action");
    private SqlColumnBinder m_nameBinder = new SqlColumnBinder("name");
    private SqlColumnBinder m_partitionNumberBinder = new SqlColumnBinder("partition_number");
    private SqlColumnBinder m_rowGroupIdBinder = new SqlColumnBinder("row_group_id");
    private SqlColumnBinder m_totalRowsBinder = new SqlColumnBinder("total_rows");
    private SqlColumnBinder m_deletedRowsBinder = new SqlColumnBinder("deleted_rows");
    private SqlColumnBinder m_sizeInBytesBinder = new SqlColumnBinder("size_in_bytes");
    private SqlColumnBinder m_stateDescBinder = new SqlColumnBinder("state_desc");
    private SqlColumnBinder m_trimReasonDescBinder = new SqlColumnBinder("trim_reason_desc");
    private SqlColumnBinder m_transitionToCompressedStateDescBinder = new SqlColumnBinder("transition_to_compressed_state_desc");
    private SqlColumnBinder m_fragmentationBinder = new SqlColumnBinder("fragmentation");

    protected override ColumnStoreFragmentationStatisticRow Bind() => new ColumnStoreFragmentationStatisticRow()
    {
      DBName = this.m_dbNameBinder.GetString((IDataReader) this.Reader, false),
      Action = this.m_actionBinder.GetString((IDataReader) this.Reader, false),
      Name = this.m_nameBinder.GetString((IDataReader) this.Reader, false),
      PartitionId = this.m_partitionNumberBinder.GetInt32((IDataReader) this.Reader),
      RowGroupId = this.m_rowGroupIdBinder.GetInt32((IDataReader) this.Reader),
      TotalRows = this.m_totalRowsBinder.GetInt64((IDataReader) this.Reader),
      DeletedRows = this.m_deletedRowsBinder.GetInt64((IDataReader) this.Reader),
      SizeInBytes = this.m_sizeInBytesBinder.GetInt64((IDataReader) this.Reader),
      State = this.m_stateDescBinder.GetString((IDataReader) this.Reader, false),
      TrimReason = this.m_trimReasonDescBinder.GetString((IDataReader) this.Reader, true),
      TransToCompressedDesc = this.m_transitionToCompressedStateDescBinder.GetString((IDataReader) this.Reader, true),
      Fragmentation = this.m_fragmentationBinder.GetInt32((IDataReader) this.Reader)
    };
  }
}
