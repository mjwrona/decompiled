// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.SupportedFeatures
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [Flags]
  public enum SupportedFeatures
  {
    None = 0,
    GetLatestOnCheckout = 1,
    OneLevelMapping = 2,
    Destroy = 4,
    CreateBranch = 8,
    GetChangesForChangeset = 16, // 0x00000010
    ProxySuite = 32, // 0x00000020
    LocalVersions = 64, // 0x00000040
    BatchedCheckins = 256, // 0x00000100
    WorkspacePermissions = 512, // 0x00000200
    CheckinDates = 1024, // 0x00000400
    OwnerVSID = 2048, // 0x00000800
    All = OwnerVSID | CheckinDates | WorkspacePermissions | BatchedCheckins | LocalVersions | ProxySuite | GetChangesForChangeset | CreateBranch | Destroy | OneLevelMapping | GetLatestOnCheckout, // 0x00000F7F
  }
}
