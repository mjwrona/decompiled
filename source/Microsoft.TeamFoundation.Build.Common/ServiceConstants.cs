// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.ServiceConstants
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ServiceConstants
  {
    internal const string BaseNamespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build";
    public const string HostingNamespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting";
    public const string BaseAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/";

    public static class SharedResource
    {
      public const string Name = "SharedResource";
      public const string Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting";
      public const string ResourceAcquiredAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/SharedResource/ResourceAcquired";
      internal const string ResourceAcquiredReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/SharedResource/ResourceAcquiredResponse";
      public const string ResourceAcquiredBody = "SharedResourceAcquired";
    }

    public static class Agent
    {
      public const string Name = "Agent";
      public const string Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting";
      public const string RequestIntermediateLogs = "RequestIntermediateLogs";
      public const string RequestIntermediateLogsAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Agent/RequestIntermediateLogs";
      public const string RequestIntermediateLogsReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Agent/RequestIntermediateLogsResponse";
      public const string ResourceAcquiredAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/SharedResource/ResourceAcquired";
      public const string ResourceAcquiredReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/SharedResource/ResourceAcquiredResponse";
      public const string StartWorkflow = "StartWorkflow";
      public const string StartWorkflowAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Agent/StartWorkflow";
      public const string StartWorkflowReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Agent/StartWorkflowResponse";
      public const string StopWorkflow = "StopWorkflow";
      public const string StopWorkflowAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Agent/StopWorkflow";
      public const string StopWorkflowReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Agent/StopWorkflowResponse";
    }

    public static class Controller
    {
      public const string Name = "Controller";
      public const string Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting";
      public const string AgentAcquiredAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/AgentAcquired";
      public const string AgentAcquiredReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/AgentAcquiredResponse";
      public const string DeleteBuildDropAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildDrop";
      internal const string DeleteBuildDropReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildDropResponse";
      public const string DeleteBuildDropBody = "DeleteBuildDrop";
      public const string DeleteBuildSymbolsAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildSymbols";
      internal const string DeleteBuildSymbolsReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/DeleteBuildSymbolsResponse";
      public const string DeleteBuildSymbolsBody = "DeleteBuildSymbols";
      public const string RequestIntermediateLogsAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/RequestIntermediateLogs";
      public const string RequestIntermediateReplyLogsAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/RequestIntermediateLogsResponse";
      public const string ResourceAcquiredAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/SharedResource/ResourceAcquired";
      public const string ResourceAcquiredReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/SharedResource/ResourceAcquiredResponse";
      internal const string StartBuild = "StartBuild";
      public const string StartBuildAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/StartBuild";
      public const string StartBuildReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/StartBuildResponse";
      internal const string StopBuild = "StopBuild";
      public const string StopBuildAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/StopBuild";
      public const string StopBuildReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/StopBuildResponse";
      public const string WorkflowCompleted = "WorkflowCompleted";
      public const string WorkflowCompletedAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/WorkflowCompleted";
      public const string WorkflowCompletedReplyAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/Controller/WorkflowCompletedResponse";
    }

    public static class ServiceHost
    {
      public const string Name = "ServiceHost";
      public const string Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting";
      public const string AgentUpdatedAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/ServiceHost/AgentUpdated";
      public const string AgentUpdatedBody = "AgentUpdated";
      public const string ControllerUpdatedAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/ServiceHost/ControllerUpdated";
      public const string ControllerUpdatedBody = "ControllerUpdated";
      public const string HostShutdownFaultAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/ServiceHost/HostShutdownFault";
      public const string ServiceUpdatedUri = "serviceUri";
      public const string ServiceUpdatedAction = "action";
    }

    public static class ElasticBuild
    {
      public const string Authorization = "Authorization";
      public const string AccessToken = "AccessToken";
      public const string MessageQueuePrefix = "elasticbuild-";
      public const string PoolName = "PoolName";
      public const string PoolToken = "PoolToken";
      public const string RequestId = "RequestId";
      public const string RoleInstanceName = "RoleInstance";
      public const string Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Elastic";
      public const string StartBuildAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Elastic/StartBuild";
      public const string PoolUpdatedAction = "http://schemas.microsoft.com/TeamFoundation/2010/Build/Elastic/PoolUpdated";
    }
  }
}
