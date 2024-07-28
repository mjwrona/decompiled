// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ResourceMigrationJob
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ResourceMigrationJob
  {
    public Guid MigrationId { get; set; }

    public Guid JobId { get; set; }

    public string Name { get; set; }

    public ResourceMigrationState Status { get; set; }

    public MigrationJobStage JobStage { get; set; }

    public int RetriesRemaining { get; set; }

    public override bool Equals(object obj) => obj is ResourceMigrationJob resourceMigrationJob && this.MigrationId == resourceMigrationJob.MigrationId && this.JobId == resourceMigrationJob.JobId && this.Name.Equals(resourceMigrationJob.Name, StringComparison.Ordinal);

    public override int GetHashCode() => ((17 * 17 + this.MigrationId.GetHashCode()) * 17 + this.JobId.GetHashCode()) * 17 + (this.Name != null ? this.Name.GetHashCode() : 0);

    public override string ToString() => string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7}, {8}: {9}, {10}: {11}", (object) "MigrationId", (object) this.MigrationId, (object) "JobId", (object) this.JobId, (object) "Name", (object) this.Name, (object) "Status", (object) this.Status, (object) "RetriesRemaining", (object) this.RetriesRemaining, (object) "JobStage", (object) this.JobStage);
  }
}
