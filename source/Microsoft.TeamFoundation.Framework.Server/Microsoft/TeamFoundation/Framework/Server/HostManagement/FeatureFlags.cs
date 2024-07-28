// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostManagement.FeatureFlags
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.HostManagement
{
  public static class FeatureFlags
  {
    public const string DisableOrchestrationActivityLog = "Microsoft.VisualStudio.Services.Orchestration.ActivityLog.Disabled";
    public const string DisableNewActivityScope = "Microsoft.VisualStudio.Services.Orchestration.UseNewActivityId.Disabled";
    public const string DispatchersUseOrchestrationIds = "VisualStudio.Services.HostManagement.DispatchersUseOrchestrationIds";
    public const string OrchestrationDispatcherMessagesPerSession = "VisualStudio.Services.HostManagement.OrchestrationDispatcherMessagesPerSession";
    public const string UseMultithreadedOrchestration = "VisualStudio.Services.HostManagement.UseMultithreadedOrchestration";
    public const string UsePollingInSeparateThread = "VisualStudio.Services.HostManagement.UsePollingInSeparateThread";
    public const string CountCpuForJobAsyncCalls = "VisualStudio.Services.HostManagement.CountCpuForJobAsyncCalls";
    public const string CountCpuCyclesPerActivity = "VisualStudio.Services.HostManagement.CountCpuCyclesPerActivity";
  }
}
