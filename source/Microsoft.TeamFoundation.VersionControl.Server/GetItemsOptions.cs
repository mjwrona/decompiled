// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.GetItemsOptions
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Flags]
  internal enum GetItemsOptions
  {
    None = 0,
    Download = 1,
    Unsorted = 2,
    IncludeBranchInfo = 4,
    IncludeSourceRenames = 8,
    LocalOnly = 16, // 0x00000010
    IncludeRecursiveDeletes = 32, // 0x00000020
  }
}
