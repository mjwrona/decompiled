// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseFileColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseFileColumns : ObjectBinder<DatabaseFileProperties>
  {
    private SqlColumnBinder m_fileIdColumn = new SqlColumnBinder("file_id");
    private SqlColumnBinder m_typeColumn = new SqlColumnBinder("type");
    private SqlColumnBinder m_dataSpaceIdColumn = new SqlColumnBinder("data_space_id");
    private SqlColumnBinder m_nameColumn = new SqlColumnBinder("name");
    private SqlColumnBinder m_physicalNameColumn = new SqlColumnBinder("physical_name");
    private SqlColumnBinder m_stateColumn = new SqlColumnBinder("state");
    private SqlColumnBinder m_sizeColumn = new SqlColumnBinder("size");
    private SqlColumnBinder m_maxSizeColumn = new SqlColumnBinder("max_size");
    private SqlColumnBinder m_growthColumn = new SqlColumnBinder("growth");
    private SqlColumnBinder m_isMediaReadOnlyColumn = new SqlColumnBinder("is_media_read_only");
    private SqlColumnBinder m_isReadOnlyColumn = new SqlColumnBinder("is_read_only");
    private SqlColumnBinder m_isSparseColumn = new SqlColumnBinder("is_sparse");
    private SqlColumnBinder m_isPercentGrowthColumn = new SqlColumnBinder("is_percent_growth");
    private SqlColumnBinder m_fileGroupNameColumn = new SqlColumnBinder("file_group_name");
    private SqlColumnBinder m_inDefaultFileGroupColumn = new SqlColumnBinder("in_default_file_group");

    protected override DatabaseFileProperties Bind() => new DatabaseFileProperties()
    {
      FileId = this.m_fileIdColumn.GetInt32((IDataReader) this.Reader),
      FileType = (DatabaseFileType) this.m_typeColumn.GetByte((IDataReader) this.Reader),
      DataSpaceId = this.m_dataSpaceIdColumn.GetInt32((IDataReader) this.Reader),
      LogicalName = this.m_nameColumn.GetString((IDataReader) this.Reader, false),
      PhysicalName = this.m_physicalNameColumn.GetString((IDataReader) this.Reader, false),
      State = (DatabaseFileState) this.m_stateColumn.GetByte((IDataReader) this.Reader),
      SizePages = this.m_sizeColumn.GetInt32((IDataReader) this.Reader),
      MaxSizePages = this.m_maxSizeColumn.GetInt32((IDataReader) this.Reader),
      Growth = this.m_growthColumn.GetInt32((IDataReader) this.Reader),
      IsMediaReadOnly = this.m_isMediaReadOnlyColumn.GetBoolean((IDataReader) this.Reader),
      IsReadOnly = this.m_isReadOnlyColumn.GetBoolean((IDataReader) this.Reader),
      IsSparse = this.m_isSparseColumn.GetBoolean((IDataReader) this.Reader),
      IsPercentGrowth = this.m_isPercentGrowthColumn.GetBoolean((IDataReader) this.Reader),
      FileGroupName = this.m_fileGroupNameColumn.GetString((IDataReader) this.Reader, (string) null),
      InDefaultFileGroup = this.m_inDefaultFileGroupColumn.GetBoolean((IDataReader) this.Reader, false, false)
    };
  }
}
