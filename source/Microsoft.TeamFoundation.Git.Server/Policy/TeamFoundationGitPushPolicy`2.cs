// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.TeamFoundationGitPushPolicy`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Exceptions;
using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using Microsoft.TeamFoundation.Policy.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  public abstract class TeamFoundationGitPushPolicy<TSettings, TContext> : 
    TeamFoundationGitRepositoryPolicy<TSettings, TContext>,
    ITeamFoundationGitPushPolicy,
    ITeamFoundationGitRepositoryPolicy,
    ITeamFoundationPolicy
    where TSettings : TeamFoundationGitRepositoryPolicySettings
    where TContext : TeamFoundationPolicyEvaluationRecordContext
  {
    private IList<ITeamFoundationGitRepositoryPolicySettings> m_allScopeSettings;

    public Guid RepositoryId { get; set; }

    public virtual bool AggregateScopeSettings => false;

    public IList<ITeamFoundationGitRepositoryPolicySettings> AllScopeSettings
    {
      get
      {
        if (!this.AggregateScopeSettings)
          throw new AggregateScopeSettingsIsFalseException();
        return this.m_allScopeSettings;
      }
      set
      {
        if (!this.AggregateScopeSettings)
          throw new AggregateScopeSettingsIsFalseException();
        this.m_allScopeSettings = value;
      }
    }

    public int ScopeOrder
    {
      get
      {
        __Boxed<TSettings> settings = (object) this.Settings;
        List<Guid> source;
        if (settings == null)
        {
          source = (List<Guid>) null;
        }
        else
        {
          GitPolicyRepositoryScope scope = settings.Scope;
          if (scope == null)
          {
            source = (List<Guid>) null;
          }
          else
          {
            IReadOnlyList<GitPolicyRepositoryScopeItem> scopeItems = scope.ScopeItems;
            source = scopeItems != null ? scopeItems.Where<GitPolicyRepositoryScopeItem>((Func<GitPolicyRepositoryScopeItem, bool>) (x => x.RepositoryId.GetValueOrDefault() != new Guid())).Select<GitPolicyRepositoryScopeItem, Guid>((Func<GitPolicyRepositoryScopeItem, Guid>) (x => x.RepositoryId.Value)).ToList<Guid>() : (List<Guid>) null;
          }
        }
        return source.Any<Guid>() ? 1 : 0;
      }
    }
  }
}
