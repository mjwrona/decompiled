// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance.PartitionToSplitBinder
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.DatabaseMaintenance
{
  internal class PartitionToSplitBinder : ObjectBinder<PartitionToSplit>
  {
    private SqlColumnBinder m_DBName = new SqlColumnBinder("DBName");
    private SqlColumnBinder m_PartitionId = new SqlColumnBinder("PartitionId");
    private SqlColumnBinder m_NOOfRecords = new SqlColumnBinder("NOOFRecords");
    private SqlColumnBinder m_BigPartition = new SqlColumnBinder("BigPartition");
    private SqlColumnBinder m_NextIsBig = new SqlColumnBinder("NextIsBig");
    private SqlColumnBinder m_SchemeName = new SqlColumnBinder("SchemeName");

    protected override PartitionToSplit Bind() => new PartitionToSplit()
    {
      PartitionId = this.m_PartitionId.GetInt32((IDataReader) this.Reader),
      DBName = this.m_DBName.GetString((IDataReader) this.Reader, true),
      NOOFRecords = this.m_NOOfRecords.GetInt64((IDataReader) this.Reader),
      BigPartition = this.m_BigPartition.GetBoolean((IDataReader) this.Reader),
      NextIsBig = this.m_NextIsBig.GetBoolean((IDataReader) this.Reader),
      SchemeName = !this.m_SchemeName.ColumnExists((IDataReader) this.Reader) || this.m_SchemeName.IsNull((IDataReader) this.Reader) ? "scheme_AnalyticsWorkItem" : this.m_SchemeName.GetString((IDataReader) this.Reader, true)
    };
  }
}
