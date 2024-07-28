// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlBackupHeaderColumns
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Configuration
{
  internal class SqlBackupHeaderColumns : ObjectBinder<SqlBackupHeader>
  {
    private SqlColumnBinder m_backupNameColumn = new SqlColumnBinder("BackupName");
    private SqlColumnBinder m_backupDescriptionColumn = new SqlColumnBinder("BackupDescription");
    private SqlColumnBinder m_backupTypeColumn = new SqlColumnBinder("BackupType");
    private SqlColumnBinder m_expirationDateColumn = new SqlColumnBinder("ExpirationDate");
    private SqlColumnBinder m_compressedColumn = new SqlColumnBinder("Compressed");
    private SqlColumnBinder m_userNameColumn = new SqlColumnBinder("UserName");
    private SqlColumnBinder m_positionColumn = new SqlColumnBinder("Position");
    private SqlColumnBinder m_serverNameColumn = new SqlColumnBinder("ServerName");
    private SqlColumnBinder m_databaseNameColumn = new SqlColumnBinder("DatabaseName");
    private SqlColumnBinder m_databaseVersionColumn = new SqlColumnBinder("DatabaseVersion");
    private SqlColumnBinder m_databaseCreationDateColumn = new SqlColumnBinder("DatabaseCreationDate");
    private SqlColumnBinder m_backupSizeColumn = new SqlColumnBinder("BackupSize");
    private SqlColumnBinder m_backupStartDateColumn = new SqlColumnBinder("BackupStartDate");
    private SqlColumnBinder m_backupFinishDateColumn = new SqlColumnBinder("BackupFinishDate");
    private SqlColumnBinder m_compatibilityLevelColumn = new SqlColumnBinder("CompatibilityLevel");
    private SqlColumnBinder m_softwareVersionMajorColumn = new SqlColumnBinder("SoftwareVersionMajor");
    private SqlColumnBinder m_softwareVersionMinorColumn = new SqlColumnBinder("SoftwareVersionMinor");
    private SqlColumnBinder m_softwareVersionBuildColumn = new SqlColumnBinder("SoftwareVersionBuild");
    private SqlColumnBinder m_machineNameColumn = new SqlColumnBinder("MachineName");
    private SqlColumnBinder m_collationColumn = new SqlColumnBinder("Collation");
    private SqlColumnBinder m_isSnapshotColumn = new SqlColumnBinder("IsSnapshot");
    private SqlColumnBinder m_isReadOnlyColumn = new SqlColumnBinder("IsReadOnly");
    private SqlColumnBinder m_isSingleUserColumn = new SqlColumnBinder("IsSingleUser");
    private SqlColumnBinder m_hasBackupChecksumsColumn = new SqlColumnBinder("HasBackupChecksums");
    private SqlColumnBinder m_isDamagedColumn = new SqlColumnBinder("IsDamaged");
    private SqlColumnBinder m_hasIncompleteMetaDataColumn = new SqlColumnBinder("HasIncompleteMetaData");
    private SqlColumnBinder m_isForceOfflineColumn = new SqlColumnBinder("IsForceOffline");
    private SqlColumnBinder m_isCopyOnlyColumn = new SqlColumnBinder("IsCopyOnly");
    private SqlColumnBinder m_backupTypeDescriptionColumn = new SqlColumnBinder("BackupTypeDescription");
    private SqlColumnBinder m_backupSetGUIDColumn = new SqlColumnBinder("BackupSetGUID");
    private SqlColumnBinder m_compressedBackupSizeColumn = new SqlColumnBinder("CompressedBackupSize");
    private readonly string m_backupFilePath;

    public SqlBackupHeaderColumns(string backupFilePath) => this.m_backupFilePath = backupFilePath;

    protected override SqlBackupHeader Bind()
    {
      if (this.m_backupTypeColumn.GetObject((IDataReader) this.Reader) == null)
        throw new InvalidBackupException(ConfigurationResources.MediaIncorrectlyFormed((object) this.m_backupFilePath));
      SqlBackupHeader sqlBackupHeader = new SqlBackupHeader()
      {
        BackupDescription = this.m_backupTypeDescriptionColumn.GetString((IDataReader) this.Reader, true),
        BackupFinishDate = this.m_backupFinishDateColumn.GetDateTime((IDataReader) this.Reader),
        BackupName = this.m_backupNameColumn.GetString((IDataReader) this.Reader, true),
        BackupSetGUID = this.m_backupSetGUIDColumn.GetGuid((IDataReader) this.Reader),
        BackupSize = this.m_backupSizeColumn.GetInt64((IDataReader) this.Reader),
        BackupStartDate = this.m_backupStartDateColumn.GetDateTime((IDataReader) this.Reader),
        BackupType = (BackupType) this.m_backupTypeColumn.GetByte((IDataReader) this.Reader),
        BackupTypeDescription = this.m_backupTypeDescriptionColumn.GetString((IDataReader) this.Reader, (string) null),
        Collation = this.m_collationColumn.GetString((IDataReader) this.Reader, true),
        CompatibilityLevel = (DatabaseCompatibilityLevel) this.m_compatibilityLevelColumn.GetByte((IDataReader) this.Reader),
        SoftwareVersionMajor = this.m_softwareVersionMajorColumn.GetInt32((IDataReader) this.Reader),
        SoftwareVersionMinor = this.m_softwareVersionMinorColumn.GetInt32((IDataReader) this.Reader),
        SoftwareVersionBuild = this.m_softwareVersionBuildColumn.GetInt32((IDataReader) this.Reader),
        CompressedBackupSize = this.m_compressedBackupSizeColumn.GetInt64((IDataReader) this.Reader),
        DatabaseCreationDate = this.m_databaseCreationDateColumn.GetDateTime((IDataReader) this.Reader),
        DatabaseName = this.m_databaseNameColumn.GetString((IDataReader) this.Reader, true),
        DatabaseVersion = this.m_databaseVersionColumn.GetInt32((IDataReader) this.Reader),
        Position = (int) this.m_positionColumn.GetInt16((IDataReader) this.Reader),
        HasBackupChecksums = this.m_hasBackupChecksumsColumn.GetBoolean((IDataReader) this.Reader),
        HasIncompleteMetaData = this.m_hasIncompleteMetaDataColumn.GetBoolean((IDataReader) this.Reader),
        IsCompressed = this.m_compressedColumn.GetByte((IDataReader) this.Reader) > (byte) 0,
        IsCopyOnly = this.m_isCopyOnlyColumn.GetBoolean((IDataReader) this.Reader),
        IsDamaged = this.m_isDamagedColumn.GetBoolean((IDataReader) this.Reader),
        IsForceOffline = this.m_isForceOfflineColumn.GetBoolean((IDataReader) this.Reader),
        IsReadOnly = this.m_isReadOnlyColumn.GetBoolean((IDataReader) this.Reader),
        IsSingleUser = this.m_isSingleUserColumn.GetBoolean((IDataReader) this.Reader),
        IsSnapshot = this.m_isSnapshotColumn.GetBoolean((IDataReader) this.Reader),
        MachineName = this.m_machineNameColumn.GetString((IDataReader) this.Reader, true),
        ServerName = this.m_serverNameColumn.GetString((IDataReader) this.Reader, true),
        UserName = this.m_userNameColumn.GetString((IDataReader) this.Reader, true)
      };
      object obj = this.m_expirationDateColumn.GetObject((IDataReader) this.Reader);
      sqlBackupHeader.ExpirationDate = obj != null ? new DateTime?((DateTime) obj) : new DateTime?();
      return sqlBackupHeader;
    }
  }
}
