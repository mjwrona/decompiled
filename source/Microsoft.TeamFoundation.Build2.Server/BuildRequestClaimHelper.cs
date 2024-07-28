// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildRequestClaimHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.PipelineCache.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildRequestClaimHelper
  {
    private const string MasterBranch = "refs/heads/master";
    private const string MainBranch = "refs/heads/main";

    internal static (string Key, string Value) GetPipelineCacheClaim(
      IVssRequestContext requestContext,
      IOrchestrationProcess orchestration,
      BuildData build,
      PipelineEnvironment environment,
      bool isForkBuild)
    {
      IEnumerable<TaskStep> taskSteps = TaskStepHelper.GetTaskSteps(requestContext, orchestration, (Func<Guid, bool>) (taskId => taskId == PipelineCacheConstants.PipelineCacheTaskId), nameof (GetPipelineCacheClaim));
      if (TaskHub.IsPipelineCachingEnabled(requestContext, taskSteps.Select<TaskStep, Guid>((Func<TaskStep, Guid>) (t => t.Reference.Id)), environment.UserVariables))
      {
        List<CacheScopePermission> cacheScopePermissionList = new List<CacheScopePermission>();
        if (build.Reason == BuildReason.PullRequest)
        {
          if (!isForkBuild)
          {
            string pipelineCacheToken = BuildRequestClaimHelper.GetPipelineCacheToken(build, BuildRequestClaimHelper.PipelineCacheTokenType.SourceBranch);
            cacheScopePermissionList.Add(new CacheScopePermission(pipelineCacheToken, SecurityDefintions.Permissions.Read));
          }
          string pipelineCacheToken1 = BuildRequestClaimHelper.GetPipelineCacheToken(build, BuildRequestClaimHelper.PipelineCacheTokenType.TargetBranch);
          cacheScopePermissionList.Add(new CacheScopePermission(pipelineCacheToken1, SecurityDefintions.Permissions.Read));
          string pipelineCacheToken2 = BuildRequestClaimHelper.GetPipelineCacheToken(build, BuildRequestClaimHelper.PipelineCacheTokenType.TargetBranchName);
          cacheScopePermissionList.Add(new CacheScopePermission(pipelineCacheToken2, SecurityDefintions.Permissions.Read));
        }
        string pipelineCacheToken3 = BuildRequestClaimHelper.GetPipelineCacheToken(build, BuildRequestClaimHelper.PipelineCacheTokenType.IntermediateBranch);
        cacheScopePermissionList.Add(new CacheScopePermission(pipelineCacheToken3, SecurityDefintions.Permissions.All));
        string[] strArray = new string[2]
        {
          BuildRequestClaimHelper.GetPipelineCacheToken(build, BuildRequestClaimHelper.PipelineCacheTokenType.MasterBranch),
          BuildRequestClaimHelper.GetPipelineCacheToken(build, BuildRequestClaimHelper.PipelineCacheTokenType.MainBranch)
        };
        foreach (string str in strArray)
        {
          string branchToken = str;
          if (!cacheScopePermissionList.Any<CacheScopePermission>((Func<CacheScopePermission, bool>) (pc => pc.Scope == branchToken)))
            cacheScopePermissionList.Add(new CacheScopePermission(branchToken, SecurityDefintions.Permissions.Read));
        }
        if (cacheScopePermissionList.Any<CacheScopePermission>())
          return ("pcc", JsonSerializer.Serialize<List<CacheScopePermission>>(cacheScopePermissionList));
      }
      return ("pcc", (string) null);
    }

    internal static string GetPipelineCacheToken(
      BuildData build,
      BuildRequestClaimHelper.PipelineCacheTokenType type)
    {
      Dictionary<string, string> buildParameterValues = BuildRequestClaimHelper.GetBuildParameterValues(build.Parameters);
      string sourceBranch;
      switch (type)
      {
        case BuildRequestClaimHelper.PipelineCacheTokenType.SourceBranch:
          buildParameterValues.TryGetValue("system.pullRequest.sourceBranch", out sourceBranch);
          break;
        case BuildRequestClaimHelper.PipelineCacheTokenType.TargetBranch:
          buildParameterValues.TryGetValue("system.pullRequest.targetBranch", out sourceBranch);
          break;
        case BuildRequestClaimHelper.PipelineCacheTokenType.TargetBranchName:
          buildParameterValues.TryGetValue("system.pullRequest.targetBranchName", out sourceBranch);
          break;
        case BuildRequestClaimHelper.PipelineCacheTokenType.IntermediateBranch:
          sourceBranch = build.SourceBranch;
          break;
        case BuildRequestClaimHelper.PipelineCacheTokenType.MasterBranch:
          sourceBranch = "refs/heads/master";
          break;
        case BuildRequestClaimHelper.PipelineCacheTokenType.MainBranch:
          sourceBranch = "refs/heads/main";
          break;
        default:
          throw new InvalidOperationException("Unreachable code");
      }
      string str = GitRefspecHelper.NormalizeSourceBranch(sourceBranch);
      return new CacheScope()
      {
        BuildDefinitionId = build.Definition.Id,
        ProjectId = build.Definition.ProjectId,
        Branch = str,
        RepositoryId = build.Repository.Id
      }.Serialize();
    }

    internal static Dictionary<string, string> GetBuildParameterValues(string parameter) => !string.IsNullOrEmpty(parameter) ? JObject.Parse(parameter).Children().OfType<JProperty>().ToDictionary<JProperty, string, string>((Func<JProperty, string>) (p => p.Name), (Func<JProperty, string>) (p => p.Value.ToString())) : new Dictionary<string, string>();

    internal enum PipelineCacheTokenType
    {
      SourceBranch,
      TargetBranch,
      TargetBranchName,
      IntermediateBranch,
      MasterBranch,
      MainBranch,
    }
  }
}
