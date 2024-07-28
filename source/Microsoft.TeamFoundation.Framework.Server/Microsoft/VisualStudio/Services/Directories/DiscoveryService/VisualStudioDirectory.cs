// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.VisualStudioDirectory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class VisualStudioDirectory : IDirectory
  {
    public string Name => "vsd";

    public DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request)
    {
      return VisualStudioDirectoryConvertKeysHelper.ConvertKeys(context, request);
    }

    public virtual DirectoryInternalGetAvatarsResponse GetAvatars(
      IVssRequestContext context,
      DirectoryInternalGetAvatarsRequest request)
    {
      return VisualStudioDirectoryGetAvatarsHelper.GetAvatars(context, request);
    }

    public DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request)
    {
      return VisualStudioDirectoryGetEntitiesHelper.GetEntities(context, request);
    }

    public DirectoryInternalGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request)
    {
      return VisualStudioDirectoryGetRelatedEntitiesHelper.GetRelatedEntities(context, request);
    }

    public DirectoryInternalGetRelatedEntityIdsResponse GetRelatedEntityIds(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntityIdsRequest request)
    {
      return VisualStudioDirectoryGetRelatedEntityIdsHelper.GetRelatedEntityIds(context, request);
    }

    public virtual DirectoryInternalSearchResponse Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      return VisualStudioDirectorySearchHelper.Search(context, request);
    }
  }
}
