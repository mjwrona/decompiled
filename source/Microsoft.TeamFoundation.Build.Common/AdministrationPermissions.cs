// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.AdministrationPermissions
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

namespace Microsoft.TeamFoundation.Build.Common
{
  public static class AdministrationPermissions
  {
    public static readonly int ViewBuildResources = 1;
    public static readonly int ManageBuildResources = 2;
    public static readonly int UseBuildResources = 4;
    public static readonly int AdministerBuildResourcePermissions = 8;
    public static readonly int ManagePipelinePolicies = 16;
    public static readonly int AllPermissions = AdministrationPermissions.ViewBuildResources | AdministrationPermissions.ManageBuildResources | AdministrationPermissions.UseBuildResources | AdministrationPermissions.AdministerBuildResourcePermissions | AdministrationPermissions.ManagePipelinePolicies;
  }
}
