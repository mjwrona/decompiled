// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.RepoSync.SearchGitRepoSettingsProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.RepoSync
{
  internal class SearchGitRepoSettingsProvider
  {
    public static readonly Guid SearchGitRepoPolicyTypeId = new Guid("0517F88D-4EC5-4343-9D26-9930EBD53069");
    private Guid m_projectId;
    private Dictionary<Guid, SearchGitRepoSettingsForPolicy> m_repoIdToRepoSettingsMap;

    internal SearchGitRepoSettingsProvider()
    {
    }

    internal SearchGitRepoSettingsProvider(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid repoId)
    {
      this.m_projectId = projectId;
      this.InitializeRepoPolicies(requestContext.GetClient<PolicyHttpClient>().GetPolicyConfigurationsAsync(projectId, repoId.ToString("N"), new Guid?(), (object) null, new CancellationToken()).Result);
    }

    internal void InitializeRepoPolicies(List<PolicyConfiguration> policyConfigurations)
    {
      this.m_repoIdToRepoSettingsMap = new Dictionary<Guid, SearchGitRepoSettingsForPolicy>();
      if (policyConfigurations == null)
        return;
      foreach (PolicyConfiguration policyConfiguration in policyConfigurations)
      {
        if (!(policyConfiguration.Type.Id != SearchGitRepoSettingsProvider.SearchGitRepoPolicyTypeId))
        {
          SearchGitRepoSettingsForPolicy settingsForPolicy = policyConfiguration.Settings.ToObject<SearchGitRepoSettingsForPolicy>();
          string[] array = settingsForPolicy.Scope.Select<GitRepositoryScopeItem, string>((Func<GitRepositoryScopeItem, string>) (x => x.RepositoryId)).ToArray<string>();
          if (array.Length != 1)
            throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("SearchGitRepoSettingsForPolicy found multiple Repo Scopes, ProjectId {0}.", (object) this.m_projectId)));
          this.m_repoIdToRepoSettingsMap[new Guid(array[0])] = settingsForPolicy;
        }
      }
    }

    internal SearchGitRepoSettingsForPolicy GetRepoSetting(Guid repositoryId)
    {
      SearchGitRepoSettingsForPolicy repoSetting;
      this.m_repoIdToRepoSettingsMap.TryGetValue(repositoryId, out repoSetting);
      return repoSetting;
    }
  }
}
