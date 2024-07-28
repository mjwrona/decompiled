// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitChangeType
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  [Flags]
  public enum TfsGitChangeType
  {
    Add = 1,
    Edit = 2,
    Delete = 4,
    Rename = 8,
    SourceRename = 16, // 0x00000010
  }
}
