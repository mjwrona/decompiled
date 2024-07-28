// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build.WebApi.Internals;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildDefinitionExtensions
  {
    public static BuildDefinitionReference ToBuildDefinitionReference(
      this Microsoft.TeamFoundation.Build2.Server.BuildDefinition source,
      IVssRequestContext requestContext,
      Version apiVersion,
      IdentityMap identityMap = null)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BuildDefinitionExtensions.ToBuildDefinitionReference"))
      {
        if (source == null)
          return (BuildDefinitionReference) null;
        if (identityMap == null)
          identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
        ISecuredObject securedObject = source.ToSecuredObject();
        BuildDefinitionReference definitionReference1 = new BuildDefinitionReference();
        definitionReference1.AuthoredBy = identityMap.GetIdentityRef(requestContext, source.AuthoredBy);
        definitionReference1.CreatedDate = source.CreatedDate;
        Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? definitionQuality = source.DefinitionQuality;
        definitionReference1.DefinitionQuality = definitionQuality.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality?((Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality) definitionQuality.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality?();
        definitionReference1.Id = source.Id;
        definitionReference1.LatestBuild = source.LatestBuild.ToWebApiBuild(requestContext, apiVersion);
        definitionReference1.LatestCompletedBuild = source.LatestCompletedBuild.ToWebApiBuild(requestContext, apiVersion);
        definitionReference1.Name = source.Name;
        definitionReference1.ParentDefinition = source.ParentDefinition.ToDefinitionReference(requestContext);
        definitionReference1.Path = source.Path;
        definitionReference1.Project = requestContext.GetTeamProjectReference(source.ProjectId, source.ProjectName);
        definitionReference1.Queue = source.Queue.ToWebApiAgentPoolQueue(requestContext, securedObject);
        definitionReference1.QueueStatus = (Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus) source.QueueStatus;
        definitionReference1.Revision = source.Revision;
        definitionReference1.Type = (Microsoft.TeamFoundation.Build.WebApi.DefinitionType) source.Type;
        definitionReference1.Uri = source.Uri;
        definitionReference1.Url = source.GetRestUrl(requestContext);
        BuildDefinitionReference result = definitionReference1;
        foreach (Microsoft.TeamFoundation.Build2.Server.BuildDefinition draft in source.Drafts)
        {
          DefinitionReference definitionReference2 = draft.ToDefinitionReference(requestContext);
          definitionReference2.Project = result.Project;
          result.Drafts.Add(definitionReference2);
        }
        if (result.ParentDefinition != null)
          result.ParentDefinition.SetNestingSecurityToken(securedObject.GetToken());
        if (source.Metrics != null)
          result.Metrics = source.Metrics.MergePullRequestMetrics(requestContext).Select<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>((Func<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>) (x => x.ToWebApiBuildMetric((ISecuredObject) result))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildMetric>();
        result.Links.TryAddLink("self", securedObject, result.Url);
        result.Links.TryAddLink("web", securedObject, (Func<string>) (() => result.GetWebUrl(requestContext)));
        result.Links.TryAddLink("editor", securedObject, (Func<string>) (() => result.GetDesignerUrl(requestContext)));
        IBuildRouteService routeService = requestContext.GetService<IBuildRouteService>();
        result.Links.TryAddLink("badge", securedObject, (Func<string>) (() => routeService.GetStatusBadgeUrl(requestContext, source.ProjectId, source.Id)));
        result.UpdateReferences(requestContext, identityMap);
        return result;
      }
    }

    public static Microsoft.TeamFoundation.Build.WebApi.BuildDefinition ToWebApiBuildDefinition(
      this Microsoft.TeamFoundation.Build2.Server.BuildDefinition source,
      IVssRequestContext requestContext,
      Version apiVersion,
      IdentityMap identityMap = null,
      AgentPoolQueueCache queueCache = null,
      BuildRepositoryCache repositoryCache = null,
      bool updateReferencesAndLinks = true,
      bool refreshRepoName = true)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BuildDefinitionExtensions.ToWebApiBuildDefinition"))
      {
        if (source == null)
          return (Microsoft.TeamFoundation.Build.WebApi.BuildDefinition) null;
        if (identityMap == null)
          identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
        if (queueCache == null)
          queueCache = new AgentPoolQueueCache(requestContext);
        if (repositoryCache == null)
          repositoryCache = new BuildRepositoryCache(requestContext);
        TeamProjectReference projectReference;
        if (updateReferencesAndLinks)
          projectReference = requestContext.GetTeamProjectReference(source.ProjectId);
        else
          projectReference = new TeamProjectReference()
          {
            Id = source.ProjectId
          };
        ISecuredObject securedObject = source.ToSecuredObject();
        Microsoft.TeamFoundation.Build.WebApi.BuildDefinition buildDefinition = new Microsoft.TeamFoundation.Build.WebApi.BuildDefinition();
        buildDefinition.AuthoredBy = identityMap.GetIdentityRef(requestContext, source.AuthoredBy);
        buildDefinition.BadgeEnabled = source.BadgeEnabled;
        buildDefinition.BuildNumberFormat = source.BuildNumberFormat;
        buildDefinition.Comment = source.Comment;
        buildDefinition.CreatedDate = source.CreatedDate;
        Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? definitionQuality = source.DefinitionQuality;
        buildDefinition.DefinitionQuality = definitionQuality.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality?((Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality) definitionQuality.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality?();
        buildDefinition.Description = source.Description;
        buildDefinition.DropLocation = source.DropLocation;
        buildDefinition.Id = source.Id;
        buildDefinition.JobAuthorizationScope = (Microsoft.TeamFoundation.Build.WebApi.BuildAuthorizationScope) source.JobAuthorizationScope;
        buildDefinition.JobCancelTimeoutInMinutes = source.JobCancelTimeoutInMinutes;
        buildDefinition.JobTimeoutInMinutes = source.JobTimeoutInMinutes;
        buildDefinition.LatestBuild = source.LatestBuild.ToWebApiBuild(requestContext, apiVersion, identityMap, queueCache, repositoryCache, updateReferencesAndLinks, refreshRepoName);
        buildDefinition.LatestCompletedBuild = source.LatestCompletedBuild.ToWebApiBuild(requestContext, apiVersion, identityMap, queueCache, repositoryCache, updateReferencesAndLinks, refreshRepoName);
        buildDefinition.Name = source.Name;
        buildDefinition.ParentDefinition = source.ParentDefinition.ToDefinitionReference(requestContext);
        buildDefinition.Path = source.Path;
        buildDefinition.ProcessParameters = source.ProcessParameters != null ? source.ProcessParameters.Clone(securedObject) : (ProcessParameters) null;
        buildDefinition.Project = projectReference;
        buildDefinition.QueueStatus = (Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus) source.QueueStatus;
        buildDefinition.Revision = source.Revision;
        buildDefinition.Type = (Microsoft.TeamFoundation.Build.WebApi.DefinitionType) source.Type;
        buildDefinition.Uri = source.Uri;
        Microsoft.TeamFoundation.Build.WebApi.BuildDefinition result = buildDefinition;
        if (source.Queue != null)
        {
          if (updateReferencesAndLinks)
            result.Queue = source.Queue.ToWebApiAgentPoolQueue(requestContext, securedObject);
          else
            result.Queue = new Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue(securedObject)
            {
              Id = source.Queue.Id,
              Name = source.Queue.Name
            };
        }
        if (source.Drafts.Count > 0)
          result.Drafts.AddRange(source.Drafts.Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, DefinitionReference>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, DefinitionReference>) (d => d.ToDefinitionReference(requestContext))));
        if (result.ParentDefinition != null)
          result.ParentDefinition.SetNestingSecurityToken(securedObject.GetToken());
        if (source.Metrics != null)
          result.Metrics = source.Metrics.MergePullRequestMetrics(requestContext).Select<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>((Func<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>) (x => x.ToWebApiBuildMetric((ISecuredObject) result))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildMetric>();
        if (source.Process != null)
        {
          Microsoft.TeamFoundation.Build2.Server.DesignerProcess process1 = source.Process as Microsoft.TeamFoundation.Build2.Server.DesignerProcess;
          Microsoft.TeamFoundation.Build2.Server.YamlProcess process2 = source.Process as Microsoft.TeamFoundation.Build2.Server.YamlProcess;
          Microsoft.TeamFoundation.Build2.Server.DockerProcess process3 = source.Process as Microsoft.TeamFoundation.Build2.Server.DockerProcess;
          Microsoft.TeamFoundation.Build2.Server.JustInTimeProcess process4 = source.Process as Microsoft.TeamFoundation.Build2.Server.JustInTimeProcess;
          result.Process = process1 == null ? (process2 == null ? (process3 == null ? (process4 == null ? new Microsoft.TeamFoundation.Build.WebApi.BuildProcess(source.Process.Type, (ISecuredObject) result) : (Microsoft.TeamFoundation.Build.WebApi.BuildProcess) process4.ToWebApiJustInTimeProcess(requestContext, (ISecuredObject) result)) : (Microsoft.TeamFoundation.Build.WebApi.BuildProcess) process3.ToWebApiDockerProcess(requestContext, (ISecuredObject) result)) : (Microsoft.TeamFoundation.Build.WebApi.BuildProcess) process2.ToWebApiYamlProcess((ISecuredObject) result)) : (Microsoft.TeamFoundation.Build.WebApi.BuildProcess) process1.ToWebApiDesignerProcess(requestContext, (ISecuredObject) result);
        }
        if (source.Options != null)
          result.Options.AddRange(source.Options.Select<Microsoft.TeamFoundation.Build2.Server.BuildOption, Microsoft.TeamFoundation.Build.WebApi.BuildOption>((Func<Microsoft.TeamFoundation.Build2.Server.BuildOption, Microsoft.TeamFoundation.Build.WebApi.BuildOption>) (x => x.ToWebApiBuildOption((ISecuredObject) result))));
        if (source.Repository != null)
        {
          Microsoft.TeamFoundation.Build.WebApi.BuildRepository apiBuildRepository = BuildRepositoryExtensions.ToWebApiBuildRepository(source.Repository, requestContext, source.ProjectId, (ISecuredObject) result, repositoryCache, refreshRepoName, new int?(source.Id));
          if (apiBuildRepository != null && !updateReferencesAndLinks)
            result.Repository = new Microsoft.TeamFoundation.Build.WebApi.BuildRepository(securedObject)
            {
              Id = apiBuildRepository.Id,
              Name = apiBuildRepository.Name,
              Type = apiBuildRepository.Type,
              Properties = apiBuildRepository.Properties
            };
          else
            result.Repository = apiBuildRepository;
        }
        if (source.Triggers != null)
        {
          if (requestContext.IsFeatureEnabled("Build2.AllowCentralizedPipelineControls") && source.ProjectId != Guid.Empty)
          {
            PipelineTriggerSettings effectiveTriggerSettings = new ProjectPipelineGeneralSettingsHelper(requestContext, source.ProjectId, true).GetEffectivePipelineTriggerSettings();
            result.Triggers.AddRange(source.Triggers.Select<Microsoft.TeamFoundation.Build2.Server.BuildTrigger, Microsoft.TeamFoundation.Build.WebApi.BuildTrigger>((Func<Microsoft.TeamFoundation.Build2.Server.BuildTrigger, Microsoft.TeamFoundation.Build.WebApi.BuildTrigger>) (x => x.ToWebApiBuildTrigger((ISecuredObject) result, effectiveTriggerSettings))));
          }
          else
            result.Triggers.AddRange(source.Triggers.Select<Microsoft.TeamFoundation.Build2.Server.BuildTrigger, Microsoft.TeamFoundation.Build.WebApi.BuildTrigger>((Func<Microsoft.TeamFoundation.Build2.Server.BuildTrigger, Microsoft.TeamFoundation.Build.WebApi.BuildTrigger>) (x => x.ToWebApiBuildTrigger((ISecuredObject) result))));
        }
        if (source.BuildCompletionTriggers != null)
          result.Triggers.AddRange((IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildTrigger>) source.BuildCompletionTriggers.Select<Microsoft.TeamFoundation.Build2.Server.BuildCompletionTrigger, Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger>((Func<Microsoft.TeamFoundation.Build2.Server.BuildCompletionTrigger, Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger>) (x => x.ToWebApiBCTrigger(requestContext, (ISecuredObject) result))));
        result.Variables.AddRange<KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>, IDictionary<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>>((IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>>) source.Variables.ToWebApiVariables((ISecuredObject) result));
        if (source.VariableGroups != null)
          result.VariableGroups = source.VariableGroups.Select<Microsoft.TeamFoundation.Build2.Server.VariableGroup, Microsoft.TeamFoundation.Build.WebApi.VariableGroup>((Func<Microsoft.TeamFoundation.Build2.Server.VariableGroup, Microsoft.TeamFoundation.Build.WebApi.VariableGroup>) (x => x.ToWebApiVariableGroup((ISecuredObject) result))).ToList<Microsoft.TeamFoundation.Build.WebApi.VariableGroup>();
        if (source.Demands != null)
          result.Demands = source.Demands.Select<Microsoft.TeamFoundation.Build2.Server.Demand, Microsoft.TeamFoundation.Build.WebApi.Demand>((Func<Microsoft.TeamFoundation.Build2.Server.Demand, Microsoft.TeamFoundation.Build.WebApi.Demand>) (x => x.ToWebApiDemand((ISecuredObject) result))).ToList<Microsoft.TeamFoundation.Build.WebApi.Demand>();
        result.Properties = source.Properties;
        result.Tags.AddRange((IEnumerable<string>) source.Tags);
        if (updateReferencesAndLinks)
        {
          using (PerformanceTimer.StartMeasure(requestContext, "GetRestUrl"))
            result.Url = source.GetRestUrl(requestContext);
          BuildDefinitionExtensions.UpdateReferences(result, requestContext, identityMap);
          result.AddLinks(requestContext);
        }
        return result;
      }
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildDefinition ToBuildServerBuildDefinition(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinition source)
    {
      if (source == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildDefinition) null;
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition1 = new Microsoft.TeamFoundation.Build2.Server.BuildDefinition();
      buildDefinition1.Id = source.Id;
      buildDefinition1.Name = source.Name;
      buildDefinition1.Uri = source.Uri;
      buildDefinition1.Url = source.Url;
      buildDefinition1.Path = source.Path;
      buildDefinition1.Type = (Microsoft.TeamFoundation.Build2.Server.DefinitionType) source.Type;
      buildDefinition1.QueueStatus = (Microsoft.TeamFoundation.Build2.Server.DefinitionQueueStatus) source.QueueStatus;
      buildDefinition1.Revision = source.Revision;
      buildDefinition1.CreatedDate = source.CreatedDate;
      Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality? definitionQuality = source.DefinitionQuality;
      buildDefinition1.DefinitionQuality = definitionQuality.HasValue ? new Microsoft.TeamFoundation.Build2.Server.DefinitionQuality?((Microsoft.TeamFoundation.Build2.Server.DefinitionQuality) definitionQuality.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.DefinitionQuality?();
      buildDefinition1.AuthoredBy = source.AuthoredBy.ToIdentityGuid() ?? Guid.Empty;
      buildDefinition1.BuildNumberFormat = source.BuildNumberFormat;
      buildDefinition1.Comment = source.Comment;
      buildDefinition1.Description = source.Description;
      buildDefinition1.DropLocation = source.DropLocation;
      buildDefinition1.JobAuthorizationScope = (Microsoft.TeamFoundation.Build2.Server.BuildAuthorizationScope) source.JobAuthorizationScope;
      buildDefinition1.JobTimeoutInMinutes = source.JobTimeoutInMinutes;
      buildDefinition1.JobCancelTimeoutInMinutes = source.JobCancelTimeoutInMinutes;
      buildDefinition1.BadgeEnabled = source.BadgeEnabled;
      buildDefinition1.ProcessParameters = source.ProcessParameters;
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition serverBuildDefinition = buildDefinition1;
      if (source.Project != null)
      {
        serverBuildDefinition.ProjectId = source.Project.Id;
        serverBuildDefinition.ProjectName = source.Project.Name;
      }
      if (source.ParentDefinition != null)
      {
        Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition2 = serverBuildDefinition;
        Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition3 = new Microsoft.TeamFoundation.Build2.Server.BuildDefinition();
        buildDefinition3.Id = source.ParentDefinition.Id;
        buildDefinition3.Revision = source.ParentDefinition.Revision;
        buildDefinition3.Type = (Microsoft.TeamFoundation.Build2.Server.DefinitionType) source.ParentDefinition.Type;
        buildDefinition3.Name = source.ParentDefinition.Name;
        buildDefinition3.QueueStatus = (Microsoft.TeamFoundation.Build2.Server.DefinitionQueueStatus) source.ParentDefinition.QueueStatus;
        buildDefinition3.Url = source.ParentDefinition.Url;
        buildDefinition3.Uri = source.ParentDefinition.Uri;
        buildDefinition3.ProjectId = serverBuildDefinition.ProjectId;
        buildDefinition3.ProjectName = serverBuildDefinition.ProjectName;
        buildDefinition2.ParentDefinition = buildDefinition3;
      }
      if (source.Queue != null)
      {
        serverBuildDefinition.Queue = new Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue()
        {
          Id = source.Queue.Id,
          Name = source.Queue.Name
        };
        if (source.Queue.Pool != null)
          serverBuildDefinition.Queue.Pool = new Microsoft.TeamFoundation.Build2.Server.TaskAgentPoolReference()
          {
            Id = source.Queue.Pool.Id,
            Name = source.Queue.Pool.Name,
            IsHosted = source.Queue.Pool.IsHosted
          };
      }
      if (source.Metrics != null)
        serverBuildDefinition.Metrics = source.Metrics.Select<Microsoft.TeamFoundation.Build.WebApi.BuildMetric, Microsoft.TeamFoundation.Build2.Server.BuildMetric>((Func<Microsoft.TeamFoundation.Build.WebApi.BuildMetric, Microsoft.TeamFoundation.Build2.Server.BuildMetric>) (x => x.ToServerBuildMetric())).ToList<Microsoft.TeamFoundation.Build2.Server.BuildMetric>();
      if (source.Process != null)
      {
        Microsoft.TeamFoundation.Build.WebApi.DesignerProcess process1 = source.Process as Microsoft.TeamFoundation.Build.WebApi.DesignerProcess;
        Microsoft.TeamFoundation.Build.WebApi.YamlProcess process2 = source.Process as Microsoft.TeamFoundation.Build.WebApi.YamlProcess;
        Microsoft.TeamFoundation.Build.WebApi.DockerProcess process3 = source.Process as Microsoft.TeamFoundation.Build.WebApi.DockerProcess;
        Microsoft.TeamFoundation.Build.WebApi.JustInTimeProcess process4 = source.Process as Microsoft.TeamFoundation.Build.WebApi.JustInTimeProcess;
        if (process1 != null)
          serverBuildDefinition.Process = (Microsoft.TeamFoundation.Build2.Server.BuildProcess) process1.ToServerDesignerProcess();
        else if (process2 != null)
          serverBuildDefinition.Process = (Microsoft.TeamFoundation.Build2.Server.BuildProcess) process2.ToServerYamlProcess();
        else if (process3 != null)
          serverBuildDefinition.Process = (Microsoft.TeamFoundation.Build2.Server.BuildProcess) process3.ToServerDockerProcess();
        else if (process4 != null)
          serverBuildDefinition.Process = (Microsoft.TeamFoundation.Build2.Server.BuildProcess) process4.ToServerJustInTimeProcess();
      }
      if (source.Options != null)
        serverBuildDefinition.Options.AddRange(source.Options.Where<Microsoft.TeamFoundation.Build.WebApi.BuildOption>((Func<Microsoft.TeamFoundation.Build.WebApi.BuildOption, bool>) (x => x != null)).Select<Microsoft.TeamFoundation.Build.WebApi.BuildOption, Microsoft.TeamFoundation.Build2.Server.BuildOption>((Func<Microsoft.TeamFoundation.Build.WebApi.BuildOption, Microsoft.TeamFoundation.Build2.Server.BuildOption>) (x => x.ToServerBuildOption())));
      if (source.Repository != null)
        serverBuildDefinition.Repository = source.Repository.ToBuildServerBuildRepository();
      if (source.Triggers != null)
      {
        foreach (Microsoft.TeamFoundation.Build.WebApi.BuildTrigger trigger in source.Triggers)
        {
          if (trigger is Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger webApiBCTrigger)
            serverBuildDefinition.BuildCompletionTriggers.Add(webApiBCTrigger.ToServerBCTrigger());
          else
            serverBuildDefinition.Triggers.Add(trigger.ToServerBuildTrigger());
        }
      }
      serverBuildDefinition.Variables.AddRange<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>, Dictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>>((IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>>) source.Variables.ToServerVariables());
      if (source.VariableGroups != null)
        serverBuildDefinition.VariableGroups = source.VariableGroups.Select<Microsoft.TeamFoundation.Build.WebApi.VariableGroup, Microsoft.TeamFoundation.Build2.Server.VariableGroup>((Func<Microsoft.TeamFoundation.Build.WebApi.VariableGroup, Microsoft.TeamFoundation.Build2.Server.VariableGroup>) (x => x.ToServerVariableGroup())).ToList<Microsoft.TeamFoundation.Build2.Server.VariableGroup>();
      if (source.Demands != null)
        serverBuildDefinition.Demands = source.Demands.Select<Microsoft.TeamFoundation.Build.WebApi.Demand, Microsoft.TeamFoundation.Build2.Server.Demand>((Func<Microsoft.TeamFoundation.Build.WebApi.Demand, Microsoft.TeamFoundation.Build2.Server.Demand>) (x => x.ToServerDemand())).ToList<Microsoft.TeamFoundation.Build2.Server.Demand>();
      serverBuildDefinition.Properties = source.Properties;
      serverBuildDefinition.Tags.AddRange((IEnumerable<string>) source.Tags);
      return serverBuildDefinition;
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildDefinition ToBuildServerBuildDefinition(
      this BuildDefinition3_2 webApiBuildDefinition,
      IVssRequestContext requestContext)
    {
      if (webApiBuildDefinition == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildDefinition) null;
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition1 = new Microsoft.TeamFoundation.Build2.Server.BuildDefinition();
      buildDefinition1.Id = webApiBuildDefinition.Id;
      buildDefinition1.Name = webApiBuildDefinition.Name;
      buildDefinition1.Uri = webApiBuildDefinition.Uri;
      buildDefinition1.Url = webApiBuildDefinition.Url;
      buildDefinition1.Path = webApiBuildDefinition.Path;
      buildDefinition1.Type = (Microsoft.TeamFoundation.Build2.Server.DefinitionType) webApiBuildDefinition.Type;
      buildDefinition1.QueueStatus = (Microsoft.TeamFoundation.Build2.Server.DefinitionQueueStatus) webApiBuildDefinition.QueueStatus;
      buildDefinition1.Revision = webApiBuildDefinition.Revision;
      buildDefinition1.CreatedDate = webApiBuildDefinition.CreatedDate;
      Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality? definitionQuality = webApiBuildDefinition.DefinitionQuality;
      buildDefinition1.DefinitionQuality = definitionQuality.HasValue ? new Microsoft.TeamFoundation.Build2.Server.DefinitionQuality?((Microsoft.TeamFoundation.Build2.Server.DefinitionQuality) definitionQuality.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.DefinitionQuality?();
      buildDefinition1.AuthoredBy = webApiBuildDefinition.AuthoredBy.ToIdentityGuid() ?? Guid.Empty;
      buildDefinition1.BuildNumberFormat = webApiBuildDefinition.BuildNumberFormat;
      buildDefinition1.Comment = webApiBuildDefinition.Comment;
      buildDefinition1.Description = webApiBuildDefinition.Description;
      buildDefinition1.DropLocation = webApiBuildDefinition.DropLocation;
      buildDefinition1.JobAuthorizationScope = (Microsoft.TeamFoundation.Build2.Server.BuildAuthorizationScope) webApiBuildDefinition.JobAuthorizationScope;
      buildDefinition1.JobTimeoutInMinutes = webApiBuildDefinition.JobTimeoutInMinutes;
      buildDefinition1.JobCancelTimeoutInMinutes = webApiBuildDefinition.JobCancelTimeoutInMinutes;
      buildDefinition1.BadgeEnabled = webApiBuildDefinition.BadgeEnabled;
      buildDefinition1.ProcessParameters = webApiBuildDefinition.ProcessParameters;
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition serverBuildDefinition = buildDefinition1;
      if (webApiBuildDefinition.Project != null)
      {
        serverBuildDefinition.ProjectId = webApiBuildDefinition.Project.Id;
        serverBuildDefinition.ProjectName = webApiBuildDefinition.Project.Name;
      }
      if (webApiBuildDefinition.ParentDefinition != null)
      {
        Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition2 = serverBuildDefinition;
        Microsoft.TeamFoundation.Build2.Server.BuildDefinition buildDefinition3 = new Microsoft.TeamFoundation.Build2.Server.BuildDefinition();
        buildDefinition3.Id = webApiBuildDefinition.ParentDefinition.Id;
        buildDefinition3.Revision = webApiBuildDefinition.ParentDefinition.Revision;
        buildDefinition3.Type = (Microsoft.TeamFoundation.Build2.Server.DefinitionType) webApiBuildDefinition.ParentDefinition.Type;
        buildDefinition3.Name = webApiBuildDefinition.ParentDefinition.Name;
        buildDefinition3.QueueStatus = (Microsoft.TeamFoundation.Build2.Server.DefinitionQueueStatus) webApiBuildDefinition.ParentDefinition.QueueStatus;
        buildDefinition3.Url = webApiBuildDefinition.ParentDefinition.Url;
        buildDefinition3.Uri = webApiBuildDefinition.ParentDefinition.Uri;
        buildDefinition3.ProjectId = serverBuildDefinition.ProjectId;
        buildDefinition3.ProjectName = serverBuildDefinition.ProjectName;
        buildDefinition2.ParentDefinition = buildDefinition3;
      }
      if (webApiBuildDefinition.Queue != null)
      {
        serverBuildDefinition.Queue = new Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue()
        {
          Id = webApiBuildDefinition.Queue.Id,
          Name = webApiBuildDefinition.Queue.Name
        };
        if (webApiBuildDefinition.Queue.Pool != null)
          serverBuildDefinition.Queue.Pool = new Microsoft.TeamFoundation.Build2.Server.TaskAgentPoolReference()
          {
            Id = webApiBuildDefinition.Queue.Pool.Id,
            Name = webApiBuildDefinition.Queue.Pool.Name,
            IsHosted = webApiBuildDefinition.Queue.Pool.IsHosted
          };
      }
      if (webApiBuildDefinition.Metrics != null)
        serverBuildDefinition.Metrics = webApiBuildDefinition.Metrics.Select<Microsoft.TeamFoundation.Build.WebApi.BuildMetric, Microsoft.TeamFoundation.Build2.Server.BuildMetric>((Func<Microsoft.TeamFoundation.Build.WebApi.BuildMetric, Microsoft.TeamFoundation.Build2.Server.BuildMetric>) (x => x.ToServerBuildMetric())).ToList<Microsoft.TeamFoundation.Build2.Server.BuildMetric>();
      Microsoft.TeamFoundation.Build2.Server.DesignerProcess designerProcess = new Microsoft.TeamFoundation.Build2.Server.DesignerProcess();
      Microsoft.TeamFoundation.Build2.Server.Phase phase = new Microsoft.TeamFoundation.Build2.Server.Phase()
      {
        Target = (Microsoft.TeamFoundation.Build2.Server.PhaseTarget) new Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget()
      };
      designerProcess.Phases.Add(phase);
      foreach (Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep step in webApiBuildDefinition.Steps)
        phase.Steps.Add(step.ToBuildServerBuildDefinitionStep());
      serverBuildDefinition.Process = (Microsoft.TeamFoundation.Build2.Server.BuildProcess) designerProcess;
      if (webApiBuildDefinition.Options != null)
        serverBuildDefinition.Options.AddRange(webApiBuildDefinition.Options.Where<Microsoft.TeamFoundation.Build.WebApi.BuildOption>((Func<Microsoft.TeamFoundation.Build.WebApi.BuildOption, bool>) (x => x != null)).Select<Microsoft.TeamFoundation.Build.WebApi.BuildOption, Microsoft.TeamFoundation.Build2.Server.BuildOption>((Func<Microsoft.TeamFoundation.Build.WebApi.BuildOption, Microsoft.TeamFoundation.Build2.Server.BuildOption>) (x => x.ToServerBuildOption())));
      if (webApiBuildDefinition.Repository != null)
        serverBuildDefinition.Repository = webApiBuildDefinition.Repository.ToBuildServerBuildRepository();
      if (webApiBuildDefinition.Triggers != null)
      {
        foreach (Microsoft.TeamFoundation.Build.WebApi.BuildTrigger trigger in webApiBuildDefinition.Triggers)
        {
          if (trigger != null)
          {
            if (trigger is Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger webApiBCTrigger)
              serverBuildDefinition.BuildCompletionTriggers.Add(webApiBCTrigger.ToServerBCTrigger());
            else
              serverBuildDefinition.Triggers.Add(trigger.ToServerBuildTrigger());
          }
        }
      }
      serverBuildDefinition.Variables.AddRange<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>, Dictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>>((IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>>) webApiBuildDefinition.Variables.ToServerVariables());
      if (webApiBuildDefinition.Demands != null)
        serverBuildDefinition.Demands = webApiBuildDefinition.Demands.Select<Microsoft.TeamFoundation.Build.WebApi.Demand, Microsoft.TeamFoundation.Build2.Server.Demand>((Func<Microsoft.TeamFoundation.Build.WebApi.Demand, Microsoft.TeamFoundation.Build2.Server.Demand>) (x => x.ToServerDemand())).ToList<Microsoft.TeamFoundation.Build2.Server.Demand>();
      serverBuildDefinition.Properties = webApiBuildDefinition.Properties;
      serverBuildDefinition.Tags.AddRange((IEnumerable<string>) webApiBuildDefinition.Tags);
      if (serverBuildDefinition.LatestBuild != null)
        serverBuildDefinition.LatestBuild = webApiBuildDefinition.LatestBuild.ToBuildServerBuildData(requestContext);
      if (serverBuildDefinition.LatestCompletedBuild != null)
        serverBuildDefinition.LatestCompletedBuild = webApiBuildDefinition.LatestCompletedBuild.ToBuildServerBuildData(requestContext);
      return serverBuildDefinition;
    }

    public static BuildDefinition3_2 ToBuildDefinition3_2(this Microsoft.TeamFoundation.Build.WebApi.BuildDefinition source)
    {
      if (source == null)
        return (BuildDefinition3_2) null;
      BuildDefinition3_2 buildDefinition32_1 = new BuildDefinition3_2();
      buildDefinition32_1.AuthoredBy = source.AuthoredBy;
      buildDefinition32_1.BadgeEnabled = source.BadgeEnabled;
      buildDefinition32_1.BuildNumberFormat = source.BuildNumberFormat;
      buildDefinition32_1.Comment = source.Comment;
      buildDefinition32_1.CreatedDate = source.CreatedDate;
      buildDefinition32_1.DefinitionQuality = source.DefinitionQuality;
      buildDefinition32_1.Description = source.Description;
      buildDefinition32_1.DropLocation = source.DropLocation;
      buildDefinition32_1.Id = source.Id;
      buildDefinition32_1.JobAuthorizationScope = source.JobAuthorizationScope;
      buildDefinition32_1.JobCancelTimeoutInMinutes = source.JobCancelTimeoutInMinutes;
      buildDefinition32_1.JobTimeoutInMinutes = source.JobTimeoutInMinutes;
      buildDefinition32_1.LatestBuild = source.LatestBuild;
      buildDefinition32_1.LatestCompletedBuild = source.LatestCompletedBuild;
      buildDefinition32_1.Name = source.Name;
      buildDefinition32_1.ParentDefinition = source.ParentDefinition;
      buildDefinition32_1.Path = source.Path;
      buildDefinition32_1.ProcessParameters = source.ProcessParameters;
      buildDefinition32_1.Project = source.Project;
      buildDefinition32_1.Queue = source.Queue;
      buildDefinition32_1.QueueStatus = source.QueueStatus;
      buildDefinition32_1.Repository = source.Repository;
      buildDefinition32_1.Revision = source.Revision;
      buildDefinition32_1.Type = source.Type;
      buildDefinition32_1.Uri = source.Uri;
      buildDefinition32_1.Url = source.Url;
      BuildDefinition3_2 buildDefinition32_2 = buildDefinition32_1;
      if (source.Demands.Count > 0)
        buildDefinition32_2.Demands.AddRange((IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Demand>) source.Demands);
      if (source.Metrics.Count > 0)
        buildDefinition32_2.Metrics.AddRange((IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildMetric>) source.Metrics);
      if (source.Options.Count > 0)
        buildDefinition32_2.Options.AddRange((IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildOption>) source.Options);
      if (source.Process is Microsoft.TeamFoundation.Build.WebApi.DesignerProcess process && process.Phases.Count > 0)
      {
        Microsoft.TeamFoundation.Build.WebApi.Phase phase = process.Phases[0];
        if (phase != null)
        {
          buildDefinition32_2.Steps.AddRange((IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep>) phase.Steps);
          Microsoft.TeamFoundation.Build.WebApi.IVariableMultiplierExecutionOptions multiplierOptions = phase.Target.GetMultiplierOptions();
          if (multiplierOptions != null)
          {
            Microsoft.TeamFoundation.Build.WebApi.BuildOption buildOption = new Microsoft.TeamFoundation.Build.WebApi.BuildOption()
            {
              Enabled = true,
              BuildOptionDefinition = new BuildOptionDefinitionReference()
              {
                Id = new Guid("{7C555368-CA64-4199-ADD6-9EBAF0B0137D}")
              }
            };
            JArray jarray = new JArray((object) multiplierOptions.Multipliers);
            buildOption.Inputs.Add("multipliers", jarray.ToString(Formatting.None));
            buildOption.Inputs.Add("parallel", multiplierOptions.MaxConcurrency > 1 ? "true" : "false");
            buildOption.Inputs.Add("continueOnError", multiplierOptions.ContinueOnError ? "true" : "false");
            buildDefinition32_2.Options.Add(buildOption);
          }
          if (phase.Target is Microsoft.TeamFoundation.Build.WebApi.ServerTarget)
            buildDefinition32_2.Options.Add(new Microsoft.TeamFoundation.Build.WebApi.BuildOption()
            {
              Enabled = true,
              BuildOptionDefinition = new BuildOptionDefinitionReference()
              {
                Id = new Guid("{5bc3cfb7-6b54-4a4b-b5d2-a3905949f8a6}")
              }
            });
        }
      }
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) source.Properties)
        buildDefinition32_2.Properties.Add(property.Key, property.Value);
      if (source.Tags.Count > 0)
        buildDefinition32_2.Tags.AddRange((IEnumerable<string>) source.Tags);
      if (source.Triggers.Count > 0)
        buildDefinition32_2.Triggers.AddRange((IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildTrigger>) source.Triggers);
      foreach (KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable> variable in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>>) source.Variables)
        buildDefinition32_2.Variables.Add(variable.Key, variable.Value);
      if (source.Links.Links.Count > 0)
        source.Links.CopyTo(buildDefinition32_2.Links);
      return buildDefinition32_2;
    }

    public static DefinitionReference ToDefinitionReference(
      this Microsoft.TeamFoundation.Build2.Server.BuildDefinition source,
      IVssRequestContext requestContext)
    {
      if (source == null)
        return (DefinitionReference) null;
      return new DefinitionReference()
      {
        CreatedDate = source.CreatedDate,
        Id = source.Id,
        Name = source.Name,
        Path = source.Path,
        QueueStatus = (Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus) source.QueueStatus,
        Revision = source.Revision,
        Type = (Microsoft.TeamFoundation.Build.WebApi.DefinitionType) source.Type,
        Uri = source.Uri
      };
    }

    public static BuildDefinitionReference3_2 ToBuildDefinitionReference3_2(
      this Microsoft.TeamFoundation.Build2.Server.BuildDefinition source,
      IVssRequestContext requestContext,
      IdentityMap identityMap = null)
    {
      if (source == null)
        return (BuildDefinitionReference3_2) null;
      if (identityMap == null)
        identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
      ISecuredObject securedObject = source.ToSecuredObject();
      BuildDefinitionReference3_2 definitionReference32 = new BuildDefinitionReference3_2();
      definitionReference32.AuthoredBy = identityMap.GetIdentityRef(requestContext, source.AuthoredBy);
      definitionReference32.CreatedDate = source.CreatedDate;
      Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? definitionQuality = source.DefinitionQuality;
      definitionReference32.DefinitionQuality = definitionQuality.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality?((Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality) definitionQuality.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality?();
      definitionReference32.Id = source.Id;
      definitionReference32.Name = source.Name;
      definitionReference32.ParentDefinition = source.ParentDefinition.ToDefinitionReference(requestContext);
      definitionReference32.Path = source.Path;
      definitionReference32.Project = requestContext.GetTeamProjectReference(source.ProjectId, source.ProjectName);
      definitionReference32.Queue = source.Queue.ToWebApiAgentPoolQueue(requestContext, securedObject);
      definitionReference32.QueueStatus = (Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus) source.QueueStatus;
      definitionReference32.Revision = source.Revision;
      definitionReference32.Type = (Microsoft.TeamFoundation.Build.WebApi.DefinitionType) source.Type;
      definitionReference32.Uri = source.Uri;
      definitionReference32.Url = source.GetRestUrl(requestContext);
      BuildDefinitionReference3_2 result = definitionReference32;
      if (source.Metrics.Count > 0)
        result.Metrics.AddRange(source.Metrics.MergePullRequestMetrics(requestContext).Select<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>((Func<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>) (x => x.ToWebApiBuildMetric((ISecuredObject) result))));
      result.Links.TryAddLink("self", securedObject, result.Url);
      result.Links.TryAddLink("web", securedObject, (Func<string>) (() => source.GetWebUrl(requestContext)));
      result.Links.TryAddLink("editor", securedObject, (Func<string>) (() => source.GetDesignerUrl(requestContext)));
      IBuildRouteService routeService = requestContext.GetService<IBuildRouteService>();
      result.Links.TryAddLink("badge", securedObject, (Func<string>) (() => routeService.GetStatusBadgeUrl(requestContext, source.ProjectId, source.Id)));
      return result;
    }

    public static BuildDefinition3_2 ToBuildDefinition3_2(
      this Microsoft.TeamFoundation.Build2.Server.BuildDefinition source,
      IVssRequestContext requestContext,
      Version apiVersion,
      IdentityMap identityMap = null)
    {
      if (source == null)
        return (BuildDefinition3_2) null;
      if (identityMap == null)
        identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
      ISecuredObject securedObject = source.ToSecuredObject();
      BuildDefinition3_2 buildDefinition32 = new BuildDefinition3_2();
      buildDefinition32.AuthoredBy = identityMap.GetIdentityRef(requestContext, source.AuthoredBy);
      buildDefinition32.BadgeEnabled = source.BadgeEnabled;
      buildDefinition32.BuildNumberFormat = source.BuildNumberFormat;
      buildDefinition32.Comment = source.Comment;
      buildDefinition32.CreatedDate = source.CreatedDate;
      Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? definitionQuality = source.DefinitionQuality;
      buildDefinition32.DefinitionQuality = definitionQuality.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality?((Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality) definitionQuality.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality?();
      buildDefinition32.Description = source.Description;
      buildDefinition32.DropLocation = source.DropLocation;
      buildDefinition32.Id = source.Id;
      buildDefinition32.JobAuthorizationScope = (Microsoft.TeamFoundation.Build.WebApi.BuildAuthorizationScope) source.JobAuthorizationScope;
      buildDefinition32.JobCancelTimeoutInMinutes = source.JobCancelTimeoutInMinutes;
      buildDefinition32.JobTimeoutInMinutes = source.JobTimeoutInMinutes;
      buildDefinition32.Name = source.Name;
      buildDefinition32.ParentDefinition = source.ParentDefinition.ToDefinitionReference(requestContext);
      buildDefinition32.Path = source.Path;
      buildDefinition32.ProcessParameters = source.ProcessParameters;
      buildDefinition32.Project = requestContext.GetTeamProjectReference(source.ProjectId, source.ProjectName);
      buildDefinition32.Queue = source.Queue.ToWebApiAgentPoolQueue(requestContext, securedObject);
      buildDefinition32.QueueStatus = (Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus) source.QueueStatus;
      buildDefinition32.Revision = source.Revision;
      buildDefinition32.Type = (Microsoft.TeamFoundation.Build.WebApi.DefinitionType) source.Type;
      buildDefinition32.Uri = source.Uri;
      buildDefinition32.Url = source.Url;
      BuildDefinition3_2 result = buildDefinition32;
      if (source.LatestBuild != null)
        result.LatestBuild = source.LatestBuild.ToWebApiBuild(requestContext, apiVersion, identityMap);
      if (source.LatestCompletedBuild != null)
        result.LatestCompletedBuild = source.LatestCompletedBuild.ToWebApiBuild(requestContext, apiVersion, identityMap);
      if (source.Repository != null)
        result.Repository = BuildRepositoryExtensions.ToWebApiBuildRepository(source.Repository, requestContext, source.ProjectId, (ISecuredObject) result, definitionId: new int?(source.Id));
      if (source.Demands.Count > 0)
        result.Demands.AddRange(source.Demands.Select<Microsoft.TeamFoundation.Build2.Server.Demand, Microsoft.TeamFoundation.Build.WebApi.Demand>((Func<Microsoft.TeamFoundation.Build2.Server.Demand, Microsoft.TeamFoundation.Build.WebApi.Demand>) (x => x.ToWebApiDemand((ISecuredObject) result))));
      if (source.Metrics.Count > 0)
        result.Metrics.AddRange(source.Metrics.MergePullRequestMetrics(requestContext).Select<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>((Func<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>) (x => x.ToWebApiBuildMetric((ISecuredObject) result))));
      if (source.Options.Count > 0)
        result.Options.AddRange(source.Options.Select<Microsoft.TeamFoundation.Build2.Server.BuildOption, Microsoft.TeamFoundation.Build.WebApi.BuildOption>((Func<Microsoft.TeamFoundation.Build2.Server.BuildOption, Microsoft.TeamFoundation.Build.WebApi.BuildOption>) (x => x.ToWebApiBuildOption((ISecuredObject) result))));
      if (source.Process is Microsoft.TeamFoundation.Build2.Server.DesignerProcess process && process.Phases.Count > 0)
      {
        Microsoft.TeamFoundation.Build2.Server.Phase phase = process.Phases[0];
        if (phase != null)
        {
          result.Steps.AddRange(phase.Steps.Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinitionStep, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep>) (x => x.ToWebApiBuildDefinitionStep((ISecuredObject) result))));
          Microsoft.TeamFoundation.Build2.Server.IVariableMultiplierExecutionOptions multiplierOptions = phase.Target.GetMultiplierOptions();
          if (multiplierOptions != null)
          {
            Microsoft.TeamFoundation.Build.WebApi.BuildOption buildOption = new Microsoft.TeamFoundation.Build.WebApi.BuildOption()
            {
              Enabled = true,
              BuildOptionDefinition = new BuildOptionDefinitionReference()
              {
                Id = new Guid("{7C555368-CA64-4199-ADD6-9EBAF0B0137D}")
              }
            };
            JArray jarray = new JArray((object) multiplierOptions.Multipliers);
            buildOption.Inputs.Add("multipliers", jarray.ToString(Formatting.None));
            buildOption.Inputs.Add("parallel", multiplierOptions.MaxConcurrency > 1 ? "true" : "false");
            buildOption.Inputs.Add("continueOnError", multiplierOptions.ContinueOnError ? "true" : "false");
            result.Options.Add(buildOption);
          }
          if (phase.Target is Microsoft.TeamFoundation.Build2.Server.ServerTarget)
            result.Options.Add(new Microsoft.TeamFoundation.Build.WebApi.BuildOption()
            {
              Enabled = true,
              BuildOptionDefinition = new BuildOptionDefinitionReference()
              {
                Id = new Guid("{5bc3cfb7-6b54-4a4b-b5d2-a3905949f8a6}")
              }
            });
        }
      }
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) source.Properties)
        result.Properties.Add(property.Key, property.Value);
      if (source.Tags.Count > 0)
        result.Tags.AddRange((IEnumerable<string>) source.Tags);
      if (source.Triggers.Count > 0)
        result.Triggers.AddRange(source.Triggers.Select<Microsoft.TeamFoundation.Build2.Server.BuildTrigger, Microsoft.TeamFoundation.Build.WebApi.BuildTrigger>((Func<Microsoft.TeamFoundation.Build2.Server.BuildTrigger, Microsoft.TeamFoundation.Build.WebApi.BuildTrigger>) (x => x.ToWebApiBuildTrigger((ISecuredObject) result))));
      if (source.BuildCompletionTriggers != null)
        result.Triggers.AddRange((IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildTrigger>) source.BuildCompletionTriggers.Select<Microsoft.TeamFoundation.Build2.Server.BuildCompletionTrigger, Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger>((Func<Microsoft.TeamFoundation.Build2.Server.BuildCompletionTrigger, Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger>) (x => x.ToWebApiBCTrigger(requestContext, (ISecuredObject) result))));
      foreach (KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> variable in source.Variables)
        result.Variables.Add(variable.Key, variable.Value.ToWebApiBuildDefinitionVariable((ISecuredObject) result));
      if (source.Drafts.Count > 0)
        result.Drafts.AddRange(source.Drafts.Select<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, DefinitionReference>((Func<Microsoft.TeamFoundation.Build2.Server.BuildDefinition, DefinitionReference>) (d => d.ToDefinitionReference(requestContext))));
      result.UpdateReferences(requestContext);
      result.AddLinks(requestContext);
      return result;
    }

    public static void AddLinks(this Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition, IVssRequestContext requestContext)
    {
      if (definition == null || definition.Project == null)
        return;
      using (PerformanceTimer.StartMeasure(requestContext, "BuildDefinitionExtensions.AddLinks"))
      {
        definition.Links.TryAddLink("self", (ISecuredObject) definition, definition.Url);
        definition.Links.TryAddLink("web", (ISecuredObject) definition, (Func<string>) (() => definition.GetWebUrl(requestContext)));
        definition.Links.TryAddLink("editor", (ISecuredObject) definition, (Func<string>) (() => definition.GetDesignerUrl(requestContext)));
        IBuildRouteService routeService = requestContext.GetService<IBuildRouteService>();
        definition.Links.TryAddLink("badge", (ISecuredObject) definition, (Func<string>) (() => routeService.GetStatusBadgeUrl(requestContext, definition.Project.Id, definition.Id)));
      }
    }

    public static void AddLinks(
      this BuildDefinition3_2 definition,
      IVssRequestContext requestContext)
    {
      if (definition == null || definition.Project == null)
        return;
      definition.Links.TryAddLink("self", (ISecuredObject) definition, definition.Url);
      definition.Links.TryAddLink("web", (ISecuredObject) definition, (Func<string>) (() => definition.GetWebUrl(requestContext)));
      IBuildRouteService routeService = requestContext.GetService<IBuildRouteService>();
      definition.Links.TryAddLink("editor", (ISecuredObject) definition, (Func<string>) (() => routeService.GetDefinitionDesignerUrl(requestContext, definition.Project.Id, definition.Id)));
      definition.Links.TryAddLink("badge", (ISecuredObject) definition, (Func<string>) (() => routeService.GetStatusBadgeUrl(requestContext, definition.Project.Id, definition.Id)));
    }

    public static void CheckSupportedBuildOptions(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition,
      IVssRequestContext requestContext,
      ApiResourceVersion apiResourceVersion)
    {
      if (definition == null)
        return;
      HashSet<Guid> checkedOptions = new HashSet<Guid>();
      foreach (Microsoft.TeamFoundation.Build.WebApi.BuildOption buildOption in definition.Options.Where<Microsoft.TeamFoundation.Build.WebApi.BuildOption>((Func<Microsoft.TeamFoundation.Build.WebApi.BuildOption, bool>) (o => o?.BuildOptionDefinition != null && !checkedOptions.Contains(o.BuildOptionDefinition.Id))))
      {
        Microsoft.TeamFoundation.Build.WebApi.BuildOption option = buildOption;
        checkedOptions.Add(option.BuildOptionDefinition.Id);
        requestContext.GetExtension<IBuildOption>((Func<IBuildOption, bool>) (bo => bo.GetId() == option.BuildOptionDefinition.Id))?.CheckSupported(requestContext, apiResourceVersion, definition.Process != null ? definition.Process.Type : 1);
      }
    }

    public static IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep> AllSteps(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition)
    {
      return !(definition?.Process is Microsoft.TeamFoundation.Build.WebApi.DesignerProcess process) ? Enumerable.Empty<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep>() : process.Phases.SelectMany<Microsoft.TeamFoundation.Build.WebApi.Phase, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep>((Func<Microsoft.TeamFoundation.Build.WebApi.Phase, IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep>>) (p => (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep>) p.Steps));
    }

    public static BuildDefinitionReference UpdateReferences(
      this BuildDefinitionReference definition,
      IVssRequestContext requestContext,
      IdentityMap identityMap)
    {
      if (definition != null)
      {
        if (definition.AuthoredBy != null)
          definition.AuthoredBy = identityMap.GetIdentityRef(requestContext, definition.AuthoredBy.Id);
        if (definition.Project != null && definition.Project.Id != Guid.Empty && string.IsNullOrEmpty(definition.Url))
          definition.Url = definition.GetRestUrl(requestContext);
        if (definition.Queue != null && string.IsNullOrEmpty(definition.Queue.Name))
        {
          TaskAgentQueue agentQueue = requestContext.GetService<IDistributedTaskPoolService>().GetAgentQueue(requestContext, definition.Project.Id, definition.Queue.Id);
          if (agentQueue != null)
            agentQueue.CopyTo(requestContext, definition.Queue);
        }
        if (definition.ParentDefinition != null)
        {
          definition.ParentDefinition.Project = definition.Project;
          if (string.IsNullOrEmpty(definition.ParentDefinition.Url))
            definition.ParentDefinition.Url = definition.ParentDefinition.GetRestUrl(requestContext);
        }
      }
      return definition;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.BuildDefinition UpdateReferences(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition,
      IVssRequestContext requestContext,
      IdentityMap identityMap)
    {
      if (definition != null)
      {
        using (PerformanceTimer.StartMeasure(requestContext, "BuildDefinitionExtensions.UpdateReferences"))
        {
          if (definition.AuthoredBy != null)
            definition.AuthoredBy = identityMap.GetIdentityRef(requestContext, definition.AuthoredBy.Id);
          if (definition.Project != null && definition.Project.Id != Guid.Empty && string.IsNullOrEmpty(definition.Url))
            definition.Url = definition.GetRestUrl(requestContext);
          if (definition.Queue != null && string.IsNullOrEmpty(definition.Queue.Name))
          {
            TaskAgentQueue agentQueue = requestContext.GetService<IDistributedTaskPoolService>().GetAgentQueue(requestContext, definition.Project.Id, definition.Queue.Id);
            if (agentQueue != null)
              agentQueue.CopyTo(requestContext, definition.Queue);
          }
          if (definition.ParentDefinition != null)
          {
            definition.ParentDefinition.Project = definition.Project;
            if (string.IsNullOrEmpty(definition.ParentDefinition.Url))
              definition.ParentDefinition.Url = definition.ParentDefinition.GetRestUrl(requestContext);
          }
          if (definition.Drafts.Count > 0)
          {
            foreach (DefinitionReference draft in definition.Drafts)
            {
              draft.Project = definition.Project;
              if (string.IsNullOrEmpty(draft.Url))
                draft.Url = draft.GetRestUrl(requestContext);
            }
          }
          definition.ConvertTriggerPathsToProjectName(requestContext);
          definition.ConvertTaskParametersToProjectName(requestContext);
          definition.ConvertVariablesToProjectName(requestContext);
        }
      }
      return definition;
    }

    public static BuildDefinition3_2 UpdateReferences(
      this BuildDefinition3_2 definition,
      IVssRequestContext requestContext)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      return definition.UpdateReferences(requestContext, new IdentityMap(service));
    }

    public static BuildDefinition3_2 UpdateReferences(
      this BuildDefinition3_2 definition,
      IVssRequestContext requestContext,
      IdentityMap identityMap)
    {
      if (definition != null)
      {
        if (definition.AuthoredBy != null)
          definition.AuthoredBy = identityMap.GetIdentityRef(requestContext, definition.AuthoredBy.Id);
        if (definition.Project != null && definition.Project.Id != Guid.Empty && string.IsNullOrEmpty(definition.Url))
          definition.Url = definition.GetRestUrl(requestContext);
        if (definition.ParentDefinition != null)
        {
          definition.ParentDefinition.Project = definition.Project;
          if (string.IsNullOrEmpty(definition.ParentDefinition.Url))
            definition.ParentDefinition.Url = definition.ParentDefinition.GetRestUrl(requestContext);
        }
        if (definition.Drafts.Count > 0)
        {
          foreach (DefinitionReference draft in definition.Drafts)
          {
            draft.Project = definition.Project;
            if (string.IsNullOrEmpty(draft.Url))
              draft.Url = draft.GetRestUrl(requestContext);
          }
        }
        definition.ConvertTriggerPathsToProjectName(requestContext);
        definition.ConvertTaskParametersToProjectName(requestContext);
        definition.ConvertVariablesToProjectName(requestContext);
      }
      return definition;
    }

    public static void ConvertTriggerPathsToProjectName(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      foreach (Microsoft.TeamFoundation.Build.WebApi.BuildTrigger trigger1 in definition.Triggers)
      {
        if (trigger1 is Microsoft.TeamFoundation.Build.WebApi.ContinuousIntegrationTrigger trigger2)
          trigger2.ConvertFilterPathsToProjectName(requestContext);
        if (trigger1 is Microsoft.TeamFoundation.Build.WebApi.GatedCheckInTrigger trigger3)
          trigger3.ConvertPathFiltersPathsToProjectName(requestContext);
      }
    }

    public static void ConvertTaskParametersToProjectName(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      foreach (Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep allStep in definition.AllSteps())
      {
        foreach (string key in allStep.Inputs.Keys.ToList<string>())
          allStep.Inputs[key] = TFVCPathHelper.ConvertToPathWithProjectName(requestContext, allStep.Inputs[key]);
      }
    }

    public static void ConvertVariablesToProjectName(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      foreach (KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable> variable in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>>) definition.Variables)
        definition.Variables[variable.Key].Value = TFVCPathHelper.ConvertToPathWithProjectName(requestContext, variable.Value.Value);
    }

    public static void ConvertTriggerPathsToProjectName(
      this BuildDefinition3_2 definition,
      IVssRequestContext requestContext)
    {
      foreach (Microsoft.TeamFoundation.Build.WebApi.BuildTrigger trigger1 in definition.Triggers)
      {
        if (trigger1 is Microsoft.TeamFoundation.Build.WebApi.ContinuousIntegrationTrigger trigger2)
          trigger2.ConvertFilterPathsToProjectName(requestContext);
        if (trigger1 is Microsoft.TeamFoundation.Build.WebApi.GatedCheckInTrigger trigger3)
          trigger3.ConvertPathFiltersPathsToProjectName(requestContext);
      }
    }

    public static void ConvertTaskParametersToProjectName(
      this BuildDefinition3_2 definition,
      IVssRequestContext requestContext)
    {
      foreach (Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionStep step in definition.Steps)
      {
        foreach (string key in step.Inputs.Keys.ToList<string>())
          step.Inputs[key] = TFVCPathHelper.ConvertToPathWithProjectName(requestContext, step.Inputs[key]);
      }
    }

    public static void ConvertVariablesToProjectName(
      this BuildDefinition3_2 definition,
      IVssRequestContext requestContext)
    {
      foreach (KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable> variable in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>>) definition.Variables)
        definition.Variables[variable.Key].Value = TFVCPathHelper.ConvertToPathWithProjectName(requestContext, variable.Value.Value);
    }

    public static void ConvertFilterPathsToProjectName(
      this Microsoft.TeamFoundation.Build.WebApi.ContinuousIntegrationTrigger trigger,
      IVssRequestContext requestContext)
    {
      BuildDefinitionExtensions.ConvertPathsToProjectName(requestContext, trigger.BranchFilters);
      BuildDefinitionExtensions.ConvertPathsToProjectName(requestContext, trigger.PathFilters);
    }

    public static ISecuredObject ToSecuredObject(this MinimalBuildDefinition definition) => (ISecuredObject) new Microsoft.TeamFoundation.Build2.Server.SecuredObject(Security.BuildNamespaceId, BuildPermissions.ViewBuildDefinition, definition.GetToken());

    private static void ConvertPathsToProjectName(
      IVssRequestContext requestContext,
      List<string> paths)
    {
      for (int index = 0; index < paths.Count; ++index)
      {
        string path = paths[index];
        paths[index] = path.Substring(0, 1) + TFVCPathHelper.ConvertToPathWithProjectName(requestContext, path.Substring(1));
      }
    }

    public static void ConvertPathFiltersPathsToProjectName(
      this Microsoft.TeamFoundation.Build.WebApi.GatedCheckInTrigger trigger,
      IVssRequestContext requestContext)
    {
      for (int index = 0; index < trigger.PathFilters.Count; ++index)
        trigger.PathFilters[index] = trigger.PathFilters[index].Substring(0, 1) + TFVCPathHelper.ConvertToPathWithProjectName(requestContext, trigger.PathFilters[index].Substring(1));
    }
  }
}
