// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ExternalGitEventExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class ExternalGitEventExtensions
  {
    private static readonly Dictionary<string, string> m_typesMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "GitHub",
        "GitHub"
      },
      {
        "GitHubEnterprise",
        "GitHubEnterprise"
      },
      {
        "Bitbucket",
        "Bitbucket"
      }
    };
    private const string c_area = "Build2";
    private const string c_layer = "ExternalPullRequestExtensions";

    public static RepositoryUpdateInfo GetRepositoryUpdateInfo(
      this ExternalGitPush push,
      IVssRequestContext requestContext,
      string repositoryType)
    {
      return new RepositoryUpdateInfo()
      {
        RepositoryId = push.Repo.Id,
        RepositoryType = repositoryType,
        UpdateId = long.Parse(push.Id),
        RefUpdates = new List<RefUpdateInfo>()
        {
          push.ToRefUpdateInfo()
        },
        IncludedChanges = push.Commits.Select<ExternalGitCommit, Change>((Func<ExternalGitCommit, Change>) (c => new Change()
        {
          Id = c.Sha,
          Message = c.Message,
          Distinct = c.AdditionalProperties != null ? c.AdditionalProperties.GetCastedValueOrDefault<string, bool?>("Distinct") : new bool?()
        })).ToList<Change>()
      };
    }

    public static RepositoryUpdateInfo GetRepositoryUpdateInfo(
      this ExternalGitPullRequest pullRequest,
      string repositoryType)
    {
      return new RepositoryUpdateInfo()
      {
        RepositoryId = pullRequest.Repo.Id,
        RepositoryType = repositoryType,
        UpdateId = long.Parse(pullRequest.Id),
        RefUpdates = new List<RefUpdateInfo>()
        {
          pullRequest.ToRefUpdateInfo()
        },
        IncludedChanges = new List<Change>()
      };
    }

    public static RefUpdateInfo ToRefUpdateInfo(this ExternalGitPush push) => new RefUpdateInfo()
    {
      RefName = GitRefspecHelper.NormalizeSourceBranch(push.GitRef),
      OldObjectId = push.BeforeSha,
      NewObjectId = push.AfterSha,
      MergeRef = string.Empty,
      MergeObjectId = "0000000000000000000000000000000000000000"
    };

    public static RefUpdateInfo ToRefUpdateInfo(this ExternalGitPullRequest pullRequest) => new RefUpdateInfo()
    {
      RefName = pullRequest.TargetRef,
      OldObjectId = pullRequest.TargetSha,
      NewObjectId = pullRequest.SourceSha,
      MergeRef = pullRequest.MergeRef,
      MergeObjectId = pullRequest.MergeCommitSha
    };

    public static void GetTriggerSourceInfo(
      this IReadOnlyBuildData build,
      out string sourceBranch,
      out string sourceVersion)
    {
      if (!build.TriggerInfo.TryGetValue("pr.sourceBranch", out sourceBranch))
        sourceBranch = build.SourceBranch;
      if (build.TriggerInfo.TryGetValue("pr.sourceSha", out sourceVersion))
        return;
      sourceVersion = build.SourceVersion;
    }

    public static void TryDeleteOldBuilds(
      this IVssRequestContext requestContext,
      Guid projectId,
      List<BuildDefinition> definitions,
      string mergeRef)
    {
      IBuildService service = requestContext.GetService<IBuildService>();
      try
      {
        IBuildService buildService = service;
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        IEnumerable<int> definitionIds = definitions.Select<BuildDefinition, int>((Func<BuildDefinition, int>) (d => d.Id));
        BuildReason? nullable1 = new BuildReason?(BuildReason.PullRequest);
        BuildStatus? nullable2 = new BuildStatus?(BuildStatus.InProgress | BuildStatus.Postponed | BuildStatus.NotStarted);
        DateTime? minFinishTime = new DateTime?();
        DateTime? maxFinishTime = new DateTime?();
        BuildReason? reasonFilter = nullable1;
        BuildStatus? statusFilter = nullable2;
        BuildResult? resultFilter = new BuildResult?();
        int? maxBuildsPerDefinition = new int?();
        List<int> list = buildService.GetBuildsLegacy(requestContext1, projectId1, int.MaxValue, definitionIds, minFinishTime: minFinishTime, maxFinishTime: maxFinishTime, reasonFilter: reasonFilter, statusFilter: statusFilter, resultFilter: resultFilter, maxBuildsPerDefinition: maxBuildsPerDefinition).ToList<BuildData>().Where<BuildData>((Func<BuildData, bool>) (b => string.Equals(b.SourceBranch, mergeRef, StringComparison.Ordinal) && ExternalGitEventExtensions.IsAutoCancel(requestContext, b))).Select<BuildData, int>((Func<BuildData, int>) (b => b.Id)).ToList<int>();
        requestContext.TraceVerbose("ExternalPullRequestExtensions", "Found {0} old pull request builds to delete", (object) list.Count);
        service.DeleteBuilds(requestContext, projectId, (IEnumerable<int>) list);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12030184, "ExternalPullRequestExtensions", ex);
      }
    }

    public static IDictionary<string, string> GetBuildParameterMap(
      this ExternalGitPullRequest pullRequest)
    {
      Dictionary<string, string> buildParameterMap = new Dictionary<string, string>()
      {
        ["system.pullRequest.pullRequestId"] = pullRequest.Id,
        ["system.pullRequest.pullRequestNumber"] = pullRequest.Number,
        ["system.pullRequest.mergedAt"] = pullRequest.MergedAt,
        ["system.pullRequest.sourceBranch"] = pullRequest.SourceRef,
        ["system.pullRequest.targetBranch"] = pullRequest.TargetRef,
        ["system.pullRequest.targetBranchName"] = pullRequest.TargetRef.Replace("refs/heads/", string.Empty),
        ["system.pullRequest.sourceRepositoryUri"] = pullRequest.Repo.Url,
        ["system.pullRequest.sourceCommitId"] = pullRequest.SourceSha,
        ["system.pullRequest.isFork"] = pullRequest.IsFork.ToString()
      };
      if (!string.IsNullOrEmpty(pullRequest.MergedAt))
        buildParameterMap["system.pullRequest.mergedAt"] = pullRequest.MergedAt;
      return (IDictionary<string, string>) buildParameterMap;
    }

    public static string GetBuildParameters(this ExternalGitPullRequest pullRequest) => pullRequest.GetBuildParameterMap().Serialize<IDictionary<string, string>>();

    public static Dictionary<string, string> GetPullRequestTriggerInfo(
      this ExternalGitPullRequest pullRequest,
      string providerId,
      bool isPusherExternal)
    {
      Dictionary<string, string> requestTriggerInfo = pullRequest.GetPullRequestTriggerInfo(isPusherExternal);
      requestTriggerInfo["pr.providerId"] = providerId;
      return requestTriggerInfo;
    }

    public static Dictionary<string, string> GetPullRequestTriggerInfo(
      this ExternalGitPullRequest pullRequest,
      bool isExternalUser)
    {
      return new Dictionary<string, string>()
      {
        ["pr.sourceBranch"] = pullRequest.SourceRef,
        ["pr.sourceSha"] = pullRequest.SourceSha,
        ["pr.id"] = pullRequest.Id,
        ["pr.title"] = StringUtil.Truncate(pullRequest.Title, 300, true),
        ["pr.number"] = pullRequest.Number,
        ["pr.isFork"] = pullRequest.IsFork.ToString(),
        ["pr.draft"] = pullRequest.Draft.ToString(),
        ["pr.sender.name"] = pullRequest.Sender?.Name,
        ["pr.sender.avatarUrl"] = pullRequest.Sender?.AvatarUrl,
        ["pr.sender.isExternal"] = isExternalUser.ToString()
      };
    }

    public static Dictionary<string, string> GetCITriggerInfo(this ExternalGitPush push)
    {
      Dictionary<string, string> ciTriggerInfo = new Dictionary<string, string>();
      ciTriggerInfo["ci.sourceBranch"] = push.GitRef;
      ciTriggerInfo["ci.sourceSha"] = push.AfterSha;
      IList<ExternalGitCommit> commits = push.Commits;
      ciTriggerInfo["ci.message"] = commits != null ? commits.FirstOrDefault<ExternalGitCommit>()?.Message : (string) null;
      return ciTriggerInfo;
    }

    public static bool TryGetRepositoryType(
      this IDictionary<string, string> eventProperties,
      IVssRequestContext requestContext,
      Guid projectGuid,
      out string repositoryType,
      out bool queueUpdateJob)
    {
      repositoryType = (string) null;
      string str = (string) null;
      queueUpdateJob = false;
      string enumerable;
      if (!eventProperties.TryGetValue("connectedServicesList", out enumerable) && !eventProperties.TryGetValue("connectedServiceId", out str))
      {
        requestContext.TraceError(nameof (ExternalGitEventExtensions), "No service endpoint ID or list associated with the external Git event");
        return false;
      }
      if (!enumerable.IsNullOrEmpty<char>())
      {
        List<Guid> serviceEndpointList = JsonConvert.DeserializeObject<List<Guid>>(enumerable);
        return ExternalGitEventExtensions.TryGetServiceEndpoint(requestContext, projectGuid, serviceEndpointList, out repositoryType, out queueUpdateJob);
      }
      if (!str.IsNullOrEmpty<char>())
      {
        queueUpdateJob = true;
        IVssRequestContext requestContext1 = requestContext;
        Guid projectGuid1 = projectGuid;
        List<Guid> serviceEndpointList = new List<Guid>();
        serviceEndpointList.Add(new Guid(str));
        ref string local1 = ref repositoryType;
        bool flag;
        ref bool local2 = ref flag;
        return ExternalGitEventExtensions.TryGetServiceEndpoint(requestContext1, projectGuid1, serviceEndpointList, out local1, out local2);
      }
      requestContext.TraceError(nameof (ExternalGitEventExtensions), "No service endpoint was found associated with the external Git event");
      return false;
    }

    private static bool TryGetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid projectGuid,
      List<Guid> serviceEndpointList,
      out string repositoryType,
      out bool queueUpdateJob)
    {
      repositoryType = (string) null;
      queueUpdateJob = false;
      foreach (Guid serviceEndpoint1 in serviceEndpointList)
      {
        ServiceEndpoint serviceEndpoint2 = requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, projectGuid, serviceEndpoint1);
        if (serviceEndpoint2 == null)
        {
          requestContext.TraceError(nameof (ExternalGitEventExtensions), "No service endpoint associated with the service endpoint ID {0}", (object) serviceEndpoint1.ToString());
          queueUpdateJob = true;
        }
        else
        {
          if (ExternalGitEventExtensions.m_typesMap.TryGetValue(serviceEndpoint2.Type, out repositoryType))
            return true;
          requestContext.TraceError(nameof (ExternalGitEventExtensions), "External Git event had unrecognized repository type {0}", (object) repositoryType);
        }
      }
      return false;
    }

    public static bool AllowsWebhooks(
      this BuildDefinition definition,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<BuildRepository>(definition?.Repository, "definition and repository");
      Guid serviceEndpointId;
      return definition.Repository.TryGetServiceEndpointId(out serviceEndpointId) && !(serviceEndpointId == Guid.Empty) && requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, definition.ProjectId, serviceEndpointId).AllowsWebhooks(requestContext);
    }

    public static List<BuildDefinition> GetDefinitions(
      this ExternalGitPullRequest notification,
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType)
    {
      return ExternalGitEventExtensions.GetDefinitions(requestContext, notification.Properties, projectId, repositoryType, notification.Repo.Id, DefinitionTriggerType.PullRequest);
    }

    public static List<BuildDefinition> GetDefinitions(
      this ExternalGitPush notification,
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType)
    {
      return ExternalGitEventExtensions.GetDefinitions(requestContext, notification.Properties, projectId, repositoryType, notification.Repo.Id, DefinitionTriggerType.ContinuousIntegration);
    }

    private static List<BuildDefinition> GetDefinitions(
      IVssRequestContext requestContext,
      IDictionary<string, string> eventProperties,
      Guid projectId,
      string repositoryType,
      string repositoryId,
      DefinitionTriggerType triggerType)
    {
      using (requestContext.TraceScope("ExternalPullRequestExtensions", nameof (GetDefinitions)))
      {
        List<BuildDefinition> definitions = new List<BuildDefinition>();
        BuildDefinitionService service = requestContext.GetService<BuildDefinitionService>();
        IVssRequestContext requestContext1 = requestContext;
        List<Guid> projectIds = new List<Guid>();
        projectIds.Add(projectId);
        string repositoryType1 = repositoryType;
        string repositoryId1 = repositoryId;
        int triggerFilter = (int) triggerType;
        IEnumerable<BuildDefinition> collection = service.GetDefinitionsWithTriggers(requestContext1, projectIds, repositoryType1, repositoryId1, (DefinitionTriggerType) triggerFilter, 10000).Where<BuildDefinition>((Func<BuildDefinition, bool>) (d =>
        {
          if (d.QueueStatus != DefinitionQueueStatus.Disabled)
          {
            DefinitionQuality? definitionQuality1 = d.DefinitionQuality;
            DefinitionQuality definitionQuality2 = DefinitionQuality.Definition;
            if (definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue && d.AllowsWebhooks(requestContext))
              return !d.IsTooNewForTriggers(requestContext);
          }
          return false;
        }));
        definitions.AddRange(collection);
        if (definitions.Count > 10)
          requestContext.TraceAlways(12030343, TraceLevel.Info, "Build2", "ExternalPullRequestExtensions", string.Format("GetDefinitionWithTriggers returned {0} candidates. triggerType={1}. repositoryId={2}.", (object) definitions.Count, (object) triggerType, (object) repositoryId));
        return definitions;
      }
    }

    private static bool IsAutoCancel(IVssRequestContext requestContext, BuildData buildData)
    {
      string str;
      bool result;
      if (buildData.TriggerInfo == null || !buildData.TriggerInfo.TryGetValue("pr.autoCancel", out str) || !bool.TryParse(str, out result))
        return true;
      if (!result)
        requestContext.TraceInfo("ExternalPullRequestExtensions", string.Format("PR Build {0} is set to not be auto cancelled", (object) buildData.Id));
      return result;
    }
  }
}
