// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.MigrationJobStage
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public enum MigrationJobStage
  {
    Target_Resources = 0,
    Target_CreateTargetMigration = 5,
    Target_CleanupMigrationOnTarget = 10, // 0x0000000A
    Target_FinalizeMigrationOnTarget = 20, // 0x00000014
    Source_CreateSourceMigration = 100, // 0x00000064
    Source_PrepareSourceDatabase = 105, // 0x00000069
    Source_PrepareBlobs = 107, // 0x0000006B
    Source_UpdateLocation = 110, // 0x0000006E
    Source_FinalizeMigrationOnSource = 120, // 0x00000078
    Source_FinalizeMigrationOnSource_Rollback = 130, // 0x00000082
    Rollback_StopMigrationJobs = 1000, // 0x000003E8
  }
}
