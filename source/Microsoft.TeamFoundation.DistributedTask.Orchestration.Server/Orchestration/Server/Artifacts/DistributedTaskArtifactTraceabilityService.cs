// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DistributedTaskArtifactTraceabilityService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class DistributedTaskArtifactTraceabilityService : 
    IDistributedTaskArtifactTraceabilityService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void SaveArtifactTraceabilityDataForPipelineRun(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      PipelineEnvironment pipelineEnvironment)
    {
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        PipelineResources resources = pipelineEnvironment?.Resources;
        if (definitionReference == null || ownerReference == null || resources == null)
          throw new ArgumentNullException("Invalid input received while saving artifact traceability data for pipeline run. Input parameters: definitionReference, ownerReference or resources is/are null");
        IArtifactTraceabilityService traceabilityService = requestContext.GetService<IArtifactTraceabilityService>()?.GetArtifactTraceabilityService(ArtifactTraceabilityConstants.ArtifactTraceabilityDeploymentService);
        IList<ServiceEndpointReference> endpointReferences = (IList<ServiceEndpointReference>) new List<ServiceEndpointReference>((IEnumerable<ServiceEndpointReference>) resources.Endpoints);
        int num1 = 0;
        int num2 = 0;
        foreach (RepositoryResource repository in (IEnumerable<RepositoryResource>) resources.Repositories)
        {
          if (DistributedTaskArtifactTraceabilityService.SaveArtifactTraceabilityDataForRepository(requestContext, projectId, definitionReference, ownerReference, endpointReferences, repository, traceabilityService))
            ++num1;
          else
            ++num2;
        }
        stringBuilder.AppendLine(string.Format("PipelineRunId: {0}, Repositories_Count: {1}, passedCount: {2}, failedCount: {3}", (object) ownerReference?.Id, (object) resources?.Repositories?.Count.GetValueOrDefault(), (object) num1, (object) num2));
        int num3 = 0;
        int num4 = 0;
        foreach (PipelineResource pipeline in (IEnumerable<PipelineResource>) resources.Pipelines)
        {
          if (DistributedTaskArtifactTraceabilityService.SaveArtifactTraceabilityDataForPipeline(requestContext, projectId, definitionReference, ownerReference, endpointReferences, pipeline, traceabilityService))
            ++num3;
          else
            ++num4;
        }
        stringBuilder.AppendLine(string.Format("Pipelines_Count: {0}, passedCount: {1}, failedCount: {2}", (object) resources?.Pipelines?.Count.GetValueOrDefault(), (object) num3, (object) num4));
        int num5 = 0;
        int num6 = 0;
        foreach (ContainerResource container in (IEnumerable<ContainerResource>) resources.Containers)
        {
          if (DistributedTaskArtifactTraceabilityService.SaveArtifactTraceabilityForContainerResource(requestContext, projectId, definitionReference.Id, ownerReference.Id, container, pipelineEnvironment, traceabilityService))
            ++num5;
          else
            ++num6;
        }
        stringBuilder.AppendLine(string.Format("Containers_Count: {0}, passedCount: {1}, failedCount: {2}", (object) resources?.Containers?.Count.GetValueOrDefault(), (object) num5, (object) num6));
        traceabilityService.SavingArtifactTraceabilityDataForPipelineRunCompleted(requestContext, projectId, ownerReference.Id);
      }
      catch (Exception ex)
      {
        string message = "Exception occured while saving traceability data. " + string.Format("Details: ProjectId: {0}, definitionId: {1}, ", (object) projectId.ToString(), (object) definitionReference?.Id) + string.Format("runId : {0}, resources is null? : {1} ", (object) ownerReference?.Id, (object) (pipelineEnvironment?.Resources == null)) + "Exception : " + ex.ToString();
        DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message);
      }
      finally
      {
        if (requestContext != null)
          requestContext.TraceAlways(10016215, ArtifactTraceabilityConstants.TraceLayer, stringBuilder.ToString());
      }
    }

    public void SaveArtifactTraceabilityDataForJob(
      IVssRequestContext requestContext,
      IResourceStore pipelineResources,
      TaskOrchestrationPlan plan,
      Job job,
      string phaseOrchestrationId)
    {
      try
      {
        if (job != null && plan != null && plan.Definition != null && plan.Owner != null && pipelineResources != null)
        {
          IList<ServiceEndpointReference> authorizedReferences = pipelineResources.Endpoints?.GetAuthorizedReferences();
          IArtifactTraceabilityService traceabilityService = requestContext.GetService<IArtifactTraceabilityService>()?.GetArtifactTraceabilityService(ArtifactTraceabilityConstants.ArtifactTraceabilityDeploymentService);
          foreach (TaskStep step in (IEnumerable<JobStep>) job.Steps)
          {
            if (step.IsCheckoutTask())
            {
              if (pipelineResources.Repositories != null && step.Inputs.ContainsKey("repository"))
              {
                string input = step.Inputs["repository"];
                RepositoryResource repoResource = pipelineResources.Repositories.Get(input);
                DistributedTaskArtifactTraceabilityService.SaveArtifactTraceabilityDataForRepository(requestContext, plan.ScopeIdentifier, plan.Definition, plan.Owner, authorizedReferences, repoResource, traceabilityService, job, phaseOrchestrationId);
              }
            }
            else if (DistributedTaskArtifactTraceabilityService.IsTraceableDownloadTask(step))
              DistributedTaskArtifactTraceabilityService.SaveArtifactTraceabilityDataForDownloadTask(requestContext, plan.ScopeIdentifier, plan.Definition, plan.Owner, step.Inputs, traceabilityService, job, phaseOrchestrationId);
          }
        }
        else
        {
          Guid guid;
          string str1;
          if (plan == null)
          {
            str1 = (string) null;
          }
          else
          {
            guid = plan.ScopeIdentifier;
            str1 = guid.ToString();
          }
          // ISSUE: variable of a boxed type
          __Boxed<int?> id1 = (ValueType) plan?.Definition?.Id;
          // ISSUE: variable of a boxed type
          __Boxed<int?> id2 = (ValueType) plan?.Owner?.Id;
          string str2 = string.Format("ProjectId: {0}, definitionId: {1}, runId : {2}, ", (object) str1, (object) id1, (object) id2);
          // ISSUE: variable of a boxed type
          __Boxed<bool> local = (ValueType) (pipelineResources == null);
          string str3;
          if (job == null)
          {
            str3 = (string) null;
          }
          else
          {
            guid = job.Id;
            str3 = guid.ToString();
          }
          string name = job?.Name;
          string str4 = string.Format("pipelineResources is null? : {0}, job Id:  {1}, Job Name: {2}", (object) local, (object) str3, (object) name);
          string message = "Invalid input data while saving traceability data for a job. " + str2 + str4;
          DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Info, message);
        }
      }
      catch (Exception ex)
      {
        string[] strArray = new string[10];
        strArray[0] = "Exception occured while saving traceability data for a job. Details: ProjectId: ";
        Guid guid;
        string str5;
        if (plan == null)
        {
          str5 = (string) null;
        }
        else
        {
          guid = plan.ScopeIdentifier;
          str5 = guid.ToString();
        }
        strArray[1] = str5;
        strArray[2] = ", ";
        strArray[3] = string.Format("definitionId: {0}, runId : {1}, pipelineResources is null? : {2}, ", (object) plan?.Definition?.Id, (object) plan?.Owner?.Id, (object) (pipelineResources == null));
        strArray[4] = "job Id:  ";
        string str6;
        if (job == null)
        {
          str6 = (string) null;
        }
        else
        {
          guid = job.Id;
          str6 = guid.ToString();
        }
        strArray[5] = str6;
        strArray[6] = ", Job Name: ";
        strArray[7] = job?.Name;
        strArray[8] = ". Exception : ";
        strArray[9] = ex.ToString();
        string message = string.Concat(strArray);
        DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message);
      }
    }

    private static void SaveArtifactTraceabilityDataForDownloadTask(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      IDictionary<string, string> taskInputs,
      IArtifactTraceabilityService service,
      Job job,
      string phaseOrchestrationId)
    {
      try
      {
        ArtifactTraceabilityData artifactTraceabilityData = DistributedTaskArtifactTraceabilityService.PrepareArtifactTraceabilityDataForDownloadTask(requestContext, projectId, definitionReference, ownerReference, taskInputs, job, phaseOrchestrationId);
        if (artifactTraceabilityData == null)
          return;
        service.AddArtifactTraceabilityForDownloadTask(requestContext, artifactTraceabilityData);
      }
      catch (Exception ex)
      {
        string message = "Exception occured while saving traceability data for download task. Details: ProjectId: " + projectId.ToString() + ", " + string.Format("definitionId: {0}, runId : {1}, ( taskInputs : {2} ), ", (object) definitionReference?.Id, (object) ownerReference?.Id, (object) DistributedTaskArtifactTraceabilityService.PrintDictionary(taskInputs)) + string.Format("jobId {0}, jobName: {1}, phaseOrchestrationId: {2}, service is null ?: {3} ", (object) job?.Id, (object) job?.Name, (object) phaseOrchestrationId, (object) (service == null)) + "Exception message : " + ex.ToString();
        DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message);
      }
    }

    private static bool SaveArtifactTraceabilityDataForRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      IList<ServiceEndpointReference> endpointReferences,
      RepositoryResource repoResource,
      IArtifactTraceabilityService service,
      Job job = null,
      string phaseOrchestrationId = null)
    {
      try
      {
        ArtifactTraceabilityData artifactTraceabilityData = DistributedTaskArtifactTraceabilityService.PrepareArtifactTraceabilityDataForRepository(requestContext, projectId, definitionReference, ownerReference, endpointReferences, repoResource, job, phaseOrchestrationId);
        if (artifactTraceabilityData != null)
        {
          service.AddArtifactTraceabilityForRepositoryResource(requestContext, artifactTraceabilityData);
          return true;
        }
      }
      catch (Exception ex)
      {
        string message = "Exception occured while saving traceability data for repository. Details: ProjectId: " + projectId.ToString() + ", " + string.Format("definitionId: {0}, runId : {1}, repoResource is null? : {2}, ", (object) definitionReference?.Id, (object) ownerReference?.Id, (object) (repoResource == null)) + string.Format("jobId {0}, jobName: {1}, phaseOrchestrationId: {2}, service is null ?: {3} ", (object) job?.Id, (object) job?.Name, (object) phaseOrchestrationId, (object) (service == null)) + "Exception : " + ex.ToString();
        DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message);
      }
      return false;
    }

    private static bool SaveArtifactTraceabilityDataForPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      IList<ServiceEndpointReference> endpointReferences,
      PipelineResource pipelineResource,
      IArtifactTraceabilityService service)
    {
      try
      {
        ArtifactTraceabilityData artifactTraceabilityData = DistributedTaskArtifactTraceabilityService.PrepareArtifactTraceabilityDataForPipeline(requestContext, projectId, definitionReference, ownerReference, endpointReferences, pipelineResource);
        if (artifactTraceabilityData != null)
        {
          service.AddArtifactTraceabilityForPipelineResource(requestContext, artifactTraceabilityData);
          return true;
        }
      }
      catch (Exception ex)
      {
        string message = "Exception occured while saving traceability data for pipeline. " + string.Format("Details: ProjectId: {0}, definitionId: {1}, ", (object) projectId.ToString(), (object) definitionReference?.Id) + string.Format("runId : {0}, pipelineResource is null? : {1}, service is null ?: {2} ", (object) ownerReference?.Id, (object) (pipelineResource == null), (object) (service == null)) + "Exception : " + ex.ToString();
        DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message);
      }
      return false;
    }

    private static bool SaveArtifactTraceabilityForContainerResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineDefinitionId,
      int pipelineRunId,
      ContainerResource containerResource,
      PipelineEnvironment pipelineEnvironment,
      IArtifactTraceabilityService service)
    {
      try
      {
        service?.AddArtifactTraceabilityForContainerResource(requestContext, projectId, pipelineDefinitionId, pipelineRunId, containerResource, pipelineEnvironment);
        return true;
      }
      catch (Exception ex)
      {
        string message = "Exception occured while saving traceability data for container. Details: ProjectId: " + projectId.ToString() + ", " + string.Format("definitionId: {0}, runId : {1}, containerResource is null? : {2}, ", (object) pipelineDefinitionId, (object) pipelineRunId, (object) (containerResource == null)) + string.Format("pipelineEnvironment is null ? {0}, service is null ?: {1} ", (object) (pipelineEnvironment == null), (object) (service == null)) + "Exception : " + ex.ToString();
        DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message);
      }
      return false;
    }

    private static ArtifactTraceabilityData PrepareArtifactTraceabilityDataForDownloadTask(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      IDictionary<string, string> taskInputs,
      Job job,
      string phaseOrchestrationId)
    {
      if (projectId != Guid.Empty && definitionReference != null && ownerReference != null && taskInputs != null && job != null)
      {
        ArtifactTraceabilityData traceabilityData = new ArtifactTraceabilityData()
        {
          ArtifactCategory = ArtifactCategory.Pipeline,
          ArtifactType = "Pipeline",
          ProjectId = projectId,
          PipelineDefinitionId = definitionReference.Id,
          PipelineRunId = ownerReference.Id,
          DownloadAllArtifacts = false
        };
        if (taskInputs.ContainsKey("alias"))
        {
          if (!"none".Equals(taskInputs["alias"], StringComparison.OrdinalIgnoreCase))
          {
            traceabilityData.ArtifactAlias = taskInputs["alias"];
          }
          else
          {
            string message = "Alias: none found while preparing data for download task. " + string.Format("ProjectId: {0}, definitionId: {1}, runId : {2}, ", (object) projectId.ToString(), (object) definitionReference?.Id, (object) ownerReference?.Id) + "( taskInputs : " + DistributedTaskArtifactTraceabilityService.PrintDictionary(taskInputs) + " ), job Id:  " + job?.Id.ToString() + ", Job Name: " + job?.Name + ", traceabilityData: " + traceabilityData?.ToString();
            DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Warning, message);
            return (ArtifactTraceabilityData) null;
          }
        }
        if (taskInputs.ContainsKey("artifact"))
          traceabilityData.ArtifactName = taskInputs["artifact"];
        else if (taskInputs.ContainsKey("artifactName"))
          traceabilityData.ArtifactName = taskInputs["artifactName"];
        if (string.IsNullOrWhiteSpace(traceabilityData.ArtifactName))
        {
          traceabilityData.ArtifactName = string.Empty;
          traceabilityData.DownloadAllArtifacts = true;
        }
        if (DistributedTaskArtifactTraceabilityService.IsSelfArtifactForDownloadTask(traceabilityData.ArtifactName, traceabilityData.ArtifactAlias))
        {
          traceabilityData.ArtifactAlias = "current";
          traceabilityData.IsSelfArtifact = true;
          traceabilityData.ArtifactVersionId = ownerReference.Id.ToString();
          traceabilityData.ArtifactVersionProperties.Add(ArtifactTraceabilityPropertyKeys.ProjectId, projectId.ToString());
          traceabilityData.ResourcePipelineDefinitionId = definitionReference.Id;
          traceabilityData.ResourceProperties.Add(PipelinePropertyNames.ProjectId, projectId.ToString());
          traceabilityData.ResourceProperties.Add(PipelinePropertyNames.DefinitionId, definitionReference.Id.ToString());
        }
        DistributedTaskArtifactTraceabilityService.UpdateJobTraceabilityData(traceabilityData, job, phaseOrchestrationId);
        return traceabilityData;
      }
      string message1 = "Invalid input value while preparing traceability data for download task. " + string.Format("ProjectId: {0}, definitionId: {1}, runId : {2}, ", (object) projectId.ToString(), (object) definitionReference?.Id, (object) ownerReference?.Id) + "( taskInputs : " + DistributedTaskArtifactTraceabilityService.PrintDictionary(taskInputs) + " ), job Id:  " + job?.Id.ToString() + ", Job Name: " + job?.Name;
      DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Info, message1);
      return (ArtifactTraceabilityData) null;
    }

    private static ArtifactTraceabilityData PrepareArtifactTraceabilityDataForRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      IList<ServiceEndpointReference> endpointReferences,
      RepositoryResource repoResource,
      Job job = null,
      string phaseOrchestrationId = null)
    {
      if (projectId != Guid.Empty && definitionReference != null && ownerReference != null && repoResource != null)
      {
        ArtifactTraceabilityData traceabilityData = new ArtifactTraceabilityData()
        {
          ArtifactCategory = ArtifactCategory.Repository,
          ArtifactType = DistributedTaskArtifactTraceabilityService.ConvertRepositoryTypeFromGitToTfsGit(repoResource.Type),
          ArtifactAlias = repoResource.Alias,
          ProjectId = projectId,
          PipelineDefinitionId = definitionReference.Id,
          PipelineRunId = ownerReference.Id,
          ResourceProperties = repoResource.GetResourceInputs(),
          ArtifactConnectionData = DistributedTaskArtifactTraceabilityService.GetArtifactConnectionData(requestContext, projectId, repoResource.Endpoint, endpointReferences)
        };
        ArtifactTraceabilityResourceInfo traceabilityResourceInfo = new ArtifactTraceabilityResourceInfo()
        {
          Type = DistributedTaskArtifactTraceabilityService.ConvertRepositoryTypeFromGitToTfsGit(repoResource.Type),
          Id = repoResource.Id
        };
        string input;
        Guid result;
        if (repoResource.Properties.TryGetValue<string>(RepositoryPropertyNames.Project, out input) && Guid.TryParse(input, out result))
        {
          traceabilityResourceInfo.ProjectId = result;
          if (string.Equals(traceabilityData.ArtifactType, "TfsGit", StringComparison.OrdinalIgnoreCase))
            traceabilityResourceInfo.Properties.Add(ArtifactTraceabilityPropertyKeys.ProjectId, input);
          traceabilityData.ResourceProperties[PipelinePropertyNames.ProjectId] = input;
          traceabilityData.ArtifactVersionProperties[ArtifactTraceabilityPropertyKeys.ProjectId] = input;
        }
        string str1;
        if (repoResource.Properties.TryGetValue<string>(RepositoryPropertyNames.Ref, out str1))
        {
          traceabilityResourceInfo.Branch = str1;
          traceabilityData.ArtifactVersionId = string.Format("{0}:{1}", (object) repoResource.Version, (object) str1);
        }
        else
          traceabilityData.ArtifactVersionId = repoResource.Version;
        string str2;
        if (repoResource.Properties.TryGetValue<string>(RepositoryPropertyNames.Version, out str2))
          traceabilityResourceInfo.Version = str2;
        string str3;
        if (repoResource.Properties.TryGetValue<string>(RepositoryPropertyNames.Name, out str3))
        {
          traceabilityResourceInfo.Name = str3;
          if (string.Equals(repoResource.Type, "GitHub", StringComparison.OrdinalIgnoreCase) || string.Equals(repoResource.Type, "Bitbucket", StringComparison.OrdinalIgnoreCase))
          {
            string[] strArray1;
            if (str3 == null)
              strArray1 = (string[]) null;
            else
              strArray1 = str3.Split('/');
            string[] strArray2 = strArray1;
            if (strArray2 != null && strArray2.Length == 2)
            {
              traceabilityData.ResourceProperties[ArtifactTraceabilityPropertyKeys.OrgName] = strArray2[0];
              traceabilityData.ResourceProperties[ArtifactTraceabilityPropertyKeys.RepoName] = strArray2[1];
            }
          }
          else if (string.Equals(traceabilityResourceInfo.Type, "TfsGit", StringComparison.OrdinalIgnoreCase))
            traceabilityData.ResourceProperties[ArtifactTraceabilityPropertyKeys.RepoName] = str3;
        }
        if (traceabilityData.ArtifactConnectionData != null && traceabilityData.ArtifactConnectionData.Id != Guid.Empty)
          traceabilityResourceInfo.Properties.Add(ArtifactTraceabilityPropertyKeys.ConnectionId, traceabilityData.ArtifactConnectionData.Id.ToString());
        traceabilityData.Resource = traceabilityResourceInfo;
        if (DistributedTaskArtifactTraceabilityService.IsSelfArtifactForRepositoryResource(repoResource.Alias))
        {
          traceabilityData.IsSelfArtifact = true;
          traceabilityData.ArtifactName = ArtifactTraceabilityConstants.GenericArtifactName;
        }
        else
        {
          traceabilityData.IsSelfArtifact = false;
          traceabilityData.ArtifactName = traceabilityResourceInfo.Name;
        }
        if (string.IsNullOrWhiteSpace(traceabilityResourceInfo.Id))
        {
          if (string.Equals(traceabilityResourceInfo.Type, "TfsGit", StringComparison.OrdinalIgnoreCase))
          {
            string message = "Repo Id is null for TFSGIT repo, found while preparing traceability data for repository resource. " + string.Format("ProjectId: {0}, definitionId: {1}, runId : {2}, ", (object) projectId.ToString(), (object) definitionReference?.Id, (object) ownerReference?.Id) + string.Format("repoResource is null ? : {0}, jobId {1}, jobName: {2}, phaseOrchestrationId: {3}", (object) (repoResource == null), (object) job?.Id, (object) job?.Name, (object) phaseOrchestrationId);
            DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Warning, message);
            return (ArtifactTraceabilityData) null;
          }
          if (!string.IsNullOrWhiteSpace(traceabilityResourceInfo.Name))
            traceabilityResourceInfo.Id = traceabilityResourceInfo.Name;
        }
        DistributedTaskArtifactTraceabilityService.UpdateJobTraceabilityData(traceabilityData, job, phaseOrchestrationId);
        return traceabilityData;
      }
      string message1 = "Invalid input value while preparing traceability data for repository resource. " + string.Format("ProjectId: {0}, definitionId: {1}, runId : {2}, ", (object) projectId.ToString(), (object) definitionReference?.Id, (object) ownerReference?.Id) + string.Format("repoResource is null ? : {0}, jobId {1}, jobName: {2}, phaseOrchestrationId: {3}", (object) (repoResource == null), (object) job?.Id, (object) job?.Name, (object) phaseOrchestrationId);
      DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Info, message1);
      return (ArtifactTraceabilityData) null;
    }

    private static ArtifactTraceabilityData PrepareArtifactTraceabilityDataForPipeline(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskOrchestrationOwner definitionReference,
      TaskOrchestrationOwner ownerReference,
      IList<ServiceEndpointReference> endpointReferences,
      PipelineResource pipelineResource)
    {
      if (projectId != Guid.Empty && definitionReference != null && ownerReference != null && pipelineResource != null)
      {
        ArtifactTraceabilityData traceabilityData = new ArtifactTraceabilityData()
        {
          ArtifactCategory = ArtifactCategory.Pipeline,
          ArtifactType = "Pipeline",
          ArtifactName = ArtifactTraceabilityConstants.GenericArtifactName,
          ArtifactAlias = pipelineResource.Alias,
          ProjectId = projectId,
          PipelineDefinitionId = definitionReference.Id,
          PipelineRunId = ownerReference.Id,
          ResourceProperties = pipelineResource.GetResourceInputs(),
          ArtifactConnectionData = DistributedTaskArtifactTraceabilityService.GetArtifactConnectionData(requestContext, projectId, pipelineResource.Endpoint, endpointReferences)
        };
        string input;
        if (pipelineResource.Properties.TryGetValue<string>(PipelinePropertyNames.ProjectId, out input) && Guid.TryParse(input, out Guid _))
          traceabilityData.ArtifactVersionProperties.Add(ArtifactTraceabilityPropertyKeys.ProjectId, input);
        string str1;
        if (pipelineResource.Properties.TryGetValue<string>(PipelinePropertyNames.Version, out str1))
          traceabilityData.ArtifactVersionName = str1;
        string s;
        int result;
        if (pipelineResource.Properties.TryGetValue<string>(PipelinePropertyNames.DefinitionId, out s) && int.TryParse(s, out result))
          traceabilityData.ResourcePipelineDefinitionId = result;
        string str2;
        if (pipelineResource.Properties.TryGetValue<string>(PipelinePropertyNames.PipelineId, out str2))
          traceabilityData.ArtifactVersionId = str2;
        return traceabilityData;
      }
      string message = "Invalid input value while preparing traceability data for pipeline resource. " + string.Format("ProjectId: {0}, definitionId: {1}, runId : {2}, ", (object) projectId.ToString(), (object) definitionReference?.Id, (object) ownerReference?.Id) + string.Format("pipelineResource is null ? : {0}", (object) (pipelineResource == null));
      DistributedTaskArtifactTraceabilityService.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Info, message);
      return (ArtifactTraceabilityData) null;
    }

    private static string ConvertRepositoryTypeFromGitToTfsGit(string type) => "Git".Equals(type, StringComparison.OrdinalIgnoreCase) ? "TfsGit" : type;

    private static void UpdateJobTraceabilityData(
      ArtifactTraceabilityData traceabilityData,
      Job job,
      string phaseOrchestrationId)
    {
      if (traceabilityData == null || job == null)
        return;
      traceabilityData.JobId = job.Id != Guid.Empty ? job.Id.ToString() : phaseOrchestrationId;
      traceabilityData.JobName = job.DisplayName;
    }

    private static bool IsTraceableDownloadTask(TaskStep task)
    {
      if (task == null || !task.IsDownloadTask())
      {
        if (task.Reference != null)
        {
          Guid id = task.Reference.Id;
          if (!id.Equals(PipelineArtifact.FileContainerArtifactDownloadTaskId))
          {
            id = task.Reference.Id;
            if (!id.Equals(PipelineArtifact.FileShareArtifactDownloadTaskId))
            {
              id = task.Reference.Id;
              if (id.Equals(PipelineArtifact.PipelineArtifactDownloadTaskId))
                goto label_5;
            }
            else
              goto label_5;
          }
          else
            goto label_5;
        }
        return false;
      }
label_5:
      return true;
    }

    private static bool IsSelfArtifactForDownloadTask(string artifactName, string alias) => !string.IsNullOrWhiteSpace(artifactName) && string.IsNullOrWhiteSpace(alias) || "current".Equals(alias, StringComparison.OrdinalIgnoreCase);

    private static bool IsSelfArtifactForRepositoryResource(string alias) => string.Equals(ArtifactTraceabilityConstants.SelfArtifactAlias, alias, StringComparison.OrdinalIgnoreCase);

    private static EndpointReferenceData GetArtifactConnectionData(
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
          string connectionId = DistributedTaskArtifactTraceabilityService.GetConnectionId(requestContext, projectId, resourceEndpointReference.Name.ToString());
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

    private static string GetConnectionId(
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
        throw new ArgumentException(TaskResources.AmbigousServiceEndpointUsed((object) endpointName));
      throw new ArgumentException(TaskResources.EndpointOfTypeNotFound((object) "AzureRM", (object) endpointName));
    }

    private static string PrintDictionary(IDictionary<string, string> dict)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (dict != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) dict)
          stringBuilder.AppendLine("Key = " + keyValuePair.Key + ", Value = " + keyValuePair.Value + " ");
      }
      return stringBuilder.ToString();
    }

    private static void TraceMessageForArtifactTraceability(
      IVssRequestContext requestContext,
      TraceLevel traceLevel,
      string message)
    {
      if (requestContext == null)
        return;
      requestContext.TraceAlways(10016215, traceLevel, "DistributedTask", ArtifactTraceabilityConstants.TraceLayer, message);
    }
  }
}
