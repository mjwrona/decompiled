// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildTriggerExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildTriggerExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildTrigger ToWebApiBuildTrigger(
      this Microsoft.TeamFoundation.Build2.Server.BuildTrigger srvBuildTrigger,
      ISecuredObject securedObject,
      Microsoft.TeamFoundation.Build2.Server.PipelineTriggerSettings effectiveSettings = null)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildTrigger == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildTrigger) null;
      switch (srvBuildTrigger.TriggerType)
      {
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.ContinuousIntegration:
          return (Microsoft.TeamFoundation.Build.WebApi.BuildTrigger) (srvBuildTrigger as Microsoft.TeamFoundation.Build2.Server.ContinuousIntegrationTrigger).ToWebApiContinuousIntegrationTrigger(securedObject);
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.Schedule:
          return (Microsoft.TeamFoundation.Build.WebApi.BuildTrigger) (srvBuildTrigger as Microsoft.TeamFoundation.Build2.Server.ScheduleTrigger).ToWebApiScheduleTrigger(securedObject);
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.GatedCheckIn:
          return (Microsoft.TeamFoundation.Build.WebApi.BuildTrigger) (srvBuildTrigger as Microsoft.TeamFoundation.Build2.Server.GatedCheckInTrigger).ToWebApiGatedCheckInTrigger(securedObject);
        case Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType.PullRequest:
          return (Microsoft.TeamFoundation.Build.WebApi.BuildTrigger) (srvBuildTrigger as Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger).ToWebApiPRTrigger(securedObject, effectiveSettings);
        default:
          return (Microsoft.TeamFoundation.Build.WebApi.BuildTrigger) null;
      }
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildTrigger ToServerBuildTrigger(
      this Microsoft.TeamFoundation.Build.WebApi.BuildTrigger webApiBuildTrigger)
    {
      if (webApiBuildTrigger == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildTrigger) null;
      switch (webApiBuildTrigger.TriggerType)
      {
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.ContinuousIntegration:
          return (Microsoft.TeamFoundation.Build2.Server.BuildTrigger) (webApiBuildTrigger as Microsoft.TeamFoundation.Build.WebApi.ContinuousIntegrationTrigger).ToServerContinuousTriggerType();
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.Schedule:
          return (Microsoft.TeamFoundation.Build2.Server.BuildTrigger) (webApiBuildTrigger as Microsoft.TeamFoundation.Build.WebApi.ScheduleTrigger).ToServerScheduleTrigger();
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.GatedCheckIn:
          return (Microsoft.TeamFoundation.Build2.Server.BuildTrigger) (webApiBuildTrigger as Microsoft.TeamFoundation.Build.WebApi.GatedCheckInTrigger).ToServerGatedCheckInTrigger();
        case Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.PullRequest:
          return (Microsoft.TeamFoundation.Build2.Server.BuildTrigger) (webApiBuildTrigger as Microsoft.TeamFoundation.Build.WebApi.PullRequestTrigger).ToServerPRTrigger();
        default:
          return (Microsoft.TeamFoundation.Build2.Server.BuildTrigger) null;
      }
    }

    public static Microsoft.TeamFoundation.Build.WebApi.ContinuousIntegrationTrigger ToWebApiContinuousIntegrationTrigger(
      this Microsoft.TeamFoundation.Build2.Server.ContinuousIntegrationTrigger srvCiTrigger,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvCiTrigger == null)
        return (Microsoft.TeamFoundation.Build.WebApi.ContinuousIntegrationTrigger) null;
      return new Microsoft.TeamFoundation.Build.WebApi.ContinuousIntegrationTrigger(securedObject)
      {
        SettingsSourceType = srvCiTrigger.SettingsSourceType,
        BatchChanges = srvCiTrigger.BatchChanges,
        MaxConcurrentBuildsPerBranch = srvCiTrigger.MaxConcurrentBuildsPerBranch > 1 ? srvCiTrigger.MaxConcurrentBuildsPerBranch : 1,
        BranchFilters = srvCiTrigger.BranchFilters,
        PathFilters = srvCiTrigger.PathFilters,
        PollingInterval = srvCiTrigger.PollingInterval,
        PollingJobId = srvCiTrigger.PollingJobId
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.ContinuousIntegrationTrigger ToServerContinuousTriggerType(
      this Microsoft.TeamFoundation.Build.WebApi.ContinuousIntegrationTrigger webApiCiTrigger)
    {
      if (webApiCiTrigger == null)
        return (Microsoft.TeamFoundation.Build2.Server.ContinuousIntegrationTrigger) null;
      Microsoft.TeamFoundation.Build2.Server.ContinuousIntegrationTrigger continuousTriggerType = new Microsoft.TeamFoundation.Build2.Server.ContinuousIntegrationTrigger();
      continuousTriggerType.SettingsSourceType = webApiCiTrigger.SettingsSourceType;
      continuousTriggerType.BatchChanges = webApiCiTrigger.BatchChanges;
      continuousTriggerType.MaxConcurrentBuildsPerBranch = webApiCiTrigger.MaxConcurrentBuildsPerBranch > 1 ? webApiCiTrigger.MaxConcurrentBuildsPerBranch : 1;
      continuousTriggerType.BranchFilters = webApiCiTrigger.BranchFilters;
      continuousTriggerType.PathFilters = webApiCiTrigger.PathFilters;
      continuousTriggerType.PollingInterval = webApiCiTrigger.PollingInterval;
      continuousTriggerType.PollingJobId = webApiCiTrigger.PollingJobId;
      return continuousTriggerType;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.GatedCheckInTrigger ToWebApiGatedCheckInTrigger(
      this Microsoft.TeamFoundation.Build2.Server.GatedCheckInTrigger srvGatedTrigger,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvGatedTrigger == null)
        return (Microsoft.TeamFoundation.Build.WebApi.GatedCheckInTrigger) null;
      return new Microsoft.TeamFoundation.Build.WebApi.GatedCheckInTrigger(securedObject)
      {
        RunContinuousIntegration = srvGatedTrigger.RunContinuousIntegration,
        UseWorkspaceMappings = srvGatedTrigger.UseWorkspaceMappings,
        PathFilters = srvGatedTrigger.PathFilters
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.GatedCheckInTrigger ToServerGatedCheckInTrigger(
      this Microsoft.TeamFoundation.Build.WebApi.GatedCheckInTrigger webApiGatedTrigger)
    {
      if (webApiGatedTrigger == null)
        return (Microsoft.TeamFoundation.Build2.Server.GatedCheckInTrigger) null;
      return new Microsoft.TeamFoundation.Build2.Server.GatedCheckInTrigger()
      {
        RunContinuousIntegration = webApiGatedTrigger.RunContinuousIntegration,
        UseWorkspaceMappings = webApiGatedTrigger.UseWorkspaceMappings,
        PathFilters = webApiGatedTrigger.PathFilters
      };
    }

    public static Microsoft.TeamFoundation.Build.WebApi.ScheduleTrigger ToWebApiScheduleTrigger(
      this Microsoft.TeamFoundation.Build2.Server.ScheduleTrigger srvScheduleTrigger,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvScheduleTrigger == null)
        return (Microsoft.TeamFoundation.Build.WebApi.ScheduleTrigger) null;
      Microsoft.TeamFoundation.Build.WebApi.ScheduleTrigger apiScheduleTrigger = new Microsoft.TeamFoundation.Build.WebApi.ScheduleTrigger(securedObject);
      if (srvScheduleTrigger.Schedules != null)
        apiScheduleTrigger.Schedules = srvScheduleTrigger.Schedules.Select<Microsoft.TeamFoundation.Build2.Server.Schedule, Microsoft.TeamFoundation.Build.WebApi.Schedule>((Func<Microsoft.TeamFoundation.Build2.Server.Schedule, Microsoft.TeamFoundation.Build.WebApi.Schedule>) (x => x.ToWebApiSchedule(securedObject))).ToList<Microsoft.TeamFoundation.Build.WebApi.Schedule>();
      return apiScheduleTrigger;
    }

    public static Microsoft.TeamFoundation.Build2.Server.ScheduleTrigger ToServerScheduleTrigger(
      this Microsoft.TeamFoundation.Build.WebApi.ScheduleTrigger webApiScheduleTrigger)
    {
      if (webApiScheduleTrigger == null)
        return (Microsoft.TeamFoundation.Build2.Server.ScheduleTrigger) null;
      Microsoft.TeamFoundation.Build2.Server.ScheduleTrigger serverScheduleTrigger = new Microsoft.TeamFoundation.Build2.Server.ScheduleTrigger();
      if (webApiScheduleTrigger.Schedules != null)
        serverScheduleTrigger.Schedules = webApiScheduleTrigger.Schedules.Select<Microsoft.TeamFoundation.Build.WebApi.Schedule, Microsoft.TeamFoundation.Build2.Server.Schedule>((Func<Microsoft.TeamFoundation.Build.WebApi.Schedule, Microsoft.TeamFoundation.Build2.Server.Schedule>) (x => x.ToServerSchedule())).ToList<Microsoft.TeamFoundation.Build2.Server.Schedule>();
      return serverScheduleTrigger;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.PullRequestTrigger ToWebApiPRTrigger(
      this Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger srvPRTrigger,
      ISecuredObject securedObject,
      Microsoft.TeamFoundation.Build2.Server.PipelineTriggerSettings effectiveSettings = null)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvPRTrigger == null)
        return (Microsoft.TeamFoundation.Build.WebApi.PullRequestTrigger) null;
      Microsoft.TeamFoundation.Build.WebApi.PullRequestTrigger webApiPrTrigger = new Microsoft.TeamFoundation.Build.WebApi.PullRequestTrigger(securedObject)
      {
        SettingsSourceType = srvPRTrigger.SettingsSourceType,
        BranchFilters = srvPRTrigger.BranchFilters,
        PathFilters = srvPRTrigger.PathFilters,
        Forks = srvPRTrigger.Forks.ToWebApiForks(securedObject),
        IsCommentRequiredForPullRequest = srvPRTrigger.IsCommentRequiredForPullRequest,
        RequireCommentsForNonTeamMembersOnly = srvPRTrigger.RequireCommentsForNonTeamMembersOnly,
        RequireCommentsForNonTeamMemberAndNonContributors = srvPRTrigger.RequireCommentsForNonTeamMemberAndNonContributors
      };
      if (effectiveSettings != null)
        webApiPrTrigger.PipelineTriggerSettings = new Microsoft.TeamFoundation.Build.WebApi.Contracts.PipelineTriggerSettings(securedObject)
        {
          ForkProtectionEnabled = effectiveSettings.ForkProtectionEnabled,
          BuildsEnabledForForks = effectiveSettings.BuildsEnabledForForks,
          EnforceJobAuthScopeForForks = effectiveSettings.EnforceJobAuthScopeForForks,
          EnforceNoAccessToSecretsFromForks = effectiveSettings.EnforceNoAccessToSecretsFromForks,
          IsCommentRequiredForPullRequest = effectiveSettings.IsCommentRequiredForPullRequest,
          RequireCommentsForNonTeamMemberAndNonContributors = effectiveSettings.RequireCommentsForNonTeamMemberAndNonContributors,
          RequireCommentsForNonTeamMembersOnly = effectiveSettings.RequireCommentsForNonTeamMembersOnly
        };
      return webApiPrTrigger;
    }

    public static Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger ToServerPRTrigger(
      this Microsoft.TeamFoundation.Build.WebApi.PullRequestTrigger webApiPRTrigger)
    {
      if (webApiPRTrigger == null)
        return (Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger) null;
      Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger serverPrTrigger = new Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger();
      serverPrTrigger.SettingsSourceType = webApiPRTrigger.SettingsSourceType;
      serverPrTrigger.BranchFilters = webApiPRTrigger.BranchFilters;
      serverPrTrigger.PathFilters = webApiPRTrigger.PathFilters;
      serverPrTrigger.Forks = webApiPRTrigger.Forks.ToServerForks();
      serverPrTrigger.IsCommentRequiredForPullRequest = webApiPRTrigger.IsCommentRequiredForPullRequest;
      serverPrTrigger.RequireCommentsForNonTeamMembersOnly = webApiPRTrigger.RequireCommentsForNonTeamMembersOnly;
      serverPrTrigger.RequireCommentsForNonTeamMemberAndNonContributors = webApiPRTrigger.RequireCommentsForNonTeamMemberAndNonContributors;
      return serverPrTrigger;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger ToWebApiBCTrigger(
      this Microsoft.TeamFoundation.Build2.Server.BuildCompletionTrigger srvBCTrigger,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBCTrigger == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger) null;
      IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
      Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger webApiBcTrigger = new Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger(securedObject);
      webApiBcTrigger.Definition = new DefinitionReference()
      {
        Id = srvBCTrigger.DefinitionId,
        Project = requestContext.GetTeamProjectReference(srvBCTrigger.ProjectId),
        Path = srvBCTrigger.Path,
        Url = service.GetDefinitionRestUrl(requestContext, srvBCTrigger.ProjectId, srvBCTrigger.DefinitionId)
      };
      webApiBcTrigger.RequiresSuccessfulBuild = srvBCTrigger.RequiresSuccessfulBuild;
      webApiBcTrigger.BranchFilters = srvBCTrigger.BranchFilters;
      webApiBcTrigger.Definition.SetNestingSecurityToken(securedObject.GetToken());
      return webApiBcTrigger;
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildCompletionTrigger ToServerBCTrigger(
      this Microsoft.TeamFoundation.Build.WebApi.BuildCompletionTrigger webApiBCTrigger)
    {
      if (webApiBCTrigger == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildCompletionTrigger) null;
      if (webApiBCTrigger.Definition == null || webApiBCTrigger.Definition.Project == null)
        throw new InvalidDefinitionQueryException(Resources.IllformedBuildCompletionTrigger());
      return new Microsoft.TeamFoundation.Build2.Server.BuildCompletionTrigger()
      {
        DefinitionId = webApiBCTrigger.Definition.Id,
        ProjectId = webApiBCTrigger.Definition.Project.Id,
        Path = webApiBCTrigger.Definition.Path,
        RequiresSuccessfulBuild = webApiBCTrigger.RequiresSuccessfulBuild,
        BranchFilters = webApiBCTrigger.BranchFilters
      };
    }
  }
}
