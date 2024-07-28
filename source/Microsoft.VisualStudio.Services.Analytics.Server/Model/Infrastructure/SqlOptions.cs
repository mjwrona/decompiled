// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.SqlOptions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  [Flags]
  internal enum SqlOptions
  {
    None = 0,
    UnknownPartitionId = 1,
    SnapshotJoin = 2,
    DisableAdaptiveJoin = 8,
    ForcePartitionFilter = 32, // 0x00000020
    TestResultJoinOptimization = 64, // 0x00000040
    TestResultRecompile = 128, // 0x00000080
    AssumeJoinPredicateDependsOnFilter = 256, // 0x00000100
    HashJoinFilterHint = 512, // 0x00000200
    HashJoinForBurnDownHint = 1024, // 0x00000400
    NoHintViewForRollup = 2048, // 0x00000800
    LoopJoinForRollupHint = 4096, // 0x00001000
  }
}
