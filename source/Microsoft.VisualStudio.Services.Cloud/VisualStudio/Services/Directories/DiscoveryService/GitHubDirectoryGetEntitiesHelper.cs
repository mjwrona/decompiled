// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.GitHubDirectoryGetEntitiesHelper
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
  internal static class GitHubDirectoryGetEntitiesHelper
  {
    internal static DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request)
    {
      if (GitHubDirectoryRequestFilter.AllowRequest(context, (DirectoryInternalRequest) request))
        return GitHubDirectoryGetEntitiesHelper.ResolveGitHubUserIds(context, request.PropertiesToReturn, request.EntityIds);
      return new DirectoryInternalGetEntitiesResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) request.EntityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (id => id), (Func<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) (id => (DirectoryInternalGetEntityResult) null), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer)
      };
    }

    private static DirectoryInternalGetEntitiesResponse ResolveGitHubUserIds(
      IVssRequestContext context,
      IEnumerable<string> propertiesToReturn,
      IEnumerable<DirectoryEntityIdentifier> entityIds)
    {
      return new DirectoryInternalGetEntitiesResponse()
      {
        Results = (IDictionary<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) entityIds.ToDictionary<DirectoryEntityIdentifier, DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>((Func<DirectoryEntityIdentifier, DirectoryEntityIdentifier>) (entityId => entityId), (Func<DirectoryEntityIdentifier, DirectoryInternalGetEntityResult>) (entityId =>
        {
          string gitHubUserId;
          if (!GitHubDirectoryEntityIdentifierHelper.TryParseUserId(entityId, out gitHubUserId))
            return (DirectoryInternalGetEntityResult) null;
          GitHubData.V3.User userById = GitHubDirectoryHelper.Instance.GetUserById(context, gitHubUserId);
          return new DirectoryInternalGetEntityResult()
          {
            Entity = (IDirectoryEntity) GitHubDirectoryEntityConverter.ConvertUser(context, userById, propertiesToReturn)
          };
        }), (IEqualityComparer<DirectoryEntityIdentifier>) DirectoryEntityIdentifier.Comparer)
      };
    }
  }
}
