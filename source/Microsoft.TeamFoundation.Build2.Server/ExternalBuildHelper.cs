// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ExternalBuildHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class ExternalBuildHelper
  {
    public const string UpdateSubscriptionsServiceConnectionsJobName = "UpdateSubscriptionsServiceConnectionsJob";
    public const string GitHubEnterpriseUpdateSubscriptionsServiceConnectionsJobFullName = "Microsoft.TeamFoundation.Build2.Server.Extensions.GitHubEnterpriseUpdateSubscriptionsServiceConnectionsJob";
    public const string GitHubUpdateSubscriptionsServiceConnectionsJobFullName = "Microsoft.TeamFoundation.Build2.Server.Extensions.GitHubUpdateSubscriptionsServiceConnectionsJob";
    public const string BitbucketUpdateSubscriptionsServiceConnectionsJobFullName = "Microsoft.TeamFoundation.Build2.Server.Extensions.BitbucketUpdateSubscriptionsServiceConnectionsJob";

    public static IEnumerable<BuildData> QueueBuilds(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      BuildDefinition definition,
      BuildReason reason,
      IEnumerable<BuildDefinitionBranch> branches,
      out IEnumerable<BuildDefinitionBranch> failedBranches,
      IDictionary<string, string> variables = null,
      IDictionary<string, string> triggerInfo = null,
      BuildRequestValidationOptions validationOptions = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProjectInfo>(projectInfo, nameof (projectInfo));
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) branches, nameof (branches));
      List<BuildDefinitionBranch> definitionBranchList = new List<BuildDefinitionBranch>();
      List<BuildData> buildDataList = new List<BuildData>();
      foreach (BuildDefinitionBranch branch in branches)
      {
        try
        {
          BuildData build = new BuildData()
          {
            ProjectId = projectInfo.Id,
            Definition = (MinimalBuildDefinition) definition,
            Priority = QueuePriority.Normal,
            QueueId = definition.Queue?.Id,
            Reason = reason,
            RequestedFor = branch.PendingSourceOwner,
            SourceBranch = branch.BranchName,
            SourceVersion = branch.PendingSourceVersion
          };
          if (variables != null)
            build.Parameters = variables.Serialize<IDictionary<string, string>>();
          if (triggerInfo != null)
            build.TriggerInfo.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) triggerInfo);
          IEnumerable<IBuildRequestValidator> validators = BuildRequestValidatorProvider.GetValidators(validationOptions ?? new BuildRequestValidationOptions());
          build = requestContext.GetService<IBuildService>().QueueBuild(requestContext, build, validators, BuildRequestValidationFlags.QueueFailedBuild, callingMethod: nameof (QueueBuilds), callingFile: "D:\\a\\_work\\1\\s\\Tfs\\Service\\Build2\\Server\\ExternalBuildHelper.cs");
          if (build != null)
          {
            if (build.ValidationResults != null && build.ValidationResults.Any<BuildRequestValidationResult>((Func<BuildRequestValidationResult, bool>) (x => x.Result == ValidationResult.Error)))
              throw new BuildRequestValidationFailedException(BuildServerResources.BuildRequestValidationFailed(), build.ValidationResults.Select<BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>((Func<BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>) (vr => vr.ToWebApiBuildRequestValidationResult(build.ToSecuredObject()))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>());
            requestContext.TraceInfo(nameof (ExternalBuildHelper), "Queued build request {0} for definition {1} using branch {2} version {3}", (object) build.Id, (object) build.Definition?.Id, (object) build.SourceBranch, (object) build.SourceVersion);
            buildDataList.Add(build);
          }
          else
            requestContext.TraceInfo(nameof (ExternalBuildHelper), "QueueBuild call for definition {0} using branch {1} version {2} did not actually queue a build", (object) definition.Id, (object) branch.BranchName, (object) branch.PendingSourceVersion);
        }
        catch (Exception ex)
        {
          definitionBranchList.Add(branch);
          requestContext.TraceException(0, nameof (ExternalBuildHelper), ex);
        }
      }
      failedBranches = (IEnumerable<BuildDefinitionBranch>) definitionBranchList;
      return (IEnumerable<BuildData>) buildDataList;
    }

    public static IdentityRef GetIdentityRefFromEmail(
      IVssRequestContext requestContext,
      string email)
    {
      return ExternalBuildHelper.GetIdentityRefFromEmail(requestContext, email, out bool _);
    }

    public static IdentityRef GetIdentityRefFromEmail(
      IVssRequestContext requestContext,
      string email,
      out bool isExternalUser)
    {
      isExternalUser = false;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (!string.IsNullOrEmpty(email))
        identity = service.GetIdentity(requestContext, email);
      if (identity == null)
      {
        identity = requestContext.GetUserIdentity();
        isExternalUser = true;
      }
      return new IdentityRef()
      {
        Id = identity.Id.ToString(),
        DisplayName = identity.DisplayName
      };
    }

    public static bool ContainsNoCIComment(IEnumerable<ExternalGitCommit> commits)
    {
      if (commits != null)
      {
        foreach (ExternalGitCommit commit in commits)
        {
          string message = commit.Message;
          if (!string.IsNullOrEmpty(message) && message.Contains(BuildCommonUtil.NoCICheckInComment))
            return true;
        }
      }
      return false;
    }

    public static string FormatCommit(byte[] commit) => BuildSourceVersion.FormatCommit(commit);

    private static Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult ToWebApiBuildRequestValidationResult(
      this BuildRequestValidationResult srvBuildRequestValidationResult,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildRequestValidationResult == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult) null;
      return new Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult(securedObject)
      {
        Result = (Microsoft.TeamFoundation.Build.WebApi.ValidationResult) srvBuildRequestValidationResult.Result,
        Message = srvBuildRequestValidationResult.Message
      };
    }

    public static void QueueUpdateSubscriptionJob(
      IVssRequestContext requestContext,
      string repoUrl,
      string repoId,
      string repositoryType,
      string eventType,
      Guid projectId,
      string channelId)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition()
      {
        Data = TeamFoundationSerializationUtility.SerializeToXml((object) new ExternalBuildHelper.UpdateSubscriptionsServiceConnectionsJobData()
        {
          RepositoryIdentifier = repoId,
          ProjectId = projectId,
          EventType = eventType,
          RepositoryUrl = repoUrl,
          ChannelId = channelId
        }),
        Name = "UpdateSubscriptionsServiceConnectionsJob",
        PriorityClass = JobPriorityClass.Idle
      };
      if (repositoryType.Equals("GitHub", StringComparison.OrdinalIgnoreCase))
        foundationJobDefinition.ExtensionName = "Microsoft.TeamFoundation.Build2.Server.Extensions.GitHubUpdateSubscriptionsServiceConnectionsJob";
      else if (repositoryType.Equals("GitHubEnterprise", StringComparison.OrdinalIgnoreCase))
        foundationJobDefinition.ExtensionName = "Microsoft.TeamFoundation.Build2.Server.Extensions.GitHubEnterpriseUpdateSubscriptionsServiceConnectionsJob";
      else if (repositoryType.Equals("Bitbucket", StringComparison.OrdinalIgnoreCase))
        foundationJobDefinition.ExtensionName = "Microsoft.TeamFoundation.Build2.Server.Extensions.BitbucketUpdateSubscriptionsServiceConnectionsJob";
      if (foundationJobDefinition.ExtensionName.IsNullOrEmpty<char>())
        return;
      service.QueueOneTimeJob(requestContext, foundationJobDefinition.Name, foundationJobDefinition.ExtensionName, foundationJobDefinition.Data, JobPriorityLevel.Idle, foundationJobDefinition.PriorityClass, TimeSpan.Zero);
    }

    public class UpdateSubscriptionsServiceConnectionsJobData
    {
      public string RepositoryIdentifier { get; set; }

      public Guid ProjectId { get; set; }

      public string EventType { get; set; }

      public string RepositoryUrl { get; set; }

      public string ChannelId { get; set; }
    }
  }
}
