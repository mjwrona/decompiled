// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.VisualStudioDirectorySearchHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class VisualStudioDirectorySearchHelper
  {
    internal static DirectoryInternalSearchResponse Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      IList<IDirectoryEntity> results = (IList<IDirectoryEntity>) null;
      string pagingToken = (string) null;
      if (VisualStudioDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
        VisualStudioDirectorySearchHelper.Search(context, request, out results, out pagingToken);
      return new DirectoryInternalSearchResponse()
      {
        Results = (IList<IDirectoryEntity>) ((object) results ?? (object) Array.Empty<IDirectoryEntity>()),
        PagingToken = pagingToken
      };
    }

    internal static IList<Microsoft.VisualStudio.Services.Identity.Identity> SearchVsdIdentities(
      IVssRequestContext context,
      string query,
      IEnumerable<string> typesToSearch,
      IEnumerable<string> filterByAncestorEntityIds,
      IEnumerable<string> filterByEntityIds,
      IEnumerable<string> propertiesToSearch,
      IEnumerable<string> propertiesToReturn,
      int maxResults,
      QueryType queryType,
      out string pagingToken,
      Guid scopeId = default (Guid))
    {
      pagingToken = (string) null;
      IdentitySearchType searchType = VisualStudioDirectorySearchHelper.GetSearchType(typesToSearch);
      if (searchType == IdentitySearchType.None)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      IEnumerable<string> propertyNameFilters = VisualStudioDirectorySearchHelper.GetPropertyNameFilters(propertiesToReturn);
      IdentityService service = context.GetService<IdentityService>();
      if (queryType == QueryType.LookUp)
      {
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(context, IdentitySearchFilter.AccountName, query, QueryMembership.None, propertyNameFilters, ReadIdentitiesOptions.FilterIllegalMemberships);
        return identityList == null || identityList.Count == 0 ? (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null : identityList;
      }
      IdentitySearchKind searchKind = VisualStudioDirectorySearchHelper.GetSearchKind(context, query, propertiesToSearch);
      if (searchKind == IdentitySearchKind.None)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      IEnumerable<IdentityDescriptor> identityDescriptors1 = VisualStudioDirectoryHelper.GetIdentityDescriptors(context, filterByAncestorEntityIds, nameof (filterByAncestorEntityIds));
      IEnumerable<IdentityDescriptor> identityDescriptors2 = VisualStudioDirectoryHelper.GetIdentityDescriptors(context, filterByEntityIds, nameof (filterByEntityIds));
      IdentitySearchResult identitySearchResult = service.SearchIdentities(context, new IdentitySearchParameters(query, searchKind, searchType, propertyNameFilters, maxResults, (string) null, identityDescriptors1, identityDescriptors2)
      {
        ScopeId = scopeId
      });
      if (identitySearchResult?.Identities == null || identitySearchResult.Identities.Count == 0)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = identitySearchResult.Identities;
      pagingToken = identitySearchResult.PagingContext;
      return VisualStudioDirectorySearchHelper.FilterActiveIdentities(identities);
    }

    private static void Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request,
      out IList<IDirectoryEntity> results,
      out string pagingToken)
    {
      results = (IList<IDirectoryEntity>) null;
      pagingToken = (string) null;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = VisualStudioDirectorySearchHelper.SearchVsdIdentities(context, request.Query, request.TypesToSearch, request.FilterByAncestorEntityIds, request.FilterByEntityIds, request.PropertiesToSearch, request.PropertiesToReturn, request.MaxResults, request.QueryType, out pagingToken, request.ScopeId);
      if (identityList == null)
        return;
      results = VisualStudioDirectoryEntityConverter.ConvertIdentities(context, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList, request.PropertiesToReturn);
    }

    private static IList<Microsoft.VisualStudio.Services.Identity.Identity> FilterActiveIdentities(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null && x.IsActive)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private static IdentitySearchKind GetSearchKind(
      IVssRequestContext context,
      string query,
      IEnumerable<string> propertiesToSearch)
    {
      if (!string.IsNullOrWhiteSpace(query) && !context.ExecutionEnvironment.IsHostedDeployment && query.Contains<char>('\\') && !VisualStudioDirectorySearchHelper.IsScopedGroupPrefix(query))
        return IdentitySearchKind.DomainAccountNamePrefix;
      IdentitySearchKind searchKind = IdentitySearchKind.None;
      foreach (string str in propertiesToSearch)
      {
        switch (str)
        {
          case "DisplayName":
            searchKind |= IdentitySearchKind.DisplayNamePrefix;
            continue;
          case "Mail":
            searchKind |= IdentitySearchKind.EmailPrefix;
            continue;
          case "MailNickname":
          case "SignInAddress":
            searchKind |= IdentitySearchKind.AccountNamePrefix;
            continue;
          case "AppId":
            searchKind |= IdentitySearchKind.AppId;
            continue;
          default:
            continue;
        }
      }
      return searchKind;
    }

    private static IdentitySearchType GetSearchType(IEnumerable<string> typesToSearch)
    {
      IdentitySearchType searchType = IdentitySearchType.None;
      foreach (string str in typesToSearch)
      {
        switch (str)
        {
          case "User":
            searchType |= IdentitySearchType.User;
            continue;
          case "Group":
            searchType |= IdentitySearchType.Group;
            continue;
          case "ServicePrincipal":
            searchType |= IdentitySearchType.ServicePrincipal;
            continue;
          default:
            continue;
        }
      }
      return searchType;
    }

    private static IEnumerable<string> GetPropertyNameFilters(IEnumerable<string> propertiesToReturn) => (IEnumerable<string>) Array.Empty<string>();

    private static bool IsScopedGroupPrefix(string query)
    {
      string str = query.Split('\\')[0];
      return !string.IsNullOrEmpty(str) && str[0] == '[' && str[str.Length - 1] == ']';
    }
  }
}
