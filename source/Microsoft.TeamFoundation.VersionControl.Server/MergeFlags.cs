// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.MergeFlags
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Flags]
  public enum MergeFlags
  {
    None = 0,
    Edit = 1,
    Add = 2,
    AcceptTheirs = 4,
    AcceptMine = 8,
    AcceptMerged = 16, // 0x00000010
    Deleted = 32, // 0x00000020
    TransitiveMergeRecord = 64, // 0x00000040
    DeleteConflict = 128, // 0x00000080
    AcceptYoursRenameTheirs = 256, // 0x00000100
  }
}
