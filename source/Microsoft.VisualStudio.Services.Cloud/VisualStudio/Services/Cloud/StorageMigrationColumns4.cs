// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.StorageMigrationColumns4
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class StorageMigrationColumns4 : ObjectBinder<StorageMigration>
  {
    protected SqlColumnBinder MigrationId = new SqlColumnBinder(nameof (MigrationId));
    protected SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
    protected SqlColumnBinder Uri = new SqlColumnBinder(nameof (Uri));
    protected SqlColumnBinder VsoArea = new SqlColumnBinder(nameof (VsoArea));
    protected SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
    protected SqlColumnBinder StatusReason = new SqlColumnBinder(nameof (StatusReason));
    protected SqlColumnBinder StorageType = new SqlColumnBinder(nameof (StorageType));
    protected SqlColumnBinder ShardIndex = new SqlColumnBinder(nameof (ShardIndex));
    protected SqlColumnBinder FilterKey = new SqlColumnBinder(nameof (FilterKey));

    protected override StorageMigration Bind()
    {
      StorageMigration storageMigration = new StorageMigration();
      storageMigration.MigrationId = this.MigrationId.GetGuid((IDataReader) this.Reader);
      storageMigration.Id = this.Id.GetString((IDataReader) this.Reader, false);
      storageMigration.Uri = this.Uri.GetString((IDataReader) this.Reader, false);
      storageMigration.VsoArea = this.VsoArea.GetString((IDataReader) this.Reader, false);
      storageMigration.Status = (StorageMigrationStatus) this.Status.GetByte((IDataReader) this.Reader);
      storageMigration.StatusReason = this.StatusReason.GetString((IDataReader) this.Reader, true);
      storageMigration.StorageType = (Microsoft.VisualStudio.Services.Cloud.StorageType) this.StorageType.GetByte((IDataReader) this.Reader);
      storageMigration.FilterKey = this.FilterKey.GetString((IDataReader) this.Reader, true);
      int int32 = this.ShardIndex.GetInt32((IDataReader) this.Reader, -1);
      if (int32 > -1)
      {
        storageMigration.IsSharded = true;
        storageMigration.ShardIndex = new int?(int32);
      }
      return storageMigration;
    }
  }
}
