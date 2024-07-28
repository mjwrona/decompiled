// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.IDirectory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  [InheritedExport]
  internal interface IDirectory
  {
    string Name { get; }

    DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request);

    DirectoryInternalGetAvatarsResponse GetAvatars(
      IVssRequestContext context,
      DirectoryInternalGetAvatarsRequest request);

    DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request);

    DirectoryInternalGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request);

    DirectoryInternalGetRelatedEntityIdsResponse GetRelatedEntityIds(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntityIdsRequest request);

    DirectoryInternalSearchResponse Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request);
  }
}
