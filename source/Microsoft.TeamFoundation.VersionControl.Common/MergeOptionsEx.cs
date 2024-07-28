// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.MergeOptionsEx
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [Flags]
  public enum MergeOptionsEx
  {
    None = 0,
    ForceMerge = 1,
    Baseless = 2,
    NoMerge = 16, // 0x00000010
    AlwaysAcceptMine = 32, // 0x00000020
    Silent = 64, // 0x00000040
    NoImplicitBaseless = 128, // 0x00000080
    Conservative = 256, // 0x00000100
    NoAutoResolve = 512, // 0x00000200
  }
}
