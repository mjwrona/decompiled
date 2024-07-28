// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryCategory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Flags]
  public enum QueryCategory : long
  {
    None = 0,
    ChartingAsOfQuery = 1,
    IdentityComparisonQuery = 2,
    IdentityInGroupQuery = 4,
    FullTextSearchQuery = 8,
    CustomLatestTableQuery = 16, // 0x0000000000000010
    WasEverQuery = 32, // 0x0000000000000020
    LowerLevelOrClauseQuery = 64, // 0x0000000000000040
  }
}
