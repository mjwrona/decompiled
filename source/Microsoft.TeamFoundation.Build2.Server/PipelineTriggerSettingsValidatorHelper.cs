// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PipelineTriggerSettingsValidatorHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class PipelineTriggerSettingsValidatorHelper
  {
    private static bool SkipUpdateIfForkProtectionIsDisabled(
      ref PipelineTriggerSettings.Optional update,
      PipelineTriggerSettings currentLevelSettings,
      PipelineTriggerSettings upperLevelSettings = null)
    {
      if ((upperLevelSettings == null ? 0 : (upperLevelSettings.ForkProtectionEnabled ? 1 : 0)) != 0 || currentLevelSettings.ForkProtectionEnabled || update.ForkProtectionEnabled.HasValue && update.ForkProtectionEnabled.Value)
        return false;
      bool? protectionEnabled = update.ForkProtectionEnabled;
      update = new PipelineTriggerSettings.Optional();
      update.ForkProtectionEnabled = protectionEnabled.HasValue ? new bool?(protectionEnabled.Value) : new bool?();
      return true;
    }

    private static bool SetUpdateToDisabledIfOnlyForkProtectionWillChange(
      ref PipelineTriggerSettings.Optional update,
      PipelineTriggerSettings currentLevelSettings)
    {
      if ((!update.ForkProtectionEnabled.HasValue || update.BuildsEnabledForForks.HasValue || update.EnforceJobAuthScopeForForks.HasValue || update.EnforceNoAccessToSecretsFromForks.HasValue || update.IsCommentRequiredForPullRequest.HasValue || update.RequireCommentsForNonTeamMembersOnly.HasValue ? 0 : (!update.RequireCommentsForNonTeamMemberAndNonContributors.HasValue ? 1 : 0)) == 0)
        return false;
      update.BuildsEnabledForForks = new bool?(PipelineTriggerSettings.Disabled.BuildsEnabledForForks);
      update.EnforceJobAuthScopeForForks = new bool?(PipelineTriggerSettings.Disabled.EnforceJobAuthScopeForForks);
      update.EnforceNoAccessToSecretsFromForks = new bool?(PipelineTriggerSettings.Disabled.EnforceNoAccessToSecretsFromForks);
      update.IsCommentRequiredForPullRequest = new bool?(PipelineTriggerSettings.Disabled.IsCommentRequiredForPullRequest);
      update.RequireCommentsForNonTeamMembersOnly = new bool?(PipelineTriggerSettings.Disabled.RequireCommentsForNonTeamMembersOnly);
      update.RequireCommentsForNonTeamMemberAndNonContributors = new bool?(PipelineTriggerSettings.Disabled.RequireCommentsForNonTeamMemberAndNonContributors);
      return true;
    }

    private static bool SkipUpdateIfBuildsAreDisabled(
      ref PipelineTriggerSettings.Optional update,
      PipelineTriggerSettings currentLevelSettings)
    {
      if (currentLevelSettings.BuildsEnabledForForks || update.BuildsEnabledForForks.HasValue && update.BuildsEnabledForForks.Value)
        return false;
      update = new PipelineTriggerSettings.Optional();
      return true;
    }

    private static bool SetUpdateToDefaultsIfOnlyBuildsEnabledWillChange(
      ref PipelineTriggerSettings.Optional update,
      PipelineTriggerSettings currentLevelSettings)
    {
      if ((!update.BuildsEnabledForForks.HasValue || update.EnforceJobAuthScopeForForks.HasValue || update.EnforceNoAccessToSecretsFromForks.HasValue || update.IsCommentRequiredForPullRequest.HasValue || update.RequireCommentsForNonTeamMembersOnly.HasValue ? 0 : (!update.RequireCommentsForNonTeamMemberAndNonContributors.HasValue ? 1 : 0)) == 0)
        return false;
      bool flag = !currentLevelSettings.BuildsEnabledForForks && update.BuildsEnabledForForks.Value;
      update.EnforceJobAuthScopeForForks = new bool?(flag ? PipelineTriggerSettings.Default.EnforceJobAuthScopeForForks : PipelineTriggerSettings.Disabled.EnforceJobAuthScopeForForks);
      update.EnforceNoAccessToSecretsFromForks = new bool?(flag ? PipelineTriggerSettings.Default.EnforceNoAccessToSecretsFromForks : PipelineTriggerSettings.Disabled.EnforceNoAccessToSecretsFromForks);
      update.IsCommentRequiredForPullRequest = new bool?(flag ? PipelineTriggerSettings.Default.IsCommentRequiredForPullRequest : PipelineTriggerSettings.Disabled.IsCommentRequiredForPullRequest);
      update.RequireCommentsForNonTeamMembersOnly = new bool?(flag ? PipelineTriggerSettings.Default.RequireCommentsForNonTeamMembersOnly : PipelineTriggerSettings.Disabled.RequireCommentsForNonTeamMembersOnly);
      update.RequireCommentsForNonTeamMemberAndNonContributors = new bool?(flag ? PipelineTriggerSettings.Default.RequireCommentsForNonTeamMemberAndNonContributors : PipelineTriggerSettings.Disabled.RequireCommentsForNonTeamMemberAndNonContributors);
      return true;
    }

    private static bool DisableOtherSettingsIfBuildsAreDisabled(
      ref PipelineTriggerSettings.Optional update)
    {
      if ((!update.BuildsEnabledForForks.HasValue ? 0 : (!update.BuildsEnabledForForks.Value ? 1 : 0)) == 0)
        return false;
      update.EnforceJobAuthScopeForForks = new bool?(PipelineTriggerSettings.Disabled.EnforceJobAuthScopeForForks);
      update.EnforceNoAccessToSecretsFromForks = new bool?(PipelineTriggerSettings.Disabled.EnforceNoAccessToSecretsFromForks);
      update.IsCommentRequiredForPullRequest = new bool?(PipelineTriggerSettings.Disabled.IsCommentRequiredForPullRequest);
      update.RequireCommentsForNonTeamMembersOnly = new bool?(PipelineTriggerSettings.Disabled.RequireCommentsForNonTeamMembersOnly);
      update.RequireCommentsForNonTeamMemberAndNonContributors = new bool?(PipelineTriggerSettings.Disabled.RequireCommentsForNonTeamMemberAndNonContributors);
      return true;
    }

    private static bool SkipUpdatingCommentSettingsIfCommentIsNotRequired(
      ref PipelineTriggerSettings.Optional update,
      PipelineTriggerSettings currentLevelSettings,
      PipelineTriggerSettings upperLevelSettings = null)
    {
      bool flag1 = !update.IsCommentRequiredForPullRequest.HasValue || !update.IsCommentRequiredForPullRequest.Value;
      bool flag2 = !currentLevelSettings.IsCommentRequiredForPullRequest;
      bool flag3 = upperLevelSettings == null || !upperLevelSettings.IsCommentRequiredForPullRequest;
      if (((update.RequireCommentsForNonTeamMembersOnly.HasValue ? 1 : (update.RequireCommentsForNonTeamMemberAndNonContributors.HasValue ? 1 : 0)) & (flag3 ? 1 : 0) & (flag2 ? 1 : 0) & (flag1 ? 1 : 0)) == 0)
        return false;
      update.RequireCommentsForNonTeamMembersOnly = new bool?();
      update.RequireCommentsForNonTeamMemberAndNonContributors = new bool?();
      return true;
    }

    private static bool DisableCommentSettingsIfCommentIsNotRequired(
      ref PipelineTriggerSettings.Optional update)
    {
      if (!update.IsCommentRequiredForPullRequest.HasValue || update.IsCommentRequiredForPullRequest.Value)
        return false;
      update.RequireCommentsForNonTeamMembersOnly = new bool?(PipelineTriggerSettings.Disabled.RequireCommentsForNonTeamMembersOnly);
      update.RequireCommentsForNonTeamMemberAndNonContributors = new bool?(PipelineTriggerSettings.Disabled.RequireCommentsForNonTeamMemberAndNonContributors);
      return true;
    }

    private static bool SetCommentSettingsToBeConsistent(
      ref PipelineTriggerSettings.Optional update,
      PipelineTriggerSettings currentLevelSettings,
      PipelineTriggerSettings upperLevelSettings = null)
    {
      bool flag1 = update.RequireCommentsForNonTeamMembersOnly.HasValue && update.RequireCommentsForNonTeamMembersOnly.Value;
      bool flag2 = update.RequireCommentsForNonTeamMemberAndNonContributors.HasValue && update.RequireCommentsForNonTeamMemberAndNonContributors.Value;
      if (flag2 & flag1 || flag1 && currentLevelSettings.RequireCommentsForNonTeamMemberAndNonContributors)
      {
        update.RequireCommentsForNonTeamMemberAndNonContributors = new bool?(false);
        return true;
      }
      if (flag2 && currentLevelSettings.RequireCommentsForNonTeamMembersOnly)
      {
        if (upperLevelSettings != null)
        {
          PipelineTriggerSettings.CommentTriggerOption commentTriggerOption = PipelineTriggerSettings.GetCommentTriggerOption(upperLevelSettings.IsCommentRequiredForPullRequest, upperLevelSettings.RequireCommentsForNonTeamMembersOnly, upperLevelSettings.RequireCommentsForNonTeamMemberAndNonContributors);
          if ((commentTriggerOption == PipelineTriggerSettings.CommentTriggerOption.NonTeamMembers ? 1 : (commentTriggerOption == PipelineTriggerSettings.CommentTriggerOption.All ? 1 : 0)) != 0)
            return true;
        }
        update.RequireCommentsForNonTeamMembersOnly = new bool?(false);
      }
      return false;
    }

    public static void ValidateAndFixPipelineTriggerSettingsUpdate(
      ref PipelineTriggerSettings.Optional update,
      PipelineTriggerSettings currentLevelSettings,
      PipelineTriggerSettings upperLevelSettings = null)
    {
      if (PipelineTriggerSettingsValidatorHelper.SkipUpdateIfForkProtectionIsDisabled(ref update, currentLevelSettings, upperLevelSettings) || PipelineTriggerSettingsValidatorHelper.SetUpdateToDisabledIfOnlyForkProtectionWillChange(ref update, currentLevelSettings) || PipelineTriggerSettingsValidatorHelper.SkipUpdateIfBuildsAreDisabled(ref update, currentLevelSettings) || PipelineTriggerSettingsValidatorHelper.SetUpdateToDefaultsIfOnlyBuildsEnabledWillChange(ref update, currentLevelSettings) || PipelineTriggerSettingsValidatorHelper.DisableOtherSettingsIfBuildsAreDisabled(ref update) || PipelineTriggerSettingsValidatorHelper.SkipUpdatingCommentSettingsIfCommentIsNotRequired(ref update, currentLevelSettings, upperLevelSettings) || PipelineTriggerSettingsValidatorHelper.DisableCommentSettingsIfCommentIsNotRequired(ref update))
        return;
      PipelineTriggerSettingsValidatorHelper.SetCommentSettingsToBeConsistent(ref update, currentLevelSettings, upperLevelSettings);
    }
  }
}
