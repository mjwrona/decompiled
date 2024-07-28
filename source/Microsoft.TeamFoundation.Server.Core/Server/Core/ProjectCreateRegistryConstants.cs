// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectCreateRegistryConstants
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class ProjectCreateRegistryConstants
  {
    public const string ProjectLimitRegistryKey = "/Service/Framework/ServerCore/ProjectLimit";
    public const string ProjectWarningRegistryKey = "/Service/Framework/ServerCore/ProjectWarning";
    public const string ProjectPerformanceWarningRegistryKey = "/Service/Framework/ServerCore/ProjectPerformanceWarning";
    public const int ProjectLimitRegistryDefault = 1000;
    public const int ProjectWarningRegistryDefault = 250;
    public const int ProjectPerformanceThresholdWarningRegistryDefault = 75;
    public const string DisableProjectCreateLimitFeatureFlag = "Project.Creation.DisableProjectLimitCheck";
    public const string ProjectLimitSharedDataName = "ProjectLimitFromRegistry";
    public const string ProjectWarningSharedDataName = "ProjectWarningFromRegistry";
  }
}
