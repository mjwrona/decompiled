// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.DeploymentsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required use of types")]
  public class DeploymentsService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Optional parameters")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> ListDeployments(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      int definitionEnvironmentId,
      int releaseId,
      string createdBy,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus,
      bool latestAttemptsOnly,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder queryOrder,
      int top,
      int deploymentContinuationToken,
      DateTime? minModifiedTime,
      DateTime? maxModifiedTime,
      bool isDeleted = false,
      string createdFor = "",
      string branchName = "",
      DateTime? minStartedTime = null,
      DateTime? maxStartedTime = null,
      string artifactTypeId = "",
      string sourceId = "",
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason releaseReason = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.None)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(context, "DataAccessLayer", "DeploymentsService.ListDeployments", 1900004))
      {
        List<Guid> createdByFilter = new List<Guid>();
        if (!string.IsNullOrWhiteSpace(createdBy))
        {
          createdByFilter = context.GetIdentityIds(createdBy).ToList<Guid>();
          releaseManagementTimer.RecordLap("DataAccessLayer", "DeploymentsService.ListDeployments.ReadIdentities", 1971040);
          if (!createdByFilter.Any<Guid>() || createdByFilter.Contains(Guid.Empty))
            return Enumerable.Empty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>();
        }
        List<Guid> createdForFilter = new List<Guid>();
        if (!string.IsNullOrWhiteSpace(createdFor))
        {
          createdForFilter = context.GetIdentityIds(createdFor).ToList<Guid>();
          releaseManagementTimer.RecordLap("DataAccessLayer", "DeploymentsService.ListDeployments.ReadCreatedForIdentities", 1971040);
          if (!createdForFilter.Any<Guid>() || createdForFilter.Contains(Guid.Empty))
            return Enumerable.Empty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>();
        }
        Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>> action = (Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>) (component =>
        {
          DeploymentSqlComponent deploymentSqlComponent = component;
          Guid projectId1 = projectId;
          int releaseDefinitionId = definitionId;
          int releaseDefinitionEnvironmentId = definitionEnvironmentId;
          int releaseId1 = releaseId;
          int num1 = (int) deploymentStatus;
          int operationStatus1 = (int) operationStatus;
          List<Guid> createdByIds = createdByFilter;
          IList<Guid> guidList = (IList<Guid>) createdForFilter;
          int num2 = latestAttemptsOnly ? 1 : 0;
          int queryOrder1 = (int) queryOrder;
          int continuationToken = deploymentContinuationToken;
          int num3 = top;
          int num4 = isDeleted ? 1 : 0;
          DateTime? minModifiedTime1 = minModifiedTime;
          DateTime? maxModifiedTime1 = maxModifiedTime;
          IList<Guid> createdForIds = guidList;
          int maxDeployments = num3;
          string branchName1 = branchName;
          DateTime? minStartedTime1 = minStartedTime;
          DateTime? maxStartedTime1 = maxStartedTime;
          string artifactTypeId1 = artifactTypeId;
          string sourceId1 = sourceId;
          int num5 = (int) releaseReason;
          return deploymentSqlComponent.ListDeployments(projectId1, releaseDefinitionId, releaseDefinitionEnvironmentId, releaseId1, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus) num1, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus) operationStatus1, (IList<Guid>) createdByIds, num2 != 0, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder) queryOrder1, continuationToken, num4 != 0, minModifiedTime1, maxModifiedTime1, createdForIds, maxDeployments, branchName1, minStartedTime1, maxStartedTime1, artifactTypeId1, sourceId1, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason) num5);
        });
        IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> source = context.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>(action);
        return source != null ? (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) source.ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>() : Enumerable.Empty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "projectIdAndDefinitionIdMap can never be null")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public virtual DeploymentsAndDefinitions GetDeploymentsAndDefinitions(
      IVssRequestContext requestContext,
      HashSet<KeyValuePair<Guid, int>> projectIdAndDefinitionIdMap,
      Guid userId)
    {
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "DeploymentsService.GetDeploymentsAndDefinitions", 1900004))
      {
        HashSet<KeyValuePair<int, int>> dataspaceDefinitionIdMap = DeploymentsService.GetDataspaceDefinitionIdMap(requestContext, projectIdAndDefinitionIdMap);
        Func<DeploymentSqlComponent, DeploymentsAndDefinitions> action = (Func<DeploymentSqlComponent, DeploymentsAndDefinitions>) (component => component.GetDeploymentsAndDefinitionsAcrossProjects(userId, dataspaceDefinitionIdMap));
        return requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, DeploymentsAndDefinitions>(action) ?? new DeploymentsAndDefinitions();
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetDeploymentsForMultipleEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      DeploymentQueryParameters queryParameters)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      DeploymentsService.ValidateDeploymentQueryParameters(queryParameters);
      return queryParameters.QueryType == DeploymentsQueryType.FailingSince ? DeploymentsService.QueryDeploymentsFailingSince(requestContext, projectId, queryParameters) : DeploymentsService.QueryDeploymentsForMultipleEnvironments(requestContext, projectId, queryParameters);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required by vssf")]
    public IList<DeploymentIssue> AddDeploymentIssues(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int deploymentId,
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Issue> issues)
    {
      Func<DeploymentSqlComponent, IList<DeploymentIssue>> action = (Func<DeploymentSqlComponent, IList<DeploymentIssue>>) (component => component.AddDeploymentIssues(projectId, releaseId, deploymentId, issues));
      return requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, IList<DeploymentIssue>>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required by vssf")]
    public IList<DeploymentIssue> GetDeploymentIssues(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int deploymentId)
    {
      Func<DeploymentSqlComponent, IList<DeploymentIssue>> action = (Func<DeploymentSqlComponent, IList<DeploymentIssue>>) (component => component.GetDeploymentIssues(projectId, releaseId, deploymentId));
      return requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, IList<DeploymentIssue>>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "IVssService design")]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetLatestDeploymentsByReleaseDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionCount,
      IEnumerable<int> releaseDefinitionIdsToExclude)
    {
      Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>> action = (Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>) (component => component.GetLatestDeploymentsByReleaseDefinitions(projectId, releaseDefinitionCount, releaseDefinitionIdsToExclude));
      return requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "IVssService design")]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetLastDeploymentForReleaseDefinitionIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> releaseDefinitionIds)
    {
      Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>> action = (Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>) (component => component.GetLastDeploymentForReleaseDefinitions(projectId, releaseDefinitionIds));
      return requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "IVssService design")]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> GetDeploymentsCreatedByAnIdentity(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid createdById,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus operationStatus,
      int maxDeployments,
      DateTime minQueuedTime,
      DateTime maxQueuedTime)
    {
      Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>> action = (Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>) (component => component.GetDeploymentsCreatedByAnIdentity(projectId, createdById, deploymentStatus, operationStatus, maxDeployments, minQueuedTime, maxQueuedTime));
      return requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>(action);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "IVssService design")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    public virtual IEnumerable<DeploymentAttemptData> GetDeploymentsByReasonForMultipleEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      HashSet<KeyValuePair<int, int>> definitionEnvironmentReleaseDefinitionIdKeyValuePairs,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentReason deploymentReasonFilter,
      int deploymentsPerEnvironment = 1,
      bool isDeleted = false)
    {
      Func<DeploymentSqlComponent, IEnumerable<DeploymentAttemptData>> action = (Func<DeploymentSqlComponent, IEnumerable<DeploymentAttemptData>>) (component => component.GetDeploymentsByReasonForMultipleEnvironments(projectId, definitionEnvironmentReleaseDefinitionIdKeyValuePairs, deploymentReasonFilter, deploymentsPerEnvironment, isDeleted));
      return requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, IEnumerable<DeploymentAttemptData>>(action);
    }

    private static HashSet<KeyValuePair<int, int>> GetDataspaceDefinitionIdMap(
      IVssRequestContext requestContext,
      HashSet<KeyValuePair<Guid, int>> projectIdAndDefinitionIdMap)
    {
      IDataspaceService service = requestContext.GetService<IDataspaceService>();
      Dictionary<Guid, int> idDataspaceIdMap = DeploymentsService.GetProjectIdDataspaceIdMap(requestContext, (IEnumerable<KeyValuePair<Guid, int>>) projectIdAndDefinitionIdMap, service);
      HashSet<KeyValuePair<int, int>> dataspaceDefinitionIdMap = new HashSet<KeyValuePair<int, int>>();
      foreach (KeyValuePair<Guid, int> idAndDefinitionId in projectIdAndDefinitionIdMap)
      {
        int key;
        if (idDataspaceIdMap.TryGetValue(idAndDefinitionId.Key, out key))
        {
          KeyValuePair<int, int> keyValuePair = new KeyValuePair<int, int>(key, idAndDefinitionId.Value);
          dataspaceDefinitionIdMap.Add(keyValuePair);
        }
      }
      return dataspaceDefinitionIdMap;
    }

    private static Dictionary<Guid, int> GetProjectIdDataspaceIdMap(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, int>> projectIdAndDefinitionIdMap,
      IDataspaceService dataspaceService)
    {
      Dictionary<Guid, int> idDataspaceIdMap = new Dictionary<Guid, int>();
      foreach (Guid guid in projectIdAndDefinitionIdMap.Select<KeyValuePair<Guid, int>, Guid>((Func<KeyValuePair<Guid, int>, Guid>) (f => f.Key)).Distinct<Guid>().ToList<Guid>())
      {
        Dataspace dataspace = dataspaceService.QueryDataspace(requestContext, "ReleaseManagement", guid, false);
        if (dataspace != null)
          idDataspaceIdMap[guid] = dataspace.DataspaceId;
      }
      return idDataspaceIdMap;
    }

    private static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> QueryDeploymentsForMultipleEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      DeploymentQueryParameters queryParameters)
    {
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "DeploymentsService.ListDeployments", 1900004))
      {
        Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>> action = (Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>) (component => component.GetDeploymentsForMultipleEnvironments(projectId, queryParameters.Environments, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus) queryParameters.DeploymentStatus, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus) queryParameters.OperationStatus, queryParameters.ArtifactSourceId, queryParameters.ArtifactTypeId, queryParameters.SourceBranch, queryParameters.ArtifactVersions, queryParameters.DeploymentsPerEnvironment, queryParameters.IsDeleted, queryParameters.Expands));
        return requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>(action);
      }
    }

    private static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> QueryDeploymentsFailingSince(
      IVssRequestContext requestContext,
      Guid projectId,
      DeploymentQueryParameters queryParameters)
    {
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "DeploymentsService.DeploymentsFailingSince", 1900034))
      {
        Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>> action = (Func<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>) (component => component.QueryDeploymentsFailingSince(projectId, queryParameters.Environments, queryParameters.ArtifactTypeId, queryParameters.ArtifactSourceId, queryParameters.SourceBranch, queryParameters.Expands));
        return requestContext.ExecuteWithinUsingWithComponent<DeploymentSqlComponent, IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>>(action);
      }
    }

    private static void ValidateDeploymentQueryParameters(DeploymentQueryParameters queryParameters)
    {
      if (queryParameters == null)
        throw new InvalidRequestException(Resources.QueryDeploymentsInvalidDeploymentQueryParametersError);
      DeploymentsService.ValidateDefinitionEnvironmentsFilter(queryParameters.Environments);
      DeploymentsService.ValidateArtifactFilters(queryParameters.ArtifactTypeId, queryParameters.ArtifactSourceId);
      if (queryParameters.QueryType != DeploymentsQueryType.FailingSince && queryParameters.ArtifactVersions != null && queryParameters.ArtifactVersions.Any<string>())
        return;
      DeploymentsService.ValidateSourceBranchFilter(queryParameters.SourceBranch);
    }

    private static void ValidateArtifactFilters(string artifactTypeId, string artifactSourceId)
    {
      if (string.IsNullOrWhiteSpace(artifactSourceId))
        throw new InvalidRequestException(Resources.QueryDeploymentsInvalidArtifactSourceIdFilter);
      if (string.IsNullOrWhiteSpace(artifactTypeId))
        throw new InvalidRequestException(Resources.QueryDeploymentsInvalidArtifactTypeIdFilter);
    }

    private static void ValidateSourceBranchFilter(string sourceBranch)
    {
      if (string.IsNullOrWhiteSpace(sourceBranch))
        throw new InvalidRequestException(Resources.ArtifactBranchCannotBeNullOrEmpty);
    }

    private static void ValidateDefinitionEnvironmentsFilter(
      IList<DefinitionEnvironmentReference> environments)
    {
      if (environments == null || environments.Count == 0)
        throw new InvalidRequestException(Resources.QueryDeploymentsInvalidEnvironmentsFilter);
      if (environments.Any<DefinitionEnvironmentReference>((Func<DefinitionEnvironmentReference, bool>) (e => e.ReleaseDefinitionId <= 0)))
        throw new InvalidRequestException(Resources.QueryDeploymentsInvalidReleaseDefinitionIdError);
      if (environments.Any<DefinitionEnvironmentReference>((Func<DefinitionEnvironmentReference, bool>) (e => e.DefinitionEnvironmentId <= 0)))
        throw new InvalidRequestException(Resources.QueryDeploymentsInvalidEnvironmentDefinitionIdError);
    }
  }
}
