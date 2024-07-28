// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SourceMigrationColumns
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class SourceMigrationColumns : ObjectBinder<SourceHostMigration>
  {
    protected SqlColumnBinder StorageOnlyColumn;
    private SqlColumnBinder MigrationIdColumn = new SqlColumnBinder("MigrationId");
    private SqlColumnBinder ParentMigrationIdColumn = new SqlColumnBinder("ParentMigrationId");
    private SqlColumnBinder HostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder StatusMessageColumn = new SqlColumnBinder("StatusMessage");
    private SqlColumnBinder StatusChangedDateColumn = new SqlColumnBinder("StatusChangedDate");
    private SqlColumnBinder CreatedDateColumn = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder TargetInstanceIdColumn = new SqlColumnBinder("TargetServiceInstanceId");
    private SqlColumnBinder CredentialIdColumn = new SqlColumnBinder("CredentialId");
    private SqlColumnBinder HostTypeColumn = new SqlColumnBinder("HostType");
    private SqlColumnBinder OnlineBlobCopyColumn = new SqlColumnBinder("OnlineBlobCopy");
    private SqlColumnBinder OptionsColumn = new SqlColumnBinder("Options");

    public SourceMigrationColumns() => this.InitRenamedColumns();

    protected override SourceHostMigration Bind()
    {
      SourceHostMigration sourceHostMigration = new SourceHostMigration();
      sourceHostMigration.MigrationId = this.MigrationIdColumn.GetGuid((IDataReader) this.Reader);
      sourceHostMigration.ParentMigrationId = this.ParentMigrationIdColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      sourceHostMigration.HostId = this.HostIdColumn.GetGuid((IDataReader) this.Reader);
      sourceHostMigration.State = (SourceMigrationState) this.StatusColumn.GetInt32((IDataReader) this.Reader);
      sourceHostMigration.StatusMessage = this.StatusMessageColumn.GetString((IDataReader) this.Reader, true);
      sourceHostMigration.StatusChangedDate = this.StatusChangedDateColumn.GetDateTime((IDataReader) this.Reader);
      sourceHostMigration.CreatedDate = this.CreatedDateColumn.GetDateTime((IDataReader) this.Reader);
      sourceHostMigration.StorageOnly = this.StorageOnlyColumn.GetBoolean((IDataReader) this.Reader);
      sourceHostMigration.TargetServiceInstanceId = this.TargetInstanceIdColumn.GetGuid((IDataReader) this.Reader);
      sourceHostMigration.CredentialId = this.CredentialIdColumn.GetInt32((IDataReader) this.Reader, 0);
      sourceHostMigration.HostType = (TeamFoundationHostType) this.HostTypeColumn.GetInt32((IDataReader) this.Reader, 0, 0);
      sourceHostMigration.OnlineBlobCopy = this.OnlineBlobCopyColumn.GetBoolean((IDataReader) this.Reader, false, false);
      HostMigrationOptions migrationOptions = sourceHostMigration.OnlineBlobCopy ? HostMigrationOptions.OnlineBlobCopy : HostMigrationOptions.None;
      sourceHostMigration.Options = (HostMigrationOptions) this.OptionsColumn.GetByte((IDataReader) this.Reader, (byte) migrationOptions, (byte) migrationOptions);
      return sourceHostMigration;
    }

    protected virtual void InitRenamedColumns() => this.StorageOnlyColumn = new SqlColumnBinder("BlobOnly");
  }
}
