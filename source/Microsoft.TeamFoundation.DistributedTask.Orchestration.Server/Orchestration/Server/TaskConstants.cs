// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class TaskConstants
  {
    public const string HubType = "DistributedTask";
    public const string JobAuthorizeAsId = "Job.AuthorizeAsId";
    public const string DisableNode6TasksRegistryKey = "/Service/DistributedTask/Settings/DisableNode6Tasks";
    public const string DisableTaskRestrictionsRegistryKey = "/Service/DistributedTask/Settings/DisableTaskRestrictions";
    public static readonly TimeSpan JobAccessTokenDuration = TimeSpan.FromHours(48.0);
    public static readonly Guid AgentCapabilityUpdateJob = new Guid("5820B920-19B7-4983-A9E7-1AF68090CC53");
    public static readonly Guid AgentRequestMonitorJob = new Guid("3F9FBAB5-E92E-46C7-9798-D739205B9AF3");
    public static readonly string BuildTaskContributionIdentifier = "ms.vss-distributed-task.tasks";
    public static readonly string MobileCenterAccountSignupEntryPoint = "MobileCenter";
    public static readonly Guid MobileCenterIntTenantId = new Guid("dd3f7e7b-c399-40be-afd2-bb79211f05e0");
    public static readonly Guid MobileCenterStagingTenantId = new Guid("0173b5a2-2181-409b-af01-7ac9b5746830");
    public static readonly Guid MobileCenterProdTenantId = new Guid("9bc98f0f-fc5c-4be2-af91-e88ac46145fd");
    public static readonly Guid QueueThrottledPlansJob = new Guid("506DA712-1E1F-46DE-BF1A-24E237AC20FE");
    public static readonly Guid PlanThrottlingJob = new Guid("F76FE4B2-3E1F-46DE-BF1A-24E237AC20FE");
    public static readonly string ExternalVariablesDownloadJobName = nameof (ExternalVariablesDownloadJobName);
    public static readonly string ExternalVariablesDownloadJobExtension = "Microsoft.VisualStudio.Services.ReleaseManagement.DistributedTask.Extensions.ExternalVariablesDownloadJob";
  }
}
