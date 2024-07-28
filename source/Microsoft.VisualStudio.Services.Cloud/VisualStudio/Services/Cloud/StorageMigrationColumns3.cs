// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.StorageMigrationColumns3
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class StorageMigrationColumns3 : StorageMigrationColumns2
  {
    private const string c_idName = "Id";
    private const string c_uriName = "Uri";
    private const string c_vsoAreaName = "VsoArea";
    protected SqlColumnBinder StorageType = new SqlColumnBinder(nameof (StorageType));
    protected SqlColumnBinder ShardIndex = new SqlColumnBinder(nameof (ShardIndex));
    protected SqlColumnBinder FilterKey = new SqlColumnBinder(nameof (FilterKey));

    protected override StorageMigration Bind()
    {
      StorageMigration storageMigration = base.Bind();
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

    protected override void InitRenamedColumns()
    {
      this.ContainerId = new SqlColumnBinder("Id");
      this.BlobContainerUri = new SqlColumnBinder("Uri");
      this.ContainerType = new SqlColumnBinder("VsoArea");
    }
  }
}
