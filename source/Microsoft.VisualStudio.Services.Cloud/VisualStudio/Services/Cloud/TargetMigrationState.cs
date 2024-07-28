// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.TargetMigrationState
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public enum TargetMigrationState
  {
    Create = 0,
    Created = 50, // 0x00000032
    BeginCopyJobs = 100, // 0x00000064
    CopyJobsComplete = 200, // 0x000000C8
    BeginCompletePendingBlobs = 300, // 0x0000012C
    CompletePendingBlobs = 400, // 0x00000190
    BeginComplete = 500, // 0x000001F4
    Complete = 600, // 0x00000258
    ResumeBlobCopy = 700, // 0x000002BC
    Failed = 800, // 0x00000320
    BeginRollback = 900, // 0x00000384
    RolledBack = 1000, // 0x000003E8
  }
}
