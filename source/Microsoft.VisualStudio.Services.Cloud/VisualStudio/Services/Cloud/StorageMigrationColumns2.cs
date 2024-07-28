// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.StorageMigrationColumns2
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class StorageMigrationColumns2 : StorageMigrationColumns
  {
    protected override StorageMigration Bind() => new StorageMigration()
    {
      MigrationId = this.MigrationId.GetGuid((IDataReader) this.Reader),
      Id = this.ContainerId.GetString((IDataReader) this.Reader, false),
      Uri = this.BlobContainerUri.GetString((IDataReader) this.Reader, false),
      VsoArea = this.ContainerType.GetString((IDataReader) this.Reader, false),
      Status = (StorageMigrationStatus) this.Status.GetByte((IDataReader) this.Reader),
      StatusReason = this.StatusReason.GetString((IDataReader) this.Reader, true)
    };
  }
}
