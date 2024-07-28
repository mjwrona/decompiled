// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRepoSettingsProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitRepoSettingsProvider
  {
    private readonly IVssRequestContext m_rc;
    private readonly RepoKey m_repoKey;
    private readonly ITeamFoundationPolicyService m_policySvc;
    private readonly IVssRegistryService m_regSvc;
    private readonly IContributedFeatureService m_featSvc;
    private const string s_Layer = "GitRepoSettingsProvider";

    public GitRepoSettingsProvider(
      IVssRequestContext rc,
      IVssRegistryService regSvc,
      ITeamFoundationPolicyService policySvc,
      IContributedFeatureService featSvc,
      RepoKey repoKey)
    {
      this.m_rc = rc;
      this.m_regSvc = regSvc;
      this.m_policySvc = policySvc;
      this.m_featSvc = featSvc;
      this.m_repoKey = repoKey;
    }

    public GitRepoSettings GetFullSettings()
    {
      Version result;
      Version.TryParse(this.m_regSvc.GetValue(this.m_rc, (RegistryQuery) "/Service/Git/Settings/MinimumRecommendedGitVersion", true, (string) null), out result);
      string str1 = this.m_regSvc.GetValue(this.m_rc, (RegistryQuery) "/Service/Git/Settings/UserAgentExemptions", true, (string) null);
      string str2 = this.m_regSvc.GetValue(this.m_rc, (RegistryQuery) "/Service/Git/Settings/UpdateGitMessage", true, (string) null);
      long? nullable1 = this.GetRegistrySetting<long>("/Service/Git/Settings/MaxPushSize");
      if (!this.m_rc.ExecutionEnvironment.IsHostedDeployment && !nullable1.HasValue)
        nullable1 = new long?(long.MaxValue);
      bool? registrySetting = this.GetRegistrySetting<bool>("/Service/Git/Settings/ForceCloneHack");
      string[] strArray;
      if (str1 == null)
        strArray = (string[]) null;
      else
        strArray = str1.Split(',');
      IReadOnlyCollection<string> strings = (IReadOnlyCollection<string>) strArray;
      Version version = result;
      string str3 = str2;
      long? nullable2 = nullable1;
      int? nullable3 = this.GetRegistrySetting<int>("/Service/Git/Settings/MaxPathLength");
      long? nullable4 = nullable3.HasValue ? new long?((long) nullable3.GetValueOrDefault()) : new long?();
      nullable3 = this.GetRegistrySetting<int>("/Service/Git/Settings/MaxPathComponentLength");
      long? nullable5 = nullable3.HasValue ? new long?((long) nullable3.GetValueOrDefault()) : new long?();
      bool? nullable6 = registrySetting;
      bool? enforceConsistentCase = new bool?();
      bool? rejectDotGit = new bool?();
      bool? optimizedByDefault = new bool?();
      nullable3 = new int?();
      int? breadcrumbDays = nullable3;
      AllowedForkTargets? allowedForkTargets = new AllowedForkTargets?();
      bool? gvfsOnly = new bool?();
      bool? detectRenameFalsePositivesByDefault = new bool?();
      IReadOnlyCollection<string> userAgentExemptions = strings;
      Version minimumRecommendedGitVersion = version;
      string updateGitMessage = str3;
      long? maxPushSize = nullable2;
      long? maxPathLength = nullable4;
      long? maxPathComponentLength = nullable5;
      bool? forceCloneHack = nullable6;
      bool? strictVoteMode = new bool?();
      bool? inheritPullRequestCreationMode = new bool?();
      bool? repoPullRequestAsDraftByDefault = new bool?();
      GitRepoSettings fullSettings = new GitRepoSettings(enforceConsistentCase, rejectDotGit, optimizedByDefault, breadcrumbDays, allowedForkTargets, gvfsOnly, detectRenameFalsePositivesByDefault: detectRenameFalsePositivesByDefault, userAgentExemptions: userAgentExemptions, minimumRecommendedGitVersion: minimumRecommendedGitVersion, updateGitMessage: updateGitMessage, maxPushSize: maxPushSize, maxPathLength: maxPathLength, maxPathComponentLength: maxPathComponentLength, forceCloneHack: forceCloneHack, strictVoteMode: strictVoteMode, inheritPullRequestCreationMode: inheritPullRequestCreationMode, repoPullRequestAsDraftByDefault: repoPullRequestAsDraftByDefault);
      IReadOnlyList<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>> allPolicies = this.GetAllPolicies();
      IEnumerator<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>> enumerator = allPolicies.Where<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>>((Func<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>, bool>) (p => !GitRepoSettingsProvider.IsRepoSpecificPolicy(p))).GetEnumerator();
      if (enumerator.MoveNext())
      {
        fullSettings = fullSettings.OverrideWith((GitRepoSettings) enumerator.Current.Settings);
        if (enumerator.MoveNext())
          throw new PolicyImplementationException(Resources.Format("GitSettingsPolicyDuplicateProject", (object) this.m_repoKey.ProjectId));
      }
      TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget> repoScopedPolicy = this.GetRepoScopedPolicy(allPolicies);
      if (repoScopedPolicy != null)
        fullSettings = fullSettings.OverrideWith((GitRepoSettings) repoScopedPolicy.Settings);
      return fullSettings;
    }

    public void UpdateRepositorySettings(GitRepoSettings repoSettings)
    {
      TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget> repoScopedPolicy1 = this.GetRepoScopedPolicy(this.GetAllPolicies());
      repoSettings = repoScopedPolicy1?.Settings?.OverrideWith(repoSettings) ?? repoSettings;
      TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget> repoScopedPolicy2 = this.GetRepoScopedPolicy(this.m_policySvc.GetApplicablePolicies<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>>(this.m_rc, (ITeamFoundationPolicyTarget) new GitRepositoryTarget(this.m_repoKey.GetProjectUri(), this.m_repoKey.RepoId), out List<PolicyFailures> _));
      string settings = JsonConvert.SerializeObject((object) repoSettings.ToPolicySettings(this.m_repoKey.RepoId));
      if (repoScopedPolicy1 != null)
        this.m_policySvc.UpdatePolicyConfiguration(this.m_rc, repoScopedPolicy2.Configuration.ConfigurationId, GitRepoSettingsForPolicy.PolicyTypeId, this.m_repoKey.ProjectId, repoScopedPolicy2.Configuration.IsEnabled, repoScopedPolicy2.Configuration.IsBlocking, repoScopedPolicy2.Configuration.IsEnterpriseManaged, settings);
      else
        this.m_policySvc.CreatePolicyConfiguration(this.m_rc, GitRepoSettingsForPolicy.PolicyTypeId, this.m_repoKey.ProjectId, true, true, false, settings);
    }

    private IReadOnlyList<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>> GetAllPolicies() => this.m_policySvc.GetApplicablePolicies<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>>(this.m_rc, (ITeamFoundationPolicyTarget) new GitRepositoryTarget(this.m_repoKey.GetProjectUri(), this.m_repoKey.RepoId), out List<PolicyFailures> _);

    private TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget> GetRepoScopedPolicy(
      IReadOnlyList<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>> policies)
    {
      List<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>> list = policies.Where<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>>((Func<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>, bool>) (p => GitRepoSettingsProvider.IsRepoSpecificPolicy(p))).OrderByDescending<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>, int>((Func<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>, int>) (p => p.Configuration.ConfigurationId)).ToList<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>>();
      TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget> repoScopedPolicy = list.FirstOrDefault<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>>();
      if (list.Count > 1)
      {
        IEnumerable<string> values = list.Skip<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>>(1).Select<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>, string>((Func<TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget>, string>) (p => p.Configuration.ConfigurationId.ToString()));
        this.m_rc.Trace(1013839, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitRepoSettingsProvider), new StringBuilder().AppendLine(string.Format("Git repository '{0}' has conflicting settings policies configured.", (object) this.m_repoKey.RepoId)).AppendLine(string.Format("Policy configuration '{0}' was used and other configurations were ignored.", (object) repoScopedPolicy.Configuration.ConfigurationId)).AppendLine("Ignored policy configuration id(s): " + string.Join(", ", values)).ToString());
      }
      return repoScopedPolicy;
    }

    private static bool IsRepoSpecificPolicy(
      TeamFoundationPolicy<GitRepoSettingsForPolicy, TeamFoundationPolicyEvaluationRecordContext, GitRepositoryTarget> policy)
    {
      return policy.Settings.Scope.ScopeItems.Where<GitPolicyRepositoryScopeItem>((Func<GitPolicyRepositoryScopeItem, bool>) (si => si.RepositoryId.HasValue)).Count<GitPolicyRepositoryScopeItem>() != 0;
    }

    private T? GetRegistrySetting<T>(string registryPath) where T : struct
    {
      T? registrySetting = this.m_regSvc.GetValue<T?>(this.m_rc, (RegistryQuery) (registryPath + "/" + this.m_repoKey.RepoId.ToString()), new T?());
      if (!registrySetting.HasValue)
        registrySetting = this.m_regSvc.GetValue<T?>(this.m_rc, (RegistryQuery) registryPath, true, new T?());
      return registrySetting;
    }
  }
}
