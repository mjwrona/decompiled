// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRepoSettingsForPolicy
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using Microsoft.TeamFoundation.Git.Server.Policy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DataContract]
  internal class GitRepoSettingsForPolicy : 
    GitRepoSettings,
    ITeamFoundationGitRepositoryPolicySettings,
    ITeamFoundationGitPolicySettings
  {
    public static readonly Guid PolicyTypeId = new Guid("7ED39669-655C-494E-B4A0-A08B4DA0FCCE");

    [JsonConstructor]
    internal GitRepoSettingsForPolicy(
      GitPolicyRepositoryScope scope,
      bool? enforceConsistentCase,
      bool? rejectDotGit,
      bool? optimizedByDefault,
      int? breadcrumbDays,
      Microsoft.TeamFoundation.Git.Server.AllowedForkTargets? allowedForkTargets,
      bool? gvfsOnly,
      IReadOnlyCollection<Guid> gvfsExemptUsers,
      IReadOnlyCollection<GvfsAllowedVersionRange> gvfsAllowedVersionRanges,
      bool? detectRenameFalsePositivesByDefault,
      bool? strictVoteMode,
      bool? inheritPullRequestCreationMode,
      bool? repoPullRequestAsDraftByDefault)
    {
      bool? enforceConsistentCase1 = enforceConsistentCase;
      bool? rejectDotGit1 = rejectDotGit;
      bool? optimizedByDefault1 = optimizedByDefault;
      int? breadcrumbDays1 = breadcrumbDays;
      Microsoft.TeamFoundation.Git.Server.AllowedForkTargets? allowedForkTargets1 = allowedForkTargets;
      bool? gvfsOnly1 = gvfsOnly;
      IReadOnlyCollection<Guid> gvfsExemptUsers1 = gvfsExemptUsers;
      IReadOnlyCollection<GvfsAllowedVersionRange> gvfsAllowedVersionRanges1 = gvfsAllowedVersionRanges;
      bool? detectRenameFalsePositivesByDefault1 = detectRenameFalsePositivesByDefault;
      bool? nullable1 = strictVoteMode;
      bool? nullable2 = inheritPullRequestCreationMode;
      bool? nullable3 = repoPullRequestAsDraftByDefault;
      long? maxPushSize = new long?();
      long? maxPathLength = new long?();
      long? maxPathComponentLength = new long?();
      bool? forceCloneHack = new bool?();
      bool? strictVoteMode1 = nullable1;
      bool? inheritPullRequestCreationMode1 = nullable2;
      bool? repoPullRequestAsDraftByDefault1 = nullable3;
      // ISSUE: explicit constructor call
      base.\u002Ector(enforceConsistentCase1, rejectDotGit1, optimizedByDefault1, breadcrumbDays1, allowedForkTargets1, gvfsOnly1, gvfsExemptUsers1, gvfsAllowedVersionRanges1, detectRenameFalsePositivesByDefault1, maxPushSize: maxPushSize, maxPathLength: maxPathLength, maxPathComponentLength: maxPathComponentLength, forceCloneHack: forceCloneHack, strictVoteMode: strictVoteMode1, inheritPullRequestCreationMode: inheritPullRequestCreationMode1, repoPullRequestAsDraftByDefault: repoPullRequestAsDraftByDefault1);
      this.Scope = scope;
    }

    [DataMember(IsRequired = true)]
    public GitPolicyRepositoryScope Scope { get; set; }

    GitScope ITeamFoundationGitPolicySettings.Scope => (GitScope) this.Scope;

    [DataMember]
    public bool? EnforceConsistentCase => this.m_enforceConsistentCase;

    [DataMember]
    public bool? RejectDotGit => this.m_rejectDotGit;

    [DataMember]
    public bool? OptimizedByDefault => this.m_optimizedByDefault;

    [DataMember]
    public int? BreadcrumbDays => this.m_breadcrumbDays;

    [DataMember]
    public Microsoft.TeamFoundation.Git.Server.AllowedForkTargets? AllowedForkTargets => this.m_allowedForkTargets;

    [DataMember]
    public bool? GvfsOnly => this.m_gvfsOnly;

    [DataMember]
    public new IEnumerable<Guid> GvfsExemptUsers => (IEnumerable<Guid>) this.m_gvfsExemptUsers;

    [DataMember]
    public new IReadOnlyCollection<GvfsAllowedVersionRange> GvfsAllowedVersionRanges => this.m_gvfsAllowedVersionRanges;

    [DataMember]
    public bool? DetectRenameFalsePositivesByDefault => this.m_detectRenameFalsePositivesByDefault;

    [DataMember]
    public bool? StrictVoteMode => this.m_strictVoteMode;

    [DataMember]
    public bool? InheritPullRequestCreationMode => this.m_inheritPullRequestCreationMode;

    [DataMember]
    public bool? RepoPullRequestAsDraftByDefault => this.m_repoPullRequestAsDraftByDefault;
  }
}
