// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.GlobalPermissions
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [Flags]
  public enum GlobalPermissions
  {
    None = 0,
    CreateWorkspace = 2,
    AdminWorkspaces = 4,
    AdminShelvesets = 8,
    AdminConnections = 16, // 0x00000010
    AdminConfiguration = 32, // 0x00000020
    All = AdminConfiguration | AdminConnections | AdminShelvesets | AdminWorkspaces | CreateWorkspace, // 0x0000003E
  }
}
