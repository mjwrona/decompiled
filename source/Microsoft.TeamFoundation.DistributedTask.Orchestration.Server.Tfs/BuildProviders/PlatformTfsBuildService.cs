// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders.PlatformTfsBuildService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Tfs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3EB20FA-6669-4C21-BA19-EC9C2EBF5243
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Tfs.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders
{
  internal class PlatformTfsBuildService : IPipelineTfsBuildService, IVssFrameworkService
  {
    private const string c_folderPathSeparator = "\\";
    private const string c_forkRepoTrigger = "isFork";

    public int GetDefinitionId(
      IVssRequestContext requestContext,
      string project,
      string definition)
    {
      IBuildDefinitionService service1 = requestContext.GetService<IBuildDefinitionService>();
      List<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> buildDefinitionList = new List<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>();
      List<Microsoft.TeamFoundation.Build2.Server.BuildDefinition> list;
      if (definition.Contains("/") || definition.Contains("\\"))
      {
        string[] source = definition.Split(new string[2]
        {
          "\\",
          "/"
        }, StringSplitOptions.RemoveEmptyEntries);
        string str1 = ((IEnumerable<string>) source).Last<string>();
        string str2 = "\\" + string.Join("\\", source, 0, source.Length - 1);
        IBuildDefinitionService service2 = service1;
        IVssRequestContext requestContext1 = requestContext;
        string project1 = project;
        string name = str1;
        string str3 = str2;
        DateTime? minLastModifiedTime = new DateTime?();
        DateTime? maxLastModifiedTime = new DateTime?();
        DateTime? minMetricsTime = new DateTime?();
        string path = str3;
        DateTime? builtAfter = new DateTime?();
        DateTime? notBuiltAfter = new DateTime?();
        Guid? taskIdFilter = new Guid?();
        list = service2.GetDefinitions(requestContext1, project1, name, count: 2, minLastModifiedTime: minLastModifiedTime, maxLastModifiedTime: maxLastModifiedTime, minMetricsTime: minMetricsTime, path: path, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, options: DefinitionQueryOptions.None, taskIdFilter: taskIdFilter).Where<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, bool>) (x =>
        {
          Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? definitionQuality1 = x.DefinitionQuality;
          Microsoft.TeamFoundation.Build2.Server.DefinitionQuality definitionQuality2 = Microsoft.TeamFoundation.Build2.Server.DefinitionQuality.Definition;
          return definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue;
        })).ToList<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>();
      }
      else
        list = service1.GetDefinitions(requestContext, project, definition, count: 2, options: DefinitionQueryOptions.None).Where<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, bool>) (x =>
        {
          Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? definitionQuality3 = x.DefinitionQuality;
          Microsoft.TeamFoundation.Build2.Server.DefinitionQuality definitionQuality4 = Microsoft.TeamFoundation.Build2.Server.DefinitionQuality.Definition;
          return definitionQuality3.GetValueOrDefault() == definitionQuality4 & definitionQuality3.HasValue;
        })).ToList<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>();
      if (list.Count == 1)
        return list[0].Id;
      if (list.Count == 0)
        throw new ResourceNotFoundException(TfsResources.BuildDefinitionNotFound((object) definition, (object) project));
      throw new AmbiguousResourceSpecificationException(TfsResources.AmbiguousBuildDefinitionsFound((object) definition, (object) project));
    }

    public PipelineInfo GetLatestPipelineInfo(
      IVssRequestContext requestContext,
      string project,
      string definition,
      string branch,
      IList<string> tags)
    {
      PipelineInfo latestPipelineInfo = (PipelineInfo) null;
      int definitionId = this.GetDefinitionId(requestContext, project, definition);
      IBuildService service = requestContext.GetService<IBuildService>();
      IVssRequestContext requestContext1 = requestContext;
      string project1 = project;
      int[] definitionIds = new int[1]{ definitionId };
      string str = branch;
      IEnumerable<string> strings = (IEnumerable<string>) tags;
      Microsoft.TeamFoundation.Build2.Server.BuildStatus? nullable1 = new Microsoft.TeamFoundation.Build2.Server.BuildStatus?(Microsoft.TeamFoundation.Build2.Server.BuildStatus.Completed);
      Microsoft.TeamFoundation.Build2.Server.BuildResult? nullable2 = new Microsoft.TeamFoundation.Build2.Server.BuildResult?(Microsoft.TeamFoundation.Build2.Server.BuildResult.Succeeded | Microsoft.TeamFoundation.Build2.Server.BuildResult.PartiallySucceeded);
      DateTime? minFinishTime = new DateTime?();
      DateTime? maxFinishTime = new DateTime?();
      Microsoft.TeamFoundation.Build2.Server.BuildReason? reasonFilter = new Microsoft.TeamFoundation.Build2.Server.BuildReason?();
      Microsoft.TeamFoundation.Build2.Server.BuildStatus? statusFilter = nullable1;
      Microsoft.TeamFoundation.Build2.Server.BuildResult? resultFilter = nullable2;
      IEnumerable<string> tagFilters = strings;
      string branchName = str;
      int? maxBuildsPerDefinition = new int?();
      BuildData buildData = service.GetBuildsLegacy(requestContext1, project1, 1, (IEnumerable<int>) definitionIds, minFinishTime: minFinishTime, maxFinishTime: maxFinishTime, reasonFilter: reasonFilter, statusFilter: statusFilter, resultFilter: resultFilter, tagFilters: tagFilters, branchName: branchName, maxBuildsPerDefinition: maxBuildsPerDefinition).FirstOrDefault<BuildData>();
      if (buildData != null)
        latestPipelineInfo = new PipelineInfo()
        {
          Id = buildData.Id,
          DefinitionId = buildData.Definition.Id,
          PipelineNumber = buildData.BuildNumber
        };
      return latestPipelineInfo;
    }

    public PipelineDefinitionReference GetPipelineDefinition(
      IVssRequestContext requestContext,
      TeamProjectReference project,
      int definitionId)
    {
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = requestContext.GetService<IBuildDefinitionService>().GetDefinition(requestContext, project.Id, definitionId);
      if (definition == null)
        throw new ResourceNotFoundException(TaskResources.BuildDefinitionNotFound((object) definitionId, (object) project));
      PipelineDefinitionReference pipelineDefinition = new PipelineDefinitionReference()
      {
        Id = definition.Id,
        Project = new TeamProjectReference()
        {
          Id = definition.ProjectId
        },
        Repository = new RepositoryResource()
        {
          Id = definition.Repository.Id,
          Type = definition.Repository.Type
        }
      };
      string str;
      if (definition.Repository.Properties.TryGetValue(ImageDetailsRepositoryPropertyKeys.RepositoryConnectionId, out str))
        pipelineDefinition.Repository.Properties.Set<string>(ImageDetailsRepositoryPropertyKeys.RepositoryConnectionId, str);
      if (!string.IsNullOrWhiteSpace(definition.Repository.Name))
        pipelineDefinition.Repository.Properties.Set<string>(ImageDetailsRepositoryPropertyKeys.RepositoryName, definition.Repository.Name);
      return pipelineDefinition;
    }

    public PipelineInfo GetPipelineInfo(
      IVssRequestContext requestContext,
      string project,
      string definition,
      string pipelineNumber)
    {
      PipelineInfo pipelineInfo = (PipelineInfo) null;
      int definitionId = this.GetDefinitionId(requestContext, project, definition);
      BuildData buildData = requestContext.GetService<IBuildService>().GetBuildsLegacy(requestContext, project, 1, (IEnumerable<int>) new int[1]
      {
        definitionId
      }, buildNumber: pipelineNumber).FirstOrDefault<BuildData>();
      if (buildData != null)
        pipelineInfo = new PipelineInfo()
        {
          Id = buildData.Id,
          DefinitionId = buildData.Definition.Id,
          PipelineNumber = buildData.BuildNumber
        };
      return pipelineInfo;
    }

    public PipelineInfo QueuePipeline(
      IVssRequestContext requestContext,
      ProjectInfo project,
      Microsoft.TeamFoundation.Build.WebApi.Build buildToQueue)
    {
      BuildData build = new BuildData()
      {
        Definition = new MinimalBuildDefinition()
        {
          Id = buildToQueue.Definition.Id
        },
        ProjectId = project.Id,
        Reason = Microsoft.TeamFoundation.Build2.Server.BuildReason.ResourceTrigger,
        SourceBranch = buildToQueue.SourceBranch,
        SourceVersion = buildToQueue.SourceVersion,
        TriggerInfo = buildToQueue.TriggerInfo
      };
      if (buildToQueue.RequestedFor != null)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().GetIdentity(requestContext, buildToQueue.RequestedFor);
        build.RequestedFor = identity.Id;
      }
      PipelineInfo pipelineInfo = (PipelineInfo) null;
      IBuildService service = requestContext.GetService<IBuildService>();
      BuildData buildData;
      if (requestContext.IsFeatureEnabled("DistributedTask.DisableResourceTriggeredBuildsForForkedGithubRepo"))
      {
        build.Parameters = buildToQueue.Parameters;
        IDictionary<string, string> dictionary;
        if (JsonUtilities.TryDeserialize<IDictionary<string, string>>(build.Parameters, out dictionary))
        {
          HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) dictionary.Keys);
          IEnumerable<IBuildRequestValidator> validators = BuildRequestValidatorProvider.GetValidators(new BuildRequestValidationOptions()
          {
            InternalRuntimeVariables = stringSet
          });
          buildData = service.QueueBuild(requestContext, build, validators, BuildRequestValidationFlags.QueueFailedBuild | BuildRequestValidationFlags.SkipSourceVersionValidation, callingMethod: nameof (QueuePipeline), callingFile: "D:\\a\\_work\\1\\s\\Tfs\\Service\\DistributedTask\\Server.Tfs\\BuildProviders\\PlatformTfsBuildService.cs");
        }
        else
          buildData = service.QueueBuild(requestContext, build);
      }
      else
        buildData = service.QueueBuild(requestContext, build);
      if (buildData != null)
        pipelineInfo = new PipelineInfo()
        {
          Id = buildData.Id,
          DefinitionId = buildData.Definition.Id,
          PipelineNumber = buildData.BuildNumber
        };
      return pipelineInfo;
    }

    public PipelineInfo GetPipelineInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId)
    {
      BuildData buildById = requestContext.GetService<IBuildService>().GetBuildById(requestContext, projectId, pipelineId);
      PipelineInfo pipelineInfo = (PipelineInfo) null;
      if (buildById != null)
      {
        IdentityRef identity = this.GetIdentity(requestContext, buildById.RequestedFor);
        pipelineInfo = new PipelineInfo()
        {
          Id = buildById.Id,
          PipelineNumber = buildById.BuildNumber,
          DefinitionId = buildById.Definition.Id,
          DefinitionName = buildById.Definition.Name,
          RepositoryType = buildById.Repository.Type,
          RequestedFor = identity,
          SourceBranch = buildById.SourceBranch,
          SourceCommit = buildById.SourceVersion,
          Reason = Enum.IsDefined(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildReason), (object) (int) buildById.Reason) ? (Microsoft.TeamFoundation.Build.WebApi.BuildReason) buildById.Reason : Microsoft.TeamFoundation.Build.WebApi.BuildReason.None,
          Uri = buildById.Uri
        };
      }
      return pipelineInfo;
    }

    private IdentityRef GetIdentity(IVssRequestContext requestContext, Guid identityId)
    {
      try
      {
        return new IdentityMap(requestContext.GetService<IdentityService>()).GetIdentityRef(requestContext, identityId);
      }
      catch (Exception ex)
      {
        return (IdentityRef) null;
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
