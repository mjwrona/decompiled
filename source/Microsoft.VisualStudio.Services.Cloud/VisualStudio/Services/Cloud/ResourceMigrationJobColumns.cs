// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ResourceMigrationJobColumns
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ResourceMigrationJobColumns : ObjectBinder<ResourceMigrationJob>
  {
    private SqlColumnBinder MigrationId = new SqlColumnBinder(nameof (MigrationId));
    private SqlColumnBinder JobId = new SqlColumnBinder(nameof (JobId));
    private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
    private SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
    private SqlColumnBinder RetriesRemaining = new SqlColumnBinder(nameof (RetriesRemaining));

    protected override ResourceMigrationJob Bind() => new ResourceMigrationJob()
    {
      MigrationId = this.MigrationId.GetGuid((IDataReader) this.Reader),
      JobId = this.JobId.GetGuid((IDataReader) this.Reader),
      Name = this.Name.GetString((IDataReader) this.Reader, true),
      Status = (ResourceMigrationState) this.Status.GetByte((IDataReader) this.Reader),
      RetriesRemaining = this.RetriesRemaining.GetInt32((IDataReader) this.Reader, 0)
    };
  }
}
