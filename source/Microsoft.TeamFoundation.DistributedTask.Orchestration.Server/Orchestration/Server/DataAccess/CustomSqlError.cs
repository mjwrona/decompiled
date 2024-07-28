// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CustomSqlError
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal static class CustomSqlError
  {
    public const int GenericDatabaseUserMessage = 50000;
    public const int TransactionRequired = 907002;
    public const int GenericDatabaseUpdateFailure = 907004;
    public const int AgentSessionInvalid = 907005;
    public const int AgentSessionConflict = 907006;
    public const int PoolNotFound = 907007;
    public const int AgentJobRequestNotFound = 907008;
    public const int AgentJobRequestTokenExpired = 907009;
    public const int PlanNotFound = 907010;
    public const int TimelineNotFound = 907011;
    public const int AgentNotFound = 907012;
    public const int AgentPoolExists = 907013;
    public const int TaskDefinitionExists = 907014;
    public const int AgentExists = 907015;
    public const int AgentStillRunningJob = 907016;
    public const int ServiceEndpointAlreadyExists = 907017;
    public const int QueueExists = 907018;
    public const int QueueNotFound = 907019;
    public const int TimelineExists = 907020;
    public const int JobNotFound = 907021;
    public const int PlanGroupNotFound = 907022;
    public const int TimelineAttemptInvalid = 907023;
    public const int ExclusiveLockRequestAlreadyQueued = 907024;
    public const int TimelineUpdateFailure = 907025;
    public const int TaskHubExists = 907051;
    public const int InvalidTimelineRecord = 907052;
    public const int TimelineRecordNotFound = 907053;
    public const int MetaTaskDefinitionExists = 907055;
    public const int MetaTaskDefinitionNotFound = 907056;
    public const int PackageExists = 907057;
    public const int DeploymentMachineGroupExists = 907058;
    public const int DeploymentMachineGroupNotFound = 907059;
    public const int VariableGroupExists = 907060;
    public const int VariableGroupNotFound = 907061;
    public const int TaskGroupRevisionAlreadyExists = 907062;
    public const int TaskGroupAlreadyUpdated = 907063;
    public const int SecureFileExists = 907064;
    public const int SecureFileNotFound = 907065;
    public const int PoolMaintenanceJobNotFound = 907066;
    public const int PoolMaintenanceDefinitionNotFound = 907067;
    public const int TaskAgentPendingUpdateNotFound = 907068;
    public const int TaskGroupPreviewExists = 907069;
    public const int TaskGroupCanOnlyEditLatestVersion = 907070;
    public const int TaskGroupDraftExists = 907071;
    public const int DeploymentMachineExists = 907072;
    public const int DeploymentGroupExistsWithPool = 907073;
    public const int CannotDeleteAndAddMetadata = 907074;
    public const int OAuthConfigurationExists = 907075;
    public const int OAuthConfigurationNotFound = 907076;
    public const int PlanLogNotFound = 907077;
    public const int AgentCloudExists = 907078;
    public const int AgentCloudNotFound = 907079;
    public const int AgentCloudRequestExists = 907080;
    public const int AgentCloudRequestNotFound = 907081;
    public const int PoolDoesNotPointToAgentCloud = 907082;
    public const int PrivateAgentInvalidProvisioningState = 907083;
    public const int EnvironmentExists = 907085;
    public const int EnvironmentExistsInProject = 907086;
    public const int EnvironmentNotFound = 907087;
    public const int EnvironmentResourceExists = 907088;
    public const int EnvironmentResourceNotFound = 907089;
    public const int EnvironmentResourcesExceededMaxCount = 907090;
    public const int EnvironmentDeploymentExecutionHistoryRecordNotFound = 907091;
    public const int EnvironmentPoolAlreadyInUse = 907092;
    public const int VirtualMachineExists = 907093;
    public const int EnvironmentPoolExists = 907094;
    public const int EnvironmentResourceDeploymentExecutionHistoryRecordNotFound = 907095;
    public const int ElasticPoolAlreadyExists = 907096;
    public const int ElasticPoolDoesNotExist = 907097;
    public const int ElasticNodeAlreadyExists = 907098;
    public const int ElasticNodeDoesNotExist = 907099;
    public const int ElasticNodeAgentAlreadyExists = 907100;
    public const int ElasticNodeComputeAlreadyExists = 907101;
    public const int DuplicatePoolNameAndType = 907102;
    public const int InternalCloudAgentDefinitionExists = 907103;
    public const int InternalCloudAgentDefinitionNotFound = 907104;
    public const int EnvironmentReferenceExists = 907105;
    public const int PersistedStageGroupMappingAlreadyExists = 907106;
  }
}
