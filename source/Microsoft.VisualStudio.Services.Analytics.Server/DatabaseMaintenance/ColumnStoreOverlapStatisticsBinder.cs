// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.ColumnStoreOverlapStatisticsBinder
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  internal class ColumnStoreOverlapStatisticsBinder : ObjectBinder<ColumnStoreOverlapStatistics>
  {
    private SqlColumnBinder m_DBName = new SqlColumnBinder("DBName");
    private SqlColumnBinder m_PartitionNumber = new SqlColumnBinder("partition_number");
    private SqlColumnBinder m_PartitionId = new SqlColumnBinder("partitionId");
    private SqlColumnBinder m_Overlaps = new SqlColumnBinder("Overlaps");
    private SqlColumnBinder m_SegmentsInPartition = new SqlColumnBinder("SegmentsInPartition");
    private SqlColumnBinder m_TableName = new SqlColumnBinder("TableName");

    protected override ColumnStoreOverlapStatistics Bind() => new ColumnStoreOverlapStatistics()
    {
      DBName = this.m_DBName.GetString((IDataReader) this.Reader, true),
      PartitionNumber = this.m_PartitionNumber.GetInt32((IDataReader) this.Reader),
      PartitionId = !this.m_PartitionId.ColumnExists((IDataReader) this.Reader) || this.m_PartitionId.IsNull((IDataReader) this.Reader) ? -1 : this.m_PartitionId.GetInt32((IDataReader) this.Reader),
      TableName = !this.m_TableName.ColumnExists((IDataReader) this.Reader) || this.m_TableName.IsNull((IDataReader) this.Reader) ? "AnalyticsModel.tbl_WorkItemHistory" : this.m_TableName.GetString((IDataReader) this.Reader, true),
      Overlaps = this.m_Overlaps.GetInt32((IDataReader) this.Reader),
      SegmentsInPartition = this.m_SegmentsInPartition.GetInt64((IDataReader) this.Reader)
    };
  }
}
