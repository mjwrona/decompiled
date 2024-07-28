// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.ActiveDirectory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class ActiveDirectory : IDirectory
  {
    public string Name => "ad";

    public virtual DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ActiveDirectory.Execute<DirectoryInternalConvertKeysRequest, DirectoryInternalConvertKeysResponse>(ActiveDirectory.\u003C\u003EO.\u003C0\u003E__ConvertKeys ?? (ActiveDirectory.\u003C\u003EO.\u003C0\u003E__ConvertKeys = new Func<IVssRequestContext, DirectoryInternalConvertKeysRequest, DirectoryInternalConvertKeysResponse>(ActiveDirectoryConvertKeysHelper.ConvertKeys)), context, request);
    }

    public virtual DirectoryInternalGetAvatarsResponse GetAvatars(
      IVssRequestContext context,
      DirectoryInternalGetAvatarsRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ActiveDirectory.Execute<DirectoryInternalGetAvatarsRequest, DirectoryInternalGetAvatarsResponse>(ActiveDirectory.\u003C\u003EO.\u003C1\u003E__GetAvatars ?? (ActiveDirectory.\u003C\u003EO.\u003C1\u003E__GetAvatars = new Func<IVssRequestContext, DirectoryInternalGetAvatarsRequest, DirectoryInternalGetAvatarsResponse>(ActiveDirectoryGetAvatarsHelper.GetAvatars)), context, request);
    }

    public virtual DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ActiveDirectory.Execute<DirectoryInternalGetEntitiesRequest, DirectoryInternalGetEntitiesResponse>(ActiveDirectory.\u003C\u003EO.\u003C2\u003E__GetEntities ?? (ActiveDirectory.\u003C\u003EO.\u003C2\u003E__GetEntities = new Func<IVssRequestContext, DirectoryInternalGetEntitiesRequest, DirectoryInternalGetEntitiesResponse>(ActiveDirectoryGetEntitiesHelper.GetEntities)), context, request);
    }

    public virtual DirectoryInternalGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ActiveDirectory.Execute<DirectoryInternalGetRelatedEntitiesRequest, DirectoryInternalGetRelatedEntitiesResponse>(ActiveDirectory.\u003C\u003EO.\u003C3\u003E__GetRelatedEntities ?? (ActiveDirectory.\u003C\u003EO.\u003C3\u003E__GetRelatedEntities = new Func<IVssRequestContext, DirectoryInternalGetRelatedEntitiesRequest, DirectoryInternalGetRelatedEntitiesResponse>(ActiveDirectoryGetRelatedEntitiesHelper.GetRelatedEntities)), context, request);
    }

    public virtual DirectoryInternalGetRelatedEntityIdsResponse GetRelatedEntityIds(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntityIdsRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ActiveDirectory.Execute<DirectoryInternalGetRelatedEntityIdsRequest, DirectoryInternalGetRelatedEntityIdsResponse>(ActiveDirectory.\u003C\u003EO.\u003C4\u003E__GetRelatedEntityIds ?? (ActiveDirectory.\u003C\u003EO.\u003C4\u003E__GetRelatedEntityIds = new Func<IVssRequestContext, DirectoryInternalGetRelatedEntityIdsRequest, DirectoryInternalGetRelatedEntityIdsResponse>(ActiveDirectoryGetRelatedEntityIdsHelper.GetRelatedEntityIds)), context, request);
    }

    public virtual DirectoryInternalSearchResponse Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ActiveDirectory.Execute<DirectoryInternalSearchRequest, DirectoryInternalSearchResponse>(ActiveDirectory.\u003C\u003EO.\u003C5\u003E__Search ?? (ActiveDirectory.\u003C\u003EO.\u003C5\u003E__Search = new Func<IVssRequestContext, DirectoryInternalSearchRequest, DirectoryInternalSearchResponse>(ActiveDirectorySearchHelper.Search)), context, request);
    }

    private static TResponse Execute<TRequest, TResponse>(
      Func<IVssRequestContext, TRequest, TResponse> function,
      IVssRequestContext context,
      TRequest request)
      where TRequest : DirectoryInternalRequest
      where TResponse : DirectoryInternalResponse
    {
      try
      {
        return function(context, request);
      }
      catch (Exception ex)
      {
        throw new DirectoryDiscoveryServiceException(ex.Message, ex);
      }
    }
  }
}
