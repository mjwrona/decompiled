// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.ActiveDirectorySearchHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class ActiveDirectorySearchHelper
  {
    internal static DirectoryInternalSearchResponse Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      DirectoryInternalSearchResponse internalSearchResponse = new DirectoryInternalSearchResponse()
      {
        Results = (IList<IDirectoryEntity>) Array.Empty<IDirectoryEntity>(),
        PagingToken = (string) null
      };
      if (!ActiveDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request) || string.IsNullOrWhiteSpace(request.Query) || request.PropertiesToSearch.IsNullOrEmpty<string>() || request.TypesToSearch.IsNullOrEmpty<string>())
        return internalSearchResponse;
      Tuple<string, string> tuple = ActiveDirectorySearchHelper.RetrieveDomainNameAndPrefix(request.Query);
      string domainName = tuple.Item1;
      string query = ActiveDirectoryHelper.SanitizeStringForLdap(tuple.Item2);
      string searchQuery = ActiveDirectorySearchHelper.ConstructPropertiesToSearchQuery(request.PropertiesToSearch, query);
      if (string.IsNullOrWhiteSpace(searchQuery))
        return internalSearchResponse;
      string ldapQueryFormat = ActiveDirectorySearchHelper.GetLdapQueryFormat(context, request.TypesToSearch);
      if (string.IsNullOrWhiteSpace(ldapQueryFormat))
        return internalSearchResponse;
      string filter = string.Format(ldapQueryFormat, (object) searchQuery);
      internalSearchResponse.Results = ActiveDirectoryHelper.Instance.SearchAd(context, filter, request.MaxResults, request.PropertiesToReturn, domainName);
      return internalSearchResponse;
    }

    internal static Tuple<string, string> RetrieveDomainNameAndPrefix(string query)
    {
      if (string.IsNullOrWhiteSpace(query))
        return new Tuple<string, string>(string.Empty, string.Empty);
      int length = query.IndexOf('\\', 0);
      if (length == -1)
        return new Tuple<string, string>(string.Empty, query);
      string str1 = query.Substring(0, length);
      if (length == query.Length - 1)
        return new Tuple<string, string>(str1, string.Empty);
      string str2 = query.Substring(length + 1, query.Length - (length + 1));
      return new Tuple<string, string>(str1, str2);
    }

    private static string ConstructPropertiesToSearchQuery(
      IEnumerable<string> propertiesToSearch,
      string query)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string key in propertiesToSearch)
      {
        string str;
        if (ActiveDirectoryProperty.PropertiesToSearchMap.TryGetValue(key, out str))
          stringBuilder.AppendFormat("({0}={1}*)", (object) str, (object) query);
      }
      return stringBuilder.ToString();
    }

    private static string GetLdapQueryFormat(
      IVssRequestContext context,
      IEnumerable<string> typesToSearch)
    {
      ActiveDirectorySettingsService service = context.To(TeamFoundationHostType.Deployment).GetService<ActiveDirectorySettingsService>();
      bool flag1 = false;
      bool flag2 = false;
      string ldapQueryFormat = string.Empty;
      foreach (string str in typesToSearch)
      {
        switch (str)
        {
          case "User":
            ldapQueryFormat = service.LdapUserSearchQueryFormat;
            flag1 = true;
            continue;
          case "Group":
            ldapQueryFormat = service.LdapSecurityGroupSearchQueryFormat;
            flag2 = true;
            continue;
          case "Any":
            flag1 = flag2 = true;
            continue;
          default:
            continue;
        }
      }
      if (flag1 & flag2)
        ldapQueryFormat = service.LdapSecurityGroupOrUserSearchStringFormat;
      return ldapQueryFormat;
    }
  }
}
