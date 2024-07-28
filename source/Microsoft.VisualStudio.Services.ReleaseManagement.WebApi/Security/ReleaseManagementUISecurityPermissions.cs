// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security.ReleaseManagementUISecurityPermissions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security
{
  [Flags]
  public enum ReleaseManagementUISecurityPermissions
  {
    None = 0,
    ViewTaskEditor = 1,
    ViewCDWorkflowEditor = 2,
    ExportReleaseDefinition = 4,
    ViewLegacyUI = 8,
    DeploymentSummaryAcrossProjects = 16, // 0x00000010
    ViewExternalArtifactCommitsAndWorkItems = 32, // 0x00000020
  }
}
