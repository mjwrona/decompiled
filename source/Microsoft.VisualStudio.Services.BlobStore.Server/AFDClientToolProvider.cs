// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.AFDClientToolProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.AzureStorage;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  internal class AFDClientToolProvider : AzureBlobClientToolProvider
  {
    private readonly IBlobStoreBlobEdgeCachingService edgeService;

    public AFDClientToolProvider(IVssRequestContext systemRequestContext, string storageAccountName)
      : base(systemRequestContext, storageAccountName)
    {
      this.edgeService = systemRequestContext.GetService<IBlobStoreBlobEdgeCachingService>();
    }

    protected override PreauthenticatedUri? ConvertOriginalUri(
      IVssRequestContext requestContext,
      PreauthenticatedUri uri,
      EdgeCache edgeCache)
    {
      if (edgeCache != EdgeCache.Allowed || !requestContext.IsFeatureEnabled("BlobStore.Features.ClientToolAzureFrontDoor"))
        return base.ConvertOriginalUri(requestContext, uri, edgeCache);
      uri = new PreauthenticatedUri(this.edgeService.GetEdgeUri(uri.NotNullUri, uri.ExpiryTime.UtcDateTime - BlobEdgeCachingService.AzureFrontDoorSasUriExpiryBuffer), EdgeType.AzureFrontDoor);
      return new PreauthenticatedUri?(uri);
    }
  }
}
