// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders.FrameworkTfsBuildService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders
{
  internal sealed class FrameworkTfsBuildService : IPipelineTfsBuildService, IVssFrameworkService
  {
    private const string c_folderPathSeparator = "\\";

    public int GetDefinitionId(
      IVssRequestContext requestContext,
      string project,
      string definition)
    {
      BuildHttpClient client = requestContext.GetClient<BuildHttpClient>();
      List<BuildDefinitionReference> definitionReferenceList = new List<BuildDefinitionReference>();
      List<BuildDefinitionReference> list;
      if (definition.Contains("/") || definition.Contains("\\"))
      {
        string[] source = definition.Split(new string[2]
        {
          "\\",
          "/"
        }, StringSplitOptions.RemoveEmptyEntries);
        string str1 = ((IEnumerable<string>) source).Last<string>();
        string str2 = "\\" + string.Join("\\", ((IEnumerable<string>) source).ToArray<string>(), 0, ((IEnumerable<string>) source).Count<string>() - 1);
        BuildHttpClient buildHttpClient = client;
        string project1 = project;
        string name = str1;
        string str3 = str2;
        int? nullable = new int?(2);
        DefinitionQueryOrder? queryOrder = new DefinitionQueryOrder?();
        int? top = nullable;
        DateTime? minMetricsTimeInUtc = new DateTime?();
        string path = str3;
        DateTime? builtAfter = new DateTime?();
        DateTime? notBuiltAfter = new DateTime?();
        bool? includeLatestBuilds = new bool?();
        Guid? taskIdFilter = new Guid?();
        int? processType = new int?();
        CancellationToken cancellationToken = new CancellationToken();
        list = buildHttpClient.GetDefinitionsAsync2(project1, name, queryOrder: queryOrder, top: top, minMetricsTimeInUtc: minMetricsTimeInUtc, path: path, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, includeLatestBuilds: includeLatestBuilds, taskIdFilter: taskIdFilter, processType: processType, cancellationToken: cancellationToken).SyncResult<IPagedList<BuildDefinitionReference>>().Where<BuildDefinitionReference>((Func<BuildDefinitionReference, bool>) (x =>
        {
          DefinitionQuality? definitionQuality1 = x.DefinitionQuality;
          DefinitionQuality definitionQuality2 = DefinitionQuality.Definition;
          return definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue;
        })).ToList<BuildDefinitionReference>();
      }
      else
      {
        BuildHttpClient buildHttpClient = client;
        string project2 = project;
        string name = definition;
        int? nullable = new int?(2);
        DefinitionQueryOrder? queryOrder = new DefinitionQueryOrder?();
        int? top = nullable;
        DateTime? minMetricsTimeInUtc = new DateTime?();
        DateTime? builtAfter = new DateTime?();
        DateTime? notBuiltAfter = new DateTime?();
        bool? includeLatestBuilds = new bool?();
        Guid? taskIdFilter = new Guid?();
        int? processType = new int?();
        CancellationToken cancellationToken = new CancellationToken();
        list = buildHttpClient.GetDefinitionsAsync2(project2, name, queryOrder: queryOrder, top: top, minMetricsTimeInUtc: minMetricsTimeInUtc, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, includeLatestBuilds: includeLatestBuilds, taskIdFilter: taskIdFilter, processType: processType, cancellationToken: cancellationToken).SyncResult<IPagedList<BuildDefinitionReference>>().Where<BuildDefinitionReference>((Func<BuildDefinitionReference, bool>) (x =>
        {
          DefinitionQuality? definitionQuality3 = x.DefinitionQuality;
          DefinitionQuality definitionQuality4 = DefinitionQuality.Definition;
          return definitionQuality3.GetValueOrDefault() == definitionQuality4 & definitionQuality3.HasValue;
        })).ToList<BuildDefinitionReference>();
      }
      if (list.Count == 1)
        return list[0].Id;
      if (list.Count == 0)
        throw new ResourceNotFoundException(TaskResources.BuildDefinitionNotFound((object) definition, (object) project));
      throw new AmbiguousResourceSpecificationException(TaskResources.AmbiguousBuildDefinitionsFound((object) definition, (object) project));
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
      BuildHttpClient client = requestContext.GetClient<BuildHttpClient>();
      string project1 = project;
      int[] definitions = new int[1]{ definitionId };
      string str = branch;
      IEnumerable<string> strings = (IEnumerable<string>) tags;
      BuildStatus? nullable1 = new BuildStatus?(BuildStatus.Completed);
      BuildResult? nullable2 = new BuildResult?(BuildResult.Succeeded);
      BuildQueryOrder? nullable3 = new BuildQueryOrder?(BuildQueryOrder.FinishTimeDescending);
      int? nullable4 = new int?(1);
      DateTime? minFinishTime = new DateTime?();
      DateTime? maxFinishTime = new DateTime?();
      BuildReason? reasonFilter = new BuildReason?();
      BuildStatus? statusFilter = nullable1;
      BuildResult? resultFilter = nullable2;
      IEnumerable<string> tagFilters = strings;
      int? top = new int?();
      int? maxBuildsPerDefinition = nullable4;
      QueryDeletedOption? deletedFilter = new QueryDeletedOption?();
      BuildQueryOrder? queryOrder = nullable3;
      string branchName = str;
      CancellationToken cancellationToken = new CancellationToken();
      Microsoft.TeamFoundation.Build.WebApi.Build build = client.GetBuildsAsync2(project1, (IEnumerable<int>) definitions, minFinishTime: minFinishTime, maxFinishTime: maxFinishTime, reasonFilter: reasonFilter, statusFilter: statusFilter, resultFilter: resultFilter, tagFilters: tagFilters, top: top, maxBuildsPerDefinition: maxBuildsPerDefinition, deletedFilter: deletedFilter, queryOrder: queryOrder, branchName: branchName, cancellationToken: cancellationToken).SyncResult<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>>().FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Build>();
      if (build != null)
        latestPipelineInfo = new PipelineInfo()
        {
          Id = build.Id,
          DefinitionId = build.Definition.Id,
          PipelineNumber = build.BuildNumber
        };
      return latestPipelineInfo;
    }

    public PipelineDefinitionReference GetPipelineDefinition(
      IVssRequestContext requestContext,
      TeamProjectReference project,
      int definitionId)
    {
      BuildDefinition buildDefinition = requestContext.GetClient<BuildHttpClient>().GetDefinitionAsync(project.Id, definitionId, new int?(), new DateTime?(), (IEnumerable<string>) null, new bool?(), (object) null, new CancellationToken()).SyncResult<BuildDefinition>();
      if (buildDefinition == null)
        throw new ResourceNotFoundException(TaskResources.BuildDefinitionNotFound((object) definitionId, (object) project));
      PipelineDefinitionReference pipelineDefinition = new PipelineDefinitionReference()
      {
        Id = buildDefinition.Id,
        Project = buildDefinition.Project,
        Repository = new RepositoryResource()
        {
          Id = buildDefinition.Repository.Id,
          Type = buildDefinition.Repository.Type
        }
      };
      string str;
      if (buildDefinition.Repository.Properties.TryGetValue(ImageDetailsRepositoryPropertyKeys.RepositoryConnectionId, out str))
        pipelineDefinition.Repository.Properties.Set<string>(ImageDetailsRepositoryPropertyKeys.RepositoryConnectionId, str);
      if (!string.IsNullOrWhiteSpace(buildDefinition.Repository.Name))
        pipelineDefinition.Repository.Properties.Set<string>(ImageDetailsRepositoryPropertyKeys.RepositoryName, buildDefinition.Repository.Name);
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
      Microsoft.TeamFoundation.Build.WebApi.Build build = requestContext.GetClient<BuildHttpClient>().GetBuildsAsync2(project, (IEnumerable<int>) new int[1]
      {
        definitionId
      }, buildNumber: pipelineNumber).SyncResult<IPagedList<Microsoft.TeamFoundation.Build.WebApi.Build>>().FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Build>();
      if (build != null)
        pipelineInfo = new PipelineInfo()
        {
          Id = build.Id,
          DefinitionId = build.Definition.Id,
          PipelineNumber = build.BuildNumber
        };
      return pipelineInfo;
    }

    public PipelineInfo GetPipelineInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId)
    {
      Microsoft.TeamFoundation.Build.WebApi.Build build = requestContext.GetClient<BuildHttpClient>().GetBuildAsync(projectId, pipelineId).SyncResult<Microsoft.TeamFoundation.Build.WebApi.Build>();
      return new PipelineInfo()
      {
        Id = build.Id,
        PipelineNumber = build.BuildNumber,
        DefinitionId = build.Definition.Id,
        DefinitionName = build.Definition.Name,
        RepositoryType = build.Repository.Type,
        RequestedFor = build.RequestedFor,
        SourceBranch = build.SourceBranch,
        SourceCommit = build.SourceVersion,
        Reason = build.Reason,
        Uri = build.Uri
      };
    }

    public PipelineInfo QueuePipeline(
      IVssRequestContext requestContext,
      ProjectInfo project,
      Microsoft.TeamFoundation.Build.WebApi.Build buildToQueue)
    {
      PipelineInfo pipelineInfo = (PipelineInfo) null;
      Microsoft.TeamFoundation.Build.WebApi.Build build = requestContext.GetClient<BuildHttpClient>().QueueBuildAsync(buildToQueue, project.Id, new bool?(), (string) null, new int?(), new int?(), (object) null, new CancellationToken()).SyncResult<Microsoft.TeamFoundation.Build.WebApi.Build>();
      if (build != null)
        pipelineInfo = new PipelineInfo()
        {
          Id = build.Id,
          DefinitionId = build.Definition.Id,
          PipelineNumber = build.BuildNumber
        };
      return pipelineInfo;
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
