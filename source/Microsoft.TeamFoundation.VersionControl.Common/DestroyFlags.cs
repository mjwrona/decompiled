// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.DestroyFlags
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [Flags]
  public enum DestroyFlags
  {
    None = 0,
    Preview = 1,
    StartCleanup = 2,
    KeepHistory = 4,
    Silent = 8,
    AffectedChanges = 16, // 0x00000010
    DeleteWorkspaceState = 32, // 0x00000020
  }
}
