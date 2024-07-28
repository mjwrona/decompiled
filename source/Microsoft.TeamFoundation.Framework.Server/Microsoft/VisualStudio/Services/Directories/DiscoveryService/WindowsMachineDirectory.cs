// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.WindowsMachineDirectory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class WindowsMachineDirectory : IDirectory
  {
    public string Name => "wmd";

    public DirectoryInternalConvertKeysResponse ConvertKeys(
      IVssRequestContext context,
      DirectoryInternalConvertKeysRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return WindowsMachineDirectory.Execute<DirectoryInternalConvertKeysRequest, DirectoryInternalConvertKeysResponse>(WindowsMachineDirectory.\u003C\u003EO.\u003C0\u003E__ConvertKeys ?? (WindowsMachineDirectory.\u003C\u003EO.\u003C0\u003E__ConvertKeys = new Func<IVssRequestContext, DirectoryInternalConvertKeysRequest, DirectoryInternalConvertKeysResponse>(WindowsMachineDirectoryConvertKeysHelper.ConvertKeys)), context, request);
    }

    public DirectoryInternalGetAvatarsResponse GetAvatars(
      IVssRequestContext context,
      DirectoryInternalGetAvatarsRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return WindowsMachineDirectory.Execute<DirectoryInternalGetAvatarsRequest, DirectoryInternalGetAvatarsResponse>(WindowsMachineDirectory.\u003C\u003EO.\u003C1\u003E__GetAvatars ?? (WindowsMachineDirectory.\u003C\u003EO.\u003C1\u003E__GetAvatars = new Func<IVssRequestContext, DirectoryInternalGetAvatarsRequest, DirectoryInternalGetAvatarsResponse>(WindowsMachineDirectoryGetAvatarsHelper.GetAvatars)), context, request);
    }

    public DirectoryInternalGetEntitiesResponse GetEntities(
      IVssRequestContext context,
      DirectoryInternalGetEntitiesRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return WindowsMachineDirectory.Execute<DirectoryInternalGetEntitiesRequest, DirectoryInternalGetEntitiesResponse>(WindowsMachineDirectory.\u003C\u003EO.\u003C2\u003E__GetEntities ?? (WindowsMachineDirectory.\u003C\u003EO.\u003C2\u003E__GetEntities = new Func<IVssRequestContext, DirectoryInternalGetEntitiesRequest, DirectoryInternalGetEntitiesResponse>(WindowsMachineDirectoryGetEntitiesHelper.GetEntities)), context, request);
    }

    public DirectoryInternalGetRelatedEntitiesResponse GetRelatedEntities(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntitiesRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return WindowsMachineDirectory.Execute<DirectoryInternalGetRelatedEntitiesRequest, DirectoryInternalGetRelatedEntitiesResponse>(WindowsMachineDirectory.\u003C\u003EO.\u003C3\u003E__GetRelatedEntities ?? (WindowsMachineDirectory.\u003C\u003EO.\u003C3\u003E__GetRelatedEntities = new Func<IVssRequestContext, DirectoryInternalGetRelatedEntitiesRequest, DirectoryInternalGetRelatedEntitiesResponse>(WindowsMachineDirectoryGetRelatedEntitiesHelper.GetRelatedEntities)), context, request);
    }

    public DirectoryInternalGetRelatedEntityIdsResponse GetRelatedEntityIds(
      IVssRequestContext context,
      DirectoryInternalGetRelatedEntityIdsRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return WindowsMachineDirectory.Execute<DirectoryInternalGetRelatedEntityIdsRequest, DirectoryInternalGetRelatedEntityIdsResponse>(WindowsMachineDirectory.\u003C\u003EO.\u003C4\u003E__GetRelatedEntityIds ?? (WindowsMachineDirectory.\u003C\u003EO.\u003C4\u003E__GetRelatedEntityIds = new Func<IVssRequestContext, DirectoryInternalGetRelatedEntityIdsRequest, DirectoryInternalGetRelatedEntityIdsResponse>(WindowsMachineDirectoryGetRelatedEntityIdsHelper.GetRelatedEntityIds)), context, request);
    }

    public DirectoryInternalSearchResponse Search(
      IVssRequestContext context,
      DirectoryInternalSearchRequest request)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return WindowsMachineDirectory.Execute<DirectoryInternalSearchRequest, DirectoryInternalSearchResponse>(WindowsMachineDirectory.\u003C\u003EO.\u003C5\u003E__Search ?? (WindowsMachineDirectory.\u003C\u003EO.\u003C5\u003E__Search = new Func<IVssRequestContext, DirectoryInternalSearchRequest, DirectoryInternalSearchResponse>(WindowsMachineDirectorySearchHelper.Search)), context, request);
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
