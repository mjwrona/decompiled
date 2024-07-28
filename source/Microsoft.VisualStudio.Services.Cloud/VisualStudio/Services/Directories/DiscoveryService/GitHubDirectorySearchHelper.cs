// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.GitHubDirectorySearchHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class GitHubDirectorySearchHelper
  {
    internal static DirectoryInternalSearchResponse Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      DirectoryInternalSearchResponse internalSearchResponse = new DirectoryInternalSearchResponse()
      {
        Results = (IList<IDirectoryEntity>) Array.Empty<IDirectoryEntity>()
      };
      if (GitHubDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
      {
        internalSearchResponse.Results = (IList<IDirectoryEntity>) GitHubDirectorySearchHelper.SearchUsers(context, request).ToList<IDirectoryEntity>();
        internalSearchResponse.PagingToken = (string) null;
      }
      return internalSearchResponse;
    }

    private static SortedSet<IDirectoryEntity> SearchUsers(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      GitHubData.V3.User[] userArray = GitHubDirectoryHelper.Instance.SearchUsers(context, request.Query, request.MaxResults);
      SortedSet<IDirectoryEntity> sortedSet = new SortedSet<IDirectoryEntity>(DirectoryEntityComparer.DefaultComparer);
      if (userArray != null)
      {
        foreach (GitHubData.V3.User user in userArray)
          sortedSet.Add((IDirectoryEntity) GitHubDirectoryEntityConverter.ConvertUser(context, user, request.PropertiesToReturn));
      }
      return sortedSet;
    }
  }
}
