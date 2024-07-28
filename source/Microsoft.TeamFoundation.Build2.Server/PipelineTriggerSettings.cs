// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PipelineTriggerSettings
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class PipelineTriggerSettings
  {
    public static readonly PipelineTriggerSettings Disabled = new PipelineTriggerSettings();
    public static readonly PipelineTriggerSettings Default = new PipelineTriggerSettings()
    {
      ForkProtectionEnabled = false,
      BuildsEnabledForForks = true,
      EnforceJobAuthScopeForForks = true,
      EnforceNoAccessToSecretsFromForks = true,
      IsCommentRequiredForPullRequest = true,
      RequireCommentsForNonTeamMembersOnly = false,
      RequireCommentsForNonTeamMemberAndNonContributors = false
    };

    public bool ForkProtectionEnabled { get; set; }

    public bool BuildsEnabledForForks { get; set; }

    public bool EnforceJobAuthScopeForForks { get; set; }

    public bool EnforceNoAccessToSecretsFromForks { get; set; }

    public bool IsCommentRequiredForPullRequest { get; set; }

    public bool RequireCommentsForNonTeamMembersOnly { get; set; }

    public bool RequireCommentsForNonTeamMemberAndNonContributors { get; set; }

    public PipelineTriggerSettings()
    {
    }

    public PipelineTriggerSettings(
      bool forkProtectionEnabled,
      bool buildsEnabledForForks,
      bool enforceJobAuthScopeForForks,
      bool enforceNoAccessToSecretsFromForks,
      bool isCommentRequiredForPullRequest,
      bool requireCommentsForNonTeamMembersOnly,
      bool requireCommentsForNonTeamMemberAndNonContributors)
    {
      this.ForkProtectionEnabled = forkProtectionEnabled;
      this.BuildsEnabledForForks = buildsEnabledForForks;
      this.EnforceJobAuthScopeForForks = enforceJobAuthScopeForForks;
      this.EnforceNoAccessToSecretsFromForks = enforceNoAccessToSecretsFromForks;
      this.IsCommentRequiredForPullRequest = isCommentRequiredForPullRequest;
      this.RequireCommentsForNonTeamMembersOnly = requireCommentsForNonTeamMembersOnly;
      this.RequireCommentsForNonTeamMemberAndNonContributors = requireCommentsForNonTeamMemberAndNonContributors;
    }

    public static PipelineTriggerSettings.CommentTriggerOption GetCommentTriggerOption(
      bool isCommentRequiredForPullRequest,
      bool requireCommentsForNonTeamMembersOnly,
      bool requireCommentsForNonTeamMemberAndNonContributors)
    {
      if (!isCommentRequiredForPullRequest)
        return PipelineTriggerSettings.CommentTriggerOption.None;
      if (!requireCommentsForNonTeamMembersOnly && !requireCommentsForNonTeamMemberAndNonContributors)
        return PipelineTriggerSettings.CommentTriggerOption.All;
      return requireCommentsForNonTeamMembersOnly ? PipelineTriggerSettings.CommentTriggerOption.NonTeamMembers : PipelineTriggerSettings.CommentTriggerOption.NonTeamMembersNonContributor;
    }

    public struct Optional
    {
      public bool? ForkProtectionEnabled;
      public bool? BuildsEnabledForForks;
      public bool? EnforceJobAuthScopeForForks;
      public bool? EnforceNoAccessToSecretsFromForks;
      public bool? IsCommentRequiredForPullRequest;
      public bool? RequireCommentsForNonTeamMembersOnly;
      public bool? RequireCommentsForNonTeamMemberAndNonContributors;
    }

    public enum CommentTriggerOption
    {
      None,
      NonTeamMembersNonContributor,
      NonTeamMembers,
      All,
    }
  }
}
