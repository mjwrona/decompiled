// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.ReadNuspecFromContentProviderHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.HttpStreams;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class ReadNuspecFromContentProviderHandler : 
    IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, byte[]>,
    IHaveInputType<PackageRequest<VssNuGetPackageIdentity>>,
    IHaveOutputType<byte[]>
  {
    private readonly IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry> metadataFetchingHandler;
    private readonly ITracerService tracerService;
    private readonly IAsyncHandler<BlobIdentifier, Stream> seekableBlobHttpStreamHandler;
    private readonly IAsyncHandler<string, byte[]> dropNameToNuspecHandler;

    public ReadNuspecFromContentProviderHandler(
      IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry> metadataFetchingHandler,
      ITracerService tracerService,
      IAsyncHandler<BlobIdentifier, Stream> seekableBlobHttpStreamHandler,
      IAsyncHandler<string, byte[]> dropNameToNuspecHandler)
    {
      this.metadataFetchingHandler = metadataFetchingHandler;
      this.tracerService = tracerService;
      this.seekableBlobHttpStreamHandler = seekableBlobHttpStreamHandler;
      this.dropNameToNuspecHandler = dropNameToNuspecHandler;
    }

    public async Task<byte[]> Handle(PackageRequest<VssNuGetPackageIdentity> request)
    {
      ReadNuspecFromContentProviderHandler sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        INuGetMetadataEntry getMetadataEntry = await sendInTheThisObject.metadataFetchingHandler.Handle(request);
        if (getMetadataEntry == null)
          throw ControllerExceptionHelper.PackageNotFound_LegacyNuGetSpecificType((IPackageIdentity) request.PackageId, request.Feed);
        switch (getMetadataEntry.PackageStorageId)
        {
          case BlobStorageId blobStorageId:
            int num;
            try
            {
              return await sendInTheThisObject.GetNuspecBytes(blobStorageId);
            }
            catch (HttpSeekableStreamRequestException ex)
            {
              num = 1;
            }
            if (num == 1)
            {
              tracer.TraceInfoAlways("Failed to read nuspec with HttpSeekableStreamRequestException : " + ex.Message + ", retrying.");
              return await sendInTheThisObject.GetNuspecBytes(blobStorageId);
            }
            goto label_12;
          case DropStorageId dropStorageId:
label_12:
            return await sendInTheThisObject.dropNameToNuspecHandler.Handle(dropStorageId.DropName);
          case UpstreamStorageId _:
            throw new UnexpectedUningestedUpstreamPackageException(request.Feed.Id, (IPackageIdentity) request.PackageId);
          default:
            throw new Exception("Unrecognized storage id");
        }
      }
    }

    private async Task<byte[]> GetNuspecBytes(BlobStorageId blobStorageId)
    {
      byte[] nuspecBytes;
      using (Stream nupkgStream = await this.seekableBlobHttpStreamHandler.Handle(blobStorageId.BlobId))
        nuspecBytes = NuGetNuspecUtils.GetNuspecBytes(nupkgStream);
      return nuspecBytes;
    }
  }
}
