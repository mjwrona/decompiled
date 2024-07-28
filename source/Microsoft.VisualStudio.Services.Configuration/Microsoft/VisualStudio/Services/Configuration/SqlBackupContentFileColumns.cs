// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlBackupContentFileColumns
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Configuration
{
  internal class SqlBackupContentFileColumns : ObjectBinder<SqlBackupContentFile>
  {
    private SqlColumnBinder m_logicalNameColumn = new SqlColumnBinder("LogicalName");
    private SqlColumnBinder m_physicalNameColumn = new SqlColumnBinder("PhysicalName");
    private SqlColumnBinder m_typeColumn = new SqlColumnBinder("Type");
    private SqlColumnBinder m_fileGroupNameColumn = new SqlColumnBinder("FileGroupName");
    private SqlColumnBinder m_sizeColumn = new SqlColumnBinder("Size");
    private SqlColumnBinder m_maxSizeColumn = new SqlColumnBinder("MaxSize");
    private SqlColumnBinder m_fileIdColumn = new SqlColumnBinder("FileId");
    private SqlColumnBinder m_uniqueIdColumn = new SqlColumnBinder("UniqueId");
    private SqlColumnBinder m_backupSizeInBytesColumn = new SqlColumnBinder("BackupSizeInBytes");
    private SqlColumnBinder m_sourceBlockSizeColumn = new SqlColumnBinder("SourceBlockSize");
    private SqlColumnBinder m_fileGroupIdColumn = new SqlColumnBinder("FileGroupId");
    private SqlColumnBinder m_isReadOnlyColumn = new SqlColumnBinder("IsReadOnly");
    private SqlColumnBinder m_isPresentColumn = new SqlColumnBinder("IsPresent");

    protected override SqlBackupContentFile Bind()
    {
      SqlBackupContentFile backupContentFile = new SqlBackupContentFile()
      {
        BackupSizeInBytes = this.m_backupSizeInBytesColumn.GetInt64((IDataReader) this.Reader),
        FileGroupId = this.m_fileGroupIdColumn.GetInt32((IDataReader) this.Reader),
        FileGroupName = this.m_fileGroupNameColumn.GetString((IDataReader) this.Reader, true),
        FileId = this.m_fileIdColumn.GetInt64((IDataReader) this.Reader),
        IsPresent = this.m_isPresentColumn.GetBoolean((IDataReader) this.Reader),
        IsReadOnly = this.m_isReadOnlyColumn.GetBoolean((IDataReader) this.Reader),
        LogicalName = this.m_logicalNameColumn.GetString((IDataReader) this.Reader, true),
        MaxSize = this.m_maxSizeColumn.GetInt64((IDataReader) this.Reader),
        PhysicalName = this.m_physicalNameColumn.GetString((IDataReader) this.Reader, true),
        Size = this.m_sizeColumn.GetInt64((IDataReader) this.Reader),
        SourceBlockSize = this.m_sourceBlockSizeColumn.GetInt32((IDataReader) this.Reader),
        UniqueId = this.m_uniqueIdColumn.GetGuid((IDataReader) this.Reader)
      };
      string str = this.m_typeColumn.GetString((IDataReader) this.Reader, false);
      backupContentFile.FileType = !str.Equals("D", StringComparison.OrdinalIgnoreCase) ? (!str.Equals("L", StringComparison.OrdinalIgnoreCase) ? DatabaseFileType.FullTextCatalog : DatabaseFileType.LogFile) : DatabaseFileType.DataFile;
      return backupContentFile;
    }
  }
}
