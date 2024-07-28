// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRepoSettings
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitRepoSettings
  {
    internal const bool EnforceConsistentCaseDefault = false;
    protected readonly bool? m_enforceConsistentCase;
    protected readonly bool? m_rejectDotGit;
    protected readonly bool? m_optimizedByDefault;
    protected readonly int? m_breadcrumbDays;
    protected readonly long? m_maxPushSize;
    protected readonly long? m_maxPathLength;
    protected readonly long? m_maxPathComponentLength;
    protected readonly AllowedForkTargets? m_allowedForkTargets;
    protected readonly bool? m_gvfsOnly;
    protected readonly IReadOnlyCollection<Guid> m_gvfsExemptUsers;
    protected readonly IReadOnlyCollection<GvfsAllowedVersionRange> m_gvfsAllowedVersionRanges;
    protected readonly IReadOnlyCollection<string> m_userAgentExemptions;
    protected readonly Version m_minimumRecommendedGitVersion;
    protected readonly string m_updateGitMessage;
    protected readonly bool? m_detectRenameFalsePositivesByDefault;
    private readonly bool? m_forceCloneHack;
    protected readonly bool? m_strictVoteMode;
    protected readonly bool? m_inheritPullRequestCreationMode;
    protected readonly bool? m_repoPullRequestAsDraftByDefault;

    public GitRepoSettings(
      bool? enforceConsistentCase = null,
      bool? rejectDotGit = null,
      bool? optimizedByDefault = null,
      int? breadcrumbDays = null,
      AllowedForkTargets? allowedForkTargets = null,
      bool? gvfsOnly = null,
      IReadOnlyCollection<Guid> gvfsExemptUsers = null,
      IReadOnlyCollection<GvfsAllowedVersionRange> gvfsAllowedVersionRanges = null,
      bool? detectRenameFalsePositivesByDefault = null,
      IReadOnlyCollection<string> userAgentExemptions = null,
      Version minimumRecommendedGitVersion = null,
      string updateGitMessage = null,
      long? maxPushSize = null,
      long? maxPathLength = null,
      long? maxPathComponentLength = null,
      bool? forceCloneHack = null,
      bool? strictVoteMode = null,
      bool? inheritPullRequestCreationMode = null,
      bool? repoPullRequestAsDraftByDefault = null)
    {
      this.m_enforceConsistentCase = enforceConsistentCase;
      this.m_rejectDotGit = rejectDotGit;
      this.m_optimizedByDefault = optimizedByDefault;
      this.m_breadcrumbDays = breadcrumbDays;
      this.m_maxPushSize = maxPushSize;
      this.m_maxPathLength = maxPathLength;
      this.m_maxPathComponentLength = maxPathComponentLength;
      this.m_allowedForkTargets = allowedForkTargets;
      this.m_gvfsOnly = gvfsOnly;
      this.m_gvfsExemptUsers = gvfsExemptUsers;
      this.m_gvfsAllowedVersionRanges = gvfsAllowedVersionRanges;
      this.m_userAgentExemptions = userAgentExemptions;
      this.m_minimumRecommendedGitVersion = minimumRecommendedGitVersion;
      this.m_updateGitMessage = updateGitMessage;
      this.m_detectRenameFalsePositivesByDefault = detectRenameFalsePositivesByDefault;
      this.m_forceCloneHack = forceCloneHack;
      this.m_strictVoteMode = strictVoteMode;
      this.m_inheritPullRequestCreationMode = inheritPullRequestCreationMode;
      this.m_repoPullRequestAsDraftByDefault = repoPullRequestAsDraftByDefault;
    }

    public bool EnforceConsistentCase => this.m_enforceConsistentCase.GetValueOrDefault(false);

    public bool RejectDotGit => this.m_rejectDotGit.GetValueOrDefault(true);

    public bool OptimizedByDefault => this.m_optimizedByDefault.GetValueOrDefault(false);

    public bool DetectRenameFalsePositivesByDefault => this.m_detectRenameFalsePositivesByDefault.GetValueOrDefault(false);

    public int BreadcrumbDays => this.m_breadcrumbDays.GetValueOrDefault(7);

    public long MaxPushSize => this.m_maxPushSize.GetValueOrDefault(5368709120L);

    public long MaxPathLength => this.m_maxPathLength.GetValueOrDefault(32766L);

    public long MaxPathComponentLength => this.m_maxPathComponentLength.GetValueOrDefault(4096L);

    public bool ForceCloneHack => this.m_forceCloneHack.GetValueOrDefault(false);

    public AllowedForkTargets AllowedForkTargets => this.GvfsOnly ? AllowedForkTargets.Nowhere : this.m_allowedForkTargets.GetValueOrDefault(AllowedForkTargets.WithinCollection);

    public bool GvfsOnly => this.m_gvfsOnly.GetValueOrDefault(false);

    public bool StrictVoteMode => this.m_strictVoteMode.GetValueOrDefault(false);

    public bool InheritPullRequestCreationMode => this.m_inheritPullRequestCreationMode.GetValueOrDefault(true);

    public bool RepoPullRequestAsDraftByDefault => this.m_repoPullRequestAsDraftByDefault.GetValueOrDefault(false);

    public IEnumerable<Guid> GvfsExemptUsers => (IEnumerable<Guid>) this.m_gvfsExemptUsers ?? Enumerable.Empty<Guid>();

    public IReadOnlyCollection<GvfsAllowedVersionRange> GvfsAllowedVersionRanges => this.m_gvfsAllowedVersionRanges ?? (IReadOnlyCollection<GvfsAllowedVersionRange>) new List<GvfsAllowedVersionRange>();

    public IReadOnlyCollection<string> UserAgentExemptions => this.m_userAgentExemptions ?? (IReadOnlyCollection<string>) new List<string>();

    public Version MinimumRecommendedGitVersion => this.m_minimumRecommendedGitVersion == (Version) null ? GitServerConstants.MinimumRecommendedGitVersion : this.m_minimumRecommendedGitVersion;

    public string UpdateGitMessage => this.m_updateGitMessage ?? Resources.Format("ConsiderUpdateGitMessage");

    internal GitRepoSettings OverrideWith(GitRepoSettings overrides)
    {
      bool? enforceConsistentCase = overrides.m_enforceConsistentCase ?? this.m_enforceConsistentCase;
      bool? rejectDotGit = overrides.m_rejectDotGit ?? this.m_rejectDotGit;
      bool? optimizedByDefault = overrides.m_optimizedByDefault ?? this.m_optimizedByDefault;
      int? breadcrumbDays = overrides.m_breadcrumbDays ?? this.m_breadcrumbDays;
      AllowedForkTargets? allowedForkTargets = overrides.m_allowedForkTargets ?? this.m_allowedForkTargets;
      bool? gvfsOnly = overrides.m_gvfsOnly ?? this.m_gvfsOnly;
      IReadOnlyCollection<Guid> gvfsExemptUsers = overrides.m_gvfsExemptUsers ?? this.m_gvfsExemptUsers;
      IReadOnlyCollection<GvfsAllowedVersionRange> gvfsAllowedVersionRanges = overrides.m_gvfsAllowedVersionRanges ?? this.m_gvfsAllowedVersionRanges;
      bool? detectRenameFalsePositivesByDefault = overrides.m_detectRenameFalsePositivesByDefault ?? this.m_detectRenameFalsePositivesByDefault;
      IReadOnlyCollection<string> userAgentExemptions = overrides.m_userAgentExemptions ?? this.m_userAgentExemptions;
      Version recommendedGitVersion = overrides.m_minimumRecommendedGitVersion;
      if ((object) recommendedGitVersion == null)
        recommendedGitVersion = this.m_minimumRecommendedGitVersion;
      string updateGitMessage = overrides.m_updateGitMessage ?? this.m_updateGitMessage;
      long? maxPushSize = overrides.m_maxPushSize ?? this.m_maxPushSize;
      long? maxPathLength = overrides.m_maxPathLength ?? this.m_maxPathLength;
      long? maxPathComponentLength = overrides.m_maxPathComponentLength ?? this.m_maxPathComponentLength;
      bool? forceCloneHack = overrides.m_forceCloneHack ?? this.m_forceCloneHack;
      bool? strictVoteMode = overrides.m_strictVoteMode ?? this.m_strictVoteMode;
      bool? inheritPullRequestCreationMode = overrides.m_inheritPullRequestCreationMode ?? this.m_inheritPullRequestCreationMode;
      bool? repoPullRequestAsDraftByDefault = overrides.m_repoPullRequestAsDraftByDefault ?? this.m_repoPullRequestAsDraftByDefault;
      return new GitRepoSettings(enforceConsistentCase, rejectDotGit, optimizedByDefault, breadcrumbDays, allowedForkTargets, gvfsOnly, gvfsExemptUsers, gvfsAllowedVersionRanges, detectRenameFalsePositivesByDefault, userAgentExemptions, recommendedGitVersion, updateGitMessage, maxPushSize, maxPathLength, maxPathComponentLength, forceCloneHack, strictVoteMode, inheritPullRequestCreationMode, repoPullRequestAsDraftByDefault);
    }

    internal GitRepoSettingsForPolicy ToPolicySettings(Guid repositoryId) => new GitRepoSettingsForPolicy(new GitPolicyRepositoryScope(new Guid?(repositoryId)), this.m_enforceConsistentCase, this.m_rejectDotGit, this.m_optimizedByDefault, this.m_breadcrumbDays, this.m_allowedForkTargets, this.m_gvfsOnly, this.m_gvfsExemptUsers, this.m_gvfsAllowedVersionRanges, this.m_detectRenameFalsePositivesByDefault, this.m_strictVoteMode, this.m_inheritPullRequestCreationMode, this.m_repoPullRequestAsDraftByDefault);
  }
}
