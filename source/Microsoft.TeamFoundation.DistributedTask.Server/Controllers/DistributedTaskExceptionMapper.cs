// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DistributedTaskExceptionMapper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  public static class DistributedTaskExceptionMapper
  {
    public static void Map(ApiExceptionMapping exceptionMap)
    {
      exceptionMap.AddStatusCode<AgentFileNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<AgentMediaTypeNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<AzureStorageAccessKeyFetchFailedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<DataNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DataNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DataNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DataNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.DataSourceNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<DeploymentGroupExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<DeploymentGroupNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DeploymentMachineGroupExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<DeploymentMachineGroupNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DeploymentMachineNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DeploymentPoolInUseException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<EnvironmentExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<EnvironmentNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<EnvironmentExecutionDeploymentHistoryRecordNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidAuthorizationDetailsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidContinuationTokenException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidDataSourceBindingException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidDatasourceException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidDeploymentMachineException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidEndpointResponseException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidJsonPathResponseSelectorException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.InvalidServiceEndpointRequestException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTaskAgentPoolException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTaskAgentVersionException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTaskDefinitionTypeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTaskJsonException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<MetaTaskDefinitionExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<MetaTaskDefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<MetaTaskDefinitionRunsOnMismatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.OAuthConfigurationExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.OAuthConfigurationNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<PackageNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<PackagePropertyUnknownException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<RESTEndpointNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SecureFileExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<SecureFileNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointQueryFailedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<EnvironmentResourceExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<EnvironmentReferenceExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<EnvironmentResourceNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<EnvironmentResourcesExceededMaxCountException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<TaskAgentAccessTokenExpiredException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<TaskAgentExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskAgentCloudRequestNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskAgentCloudRequestAlreadyCompleteException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentJobNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskAgentJobStillRunningException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentJobTokenExpiredException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskAgentPendingUpdateExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskAgentPendingUpdateNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentPoolExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskAgentPoolMaintenanceDefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskAgentPoolMaintenanceJobNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskAgentPoolMaintenanceNotEnabledException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentPoolNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskAgentPoolNotEnoughPermissionsException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<TaskAgentPoolRemovedException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskAgentPoolTypeMismatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentQueueExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskAgentQueueNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskAgentSessionConflictException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskAgentSessionDeletedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentSessionExpiredException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskAgentVersionNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskDefinitionExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskDefinitionExistsWithHigherVersionException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskDefinitionHostContextMismatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskDefinitionInvalidException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskDefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TaskGroupAlreadyUpdatedException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskGroupCyclicDependencyException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskGroupDraftExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskGroupPreviewExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskGroupRevisionAlreadyExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<TaskGroupUpdateFailedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TaskJsonNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TimelineExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<VariableGroupExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<VariableGroupNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.DistributedTask.WebApi.XPathJTokenParseException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FileIdNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<FormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IndexOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidPathException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidAuthorizationDetailsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidEndpointResponseException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.InvalidJsonPathResponseSelectorException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointQueryFailedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.XPathJTokenParseException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UriFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddTranslation<TaskAgentSessionDeletedException, TaskAgentSessionExpiredException>();
    }
  }
}
