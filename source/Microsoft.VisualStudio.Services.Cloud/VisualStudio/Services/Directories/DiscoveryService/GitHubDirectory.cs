// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.GitHubDirectory
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class GitHubDirectory : IDirectory
  {
    public string Name => "ghb";

    public DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request)
    {
      return GitHubDirectoryConvertKeysHelper.ConvertKeys(context, request);
    }

    public virtual DirectoryInternalGetAvatarsResponse GetAvatars(
      IVssRequestContext context,
      DirectoryInternalGetAvatarsRequest request)
    {
      return GitHubDirectoryGetAvatarsHelper.GetAvatars(context, request);
    }

    public DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request)
    {
      return GitHubDirectoryGetEntitiesHelper.GetEntities(context, request);
    }

    public DirectoryInternalGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request)
    {
      return GitHubDirectoryGetRelatedEntitiesHelper.GetRelatedEntities(context, request);
    }

    public DirectoryInternalGetRelatedEntityIdsResponse GetRelatedEntityIds(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntityIdsRequest request)
    {
      return GitHubDirectoryGetRelatedEntityIdsHelper.GetRelatedEntityIds(context, request);
    }

    public virtual DirectoryInternalSearchResponse Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      return GitHubDirectorySearchHelper.Search(context, request);
    }
  }
}
