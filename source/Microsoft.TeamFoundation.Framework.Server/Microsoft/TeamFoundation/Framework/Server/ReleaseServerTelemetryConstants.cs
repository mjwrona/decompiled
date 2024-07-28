// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ReleaseServerTelemetryConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ReleaseServerTelemetryConstants
  {
    public static readonly string ReleasesCountPerDayRegistryKey = "/Service/TFS/OnPrem/Release/ReleasesCountPerDay";
    public static readonly string ReleaseTasksTelemetryRegistryRoot = "/Service/TFS/OnPrem/Release/ReleaseTaskCountPerDay/";
    public static readonly string ReleaseDefinitionsCreatedCountPerDayRegistryKey = "/Service/TFS/OnPrem/Release/ReleaseDefinitionsCreatedCountPerDay";
    public static readonly string EnvironmentsCountPerDayRegistryKey = "/Service/TFS/OnPrem/Release/ReleaseEnvironmentsCountPerDay";
    public static readonly string ReleaseEnvironmentStatusRegistryRoot = "/Service/TFS/OnPrem/Release/ReleaseEnvironmentsStatusPerDay/";
    public static readonly string ReleasesCompletedPerDayRegistryKey = "/Service/TFS/OnPrem/Release/ReleasesCompletedCountPerDay";
    public static readonly string ReleaseArtifactsCountPerDayRegistryKey = "/Service/TFS/OnPrem/Release/ReleaseArtifactsCountPerDay";
    public static readonly string ReleaseTotalTasksTelemetryRegistryKey = "/Service/TFS/OnPrem/Release/ReleaseTotalTasksCountPerDay";
    public static readonly string ReleaseDefinitionsUpdateCountPerDayRegistryKey = "/Service/TFS/OnPrem/Release/ReleaseDefinitionsUpdateCountPerDay";
    public static readonly string ReleaseApprovalsCountPerDayRegistryKey = "/Service/TFS/OnPrem/Release/ReleaseApprovalsCountPerDay";
    public static readonly string EnvironmentsWithEndpointCountRegistryRoot = "/Service/TFS/OnPrem/Release/ReleaseEnvironmentsWithEndpointCountPerDay/";
    public static readonly string AzureSubscriptionsUsedPerDayRegistryKey = "/Service/TFS/OnPrem/Release/AzureSubscriptionsUsedPerDay";
    public static readonly string AzureSubscriptionsInDeploymentTargetsUsedPerDayRegistryKey = "/Service/TFS/OnPrem/Release/AzureSubscriptionsInDeploymentTargetsUsedPerDay";
  }
}
