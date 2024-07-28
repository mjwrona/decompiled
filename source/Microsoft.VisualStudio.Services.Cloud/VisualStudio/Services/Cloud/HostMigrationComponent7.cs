// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationComponent7
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationComponent7 : HostMigrationComponent6
  {
    public override void CreateResourceMigrationJob(ResourceMigrationJob resourceMigrationJob)
    {
      this.PrepareStoredProcedure("Migration.prc_CreateResourceMigrationJob");
      this.BindGuid("@migrationId", resourceMigrationJob.MigrationId);
      this.BindGuid("@jobId", resourceMigrationJob.JobId);
      this.BindString("@name", resourceMigrationJob.Name, 100, true, SqlDbType.VarChar);
      this.BindByte("@status", (byte) resourceMigrationJob.Status);
      this.BindInt("@RetriesRemaining", resourceMigrationJob.RetriesRemaining);
      this.BindInt("@jobStage", (int) resourceMigrationJob.JobStage);
      this.ExecuteNonQuery();
    }

    protected override ResourceMigrationJobColumns CreateResourceMigrationJobColumns() => (ResourceMigrationJobColumns) new ResourceMigrationJobColumns2();
  }
}
