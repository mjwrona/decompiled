// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.StorageMigrationColumns
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class StorageMigrationColumns : ObjectBinder<StorageMigration>
  {
    protected SqlColumnBinder MigrationId = new SqlColumnBinder(nameof (MigrationId));
    protected SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
    protected SqlColumnBinder StatusReason = new SqlColumnBinder(nameof (StatusReason));
    protected SqlColumnBinder BlobContainerUri;
    protected SqlColumnBinder ContainerId;
    protected SqlColumnBinder ContainerType;
    private const string c_idName = "ContainerId";
    private const string c_uriName = "BlobContainerUri";
    private const string c_vsoAreaName = "ContainerType";

    public StorageMigrationColumns() => this.InitRenamedColumns();

    protected override StorageMigration Bind() => new StorageMigration()
    {
      MigrationId = this.MigrationId.GetGuid((IDataReader) this.Reader),
      Id = this.ContainerId.GetGuid((IDataReader) this.Reader).ToString("n"),
      Uri = this.BlobContainerUri.GetString((IDataReader) this.Reader, false),
      VsoArea = this.ContainerType.GetString((IDataReader) this.Reader, false),
      Status = (StorageMigrationStatus) this.Status.GetByte((IDataReader) this.Reader),
      StatusReason = this.StatusReason.GetString((IDataReader) this.Reader, true)
    };

    protected virtual void InitRenamedColumns()
    {
      this.ContainerId = new SqlColumnBinder("ContainerId");
      this.BlobContainerUri = new SqlColumnBinder("BlobContainerUri");
      this.ContainerType = new SqlColumnBinder("ContainerType");
    }
  }
}
