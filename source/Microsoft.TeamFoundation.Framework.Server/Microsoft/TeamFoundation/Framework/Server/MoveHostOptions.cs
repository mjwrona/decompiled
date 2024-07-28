// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MoveHostOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  public enum MoveHostOptions
  {
    None = 0,
    DeleteSourceDatabase = 1,
    IgnoreDatabaseType = 2,
    CopyCommonTablesOnly = 4,
    SourceReadOnly = 8,
    AcquirePartition = 16, // 0x00000010
    ServiceLevelCompareForImport = 32, // 0x00000020
    Partial = 64, // 0x00000040
    Resumable = 128, // 0x00000080
    UseSourceHostId = 256, // 0x00000100
    IncrementMaxTenantsIfFull = 512, // 0x00000200
    CleanupOnlyTargetedTables = 1024, // 0x00000400
    SkipSizeCheck = 2048, // 0x00000800
    SinglePartitionDb = 4096, // 0x00001000
    PerformValidation = 8192, // 0x00002000
    DoNotCopyTableData = 16384, // 0x00004000
    UseReadOnlyMode = 32768, // 0x00008000
  }
}
