// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.VersionControlSettings
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class VersionControlSettings
  {
    private const long DefaultMaxFileSize = 524288;
    private const string c_witMentionsEnabledPathFormat = "/VersionControl/Repositories/{0}/WitMentionsEnabled";
    private const string c_witResolutionMentionsEnabledPathFormat = "/VersionControl/Repositories/{0}/WitResolutionMentionsEnabled";
    private const string c_witTransitionsStickyPathFormat = "/VersionControl/Repositories/{0}/WitTransitionsSticky";
    private const string c_gitDefaultBranchNameSettingsPath = "Git/DefaultBranchName";
    private const string c_RepoCreatedBranchesManagePermissionsEnabledPathFormat = "/VersionControl/Repositories/{0}/CreatedBranchesManagePermissionsEnabled";
    private const string c_NewReposCreatedBranchesManagePermissionsEnabledPathFormat = "VersionControl/Projects/{0}/CreatedBranchesManagePermissionsEnabled";
    private const string c_PullRequestAsDraftByDefaultPathFormat = "VersionControl/Repositories/PullRequestAsDraftByDefault";
    private const string c_suggestionNamePath = "Git/SuggestionName";

    public static long ReadMaxFileSize(IVssRequestContext requestContext, long? defaultMaxFileSize = null)
    {
      if (defaultMaxFileSize.HasValue)
      {
        long? nullable = defaultMaxFileSize;
        long num = 0;
        if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
          goto label_3;
      }
      defaultMaxFileSize = new long?(524288L);
label_3:
      return requestContext.GetService<IVssRegistryService>().ReadWebSetting<long>(requestContext, "/VersionControl/MaxFileSize", defaultMaxFileSize.Value);
    }

    public static bool ReadGravatarEnabled(IVssRequestContext requestContext) => GitServerUtils.UseGravatarForExternalIdentities(requestContext);

    public static void UpdateGravatarEnabled(IVssRequestContext requestContext, bool value) => requestContext.GetService<IVssRegistryService>().WriteSetting<bool>(requestContext, "/WebAccess/IdentityImage/DisableGravatar", !value);

    public static string ReadDefaultBranchName(IVssRequestContext requestContext, Guid? projectId) => projectId.HasValue ? requestContext.GetService<ISettingsService>().GetValue<string>(requestContext, SettingsUserScope.AllUsers, "Project", projectId.Value.ToString(), "Git/DefaultBranchName", (string) null) : requestContext.GetService<IVssRegistryService>().ReadSetting<string>(requestContext, "/Service/Git/Settings/DefaultBranchName", (string) null);

    public static void UpdateDefaultBranchName(
      IVssRequestContext requestContext,
      Guid? projectId,
      string defaultBranchName)
    {
      if (projectId.HasValue)
        requestContext.GetService<ISettingsService>().SetValue(requestContext, SettingsUserScope.AllUsers, "Project", projectId.Value.ToString(), "Git/DefaultBranchName", (object) defaultBranchName);
      else
        requestContext.GetService<IVssRegistryService>().WriteSetting<string>(requestContext, "/Service/Git/Settings/DefaultBranchName", defaultBranchName);
    }

    public static void UpdateDisableTfvcRepositories(IVssRequestContext requestContext, bool value)
    {
      if (!requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "WebAccess.VersionControl.DisableTfvcRepository"))
        return;
      requestContext.GetService<IVssRegistryService>().WriteSetting<bool>(requestContext, "/Service/Git/Settings/DisableTfvcRepositories", value);
      VersionControlSettings.WriteUpdatedSettingToTelemetry(requestContext, "DisableTfvcRepositoriesCreation", "UpdateDisableTfvcRepositoriesCreationToggle", value, new Guid?());
    }

    private static void WriteUpdatedSettingToTelemetry(
      IVssRequestContext requestContext,
      string feature,
      string eventType,
      bool value,
      Guid? projectId)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("EventType", eventType);
      properties.Add("Level", projectId.HasValue ? "Project" : "Organization");
      properties.Add("ProjectId", projectId.HasValue ? (object) projectId.Value : (object) null);
      properties.Add("NewValue", value);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "VersionControlSetting", feature, properties);
    }

    public static bool ReadDisableTfvcRepositories(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().ReadSetting<bool>(requestContext, "/Service/Git/Settings/DisableTfvcRepositories", false);

    public static bool ReadWitMentionsEnabled(IVssRequestContext requestContext, Guid repositoryId) => requestContext.GetService<IVssRegistryService>().ReadWebSetting<bool>(requestContext, string.Format("/VersionControl/Repositories/{0}/WitMentionsEnabled", (object) repositoryId), true);

    public static void UpdateWitMentionsEnabled(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool value)
    {
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        repositoryById.Permissions.CheckEditPolicies();
      requestContext.GetService<IVssRegistryService>().WriteWebSetting<bool>(requestContext, string.Format("/VersionControl/Repositories/{0}/WitMentionsEnabled", (object) repositoryId), value);
    }

    public static bool ReadWitResolutionMentionsEnabled(
      IVssRequestContext requestContext,
      Guid repositoryId)
    {
      return requestContext.GetService<IVssRegistryService>().ReadWebSetting<bool>(requestContext, string.Format("/VersionControl/Repositories/{0}/WitResolutionMentionsEnabled", (object) repositoryId), true);
    }

    public static void UpdateWitResolutionMentionsEnabled(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool value)
    {
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        repositoryById.Permissions.CheckEditPolicies();
      requestContext.GetService<IVssRegistryService>().WriteWebSetting<bool>(requestContext, string.Format("/VersionControl/Repositories/{0}/WitResolutionMentionsEnabled", (object) repositoryId), value);
    }

    public static bool? ReadForksEnabled(IVssRequestContext requestContext, Guid repositoryId)
    {
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        return repositoryById.Settings.GvfsOnly ? new bool?() : new bool?(repositoryById.Settings.AllowedForkTargets != 0);
    }

    public static void UpdatedForksEnable(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool value)
    {
      requestContext.GetService<ITeamFoundationGitRepositoryService>().UpdateRepositorySettingsById(requestContext, repositoryId, new GitRepoSettings(allowedForkTargets: new AllowedForkTargets?(value ? AllowedForkTargets.WithinCollection : AllowedForkTargets.Nowhere)));
    }

    public static void UpdateStrictVoteMode(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool value)
    {
      ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repositoryById = service.FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        repositoryById.Permissions.CheckEditPolicies();
      service.UpdateRepositorySettingsById(requestContext, repositoryId, new GitRepoSettings(strictVoteMode: new bool?(value)));
    }

    public static bool ReadStrictVoteMode(IVssRequestContext requestContext, Guid repositoryId)
    {
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        return repositoryById.Settings.StrictVoteMode;
    }

    public static void UpdateInheritPullRequestCreationMode(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool value)
    {
      ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repositoryById = service.FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        repositoryById.Permissions.CheckEditPolicies();
      service.UpdateRepositorySettingsById(requestContext, repositoryId, new GitRepoSettings(inheritPullRequestCreationMode: new bool?(value)));
    }

    public static bool ReadInheritPullRequestCreationMode(
      IVssRequestContext requestContext,
      Guid repositoryId)
    {
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        return repositoryById.Settings.InheritPullRequestCreationMode;
    }

    public static void UpdateRepoPullRequestAsDraftByDefault(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool value)
    {
      requestContext.TraceAlways(1013910, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (VersionControlSettings), string.Format("Repo setting 'Create PR's as draft by default' has been updated for repository: {0}. New value: {1}", (object) repositoryId, (object) value));
      ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repositoryById = service.FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        repositoryById.Permissions.CheckEditPolicies();
      service.UpdateRepositorySettingsById(requestContext, repositoryId, new GitRepoSettings(repoPullRequestAsDraftByDefault: new bool?(value)));
    }

    public static bool ReadRepoPullRequestAsDraftByDefault(
      IVssRequestContext requestContext,
      Guid repositoryId)
    {
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        return repositoryById.Settings.RepoPullRequestAsDraftByDefault;
    }

    public static bool ReadWitTransitionsSticky(
      IVssRequestContext requestContext,
      Guid repositoryId)
    {
      return requestContext.GetService<IVssRegistryService>().ReadWebSetting<bool>(requestContext, string.Format("/VersionControl/Repositories/{0}/WitTransitionsSticky", (object) repositoryId), true);
    }

    public static void UpdateWitTransitionsSticky(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool value)
    {
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        repositoryById.Permissions.CheckEditPolicies();
      requestContext.GetService<IVssRegistryService>().WriteWebSetting<bool>(requestContext, string.Format("/VersionControl/Repositories/{0}/WitTransitionsSticky", (object) repositoryId), value);
    }

    public static bool ReadDisabledRepo(IVssRequestContext requestContext, Guid repositoryId) => requestContext.GetService<ITeamFoundationGitRepositoryService>().GetIsRepositoryDisabledById(requestContext, repositoryId);

    public static void UpdateDisabledState(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool isDisabled)
    {
      ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repositoryById = service.FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
      {
        if (isDisabled)
          service.DisableRepository(requestContext, (RepoScope) repositoryById.Key);
        else
          service.EnableRepository(requestContext, (RepoScope) repositoryById.Key);
      }
    }

    public static bool ReadRepoCreatedBranchesManagePermissionsEnabled(
      IVssRequestContext requestContext,
      Guid repositoryId)
    {
      return requestContext.GetService<IVssRegistryService>().ReadWebSetting<bool>(requestContext, string.Format("/VersionControl/Repositories/{0}/CreatedBranchesManagePermissionsEnabled", (object) repositoryId), true);
    }

    public static void UpdateRepoCreatedBranchesManagePermissionsEnabled(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool value)
    {
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId, includeDisabled: true))
        repositoryById.Permissions.CheckEditPolicies();
      requestContext.GetService<IVssRegistryService>().WriteWebSetting<bool>(requestContext, string.Format("/VersionControl/Repositories/{0}/CreatedBranchesManagePermissionsEnabled", (object) repositoryId), value);
    }

    public static bool ReadNewReposCreatedBranchesManagePermissionsEnabled(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.GetService<ISettingsService>().GetValue<bool>(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "VersionControl/Projects/{0}/CreatedBranchesManagePermissionsEnabled", true);
    }

    public static void UpdateNewReposCreatedBranchesManagePermissionsEnabled(
      IVssRequestContext requestContext,
      Guid projectId,
      bool value)
    {
      requestContext.GetService<ISettingsService>().SetValue(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "VersionControl/Projects/{0}/CreatedBranchesManagePermissionsEnabled", (object) value);
    }

    public static bool ReadPullRequestAsDraftByDefault(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.GetService<ISettingsService>().GetValue<bool>(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "VersionControl/Repositories/PullRequestAsDraftByDefault");
    }

    public static void UpdatePullRequestAsDraftByDefault(
      IVssRequestContext requestContext,
      Guid projectId,
      bool value)
    {
      requestContext.TraceAlways(1013905, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (VersionControlSettings), string.Format("Project setting 'Create PR's as draft by default' has been updated for project: {0}. New value: {1}", (object) projectId, (object) value));
      requestContext.GetService<ISettingsService>().SetValue(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "VersionControl/Repositories/PullRequestAsDraftByDefault", (object) value);
    }

    public static string ReadSuggestion(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<ISettingsService>().GetValue<string>(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "Git/SuggestionName", "");

    public static void UpdateSuggestion(
      IVssRequestContext requestContext,
      Guid projectId,
      string value)
    {
      requestContext.GetService<ISettingsService>().SetValue(requestContext, SettingsUserScope.AllUsers, "Project", projectId.ToString(), "Git/SuggestionName", (object) value);
    }
  }
}
