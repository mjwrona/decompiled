// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuSpecStreamingHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuSpecStreamingHandler : 
    IAsyncHandler<
    #nullable disable
    IPackageFileRequest<VssNuGetPackageIdentity>, HttpResponseMessage>,
    IHaveInputType<IPackageFileRequest<VssNuGetPackageIdentity>>,
    IHaveOutputType<HttpResponseMessage>
  {
    private readonly IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService;
    private readonly ICache<string, object> requestContextItemsCache;

    public NuSpecStreamingHandler(
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService,
      ICache<string, object> requestContextItemsCache)
    {
      this.metadataService = metadataService;
      this.requestContextItemsCache = requestContextItemsCache;
    }

    public async Task<HttpResponseMessage> Handle(
      IPackageFileRequest<VssNuGetPackageIdentity> request)
    {
      if (!request.FilePath.EndsWith(".nuspec"))
        return (HttpResponseMessage) null;
      string expectedFile = request.PackageId.Name.NormalizedName + ".nuspec";
      if (!request.FilePath.Equals(expectedFile, StringComparison.OrdinalIgnoreCase))
        throw ControllerExceptionHelper.PackageSubresourceNotFound(request.FilePath, request.PackageId, expectedFile);
      INuGetMetadataEntry versionStateAsync = await this.metadataService.GetPackageVersionStateAsync((IPackageRequest<VssNuGetPackageIdentity>) request);
      this.requestContextItemsCache.AddNuspecDownloadTelemetry((IMetadataEntry) versionStateAsync);
      VssServerStreamContent serverStreamContent = new VssServerStreamContent((Stream) new MemoryStream(versionStateAsync.AreBytesCompressed ? CompressionHelper.InflateByteArray(versionStateAsync.NuspecBytes) : versionStateAsync.NuspecBytes), (object) FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(request.Feed));
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) serverStreamContent
      };
      httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
      return httpResponseMessage;
    }
  }
}
