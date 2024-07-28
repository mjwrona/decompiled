// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlockCacheService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.BlockCache;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class BlockCacheService : IBlockCacheService, IVssFrameworkService
  {
    internal const string BlockCacheTypePath = "/Configuration/Caching/BlockCacheType";

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    private IBlockCacheService GetService(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      BlockCacheType blockCacheType = requestContext.GetService<IVssRegistryService>().GetValue<BlockCacheType>(requestContext, (RegistryQuery) "/Configuration/Caching/BlockCacheType", true, BlockCacheType.Local);
      switch (blockCacheType)
      {
        case BlockCacheType.None:
          return (IBlockCacheService) requestContext.GetService<NoBlockCacheService>();
        case BlockCacheType.Local:
          return (IBlockCacheService) requestContext.GetService<DeploymentLocalBlockCacheService>();
        case BlockCacheType.Redis:
          return (IBlockCacheService) requestContext.GetService<RedisBlockCacheService>();
        default:
          throw new NotImplementedException(string.Format("Unsupported BlockCacheType {0}.", (object) blockCacheType));
      }
    }

    public void SetBlockStatus(
      IVssRequestContext requestContext,
      Tuple<IDomainId, byte[]> key,
      BlockUploadStatus status)
    {
      this.GetService(requestContext).SetBlockStatus(requestContext, key, status);
    }

    public BlockUploadStatus GetBlockStatus(
      IVssRequestContext requestContext,
      Tuple<IDomainId, byte[]> key)
    {
      return this.GetService(requestContext).GetBlockStatus(requestContext, key);
    }
  }
}
