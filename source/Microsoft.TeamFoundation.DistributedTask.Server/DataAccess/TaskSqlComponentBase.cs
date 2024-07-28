// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskSqlComponentBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal abstract class TaskSqlComponentBase : TeamFoundationSqlResourceComponent
  {
    private static Lazy<IDictionary<int, SqlExceptionFactory>> s_translatedExceptions = new Lazy<IDictionary<int, SqlExceptionFactory>>(new Func<IDictionary<int, SqlExceptionFactory>>(TaskSqlComponentBase.CreateExceptionMap));

    protected TaskSqlComponentBase() => this.ContainerErrorCode = 50000;

    protected override string TraceArea => "DistributedTask";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => TaskSqlComponentBase.s_translatedExceptions.Value;

    protected void BindDataspaceId(Guid dataspaceIdentifier) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier, true));

    protected override SqlParameter BindDateTime2(string parameterName, DateTime parameterValue)
    {
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.DateTime2);
      sqlParameter.Size = 7;
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected override SqlParameter BindNullableDateTime2(
      string parameterName,
      DateTime? parameterValue)
    {
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.DateTime2);
      sqlParameter.Size = 7;
      if (parameterValue.HasValue)
        sqlParameter.Value = (object) parameterValue.Value;
      else
        sqlParameter.Value = (object) DBNull.Value;
      return sqlParameter;
    }

    protected void PrepareStoredProcedure(string storedProcedure, Guid dataspaceIdentifier)
    {
      this.PrepareStoredProcedure(storedProcedure);
      this.BindInt("dataspaceId", this.GetDataspaceId(dataspaceIdentifier, "DistributedTask", true));
    }

    private static Exception CreateException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException sqlException,
      SqlError sqlError)
    {
      Exception exception = (Exception) null;
      switch (errorNumber)
      {
        case 907006:
          exception = (Exception) new TaskAgentSessionConflictException(TaskResources.AgentSessionConflict((object) sqlError.ExtractString("agentName"), (object) sqlError.ExtractString("ownerName")));
          break;
        case 907007:
          exception = (Exception) new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) sqlError.ExtractInt("poolId")));
          break;
        case 907008:
          exception = (Exception) new TaskAgentJobNotFoundException(TaskResources.AgentRequestNotFound((object) sqlError.ExtractLong("reservationId")));
          break;
        case 907009:
          exception = (Exception) new TaskAgentJobTokenExpiredException(TaskResources.AgentRequestExpired((object) sqlError.ExtractLong("reservationId")));
          break;
        case 907012:
          exception = (Exception) new TaskAgentNotFoundException(TaskResources.AgentNotFound((object) sqlError.ExtractInt("poolId"), (object) sqlError.ExtractInt("agentId")));
          break;
        case 907013:
          exception = (Exception) new TaskAgentPoolExistsException(TaskResources.AgentPoolExists((object) sqlError.ExtractString("poolName")));
          break;
        case 907014:
          exception = (Exception) new TaskDefinitionExistsException(TaskResources.TaskDefinitionAlreadyUploaded((object) sqlError.ExtractString("taskId"), (object) sqlError.ExtractString("name"), (object) sqlError.ExtractTaskVersion()));
          break;
        case 907015:
          exception = (Exception) new TaskAgentExistsException(TaskResources.AgentExists((object) sqlError.ExtractString("poolName"), (object) sqlError.ExtractString("agentName")));
          break;
        case 907016:
          exception = (Exception) new TaskAgentJobStillRunningException(TaskResources.AgentStillRunningJob((object) sqlError.ExtractString("agentName"), (object) sqlError.ExtractString("poolName")));
          break;
        case 907018:
          exception = (Exception) new TaskAgentQueueExistsException(TaskResources.QueueExists((object) sqlError.ExtractString("queueName")));
          break;
        case 907019:
          exception = (Exception) new TaskAgentQueueNotFoundException(TaskResources.QueueNotFound((object) sqlError.ExtractInt("queueId")));
          break;
        case 907024:
          exception = (Exception) new ExclusiveLockRequestAlreadyQueuedException(TaskResources.ExclusiveLockRequestAlreadyQueued((object) sqlError.ExtractString("requestIds")));
          break;
        case 907055:
          exception = (Exception) new MetaTaskDefinitionExistsException(TaskResources.MetaTaskDefinitionExists((object) sqlError.ExtractString("metaTaskName")));
          break;
        case 907056:
          exception = (Exception) new MetaTaskDefinitionNotFoundException(TaskResources.MetaTaskDefinitionNotFound((object) sqlError.ExtractString("metaTaskName")));
          break;
        case 907057:
          exception = (Exception) new PackageExistsException(TaskResources.PackageExists((object) sqlError.ExtractString("packageType"), (object) sqlError.ExtractString("platform"), (object) sqlError.ExtractInt("majorVersion"), (object) sqlError.ExtractInt("minorVersion"), (object) sqlError.ExtractInt("patchVersion")));
          break;
        case 907058:
          exception = (Exception) new DeploymentMachineGroupExistsException(TaskResources.DeploymentMachineGroupExists((object) sqlError.ExtractString("machineGroupName")));
          break;
        case 907059:
          exception = (Exception) new DeploymentMachineGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) sqlError.ExtractInt("machineGroupId")));
          break;
        case 907060:
          exception = (Exception) new VariableGroupExistsException(TaskResources.VariableGroupExist((object) sqlError.ExtractString("groupName")));
          break;
        case 907061:
          exception = (Exception) new VariableGroupNotFoundException(TaskResources.VariableGroupNotFound((object) sqlError.ExtractString("groupId")));
          break;
        case 907062:
          exception = (Exception) new TaskGroupRevisionAlreadyExistsException(TaskResources.TaskGroupRevisionAlreadyExists((object) sqlError.ExtractString("definitionId"), (object) sqlError.ExtractInt("definitionRevision")));
          break;
        case 907063:
          exception = (Exception) new TaskGroupAlreadyUpdatedException(TaskResources.TaskGroupAlreadyUpdated((object) sqlError.ExtractString("definitionId")));
          break;
        case 907064:
          exception = (Exception) new SecureFileExistsException(TaskResources.SecureFileExists((object) sqlError.ExtractString("secureFileName")));
          break;
        case 907065:
          exception = (Exception) new SecureFileNotFoundException(TaskResources.SecureFileNotFound((object) sqlError.ExtractString("secureFileId")));
          break;
        case 907066:
          exception = (Exception) new TaskAgentPoolMaintenanceJobNotFoundException(TaskResources.MaintenanceJobNotFound((object) sqlError.ExtractInt("jobId"), (object) sqlError.ExtractInt("poolId")));
          break;
        case 907067:
          exception = (Exception) new TaskAgentPoolMaintenanceDefinitionNotFoundException(TaskResources.MaintenanceDefinitionNotFound((object) sqlError.ExtractInt("definitionId"), (object) sqlError.ExtractInt("poolId")));
          break;
        case 907068:
          exception = (Exception) new TaskAgentPendingUpdateNotFoundException(TaskResources.AgentPendingUpdateNotFound((object) sqlError.ExtractInt("agentId"), (object) sqlError.ExtractInt("poolId")));
          break;
        case 907069:
          exception = (Exception) new TaskGroupPreviewExistsException(TaskResources.TaskGroupPreviewExists((object) sqlError.ExtractString("metaTaskName")));
          break;
        case 907070:
          exception = (Exception) new TaskGroupUpdateFailedException(TaskResources.TaskGroupCanOnlyEditLatestVersion((object) sqlError.ExtractString("taskGroupName"), (object) sqlError.ExtractInt("majorVersion"), (object) sqlError.ExtractInt("existingMajorVersion")));
          break;
        case 907071:
          exception = (Exception) new TaskGroupDraftExistsException(TaskResources.TaskGroupDraftExists((object) sqlError.ExtractString("metaTaskName")));
          break;
        case 907072:
          exception = (Exception) new DeploymentMachineExistsException(TaskResources.DeploymentMachineExists((object) sqlError.ExtractInt("deploymentGroupId"), (object) sqlError.ExtractInt("agentId")));
          break;
        case 907073:
          exception = (Exception) new DeploymentGroupExistsException(TaskResources.DeploymentGroupExistsWithPool((object) sqlError.ExtractString("deploymentGroupName"), (object) sqlError.ExtractInt("poolId")));
          break;
        case 907074:
          exception = (Exception) new CannotDeleteAndAddMetadataException(TaskResources.CannotDeleteAndAddMetadata());
          break;
        case 907078:
          exception = (Exception) new TaskAgentCloudExistsException(TaskResources.AgentCloudExists((object) sqlError.ExtractString("agentCloudName")));
          break;
        case 907079:
          exception = (Exception) new TaskAgentCloudNotFoundException(TaskResources.AgentCloudNotFound((object) sqlError.ExtractInt("agentCloudId")));
          break;
        case 907080:
          exception = (Exception) new TaskAgentCloudRequestExistsException(TaskResources.AgentCloudRequestExists((object) sqlError.ExtractInt("poolId"), (object) sqlError.ExtractInt("agentId")));
          break;
        case 907081:
          exception = (Exception) new TaskAgentCloudRequestNotFoundException(TaskResources.AgentCloudRequestNotFound((object) sqlError.ExtractInt("agentCloudId"), (object) sqlError.ExtractString("requestId")));
          break;
        case 907082:
          exception = (Exception) new TaskAgentPoolReferencesDifferentAgentCloudException(TaskResources.AgentPoolReferencesDifferentAgentCloud((object) sqlError.ExtractInt("poolId"), (object) sqlError.ExtractInt("poolAgentCloudId"), (object) sqlError.ExtractInt("agentCloudId")));
          break;
        case 907083:
          exception = (Exception) new PrivateTaskAgentProvisioningStateInvalidException(TaskResources.PrivateTaskAgentInvalidProvisioningState((object) sqlError.ExtractString("provisioningState")));
          break;
        case 907085:
          exception = (Exception) new EnvironmentExistsException(TaskResources.EnvironmentExists((object) sqlError.ExtractString("name")));
          break;
        case 907086:
          exception = (Exception) new EnvironmentExistsException(TaskResources.EnvironmentExistsInProject((object) sqlError.ExtractString("name")));
          break;
        case 907087:
          exception = (Exception) new EnvironmentNotFoundException(TaskResources.EnvironmentNotFound((object) sqlError.ExtractInt("environmentId")));
          break;
        case 907088:
          exception = (Exception) new EnvironmentResourceExistsException(TaskResources.EnvironmentResourceExists((object) sqlError.ExtractString("name")));
          break;
        case 907089:
          exception = (Exception) new EnvironmentResourceNotFoundException(TaskResources.EnvironmentResourceNotFound((object) sqlError.ExtractInt("resourceId"), (object) sqlError.ExtractInt("environmentId")));
          break;
        case 907090:
          exception = (Exception) new EnvironmentResourcesExceededMaxCountException(TaskResources.EnvironmentResourcesExceededMaxCount((object) sqlError.ExtractInt("environmentId"), (object) sqlError.ExtractInt("resourcesCount")));
          break;
        case 907091:
          exception = (Exception) new EnvironmentExecutionDeploymentHistoryRecordNotFoundException(TaskResources.EnvironmentDeploymentExecutionHistoryRecordNotFound((object) sqlError.ExtractLong("recordId"), (object) sqlError.ExtractInt("environmentId")));
          break;
        case 907092:
          exception = (Exception) new EnvironmentPoolAlreadyInUseException(TaskResources.EnvironmentPoolAlreadyInUse((object) sqlError.ExtractInt("environmentId")));
          break;
        case 907094:
          exception = (Exception) new EnvironmentPoolExistsException(TaskResources.EnvironmentPoolExists((object) sqlError.ExtractInt("deploymentPoolId"), (object) sqlError.ExtractInt("environmentId")));
          break;
        case 907095:
          exception = (Exception) new EnvironmentResourceDeploymentHistoryRecordNotFoundException(TaskResources.EnvironmentResourceDeploymentHistoryRecordNotFound((object) sqlError.ExtractLong("recordId"), (object) sqlError.ExtractInt("environmentId"), (object) sqlError.ExtractInt("resourceId")));
          break;
        case 907096:
          exception = (Exception) new ElasticPoolAlreadyExistsException(TaskResources.ElasticPoolAlreadyExistsExceptionMessage((object) sqlError.ExtractString("azureId")));
          break;
        case 907097:
          exception = (Exception) new ElasticPoolDoesNotExistException(TaskResources.ElasticPoolDoesNotExistExceptionMessage((object) sqlError.ExtractString("cloudId")));
          break;
        case 907102:
          exception = (Exception) new DuplicatePoolNameAndTypeException(TaskResources.DuplicatePoolNameAndTypeExceptionMessage((object) sqlError.ExtractString("name"), (object) sqlError.ExtractString("existingPoolType"), (object) sqlError.ExtractString("poolId")));
          break;
        case 907103:
          exception = (Exception) new InternalCloudAgentDefinitionExistsException(TaskResources.InternalCloudAgentDefinitionExistsExceptionMessage((object) sqlError.ExtractString("identifier")));
          break;
        case 907104:
          exception = (Exception) new InternalCloudAgentDefinitionNotFoundException(TaskResources.InternalCloudAgentDefinitionNotFoundExceptionMessage((object) sqlError.ExtractString("identifier")));
          break;
        case 907105:
          exception = (Exception) new EnvironmentReferenceExistsException(TaskResources.EnvironmentReferenceExists((object) sqlError.ExtractInt("environmentId")));
          break;
        case 907106:
          exception = (Exception) new PersistedStageGroupMappingAlreadyExists(TaskResources.PersistedStageGroupAlreadyMapped((object) sqlError.ExtractString("stageName"), (object) sqlError.ExtractString("group"), (object) sqlError.ExtractInt("buildId"), (object) sqlError.ExtractInt("definitionId")));
          break;
      }
      return exception;
    }

    private static IDictionary<int, SqlExceptionFactory> CreateExceptionMap() => (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>()
    {
      {
        907006,
        new SqlExceptionFactory(typeof (TaskAgentSessionConflictException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907008,
        new SqlExceptionFactory(typeof (TaskAgentJobNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907009,
        new SqlExceptionFactory(typeof (TaskAgentJobTokenExpiredException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907015,
        new SqlExceptionFactory(typeof (TaskAgentExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907012,
        new SqlExceptionFactory(typeof (TaskAgentNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907007,
        new SqlExceptionFactory(typeof (TaskAgentPoolNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907018,
        new SqlExceptionFactory(typeof (TaskAgentQueueExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907019,
        new SqlExceptionFactory(typeof (TaskAgentQueueNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907013,
        new SqlExceptionFactory(typeof (TaskAgentPoolExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907014,
        new SqlExceptionFactory(typeof (TaskDefinitionExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907016,
        new SqlExceptionFactory(typeof (TaskAgentJobStillRunningException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907055,
        new SqlExceptionFactory(typeof (MetaTaskDefinitionExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907056,
        new SqlExceptionFactory(typeof (MetaTaskDefinitionNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907071,
        new SqlExceptionFactory(typeof (TaskGroupDraftExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907069,
        new SqlExceptionFactory(typeof (TaskGroupPreviewExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907070,
        new SqlExceptionFactory(typeof (TaskGroupUpdateFailedException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907062,
        new SqlExceptionFactory(typeof (TaskGroupRevisionAlreadyExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907063,
        new SqlExceptionFactory(typeof (TaskGroupAlreadyUpdatedException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907057,
        new SqlExceptionFactory(typeof (PackageExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907058,
        new SqlExceptionFactory(typeof (DeploymentMachineGroupExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907059,
        new SqlExceptionFactory(typeof (DeploymentMachineGroupNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907060,
        new SqlExceptionFactory(typeof (VariableGroupExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907061,
        new SqlExceptionFactory(typeof (VariableGroupNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907075,
        new SqlExceptionFactory(typeof (OAuthConfigurationExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907076,
        new SqlExceptionFactory(typeof (OAuthConfigurationNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907064,
        new SqlExceptionFactory(typeof (SecureFileExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907065,
        new SqlExceptionFactory(typeof (SecureFileNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907067,
        new SqlExceptionFactory(typeof (TaskAgentPoolMaintenanceDefinitionNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907066,
        new SqlExceptionFactory(typeof (TaskAgentPoolMaintenanceJobNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907068,
        new SqlExceptionFactory(typeof (TaskAgentPendingUpdateNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907072,
        new SqlExceptionFactory(typeof (DeploymentMachineExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907073,
        new SqlExceptionFactory(typeof (DeploymentGroupExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907074,
        new SqlExceptionFactory(typeof (CannotDeleteAndAddMetadataException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907078,
        new SqlExceptionFactory(typeof (TaskAgentCloudExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907079,
        new SqlExceptionFactory(typeof (TaskAgentCloudNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907080,
        new SqlExceptionFactory(typeof (TaskAgentCloudRequestExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907081,
        new SqlExceptionFactory(typeof (TaskAgentCloudRequestNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907082,
        new SqlExceptionFactory(typeof (TaskAgentPoolReferencesDifferentAgentCloudException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907083,
        new SqlExceptionFactory(typeof (PrivateTaskAgentProvisioningStateInvalidException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907085,
        new SqlExceptionFactory(typeof (EnvironmentExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907086,
        new SqlExceptionFactory(typeof (EnvironmentExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907087,
        new SqlExceptionFactory(typeof (EnvironmentNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907088,
        new SqlExceptionFactory(typeof (EnvironmentResourceExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907105,
        new SqlExceptionFactory(typeof (EnvironmentReferenceExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907089,
        new SqlExceptionFactory(typeof (EnvironmentResourceNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907090,
        new SqlExceptionFactory(typeof (EnvironmentResourcesExceededMaxCountException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907091,
        new SqlExceptionFactory(typeof (EnvironmentExecutionDeploymentHistoryRecordNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907094,
        new SqlExceptionFactory(typeof (EnvironmentPoolExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907095,
        new SqlExceptionFactory(typeof (EnvironmentResourceDeploymentHistoryRecordNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907096,
        new SqlExceptionFactory(typeof (ElasticPoolAlreadyExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907097,
        new SqlExceptionFactory(typeof (ElasticPoolDoesNotExistException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907103,
        new SqlExceptionFactory(typeof (InternalCloudAgentDefinitionExistsException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907104,
        new SqlExceptionFactory(typeof (InternalCloudAgentDefinitionNotFoundException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907024,
        new SqlExceptionFactory(typeof (ExclusiveLockRequestAlreadyQueuedException), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      },
      {
        907106,
        new SqlExceptionFactory(typeof (PersistedStageGroupMappingAlreadyExists), TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException ?? (TaskSqlComponentBase.\u003C\u003EO.\u003C0\u003E__CreateException = new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(TaskSqlComponentBase.CreateException)))
      }
    };

    protected struct SqlMethodScope : IDisposable
    {
      private readonly string m_method;
      private readonly TaskSqlComponentBase m_component;

      public SqlMethodScope(TaskSqlComponentBase component, [CallerMemberName] string method = null)
      {
        this.m_method = method;
        this.m_component = component;
        this.m_component.TraceEnter(0, this.m_method);
      }

      public void Dispose() => this.m_component.TraceLeave(0, this.m_method);
    }
  }
}
