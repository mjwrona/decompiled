// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildExtensions
  {
    private static readonly Guid s_externalUserId = new Guid("CBB1919B-AB53-4CA5-898C-E1DA89DAF9AF");
    private static readonly string[] s_accessMappingsToCheck = new string[2]
    {
      AccessMappingConstants.VstsAccessMapping,
      AccessMappingConstants.DevOpsAccessMapping
    };
    private const string TraceLayer = "BuildExtensions";
    private static readonly Version v6_1 = new Version(6, 1);

    public static Microsoft.TeamFoundation.Build.WebApi.Build ToWebApiBuild(
      this IReadOnlyBuildData srvBuildData,
      IVssRequestContext requestContext,
      Version apiVersion,
      IdentityMap identityMap = null,
      AgentPoolQueueCache queueCache = null,
      BuildRepositoryCache repositoryCache = null,
      bool updateReferencesAndLinks = true,
      bool refreshRepoName = true,
      bool resolveExternalIdentities = false)
    {
      if (srvBuildData == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      using (PerformanceTimer.StartMeasure(requestContext, "BuildExtensions.ToWebApiBuild"))
      {
        if (identityMap == null)
          identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
        if (queueCache == null)
          queueCache = new AgentPoolQueueCache(requestContext);
        if (repositoryCache == null)
          repositoryCache = new BuildRepositoryCache(requestContext);
        TeamProjectReference projectReference1;
        if (updateReferencesAndLinks)
          projectReference1 = requestContext.GetTeamProjectReference(srvBuildData.ProjectId);
        else
          projectReference1 = new TeamProjectReference()
          {
            Id = srvBuildData.ProjectId
          };
        Microsoft.TeamFoundation.Build.WebApi.Build build1 = new Microsoft.TeamFoundation.Build.WebApi.Build();
        build1.Id = srvBuildData.Id;
        build1.BuildNumber = srvBuildData.BuildNumber;
        Microsoft.TeamFoundation.Build2.Server.BuildStatus? status = srvBuildData.Status;
        build1.Status = status.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?((Microsoft.TeamFoundation.Build.WebApi.BuildStatus) status.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
        Microsoft.TeamFoundation.Build2.Server.BuildResult? result = srvBuildData.Result;
        build1.Result = result.HasValue ? new Microsoft.TeamFoundation.Build.WebApi.BuildResult?((Microsoft.TeamFoundation.Build.WebApi.BuildResult) result.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build.WebApi.BuildResult?();
        build1.QueueTime = srvBuildData.QueueTime;
        build1.StartTime = srvBuildData.StartTime;
        build1.FinishTime = srvBuildData.FinishTime;
        build1.BuildNumberRevision = srvBuildData.BuildNumberRevision;
        build1.Project = projectReference1;
        build1.Uri = updateReferencesAndLinks ? srvBuildData.Uri : (Uri) null;
        build1.SourceBranch = srvBuildData.SourceBranch;
        build1.SourceVersion = srvBuildData.SourceVersion;
        build1.Priority = (Microsoft.TeamFoundation.Build.WebApi.QueuePriority) srvBuildData.Priority;
        build1.Reason = (Microsoft.TeamFoundation.Build.WebApi.BuildReason) srvBuildData.Reason;
        build1.RequestedBy = identityMap.GetIdentityRef(requestContext, srvBuildData.RequestedBy);
        build1.LastChangedDate = srvBuildData.LastChangedDate;
        build1.LastChangedBy = identityMap.GetIdentityRef(requestContext, srvBuildData.LastChangedBy);
        build1.DeletedDate = srvBuildData.DeletedDate;
        Guid? deletedBy = srvBuildData.DeletedBy;
        IdentityRef identityRef;
        if (!deletedBy.HasValue)
        {
          identityRef = (IdentityRef) null;
        }
        else
        {
          IdentityMap identityMap1 = identityMap;
          IVssRequestContext requestContext1 = requestContext;
          deletedBy = srvBuildData.DeletedBy;
          Guid identifier = deletedBy.Value;
          identityRef = identityMap1.GetIdentityRef(requestContext1, identifier);
        }
        build1.DeletedBy = identityRef;
        build1.DeletedReason = srvBuildData.DeletedReason;
        build1.Parameters = srvBuildData.Parameters;
        build1.QueueOptions = (Microsoft.TeamFoundation.Build.WebApi.QueueOptions) srvBuildData.QueueOptions;
        build1.Deleted = srvBuildData.Deleted;
        build1.Properties = srvBuildData.Properties;
        build1.Tags = srvBuildData.Tags;
        build1.AppendCommitMessageToRunName = srvBuildData.AppendCommitMessageToRunName;
        build1.KeepForever = apiVersion >= BuildExtensions.v6_1 ? new bool?() : new bool?(srvBuildData.RetentionLeases.Any<Microsoft.TeamFoundation.Build2.Server.RetentionLease>((Func<Microsoft.TeamFoundation.Build2.Server.RetentionLease, bool>) (l => !l.HighPriority)));
        build1.RetainedByRelease = new bool?(srvBuildData.RetentionLeases.Any<Microsoft.TeamFoundation.Build2.Server.RetentionLease>((Func<Microsoft.TeamFoundation.Build2.Server.RetentionLease, bool>) (l => l.HighPriority)));
        Microsoft.TeamFoundation.Build.WebApi.Build webApiBuild = build1;
        if (srvBuildData.TemplateParameters != null)
        {
          foreach (KeyValuePair<string, object> templateParameter in srvBuildData.TemplateParameters)
          {
            if (templateParameter.Key != null)
              webApiBuild.TemplateParameters[templateParameter.Key] = templateParameter.Value != null ? templateParameter.Value.ToString() : "";
          }
        }
        try
        {
          webApiBuild.TriggerInfo = srvBuildData.TriggerInfo;
        }
        catch (Exception ex)
        {
          requestContext.TraceError(nameof (BuildExtensions), "TriggerInfo failed to deserialize the string {0} with the following error: {1}", (object) srvBuildData.TriggerInfoString, (object) ex.Message);
          webApiBuild.TriggerInfo = (IDictionary<string, string>) new Dictionary<string, string>();
        }
        string token = srvBuildData.GetToken();
        webApiBuild.SetNestingSecurityToken(token);
        webApiBuild.RequestedFor = BuildExtensions.GetRequestedForIdentity(requestContext, srvBuildData, identityMap, (ISecuredObject) webApiBuild, resolveExternalIdentities);
        if (srvBuildData.Definition != null)
        {
          if (new BuildSecurityProvider().HasDefinitionPermission(requestContext, srvBuildData.ProjectId, srvBuildData.Definition, BuildPermissions.ViewBuilds, false))
          {
            if (!updateReferencesAndLinks)
            {
              Microsoft.TeamFoundation.Build.WebApi.Build build2 = webApiBuild;
              BuildDefinitionReference definitionReference = new BuildDefinitionReference();
              definitionReference.Id = srvBuildData.Definition.Id;
              definitionReference.Path = srvBuildData.Definition.Path;
              definitionReference.Name = srvBuildData.Definition.Name;
              definitionReference.Project = projectReference1;
              build2.Definition = (DefinitionReference) definitionReference;
            }
            else
            {
              Microsoft.TeamFoundation.Build.WebApi.Build build3 = webApiBuild;
              BuildDefinitionReference definitionReference = new BuildDefinitionReference();
              definitionReference.Id = srvBuildData.Definition.Id;
              definitionReference.Path = srvBuildData.Definition.Path;
              definitionReference.Name = srvBuildData.Definition.Name;
              definitionReference.Project = projectReference1;
              definitionReference.Type = (Microsoft.TeamFoundation.Build.WebApi.DefinitionType) srvBuildData.Definition.Type;
              definitionReference.QueueStatus = (Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus) srvBuildData.Definition.QueueStatus;
              definitionReference.Revision = srvBuildData.Definition.Revision;
              definitionReference.Uri = updateReferencesAndLinks ? srvBuildData.Definition.Uri : (Uri) null;
              build3.Definition = (DefinitionReference) definitionReference;
            }
            webApiBuild.Definition.SetRequiredPermissions(BuildPermissions.ViewBuilds);
          }
          else
          {
            webApiBuild.Definition = new DefinitionReference()
            {
              Id = srvBuildData.Definition.Id,
              Path = srvBuildData.Definition.Path,
              Type = (Microsoft.TeamFoundation.Build.WebApi.DefinitionType) srvBuildData.Definition.Type,
              Name = srvBuildData.Definition.Name,
              Project = projectReference1
            };
            webApiBuild.Definition.SetRequiredPermissions(BuildPermissions.ViewBuilds);
            webApiBuild.Definition.SetNestingSecurityToken(token);
          }
        }
        int? queueId1 = srvBuildData.QueueId;
        if (queueId1.HasValue)
        {
          AgentPoolQueueCache agentPoolQueueCache = queueCache;
          Guid projectId = srvBuildData.ProjectId;
          queueId1 = srvBuildData.QueueId;
          int queueId2 = queueId1.Value;
          Microsoft.TeamFoundation.Build.WebApi.Build build4 = webApiBuild;
          Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue queue = agentPoolQueueCache.GetQueue(projectId, queueId2, (ISecuredObject) build4);
          if (queue != null && !updateReferencesAndLinks)
            webApiBuild.Queue = new Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue((ISecuredObject) webApiBuild)
            {
              Id = queue.Id,
              Name = queue.Name
            };
          else
            webApiBuild.Queue = queue;
        }
        if (srvBuildData.Demands != null)
          webApiBuild.Demands = srvBuildData.Demands.Select<Microsoft.TeamFoundation.Build2.Server.Demand, Microsoft.TeamFoundation.Build.WebApi.Demand>((Func<Microsoft.TeamFoundation.Build2.Server.Demand, Microsoft.TeamFoundation.Build.WebApi.Demand>) (x => x.ToWebApiDemand((ISecuredObject) webApiBuild))).ToList<Microsoft.TeamFoundation.Build.WebApi.Demand>();
        if (srvBuildData.OrchestrationPlan != null)
          webApiBuild.OrchestrationPlan = srvBuildData.OrchestrationPlan.ToWebApiTaskOrchestrationPlanReference((ISecuredObject) webApiBuild);
        if (srvBuildData.Plans != null)
          webApiBuild.Plans = srvBuildData.Plans.Select<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, Microsoft.TeamFoundation.Build.WebApi.TaskOrchestrationPlanReference>((Func<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, Microsoft.TeamFoundation.Build.WebApi.TaskOrchestrationPlanReference>) (x => x.ToWebApiTaskOrchestrationPlanReference((ISecuredObject) webApiBuild))).ToList<Microsoft.TeamFoundation.Build.WebApi.TaskOrchestrationPlanReference>();
        if (srvBuildData.Logs != null)
          webApiBuild.Logs = srvBuildData.Logs.ToWebApiLogReference((ISecuredObject) webApiBuild);
        if (srvBuildData.Repository != null)
        {
          Microsoft.TeamFoundation.Build.WebApi.BuildRepository apiBuildRepository = srvBuildData.Repository.ToWebApiBuildRepository(requestContext, srvBuildData.ProjectId, (ISecuredObject) webApiBuild, repositoryCache, refreshRepoName, new int?(srvBuildData.Definition.Id));
          if (apiBuildRepository != null && !updateReferencesAndLinks)
            webApiBuild.Repository = new Microsoft.TeamFoundation.Build.WebApi.BuildRepository((ISecuredObject) webApiBuild)
            {
              Id = apiBuildRepository.Id,
              Name = apiBuildRepository.Name,
              Type = apiBuildRepository.Type,
              Url = apiBuildRepository.Url
            };
          else
            webApiBuild.Repository = apiBuildRepository;
        }
        if (srvBuildData.ValidationResults != null)
          webApiBuild.ValidationResults.AddRange(srvBuildData.ValidationResults.Select<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>) (x => x.ToWebApiBuildRequestValidationResult((ISecuredObject) webApiBuild))));
        if (srvBuildData.TriggeredByBuild != null)
        {
          TeamProjectReference projectReference2 = requestContext.GetTeamProjectReference(srvBuildData.TriggeredByBuild.ProjectId);
          webApiBuild.TriggeredByBuild = new Microsoft.TeamFoundation.Build.WebApi.Build()
          {
            Id = srvBuildData.TriggeredByBuild.BuildId,
            Definition = new DefinitionReference()
            {
              Id = srvBuildData.TriggeredByBuild.DefinitionId,
              Revision = srvBuildData.TriggeredByBuild.DefinitionVersion,
              Project = projectReference2
            },
            Project = projectReference2
          };
          webApiBuild.TriggeredByBuild.SetNestingSecurityToken(token);
          webApiBuild.TriggeredByBuild.Definition.SetRequiredPermissions(BuildPermissions.ViewBuilds);
          webApiBuild.TriggeredByBuild.Definition.SetNestingSecurityToken(token);
        }
        if (updateReferencesAndLinks)
        {
          webApiBuild.UpdateReferences(requestContext, true);
          webApiBuild.AddLinks(requestContext);
        }
        else
          webApiBuild.AddTriggeringBuildNumber(requestContext);
        return webApiBuild;
      }
    }

    public static BuildData ToBuildServerBuildData(
      this Microsoft.TeamFoundation.Build.WebApi.Build webApiBuild,
      IVssRequestContext requestContext,
      IdentityMap identityMap = null)
    {
      if (webApiBuild == null)
        return (BuildData) null;
      if (identityMap == null)
        identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
      BuildData buildData = new BuildData();
      buildData.Id = webApiBuild.Id;
      buildData.BuildNumber = webApiBuild.BuildNumber;
      buildData.ProjectId = webApiBuild.Project != null ? webApiBuild.Project.Id : Guid.Empty;
      buildData.Reason = (Microsoft.TeamFoundation.Build2.Server.BuildReason) webApiBuild.Reason;
      Microsoft.TeamFoundation.Build.WebApi.BuildResult? result = webApiBuild.Result;
      buildData.Result = result.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildResult?((Microsoft.TeamFoundation.Build2.Server.BuildResult) result.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildResult?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? status = webApiBuild.Status;
      buildData.Status = status.HasValue ? new Microsoft.TeamFoundation.Build2.Server.BuildStatus?((Microsoft.TeamFoundation.Build2.Server.BuildStatus) status.GetValueOrDefault()) : new Microsoft.TeamFoundation.Build2.Server.BuildStatus?();
      buildData.BuildNumberRevision = webApiBuild.BuildNumberRevision;
      buildData.SourceBranch = webApiBuild.SourceBranch;
      buildData.SourceVersion = webApiBuild.SourceVersion;
      buildData.Parameters = webApiBuild.Parameters;
      buildData.Priority = (Microsoft.TeamFoundation.Build2.Server.QueuePriority) webApiBuild.Priority;
      buildData.StartTime = webApiBuild.StartTime;
      buildData.FinishTime = webApiBuild.FinishTime;
      buildData.QueueTime = webApiBuild.QueueTime;
      buildData.LastChangedDate = webApiBuild.LastChangedDate;
      buildData.Deleted = webApiBuild.Deleted;
      buildData.DeletedReason = webApiBuild.DeletedReason;
      buildData.DeletedDate = webApiBuild.DeletedDate;
      buildData.LegacyInputKeepForever = webApiBuild.KeepForever;
      buildData.LegacyInputRetainedByRelease = webApiBuild.RetainedByRelease;
      buildData.QueueOptions = (Microsoft.TeamFoundation.Build2.Server.QueueOptions) webApiBuild.QueueOptions;
      buildData.Tags = webApiBuild.Tags;
      buildData.Uri = webApiBuild.Uri;
      buildData.Properties = webApiBuild.Properties;
      buildData.TriggerInfo = webApiBuild.TriggerInfo;
      buildData.AppendCommitMessageToRunName = webApiBuild.AppendCommitMessageToRunName;
      BuildData buildServerBuildData = buildData;
      buildServerBuildData.RequestedFor = IdentityRefExtensions.ToIdentityGuid(webApiBuild.RequestedFor, requestContext, identityMap);
      buildServerBuildData.RequestedBy = IdentityRefExtensions.ToIdentityGuid(webApiBuild.RequestedBy, requestContext, identityMap);
      buildServerBuildData.LastChangedBy = IdentityRefExtensions.ToIdentityGuid(webApiBuild.LastChangedBy, requestContext, identityMap);
      buildServerBuildData.DeletedBy = new Guid?(IdentityRefExtensions.ToIdentityGuid(webApiBuild.DeletedBy, requestContext, identityMap));
      foreach (KeyValuePair<string, string> templateParameter in webApiBuild.TemplateParameters)
        buildServerBuildData.TemplateParameters[templateParameter.Key] = (object) templateParameter.Value;
      if (webApiBuild.Definition != null)
      {
        buildServerBuildData.Definition = new MinimalBuildDefinition()
        {
          Id = webApiBuild.Definition.Id,
          Name = webApiBuild.Definition.Name,
          Type = (Microsoft.TeamFoundation.Build2.Server.DefinitionType) webApiBuild.Definition.Type,
          Path = webApiBuild.Definition.Path,
          QueueStatus = (Microsoft.TeamFoundation.Build2.Server.DefinitionQueueStatus) webApiBuild.Definition.QueueStatus,
          Revision = webApiBuild.Definition.Revision,
          Uri = webApiBuild.Definition.Uri
        };
        if (webApiBuild.Definition.Project != null)
          buildServerBuildData.Definition.ProjectId = webApiBuild.Definition.Project.Id;
      }
      if (webApiBuild.Queue != null)
        buildServerBuildData.QueueId = new int?(webApiBuild.Queue.Id);
      if (webApiBuild.Demands != null)
        buildServerBuildData.Demands = webApiBuild.Demands.Select<Microsoft.TeamFoundation.Build.WebApi.Demand, Microsoft.TeamFoundation.Build2.Server.Demand>((Func<Microsoft.TeamFoundation.Build.WebApi.Demand, Microsoft.TeamFoundation.Build2.Server.Demand>) (x => x.ToServerDemand())).ToList<Microsoft.TeamFoundation.Build2.Server.Demand>();
      if (webApiBuild.OrchestrationPlan != null)
        buildServerBuildData.OrchestrationPlan = webApiBuild.OrchestrationPlan.ToServerTaskOrchPlanRef();
      if (webApiBuild.Plans != null)
        buildServerBuildData.Plans = webApiBuild.Plans.Select<Microsoft.TeamFoundation.Build.WebApi.TaskOrchestrationPlanReference, Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>((Func<Microsoft.TeamFoundation.Build.WebApi.TaskOrchestrationPlanReference, Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>) (x => x.ToServerTaskOrchPlanRef())).ToList<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>();
      if (webApiBuild.Logs != null)
        buildServerBuildData.Logs = webApiBuild.Logs.ToServerLogReference();
      if (webApiBuild.Repository != null)
        buildServerBuildData.Repository = (MinimalBuildRepository) webApiBuild.Repository.ToBuildServerBuildRepository();
      if (webApiBuild.ValidationResults != null)
        buildServerBuildData.ValidationResults.AddRange(webApiBuild.ValidationResults.Select<Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult, Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult, Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult>) (x => x.ToServerBuildRequestValidationResult())));
      if (webApiBuild.TriggeredByBuild != null)
      {
        buildServerBuildData.TriggeredByBuild = new TriggeredByBuild()
        {
          BuildId = webApiBuild.TriggeredByBuild.Id
        };
        if (webApiBuild.TriggeredByBuild.Definition != null)
        {
          buildServerBuildData.TriggeredByBuild.DefinitionId = webApiBuild.TriggeredByBuild.Definition.Id;
          buildServerBuildData.TriggeredByBuild.DefinitionVersion = webApiBuild.TriggeredByBuild.Definition.Revision;
        }
        if (webApiBuild.TriggeredByBuild.Project != null)
          buildServerBuildData.TriggeredByBuild.ProjectId = webApiBuild.TriggeredByBuild.Project.Id;
      }
      if (webApiBuild.AgentSpecification != null)
        buildServerBuildData.AgentSpecification = webApiBuild.AgentSpecification.ToServerAgentSpecification();
      return buildServerBuildData;
    }

    public static string GetStatusText(this Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      switch (build.Status.GetValueOrDefault())
      {
        case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress:
          return Resources.BuildStatusTextInProgress();
        case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed:
          return Resources.BuildStatusTextCompleted((object) BuildExtensions.GetBuildResultText(build.Result));
        case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Cancelling:
          return Resources.BuildStatusTextCancelling();
        case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Postponed:
          return Resources.BuildStatusTextPostPoned();
        default:
          return Resources.BuildStatusTextNotStarted();
      }
    }

    public static string GetDurationText(this Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      DateTime? nullable = build.StartTime;
      DateTime from = nullable ?? DateTime.UtcNow;
      nullable = build.FinishTime;
      DateTime dateTime = nullable ?? DateTime.UtcNow;
      string str = string.Empty;
      if (build.Queue != null)
        str = build.Queue.Name;
      else if (build.Controller != null)
        str = build.Controller.Name;
      if (dateTime == DateTime.MinValue)
        dateTime = from;
      return build.Status.GetValueOrDefault() == Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed ? Resources.BuildDurationTextCompleted((object) DateTimeHelper.GetDurationText(from, dateTime), (object) str, (object) DateTimeHelper.GetDurationText(dateTime, DateTime.UtcNow)) : Resources.BuildDurationTextInProgress((object) DateTimeHelper.GetDurationText(from, DateTime.UtcNow), (object) str);
    }

    internal static Microsoft.TeamFoundation.Build.WebApi.Build UpdateReferences(
      this Microsoft.TeamFoundation.Build.WebApi.Build build,
      IVssRequestContext requestContext,
      bool skipAddingBuildLinks = false)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BuildExtensions.UpdateReferences"))
      {
        if (build != null)
        {
          requestContext.TraceInfo(0, "Performance", "Creating urls for build {0}.", (object) build.Id);
          if (string.IsNullOrEmpty(build.Url))
            build.Url = build.GetRestUrl(requestContext);
          if (build.Definition != null && string.IsNullOrEmpty(build.Definition.Url))
            build.Definition.Url = build.Definition.GetRestUrl(requestContext);
          if (build.Project != null && build.Logs != null && string.Equals(build.Logs.Type, "Container", StringComparison.OrdinalIgnoreCase))
          {
            IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
            build.Logs.Url = service.GetBuildLogsRestUrl(requestContext, build.Project.Id, build.Id);
          }
          if (build.TriggeredByBuild != null)
          {
            if (build.TriggeredByBuild.Definition != null)
              build.TriggeredByBuild.Definition.Url = build.TriggeredByBuild.Definition.GetRestUrl(requestContext);
            build.AddTriggeringBuildNumber(requestContext);
          }
          if (!skipAddingBuildLinks)
          {
            requestContext.TraceInfo(0, "Performance", "Adding links for build {0}.", (object) build.Id);
            build.AddLinks(requestContext);
            requestContext.TraceInfo(0, "Performance", "Update references for build {0} completed.", (object) build.Id);
          }
        }
        return build;
      }
    }

    internal static Microsoft.TeamFoundation.Build.WebApi.Build AddTriggeringBuildNumber(
      this Microsoft.TeamFoundation.Build.WebApi.Build build,
      IVssRequestContext requestContext)
    {
      if (build != null && build.TriggeredByBuild != null && requestContext.UserContext != (IdentityDescriptor) null)
      {
        BuildData buildById = requestContext.GetService<IBuildService>().GetBuildById(requestContext, build.TriggeredByBuild.Project.Id, build.TriggeredByBuild.Id);
        if (buildById != null)
          build.TriggeredByBuild.BuildNumber = buildById.BuildNumber;
      }
      return build;
    }

    public static void AddLinks(this Microsoft.TeamFoundation.Build.WebApi.Build build, IVssRequestContext requestContext)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BuildExtensions.AddLinks"))
      {
        if (build == null)
          return;
        build.Links.TryAddLink("self", (ISecuredObject) build, (Func<string>) (() => build.GetSelfLink(requestContext)));
        build.Links.TryAddLink("web", (ISecuredObject) build, (Func<string>) (() => build.GetWebUrl(requestContext)));
        if (build.Repository != null)
        {
          IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, build.Repository.Type, false);
          if (sourceProvider != null && sourceProvider.GetAttributes(requestContext).SupportsSourceLinks)
            build.Links.TryAddLink("sourceVersionDisplayUri", (ISecuredObject) build, (Func<string>) (() => build.GetSourceLink(requestContext)));
        }
        if (build.Definition == null)
          return;
        if (build.Definition.Type == Microsoft.TeamFoundation.Build.WebApi.DefinitionType.Build)
        {
          build.Links.TryAddLink("timeline", (ISecuredObject) build, (Func<string>) (() => build.GetTimelineUrl(requestContext)));
          IBuildRouteService routeService = requestContext.GetService<IBuildRouteService>();
          build.Links.TryAddLink("badge", (ISecuredObject) build, (Func<string>) (() => routeService.GetStatusBadgeUrl(requestContext, build.Definition.Project.Id, build.Definition.Id)));
        }
        if (build.Definition.Type != Microsoft.TeamFoundation.Build.WebApi.DefinitionType.Xaml)
          return;
        string informationNodesRestUrl = requestContext.GetService<IXamlBuildRouteService>().GetInformationNodesRestUrl(requestContext, build.Project.Id, build.Id);
        build.Links.TryAddLink("details", (ISecuredObject) build, informationNodesRestUrl);
      }
    }

    internal static string GetTimelineUrl(this Microsoft.TeamFoundation.Build.WebApi.Build build, IVssRequestContext requestContext) => build.Project != null ? requestContext.GetService<IBuildRouteService>().GetTimelineRestUrl(requestContext, build.Project.Id, build.Id) : (string) null;

    internal static string GetSourceLink(this Microsoft.TeamFoundation.Build.WebApi.Build build, IVssRequestContext requestContext) => build.Project != null ? requestContext.GetService<IBuildRouteService>().GetBuildSourcesRestUrl(requestContext, build.Project.Id, build.Id) : (string) null;

    internal static string GetSelfLink(this Microsoft.TeamFoundation.Build.WebApi.Build build, IVssRequestContext requestContext)
    {
      if (build == null)
        return (string) null;
      return !string.IsNullOrEmpty(build.Url) ? build.Url : build.GetRestUrl(requestContext);
    }

    public static IdentityRef GetRequestedForIdentity(
      IVssRequestContext requestContext,
      IReadOnlyBuildData srvBuildData,
      IdentityMap identityMap,
      ISecuredObject securedObject,
      bool resolveExternalIdentities)
    {
      if (!resolveExternalIdentities)
        return identityMap.GetIdentityRef(requestContext, srvBuildData.RequestedFor);
      if (srvBuildData.Reason == Microsoft.TeamFoundation.Build2.Server.BuildReason.Manual)
      {
        IdentityRef identityRef = identityMap.GetIdentityRef(requestContext, srvBuildData.RequestedFor);
        if (!BuildExtensions.IsDisplayableRequestedForIdentity(requestContext, identityRef) && requestContext.IsFeatureEnabled("Build2.DataProvidersCanReturnEmptyIdentities"))
        {
          requestContext.TraceInfo(nameof (BuildExtensions), "Returning empty identity. (manual build)");
          return (IdentityRef) new ExternalIdentityRef(securedObject);
        }
        requestContext.TraceInfo(nameof (BuildExtensions), "Returning identityMap identity for {0}. (manual build)", (object) srvBuildData.RequestedFor);
        return identityRef;
      }
      Microsoft.TeamFoundation.Build2.Server.SourceProviderAttributes attributes;
      if (SourceProviderHelper.TryGetAttributes(requestContext, srvBuildData.Repository?.Type, out attributes) && attributes.IsExternal)
      {
        requestContext.TraceInfo(nameof (BuildExtensions), "Calling TryGetSourceVersionIdentity for external repository type, {0}.", (object) srvBuildData.Repository?.Type);
        ExternalIdentityRef sourceVersionIdentity;
        if (BuildExtensions.TryGetSourceVersionIdentity(requestContext, srvBuildData, securedObject, out sourceVersionIdentity))
        {
          requestContext.TraceInfo(nameof (BuildExtensions), "Returning sourceVersionIdentity for external repository type, {0}", (object) srvBuildData.Repository?.Type);
          return (IdentityRef) sourceVersionIdentity;
        }
      }
      IdentityRef identityRef1 = identityMap.GetIdentityRef(requestContext, srvBuildData.RequestedFor);
      if (!BuildExtensions.IsDisplayableRequestedForIdentity(requestContext, identityRef1))
      {
        requestContext.TraceInfo(nameof (BuildExtensions), "Calling TryGetSourceVersionIdentity to replace undisplayable identity, {0}.", (object) srvBuildData.RequestedFor);
        ExternalIdentityRef sourceVersionIdentity;
        if (BuildExtensions.TryGetSourceVersionIdentity(requestContext, srvBuildData, securedObject, out sourceVersionIdentity))
        {
          requestContext.TraceInfo(nameof (BuildExtensions), "Returning sourceVersionIdentity instead of undisplayable identity.");
          return (IdentityRef) sourceVersionIdentity;
        }
        if (requestContext.IsFeatureEnabled("Build2.DataProvidersCanReturnEmptyIdentities"))
        {
          requestContext.TraceInfo(nameof (BuildExtensions), "Returning empty identity.");
          return (IdentityRef) new ExternalIdentityRef(securedObject);
        }
      }
      requestContext.TraceInfo(nameof (BuildExtensions), "Returning identityMap identity for {0}.", (object) srvBuildData.RequestedFor);
      return identityRef1;
    }

    internal static bool TryUpdateUrlAccessMapping(
      IVssRequestContext requestContext,
      string url,
      out string updatedUrl)
    {
      if (!string.IsNullOrEmpty(url))
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        string locationServiceUrl1 = service.GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker);
        if (!string.IsNullOrEmpty(locationServiceUrl1) && !url.StartsWith(locationServiceUrl1, StringComparison.OrdinalIgnoreCase))
        {
          foreach (string accessMappingMoniker in BuildExtensions.s_accessMappingsToCheck)
          {
            string locationServiceUrl2 = service.GetLocationServiceUrl(requestContext, Guid.Empty, accessMappingMoniker);
            if (!string.IsNullOrEmpty(locationServiceUrl2) && url.StartsWith(locationServiceUrl2, StringComparison.OrdinalIgnoreCase))
            {
              updatedUrl = UriUtility.Combine(locationServiceUrl1, url.Substring(locationServiceUrl2.Length), true).AbsoluteUri;
              return true;
            }
          }
        }
      }
      updatedUrl = (string) null;
      return false;
    }

    private static bool TryGetSourceVersionIdentity(
      IVssRequestContext requestContext,
      IReadOnlyBuildData build,
      ISecuredObject securedObject,
      out ExternalIdentityRef sourceVersionIdentity)
    {
      if (!string.IsNullOrEmpty(build.SourceVersionInfo.AuthorDisplayName))
      {
        ref ExternalIdentityRef local = ref sourceVersionIdentity;
        ExternalIdentityRef externalIdentityRef = new ExternalIdentityRef(securedObject);
        externalIdentityRef.DisplayName = build.SourceVersionInfo.AuthorDisplayName;
        local = externalIdentityRef;
        if (!requestContext.IsFeatureEnabled("Build2.DataProvidersCanReturnEmptyIdentities"))
          sourceVersionIdentity.Id = BuildExtensions.s_externalUserId.ToString();
        string str = build.SourceVersionInfo.AuthorAvatarUrl;
        string updatedUrl;
        if (BuildExtensions.TryUpdateUrlAccessMapping(requestContext, str, out updatedUrl))
          str = updatedUrl;
        sourceVersionIdentity.Links = new ReferenceLinks();
        sourceVersionIdentity.Links.AddLink("avatar", str, securedObject);
        return true;
      }
      IVssRequestContext requestContext1 = requestContext;
      object[] objArray1 = new object[9]
      {
        (object) build.BuildNumber,
        (object) build.ProjectId,
        (object) build.Id,
        (object) build.SourceVersion,
        (object) build.Status,
        (object) build.Result,
        null,
        null,
        null
      };
      DateTime? queueTime = build.QueueTime;
      ref DateTime? local1 = ref queueTime;
      objArray1[6] = (object) (local1.HasValue ? local1.GetValueOrDefault().ToString("d", (IFormatProvider) CultureInfo.InvariantCulture) : (string) null);
      objArray1[7] = (object) build.Reason;
      objArray1[8] = (object) build.Repository?.Type;
      string format = string.Format("No persisted author for {0} (ProjectId: {1}, BuildId: {2}, SourceVersion: {3}, Status: {4}, Result: {5}, QueueDate: {6}, Reason: {7}, RepoType: {8}). Older build?", objArray1);
      object[] objArray2 = Array.Empty<object>();
      requestContext1.TraceAlways(0, TraceLevel.Warning, "SourceVersionInfo", nameof (BuildExtensions), format, objArray2);
      string str1;
      bool result;
      string str2;
      string str3;
      if (((!build.TriggerInfo.TryGetValue("pr.sender.isExternal", out str1) ? 0 : (bool.TryParse(str1, out result) ? 1 : 0)) & (result ? 1 : 0)) != 0 && build.TriggerInfo.TryGetValue("pr.sender.name", out str2) && build.TriggerInfo.TryGetValue("pr.sender.avatarUrl", out str3))
      {
        string updatedUrl;
        if (BuildExtensions.TryUpdateUrlAccessMapping(requestContext, str3, out updatedUrl))
          str3 = updatedUrl;
        ref ExternalIdentityRef local2 = ref sourceVersionIdentity;
        ExternalIdentityRef externalIdentityRef = new ExternalIdentityRef(securedObject);
        externalIdentityRef.Id = BuildExtensions.s_externalUserId.ToString();
        externalIdentityRef.DisplayName = str2;
        externalIdentityRef.ImageUrl = str3;
        local2 = externalIdentityRef;
        sourceVersionIdentity.Links = new ReferenceLinks();
        sourceVersionIdentity.Links.AddLink("avatar", str3, securedObject);
        return true;
      }
      sourceVersionIdentity = (ExternalIdentityRef) null;
      return false;
    }

    private static bool IsDisplayableRequestedForIdentity(
      IVssRequestContext requestContext,
      IdentityRef identity)
    {
      bool flag = identity != null && !string.Equals(identity.Descriptor.SubjectType, "s2s", StringComparison.Ordinal) && !string.Equals(identity.Descriptor.SubjectType, "svc", StringComparison.Ordinal);
      requestContext.TraceInfo(nameof (BuildExtensions), "Identity descriptor: {0}. IsDisplayable: {1}", (object) identity?.Descriptor, (object) flag);
      return flag;
    }

    private static string GetBuildResultText(Microsoft.TeamFoundation.Build.WebApi.BuildResult? result)
    {
      switch (result.GetValueOrDefault())
      {
        case Microsoft.TeamFoundation.Build.WebApi.BuildResult.Succeeded:
          return Resources.BuildResultTextSucceeded();
        case Microsoft.TeamFoundation.Build.WebApi.BuildResult.PartiallySucceeded:
          return Resources.BuildResultTextPartiallySucceeded();
        case Microsoft.TeamFoundation.Build.WebApi.BuildResult.Failed:
          return Resources.BuildResultTextFailed();
        case Microsoft.TeamFoundation.Build.WebApi.BuildResult.Canceled:
          return Resources.BuildResultTextCanceled();
        default:
          return Resources.BuildResultTextUnknown();
      }
    }
  }
}
