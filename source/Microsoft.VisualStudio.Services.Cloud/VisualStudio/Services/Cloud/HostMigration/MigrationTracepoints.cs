// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.MigrationTracepoints
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public static class MigrationTracepoints
  {
    public const int BlobMigrationJob = 197000;
    public const int BlobMigrationJob_Monitor = 197001;
    public const int BlobMigrationJob_StatusChange = 197002;
    public const int TfsBlobMigrationJob = 197050;
    public const int TfsBlobMigrationJob_Monitor = 197051;
    public const int TfsBlobMigrationJob_StatusChange = 197052;
    public const int BlobCopyUtil_Monitor = 197101;
    public const int HostMigrationStepPerformer = 197200;
    public const int HostMigrationStepPerformer_Monitor = 197201;
    public const int ParallelBlobMigrationCoordinatingJob = 197301;
    public const int ParallelBlobMetadataTableMigrationCoordinatingJob = 197400;
    public const int AzureTableMigrator = 197500;
    public const int ArtifactMigrationStepPerformer = 197600;
    public const int ParallelArtifactTableMigrationCoordinationJob = 197700;
    public const int TableMigrationRunner = 197800;
    public const int ReverseBlobMigrationJob = 197900;
    public const int ReverseBlobMigrationJob_Queue = 197901;
    public const int ReverseBlobMigrationJob_Monitor = 197900;
  }
}
