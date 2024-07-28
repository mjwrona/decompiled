// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SourceMigrationState
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public enum SourceMigrationState
  {
    BeginCreate = 0,
    Created = 100, // 0x00000064
    BeginPrepareDatabase = 150, // 0x00000096
    PrepareDatabase = 200, // 0x000000C8
    BeginPrepareBlobs = 250, // 0x000000FA
    PrepareBlobs = 300, // 0x0000012C
    BeginUpdateLocation = 400, // 0x00000190
    UpdatedLocation = 500, // 0x000001F4
    BeginComplete = 600, // 0x00000258
    Complete = 700, // 0x000002BC
    Failed = 800, // 0x00000320
    BeginRollback = 900, // 0x00000384
    RolledBack = 1000, // 0x000003E8
  }
}
