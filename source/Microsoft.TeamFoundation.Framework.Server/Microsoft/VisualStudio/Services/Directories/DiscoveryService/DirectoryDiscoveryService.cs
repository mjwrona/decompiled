// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryDiscoveryService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  [DefaultServiceImplementation(typeof (FrameworkDirectoryDiscoveryService))]
  public abstract class DirectoryDiscoveryService : IVssFrameworkService
  {
    public abstract void ServiceEnd(IVssRequestContext context);

    public abstract void ServiceStart(IVssRequestContext context);

    public abstract DirectoryConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryConvertKeysRequest request);

    public abstract DirectoryGetAvatarsResponse GetAvatars(
      IVssRequestContext context,
      DirectoryGetAvatarsRequest request);

    public abstract DirectoryGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryGetEntitiesRequest request);

    public abstract DirectoryGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryGetRelatedEntitiesRequest request);

    public abstract DirectoryGetRelatedEntityIdsResponse GetRelatedEntityIds(
      IVssRequestContext context,
      DirectoryGetRelatedEntityIdsRequest request);

    public abstract DirectorySearchResponse Search(
      IVssRequestContext context,
      DirectorySearchRequest request);

    internal abstract DirectoryGetEntitiesResponse GetEntitiesInternal(
      IVssRequestContext context,
      DirectoryGetEntitiesInternalRequest request);
  }
}
