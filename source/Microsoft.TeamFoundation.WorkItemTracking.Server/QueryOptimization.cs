// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryOptimization
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Flags]
  public enum QueryOptimization
  {
    None = 0,
    ForceOrder = 1,
    ForceCustomTablePK = 2,
    FullTextSearchResultInTempTable = 4,
    FullTextJoinForceOrder = 8,
    DisableNonClusteredColumnstoreIndex = 16, // 0x00000010
    ForceFullTextIndex = 32, // 0x00000020
    DoNotForceFullTextIndex = 64, // 0x00000040
    MoveOrClauseUp = 128, // 0x00000080
  }
}
