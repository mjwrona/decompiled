// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.TracePoints
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class TracePoints
  {
    public static readonly string Area = "Pipelines";

    public class Events
    {
      public static readonly int PostEventEnter = 15287100;
      public static readonly int PostEventLeave = 15287101;
      public static readonly int HandleEvent = 15287103;
      public static readonly int CheckRunUpdateError = 15287104;
      public static readonly int BuildCIPublishingError = 15287105;
      public static readonly int DefinitionCIPublishingError = 15287106;
      public static readonly int PullRequestHandlerJobEnter = 15287107;
      public static readonly int PullRequestHandlerJobLeave = 15287108;
      public static readonly int PullRequestHandlerJobError = 15287109;
      public static readonly int PullRequestHandlerJob = 15287110;
      public static readonly int BuildCompletedEventListenerHandleEvent = 15287111;
      public static readonly int BuildDefinitionChangedEventListenerHandleEvent = 15287112;
      public static readonly int BuildQueuedEventListenerHandleEvent = 15287113;
      public static readonly int CommentHandlerJobEnter = 15287114;
      public static readonly int CommentHandlerJobLeave = 15287115;
      public static readonly int CommentHandlerJobError = 15287116;
      public static readonly int CommentHandlerJob = 15287117;
      public static readonly int AbstractJobExtension = 15287118;
      public static readonly int FlowNoEventsFound = 15287119;
      public static readonly int FlowInfo = 15287120;
      public static readonly int TryUpdateRepositoryInformation = 15287121;
      public static readonly int RepositoryUrlMapping = 15287122;
      public static readonly int PipelineEventFilter = 15287123;
    }

    public class Connections
    {
      public static readonly int CreateConnectionEnter = 15287200;
      public static readonly int CreateConnectionLeave = 15287201;
      public static readonly int QueuePipelineCreationEnter = 15287202;
      public static readonly int QueuePipelineCreationLeave = 15287203;
      public static readonly int CreateDefinition = 15287204;
      public static readonly int CreateTeamProject = 15287205;
      public static readonly int Extensions = 15287206;
      public static readonly int IsProviderDefinition = 15287207;
      public static readonly int IsProviderEndpoint = 15287208;
      public static readonly int Helpers = 15287209;
      public static readonly int Operations = 15287210;
      public static readonly int DeleteConnectionEnter = 15287211;
      public static readonly int DeleteConnectionLeave = 15287212;
    }

    public class Templates
    {
      public static readonly int GetTemplatesEnter = 15287300;
      public static readonly int GetTemplatesLeave = 15287301;
      public static readonly int GetTemplateEnter = 15287302;
      public static readonly int GetTemplateLeave = 15287303;
      public static readonly int GetTemplateControllerError = 15287304;
      public static readonly int RenderTemplateEnter = 15287305;
      public static readonly int RenderTemplateLeave = 15287306;
      public static readonly int TemplateResourcesController = 15287307;
      public static readonly int CreateResourcesEnter = 15287308;
      public static readonly int CreateResourcesLeave = 15287309;
    }

    public class Provider
    {
      public static readonly int CreateConnection = 15287400;
      public static readonly int ValidateIncomingEvent = 15287401;
      public static readonly int HandleEvent = 15287402;
      public static readonly int UpdateStatus = 15287403;
      public static readonly int GetBuildRepository = 15287404;
      public static readonly int GetExternalPullRequest = 15287405;
      public static readonly int HostRoutingNotRequired = 15287406;
      public static readonly int ServiceIdentityNotFound = 15287407;
      public static readonly int ExternalApp = 15287408;
      public static readonly int GetExternalPullRequestWithCommits = 15287409;
      public static readonly int HasMemberWriteAccess = 15287410;
    }

    public class EventsRouting
    {
      public static readonly int BeginRequest = 15287500;
      public static readonly int RequestInvalid = 15287501;
      public static readonly int RequestRouted = 15287502;
      public static readonly int Mapping = 15287503;
      public static readonly int RoutingException = 15287504;
      public static readonly int EventReceived = 15287505;
    }

    public class Artifacts
    {
      public static readonly int GetArtifactsEnter = 15287600;
      public static readonly int GetArtifactsLeave = 15287601;
      public static readonly int GetArtifacts = 15287602;
      public static readonly int CheckBuild = 15287603;
    }

    public class BuildFrameworkDetection
    {
      public static readonly int DetectEnter = 15287650;
      public static readonly int DetectLeave = 15287651;
      public static readonly int NodeJsTryDetectEnter = 15287652;
      public static readonly int NodeJsTryDetectLeave = 15287653;
      public static readonly int PythonTryDetectEnter = 15287654;
      public static readonly int PythonTryDetectLeave = 15287655;
      public static readonly int PhpTryDetectEnter = 15287656;
      public static readonly int PhpTryDetectLeave = 15287657;
      public static readonly int MsBuildTryDetectEnter = 15287658;
      public static readonly int MsBuildTryDetectLeave = 15287659;
      public static readonly int MsBuildTryDetectError = 15287660;
      public static readonly int FallbackTryDetectEnter = 15287661;
      public static readonly int FallbackTryDetectLeave = 15287662;
      public static readonly int FallbackTryDetectError = 15287663;
      public static readonly int DotNetCoreTryDetectEnter = 15287664;
      public static readonly int DotNetCoreTryDetectLeave = 15287665;
      public static readonly int DockerTryDetectEnter = 15287666;
      public static readonly int Statistics = 15287667;
      public static readonly int DockerTryDetectLeave = 15287668;
      public static readonly int DockerfileParserBuildStagesLimitReached = 15287669;
      public static readonly int DockerfileParserExposedPortsPerStageLimitReached = 15287670;
      public static readonly int PowershellTryDetectEnter = 15287671;
      public static readonly int PowershellTryDetectLeave = 15287672;
    }

    public class DataProvider
    {
      public static readonly int CreateAndRunPipelineQueueBuildFailed = 15287700;
      public static readonly int CreateAndRunPipelineSetProjectVisibilityFailed = 15287701;
      public static readonly int RecommendedServiceConnectionDataProviderFailed = 15287702;
      public static readonly int GetConfigurationDataProvider = 15287703;
    }

    public class Configurations
    {
      public static readonly int CreateConfigurationEnter = 15287800;
      public static readonly int CreateConfigurationLeave = 15287801;
      public static readonly int ExistingConfigurationDataProviderError = 15287802;
      public static readonly int RecommendedConfigurationDataProviderError = 15287803;
      public static readonly int TemplatePropertiesHandlerError = 15287804;
      public static readonly int FindConfigurationFilesDataProviderError = 15287805;
      public static readonly int CreateResourcesDataProviderError = 15287806;
      public static readonly int CustomizedTemplateDataProviderError = 15287807;
    }

    public class RepositoryAnalysis
    {
      public static readonly int GetExistingConfigurationFileEnter = 15287900;
      public static readonly int GetExistingConfigurationFileLeave = 15287901;
      public static readonly int GetSuggestedConfigurationFilesEnter = 15287902;
      public static readonly int GetSuggestedConfigurationFilesLeave = 15287903;
      public static readonly int SearchForConfigurationFile = 15287904;
      public static readonly int FindRecommendations = 15287905;
      public static readonly int UpdateToRecommended = 15287906;
      public static readonly int FindExistingConfigurationFiles = 15287907;
      public static readonly int GetRecommendedConfigurationsEnter = 15287908;
      public static readonly int GetRecommendedConfigurationsLeave = 15287909;
      public static readonly int FindRecommendedConfigurations = 15287910;
      public static readonly int GetExistingConfigurationFiles = 15287911;
      public static readonly int GetSuggestedConfigurationFilesFromBuildFrameworksEnter = 15287912;
      public static readonly int GetSuggestedConfigurationFilesFromBuildFrameworksLeave = 15287913;
      public static readonly int RepositoryAnalysisRecommendationsJobEnter = 15287914;
      public static readonly int RepositoryAnalysisRecommendationsJobLeave = 15287915;
      public static readonly int RepositoryAnalysisRecommendationsJobError = 15287916;
      public static readonly int RepositoryAnalysisRecommendationsJobErrorReportingError = 15287917;
      public static readonly int GetRecommendedConfigurationPath = 15287918;
    }
  }
}
