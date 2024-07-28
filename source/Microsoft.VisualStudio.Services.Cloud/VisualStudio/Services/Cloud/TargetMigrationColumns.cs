// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.TargetMigrationColumns
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class TargetMigrationColumns : ObjectBinder<TargetHostMigration>
  {
    protected SqlColumnBinder StorageOnlyColumn;
    private SqlColumnBinder MigrationIdColumn = new SqlColumnBinder("MigrationId");
    private SqlColumnBinder ParentMigrationIdColumn = new SqlColumnBinder("ParentMigrationId");
    private SqlColumnBinder HostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder SourceDatabaseIdColumn = new SqlColumnBinder("SourceDatabaseId");
    private SqlColumnBinder TargetDatabaseIdColumn = new SqlColumnBinder("TargetDatabaseId");
    private SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder StatusMessageColumn = new SqlColumnBinder("StatusMessage");
    private SqlColumnBinder StatusChangedDateColumn = new SqlColumnBinder("StatusChangedDate");
    private SqlColumnBinder CreatedDateColumn = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder StorageAccountIdColumn = new SqlColumnBinder("StorageAccountId");
    private SqlColumnBinder SourceServiceInstanceIdColumn = new SqlColumnBinder("SourceServiceInstanceId");
    private SqlColumnBinder HostTypeColumn = new SqlColumnBinder("HostType");
    private SqlColumnBinder OnlineBlobCopyColumn = new SqlColumnBinder("OnlineBlobCopy");
    private SqlColumnBinder OptionsColumn = new SqlColumnBinder("Options");

    public TargetMigrationColumns() => this.InitRenamedColumns();

    protected override TargetHostMigration Bind()
    {
      TargetHostMigration targetHostMigration = new TargetHostMigration();
      targetHostMigration.MigrationId = this.MigrationIdColumn.GetGuid((IDataReader) this.Reader);
      targetHostMigration.ParentMigrationId = this.ParentMigrationIdColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      targetHostMigration.HostId = this.HostIdColumn.GetGuid((IDataReader) this.Reader);
      targetHostMigration.SourceDatabaseId = this.SourceDatabaseIdColumn.GetInt32((IDataReader) this.Reader, DatabaseManagementConstants.InvalidDatabaseId);
      targetHostMigration.TargetDatabaseId = this.TargetDatabaseIdColumn.GetInt32((IDataReader) this.Reader, DatabaseManagementConstants.InvalidDatabaseId);
      targetHostMigration.State = (TargetMigrationState) this.StatusColumn.GetInt32((IDataReader) this.Reader);
      targetHostMigration.StatusMessage = this.StatusMessageColumn.GetString((IDataReader) this.Reader, true);
      targetHostMigration.StatusChangedDate = this.StatusChangedDateColumn.GetDateTime((IDataReader) this.Reader);
      targetHostMigration.CreatedDate = this.CreatedDateColumn.GetDateTime((IDataReader) this.Reader);
      targetHostMigration.StorageOnly = this.StorageOnlyColumn.GetBoolean((IDataReader) this.Reader);
      targetHostMigration.StorageAccountId = this.StorageAccountIdColumn.GetInt32((IDataReader) this.Reader, -1);
      targetHostMigration.SourceServiceInstanceId = this.SourceServiceInstanceIdColumn.GetGuid((IDataReader) this.Reader);
      targetHostMigration.HostType = (TeamFoundationHostType) this.HostTypeColumn.GetInt32((IDataReader) this.Reader, 0, 0);
      targetHostMigration.OnlineBlobCopy = this.OnlineBlobCopyColumn.GetBoolean((IDataReader) this.Reader, false, false);
      HostMigrationOptions migrationOptions = targetHostMigration.OnlineBlobCopy ? HostMigrationOptions.OnlineBlobCopy : HostMigrationOptions.None;
      targetHostMigration.Options = (HostMigrationOptions) this.OptionsColumn.GetByte((IDataReader) this.Reader, (byte) migrationOptions, (byte) migrationOptions);
      return targetHostMigration;
    }

    protected virtual void InitRenamedColumns() => this.StorageOnlyColumn = new SqlColumnBinder("BlobOnly");
  }
}
