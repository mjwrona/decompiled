// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionedItemPermissions
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [Flags]
  public enum VersionedItemPermissions
  {
    None = 0,
    Read = 1,
    PendChange = 2,
    Checkin = 4,
    Label = 8,
    Lock = 16, // 0x00000010
    ReviseOther = 32, // 0x00000020
    UnlockOther = 64, // 0x00000040
    UndoOther = 128, // 0x00000080
    LabelOther = 256, // 0x00000100
    AdminProjectRights = 1024, // 0x00000400
    CheckinOther = 2048, // 0x00000800
    Merge = 4096, // 0x00001000
    ManageBranch = 8192, // 0x00002000
    All = ManageBranch | Merge | CheckinOther | AdminProjectRights | LabelOther | UndoOther | UnlockOther | ReviseOther | Lock | Label | Checkin | PendChange | Read, // 0x00003DFF
  }
}
