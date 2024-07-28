// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.ArtifactTraceabilityService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.Pipelines.Deployment.DataAccess;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models;
using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Exceptions;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  public sealed class ArtifactTraceabilityService : 
    IArtifactTraceabilityService,
    IVssFrameworkService
  {
    public void SavingArtifactTraceabilityDataForPipelineRunCompleted(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId)
    {
      ArtifactTraceabilityService.StartPipelineRunTraceabilitySnapshotJob(requestContext, projectId, pipelineRunId);
    }

    public void AddArtifactTraceabilityForRepositoryResource(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData)
    {
      if (artifactTraceabilityData == null)
        return;
      ArtifactTraceabilityHelper.UpdateUniqueResourceIdentifier(requestContext, artifactTraceabilityData);
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
        component.AddArtifactTraceabilityForRepositoryOrContainerResource(artifactTraceabilityData);
    }

    public void AddArtifactTraceabilityForContainerResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      int pipelineRunId,
      ContainerResource containerResource,
      PipelineEnvironment pipelineEnvironment)
    {
      IList<ServiceEndpointReference> endpointReferences = (IList<ServiceEndpointReference>) new List<ServiceEndpointReference>((IEnumerable<ServiceEndpointReference>) pipelineEnvironment.Resources.Endpoints);
      this.SaveArtifactTraceabilityDataForContainer(requestContext, projectId, pipelineDefinitionId, pipelineRunId, endpointReferences, containerResource, pipelineEnvironment?.UserVariables, pipelineEnvironment?.SystemVariables);
    }

    public void AddArtifactTraceabilityForContainerResourceAtJobLevel(
      IVssRequestContext requestContext,
      Guid planId,
      string planType,
      Guid scopeId,
      IList<string> imageUriList,
      string jobId,
      string jobName)
    {
      try
      {
        this.PrepareAndAddArtifactTraceabilityDataForContainerUris(requestContext, planId, planType, scopeId, imageUriList, jobId, jobName);
      }
      catch (Exception ex)
      {
        requestContext.Trace(100161008, TraceLevel.Error, "Deployment", ArtifactTraceabilityConstants.TraceLayer, DeploymentResources.ContainerJobTraceabilityError((object) jobId, (object) ex.Message));
      }
    }

    public void AddArtifactTraceabilityForDownloadTask(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData)
    {
      if (artifactTraceabilityData.IsSelfArtifact)
        ArtifactTraceabilityHelper.UpdateUniqueResourceIdentifier(requestContext, artifactTraceabilityData);
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
      {
        DownloadTaskTraceabilityError errorCode = component.AddArtifactTraceabilityForDownloadTask(artifactTraceabilityData);
        this.TraceDownloadTaskTraceabilityError(requestContext, artifactTraceabilityData, errorCode);
      }
    }

    public void AddArtifactTraceabilityForPipelineResource(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData)
    {
      string input;
      Guid result;
      if (artifactTraceabilityData == null || !artifactTraceabilityData.ArtifactVersionProperties.TryGetValue(ArtifactTraceabilityPropertyKeys.ProjectId, out input) || !Guid.TryParse(input, out result))
        return;
      ArtifactTraceabilityHelper.UpdateUniqueResourceIdentifier(requestContext, artifactTraceabilityData);
      IList<SubArtifactDataRow> subArtifactNameList;
      IList<ArtifactVersionRepoInfo> repoInfoList;
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
        component.GetArtifactNameAndRepoInfoForPipelineRunId(result, artifactTraceabilityData.ArtifactVersionId, out subArtifactNameList, out repoInfoList);
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
      {
        if (subArtifactNameList == null || repoInfoList == null)
          return;
        component.AddArtifactTraceabilityForPipelineResource(artifactTraceabilityData, subArtifactNameList, repoInfoList);
      }
    }

    public void AddArtifactTraceabilityForPublishedArtifact(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData)
    {
      if (artifactTraceabilityData == null)
        return;
      IList<ArtifactVersionRepoInfo> repoInfoList = (IList<ArtifactVersionRepoInfo>) null;
      ArtifactTraceabilityHelper.UpdateUniqueResourceIdentifier(requestContext, artifactTraceabilityData);
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
        repoInfoList = component.GetArtifactVersionRepoInfoForJob(artifactTraceabilityData.ProjectId, artifactTraceabilityData.PipelineRunId, artifactTraceabilityData.JobId);
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
        component.AddArtifactTraceabilityForPublishedArtifact(artifactTraceabilityData, repoInfoList);
    }

    public int GetArtifactsCountForPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      IList<string> aliases = null,
      IList<ArtifactCategory> artifactCategories = null,
      bool includeSelfRepo = true)
    {
      int num = 0;
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnableArtifactTraceability"))
        return 0;
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
        num = component.GetArtifactsCountForPipeline(projectId, pipelineRunId, aliases, artifactCategories);
      return !includeSelfRepo && num >= 0 && this.IncludesRepositoryCategory(artifactCategories) ? num : num + 1;
    }

    public IList<ArtifactVersion> GetArtifactTraceabilityDataForPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      IList<string> aliases = null,
      bool includeSourceDetails = false,
      IList<ArtifactCategory> artifactCategories = null,
      bool includeSelfRepo = true)
    {
      IList<ArtifactVersion> artifactVersions = (IList<ArtifactVersion>) null;
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnableArtifactTraceability"))
        return (IList<ArtifactVersion>) Enumerable.Empty<ArtifactVersion>().ToList<ArtifactVersion>();
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
        artifactVersions = component.GetArtifactTraceabilityDataForPipelineOrJob(requestContext, projectId, pipelineRunId, (string) null, aliases, includeSourceDetails, artifactCategories);
      if (this.IncludesRepositoryCategory(artifactCategories))
      {
        if (!includeSelfRepo)
          this.RemoveSelfRepositoryArtifactVersion(artifactVersions);
        this.CleanupSelfRepoSubArtifacts(artifactVersions);
      }
      return artifactVersions;
    }

    public IList<ArtifactVersion> GetArtifactTraceabilityDataForJob(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      string jobId,
      IList<string> aliases = null,
      bool includeSourceDetails = false,
      IList<ArtifactCategory> artifactCategories = null,
      bool includeSelfRepo = true)
    {
      IList<ArtifactVersion> artifactVersions = (IList<ArtifactVersion>) null;
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnableArtifactTraceability"))
        return (IList<ArtifactVersion>) Enumerable.Empty<ArtifactVersion>().ToList<ArtifactVersion>();
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
        artifactVersions = component.GetArtifactTraceabilityDataForPipelineOrJob(requestContext, projectId, pipelineRunId, jobId, aliases, includeSourceDetails, artifactCategories);
      if (this.IncludesRepositoryCategory(artifactCategories))
      {
        if (!includeSelfRepo)
          this.RemoveSelfRepositoryArtifactVersion(artifactVersions);
        this.CleanupSelfRepoSubArtifacts(artifactVersions);
      }
      return artifactVersions;
    }

    public IArtifactTraceabilityService GetArtifactTraceabilityService(string name) => (IArtifactTraceabilityService) null;

    public string GetServiceName() => ArtifactTraceabilityConstants.ArtifactTraceabilityDeploymentService;

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IList<CDPipelineRunData> GetCDPipelinesRunsDataForCIPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId,
      int paginationLimit,
      int continuationToken)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnableCDPipelinesTabForCIPipelinesRun"))
        return (IList<CDPipelineRunData>) Enumerable.Empty<CDPipelineRunData>().ToList<CDPipelineRunData>();
      if (pipelineRunId <= 0 || paginationLimit <= 0 || continuationToken < 0)
      {
        string message = "Invalid data while fetching data for CDPipelinesRuns For CIPipelineRun" + string.Format("projectId: {0}, pipelineRunId: {1}, paginationLimit: {2}, continuationToken: {3}", (object) projectId.ToString(), (object) pipelineRunId, (object) paginationLimit, (object) continuationToken);
        requestContext.Trace(100161018, TraceLevel.Error, "Deployment", ArtifactTraceabilityConstants.TraceLayer, message);
        return (IList<CDPipelineRunData>) Enumerable.Empty<CDPipelineRunData>().ToList<CDPipelineRunData>();
      }
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
        return component.GetCDPipelinesRunsDataForCIPipeline(requestContext, projectId, pipelineRunId, paginationLimit, continuationToken);
    }

    public IList<CDPipelineRunData> GetPipelineRunsUsingExistingPipelineRun(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      Guid existingPipelineProjectId,
      int existingPipelineDefinitionId,
      int existingPipelineRunId)
    {
      if (pipelineDefinitionId <= 0 || existingPipelineDefinitionId < 0 || existingPipelineRunId <= 0)
      {
        string message = "Invalid data while fetching data for GetPipelineRunsUsingExistingPipelineRun" + string.Format("projectId: {0}, pipelineDefinitionId: {1}, existingPipelineProjectId: {2}, existingPipelineDefinitionId: {3}, existingPipelineRunId: {4}", (object) projectId, (object) pipelineDefinitionId, (object) existingPipelineProjectId, (object) existingPipelineDefinitionId, (object) existingPipelineRunId);
        requestContext.Trace(100161018, TraceLevel.Error, "Deployment", ArtifactTraceabilityConstants.TraceLayer, message);
        return (IList<CDPipelineRunData>) Enumerable.Empty<CDPipelineRunData>().ToList<CDPipelineRunData>();
      }
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
      {
        string resourceIdentifier = UniqueResourceIdentiferHelper.GenerateUniqueResourceIdentifier(requestContext, existingPipelineProjectId, "Pipeline", (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            PipelinePropertyNames.DefinitionId,
            existingPipelineDefinitionId.ToString()
          },
          {
            PipelinePropertyNames.ProjectId,
            existingPipelineProjectId.ToString()
          }
        });
        return component.GetPipelineRunsUsingExistingPipelineRun(projectId, pipelineDefinitionId, resourceIdentifier, existingPipelineRunId);
      }
    }

    private static void StartPipelineRunTraceabilitySnapshotJob(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineRunId)
    {
      if (!requestContext.IsFeatureEnabled("Pipelines.Traceability.EnableTraceabilityForPipelineRun"))
        return;
      string jobName = "SavePipelineRunTraceabilitySnapshot";
      string extensionName = "Microsoft.TeamFoundation.Build2.Server.Extensions.PipelineRunTraceabilitySnapshotJob";
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new PipelineRunTraceabilityJobData()
      {
        CurrentRunId = pipelineRunId,
        ProjectId = projectId
      });
      ArgumentUtility.CheckForNull<XmlNode>(xml, "jobData");
      Guid guid = requestContext.GetService<TeamFoundationJobService>().QueueOneTimeJob(requestContext, jobName, extensionName, xml, JobPriorityLevel.Normal);
      requestContext.Trace(100161017, TraceLevel.Info, "Deployment", nameof (ArtifactTraceabilityService), "{0} job started, JobId: {1}", (object) jobName, (object) guid);
    }

    private void AddArtifactTraceabilityDataForContainerResource(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData)
    {
      if (artifactTraceabilityData == null)
        return;
      ArtifactTraceabilityHelper.UpdateUniqueResourceIdentifier(requestContext, artifactTraceabilityData);
      using (ArtifactTraceabilityComponent component = requestContext.CreateComponent<ArtifactTraceabilityComponent>())
        component.AddArtifactTraceabilityForRepositoryOrContainerResource(artifactTraceabilityData);
    }

    private void PrepareAndAddArtifactTraceabilityDataForContainerUris(
      IVssRequestContext requestContext,
      Guid planId,
      string planType,
      Guid scopeId,
      IList<string> imageUriList,
      string jobId,
      string jobName)
    {
      if (planId != Guid.Empty && !string.IsNullOrWhiteSpace(planType) && scopeId != Guid.Empty && imageUriList != null && imageUriList.Count > 0 && !string.IsNullOrWhiteSpace(jobId) && !string.IsNullOrWhiteSpace(jobName))
      {
        TaskOrchestrationPlan plan = this.GetTaskHub(requestContext, planType, scopeId).GetPlan(requestContext, scopeId, planId);
        PipelineEnvironment pipelineEnvironment = plan != null && plan.ProcessEnvironment is PipelineEnvironment ? plan.ProcessEnvironment as PipelineEnvironment : throw new TaskOrchestrationPlanNotFoundException(DeploymentResources.PlanNotFound((object) planId));
        PipelineResources resources = pipelineEnvironment.Resources;
        IDictionary<string, ContainerResource> containerResourcesMap = this.GetContainerResourcesMap(requestContext, resources, pipelineEnvironment.UserVariables);
        IList<ServiceEndpointReference> endpointReferences = (IList<ServiceEndpointReference>) new List<ServiceEndpointReference>((IEnumerable<ServiceEndpointReference>) resources.Endpoints);
        foreach (string imageUri in (IEnumerable<string>) imageUriList)
        {
          ContainerResource containerResource;
          if (containerResourcesMap.TryGetValue(imageUri, out containerResource))
            this.SaveArtifactTraceabilityDataForContainer(requestContext, scopeId, plan.Definition.Id, plan.Owner.Id, endpointReferences, containerResource, pipelineEnvironment.UserVariables, pipelineEnvironment.SystemVariables, jobId, jobName);
        }
      }
      else
      {
        string message = "Invalid data while saving traceability data for ContainerUris. planId: " + planId.ToString() + ", planType: " + planType + ", scopeId : " + scopeId.ToString() + ", " + string.Format("imageUriList is null ? : {0}, job id : {1}, job name: {2}", (object) (imageUriList == null), (object) jobId, (object) jobName);
        ArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Info, message);
        throw new InvalidParameterException(DeploymentResources.InvalidParameterFound((object) nameof (ArtifactTraceabilityService)));
      }
    }

    private void SaveArtifactTraceabilityDataForContainer(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      int pipelineRunId,
      IList<ServiceEndpointReference> endpointReferences,
      ContainerResource containerResource,
      IList<IVariable> userVariables,
      IDictionary<string, VariableValue> systemVariables,
      string jobId = null,
      string jobName = null)
    {
      try
      {
        ArtifactTraceabilityData artifactTraceabilityData = this.PrepareArtifactTraceabilityDataForContainer(requestContext, projectId, pipelineDefinitionId, pipelineRunId, endpointReferences, containerResource, userVariables, systemVariables, jobId, jobName);
        if (artifactTraceabilityData == null)
          return;
        this.AddArtifactTraceabilityDataForContainerResource(requestContext, artifactTraceabilityData);
      }
      catch (Exception ex)
      {
        string message = "Exception occured while saving traceability data. Details: ProjectId: " + projectId.ToString() + ", " + string.Format("definitionId: {0}, runId : {1}, containerResource is null? : {2}, ", (object) pipelineDefinitionId, (object) pipelineRunId, (object) (containerResource == null)) + string.Format("userVariables is null ? {0}, systemVariables is null ? {1}, job id : {2}, job name: {3} ", (object) (userVariables == null), (object) (systemVariables == null), (object) jobId, (object) jobName) + "Exception: " + ex.ToString();
        ArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message);
      }
    }

    private ArtifactTraceabilityData PrepareArtifactTraceabilityDataForContainer(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      int pipelineRunId,
      IList<ServiceEndpointReference> endpointReferences,
      ContainerResource containerResource,
      IList<IVariable> userVariables,
      IDictionary<string, VariableValue> systemVariables,
      string jobId = null,
      string jobName = null)
    {
      if (projectId != Guid.Empty && containerResource != null && userVariables != null)
      {
        ArtifactTraceabilityData traceabilityData = new ArtifactTraceabilityData()
        {
          ArtifactCategory = ArtifactCategory.Container,
          ArtifactAlias = containerResource.Alias,
          ProjectId = projectId,
          PipelineDefinitionId = pipelineDefinitionId,
          PipelineRunId = pipelineRunId,
          ResourceProperties = containerResource.GetResourceInputs()
        };
        if (containerResource.Endpoint != null && containerResource.Endpoint.Id != Guid.Empty)
        {
          ServiceEndpointReference resourceEndpointReference = new ServiceEndpointReference()
          {
            Id = containerResource.Endpoint.Id
          };
          traceabilityData.ArtifactConnectionData = this.GetArtifactConnectionData(requestContext, projectId, resourceEndpointReference, endpointReferences);
        }
        else
        {
          string str;
          if (containerResource.Properties.TryGetValue<string>("azureSubscription", out str))
          {
            ServiceEndpointReference endpointReference = new ServiceEndpointReference();
            endpointReference.Name = (ExpressionValue<string>) str;
            ServiceEndpointReference resourceEndpointReference = endpointReference;
            traceabilityData.ArtifactConnectionData = this.GetArtifactConnectionData(requestContext, projectId, resourceEndpointReference, endpointReferences);
          }
        }
        IDictionary<string, string> variablesDictionary = this.GetVariablesDictionary(requestContext, userVariables);
        string type;
        if (variablesDictionary.TryGetValue(WellKnownContainerArtifactVariables.GetVariableKey(containerResource.Alias, "type"), out type))
        {
          traceabilityData.ArtifactType = ArtifactTraceabilityHelper.ConvertContainerTypeFromACRToAzureContainerRepository(type);
          string str1;
          if (variablesDictionary.TryGetValue(WellKnownContainerArtifactVariables.GetVariableKey(containerResource.Alias, "digest"), out str1))
            traceabilityData.ArtifactVersionId = str1;
          string str2;
          if (variablesDictionary.TryGetValue(WellKnownContainerArtifactVariables.GetVariableKey(containerResource.Alias, "tag"), out str2))
            traceabilityData.ArtifactVersionName = str2;
          string str3;
          if (variablesDictionary.TryGetValue(WellKnownContainerArtifactVariables.GetVariableKey(containerResource.Alias, "repository"), out str3))
            traceabilityData.ArtifactName = str3;
          string str4;
          if (variablesDictionary.TryGetValue(WellKnownContainerArtifactVariables.GetVariableKey(containerResource.Alias, "registry"), out str4))
            traceabilityData.ArtifactVersionProperties.Add(ArtifactTraceabilityPropertyKeys.ContainerRegistry, str4);
          string str5;
          if (variablesDictionary.TryGetValue(WellKnownContainerArtifactVariables.GetVariableKey(containerResource.Alias, "location"), out str5))
            traceabilityData.ArtifactVersionProperties.Add(ArtifactTraceabilityPropertyKeys.ContainerLocation, str5);
          string containerUri;
          if (variablesDictionary.TryGetValue(WellKnownContainerArtifactVariables.GetVariableKey(containerResource.Alias, "URI"), out containerUri))
          {
            traceabilityData.ArtifactVersionProperties.Add(ArtifactTraceabilityPropertyKeys.ContainerUri, containerUri);
            IImageDetailsRepositoryService service = requestContext.GetService<IImageDetailsRepositoryService>();
            containerUri = this.AddHttpsToContainerUri(containerUri);
            string reverseDnsHostName = this.GetReverseDNSHostName(systemVariables);
            if (!string.IsNullOrWhiteSpace(reverseDnsHostName))
              traceabilityData.Resource = service.GetImageRepositoryDetails(requestContext, projectId, containerUri, reverseDnsHostName);
          }
          if (!string.IsNullOrWhiteSpace(jobId) && !string.IsNullOrWhiteSpace(jobName))
          {
            traceabilityData.JobId = jobId;
            traceabilityData.JobName = jobName;
          }
          return traceabilityData;
        }
        string message = DeploymentResources.MissingContainerResourceField((object) "type", (object) containerResource.Alias) + ". " + string.Format("Other Details: projectId: {0}, pipelineDefinitionId: {1}, pipelineRunId: {2}", (object) projectId.ToString(), (object) pipelineDefinitionId, (object) pipelineRunId);
        ArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Warning, message);
        return (ArtifactTraceabilityData) null;
      }
      string message1 = "Invalid input value while preparing traceability data for container. ProjectId: " + projectId.ToString() + "," + string.Format(" definitionId : {0}, runId : {1}, ContainerResource : {2}, ", (object) pipelineDefinitionId, (object) pipelineRunId, (object) containerResource) + string.Format("userVariables : {0}, Job Id: {1}, Job Name: {2}", (object) userVariables, (object) jobId, (object) jobName);
      ArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Info, message1);
      return (ArtifactTraceabilityData) null;
    }

    private string GetReverseDNSHostName(IDictionary<string, VariableValue> systemVariables)
    {
      VariableValue variableValue;
      if (systemVariables != null && systemVariables.TryGetValue(ImageDetailsRepositoryPropertyKeys.TeamFoundationCollectionURIKey, out variableValue))
      {
        Uri uri = new Uri(variableValue.Value);
        IEnumerable<string> strings;
        if ((object) uri == null)
        {
          strings = (IEnumerable<string>) null;
        }
        else
        {
          string host = uri.Host;
          if (host == null)
            strings = (IEnumerable<string>) null;
          else
            strings = ((IEnumerable<string>) host.Split('.')).Reverse<string>();
        }
        IEnumerable<string> values = strings;
        if (values != null)
          return string.Join(".", values);
      }
      return (string) null;
    }

    private string AddHttpsToContainerUri(string containerUri) => !string.IsNullOrWhiteSpace(containerUri) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://{0}", (object) containerUri) : containerUri;

    private IDictionary<string, ContainerResource> GetContainerResourcesMap(
      IVssRequestContext requestContext,
      PipelineResources resources,
      IList<IVariable> userVariables)
    {
      IDictionary<string, ContainerResource> containerResourcesMap = (IDictionary<string, ContainerResource>) new Dictionary<string, ContainerResource>();
      IDictionary<string, string> variablesDictionary = this.GetVariablesDictionary(requestContext, userVariables);
      foreach (ContainerResource container in (IEnumerable<ContainerResource>) resources?.Containers)
      {
        string containerUri;
        if (variablesDictionary.TryGetValue(WellKnownContainerArtifactVariables.GetVariableKey(container.Alias, "URI"), out containerUri))
        {
          containerUri = this.AddHttpsToContainerUri(containerUri);
          if (!containerResourcesMap.ContainsKey(containerUri))
            containerResourcesMap.Add(containerUri, container);
        }
      }
      return containerResourcesMap;
    }

    private EndpointReferenceData GetArtifactConnectionData(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpointReference resourceEndpointReference,
      IList<ServiceEndpointReference> endpointReferences)
    {
      EndpointReferenceData artifactConnectionData = (EndpointReferenceData) null;
      if (resourceEndpointReference != null)
      {
        Guid result = Guid.Empty;
        if (resourceEndpointReference.Id != Guid.Empty)
          result = resourceEndpointReference.Id;
        else if (resourceEndpointReference.Name != (ExpressionValue<string>) null && endpointReferences != null)
        {
          foreach (ServiceEndpointReference endpointReference in (IEnumerable<ServiceEndpointReference>) endpointReferences)
          {
            if (object.Equals((object) endpointReference.Name, (object) resourceEndpointReference.Name))
            {
              result = endpointReference.Id;
              break;
            }
          }
        }
        if (result == Guid.Empty && resourceEndpointReference.Name != (ExpressionValue<string>) null)
        {
          string connectionId = this.GetConnectionId(requestContext, projectId, resourceEndpointReference.Name.ToString());
          if (connectionId != null)
            Guid.TryParse(connectionId, out result);
        }
        if (result != Guid.Empty)
        {
          IServiceEndpointService2 service = requestContext.GetService<IServiceEndpointService2>();
          if (service != null)
          {
            Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = service.GetServiceEndpoint(requestContext, projectId, result);
            if (serviceEndpoint != null)
              artifactConnectionData = new EndpointReferenceData()
              {
                Id = serviceEndpoint.Id,
                Url = serviceEndpoint.Url
              };
          }
        }
      }
      return artifactConnectionData;
    }

    private string GetConnectionId(
      IVssRequestContext requestContext,
      Guid projectId,
      string endpointName)
    {
      Guid result;
      IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> source;
      if (Guid.TryParse(endpointName, out result))
        source = DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext, projectId, "AzureRM", (IEnumerable<string>) null, (IEnumerable<Guid>) new List<Guid>()
        {
          result
        }, true, false, ServiceEndpointActionFilter.None);
      else
        source = DistributedTaskEndpointServiceHelper.QueryServiceEndpoints(requestContext, projectId, "AzureRM", (IEnumerable<string>) null, (IEnumerable<string>) new List<string>()
        {
          endpointName
        }, true, false, ServiceEndpointActionFilter.None);
      if (source.Count<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>() == 1)
        return source.First<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>().Id.ToString();
      if (source.Count<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>() > 1)
        throw new ArgumentException(DeploymentResources.AmbigousServiceEndpointUsed((object) endpointName));
      throw new ArgumentException(DeploymentResources.EndpointOfTypeNotFound((object) "AzureRM", (object) endpointName));
    }

    private IDictionary<string, string> GetVariablesDictionary(
      IVssRequestContext requestContext,
      IList<IVariable> variables)
    {
      IDictionary<string, string> variablesDictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      bool flag = false;
      if (variables != null)
      {
        foreach (IVariable variable1 in (IEnumerable<IVariable>) variables)
        {
          if (typeof (Variable).IsInstanceOfType((object) variable1))
          {
            Variable variable2 = (Variable) variable1;
            if (!variablesDictionary.ContainsKey(variable2.Name))
              variablesDictionary.Add(variable2.Name, variable2.Value);
          }
          else
            flag = true;
        }
        if (flag)
        {
          string message = "One or more variable not an instance of Variable class.";
          ArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Warning, message);
        }
      }
      return variablesDictionary;
    }

    private TaskHub GetTaskHub(IVssRequestContext requestContext, string hubName, Guid scopeId)
    {
      TaskHub taskHub = requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, hubName, false);
      taskHub.CreateScope(requestContext, scopeId);
      return taskHub;
    }

    private bool IsSelfRepositoryArtifact(ArtifactVersion artifactVersion) => artifactVersion != null && artifactVersion.ArtifactCategory == ArtifactCategory.Repository && ArtifactTraceabilityConstants.SelfArtifactAlias.Equals(artifactVersion.Alias, StringComparison.OrdinalIgnoreCase);

    private bool IncludesRepositoryCategory(IList<ArtifactCategory> artifactCategories) => artifactCategories != null && artifactCategories.Contains(ArtifactCategory.Repository) || artifactCategories != null && artifactCategories.Contains(ArtifactCategory.All);

    private IList<ArtifactVersion> RemoveSelfRepositoryArtifactVersion(
      IList<ArtifactVersion> artifactVersions)
    {
      IList<ArtifactVersion> source = artifactVersions;
      if (source != null)
        source.ToList<ArtifactVersion>()?.ForEach((Action<ArtifactVersion>) (artifactVersion =>
        {
          if (!this.IsSelfRepositoryArtifact(artifactVersion))
            return;
          artifactVersions.Remove(artifactVersion);
        }));
      return artifactVersions;
    }

    private void CleanupSelfRepoSubArtifacts(IList<ArtifactVersion> artifactVersions)
    {
      if (artifactVersions == null)
        return;
      artifactVersions.ToList<ArtifactVersion>()?.ForEach((Action<ArtifactVersion>) (artifactVersion =>
      {
        if (artifactVersion.ArtifactCategory != ArtifactCategory.Repository || this.IsSelfRepositoryArtifact(artifactVersion))
          return;
        IList<SubArtifactVersion> artifactVersions1 = artifactVersion.SubArtifactVersions;
        if (artifactVersions1 == null)
          return;
        artifactVersions1.ToList<SubArtifactVersion>()?.ForEach((Action<SubArtifactVersion>) (subArtifactVersion =>
        {
          if (!ArtifactTraceabilityConstants.GenericArtifactName.Equals(subArtifactVersion.ArtifactName))
            return;
          artifactVersion.SubArtifactVersions.Remove(subArtifactVersion);
        }));
      }));
    }

    private void TraceDownloadTaskTraceabilityError(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData artifactTraceabilityData,
      DownloadTaskTraceabilityError errorCode)
    {
      string str = "";
      switch (errorCode)
      {
        case DownloadTaskTraceabilityError.NoTraceabilityDataError:
          str = "No traceability data found";
          break;
        case DownloadTaskTraceabilityError.NoPublishedArtifactsFoundError:
          str = "No published arifacts found";
          break;
        case DownloadTaskTraceabilityError.UnknownError:
          str = "Some Unknown error occured";
          break;
      }
      if (string.IsNullOrEmpty(str))
        return;
      string message = str + " while tracing artifact traceability data for download task. Data : " + artifactTraceabilityData?.ToString();
      ArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message);
    }

    private static void TraceMessageForArtifactTraceability(
      IVssRequestContext requestContext,
      TraceLevel traceLevel,
      string message)
    {
      if (requestContext == null)
        return;
      requestContext.TraceAlways(100161008, traceLevel, "Deployment", ArtifactTraceabilityConstants.TraceLayer, message);
    }
  }
}
