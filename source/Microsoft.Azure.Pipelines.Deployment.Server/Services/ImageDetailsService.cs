// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.ImageDetailsService
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Exceptions;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  internal sealed class ImageDetailsService : 
    IImageDetailsService,
    IVssFrameworkService,
    IImageDetailsRepositoryService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddImageDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails)
    {
      try
      {
        this.CheckProjectPermission(requestContext, projectId, TeamProjectPermissions.GenericRead);
        if (!ArtifactMetadataHelper.IsPublishPipelineMetadataEnabled(requestContext, projectId))
          return;
        IArtifactMetadataService service = requestContext.GetService<IArtifactMetadataService>();
        foreach (Grafeas.V1.Note buildAndImageNote in imageDetails.ToBuildAndImageNotes(projectId))
        {
          try
          {
            service.CreateNote(requestContext, buildAndImageNote);
          }
          catch (NoteExistsException ex)
          {
          }
        }
        foreach (Grafeas.V1.Occurrence andImageOccurrence in imageDetails.ToBuildAndImageOccurrences(projectId))
          service.CreateOccurrence(requestContext, andImageOccurrence);
        CustomerIntelligenceHelper.PublishImageDetailsAddedEvent(requestContext, imageDetails);
      }
      catch (Exception ex)
      {
        CustomerIntelligenceHelper.PublishImageDetailsFailedEvent(requestContext, imageDetails, ex);
        throw;
      }
    }

    public Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails GetImageDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      string imageName)
    {
      this.CheckProjectPermission(requestContext, projectId, TeamProjectPermissions.GenericRead);
      IArtifactMetadataService service = requestContext.GetService<IArtifactMetadataService>();
      IList<Grafeas.V1.OccurrenceReference> occurrences = service.GetOccurrences(requestContext, imageName);
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails = new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails();
      Grafeas.V1.OccurrenceReference occurrenceReference = occurrences.FirstOrDefault<Grafeas.V1.OccurrenceReference>((Func<Grafeas.V1.OccurrenceReference, bool>) (x => x.Kind == NoteKind.Image));
      occurrences.FirstOrDefault<Grafeas.V1.OccurrenceReference>((Func<Grafeas.V1.OccurrenceReference, bool>) (x => x.Kind == NoteKind.Build));
      if (occurrenceReference != null)
      {
        Grafeas.V1.ImageOccurrence occurrence = (Grafeas.V1.ImageOccurrence) service.GetOccurrence(requestContext, occurrenceReference.Name);
        ArgumentUtility.CheckForNull<List<Grafeas.V1.ImageLayer>>(occurrence.LayerInfo, "LayerInfo");
        List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer> imageLayerList = new List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer>();
        try
        {
          foreach (Grafeas.V1.ImageLayer imageLayer in occurrence.LayerInfo)
            imageLayerList.Add(new Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer()
            {
              Arguments = imageLayer.Arguments,
              Directive = imageLayer.Directive,
              Size = imageLayer.Size,
              CreatedOn = imageLayer.CreatedOn
            });
        }
        catch (Exception ex)
        {
          requestContext.TraceException(100161012, "Deployment", "Service", ex);
        }
        imageDetails.ImageName = occurrence.ResourceUri;
        imageDetails.ImageUri = occurrence.ResourceUri;
        imageDetails.ImageType = occurrence.ImageType;
        imageDetails.MediaType = occurrence.MediaType;
        imageDetails.Tags = occurrence.Tags;
        imageDetails.JobName = occurrence.JobName;
        imageDetails.ImageSize = occurrence.ImageSize;
        imageDetails.LayerInfo = imageLayerList;
        imageDetails.PipelineVersion = occurrence.PipelineVersion;
        imageDetails.PipelineName = occurrence.PipelineName;
        imageDetails.RunId = occurrence.RunId;
        imageDetails.PipelineId = occurrence.PipelineId;
        imageDetails.CreateTime = occurrence.CreateTime;
        imageDetails.RepositoryId = occurrence.RepositoryId;
        imageDetails.RepositoryName = occurrence.RepositoryName;
        imageDetails.RepositoryTypeName = occurrence.RepositoryTypeName;
        imageDetails.Branch = occurrence.Branch;
        imageDetails.ProjectId = new Guid?(occurrence.ScopeId);
      }
      return imageDetails;
    }

    public IEnumerable<string> GetResourcesWithExistingImages(
      IVssRequestContext requestContext,
      ISet<string> resourceIds)
    {
      return requestContext.GetService<IArtifactMetadataService>().GetResourceIdsByKind(requestContext, resourceIds, NoteKind.Image);
    }

    public IEnumerable<string> GetImageResourceIds(
      IVssRequestContext requestContext,
      Guid projectId,
      int runId)
    {
      IArtifactMetadataService service = requestContext.GetService<IArtifactMetadataService>();
      string pipelineRunIdTag = MetadataDetailsExtensions.GetPipelineRunIdTag(projectId, runId);
      IVssRequestContext requestContext1 = requestContext;
      string tag = pipelineRunIdTag;
      List<string> list = service.GetResourceIdsByTag(requestContext1, tag).ToList<string>();
      IArtifactTraceabilityService traceabilityService = requestContext.GetService<IArtifactTraceabilityService>()?.GetArtifactTraceabilityService(ArtifactTraceabilityConstants.ArtifactTraceabilityDeploymentService);
      IList<ArtifactVersion> artifactVersionList;
      if (traceabilityService == null)
        artifactVersionList = (IList<ArtifactVersion>) null;
      else
        artifactVersionList = traceabilityService.GetArtifactTraceabilityDataForPipeline(requestContext, projectId, runId, artifactCategories: (IList<ArtifactCategory>) new List<ArtifactCategory>()
        {
          ArtifactCategory.Container
        });
      IList<ArtifactVersion> source = artifactVersionList;
      if (source != null && source.Any<ArtifactVersion>())
      {
        foreach (ArtifactVersion artifactVersion in (IEnumerable<ArtifactVersion>) source)
        {
          string empty = string.Empty;
          IDictionary<string, string> versionProperties = artifactVersion.ArtifactVersionProperties;
          if ((versionProperties != null ? (versionProperties.TryGetValue(ArtifactTraceabilityPropertyKeys.ContainerUri, out empty) ? 1 : 0) : 0) != 0)
            list.Add("https://" + empty);
        }
      }
      return (IEnumerable<string>) list;
    }

    public ArtifactTraceabilityResourceInfo GetImageRepositoryDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      string imageName,
      string hostName)
    {
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails = requestContext.GetService<IImageDetailsService>()?.GetImageDetails(requestContext, projectId, imageName);
      if (imageDetails != null)
      {
        List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer> layerInfo = imageDetails.LayerInfo;
        if ((layerInfo != null ? (layerInfo.Any<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer>() ? 1 : 0) : 0) != 0)
        {
          string repositoryName;
          string repoProjectName;
          string version;
          string repoBranchName;
          int definitionId1;
          if (this.IsRequiredContainerRepositoryDetailsAvailable(this.GetVariablesMap(imageDetails.LayerInfo), hostName, imageDetails, out repositoryName, out repoProjectName, out version, out repoBranchName, out definitionId1))
          {
            IProjectService service1 = requestContext.GetService<IProjectService>();
            Guid projectId1;
            try
            {
              projectId1 = service1.GetProjectId(requestContext, repoProjectName);
            }
            catch (ProjectDoesNotExistWithNameException ex)
            {
              string message = string.Format("Unable to fetch project guid for repository project: '{0}'. Input details: project: '{1}', imageName: '{2}', hostName: '{3}'. \nException: '{4}'", (object) repoProjectName, (object) projectId.ToString(), (object) imageName, (object) hostName, (object) ex.Message);
              this.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message);
              return (ArtifactTraceabilityResourceInfo) null;
            }
            IPipelineTfsBuildService service2 = requestContext.GetService<IPipelineTfsBuildService>();
            TeamProjectReference projectReference = new TeamProjectReference()
            {
              Id = projectId1
            };
            IVssRequestContext requestContext1 = requestContext;
            TeamProjectReference project = projectReference;
            int definitionId2 = definitionId1;
            PipelineDefinitionReference pipelineDefinition = service2.GetPipelineDefinition(requestContext1, project, definitionId2);
            string b;
            if (pipelineDefinition.Repository.Properties.TryGetValue<string>(ImageDetailsRepositoryPropertyKeys.RepositoryName, out b) && string.Equals(repositoryName, b, StringComparison.OrdinalIgnoreCase))
            {
              ArtifactTraceabilityResourceInfo repositoryDetails = new ArtifactTraceabilityResourceInfo()
              {
                Name = repositoryName,
                Id = pipelineDefinition.Repository.Id,
                Type = pipelineDefinition.Repository.Type,
                Version = version,
                Branch = repoBranchName,
                ProjectId = projectId1
              };
              repositoryDetails.Properties.Add(ArtifactTraceabilityPropertyKeys.ProjectId, projectId1.ToString());
              string str;
              if (pipelineDefinition.Repository.Properties.TryGetValue<string>(ImageDetailsRepositoryPropertyKeys.RepositoryConnectionId, out str))
                repositoryDetails.Properties.Add(ArtifactTraceabilityPropertyKeys.ConnectionId, str);
              return repositoryDetails;
            }
            string message1 = string.Format("Unable to match container repository name with yaml self repository name for project: '{0}', imageName: '{1}', hostName: '{2}'. Fetched details container repositoryName: '{3}', yaml self repository Name: '{4}', repoProjectName: '{5}', version: '{6}', Branch: '{7}' and definitionId: '{8}'", (object) projectId.ToString(), (object) imageName, (object) hostName, (object) repositoryName, (object) b, (object) repoProjectName, (object) version, (object) repoBranchName, (object) definitionId1);
            this.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message1);
            goto label_16;
          }
          else
          {
            string message = string.Format("Unable to fetch repo details for project: '{0}', imageName: '{1}', hostName: '{2}'. Fetched details: repositoryName: '{3}', repoProjectName: '{4}', version: '{5}', Branch: '{6}' and definitionId: '{7}'.", (object) projectId.ToString(), (object) imageName, (object) hostName, (object) repositoryName, (object) repoProjectName, (object) version, (object) repoBranchName, (object) definitionId1);
            this.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Error, message);
            goto label_16;
          }
        }
      }
      object[] objArray = new object[5]
      {
        (object) projectId.ToString(),
        (object) imageName,
        (object) hostName,
        (object) imageDetails,
        null
      };
      int num;
      if (imageDetails == null)
      {
        num = 0;
      }
      else
      {
        List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer> layerInfo = imageDetails.LayerInfo;
        bool? nullable = layerInfo != null ? new bool?(layerInfo.Any<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer>()) : new bool?();
        bool flag = true;
        num = nullable.GetValueOrDefault() == flag & nullable.HasValue ? 1 : 0;
      }
      objArray[4] = (object) (bool) num;
      string message2 = string.Format("Unable to fetch image details for inputs: project: '{0}', imageName: '{1}', hostName: '{2}'. Fetched details: imageDetails: '{3}', layerInfo: '{4}'.", objArray);
      this.TraceMessageForArtifactTraceability(requestContext, TraceLevel.Warning, message2);
label_16:
      return (ArtifactTraceabilityResourceInfo) null;
    }

    private void CheckProjectPermission(
      IVssRequestContext requestContext,
      Guid scopeId,
      int permission)
    {
      ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
      string projectUri = ProjectInfo.GetProjectUri(scopeId);
      requestContext.GetService<IProjectService>().CheckProjectPermission(requestContext, projectUri, permission);
    }

    private string GetLabel(string hostName, string labelName) => hostName != null && labelName != null ? string.Format("{0}.image.{1}", (object) hostName, (object) labelName) : "";

    private IDictionary<string, string> GetVariablesMap(List<Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer> layerInfo)
    {
      IDictionary<string, string> variablesMap = (IDictionary<string, string>) new Dictionary<string, string>();
      if (layerInfo != null)
      {
        foreach (Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer imageLayer in layerInfo)
        {
          if (!string.IsNullOrWhiteSpace(imageLayer.Arguments) && imageLayer.Arguments.Contains("="))
          {
            string[] strArray = imageLayer.Arguments.Split('=');
            if (!variablesMap.ContainsKey(strArray[0]))
              variablesMap[strArray[0]] = strArray[1].Split(';')[0];
          }
        }
      }
      return variablesMap;
    }

    private bool IsRequiredContainerRepositoryDetailsAvailable(
      IDictionary<string, string> variables,
      string hostName,
      Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails imageDetails,
      out string repositoryName,
      out string repoProjectName,
      out string version,
      out string repoBranchName,
      out int definitionId)
    {
      definitionId = ArtifactTraceabilityConstants.IncorrectId;
      variables.TryGetValue(this.GetLabel(hostName, ImageDetailsRepositoryPropertyKeys.BuildRepositoryName), out repositoryName);
      variables.TryGetValue(this.GetLabel(hostName, ImageDetailsRepositoryPropertyKeys.SystemTeamProject), out repoProjectName);
      variables.TryGetValue(this.GetLabel(hostName, ImageDetailsRepositoryPropertyKeys.BuildSourceVersion), out version);
      variables.TryGetValue(this.GetLabel(hostName, ImageDetailsRepositoryPropertyKeys.BuildSourceBranchName), out repoBranchName);
      return !string.IsNullOrWhiteSpace(repositoryName) && !string.IsNullOrWhiteSpace(repoProjectName) && !string.IsNullOrWhiteSpace(version) && !string.IsNullOrWhiteSpace(repoBranchName) && !string.IsNullOrWhiteSpace(imageDetails?.PipelineId) && int.TryParse(imageDetails?.PipelineId, out definitionId);
    }

    private void TraceMessageForArtifactTraceability(
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
