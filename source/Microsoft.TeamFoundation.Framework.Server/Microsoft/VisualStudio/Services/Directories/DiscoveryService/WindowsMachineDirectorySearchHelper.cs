// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.WindowsMachineDirectorySearchHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class WindowsMachineDirectorySearchHelper
  {
    private const string MissingValueTraceStringFormat = "Missing values for property:{0} in DirectoryInternalSearchRequest";

    internal static DirectoryInternalSearchResponse Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      DirectoryInternalSearchResponse internalSearchResponse = new DirectoryInternalSearchResponse()
      {
        Results = (IList<IDirectoryEntity>) Array.Empty<IDirectoryEntity>(),
        PagingToken = (string) null
      };
      if (!WindowsMachineDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
        return internalSearchResponse;
      if (string.IsNullOrWhiteSpace(request.Query))
      {
        context.Trace(11445010, TraceLevel.Warning, "DirectoryDiscovery", "WindowsMachineDirectory", "Missing values for property:{0} in DirectoryInternalSearchRequest", (object) "Query");
        return internalSearchResponse;
      }
      if (request.PropertiesToSearch.IsNullOrEmpty<string>())
      {
        context.Trace(11445012, TraceLevel.Warning, "DirectoryDiscovery", "WindowsMachineDirectory", "Missing values for property:{0} in DirectoryInternalSearchRequest", (object) "PropertiesToSearch");
        return internalSearchResponse;
      }
      if (request.TypesToSearch.IsNullOrEmpty<string>())
      {
        context.Trace(11445014, TraceLevel.Warning, "DirectoryDiscovery", "WindowsMachineDirectory", "Missing values for property:{0} in DirectoryInternalSearchRequest", (object) "TypesToSearch");
        return internalSearchResponse;
      }
      string query = request.Query;
      Tuple<string, string> tuple = WindowsMachineDirectoryHelper.RetrieveDomainNameAndPrefix(request.Query);
      string domainName = tuple.Item1;
      if (!string.IsNullOrWhiteSpace(domainName))
      {
        if (!WindowsMachineDirectoryHelper.IsLocalMachine(domainName))
          return internalSearchResponse;
        query = tuple.Item2;
      }
      HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer);
      foreach (string key in request.PropertiesToSearch)
      {
        string str;
        if (WindowsMachineDirectoryProperty.PropertiesToSearchMap.TryGetValue(key, out str))
          stringSet1.Add(str);
      }
      if (!stringSet1.Any<string>())
        return internalSearchResponse;
      HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.WMDSchemaClassName);
      foreach (string str in request.TypesToSearch)
      {
        switch (str)
        {
          case "User":
            stringSet2.Add("user");
            continue;
          case "Group":
            stringSet2.Add("group");
            continue;
          case "Any":
            stringSet2.Add("user");
            stringSet2.Add("group");
            continue;
          default:
            continue;
        }
      }
      if (!stringSet2.Any<string>())
        return internalSearchResponse;
      internalSearchResponse.Results = WindowsMachineDirectoryHelper.Instance.Search(context, query, request.MaxResults, stringSet2, stringSet1, request.PropertiesToReturn);
      return internalSearchResponse;
    }
  }
}
