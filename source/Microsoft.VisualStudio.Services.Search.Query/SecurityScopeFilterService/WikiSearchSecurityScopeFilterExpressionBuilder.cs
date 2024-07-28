// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityScopeFilterService.WikiSearchSecurityScopeFilterExpressionBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityScopeFilterService
{
  public class WikiSearchSecurityScopeFilterExpressionBuilder : 
    ISearchSecurityScopeFilterExpressionBuilder
  {
    public IExpression GetScopeFilterExpression(
      IVssRequestContext requestContext,
      bool enableSecurityChecks,
      out bool noResultAccessible,
      ProjectInfo projectInfo)
    {
      IList<IExpression> source = (IList<IExpression>) new List<IExpression>();
      noResultAccessible = false;
      if (enableSecurityChecks)
      {
        bool allReposAccessible = false;
        IEnumerable<string> accessibleRepositories = this.GetUserAccessibleRepositories(requestContext, out allReposAccessible);
        if (!allReposAccessible)
          source.Add(this.GetRepositoriesScopeExpression(accessibleRepositories));
        noResultAccessible = !allReposAccessible && !accessibleRepositories.Any<string>();
      }
      if (projectInfo != null)
        source.Add(this.GetProjectScopeExpression(projectInfo));
      source.Add(this.GetCollectionScopeExpression(requestContext));
      string currentHostConfigValue = requestContext.GetCurrentHostConfigValue<string>("/Service/SearchShared/Settings/SoftDeletedProjectIds");
      if (!string.IsNullOrWhiteSpace(currentHostConfigValue))
        source.Add((IExpression) new NotExpression((IExpression) new TermsExpression("projectId", Operator.In, (IEnumerable<string>) ((IEnumerable<string>) currentHostConfigValue.Split(',')).Select<string, string>((Func<string, string>) (i => i.Trim())).Where<string>((Func<string, bool>) (i => !string.IsNullOrEmpty(i))).ToList<string>())));
      return source.Aggregate<IExpression>((Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new AndExpression(new IExpression[2]
      {
        current,
        filter
      })));
    }

    private IExpression GetRepositoriesScopeExpression(IEnumerable<string> accessibleRepos)
    {
      if (accessibleRepos == null)
        accessibleRepos = (IEnumerable<string>) new List<string>();
      return (IExpression) new TermsExpression("repositoryId", Operator.In, accessibleRepos);
    }

    private IExpression GetProjectScopeExpression(ProjectInfo projectInfo) => (IExpression) new TermExpression("projectId", Operator.Equals, projectInfo.Id.ToString().ToLowerInvariant());

    private IExpression GetCollectionScopeExpression(IVssRequestContext requestContext) => (IExpression) new TermExpression("collectionId", Operator.Equals, requestContext.GetCollectionID().ToString().ToLowerInvariant());

    private IEnumerable<string> GetUserAccessibleRepositories(
      IVssRequestContext requestContext,
      out bool allReposAccessible)
    {
      IEnumerable<GitRepositoryData> accessibleRepositories = requestContext.GetService<IWikiSecurityChecksService>().GetUserAccessibleRepositories(requestContext, out allReposAccessible);
      return accessibleRepositories == null ? (IEnumerable<string>) null : accessibleRepositories.Select<GitRepositoryData, string>((Func<GitRepositoryData, string>) (repo => repo.Id.ToString().ToLowerInvariant()));
    }
  }
}
