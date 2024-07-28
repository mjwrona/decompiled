// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security.ReleaseManagementSecurityPermissions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security
{
  [Flags]
  public enum ReleaseManagementSecurityPermissions
  {
    None = 0,
    ViewReleaseDefinition = 1,
    EditReleaseDefinition = 2,
    DeleteReleaseDefinition = 4,
    ManageReleaseApprovers = 8,
    ManageReleases = 16, // 0x00000010
    ViewReleases = 32, // 0x00000020
    CreateReleases = 64, // 0x00000040
    EditReleaseEnvironment = 128, // 0x00000080
    DeleteReleaseEnvironment = 256, // 0x00000100
    AdministerReleasePermissions = 512, // 0x00000200
    DeleteReleases = 1024, // 0x00000400
    ManageDeployments = 2048, // 0x00000800
    ManageReleaseSettings = 4096, // 0x00001000
    ManageTaskHubExtension = 8192, // 0x00002000
  }
}
