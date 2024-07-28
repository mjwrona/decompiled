// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService.ICodeSecurityChecksService
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService
{
  [DefaultServiceImplementation(typeof (CodeSecurityChecksService))]
  public interface ICodeSecurityChecksService : ISecurityChecksService, IVssFrameworkService
  {
    IEnumerable<GitRepositoryData> GetUserAccessibleRepositories(
      IVssRequestContext requestContext,
      out bool allReposAreAccessible);

    IEnumerable<GitRepositoryData> GetUserAccessibleRepositories(
      IVssRequestContext requestContext,
      string projectIdentifier);

    IEnumerable<string> GetUserAccessibleProjects(IVssRequestContext requestContext);

    IEnumerable<CodeResult> GetUserAccessibleTfvcFiles(
      IVssRequestContext userRequestContext,
      IEnumerable<CodeResult> results);

    IEnumerable<FilterCategory> GetUserAccessibleFacets(
      IVssRequestContext userRequestContext,
      IEnumerable<FilterCategory> allFacets);

    IEnumerable<GitRepositoryData> GetUserAccessibleRepositoriesScopedToSearchQuery(
      IVssRequestContext userRequestContext,
      EntitySearchQuery searchQuery,
      out bool allReposAreAccessible);

    IEnumerable<string> GetUserAccessibleCustomProjects(IVssRequestContext requestContext);

    bool GitRepoHasReadPermission(IVssRequestContext requestContext, GitRepositoryData repo);
  }
}
