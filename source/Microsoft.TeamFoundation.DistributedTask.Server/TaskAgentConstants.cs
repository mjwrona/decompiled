// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskAgentConstants
  {
    internal static readonly string AgentPackageType = "agent";
    internal static readonly string PipelinesAgentPackageType = "pipelines-agent";
    internal static readonly string AgentVersionRegistryPath = "/Service/DistributedTask/AgentVersion";
    internal static readonly string OfflineAgentsDirectory = "Microsoft\\Azure DevOps\\Agents";
    internal static readonly PackageVersion CoreAgentInitVersion = new PackageVersion("2.99.0");
    internal static readonly string CoreV1WindowsPlatformName = "win7-x64";
    internal static readonly string CoreV1Ubuntu14PlatformName = "ubuntu.14.04-x64";
    internal static readonly string CoreV1Ubuntu16PlatformName = "ubuntu.16.04-x64";
    internal static readonly string CoreV1Rhel7PlatformName = "rhel.7.2-x64";
    internal static readonly string CoreV1OSXPlatformName = "osx.10.11-x64";
    internal static readonly string CoreV2WindowsPlatformName = "win-x64";
    internal static readonly string CoreV2LinuxPlatformName = "linux-x64";
    internal static readonly string CoreV2OSXPlatformName = "osx-x64";
    internal static readonly string PoolMaintenanceHubName = "PoolMaintenance";
    internal static readonly PackageVersion MinimumPoolProviderAgentVersion = new PackageVersion("2.147.1");
  }
}
